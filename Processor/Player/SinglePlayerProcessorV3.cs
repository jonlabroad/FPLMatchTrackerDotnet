using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

public class SinglePlayerProcessorV3
{
    int _gameweek;
    Footballer _footballer;
    List<FootballerScoreDetailElement> _currentExplains;
    Live _currentLiveData;
    ProcessedPlayer _previousData;
    ProcessedPlayerProviderV3 _playerProvider;
    private static Logger _log = LogManager.GetCurrentClassLogger();

    public SinglePlayerProcessorV3(ProcessedPlayerProviderV3 playerProvider, int gameweek, Footballer footballer, List<FootballerScoreDetailElement> explains, Live liveData) {
        _footballer = footballer;
        _currentExplains = explains;
        _currentLiveData = liveData;
        _gameweek = gameweek;

        _playerProvider = playerProvider;
    }

    public async Task<ProcessedPlayer> process() {
        _previousData = await _playerProvider.getPlayer(_footballer.id);
        if (_currentExplains == null) {
            _log.Warn(string.Format("No explains: {0}\n", _footballer.web_name));
            return _previousData;
        }

        ProcessedPlayer currentPlayerData = new ProcessedPlayer(_footballer, _currentExplains, _previousData);
        determineFixtureStatus(currentPlayerData);
        var prevElements = _previousData != null && _previousData.rawData != null ?_previousData.rawData.explain : new List<FootballerScoreDetailElement>();
        List<FootballerScoreDetailElement> currExplains = _currentExplains;
        for (var i=0; i<currExplains.Count; i++) {
            FootballerScoreDetailElement prevExplain = i < prevElements.Count ? prevElements[i] : null;
            FootballerScoreDetailElement diff = getPlayerDiff(currExplains[i], prevExplain);
            addNewEvents(currentPlayerData.events, diff, _footballer, currExplains[i]);
        }

        return currentPlayerData;
    }

    private FootballerScoreDetailElement getPlayerDiff(FootballerScoreDetailElement currentExplain, FootballerScoreDetailElement prevExplain) {
        return currentExplain.compare(prevExplain != null ? prevExplain : null);
    }

    private static void addNewEvents(List<MatchEvent> diff, FootballerScoreDetailElement detailsDiff, Footballer footballer, FootballerScoreDetailElement currentDetail) {
        List<MatchEvent> newEvents = PlayerEventGenerator.createNewEvents(detailsDiff, footballer, currentDetail);
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
        //var now = DateTime.Now;
        return !fixture.finished_provisional && fixture.started;
    }

    private bool isFixtureComplete(Fixture fixture) {
        return fixture.finished;
    }
}
