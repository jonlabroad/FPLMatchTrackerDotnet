public class ProcessedPick
{
    public ProcessedPlayer footballer {get; set;}
    public Pick pick {get; set;}
    public int score {get; set;} = 0;

    public ProcessedPick() {}

    public ProcessedPick(ProcessedPlayer player, Pick p) {
        footballer = player;
        pick = p;
        if (player.rawData.explains != null) {
            score = new ScoreCalculator().calculateFootballerScore(player.rawData.explains) * pick.multiplier;
        }
    }

    public bool equals(ProcessedPick o) {
        // Ignoring Vice Captainship since we don't really know if it will matter in the end
        return footballer.rawData.footballer.id == o.footballer.rawData.footballer.id &&
                pick.is_captain == o.pick.is_captain &&
                pick.multiplier == o.pick.multiplier;
    }

    public bool isCaptain() {
        return pick.is_captain;
    }

    public bool isViceCaptain() {
        return pick.is_vice_captain;
    }

    public int getMultiplier() {
        return pick.multiplier;
    }
}
