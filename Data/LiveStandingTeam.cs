using System;

public class LiveStandingTeam : IComparable
{
    string teamName {get; set;}
    int teamId {get; set;}
    Standing standing {get; set;}
    string liveResult {get; set;}
    Score currentWeekScore {get; set;} = new Score();

    public LiveStandingTeam(ProcessedTeam team, ProcessedTeam otherTeam, Standings standings)
    {
        Standing oldStanding = findStanding(team.id, standings);
        standing = new Standing(oldStanding);

        teamName = team.entry != null ? team.entry.entry.name : "AVERAGE";
        teamId = team.id;
        currentWeekScore.startingScore = team.score.startingScore;
        currentWeekScore.subScore = team.score.subScore;

        if (doIncrementStandings(standing)) {
            liveResult = "D";
            standing.matches_played++;
            standing.points_for += team.score.startingScore;
            if (team.score.startingScore > otherTeam.score.startingScore) {
                liveResult = "W";
                standing.matches_won++;
                standing.points_total += 3;
            } else if (team.score.startingScore < otherTeam.score.startingScore) {
                liveResult = "L";
                standing.matches_lost++;
                standing.points_total += 0;
            } else {
                standing.matches_drawn++;
                standing.points_total += 1;
            }
        }
    }

    private bool doIncrementStandings(Standing standing)
    {
        return standing.matches_played < GlobalConfig.CloudAppConfig.CurrentGameWeek;
    }

    private Standing findStanding(int teamId, Standings standings)
    {
        foreach (var result in standings.standings.results)
        {
            if (result.entry == teamId) {
                return result;
            }
        }
        return null;
    }

    public int CompareTo(object other) {
        var o = other as LiveStandingTeam;
        int comp = o.standing.points_total.CompareTo(this.standing.points_total);
        if (comp == 0) {
            comp = o.standing.points_for.CompareTo(this.standing.points_for);
        }
        return comp;
    }
}


