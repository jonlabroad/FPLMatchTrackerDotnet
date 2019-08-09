using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

public class AllProcessorV3
{
    EPLClient _client;
    int _leagueId;
    List<Task> _matchProcessingTasks = new List<Task>();
    IDictionary<string, MatchInfo> _matchInfos = new ConcurrentDictionary<string, MatchInfo>();
    private Logger _log = LogManager.GetCurrentClassLogger();
    public AllProcessorV3(EPLClient client, int leagueId)
    {
        _client = client;
        _leagueId = leagueId;
    }

    public async Task Process()
    {
        //var teamsToProcess = await _client.getTeamsInLeague(_leagueId);
        //var preloadEntryTask = new CachePreloader(_client).PreloadEntryCache(teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);

        _log.Debug("Starting Player Processors");
        var playerProcessor = new PlayerProcessorV3(_client);
        var processedPlayers = await playerProcessor.process();
        _log.Debug("Player Processing Complete");

        //await AddLiveStandingsAndSave();

        //await new AlertProcessor(_leagueId, teamsToProcess, _client, _matchInfos.Values).process();
    }
}