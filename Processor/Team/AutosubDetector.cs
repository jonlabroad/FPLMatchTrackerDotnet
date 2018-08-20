using System;
using System.Collections.Generic;

public class AutosubDetector
{
    ISet<int> startingPositions = new HashSet<int>();
    ISet<int> subPositions = new HashSet<int>();

    public AutosubDetector()  {
        for (int i = 1; i <= 11; i++) {
            startingPositions.Add(i);
        }
        for (int i = 12; i <= 15; i++) {
            subPositions.Add(i);
        }
    }

    public List<TeamMatchEvent> detectAutoSubs(int teamId, IList<ProcessedPick> oldPicks, IList<ProcessedPick> newPicks) {
        var subEvents = new List<TeamMatchEvent>();
        if (oldPicks == null || newPicks == null) {
            return subEvents;
        }

        foreach (var newPick in newPicks) {
            bool oldSub = isSub(oldPicks[newPick.footballer.rawData.footballer.id]);
            bool newSub = isSub(newPick);
            if (oldSub && !newSub) {
                var ev = PlayerEventGenerator.createMatchEvent(DateTime.Now,
                        MatchEventType.AUTOSUB,
                        newPick.footballer.rawData.footballer,
                        1,
                        0);
                TeamMatchEvent tEvent = new TeamMatchEvent(teamId, newPick.isCaptain(), newPick.getMultiplier(), ev);
                subEvents.Add(tEvent);
            }
        }
        return subEvents;
    }

    private bool isSub(ProcessedPick pick) {
        return subPositions.Contains(pick.pick.position);
    }
}
