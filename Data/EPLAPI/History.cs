using Newtonsoft.Json;

public class History
{
    public int id { get; set; }//: 2365803,
    public string movement { get; set; }// "new",
    public int points { get; set; }//: 72,
    public int total_points { get; set; }//: 72,
    public int rank { get; set; }//: 727830,
    public int rank_sort { get; set; }//: 774219,
    public int overall_rank { get; set; }//: 727830,
    //targets: null,
    public int event_transfers { get; set; }//: 0,
    public int event_transfers_cost { get; set; }//: 0,
    public int value { get; set; }//: 1000,
    public int points_on_bench { get; set; }//: 11,
    public int bank { get; set; }//: 0,
    public int entry { get; set; }//: 2365803,
    [JsonProperty(PropertyName="event")]
    public int eventId { get; set; }//: 1
}
