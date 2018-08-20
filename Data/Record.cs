using System;

public class Record : IComparable
{
    public int teamId {get; set;}
    public string teamName { get; set; }
    public int wins { get; set; } = 0;
    public int draws { get; set; } = 0;
    public int losses { get; set; } = 0;

    public int getPoints() {
        return wins*3 + draws;
    }

    public int CompareTo(Object o) {
        var other = o as Record;
        return other.getPoints().CompareTo(getPoints());
    }
}
