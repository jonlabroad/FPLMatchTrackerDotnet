using Newtonsoft.Json;

public class AutomaticSub
{
    public int id { get; set; }
    
    public int element_in { get; set; }
    
    public int element_out { get; set; }
    
    public int entry { get; set; }
    
    [JsonProperty(PropertyName="event")]
    public int eventId { get; set; }
}