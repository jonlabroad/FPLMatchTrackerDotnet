using RestSharp;

public class RequestGenerator
{
    public static IRestRequest search(int gameweek) {
        return build(string.Format(ApiUrl.SEARCH_FMT,
                GlobalConfig.YoutubeChannelId,
                createPlaylistTitle(gameweek),
                GlobalConfig.Secrets.googleApiKey));
    }

    public static IRestRequest playlistItems(string playlistId) {
        return build(string.Format(ApiUrl.PLAYLIST_ITEMS_FMT,
                playlistId, GlobalConfig.Secrets.googleApiKey));
    }

    private static string createPlaylistTitle(int gameweek) {
        return string.Format("2018+2019+Premier+League+Season+Matchday+{0}", gameweek);
    }

    private static IRestRequest build(string path) {
        return new RestRequest(path, Method.GET);
    }
}
