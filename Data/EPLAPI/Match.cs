using Newtonsoft.Json;

public class Match
{
    public int id { get; set; }
    
    [JsonProperty(PropertyName="entry_1_entry")]
    private int _entry_1_entry { get; set; }
    
    [JsonIgnore]
    public int entry_1_entry {
        get {
            return _entry_1_entry;
        } 
        set {
            _entry_1_entry = value;
        }
    }
    public string entry_1_name { get; set; }
    public string entry_1_player_name { get; set; }
    [JsonProperty(PropertyName="entry_2_entry")]
    private int _entry_2_entry;
    [JsonIgnore]
    public int entry_2_entry {
        get {
            return _entry_2_entry;
        } 
        set {
            _entry_2_entry = value;
        }
    }
    public string entry_2_name { get; set; }
    public string entry_2_player_name { get; set; }
    public bool is_knockout { get; set; }
    //winner: null,
    //tiebreak: null,
    public bool own_entry { get; set; }
    public int entry_1_points { get; set; }
    public int entry_1_win { get; set; }
    public int entry_1_draw { get; set; }
    public int entry_1_loss { get; set; }
    public int entry_2_points { get; set; }
    public int entry_2_win { get; set; }
    public int entry_2_draw { get; set; }
    public int entry_2_loss { get; set; }
    public int entry_1_total { get; set; }
    public int entry_2_total { get; set; }
    //seed_value: null,
    [JsonProperty(PropertyName="event")]
    private int _eventId { get; set; }
    public int eventId {
        get {
            return _eventId;
        }
        private set {
            _eventId = value;
        }
     }
}
