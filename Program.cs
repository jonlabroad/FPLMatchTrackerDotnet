using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FPLMatchTrackerDotnet
{
    class Program
    {
        static void Main(string[] args)
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
                Console.WriteLine("It's not time yet! Quiting...");
                highlightTask.Wait();
                dailyProcessorTask.Wait();
                return;
            }

            var start = DateTime.Now;
            var configUpdater = new CloudConfigUpdater(client);
            var generateScoutingReports = false;
            if (configUpdater.update().Result) {
                generateScoutingReports = true;
            }

            var eventProcessor = new EventProcessor(client, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            var eventProcessorTask = eventProcessor.process();

            var allMatchProcessor = new AllMatchProcessor(client, leagueId);
            var matchProcessingTasks = allMatchProcessor.Process();

            Task scoutingTask = null;
            if (generateScoutingReports)
            {
                scoutingTask = Task.Run(async () => {
                    var scoutingProcessor = new ScoutingProcessor(leagueId, client, await allMatchProcessor.GetProcessedTeams());
                    await scoutingProcessor.Process();
                });
            }

            var writeTasks = new List<Task>();
            Task.Run(async () =>
            {
                var matchInfos = await allMatchProcessor.GetMatchInfos();
                if (leagueId > 0) {
                    // Gross
                    if (true) {
                        var liveStandings = new LiveStandings(matchInfos, await client.getStandings(leagueId));
                        liveStandings?.liveStandings?.Sort();
                        foreach (var matchInfo in matchInfos) {
                            try {
                                matchInfo.liveStandings = liveStandings;
                            } finally {
                                writeTasks.Add(MatchProcessor.writeMatchInfo(leagueId, matchInfo));
                            }
                        }
                        Task.WhenAll(writeTasks).Wait();
                    }
                }
            }).Wait();
            preloadTask.Wait();
            dailyProcessorTask.Wait();
            highlightTask.Wait();
            scoutingTask?.Wait();
            matchProcessingTasks.Wait();
            stopWatch.Stop();
            Console.Write($"All processing took {stopWatch.Elapsed.TotalSeconds} sec");
        }

        private static async Task<bool> IsTimeToPoll(EPLClient client)
        {
            var eventFinder = new EventFinder(client);
            var currentTime = DateTime.Now;
            var ev = await eventFinder.GetCurrentEvent();
            var eventStart = eventFinder.GetEventStartTime(ev);
            Console.WriteLine(string.Format("Start date: {0}\n", eventStart.ToString()));
            Console.WriteLine(string.Format("Current date: {0}\n", currentTime.ToString()));
            Console.WriteLine(string.Format("Finished: {0}\n", ev.finished));
            Console.WriteLine(string.Format("Data checked: {0}\n", ev.data_checked));

            var fixtureTimer = new EventTimer(client);
            if (!await fixtureTimer.IsFixtureTime(ev)) {
                Console.WriteLine("No fixtures are currently on");
                return false;
            }

            return true;
        }
    }
}
