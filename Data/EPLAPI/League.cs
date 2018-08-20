public class League
{
    public int id { get; set; }
    //leagueban_set: [ ],
    public string name { get; set; }
    public bool has_started { get; set; }
    public bool can_delete { get; set; }
    //short_name: null,
    public string created { get; set; }
    public bool closed { get; set; }
    public bool forum_disabled { get; set; }
    public bool make_code_public { get; set; }
    //rank: null,
    //size: null,
    public string league_type { get; set; }
    public string _scoring { get; set; }
    public int ko_rounds { get; set; }
    public int admin_entry { get; set; }
    public int start_event { get; set; }
}
