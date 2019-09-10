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
        // my team: 55385

        // Only required once per year ;)
        //var leagueScheduleProcesser = new LeagueScheduleProcessor(_client);
        //leagueScheduleProcesser.processH2hLeagues().Wait();

        //var teamsToProcess = await _client.getTeamsInLeague(_leagueId);
        //var preloadEntryTask = new CachePreloader(_client).PreloadEntryCache(teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);

        // TODO make only daily processor
        //await new LeagueStandingsProcessor(_client).process();

        await (new DailyProcessorV3(_leagueId, _client).Process());

        if (await IsTimeToPoll(_client)) {
            _log.Debug("Starting Player Processors");
            var playerProcessor = new PlayerProcessorV3(_client);
            var processedPlayers = await playerProcessor.process();
            _log.Debug("Player Processing Complete");
        }

        //await AddLiveStandingsAndSave();

        //await new AlertProcessor(_leagueId, teamsToProcess, _client, _matchInfos.Values).process();
    }

    private async Task<bool> IsTimeToPoll(EPLClient client)
    {
        var eventFinder = new EventFinder(client);
        var currentTime = DateTime.Now;
        var ev = await eventFinder.GetCurrentEvent();
        var eventStart = eventFinder.GetEventStartTime(ev);
        _log.Info(string.Format("Start date: {0}\n", eventStart.ToString()));
        _log.Info(string.Format("Current date: {0}\n", currentTime.ToString()));
        _log.Info(string.Format("Finished: {0}\n", ev.finished));
        _log.Info(string.Format("Data checked: {0}\n", ev.data_checked));

        if (ev.is_current && !ev.finished && !ev.data_checked) {
            return true;
        }

        return false;
    }
}