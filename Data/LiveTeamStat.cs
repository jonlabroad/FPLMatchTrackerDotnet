using System;
using System.Collections.Generic;

public class LiveTeamStat : IComparable<LiveTeamStat>
{
    public int teamId {get; set;}
    public string teamName {get; set;}
    public Score score  {get; set;} =  new Score();

    public int CompareTo(LiveTeamStat other)
    {
        int comparison = this.score.startingScore.CompareTo(other.score.startingScore);
        if (comparison == 0) {
            comparison = this.score.subScore.CompareTo(other.score.subScore);
        }
        return comparison;
    }
}
