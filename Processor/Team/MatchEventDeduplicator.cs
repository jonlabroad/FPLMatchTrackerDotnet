using System;
using System.Collections.Generic;
using NLog;

public class MatchEventDeduplicator
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public List<TeamMatchEvent> deduplicate(ProcessedTeam team1, ProcessedTeam team2) {
        if (team1.id == team2.id) {
            return team1.events;
        }

        var copiedEvents = copyEvents(team1.events, team2.events);

        foreach (var team1Event in team1.events) {
            var team2Event = getEqualEvent(team1Event, team2.events);
            if (team2Event != null) {
                var team1Pick = team1.getPick(team1Event.footballerId);
                var team2Pick = team2.getPick(team2Event.footballerId);
                if (team1Pick.equals(team2Pick)) {
                    _log.Info(string.Format("Found identical event: {0} {1}\n", team1Event.footballerName, team1Event.type.ToString()));
                    var sharedEvent = copiedEvents[copiedEvents.IndexOf(team1Event)];
                    sharedEvent.teamId = -1;
                    copiedEvents.Remove(team2Event);
                }
            }
        }
        return copiedEvents;
    }

    private TeamMatchEvent getEqualEvent(MatchEvent eventObj, IList<TeamMatchEvent> events) {
        foreach (var e in events) {
            MatchEvent baseEvent = e;
            if (baseEvent.basicEquals(eventObj)) {
                return e;
            }
        }
        return null;
    }

    private List<TeamMatchEvent> copyEvents(IList<TeamMatchEvent> team1Events, IList<TeamMatchEvent> team2Events) {
        var newEvents = new List<TeamMatchEvent>();
        foreach (var eventObj in team1Events) {
            newEvents.Add(new TeamMatchEvent(eventObj));
        }
        foreach (var eventObj in team2Events) {
            newEvents.Add(new TeamMatchEvent(eventObj));
        }
        return newEvents;
    }
}
