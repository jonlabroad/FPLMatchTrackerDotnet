using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EPLClient
{
    private EPLRequestGenerator _generator;
    private IRequestExecutor _executor;
    private FootballerDataCache _footballerCache;

    public EPLClient(IRequestExecutor executor)  {
        initialize(executor);
    }

    public async Task<IDictionary<int, Footballer>> getFootballers()
    {
        if (_footballerCache.footballers.Count <= 0) {
            var request = _generator.GenerateFootballersRequest();
            Bootstrap bootstrap = await _executor.Execute<Bootstrap>(request);
            _footballerCache.setFootballers(bootstrap.elements);
        }
        return _footballerCache.footballers;
    }

    public async Task<Bootstrap> getBootstrap() {
        //if (_footballerCache.footballers.size() <= 0) {
            var request = _generator.GenerateFootballersRequest();
            var bootstrap = await _executor.Execute<Bootstrap>(request);
            //_footballerCache.setFootballers(bootstrap.elements);
        //}
        return bootstrap;
    }

    public async Task<IDictionary<int, FootballerDetails>> getFootballerDetails(ICollection<int> ids)
    {
        return await readFootballerDetails(ids);
    }

    public async Task<IDictionary<int, FootballerDetails>> readFootballerDetails(ICollection<int> ids)
    {
        foreach (var id in ids) {
            FootballerDetails detail = getCachedDetails(id);
            if (detail == null) {
                _footballerCache.footballerDetails.Add(id, await readFootballerDetails(id));
            }
        }
        return _footballerCache.footballerDetails;
    }

    public async Task<FootballerDetails> readFootballerDetails(int footballerId)
    {
        var request = _generator.GenerateFootballerDetailRequest(footballerId);
        FootballerDetails details = await _executor.Execute<FootballerDetails>(request);
        return details;
    }

    public async Task<Live> getLiveData(int eventId) {
        if (!_footballerCache.liveData.ContainsKey(eventId)) {
            Live data = await readLiveEventData(eventId);
            _footballerCache.liveData.Add(eventId, data);
        }
        return _footballerCache.liveData[eventId];
    }

    public async Task<Live> readLiveEventData(int eventId) {
        var request = _generator.GenerateLiveDataRequest(eventId);
        var liveString = await _executor.Execute(request);
        var live = JsonConvert.DeserializeObject<Live>(liveString);
        foreach (var fixture in live.fixtures) {
            fixture.parsedStats = fixture.getStats();
        }
        return live;
    }

    private FootballerDetails getCachedDetails(int id) {
        FootballerDetails data;
        return _footballerCache.footballerDetails.TryGetValue(id, out data) ? data : null;
    }

    private void initialize(IRequestExecutor executor)
    {
        _generator = new EPLRequestGenerator();
        _executor = executor;
        _footballerCache = new FootballerDataCache();
    }
}
