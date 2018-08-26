using System;

public class MatchEvent
{
    public string dateTime { get; set; }
    public MatchEventType type { get; set; }
    public string typeString { get; set; }
    public int footballerId { get; set; }
    public string footballerName { get; set; }
    public int pointDifference { get; set; }
    public int number { get; set; }

    public MatchEvent() {}

    public MatchEvent(MatchEvent other) {
        dateTime = other.dateTime;
        type = other.type;
        typeString = other.typeString;
        footballerId = other.footballerId;
        footballerName = other.footballerName;
        pointDifference = other.pointDifference;
        number = other.number;
    }

    public override bool Equals(Object otherObj) {
        MatchEvent other = (MatchEvent) otherObj;
        return  basicEquals(other);
    }

    public bool basicEquals(MatchEvent other) {
        return  type == other.type &&
                footballerId == other.footballerId &&
                pointDifference == other.pointDifference &&
                number == other.number;
    }

    public String typeToReadableString(int number) {
        String plural = number > 1 ? "s" : "";
        String ePlural = number > 1 ? "es" : "";
        switch (type) {
            case MatchEventType.GOAL:
                return "Goal" + plural;
            case MatchEventType.ASSIST:
                return "Assist" + plural;
            case MatchEventType.MINUTES_PLAYED:
                return "Minute" + plural + " played";
            case MatchEventType.CLEAN_SHEET:
                return "Clean sheet" + plural;
            case MatchEventType.BONUS:
                return "Bonus";
            case MatchEventType.YELLOW_CARD:
                return "Yellow card" + plural;
            case MatchEventType.RED_CARD:
                return "Red card" + plural;
            case MatchEventType.PENALTY_MISS:
                return "Penalty miss" + ePlural;
            case MatchEventType.GOALS_CONCEDED:
                return "Goal" + plural + " conceded";
            case MatchEventType.SAVES:
                return "Save" + plural;
            case MatchEventType.PENALTY_SAVES:
                return "Penalty save" + plural;
            case MatchEventType.OWN_GOALS:
                return "Own goal" + plural;
            case MatchEventType.AUTOSUB_OUT:
                return "Substitution Out";
            case MatchEventType.AUTOSUB_IN:
                return "Substitution In";
                
            default:
                return type.ToString();
        }
    }
}
