using System;

public class TeamMatchEvent : MatchEvent
{
    public int teamId {get;set;}
    public bool isCaptain {get;set;}
    public int multiplier {get;set;}

    public TeamMatchEvent() {}

    public TeamMatchEvent(TeamMatchEvent other)
        : base(other)
    {
        teamId = other.teamId;
        isCaptain = other.isCaptain;
        multiplier = other.multiplier;
    }

    public TeamMatchEvent(int team, bool isCpt, int mult, MatchEvent ev)
        : base(ev)
    {
        teamId = team;
        isCaptain = isCpt;
        multiplier = mult;
    }

    public override bool Equals(Object otherObj) {
        TeamMatchEvent other = (TeamMatchEvent) otherObj;
        return base.Equals(other) &&
                teamId == other.teamId &&
                isCaptain == other.isCaptain &&
                multiplier == other.multiplier;
    }
}
