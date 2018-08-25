public class FootballerScoreDetailElement {
    public ScoreExplain minutes { get; set; } = new ScoreExplain();
    public ScoreExplain goals_scored { get; set; } = new ScoreExplain();
    public ScoreExplain bonus { get; set; } = new ScoreExplain();
    public ScoreExplain clean_sheets { get; set; } = new ScoreExplain();
    public ScoreExplain assists { get; set; } = new ScoreExplain();
    public ScoreExplain yellow_cards { get; set; } = new ScoreExplain();
    public ScoreExplain red_cards { get; set; } = new ScoreExplain();
    public ScoreExplain penalties_missed { get; set; } = new ScoreExplain();
    public ScoreExplain goals_conceded { get; set; } = new ScoreExplain();
    public ScoreExplain saves { get; set; } = new ScoreExplain();
    public ScoreExplain penalties_saved { get; set; } = new ScoreExplain();
    public ScoreExplain own_goals { get; set; } = new ScoreExplain();


    public FootballerScoreDetailElement compare(FootballerScoreDetailElement other) {
        if (other == null) {
            return this;
        }
        FootballerScoreDetailElement diff = new FootballerScoreDetailElement();
        diff.minutes = minutes.diff(other.minutes);
        diff.goals_scored = goals_scored.diff(other.goals_scored);
        diff.bonus = bonus.diff(other.bonus);
        diff.clean_sheets = clean_sheets.diff(other.clean_sheets);
        diff.assists = assists.diff(other.assists);
        diff.yellow_cards = yellow_cards.diff(other.yellow_cards);
        diff.red_cards = red_cards.diff(other.red_cards);
        diff.penalties_missed = penalties_missed.diff(other.penalties_missed);
        diff.goals_conceded = goals_conceded.diff(other.goals_conceded);
        diff.saves = saves.diff(other.saves);
        diff.penalties_saved = penalties_saved.diff(other.penalties_saved);
        diff.own_goals = own_goals.diff(other.own_goals);

        return diff;
    }

    public void set(FootballerScoreDetailElement other) {
        minutes = other.minutes;
        goals_scored = other.goals_scored;
        bonus = other.bonus;
        clean_sheets = other.clean_sheets;
        assists = other.assists;
        yellow_cards = other.yellow_cards;
        red_cards = other.red_cards;
        penalties_missed = other.penalties_missed;
        goals_conceded = other.goals_conceded;
        saves = other.saves;
        penalties_saved = other.penalties_saved;
        own_goals = other.own_goals;
    }
}
