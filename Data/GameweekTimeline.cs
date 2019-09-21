using System;
using System.Collections.Generic;

public class GameweekTimelineEntry {
    public GameweekTimelineEntry() {
        timestamp = new DateTime();
    }
    
    public DateTime timestamp {get; set;}
    public List<LiveElementBase> diff {get; set;} = new List<LiveElementBase>();
    public List<LiveElementBase> predictionDiff {get;set;} = new List<LiveElementBase>();
}

public class GameweekTimeline {
    public GameweekTimeline() {
    }

    public GameweekTimeline(GameweekTimeline prev, List<LiveElementBase> le) {
        liveElements = le;
        if (prev != null) {
            timeline = prev.timeline;
        }
    }

    public bool isStarted { get; set; } = false;
    public bool isFinished { get; set; } = false;
    public List<LiveElementBase> liveElements {get; set;} = new List<LiveElementBase>();
    public List<LiveElementBase> prediction { get; set;}
    public List<GameweekTimelineEntry> timeline {get; set;} = new List<GameweekTimelineEntry>();
}