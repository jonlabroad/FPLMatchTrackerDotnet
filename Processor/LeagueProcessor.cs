using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;

public class LeagueProcessor
{
    private static readonly string KEY_PATH_FORMAT = "{0}/{1}/LiveLeague_{2}.json";

    private int _gameweek;
    private int _leagueId;
    private ICollection<ProcessedTeam> _teams;
    private static Logger _log = LogManager.GetCurrentClassLogger();

    public LeagueProcessor(ICollection<ProcessedTeam> teams, int leagueId, int gameweek)
    {
        _teams = teams;
        _leagueId = leagueId;
        _gameweek = gameweek;
    }

    public async Task process()
    {
        var sw = new Stopwatch();
        sw.Start();
        await findLiveGameweekStandings();
        sw.Stop();
        _log.Info($"LeagueProcessor took {sw.Elapsed.TotalSeconds} sec");
    }

    private async Task findLiveGameweekStandings()
    {
        var stats = new List<LiveTeamStat>();
        foreach (var team in _teams) {
            if (team.id == 0) {
                continue;
            }

            LiveTeamStat stat = new LiveTeamStat();
            stat.teamId = team.id;
            stat.teamName = team.entry.entry != null ? team.entry.entry.name : "???";
            stat.score = team.score;
            stats.Add(stat);
        }
        stats.Sort();
        await new S3JsonWriter().write(createLiveStandingsKey(), stats);
    }

    private string createLiveStandingsKey() {
        return string.Format(KEY_PATH_FORMAT, GlobalConfig.MatchInfoRoot, _leagueId, _gameweek);
    }
}
