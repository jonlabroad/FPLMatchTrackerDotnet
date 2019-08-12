using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LeagueScheduleProcessor {
    EPLClient _client;

    public LeagueScheduleProcessor(EPLClient client) {
        _client = client;
    }

    public async Task processH2hLeagues() {
        var entry = await _client.getEntryV3(GlobalConfig.LoginTeamId);
        
        foreach (var league in entry.leagues.h2h) {
            var leagueId = league.id;
            var allLeagueFixtures = new Dictionary<int, List<JObject>>();
            for (int eventId=1; eventId<39; eventId++) {
                var leagueFixtures = getLeagueFixturesJson(eventId, leagueId);
                allLeagueFixtures.Add(eventId, leagueFixtures);
            }
            await new S3JsonWriter().write($"{GlobalConfig.DataRoot}/leagues/{leagueId}/fixtures.json", allLeagueFixtures, true);
        }
    }

    private List<JObject> getLeagueFixturesJson(int eventId, int leagueId) {
        var pyClient = new EPLPythonClient();
        // All pages for a team/league
        var responses = pyClient.leaguesH2hMatches(eventId, leagueId);
        return responses;
    }
}