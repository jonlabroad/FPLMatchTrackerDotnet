using System;
using System.Threading.Tasks;
using NLog;

public class HighlightProcessor
{
    private int _leagueId;
    private int _gameweek;
    private HighlightCache _highlightCache;
    private Logger _log = LogManager.GetCurrentClassLogger();

    public HighlightProcessor(int gameweek, int leagueId) {
        _highlightCache = new HighlightCache(gameweek);
        _gameweek = gameweek;
        _leagueId = leagueId;
    }

    public async Task Process() {
        try {
            YoutubeClient client = new YoutubeClient();
            var highlights = await client.getHighlights(_gameweek);
            if (highlights != null) {
                if (await _highlightCache.hasChanged(highlights)) {
                    Console.WriteLine("New highlights available!");
                    await new S3JsonWriter().write(
                            string.Format(GlobalConfig.DataRoot + "/highlights/{0}/youtube.json", _gameweek),
                            highlights,
                            true);
                }
            }
        }
        catch (Exception ex) {
            _log.Error(ex);
        }
    }
}
