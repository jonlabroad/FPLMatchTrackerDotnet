using Newtonsoft.Json;

public class Entry
{
    public int id{ get; set; }
    public string player_first_name{ get; set; }
    public string player_last_name{ get; set; }
    public int player_region_id{ get; set; }
    public string player_region_name{ get; set; }
    public string player_region_short_iso{ get; set; }
    public int summary_overall_points{ get; set; }
    public int summary_overall_rank{ get; set; }
    public int summary_event_points{ get; set; }
    public int? summary_event_rank{ get; set; }
    public int joined_seconds{ get; set; }
    public int current_event{ get; set; }
    public int total_transfers{ get; set; }
    public int total_loans{ get; set; }
    public int total_loans_active{ get; set; }
    public string transfers_or_loans{ get; set; }
    public bool deleted{ get; set; }
    public bool email{ get; set; }
    public string joined_time{ get; set; }
    public string name{ get; set; }
    public int bank{ get; set; }
    public int value{ get; set; }
    public string kit{ get; set; }
    public Kit kitParsed{ get; set; }
    public int event_transfers{ get; set; }
    public int event_transfers_cost{ get; set; }
    public int extra_free_transfers{ get; set; }
    //public int strategy: null{ get; set; }
    public int? favourite_team{ get; set; }
    public int started_event{ get; set; }
    public int player{ get; set; }

    public void parseKit() {
        if (kit != null)
        {
            kitParsed = JsonConvert.DeserializeObject<Kit>(kit);
        }
    }
}
