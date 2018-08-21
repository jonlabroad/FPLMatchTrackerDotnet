public class ApiUrl
{
    public static string BASE_URL = "https://www.googleapis.com/youtube/v3";

    public static string SEARCH = "/search";
    public static string SEARCH_FMT = BASE_URL + SEARCH + "?part=snippet&channelId={0}&q={1}&type=playlist&key={2}";

    public static string PLAYLIST_ITEMS = "/playlistItems";
    public static string PLAYLIST_ITEMS_FMT = BASE_URL + PLAYLIST_ITEMS + "?part=snippet&maxResults=50&playlistId={0}&key={1}";
}
