using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Python.Runtime;

public class EPLPythonClient {
    public List<JObject> leaguesH2hMatches(int eventId, int leagueId) {
        int page = 1;
        var results = new List<JObject>();
        while(true) {
            Console.WriteLine($"https://fantasy.premierleague.com/api/leagues-h2h-matches/league/{leagueId}/?page={page}&event={eventId}");
            var result = ShellHelper.PythonScript($"Python/leaguesH2hMatches.py {GlobalConfig.Secrets.fplEmail} {GlobalConfig.Secrets.fplPassword} {eventId} {leagueId} {page}");
            Console.WriteLine(result);
            var parsed = JsonConvert.DeserializeObject<LeaguesH2hMatches>(result);
            foreach (var jObj in parsed.results) {
                var resultObj = (JObject)jObj;
                results.Add(resultObj);
            }

            if (!parsed.has_next) {
                break;
            }
        }
        return results;
    }

    public JObject leaguesH2hStandings(int leagueId) {
        var page = 1;
        var result = ShellHelper.PythonScript($"Python/leaguesH2hStandings.py  {GlobalConfig.Secrets.fplEmail} {GlobalConfig.Secrets.fplPassword} {leagueId} {page}");
        var jObj = JObject.Parse(result);
        return jObj;
    }
}