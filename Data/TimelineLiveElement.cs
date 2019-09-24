public class TimelineLiveElement : LiveElementBase {
    public TimelineLiveElement()
    {

    }
    
    public TimelineLiveElement(TimelineLiveElement other)
    : base(other)
    {
        score = other.score;
    }

    public TimelineLiveElement(LiveElementBase other)
    : base(other)
    {
        CalcScore();
    }

    public int score { get; set; }

    public void CalcScore() {
        score = new ScoreCalculator().calculateFootballerScore(this.explain);
    }
}