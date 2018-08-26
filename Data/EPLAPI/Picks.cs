using System.Collections.Generic;
using Newtonsoft.Json;

public class Picks
{
    public string active_chip { get; set; }
    [JsonProperty(PropertyName="event")]
    public Event eventData { get; set; }
    public List<Pick> picks { get; set; }
    public EntryHistory entry_history { get; set; }
    public List<AutomaticSub> automatic_subs { get; set; }
}
