public class ScoreExplain
{
    public int points { get; set; }
    public string name { get; set; }
    public int value { get; set; }

    public ScoreExplain diff(ScoreExplain other) {
        ScoreExplain diff = new ScoreExplain();
        diff.name = other.name;
        diff.points = points - other.points;
        diff.value = value - other.value;
        return diff;
    }
}
