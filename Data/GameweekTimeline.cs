using System;
using System.Collections.Generic;

public class GameweekTimelineEntry {
    public GameweekTimelineEntry() {
        timestamp = new DateTime();
    }
    
    public DateTime timestamp {get; set;}
    public List<TimelineLiveElement> diff {get; set;} = new List<TimelineLiveElement>();
    public List<TimelineLiveElement> predictionDiff {get;set;} = new List<TimelineLiveElement>();
}

public class GameweekTimeline {
    public GameweekTimeline() {
    }

    public GameweekTimeline(GameweekTimeline prev, List<TimelineLiveElement> le) {
        liveElements = le;
        if (prev != null) {
            timeline = prev.timeline;
        }
    }

    public bool isStarted { get; set; } = false;
    public bool isFinished { get; set; } = false;
    public List<TimelineLiveElement> liveElements {get; set;} = new List<TimelineLiveElement>();
    public List<TimelineLiveElement> prediction { get; set;}
    public List<GameweekTimelineEntry> timeline {get; set;} = new List<GameweekTimelineEntry>();
}