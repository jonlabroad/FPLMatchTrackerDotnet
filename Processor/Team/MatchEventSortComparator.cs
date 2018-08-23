using System;
using System.Collections;
using System.Collections.Generic;
using NLog;

public class MatchEventSortComparator : IComparer<TeamMatchEvent>
{
    private Logger _log = LogManager.GetCurrentClassLogger();

    public int Compare(TeamMatchEvent other1, TeamMatchEvent other2) {
        var o1 = other1 as MatchEvent;
        var o2 = other2 as MatchEvent;
        try {
            DateTime date1 = Date.fromString(o1.dateTime);
            DateTime date2 = Date.fromString(o2.dateTime);
            return date1.CompareTo(date2);
        }
        catch (Exception ex)
        {
            _log.Error(o1.dateTime);
            _log.Error(o2.dateTime);
            _log.Error(ex);
        }
        return 0;
    }
}
