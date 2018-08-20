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
            stopWatch.Stop();
            Console.Write($"All processing took {stopWatch.Elapsed.TotalSeconds} sec");
            
        }
    }
}
