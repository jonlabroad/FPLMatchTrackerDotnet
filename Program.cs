using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using NLog.AWS.Logger;

namespace FPLMatchTrackerDotnet
{
    class Program
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var leagueId = 5815;

                var client = new EPLClient(new RequestExecutor());
                var cachePreloader = new CachePreloader(client);
                var preloadTask = cachePreloader.PreloadCache();

                // Processing only performed if this is running on-premise
                var highlightProcessor = new HighlightProcessor(GlobalConfig.CloudAppConfig.CurrentGameWeek, leagueId);
                var highlightTask = highlightProcessor.Process();

                var dailyProcessorTask = new DailyProcessor(leagueId, client).Process();

                if (false && !IsTimeToPoll(client).Result) {
                    _log.Info("It's not time yet! Quiting...");
                    highlightTask.Wait();
                    dailyProcessorTask.Wait();
                    return;
                }

                new CloudConfigUpdater(client).update().Wait();

                var start = DateTime.Now;
                var eventProcessor = new EventProcessor(client, GlobalConfig.CloudAppConfig.CurrentGameWeek);
                var eventProcessorTask = eventProcessor.process();

                var allMatchProcessor = new AllMatchProcessor(client, leagueId);
                allMatchProcessor.Process().Wait();

                preloadTask.Wait();
                dailyProcessorTask.Wait();
                highlightTask.Wait();
                stopWatch.Stop();
                _log.Info($"All processing took {stopWatch.Elapsed.TotalSeconds} sec");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        private static async Task<bool> IsTimeToPoll(EPLClient client)
        {
            var eventFinder = new EventFinder(client);
            var currentTime = DateTime.Now;
            var ev = await eventFinder.GetCurrentEvent();
            var eventStart = eventFinder.GetEventStartTime(ev);
            _log.Info(string.Format("Start date: {0}\n", eventStart.ToString()));
            _log.Info(string.Format("Current date: {0}\n", currentTime.ToString()));
            _log.Info(string.Format("Finished: {0}\n", ev.finished));
            _log.Info(string.Format("Data checked: {0}\n", ev.data_checked));

            var fixtureTimer = new EventTimer(client);
            if (!await fixtureTimer.IsFixtureTime(ev)) {
                _log.Info("No fixtures are currently on");
                return false;
            }

            return true;
        }
    }
}
