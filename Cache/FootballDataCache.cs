using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

public class FootballerDataCache
{
    public IDictionary<int, Footballer> footballers = new ConcurrentDictionary<int, Footballer>();
    public IDictionary<int, FootballerDetails> footballerDetails = new ConcurrentDictionary<int, FootballerDetails>();
    public IDictionary<int, Live> liveData = new ConcurrentDictionary<int, Live>();
    public IDictionary<int, EntryData> entries = new ConcurrentDictionary<int, EntryData>();
    public IDictionary<int, TeamHistory> history = new ConcurrentDictionary<int, TeamHistory>();
    public IDictionary<string, Picks> picks = new ConcurrentDictionary<string, Picks>();
    public IDictionary<int, ProcessedLeagueFixtureList> leagueEntriesAndMatches = new ConcurrentDictionary<int, ProcessedLeagueFixtureList>();
    public Standings standings = null;
    public BootstrapStatic bootstrapStatic = null;

    public SemaphoreSlim bootstrapStaticLock = new SemaphoreSlim(1, 1);
    public SemaphoreSlim bootstrapLock = new SemaphoreSlim(1, 1);
    public SemaphoreSlim liveDataLock = new SemaphoreSlim(1, 1);
    public SemaphoreSlim entriesLock = new SemaphoreSlim(1, 1);
    public SemaphoreSlim historyLock = new SemaphoreSlim(1, 1);
    public SemaphoreSlim leagueEntriesLock = new SemaphoreSlim(1, 1);
    public SemaphoreSlim standingsLock = new SemaphoreSlim(1, 1);
    public SemaphoreSlim picksLock = new SemaphoreSlim(1, 1);

    public void clear() {
        footballers.Clear();
        footballerDetails.Clear();
        entries.Clear();
        liveData.Clear();
        leagueEntriesAndMatches = null;
        bootstrapStatic = null;
        standings = null;
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
