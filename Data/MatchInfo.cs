using System.Collections.Generic;

public class MatchInfo
{
    public int gameweek { get; set; }
    public IDictionary<int, ProcessedMatchTeam> teams { get; set; } = new Dictionary<int, ProcessedMatchTeam>();
    public List<TeamMatchEvent> allEvents  { get; set; } = new List<TeamMatchEvent>();
    public HashSet<int> differentials { get; set; } = new HashSet<int>();
    public IDictionary<int, Fixture> fixtures { get; set; } = new Dictionary<int, Fixture>();
    public IDictionary<int, Record> simulatedH2h { get; set; } = new Dictionary<int, Record>();

    // The only reason this is here is because I'm lazy. It should instead be part of a league-wide structure
    public LiveStandings liveStandings  { get; set; } = null;

    public MatchInfo() {}

    public MatchInfo(int gw, List<TeamMatchEvent> events, ProcessedMatchTeam team1, ProcessedMatchTeam team2, IDictionary<int, Fixture> fix, Record t1H2h, Record t2H2h) {
        gameweek = gw;
        teams[team1.id] = team1;
        teams[team2.id] = team2;
        allEvents = events;
        differentials = new DifferentialFinder(team1, team2).find();
        fixtures = fix;
        simulatedH2h = new Dictionary<int, Record>();
        simulatedH2h[team1.id] = t1H2h;
        simulatedH2h[team2.id] = t2H2h;
    }
}
