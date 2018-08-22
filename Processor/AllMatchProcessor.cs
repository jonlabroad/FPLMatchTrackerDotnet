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
    Task _teamWaitTask;
    Task _matchWaitTask;

    public AllMatchProcessor(EPLClient client, int leagueId)
    {
        _client = client;
        _leagueId = leagueId;
        _teamWaitTask = TeamWaitTask();
        _matchWaitTask = MatchWaitTask();
    }

    public async Task Process()
    {
        var teamsToProcess = _client.getTeamsInLeague(_leagueId).Result;
        var preloadEntryTask = new CachePreloader(_client).PreloadEntryCache(teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);

        var matchTask = _client.findMatches(_leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
        Console.WriteLine("Starting Player Processors");
        var playerProcessor = new PlayerProcessor(_client);
        var processedPlayers = playerProcessor.process().Result;
        Console.WriteLine("Player Processing Complete");

        Console.WriteLine("Starting team Processors");
        var teamProcessor = new TeamProcessor(_client, teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek, _leagueId);
        _processedTeams = await teamProcessor.process();
        Console.WriteLine("Team Processing Complete");
        _teamLock.Release();
        
        ScoreCalculator.EstimateAverageScore(await GetProcessedTeams());

        var matches = await matchTask;
        Console.WriteLine("Starting Match Processing");
        foreach (var match in matches)
        {
            var matchKey = $"{match.entry_1_entry} {match.entry_2_entry}";
            _matchProcessingTasks.Add(Task.Run(async () =>
            {
                var matchProcessor = new MatchProcessor(_client, _leagueId, await GetProcessedTeams(), match);
                await matchProcessor.process().ContinueWith((matchInfo) =>
                {
                    _matchInfos[matchKey] = matchInfo.Result;
                });
            }));
        }
        await Task.WhenAll(_matchProcessingTasks);
        _matchLock.Release();
        Console.WriteLine("Match Processing Complete");
    }

    public async Task<IDictionary<int, ProcessedTeam>> GetProcessedTeams()
    {
        await _teamWaitTask;
        return _processedTeams;
    }

    public async Task<ICollection<MatchInfo>> GetMatchInfos()
    {
        await _matchWaitTask;
        return _matchInfos.Values;
    }

    private async Task TeamWaitTask()
    {
        await Task.Run(async () => await _teamLock.WaitAsync());
    }

    private async Task MatchWaitTask()
    {
        await Task.Run(async () => await _matchLock.WaitAsync());
    }

}