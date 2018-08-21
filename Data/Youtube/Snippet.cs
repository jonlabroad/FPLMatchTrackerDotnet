using Newtonsoft.Json.Linq;

public class Snippet
{
    public string publishedAt{ get; set; } //"2018-02-24T22:18:25.000Z"
    public string channelId{ get; set; }
    public string title{ get; set; } //2017-2018 Premier League Season: Matchday 28
    public string description{ get; set; }
    public JObject thumbnails{ get; set; }
    public string channelTitle{ get; set; }
    public string liveBroadcastContent{ get; set; }
    public JObject resourceId{ get; set; }
}
