using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MatchProcessor
{
    protected EPLClient _client;

    protected IDictionary<int, ProcessedTeam> _teams;
    protected Match _match;
    protected int _leagueId = -1;

    protected MatchInfo _result = null;

    public MatchProcessor(EPLClient client, int leagueId, IDictionary<int, ProcessedTeam> teams, Match match) {
        _teams = teams;
        _match = match;
        _leagueId = leagueId;
        _client = client != null ? client : EPLClientFactory.createClient();
    }

    public MatchProcessor(IDictionary<int, ProcessedTeam> teams, int leagueId) {
        _teams = teams;
        _leagueId = leagueId;

        _client = EPLClientFactory.createClient();
    }

    public async Task<MatchInfo> process() {
        Standings standings = _leagueId > 0 ? await _client.getStandings(_leagueId) : null;

        ProcessedTeam pTeam1, pTeam2;
        var v = _teams.TryGetValue(_match.entry_1_entry, out pTeam1) ? pTeam1 : null;
        v = _teams.TryGetValue(_match.entry_2_entry, out pTeam2) ? pTeam2 : null;

        if (pTeam1 == null) {
            pTeam1 = pTeam2;
        }

        if (pTeam2 == null) {
            pTeam2 = pTeam1;
        }

        var team1 = new ProcessedMatchTeam(pTeam1, getStanding(standings, _match.entry_1_entry));
        var team2 = new ProcessedMatchTeam(pTeam2, getStanding(standings, _match.entry_2_entry));

        H2hSimulator h2hSim = new H2hSimulator(_client, _match.entry_1_entry, _match.entry_2_entry);
        IDictionary<int, Record> h2hResults = new Dictionary<int, Record>();
        try {
            h2hResults = await h2hSim.simulate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        var sharedEvents = new MatchEventDeduplicator().deduplicate(team1, team2);
        sharedEvents.Sort(new MatchEventSortComparator());
        _result = await createMatchInfo(_match, sharedEvents, team1, team2, h2hResults);
        return _result;
    }

    public MatchInfo getResult() {
        return _result;
    }

    protected Standing getStanding(Standings standings, int teamId) {
        if (standings == null) {
            return null;
        }

        foreach (var standing in standings.standings.results) {
            if (standing.entry == teamId) {
                return standing;
            }
        }
        return null;
    }

    protected async Task<IDictionary<int, Fixture>> getFixtures(int gameweek) {
        Live liveData = await _client.getLiveData(gameweek);
        if (liveData != null) {
            return organizeFixtures(liveData);
        }
        return new Dictionary<int, Fixture>();
    }

    protected IDictionary<int, Fixture> organizeFixtures(Live liveData) {
        return new Dictionary<int, Fixture>(); //TODO
    }

    protected async Task<MatchInfo> createMatchInfo(Match match, List<TeamMatchEvent> events, ProcessedMatchTeam team1, ProcessedMatchTeam team2, IDictionary<int, Record> h2hSim) {
        Record h2h1 = null, h2h2 = null;
        h2hSim.TryGetValue(team1.id, out h2h1);
        h2hSim.TryGetValue(team1.id, out h2h2);
        MatchInfo info = new MatchInfo(match.eventId, events, team1, team2, await getFixtures(match.eventId), h2h1, h2h2);
        return info;
    }

    public static async Task writeMatchInfo(int leagueId, MatchInfo info) {
        foreach (var id in info.teams.Keys)
        {
            Console.WriteLine(string.Format("Writing data for {0}\n", id));
            if (leagueId > 0) {
                await new MatchInfoProvider(leagueId).writeCurrent(id, info);
            }
            else {
                await new MatchInfoProvider(leagueId).writeCup(id, info);
            }
        }
    }
}
