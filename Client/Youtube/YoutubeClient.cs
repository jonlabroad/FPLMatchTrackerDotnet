using System.Threading.Tasks;
using Newtonsoft.Json;

public class YoutubeClient
{
    RequestExecutor executor = new RequestExecutor(GlobalConfig.EplBaseUrl);

    public async Task<Item> getPlaylist(int gameweek) {
        var request = RequestGenerator.search(gameweek);
        var response = await executor.Execute(request);
        SearchList list = JsonConvert.DeserializeObject<SearchList>(response, new JsonSerializerSettings {
                                                            NullValueHandling = NullValueHandling.Ignore
                                                        });
        return findItem(list);
    }

    public async Task<PlaylistItem[]> getHighlights(int gameweek)
    {
        var playlist = await getPlaylist(gameweek);
        if (playlist != null && playlist.id != null)
        {
            var request = RequestGenerator.playlistItems(playlist.id.GetValue("playlistId").ToString());
            var response = await executor.Execute(request);
            var playlistItems = JsonConvert.DeserializeObject<PlaylistItems>(response, new JsonSerializerSettings {
                                                            NullValueHandling = NullValueHandling.Ignore
                                                        });
            return playlistItems.items;
        }
        return null;
    }

    private Item findItem(SearchList list) {
        if (list.items.Length > 0) {
            return list.items[0];
        }
        return null;
    }
}
