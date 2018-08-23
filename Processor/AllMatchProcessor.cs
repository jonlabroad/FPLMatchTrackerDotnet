using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class AllMatchProcessor
{
    EPLClient _client;
    int _leagueId;
    IDictionary<int, ProcessedTeam> _processedTeams = new ConcurrentDictionary<int, ProcessedTeam>();
    Task<IDictionary<int, ProcessedTeam>> _processedTeamTask;
    SemaphoreSlim _teamLock = new SemaphoreSlim(0,1);
    List<Task> _matchProcessingTasks = new List<Task>();
    IDictionary<string, MatchInfo> _matchInfos = new ConcurrentDictionary<string, MatchInfo>();
    SemaphoreSlim _matchLock = new SemaphoreSlim(0,1);

    public AllMatchProcessor(EPLClient client, int leagueId)
    {
        _client = client;
        _leagueId = leagueId;
    }

    public async Task Process()
    {
        var teamsToProcess = await _client.getTeamsInLeague(_leagueId);
        var preloadEntryTask = new CachePreloader(_client).PreloadEntryCache(teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);

        var matchTask = _client.findMatches(_leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
        Console.WriteLine("Starting Player Processors");
        var playerProcessor = new PlayerProcessor(_client);
        var processedPlayers = await playerProcessor.process();
        Console.WriteLine("Player Processing Complete");

        Console.WriteLine("Starting team Processors");
        var teamProcessorTask = new TeamProcessor(_client, teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek, _leagueId, processedPlayers).process();
        Console.WriteLine("Team Processing Complete");
        
        ScoreCalculator.EstimateAverageScore(await teamProcessorTask);

        var matches = await matchTask;
        Console.WriteLine("Starting Match Processing");
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
        await Task.WhenAll(_matchProcessingTasks);
        await AddLiveStandingsAndSave();
        Console.WriteLine("Match Processing Complete");
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
}