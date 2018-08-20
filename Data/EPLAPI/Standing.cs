public class Standing
{
    public int id { get; set; }
    public string entry_name { get; set; }
    public string player_name { get; set; }
    public string movement { get; set; }
    public bool own_entry { get; set; }
    public int rank { get; set; }
    public int last_rank { get; set; }
    public int rank_sort { get; set; }
    public int total { get; set; }
    public int matches_played { get; set; }
    public int matches_won { get; set; }
    public int matches_drawn { get; set; }
    public int matches_lost { get; set; }
    public int points_for { get; set; }
    public int points_against { get; set; }
    public int points_total { get; set; }
    public int division { get; set; }
    public int? entry { get; set; }

    public Standing()
    {

    }

    public Standing(Standing other) {
        this.id = other.id;
        this.entry_name = other.entry_name;
        this.player_name = other.player_name;
        this.movement = other.movement;
        this.own_entry = other.own_entry;
        this.rank = other.rank;
        this.last_rank = other.last_rank;
        this.rank_sort = other.rank_sort;
        this.total = other.total;
        this.matches_played = other.matches_played;
        this.matches_won = other.matches_won;
        this.matches_drawn = other.matches_drawn;
        this.matches_lost = other.matches_lost;
        this.points_for = other.points_for;
        this.points_against = other.points_against;
        this.points_total = other.points_total;
        this.division = other.division;
        this.entry = other.entry;
    }
}
