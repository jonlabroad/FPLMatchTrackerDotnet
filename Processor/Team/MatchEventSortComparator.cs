using System;
using System.Collections;
using System.Collections.Generic;

public class MatchEventSortComparator : IComparer<TeamMatchEvent>
{
    public int Compare(TeamMatchEvent other1, TeamMatchEvent other2) {
        var o1 = other1 as MatchEvent;
        var o2 = other2 as MatchEvent;
        DateTime date1 = Date.fromString(o1.dateTime);
        DateTime date2 = Date.fromString(o2.dateTime);
        return date1.CompareTo(date2);
    }
}
