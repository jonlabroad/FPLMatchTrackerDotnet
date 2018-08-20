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
            var preloadTask = PreloadCache(client);

            var teamsToProcess = client.getTeamsInLeague(leagueId).Result;
            var preloadEntryTask = PreloadEntryCache(client, teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);

            var matchTask = client.findMatches(leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            Console.WriteLine("Starting Player Processors");
            var playerProcessor = new PlayerProcessor(client);
            var processedPlayers = playerProcessor.process().Result;
            Console.WriteLine("Player Processing Complete");

            Console.WriteLine("Starting team Processors");
            var teamProcessor = new TeamProcessor(client, teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            var teams = teamProcessor.process().Result;
            estimateAverageScore(teams);
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
            stopWatch.Stop();
            Console.Write($"All processing took {stopWatch.Elapsed.TotalSeconds} sec");
            
        }

        private static void estimateAverageScore(IDictionary<int, ProcessedTeam> teams)
        {
            ProcessedTeam average = null;
            average = teams.TryGetValue(0, out average) ? average : null;
            if (average != null) {
                int totalScore = 0;
                int numAvgd = 0;
                foreach (ProcessedTeam team in teams.Values) {
                    if (team.id != 0) {
                        totalScore += team.score.startingScore + team.transferCost;
                        numAvgd++;
                    }
                }
                int avgScore = totalScore/numAvgd;
                average.score.startingScore = avgScore;
            }
        }

        private static async Task PreloadCache(EPLClient client)
        {
            var tasks = new List<Task>();
            tasks.Add(client.getFootballers());
            tasks.Add(client.getBootstrapStatic());
            await Task.WhenAll(tasks);
        }
        
        private static async Task PreloadEntryCache(EPLClient client, ICollection<int> teamIds, int gameweek)
        {   
            var tasks = teamIds.Select(async id => await client.getEntry(id)).ToList();
            await Task.WhenAll(teamIds.Select(async id => await client.getPicks(id, gameweek)).ToList());
            await Task.WhenAll(teamIds.Select(async id => await client.getHistory(id)).ToList());
            await Task.WhenAll(tasks);
        }
    }
}
