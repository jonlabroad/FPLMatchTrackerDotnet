using Newtonsoft.Json.Linq;

public class Item
{
    public string kind {get; set;}
    public string etag {get; set;}
    public JObject id {get; set;}
    public Snippet snippet {get; set;}
}
