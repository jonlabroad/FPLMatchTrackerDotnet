using System.Collections.Generic;
using Newtonsoft.Json;

public class BootstrapStatic {
    [JsonProperty(PropertyName="current-event")]
    public int currentEvent { get; set; } = 1;
    public List<Event> events { get; set; } = new List<Event>();
    public List<Footballer> elements {get; set;}
    public List<Club> teams { get; set; }
    // There's a lot more to this but it's not important, yet
}
