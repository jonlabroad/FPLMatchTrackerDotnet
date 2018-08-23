using System;
using System.Collections.Generic;
using NLog;

public class ScoreCalculator
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public Score calculate(Picks picks, List<ProcessedPick> processedPicks, bool benchBoost)
    {
        // Find the footballers and tally the current score
        Score score = new Score();
        for (int i = 0; i < processedPicks.Count; i++)
        {
            var isSub = !benchBoost && i >= 11;
            ProcessedPick processedPick = processedPicks[i];
            Pick pick = processedPick.pick;
            var gwExplains = processedPick.footballer.rawData.explains;
            int thisScore = calculateFootballerScore(gwExplains) * pick.multiplier;
            if (!isSub)
            {
                score.startingScore += thisScore;
            }
            else
            {
                score.subScore += thisScore;
            }
        }
        score.startingScore -= picks != null && picks.entry_history != null ? picks.entry_history.event_transfers_cost : 0;
        return score;
    }

    public int calculateFootballerScore(IList<FootballerScoreDetailElement> explains)
    {
        int score = 0;
        var fields = typeof(FootballerScoreDetailElement).GetProperties();
        foreach (var gwExplain in explains)
        {
            foreach (var field in fields)
            {
                try
                {
                    ScoreExplain explain = (ScoreExplain)field.GetValue(gwExplain);
                    score += explain.points;
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            }
        }
        return score;
    }

    public static void EstimateAverageScore(IDictionary<int, ProcessedTeam> teams)
    {
        ProcessedTeam average = null;
        average = teams.TryGetValue(0, out average) ? average : null;
        if (average != null)
        {
            int totalScore = 0;
            int numAvgd = 0;
            foreach (ProcessedTeam team in teams.Values)
            {
                if (team.id != 0)
                {
                    totalScore += team.score.startingScore + team.transferCost;
                    numAvgd++;
                }
            }
            int avgScore = totalScore / numAvgd;
            average.score.startingScore = avgScore;
        }
    }
}