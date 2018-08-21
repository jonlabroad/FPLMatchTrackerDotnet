using System.Collections.Generic;
using Newtonsoft.Json;

public class EventInfo
{
    [JsonProperty(PropertyName="event")]
    public int eventId {get; set;}
    public List<Fixture> fixtures {get; set;}
    public List<Club> clubs {get; set;}
}
