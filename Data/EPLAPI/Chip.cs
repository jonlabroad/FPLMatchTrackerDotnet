using Newtonsoft.Json;

public class Chip
{
    public string played_time_formatted { get; set; }
    public string status { get; set; }
    public string name { get; set; }
    public string time { get; set; }
    public int chip { get; set; }
    public int entry { get; set; }
    [JsonProperty(PropertyName="event")]
    public int eventId { get; set; }
}