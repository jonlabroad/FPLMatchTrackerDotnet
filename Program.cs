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
                var leagueId = 22356;

                // RunUltimateH2h(leagueId); return;

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var client = new EPLClient(new RequestExecutor(GlobalConfig.EplBaseUrl));

                var allProcessorV3 = new AllProcessorV3(client, leagueId);
                var allProcessorV3Task = allProcessorV3.Process();
                allProcessorV3Task.Wait();

                stopWatch.Stop();
                _log.Info($"All processing took {stopWatch.Elapsed.TotalSeconds} sec");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        private static void RunUltimateH2h(int leagueId)
        {
            var uh2h = new UltimateH2h(leagueId);
            uh2h.Calculate().Wait();
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
