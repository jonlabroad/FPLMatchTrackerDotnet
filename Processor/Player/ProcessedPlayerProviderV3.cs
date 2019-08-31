using System.Linq;
using System.Threading.Tasks;

public class ProcessedPlayerProviderV3
{
    static readonly string FULL_FILENAME = "players_full.json";
    static readonly string API_FILENAME = "players.json";

    int _gameweek;
    string _basePath;

    private S3JsonWriter _writer = new S3JsonWriter();
    private S3JsonReader _reader = new S3JsonReader();

    ProcessedPlayerCollection _playerCache = null;

    public ProcessedPlayerProviderV3(int gameweek, ProcessedPlayerCollection inputPlayers=null) {
        _gameweek = gameweek;
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
            if (key.EndsWith(FULL_FILENAME)) {
                ProcessedPlayerCollection players = await _reader.Read<ProcessedPlayerCollection>(key);
                _playerCache.merge(players);
            }
        }
    }

    public async Task writeFullPlayers(ProcessedPlayerCollection players) {
        var pathName = getFullPathName();
        await _writer.write(pathName, players, true);
    }

    public async Task writeApiPlayers(ProcessedPlayerCollection players) {
        var pathName = getApiPathName();
        await _writer.write(pathName, players.copyApi(), true);
    }

    private string getFullPathName() {
        return string.Format("{0}/{1}", _basePath, FULL_FILENAME);
    }

    private string getApiPathName() {
        return string.Format("{0}/{1}", _basePath, API_FILENAME);
    }

    private string getBasePath() {
        return $"{GlobalConfig.PlayerDataRoot}/{_gameweek}";
    }
}
