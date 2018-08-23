using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class H2hSimulator
{
    protected int _team1Id;
    protected int _team2Id;
    protected EPLClient _client;

    public H2hSimulator(EPLClient client, int team1Id, int team2Id) {
        if (client == null) {
            client = EPLClientFactory.createClient();
        }
        _client = client;
        _team1Id = team1Id;
        _team2Id = team2Id;
    }

    public async Task<IDictionary<int, Record>> simulate() {
        TeamHistory team1History = await getHistory(_team1Id);
        TeamHistory team2History = await getHistory(_team2Id);
        if (team1History == null) {
            team1History = team2History;
        }
        else if (team2History == null) {
            team2History = team1History;
        }
        if (team1History == null || team2History == null)
        {
            return new Dictionary<int, Record>();
        }
        return simulate(team1History, team2History);
    }

    protected IDictionary<int, Record> simulate(TeamHistory team1, TeamHistory team2) {
        Record r1 = new Record();
        Record r2 = new Record();
        var records = new Dictionary<int, Record>();
        records[team1.entry?.id ?? 0] = r1;
        records[team2.entry?.id ?? 0] = r2;
        int minGw = Math.Max(findMinGameweek(team1), findMinGameweek(team2));
        int maxGw = Math.Min(findMaxGameweek(team1), findMaxGameweek(team2));
        for (int gw = minGw; gw <= maxGw; gw++) {
            updateRecords(gw, team1, team2, r1, r2);
        }
        return records;
    }

    protected void updateRecords(int gameweek, TeamHistory team1, TeamHistory team2, Record r1, Record r2) {
        int pts1 = getGameweekPoints(gameweek, team1);
        int pts2 = getGameweekPoints(gameweek, team2);
        if (pts1 > pts2) {
            r1.wins++;
            r2.losses++;
        }
        else if (pts1 < pts2) {
            r1.losses++;
            r2.wins++;
        }
        else if (pts1 == pts2) {
            r1.draws++;
            r2.draws++;
        }
    }

    protected int getGameweekPoints(int gameweek, TeamHistory team) {
        return team.history.Where(h => h.eventId == gameweek).First().points;
    }

    protected int findMinGameweek(TeamHistory team) {
        return team.history.Select(h => h.eventId).Min();
    }

    protected int findMaxGameweek(TeamHistory team) {
        return team.history.Select(h => h.eventId).Max();
    }

    protected async Task<TeamHistory> getHistory(int teamId) {
        return await _client.getHistory(teamId);
    }
}
