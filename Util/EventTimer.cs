using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

public class EventTimer
{
    EPLClient _client;
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public EventTimer(EPLClient client)
    {
        _client = client;
    }

    public async Task<bool> IsFixtureTime(Event ev)
    {
        var todaysFixtures = await getTodaysFixtures(ev);

        if (false && allFixturesComplete(todaysFixtures))
        {
            if (!GlobalConfig.CloudAppConfig.finalPollOfDayCompleted)
            {
                GlobalConfig.CloudAppConfig.finalPollOfDayCompleted = true;
                await new CloudAppConfigProvider().write(GlobalConfig.CloudAppConfig);
                _log.Info("All fixtures for the day are complete! Performing final poll");
                return true;
            }
            _log.Info("All fixtures for the day are complete and final poll has been performed!");
            return false;
        }

        foreach (var fixture in todaysFixtures)
        {
            _log.Info(string.Format("{0} ({1}) @ ({2}) {3}: {4}\n", fixture.team_a, fixture.team_a_score, fixture.team_h_score, fixture.team_h, fixture.kickoff_time));
            DateTime now = DateTime.Now;
            if (fixture.started && now.CompareTo(Date.fromApiString(fixture.kickoff_time).AddHours(8)) <= 0)
            {
                _log.Info(string.Format("Found fixture: {0} @ {1}\n", fixture.team_a, fixture.team_h));
                return true;
            }
        }
        return false;
    }

    private async Task<List<Fixture>> getTodaysFixtures(Event ev)
    {
        Live liveData = await _client.getLiveData(ev.id);
        if (liveData == null) {
            return new List<Fixture>();
        }

        List<Fixture> retFixtures = new List<Fixture>();
        foreach (var fixture in liveData.fixtures) {
            DateTime now = DateTime.Now;
            DateTime kickoff = Date.fromApiString(fixture.kickoff_time);
            if (kickoff.Month == now.Month && kickoff.Day == now.Day) {
                retFixtures.Add(fixture);
            }
        }
        return retFixtures;
    }

    private static bool allFixturesComplete(List<Fixture> fixtures)
    {
            var allComplete = fixtures.Count > 0 ? true : false;
            foreach (var fixture in fixtures) {
                allComplete &= fixture.finished && fixture.finished_provisional;
            }
            return allComplete;
        }

}