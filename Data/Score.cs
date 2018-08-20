public class Score
{
    public int startingScore { get; set; } = 0;
    public int subScore { get; set; } = 0;

    public bool Equals(Score other) {
        return (startingScore == other.startingScore) && (subScore == other.subScore);
    }
}
