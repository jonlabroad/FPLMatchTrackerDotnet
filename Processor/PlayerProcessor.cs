using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

public class PlayerProcessor
{
    private EPLClient _client;
    private int _playerStart = -1;
    private int _playerEnd = -1;
    private Logger _log = LogManager.GetCurrentClassLogger();

    public PlayerProcessor() {
        initialize(EPLClientFactory.createHttpClient());
    }

    public PlayerProcessor(EPLClient client) {
        initialize(client);
    }

    public PlayerProcessor(int start, int end) {
        PlayerProcessorConfig config = PlayerProcessorConfig.getInstance();
        initialize(EPLClientFactory.createHttpClient(config.record, config.recorderSequence));
        _playerStart = start;
        _playerEnd = end;
    }

    public async Task<ProcessedPlayerCollection> process() {
        try {
            // Get all the footballer data required
            var footballers = await getFootballers();
            var players = getFootballersToProcess(footballers);
            var explains = await getLiveExplains(players);

            var provider = new ProcessedPlayerProvider();
            var playerCollection = new ProcessedPlayerCollection();
            foreach (var id in players) {
                var footballer = footballers[id];
                List<FootballerScoreDetailElement> gwExplains = new List<FootballerScoreDetailElement>();
                explains.TryGetValue(id, out gwExplains);
                var liveData = await _client.getLiveData(GlobalConfig.CloudAppConfig.CurrentGameWeek);
                var processor = new SinglePlayerProcessor(provider, GlobalConfig.CloudAppConfig.CurrentGameWeek, footballer, gwExplains, liveData);
                var player = await processor.process();
                playerCollection.players.Add(id, player);
            }
            await provider.writePlayers(playerCollection);
            return playerCollection;
        }
        catch (Exception ex) {
            _log.Error(ex);
        }
        return await Task.FromResult((ProcessedPlayerCollection) null);
    }

    private ICollection<int> getFootballersToProcess(IDictionary<int, Footballer> footballers) {
        ICollection<int> players = new HashSet<int>();
        if (_playerStart < 0) {
            players = footballers.Keys;
        }
        else {
            if (_playerEnd > footballers.Count - 1) {
                _playerEnd = footballers.Count;
            }
            for (int i = _playerStart; i <= _playerEnd; i++) {
                players.Add(i);
            }
        }
        return players;
    }

    private async Task<IDictionary<int, Footballer>> getFootballers()
    {
        return await _client.getFootballers();
    }

    private async Task<IDictionary<int, FootballerDetails>> getDetails(ICollection<int> ids)
    {
        return await _client.getFootballerDetails(ids);
    }

    private async Task<IDictionary<int, List<FootballerScoreDetailElement>>> getLiveExplains(ICollection<int> ids)
    {
        var explains = new Dictionary<int, List<FootballerScoreDetailElement>>();
        var liveData = await _client.getLiveData(GlobalConfig.CloudAppConfig.CurrentGameWeek);
        foreach (int id in ids) {
            LiveElement element;
            if (liveData.elements.TryGetValue(id, out element))
            {
                explains.Add(id, liveData.elements[id].getExplains());
            }
        }
        return explains;
    }

    private void initialize(EPLClient client) {
        _client = client;
    }
}
