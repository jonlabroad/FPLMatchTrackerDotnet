using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CachePreloader
{
    EPLClient _client;

    public CachePreloader(EPLClient client)
    {
        _client = client;
    }

    public async Task PreloadCache()
    {
        var tasks = new List<Task>();
        tasks.Add(_client.getFootballers());
        tasks.Add(_client.getBootstrapStatic());
        await Task.WhenAll(tasks);
    }

    public async Task PreloadEntryCache(IEnumerable<int> teamIds, int gameweek)
    {
        var tasks = teamIds.Select(async id => await _client.getEntry(id)).ToList();
        await Task.WhenAll(teamIds.Select(async id => await _client.getPicks(id, gameweek)).ToList());
        await Task.WhenAll(teamIds.Select(async id => await _client.getHistory(id)).ToList());
        await Task.WhenAll(tasks);
    }
}