using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class TimelinePredictor {
    private EPLClient _client;
    private int _gameweek;
    private BootstrapStatic _bootstrap;
    private Live _live;
    private List<Fixture> _fixtures;

    public TimelinePredictor(int gameweek, EPLClient client) {
        _client = client;
        _gameweek = gameweek;
    }

    public async Task Init() {
        _bootstrap = await _client.getBootstrapStatic();
        _live = await _client.getLiveData(_gameweek);
        _fixtures = await _client.GetFixtures(_gameweek);
    }

    public async Task<TimelineLiveElement> Predict(TimelineLiveElement current) {
        var prediction = new TimelineLiveElement(current);
        foreach (var fixturePrediction in prediction.explain) {
            var fixtureId = fixturePrediction.fixture;
            var fixtures = await _client.GetFixtures(_gameweek);
            var fixture = fixtures.FirstOrDefault(f => f.id == fixtureId);
            if (fixture != null) {
                MakePrediction(current, fixture, fixturePrediction);
            }

            // Ideas:
            /*
                If fixture is on:
                    If player is playing: +1 (don't bother adding? since it gets added anyway)
                    If player is playing, <60min and could possibly get to 60min, +1
                    If gk/def still has CS (note: goals scored while player is out don't count... check CS rules): +4
                    If mid still has CS: +1
                    Mid/striker +avg, scale down based on time left in fixture, floor it on actual score

                    Potential subs???? Add sub pts (if sub is valid)
                "High" prediction: 
            */
        }
        prediction.CalcScore();
        return prediction;
    }

    private void MakePrediction(TimelineLiveElement current, Fixture fixture, Explain explain) {
        AddPotential(current, fixture, explain);
    }

    private void AddPotential(TimelineLiveElement current, Fixture fixture, Explain explain) {
        var fixtureMinutes = GetEstimatedFixtureMinutes(fixture);
        var minutesExplain = GetMinutes(explain);
        var element = GetElement(current.id);
        
        if (minutesExplain == null) {
            explain.stats.Add(new ExplainElement() {
                identifier = "minutes",
                value = 0,
                points = 0
            });
        }

        // Add minutes (assume > 60min if it is still possible)
        if (minutesExplain.value > 0 && minutesExplain.value < 60) {
            var maxMinutes = (90 - fixtureMinutes) + minutesExplain.value;
            if (maxMinutes > 60) {
                minutesExplain.value = 61;
                minutesExplain.points = 2;
            }
        }
        else if (minutesExplain.value == 0) {
            // No minutes yet
            if (fixture.started && !fixture.finished_provisional) {
                // Fixture is on, how many can we get?
                var maxMinutes = (90 - fixtureMinutes) + minutesExplain.value;
                minutesExplain.value = (int) Math.Round(maxMinutes);
                minutesExplain.points = maxMinutes >= 60 ? 2 : 1;
            }
            else if (!fixture.started) {
                // Fixture hasn't started, assume start
                minutesExplain.value = 90;
                minutesExplain.points = 2;
            }
        }

        // Goalkeepers and defenders get CS, if playing and eligible
        if (IsMidGkOrDef(element)) {
            var csExplain = GetCS(explain);
            if (csExplain == null) {
                if (!HasOtherTeamScored(element, fixture)) {
                    // Player has no CS and is playing... 
                    var maxMinutes = 93.0 - fixtureMinutes + minutesExplain.value;
                    if (maxMinutes >= 60.0) {
                        explain.stats.Add(new ExplainElement() {
                            identifier = "clean_sheets",
                            value = 1,
                            points = element.element_type == 3 ? 1 : 4
                        });
                    }
                }
                else {
                    explain.stats.Add(new ExplainElement() {
                        identifier = "clean_sheets",
                        value = 0,
                        points = 0
                    });
                }
            }
        }

        var currentScore = new ScoreCalculator().calculateFootballerScore(explain);
        var avg = GetAverage(element);
        
        // If fixture has started, scale the average based on minutes played is player is in
        if (fixtureMinutes > 1.0e-6 && minutesExplain != null) {
            avg.points = (int) Math.Round(avg.points * (1.0 - fixtureMinutes/90.0));
            avg.value = 0;
        }

        // Only use average if average is > than the current + potential CS. If using average, only use the difference between avg and current
        if (avg.points > currentScore) {
            // If fixture hasn't started, use the average
            avg.points = avg.points - currentScore;
            explain.stats.Add(avg);
        }
    }

    private Footballer GetElement(int elementId) {
        return _bootstrap.elements.Find(e => e.id == elementId);
    }

    private bool IsMidGkOrDef(Footballer element) {
        return element.element_type == 3 || element.element_type == 2 || element.element_type == 1;
    }

    private bool HasOtherTeamScored(Footballer element, Fixture fixture) {
        var fixtureScoreOther = element.team == fixture.team_h ? fixture.team_a_score : fixture.team_h_score;
        return fixtureScoreOther != 0;
    }

    private ExplainElement GetAverage(Footballer element) {
        var ppgStr = element.points_per_game;
        var ppg = double.Parse(ppgStr);
        return new ExplainElement() {
            identifier = "avg",
            points = (int)Math.Round(ppg),
            value = 1
        };
    }

    private ExplainElement GetMinutes(Explain fixtureExplain) {
        return fixtureExplain.stats.FirstOrDefault(e => e.identifier.Equals("minutes"));
    }

    private ExplainElement GetCS(Explain fixtureExplain) {
        return fixtureExplain.stats.FirstOrDefault(e => e.identifier.Equals("clean_sheets"));
    }

    private bool IsFixtureInProgress(Fixture fixture) {
        return fixture.started && !fixture.finished_provisional;
    }

    private double GetEstimatedFixtureMinutes(Fixture fixture) {
        if (!fixture.started) {
            return 0.0;
        }

        if (fixture.finished_provisional) {
            return 90.0;
        }

        var now = DateTime.UtcNow;
        var culture = CultureInfo.CreateSpecificCulture("en-GB");
        var kickoffTime = DateTime.Parse(fixture.kickoff_time, culture);
        kickoffTime = kickoffTime.ToUniversalTime();
    
        var endTime = kickoffTime.AddMinutes(120);
        var span = now - kickoffTime;
        var estimatedStoppage = 8;
        if (span.TotalMinutes < 45) {
            return span.TotalMinutes;
        }
        else if (span.TotalMinutes >= 45 && span.TotalMinutes < (60 + estimatedStoppage)) {
            return 45;
        }
        else {
            return (45 + (span.TotalMinutes - (60 + estimatedStoppage)));
        }
    }
}