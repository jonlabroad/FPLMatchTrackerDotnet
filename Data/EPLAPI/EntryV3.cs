using Newtonsoft.Json;

public class EntryV3
{
    public int id{ get; set; }
    public string joined_time{ get; set; }
    public int started_event{ get; set; }
    public int favourite_team { get; set; }
    public string player_first_name{ get; set; }
    public string player_last_name{ get; set; }
    public int player_region_id{ get; set; }
    public string player_region_name{ get; set; }
    public string player_region_iso_code_short{ get; set; }
    public string player_region_iso_code_long{ get; set; }

    public int summary_overall_points{ get; set; }
    public int summary_overall_rank{ get; set; }
    public int summary_event_points{ get; set; }
    public int summary_event_rank{ get; set; }
    public int current_event { get; set; }
    public string kit{ get; set; }
    public Kit kitParsed{ get; set; }

    public Leagues leagues { get; set; }

    public void parseKit() {
        if (kit != null)
        {
            kitParsed = JsonConvert.DeserializeObject<Kit>(kit, new JsonSerializerSettings {
                                                            NullValueHandling = NullValueHandling.Ignore
                                                        });
        }
    }
}
