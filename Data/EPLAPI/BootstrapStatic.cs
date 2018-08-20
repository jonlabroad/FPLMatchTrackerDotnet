using System.Collections.Generic;
using Newtonsoft.Json;

public class BootstrapStatic {
    [JsonProperty(PropertyName="current-event")]
    public int currentEvent { get; set; }
    public List<Event> events { get; set; } = new List<Event>();

    // There's a lot more to this but it's not important, yet
}
