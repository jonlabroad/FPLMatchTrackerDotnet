using System.Collections.Generic;

public class FootballerDataCache
{
    public IDictionary<int, Footballer> footballers = new Dictionary<int, Footballer>();
    public IDictionary<int, FootballerDetails> footballerDetails = new Dictionary<int, FootballerDetails>();
    public IDictionary<int, Live> liveData = new Dictionary<int, Live>();
    //public IDictionary<int, EntryData> entries = new Dictionary<int, EntryData>();
    //public IDictionary<int, TeamHistory> history = new Dictionary<int, TeamHistory>();
    //public IDictionary<int, ProcessedLeagueFixtureList> leagueEntriesAndMatches = new Dictionary<int, ProcessedLeagueFixtureList>();
    //public Standings standings = null;
    //public BootstrapStatic bootstrapStatic = null;

    public void clear() {
        footballers.Clear();
        footballerDetails.Clear();
        //entries.Clear();
        liveData.Clear();
        //leagueEntriesAndMatches = null;
        //bootstrapStatic = null;
        //standings = null;
    }

    public Footballer getFootballer(int id) {
        Footballer footballer = null;
        var t = footballers.TryGetValue(id, out footballer) ? footballer : null;
        return footballer;
    }

    public FootballerDetails getDetails(int id) {
        FootballerDetails details;
        var t = footballerDetails.TryGetValue(id, out details) ? details : null;
        return details;
    }

    public void setFootballers(ICollection<Footballer> footballersArray) {
        foreach (var footballer in footballersArray) {
            footballers.Add(footballer.id, footballer);
        }
    }
}
