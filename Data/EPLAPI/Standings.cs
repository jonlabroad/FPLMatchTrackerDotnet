using System.Collections.Generic;

public class Standings
{
    public bool has_next { get; set; }
    public int number { get; set; }
    public StandingsList standings { get; set; }
    public Matches matches_next { get; set; }
    public Matches matches_this { get; set; }

    public Standings()
    {
    }

    public Standings(NewStandings ns)
    {
        has_next = ns.new_entries.has_next;
        number = ns.new_entries.number;
        standings = new StandingsList();
        standings.results = new List<Standing>();
        foreach (var nr in ns.new_entries.results) {
            Standing standing = new Standing();
            standing.entry = nr.entry;
            standing.id = nr.id;
            standing.player_name = nr.player_first_name + " " + nr.player_last_name;
            standing.entry_name = nr.entry_name;
            standings.results.Add(standing);
        }
    }
}
