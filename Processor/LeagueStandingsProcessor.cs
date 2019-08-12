using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class LeagueStandingsProcessor {
    private EPLClient _client;
    
    public LeagueStandingsProcessor(EPLClient client) {
        _client = client;
    }

    public async Task process() {
        var entry = await _client.getEntryV3(GlobalConfig.LoginTeamId);
        foreach (var league in entry.leagues.h2h) {
            var leagueId = league.id;
            var standings = await getLeagueH2hStandingsJson(leagueId);
            await new S3JsonWriter().write($"{GlobalConfig.DataRoot}/leagues/{leagueId}/standings.json", standings, true);
        }
    }

    private async Task<JObject> getLeagueH2hStandingsJson(int leagueId) {
        return await Task.Run(() => {
            var pyClient = new EPLPythonClient();
            var response = pyClient.leaguesH2hStandings(leagueId);
            return response;
        });
    }
}