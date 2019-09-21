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
        var newTimeline = new GameweekTimeline(previousTimeline, currentLive.elements.Select(el => new LiveElementBase(el)).ToList());

        // Find and log differences between previous timeline and current live
        var diffs = FindAllPlayerDifferences(previousTimeline, currentLive);
        var timestamp = DateTime.Now;

        var prediction = new List<LiveElementBase>();
        var predictor = new TimelinePredictor(gw, _client);
        await predictor.Init();
        foreach (var element in bs.elements) {
            var liveElement = currentLive.elements.FirstOrDefault(e => e.id == element.id) ?? new LiveElementBase() {
                id = element.id
            };
            prediction.Add(await predictor.Predict(liveElement));
        }
        newTimeline.prediction = prediction;
        var predictionDiff = FindAllPlayerPredictionDifferences(previousTimeline != null ? previousTimeline.prediction : new List<LiveElementBase>(), newTimeline.prediction);

        if (diffs.Count > 0 || predictionDiff.Count > 0) {
            newTimeline.timeline.Add(new GameweekTimelineEntry() {
                timestamp = timestamp,
                diff = diffs,
                predictionDiff = predictionDiff
            });
            await WriteTimeline(gw, newTimeline);
        }
    }

    private List<LiveElementBase> FindAllPlayerDifferences(GameweekTimeline prev, Live currLive) {
        var diffs = new List<LiveElementBase>();
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
                diffs.Add(comparison);
            }
        }
        return diffs;
    }

    private List<LiveElementBase> FindAllPlayerPredictionDifferences(List<LiveElementBase> prev, List<LiveElementBase> curr) {
        var diffs = new List<LiveElementBase>();
        foreach (LiveElementBase currLiveElement in curr) {
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
                diffs.Add(comparison);
            }
        }
        return diffs;
    }

    private LiveElementBase getBaseDiff(LiveElementBase curr) {
        var diff = new LiveElementBase() {
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
        
        var anyStats = diff.explain.Any(ex => ex.stats.Count > 0);
        return anyStats ? diff : null;
    }

    private LiveElementBase Compare(LiveElementBase prev, LiveElementBase curr) {
        var diff = new LiveElementBase() {
            id = curr.id
        };
        for (var i = 0; i < curr.explain.Count; i++) {
            var currFixtureExplain = curr.explain[i];
            var prevFixtureExplain = prev.explain.Count > i ? prev.explain[i] : null;
            var diffExplain = new Explain() {
                fixture = currFixtureExplain.fixture,
                stats = new List<ExplainElement>()
            };
            diff.explain.Add(diffExplain);

            foreach (var currStat in currFixtureExplain.stats) {
                var prevStat = prevFixtureExplain.stats.FirstOrDefault(s => s.identifier.Equals(currStat.identifier));
                if (prevStat == null) {
                    prevStat = new ExplainElement() {
                        identifier = currStat.identifier
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
            return diff;
        }

        return null;
    }

    private ExplainElement GetExplainDiff(ExplainElement curr, ExplainElement prev) {
        if (curr.points == prev.points && curr.value == prev.value) {
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