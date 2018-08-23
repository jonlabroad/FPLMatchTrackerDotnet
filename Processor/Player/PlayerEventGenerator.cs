using System;
using System.Collections.Generic;
using NLog;

public static class PlayerEventGenerator
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public static List<MatchEvent> createNewEvents(FootballerScoreDetailElement detailsDiff, Footballer footballer, FootballerScoreDetailElement currentDetail)
    {
        var events = new List<MatchEvent>();
        DateTime time = DateTime.Now;
        if (detailsDiff.assists.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.ASSIST, footballer, detailsDiff.assists.value, detailsDiff.assists.points));
        }
        if (detailsDiff.goals_scored.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.GOAL, footballer, detailsDiff.goals_scored.value, detailsDiff.goals_scored.points));
        }
        if (detailsDiff.minutes.points != 0) {
            events.Add(createMatchEvent(time, MatchEventType.MINUTES_PLAYED, footballer, currentDetail.minutes.value, detailsDiff.minutes.points));
        }
        if (detailsDiff.clean_sheets.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.CLEAN_SHEET, footballer, detailsDiff.clean_sheets.value, detailsDiff.clean_sheets.points));
        }
        if (detailsDiff.bonus.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.BONUS, footballer, detailsDiff.bonus.value, detailsDiff.bonus.points));
        }
        if (detailsDiff.yellow_cards.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.YELLOW_CARD, footballer, detailsDiff.yellow_cards.value, detailsDiff.yellow_cards.points));
        }
        if (detailsDiff.red_cards.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.RED_CARD, footballer, detailsDiff.red_cards.value, detailsDiff.red_cards.points));
        }
        if (detailsDiff.penalties_missed.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.PENALTY_MISS, footballer, detailsDiff.penalties_missed.value, detailsDiff.penalties_missed.points));
        }
        if (detailsDiff.goals_conceded.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.GOALS_CONCEDED, footballer, detailsDiff.goals_conceded.value, detailsDiff.goals_conceded.points));
        }
        if (detailsDiff.saves.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.SAVES, footballer, detailsDiff.saves.value, detailsDiff.saves.points));
        }
        if (detailsDiff.penalties_saved.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.PENALTY_SAVES, footballer, detailsDiff.penalties_saved.value, detailsDiff.penalties_saved.points));
        }
        if (detailsDiff.own_goals.value != 0) {
            events.Add(createMatchEvent(time, MatchEventType.OWN_GOALS, footballer, detailsDiff.own_goals.value, detailsDiff.own_goals.points));
        }
        printMatchEvents(events);
        return events;
    }

    public static MatchEvent createMatchEvent(DateTime time, MatchEventType type, Footballer footballer, int number, int scoreDiff)
    {
        var ev = new MatchEvent();
        ev.type = type;
        if (footballer != null) {
            ev.footballerName = footballer.web_name;
            ev.footballerId = footballer.id;
        } else {
            ev.footballerName = "";
            ev.footballerId = 0;
        }
        ev.dateTime = timeToString(time);
        ev.typeString = type.ToString();
        ev.pointDifference = scoreDiff;
        ev.number = number;
        return ev;
    }

    private static String timeToString(DateTime time) {
        return Date.toString(time);
    }

    private static void printMatchEvents(List<MatchEvent> events) {
        foreach (MatchEvent ev in events) {
            _log.Info(string.Format("{0} {1} {2} {3}", ev.number, ev.typeString, ev.footballerName, ev.pointDifference));
        }
    }
}
