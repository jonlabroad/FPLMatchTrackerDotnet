using System.Collections.Generic;
using System.Linq;

public class LiveStandings
{
    public List<LiveStandingTeam> liveStandings {get; set;} = new List<LiveStandingTeam>();

    public LiveStandings(ICollection<MatchInfo> matchInfos, Standings standings)
    {
        foreach (var matchInfo in matchInfos.Where(m => !m.isCup))
        {
            var teams = matchInfo.teams.Values.ToArray();
            LiveStandingTeam team1 = new LiveStandingTeam(teams[0], teams[1], standings);
            LiveStandingTeam team2 = new LiveStandingTeam(teams[1], teams[0], standings);
            liveStandings.Add(team1);
            liveStandings.Add(team2);
        }
    }
}
