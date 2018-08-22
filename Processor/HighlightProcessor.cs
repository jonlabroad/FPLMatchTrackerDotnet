using System;
using System.Threading.Tasks;

public class HighlightProcessor
{
    private int _leagueId;
    private int _gameweek;
    private HighlightCache _highlightCache;

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
            Console.WriteLine(ex);
        }
    }
}
