using System;
using System.Collections.Generic;

public class GameweekTimelineEntry {
    public GameweekTimelineEntry() {
        timestamp = new DateTime();
    }
    
    public DateTime timestamp {get; set;}
    public List<LiveElement> diff {get; set;} = new List<LiveElement>();
}

public class GameweekTimeline {
    public GameweekTimeline() {
    }

    public GameweekTimeline(GameweekTimeline prev, List<LiveElement> le) {
        liveElements = le;
        if (prev != null) {
            timeline = prev.timeline;
        }
    }

    public bool isStarted { get; set; } = false;
    public bool isFinished { get; set; } = false;
    public List<LiveElement> liveElements {get; set;} = new List<LiveElement>();
    public List<GameweekTimelineEntry> timeline {get; set;} = new List<GameweekTimelineEntry>();
}