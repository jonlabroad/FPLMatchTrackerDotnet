using Newtonsoft.Json;

public class EntryHistory
{
    public int id { get;set; }
    public string movement { get;set; }
    public int points { get;set; }
    public int total_points { get;set; }
    public int rank { get;set; }
    public int rank_sort { get;set; }
    public int overall_rank { get;set; }
    public int event_transfers { get;set; }
    public int event_transfers_cost { get;set; }
    public int value { get;set; }
    public int points_on_bench { get;set; }
    public int bank { get;set; }
    public int entry { get;set; }

    [JsonProperty(PropertyName="event")]
    public int eventId { get;set; }
}
