using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

public class DailyProcessorV3
{
    int _leagueId;
    EPLClient _client;
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public DailyProcessorV3(int leagueId, EPLClient client)
    {
        _leagueId = leagueId;
        _client = client;
    }

    public async Task Process()
    {
        if (!IsTimeToProcess())
        {
            _log.Info("Daily processing already completed");
            return;
        }

        try {
            await new LeagueStandingsProcessor(_client).process();
            await UpdateCloudAppConfig();
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
        }

        //var teamsInLeague = await _client.getTeamsInLeague(_leagueId);
        //var processedPlayers = await new PlayerProcessorV3(_client).process();
        //var processedTeams = await new TeamProcessor(_client, teamsInLeague, GlobalConfig.CloudAppConfig.CurrentGameWeek, _leagueId, processedPlayers).process();
        //await new ScoutingProcessor(_leagueId, _client, processedTeams).Process();
    }

    private bool IsTimeToProcess()
    {
        var dailyTimer = new DailyTimer();
        return dailyTimer.IsNewDay();
    }

    private async Task UpdateCloudAppConfig()
    {
        var currentTime = DateTime.UtcNow;
        GlobalConfig.CloudAppConfig.day = DateTime.Now.Day;
        await new CloudAppConfigProvider().write(GlobalConfig.CloudAppConfig);
    }
}