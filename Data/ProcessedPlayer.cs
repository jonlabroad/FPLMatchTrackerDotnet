using System.Collections.Generic;
using System.Linq;

public class ProcessedPlayer
{
    public FullFootballerData rawData {get; set;} = new FullFootballerData();
    public List<MatchEvent> events {get; set;} = new List<MatchEvent>();

    public bool isCurrentlyPlaying  {get; set;} = false;
    public bool isDonePlaying  {get; set;} = false;

    public ProcessedPlayer() {}

    public ProcessedPlayer(Footballer footballer, List<FootballerScoreDetailElement> explains, ProcessedPlayer oldData) {
        rawData.footballer = footballer;
        rawData.explain = explains;
        if (oldData != null) {
            oldData.events.ForEach(e => events.Add(e));
        }
    }
}
