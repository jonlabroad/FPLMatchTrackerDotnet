using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TimelinePlayerProcessor {
    private EPLClient _client;

    private S3JsonReader _reader = new S3JsonReader();
    private S3JsonWriter _writer = new S3JsonWriter();

    public TimelinePlayerProcessor(EPLClient client) {
        _client = client;
    }

    public async Task Process() {
        var bs = await _client.getBootstrapStatic();
        var currEvent = bs.events.FirstOrDefault(ev => ev.is_current);
        var gw = currEvent.id;
        var currentLive = await _client.getLiveData(gw);
        var previousTimeline = await ReadPreviousTimeline(gw);
        var newTimeline = new GameweekTimeline(previousTimeline, currentLive.elements.Select(el => new TimelineLiveElement(el)).ToList());

        // Find and log differences between previous timeline and current live
        var diffs = FindAllPlayerDifferences(previousTimeline, currentLive);
        var timestamp = DateTime.Now;

        var prediction = new List<TimelineLiveElement>();
        var predictor = new TimelinePredictor(gw, _client);
        await predictor.Init();
        foreach (var element in bs.elements) {
            var liveElementBase = currentLive.elements.FirstOrDefault(e => e.id == element.id);
            var liveElement = liveElementBase != null ? new TimelineLiveElement(liveElementBase) : new TimelineLiveElement() { id = element.id };
            prediction.Add(await predictor.Predict(liveElement));
        }
        newTimeline.prediction = prediction;
        var predictionDiff = FindAllPlayerPredictionDifferences(previousTimeline != null ? previousTimeline.prediction : new List<TimelineLiveElement>(), newTimeline.prediction);

        Console.WriteLine($"Diffs: {diffs.Count}");
        Console.WriteLine($"PredictionDiffs: {predictionDiff.Count}");
        if (diffs.Count > 0 || predictionDiff.Count > 0) {
            newTimeline.timeline.Add(new GameweekTimelineEntry() {
                timestamp = timestamp,
                diff = diffs,
                predictionDiff = predictionDiff
            });
            await WriteTimeline(gw, newTimeline);
        }
    }

    private List<TimelineLiveElement> FindAllPlayerDifferences(GameweekTimeline prev, Live currLive) {
        var diffs = new List<TimelineLiveElement>();
        foreach (LiveElementBase currLiveElement in currLive.elements) {
            var elementId = currLiveElement.id;
            var comparison = currLiveElement;
            if (prev != null) {
                var prevLiveElement = prev.liveElements.FirstOrDefault(p => p.id == elementId);
                comparison = Compare(prevLiveElement, currLiveElement);
            }
            else {
                comparison = getBaseDiff(currLiveElement);
            }
            if (comparison != null) {
                diffs.Add(new TimelineLiveElement(currLiveElement));
            }
        }
        return diffs;
    }

    private List<TimelineLiveElement> FindAllPlayerPredictionDifferences(List<TimelineLiveElement> prev, List<TimelineLiveElement> curr) {
        var diffs = new List<TimelineLiveElement>();
        foreach (TimelineLiveElement currLiveElement in curr) {
            var elementId = currLiveElement.id;
            var comparison = currLiveElement;
            if (prev != null) {
                var prevLiveElement = prev.FirstOrDefault(p => p.id == elementId);
                comparison = Compare(prevLiveElement, currLiveElement);
            }
            else {
                comparison = getBaseDiff(currLiveElement);
            }
            if (comparison != null) {
                diffs.Add(currLiveElement);
            }
        }
        return diffs;
    }

    private TimelineLiveElement getBaseDiff(LiveElementBase curr) {
        var diff = new TimelineLiveElement() {
            id = curr.id
        };

        var diffExplains = new List<Explain>();
        foreach (var explain in curr.explain) {
            var diffExplain = new Explain() {fixture = explain.fixture}; 
            diff.explain.Add(diffExplain);
            var diffStats = new List<ExplainElement>();
            foreach (var stat in explain.stats) {
                if (stat.points != 0 || stat.value != 0) {
                    diffStats.Add(stat);
                }
            }
            if (diffStats.Count > 0) {
                diffExplain.stats = diffStats;
            }
        }
        diff.CalcScore();
        var anyStats = diff.explain.Any(ex => ex.stats.Count > 0);
        return anyStats ? diff : null;
    }

    private TimelineLiveElement Compare(LiveElementBase prev, LiveElementBase curr) {
        var diff = new TimelineLiveElement() {
            id = curr.id
        };

        for (var i = 0; i < curr.explain.Count; i++) {
            var allExplainIdentifiers = new HashSet<string>(curr.explain[i].stats.Select(s => s.identifier).ToHashSet());
            if (prev != null && prev.explain != null && prev.explain.Count > i) {
                foreach (var stat in prev.explain[i].stats) {
                    allExplainIdentifiers.Add(stat.identifier);
                }
            }

            var currFixtureExplain = curr.explain[i];
            var prevFixtureExplain = prev != null && prev.explain != null && prev.explain.Count > i ? prev.explain[i] : null;
            var diffExplain = new Explain() {
                fixture = currFixtureExplain.fixture,
                stats = new List<ExplainElement>()
            };
            diff.explain.Add(diffExplain);

            foreach (var explainIdentifier in allExplainIdentifiers) {
                var currStat = currFixtureExplain.stats.FirstOrDefault(e => e.identifier.Equals(explainIdentifier));
                var prevStat = prevFixtureExplain != null ? prevFixtureExplain.stats.FirstOrDefault(s => s.identifier.Equals(explainIdentifier)) : null;
                if (prevStat == null) {
                    prevStat = new ExplainElement() {
                        identifier = explainIdentifier
                    };
                }
                if (currStat == null) {
                    currStat = new ExplainElement() {
                        identifier = explainIdentifier
                    };
                }
                var statDiff = GetExplainDiff(currStat, prevStat);
                if (statDiff != null) {
                    diffExplain.stats.Add(statDiff);
                }
            }
        }

        var saveDiff = diff.explain.Any(ex => ex.stats.Count > 0);
        if (saveDiff) {
            diff.CalcScore();
            return diff;
        }

        return null;
    }

    private ExplainElement GetExplainDiff(ExplainElement curr, ExplainElement prev) {
        // Ignore value, only get diff is points have changed
        if (curr.points == prev.points) {
            return null;
        }
        
        var diff = new ExplainElement() {
            identifier = curr.identifier
        };
        diff.points = curr.points - prev.points;
        diff.value = curr.value - prev.value;
        return diff;
    }

    private void MakeRandomChange(Live live) {
        var rand = new Random();
        var i = rand.Next(0, live.elements.Count);
        var j = rand.Next(0, live.elements[i].explain.Count);
        var stats = live.elements[i].explain[j].stats;
        var k = rand.Next(0, stats.Count);
        var explain = stats[k];

        var incValue = rand.Next(1, 5);
        var incPoints = rand.Next(1,6);
        explain.value += incValue;
        explain.points += incPoints;
    }

    private async Task WriteTimeline(int gw, GameweekTimeline timeline) {
        await _writer.write(GetS3Key(gw), timeline, true);
    }

    private async Task<GameweekTimeline> ReadPreviousTimeline(int gw) {
        return await _reader.Read<GameweekTimeline>(GetS3Key(gw));
    }

    private string GetS3Key(int gw) {
        return $"{GlobalConfig.DataRoot}/players/{gw}/GameweekTimeline.json";
    }
}