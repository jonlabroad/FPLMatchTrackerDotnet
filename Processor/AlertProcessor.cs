using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
/*
public class AlertProcessor
{
    AlertProcessorConfig _config;
    int _leagueId;
    IEnumerable<int> _teamIds;
    MatchInfoProvider _matchInfoProvider;
    EPLClient _eplClient;

    ISet<int> _processedTeams = new HashSet<int>();

    private Logger _log = LogManager.GetCurrentClassLogger();

    public AlertProcessor(int leagueId, IEnumerable<int> teamIds, EPLClient client, ICollection<MatchInfo> matchInfos=null) {
        AlertProcessorConfig config = readConfig().Result;
        _config = config != null ? config : new AlertProcessorConfig();
        _leagueId = leagueId;
        _matchInfoProvider = new MatchInfoProvider(_leagueId, matchInfos);
        _teamIds = teamIds != null ? teamIds : new List<int>();
        _eplClient = client;
    }

    public async Task process()
    {
        await SetAvailableTeams();

        // Read match info data
        var matchInfos = await _matchInfoProvider.readAll();

        // Loop through matches and alert if necessary
        foreach (var info in matchInfos) {
            await process(info);
        }

        _config.LastProcessTime = Date.toString(DateTime.Now);
        await writeConfig();
    }

    private async Task process(MatchInfo info)
    {
        if (alreadyProcessed(info.teams.Keys)) {
            _log.Info(string.Format("Teams already processed. Skipping alerts\n"));
            return;
        }

        // Loop through events and find any that have been posted since last processing time
        int numRecentEvents = 0;
        DateTime lastPollDate = Date.fromString(_config.LastProcessTime);
        foreach (var ev in info.allEvents) {
            if (ev.type == MatchEventType.AUTOSUB_IN || ev.type == MatchEventType.AUTOSUB_OUT) {
                continue;
            }
            DateTime eventDate = Date.fromString(ev.dateTime);
            if (eventDate.CompareTo(lastPollDate) > 0) {
                _log.Info(string.Format("Found new event: {0} {1}\n", ev.footballerName, ev.type));
                numRecentEvents++;
            }
        }

        if (numRecentEvents > 0) {
            String alertTitle = generateAlertTitle(info);
            String alertText = generateAlertSubtitle(numRecentEvents);
            foreach (var teamId in info.teams.Keys) {
                if (shouldAlertTeam(teamId)) {
                    AndroidAlertSender sender = new AndroidAlertSender();
                    await sender.SendAlert(teamId, alertTitle, alertText);
                }
            }
        }

        info.teams.Keys.ToList().ForEach(k => _processedTeams.Add(k));
    }

    private bool alreadyProcessed(ICollection<int> teams) {
        var processed = false;
        foreach (var teamId in teams) {
            if (_processedTeams.Contains(teamId)) {
                return true;
            }
        }
        return processed;
    }

    private async Task<AlertProcessorConfig> readConfig() {
        return await new ConfigProvider().read();
    }

    private async Task writeConfig() {
        await new ConfigProvider().write(_config);
    }

    private string generateAlertTitle(MatchInfo info) {
        var teamIds = new List<int>();
        foreach (var teamId in info.teams.Keys) {
            teamIds.Add(teamId);
        }

        ProcessedMatchTeam team1 = info.teams[teamIds[0]];
        ProcessedMatchTeam team2 = info.teams[teamIds[1]];

        return string.Format("{0} ({1}) {2} - {3} ({4}) {5}", getTeamName(team1), team1.score.subScore, team1.score.startingScore,
                team2.score.startingScore, team2.score.subScore, getTeamName(team2));
    }

    private string getTeamName(ProcessedMatchTeam team) {
        String name = team.standing != null ? team.standing.entry_name : "";
        if (string.IsNullOrEmpty(name))
        {
            name = team.entry != null ? team.entry.entry.name : "";
        }
        return name;
    }

    private string generateAlertSubtitle(int numNewEvents) {
        return string.Format("{0} new events!", numNewEvents);
    }

    private bool shouldAlertTeam(int teamId) {
        if (_teamIds == null || _teamIds.Count() == 0) {
            return true;
        }
        return _teamIds.Contains(teamId);
    }

    private async Task SetAvailableTeams() {
        if (GlobalConfig.CloudAppConfig.AvailableTeams.Count == 0) {
            Standings standings = await _eplClient.getStandings(_leagueId);
            if (standings != null) {
                foreach (Standing standing in standings.standings.results) {
                    TeamIdName team = new TeamIdName();
                    team.teamId = standing.entry;
                    team.teamName = standing.entry_name;
                    team.teamOwner = standing.player_name;
                    GlobalConfig.CloudAppConfig.AvailableTeams[team.teamId] = team;
                }
                await new CloudAppConfigProvider().write(GlobalConfig.CloudAppConfig);
            }
        }
    }
}
 */