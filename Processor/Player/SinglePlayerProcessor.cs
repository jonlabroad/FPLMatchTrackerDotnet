using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SinglePlayerProcessor
{
    int _gameweek;
    Footballer _footballer;
    List<FootballerScoreDetailElement> _currentExplains;
    Live _currentLiveData;
    ProcessedPlayer _previousData;
    ProcessedPlayerProvider _playerProvider;

    public SinglePlayerProcessor(ProcessedPlayerProvider playerProvider, int gameweek, Footballer footballer, List<FootballerScoreDetailElement> explains, Live liveData) {
        _footballer = footballer;
        _currentExplains = explains;
        _currentLiveData = liveData;
        _gameweek = gameweek;

        _playerProvider = playerProvider;
    }

    public async Task<ProcessedPlayer> process() {
        _previousData = await _playerProvider.getPlayer(_footballer.id);
        if (_currentExplains == null) {
            Console.WriteLine(string.Format("No explains: {0}\n", _footballer.web_name));
            return _previousData;
        }

        ProcessedPlayer currentPlayerData = new ProcessedPlayer(_footballer, _currentExplains, _previousData);
        determineFixtureStatus(currentPlayerData);
        var prevElements = _previousData != null ?_previousData.rawData.explains : new List<FootballerScoreDetailElement>();
        for (int i = 0; i < _currentExplains.Count; i++) {
            FootballerScoreDetailElement currExplain = _currentExplains[i];
            FootballerScoreDetailElement prevExplain = i < prevElements.Count ? prevElements[i] : null;
            FootballerScoreDetailElement diff = getPlayerDiff(currExplain, prevExplain);

// TODO don't think I need this anymore
//            if (prevExplain != null) {
//                DataFilter filter = new DataFilter(currExplain, prevExplain, diff);
//                diff = filter.filter();
//            }
            addNewEvents(currentPlayerData.events, diff, _footballer, currExplain);
        }

        return currentPlayerData;
    }

    private FootballerScoreDetailElement getPlayerDiff(FootballerScoreDetailElement currentExplain, FootballerScoreDetailElement prevExplain) {
        return currentExplain.compare(prevExplain != null ? prevExplain : null);
    }

    private static void addNewEvents(List<MatchEvent> diff, FootballerScoreDetailElement detailsDiff, Footballer footballer, FootballerScoreDetailElement currentDetail) {
        PlayerEventGenerator generator = new PlayerEventGenerator();
        List<MatchEvent> newEvents = generator.createNewEvents(detailsDiff, footballer, currentDetail);
        newEvents.ForEach(e => diff.Add(e));
    }

    private void determineFixtureStatus(ProcessedPlayer player) {
        player.isCurrentlyPlaying = false;
        player.isDonePlaying = false;
        int realTeamId = _footballer.team;
        foreach (Fixture fixture in _currentLiveData.fixtures) {
            if (fixture.team_a == realTeamId || fixture.team_h == realTeamId) {
                if (isFixtureInProgress(fixture)) {
                    player.isCurrentlyPlaying = true;
                }
                else if (isFixtureComplete(fixture)) {
                    player.isDonePlaying = true;
                }
            }
        }
    }

    private bool isFixtureInProgress(Fixture fixture) {
        return !fixture.finished && fixture.started;
    }

    private bool isFixtureComplete(Fixture fixture) {
        return fixture.finished;
    }
}
