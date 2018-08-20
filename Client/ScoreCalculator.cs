using System;
using System.Collections.Generic;

public class ScoreCalculator
{
    public Score calculate(Picks picks, List<ProcessedPick> processedPicks, bool benchBoost) {
        // Find the footballers and tally the current score
        Score score = new Score();
        for (int i = 0; i < processedPicks.Count; i++) {
            var isSub = !benchBoost && i >= 11;
            ProcessedPick processedPick = processedPicks[i];
            Pick pick = processedPick.pick;
            var gwExplains = processedPick.footballer.rawData.explains;
            int thisScore = calculateFootballerScore(gwExplains) * pick.multiplier;
            if (!isSub) {
                score.startingScore += thisScore;
            } else {
                score.subScore += thisScore;
            }
        }
        score.startingScore -= picks != null && picks.entry_history != null ? picks.entry_history.event_transfers_cost : 0;
        return score;
    }

    public int calculateFootballerScore(IList<FootballerScoreDetailElement> explains) {
        int score = 0;
        var fields = typeof(FootballerScoreDetailElement).GetProperties();
        foreach (var gwExplain in explains)
        {
            foreach (var field in fields) {
                try
                {
                    ScoreExplain explain = (ScoreExplain) field.GetValue(gwExplain);
                    score += explain.points;
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
        return score;
    }
}