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
            var highlightTask = highlightProcessor.process();

            if (!IsTimeToPoll(client).Result) {
                Console.WriteLine("It's not time yet! Quiting...");
                highlightTask.Wait();
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

            var teamsToProcess = client.getTeamsInLeague(leagueId).Result;
            var preloadEntryTask = cachePreloader.PreloadEntryCache(teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);

            var matchTask = client.findMatches(leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            Console.WriteLine("Starting Player Processors");
            var playerProcessor = new PlayerProcessor(client);
            var processedPlayers = playerProcessor.process().Result;
            Console.WriteLine("Player Processing Complete");

            Console.WriteLine("Starting team Processors");
            var teamProcessor = new TeamProcessor(client, teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            var teams = teamProcessor.process().Result;
            ScoreCalculator.EstimateAverageScore(teams);
            Console.WriteLine("Team Processing Complete");

            var leagueProcessorTask = new LeagueProcessor(teams.Values, leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek).process();

            var matches = matchTask.Result;
            Console.WriteLine("Starting Match Processing");
            var matchInfos = new ConcurrentDictionary<string, MatchInfo>();
            var matchProcessingTasks = new List<Task>();
            foreach (var match in matches)
            {
                var matchKey = $"{match.entry_1_entry} {match.entry_2_entry}";
                matchProcessingTasks.Add(Task.Run(async () => {
                    var matchProcessor = new MatchProcessor(client, leagueId, teams, match);
                    await matchProcessor.process().ContinueWith((matchInfo) => {
                        matchInfos[matchKey] = matchInfo.Result;
                    });
                }));
            }
            Task.WhenAll(matchProcessingTasks).Wait();
            Console.WriteLine("Match Processing Complete");

            var writeTasks = new List<Task>();
            Task.Run(async () =>
            {
                if (leagueId > 0) {
                    // Gross
                    if (true) {
                        var liveStandings = new LiveStandings(matchInfos.Values, await client.getStandings(leagueId));
                        liveStandings?.liveStandings?.Sort();
                        foreach (var matchInfo in matchInfos.Values) {
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
            leagueProcessorTask.Wait();
            preloadTask.Wait();
            preloadEntryTask.Wait();
            highlightTask.Wait();
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

            var lastConfigTimestamp = Date.fromString(GlobalConfig.CloudAppConfig.day);
            if (string.IsNullOrEmpty(GlobalConfig.CloudAppConfig.day) || lastConfigTimestamp.Day != currentTime.Day) {
                Console.WriteLine("It's a new day!");
                GlobalConfig.CloudAppConfig.finalPollOfDayCompleted = false;
                GlobalConfig.CloudAppConfig.day = Date.toString(currentTime);
                await new CloudAppConfigProvider().write(GlobalConfig.CloudAppConfig);
            }

            var fixtureTimer = new EventTimer(client);
            if (!await fixtureTimer.IsFixtureTime(ev)) {
                Console.WriteLine("No fixtures are currently on");
                return false;
            }

            return true;
        }
    }
}
