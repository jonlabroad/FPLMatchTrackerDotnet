using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var teamsToProcessTask = client.getTeamsInLeague(leagueId);

            var matchTask = client.findMatches(leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            var playerProcessor = new PlayerProcessor(client);
            var processedPlayers = playerProcessor.process().Result;

            var teamsToProcess = teamsToProcessTask.Result;
            var teamProcessor = new TeamProcessor(client, teamsToProcess, GlobalConfig.CloudAppConfig.CurrentGameWeek);
            var teams = teamProcessor.process().Result;
            estimateAverageScore(teams);

            var leagueProcessorTask = new LeagueProcessor(teams.Values, leagueId, GlobalConfig.CloudAppConfig.CurrentGameWeek).process();

            var matches = matchTask.Result;
            var matchInfos = new List<MatchInfo>();
            foreach (var match in matches)
            {
                var matchProcessor = new MatchProcessor(client, leagueId, teams, match);
                matchInfos.Add(matchProcessor.process().Result);
            }

            var writeTasks = new List<Task>();
            Task.Run(async () =>
            {
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
            leagueProcessorTask.Wait();
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
    }
}
