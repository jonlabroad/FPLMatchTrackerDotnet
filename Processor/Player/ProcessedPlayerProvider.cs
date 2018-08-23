using System.Linq;
using System.Threading.Tasks;

public class ProcessedPlayerProvider
{
    static readonly string FILENAME = "players.json";

    int _gameweek;
    string _basePath;

    private S3JsonWriter _writer = new S3JsonWriter();
    private S3JsonReader _reader = new S3JsonReader();

    ProcessedPlayerCollection _playerCache = null;

    public ProcessedPlayerProvider(ProcessedPlayerCollection inputPlayers=null) {
        _gameweek = GlobalConfig.CloudAppConfig.CurrentGameWeek;
        _basePath = getBasePath();
        _writer = new S3JsonWriter();
        _reader = new S3JsonReader();
        _playerCache = inputPlayers;
    }

    public async Task<ProcessedPlayer> getPlayer(int id) {
        if (_playerCache == null) {
            await readAllPlayers();
        }

        ProcessedPlayer player = null;
        return _playerCache.players.TryGetValue(id, out player) ? player : null;
    }

    public async Task readAllPlayers() {
        _playerCache = new ProcessedPlayerCollection();
        var keys = await _reader.getKeys(getBasePath());
        foreach (var key in keys) {
            if (key.EndsWith(FILENAME)) {
                ProcessedPlayerCollection players = await _reader.Read<ProcessedPlayerCollection>(key);
                _playerCache.merge(players);
            }
        }
    }

    public async Task writePlayers(ProcessedPlayerCollection players) {
        int[] minMax = getMinMaxIds(players);
        var pathName = getPathName(minMax[0], minMax[1]);
        await _writer.write(pathName, players);
    }

    private int[] getMinMaxIds(ProcessedPlayerCollection players) {
        int[] minMax = new int[2];
        minMax[0] = players.players.Keys.Min();
        minMax[1] = players.players.Keys.Min();
        return minMax;
    }

    private string getPathName(int min, int max) {
        if (GlobalConfig.BinPlayerData) {
            return string.Format("{0}/{1}_{2}/{3}", _basePath, min, max, FILENAME);
        }
        return string.Format("{0}/0/{1}", _basePath, FILENAME);
    }

    private string getBasePath() {
        return $"{GlobalConfig.PlayerDataRoot}/{_gameweek}/bins";
    }
}
