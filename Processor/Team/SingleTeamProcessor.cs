using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;

public class SingleTeamProcessor
{
    private int _teamId;
    int _gameweek;
    int _leagueId;
    EPLClient _client;
    ProcessedPlayerProvider _playerProvider;
    ProcessedTeam _processedTeam = null;
    private static Logger _log = LogManager.GetCurrentClassLogger();

    public SingleTeamProcessor(ProcessedPlayerProvider provider, int teamId, int gameweek, int leagueId, EPLClient client)
    {
        _teamId = teamId;
        _gameweek = gameweek;
        _leagueId = leagueId;

        _playerProvider = provider;
        _client = client;
    }

    public async Task<ProcessedTeam> process()
    {
        _log.Info($"Processing {_teamId}");
        // Collect the player information
        var processedPicks = await getPlayersForTeam();

        var picks = await _client.getPicks(_teamId, _gameweek);
        var ev = await getCurrentEvent();
        var useEventScore = ev.data_checked && ev.finished;

        // Calculate score
        Score score;
        if (!useEventScore) {
            score = new ScoreCalculator().calculate(picks, processedPicks, picks != null && picks.active_chip.Equals("bboost"));
        }
        else {
            score = new Score();
            if (picks != null) {
                score.startingScore = picks.entry_history.points;
                score.subScore = picks.entry_history.points_on_bench;
            }
            else {
                // AVERAGE
                Standings standings = await _client.getStandings(_leagueId);
                foreach (Match match in standings.matches_this.results) {
                    if (match.entry_1_entry == 0) {
                        score.startingScore = match.entry_1_points;
                        break;
                    }
                    else if (match.entry_2_entry == 0) {
                        score.startingScore = match.entry_2_points;
                        break;
                    }
                }
            }
        }

        // Merge all the events into a single stream
        List<TeamMatchEvent> events = mergeEvents(processedPicks);
        if (picks != null)
        {
            (await CreateAutosubEvents(picks)).ForEach(s => events.Add(s));
        }
        EntryData entry = await _client.getEntry(_teamId);
        var history = await _client.getHistory(_teamId);
        ProcessedTeam team = new ProcessedTeam(_teamId, entry, processedPicks, score, events, picks != null ? picks.active_chip : "", history);
        team.transferCost = picks != null ? picks.entry_history.event_transfers_cost : 0;

        _processedTeam = team;
        _log.Info($"Completed Processing {_teamId}");
        return team;
    }

    public ProcessedTeam getResult() {
        return _processedTeam;
    }

    public int getId() {
        return _teamId;
    }

    public async Task<List<ProcessedPick>> getPlayersForTeam() {
        List<ProcessedPick> processedPicks = new List<ProcessedPick>();
        var picks = await _client.getPicks(_teamId, _gameweek);
        if (picks == null) {
            return processedPicks;
        }

        foreach (var pick in picks.picks) {
            ProcessedPlayer processedPlayer = await readProcessedPlayer(pick.element);
            ProcessedPick processedPick = new ProcessedPick(processedPlayer, pick);
            processedPicks.Add(processedPick);
        }
        return processedPicks;
    }

    List<TeamMatchEvent> mergeEvents(List<ProcessedPick> picks) {
        List<TeamMatchEvent> events = new List<TeamMatchEvent>();
        foreach (var pick in picks) {
            foreach (MatchEvent ev in pick.footballer.events) {
                TeamMatchEvent tEvent = new TeamMatchEvent(_teamId, pick.isCaptain(), pick.getMultiplier(), ev);
                events.Add(tEvent);
            }
        }
        return events;
    }

    private async Task<List<TeamMatchEvent>> CreateAutosubEvents(Picks picks)
    {
        var footballers = await _client.getFootballers();
        var subList = new List<TeamMatchEvent>();
        foreach (var sub in picks.automatic_subs)
        {
            var footballerIn = footballers[sub.element_in];
            var footballerOut = footballers[sub.element_out];
            var eventOut = new TeamMatchEvent
            {
                type = MatchEventType.AUTOSUB_OUT,
                footballerName = footballerOut.web_name,
                footballerId = footballerOut.id,
                dateTime = Date.toString(DateTime.Now),
                typeString = MatchEventType.AUTOSUB_OUT.ToString(),
                pointDifference = 0,
                number = 0,
                teamId = _teamId
            };
            var eventIn = new TeamMatchEvent
            {
                type = MatchEventType.AUTOSUB_IN,
                footballerName = footballerIn.web_name,
                footballerId = footballerIn.id,
                dateTime = Date.toString(DateTime.Now),
                typeString = MatchEventType.AUTOSUB_IN.ToString(),
                pointDifference = 0,
                number = 0,
                teamId = _teamId
            };
            subList.Add(eventOut);
            subList.Add(eventIn);
        }
        return subList;
    }

    private async Task<ProcessedPlayer> readProcessedPlayer(int footballerId) {
        return await _playerProvider.getPlayer(footballerId);
    }

    private async Task<Event> getCurrentEvent() {
        var boot = await _client.getBootstrapStatic();
        int currentEvent = boot.currentEvent;
        foreach (var ev in boot.events) {
            if (ev.id == currentEvent) {
                return ev;
            }
        }
        return null;
    }
}
