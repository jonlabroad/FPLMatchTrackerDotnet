using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UltimateH2h
{
    private int _leagueId;

    public UltimateH2h(int leagueId)
    {
        _leagueId = leagueId;
    }

    public async Task Calculate() {
        var client = EPLClientFactory.createClient();
        var standings = await client.getStandings(_leagueId);
        var records = new Dictionary<int, Record>();
        foreach (var standing in standings.standings.results) {
            var newRecord = new Record();
            newRecord.teamName = standing.entry_name;
            newRecord.teamId = standing.entry;
            records[standing.entry] = newRecord;
        }

        foreach (var standing in standings.standings.results) {
            foreach (Standing opp in standings.standings.results) {
                if (standing.entry == opp.entry) {
                    continue;
                }

                H2hSimulator simulator = new H2hSimulator(client, standing.entry, opp.entry);
                var rec = await simulator.simulate();
                foreach (var teamId in rec.Keys) {
                    records[teamId].wins += rec[teamId].wins;
                    records[teamId].draws += rec[teamId].draws;
                    records[teamId].losses += rec[teamId].losses;
                }
            }
        }

        var recList = new List<Record>(records.Values.ToList());
        recList.Sort();
        int place = 1;
        foreach (var rec in recList) {
            Standing standing = null;
            foreach (var s in standings.standings.results) {
                if (s.entry == rec.teamId) {
                    standing = s;
                    break;
                }
            }

            Console.WriteLine(string.Format("{0}. {1}: {2}W-{3}D-{4}L {5} {6}", place, rec.teamName, rec.wins, rec.draws, rec.losses, rec.getPoints(), place - standing.rank));
            place++;
        }
    }
}