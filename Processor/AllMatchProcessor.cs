using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

public class AllMatchProcessor
{
    EPLClient _client;
    int _leagueId;
    List<Task> _matchProcessingTasks = new List<Task>();
    IDictionary<string, MatchInfo> _matchInfos = new ConcurrentDictionary<string, MatchInfo>();
    private Logger _log = LogManager.GetCurrentClassLogger();
    public AllMatchProcessor(EPLClient client, int leagueId)
    {
        _client = client;
        _leagueId = leagueId;
    }

    public async Task Process()
    {
        var teamsToProcess = await _client.getTeamsInLeague(_leagueId);
        var cupTeams = await GetCupTeams(teamsToProcess);
        teamsToProcess = cupTeams.Union(teamsToProcess);
        var preloadEntryTask = new CachePreloader(_client).PreloadEntryCache(teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);

        var matchTask = _client.findMatches(_leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
        _log.Debug("Starting Player Processors");
        var playerProcessor = new PlayerProcessor(_client);
        var processedPlayers = await playerProcessor.process();
        _log.Debug("Player Processing Complete");

        _log.Debug("Starting team Processors");
        var teamProcessorTask = new TeamProcessor(_client, teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek, _leagueId, processedPlayers).process();
        _log.Debug("Team Processing Complete");
        
        ScoreCalculator.EstimateAverageScore(await teamProcessorTask);

        var matches = await matchTask;
        _log.Debug("Starting Match Processing");
        foreach (var match in matches)
        {
            var matchKey = $"{match.entry_1_entry} {match.entry_2_entry}";
            _matchProcessingTasks.Add(Task.Run(async () =>
            {
                var matchProcessor = new MatchProcessor(_client, _leagueId, await teamProcessorTask, match);
                await matchProcessor.process().ContinueWith(async (matchInfo) =>
                {
                    _matchInfos[matchKey] = await matchInfo;
                });
            }));
        }

        _log.Debug("Starting Cup Processing");
        foreach (var teamId in teamsToProcess)
        {
            var match = await _client.findCupMatch(teamId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            if (match == null)
            {
                continue;
            }

            var matchKey = $"{match.entry_1_entry} {match.entry_2_entry}";
            _matchProcessingTasks.Add(Task.Run(async () =>
            {
                var matchProcessor = new MatchProcessor(_client, 0 /* cup */, await teamProcessorTask, match);
                await matchProcessor.process().ContinueWith(async (matchInfo) =>
                {
                    _matchInfos[matchKey] = await matchInfo;
                });
            }));
        }

        await Task.WhenAll(_matchProcessingTasks);
        await AddLiveStandingsAndSave();

        await new AlertProcessor(_leagueId, teamsToProcess, _client, _matchInfos.Values).process();

        _log.Debug("Match Processing Complete");
    }

    private async Task AddLiveStandingsAndSave()
    {
        if (_leagueId > 0)
        {
            // Gross
            if (true)
            {
                var writeTasks = new List<Task>();
                var liveStandings = new LiveStandings(_matchInfos.Values, await _client.getStandings(_leagueId));
                liveStandings?.liveStandings?.Sort();
                foreach (var matchInfo in _matchInfos.Values)
                {
                    try
                    {
                        matchInfo.liveStandings = liveStandings;
                    }
                    finally
                    {
                        writeTasks.Add(MatchProcessor.writeMatchInfo(_leagueId, matchInfo));
                    }
                }
                Task.WhenAll(writeTasks).Wait();
            }
        }
    }

    private async Task<IEnumerable<int>> GetCupTeams(IEnumerable<int> leagueTeams)
    {
        var newTeams = new HashSet<int>();
        foreach(var teamId in leagueTeams)
        {
            var cupMatch = await _client.findCupMatch(teamId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            if (cupMatch != null) {
                if (cupMatch.entry_1_entry != teamId) {
                    newTeams.Add(cupMatch.entry_1_entry);
                }
                newTeams.Add(cupMatch.entry_2_entry);
            }
        }
        return newTeams;
    }
}