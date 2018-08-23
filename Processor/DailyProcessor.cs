using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DailyProcessor
{
    int _leagueId;
    EPLClient _client;

    public DailyProcessor(int leagueId, EPLClient client)
    {
        _leagueId = leagueId;
        _client = client;
    }

    public async Task Process()
    {
        if (!IsTimeToProcess())
        {
            Console.WriteLine("Daily processing already completed");
            return;
        }

        await UpdateCloudAppConfig();

        var teamsInLeagueTask = _client.getTeamsInLeague(_leagueId);
        var playerProcessorTask = new PlayerProcessor(_client).process();
        var teamProcessorTask = new TeamProcessor(_client, await teamsInLeagueTask, GlobalConfig.CloudAppConfig.CurrentGameWeek, _leagueId, await playerProcessorTask).process();
        await new ScoutingProcessor(_leagueId, _client, await teamProcessorTask).Process();
    }

    private bool IsTimeToProcess()
    {
        var dailyTimer = new DailyTimer();
        return dailyTimer.IsNewDay();
    }

    private async Task UpdateCloudAppConfig()
    {
        var currentTime = DateTime.Now;
        GlobalConfig.CloudAppConfig.finalPollOfDayCompleted = false;
        GlobalConfig.CloudAppConfig.day = Date.toString(currentTime);
        await new CloudAppConfigProvider().write(GlobalConfig.CloudAppConfig);
    }
}