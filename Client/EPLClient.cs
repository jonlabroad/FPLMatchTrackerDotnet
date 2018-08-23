using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

public class EPLClient
{
    private EPLRequestGenerator _generator;
    private IRequestExecutor _executor;
    private FootballerDataCache _footballerCache;
    private Logger _log = LogManager.GetCurrentClassLogger();

    public EPLClient(IRequestExecutor executor)  {
        initialize(executor);
    }

    public async Task<IDictionary<int, Footballer>> getFootballers()
    {
        if (_footballerCache.footballers.Count <= 0) {
            await _footballerCache.bootstrapLock.WaitAsync();
            if (_footballerCache.footballers.Count <= 0) {
                var request = _generator.GenerateFootballersRequest();
                Bootstrap bootstrap = await _executor.Execute<Bootstrap>(request);
                _footballerCache.setFootballers(bootstrap.elements);
            }
            _footballerCache.bootstrapLock.Release();
        }
        return _footballerCache.footballers;
    }

    public async Task<IDictionary<int, FootballerDetails>> getFootballerDetails(ICollection<int> ids)
    {
        return await readFootballerDetails(ids);
    }

    public async Task<IDictionary<int, FootballerDetails>> readFootballerDetails(ICollection<int> ids)
    {
        foreach (var id in ids) {
            FootballerDetails detail = getCachedDetails(id);
            if (detail == null) {
                _footballerCache.footballerDetails.Add(id, await readFootballerDetails(id));
            }
        }
        return _footballerCache.footballerDetails;
    }

    public async Task<FootballerDetails> readFootballerDetails(int footballerId)
    {
        var request = _generator.GenerateFootballerDetailRequest(footballerId);
        FootballerDetails details = await _executor.Execute<FootballerDetails>(request);
        return details;
    }

    public async Task<Live> getLiveData(int eventId) {
        if (!_footballerCache.liveData.ContainsKey(eventId))
        {
            await _footballerCache.liveDataLock.WaitAsync();
            if (!_footballerCache.liveData.ContainsKey(eventId))
            {
                Live data = await readLiveEventData(eventId);
                _footballerCache.liveData[eventId] = data;
            }
            _footballerCache.liveDataLock.Release();
        }
        return _footballerCache.liveData[eventId];
    }

    public async Task<Live> readLiveEventData(int eventId) {
        var request = _generator.GenerateLiveDataRequest(eventId);
        var liveString = await _executor.Execute(request);
        var live = JsonConvert.DeserializeObject<Live>(liveString);
        foreach (var fixture in live.fixtures) {
            fixture.parsedStats = fixture.getStats();
        }
        return live;
    }

    public async Task<Picks> getPicks(int teamId, int eventId) {
        var key = $"{teamId}_{eventId}";
        if (!_footballerCache.picks.ContainsKey(key))
        {
            if (!_footballerCache.picks.ContainsKey(key))
            {
                var picks = await readPicks(teamId, eventId);
                _footballerCache.picks[key] = picks;
            }
        }
        return _footballerCache.picks[key];
    }

    private async Task<Picks> readPicks(int teamId, int eventId)
    {
        var request = _generator.GeneratePicksRequest(teamId, eventId);
        try {
            return await _executor.Execute<Picks>(request);
        }
        catch (Exception ex) {
            _log.Error(ex);
            return null;
        }
    }

    public async Task<BootstrapStatic> getBootstrapStatic()
    {
        if (_footballerCache.bootstrapStatic == null)
        {
            await _footballerCache.bootstrapStaticLock.WaitAsync();
            if (_footballerCache.bootstrapStatic == null)
            {
                var request = _generator.GenerateBootstrapStaticRequest();
                var bootstrap = await _executor.Execute<BootstrapStatic>(request);
                _footballerCache.bootstrapStatic = bootstrap;
            }
            _footballerCache.bootstrapStaticLock.Release();
        }
        return _footballerCache.bootstrapStatic;
    }

    public async Task<EntryData> getEntry(int teamId)
    {
        if (!_footballerCache.entries.ContainsKey(teamId))
        {
            EntryData data = await readEntry(teamId);
            _footballerCache.entries[teamId] = data;
        }
        return _footballerCache.entries[teamId];
    }

    private async Task<EntryData> readEntry(int teamId) {
        if (teamId > 0) {
            var request = _generator.GenerateEntryRequest(teamId);
            EntryData data = await _executor.Execute<EntryData>(request);
            data.entry.parseKit();
            return data;
        }
        return null;
    }

    public async Task<Standings> getStandings(int leagueId) {
        if (_footballerCache.standings == null)
        {
            await _footballerCache.standingsLock.WaitAsync();
        if (_footballerCache.standings == null)
        {
            Standings standings = await readStandings(leagueId);
            _footballerCache.standings = standings;
        }
        _footballerCache.standingsLock.Release();
        }
        return _footballerCache.standings;
    }

    private async Task<Standings> readStandings(int leagueId) {
        var request = _generator.GenerateLeagueH2hStandingsRequest(leagueId);
        try {
            var standingsString = await _executor.Execute(request);
            return JsonConvert.DeserializeObject<Standings>(standingsString);
        }
        catch(Exception ex) {
            NewStandings newStandings = await _executor.Execute<NewStandings>(request);
            _log.Error(ex);
            return new Standings(newStandings);
        }
    }

    public async Task<TeamHistory> getHistory(int teamId) {
        if (!_footballerCache.history.ContainsKey(teamId))
        {
            TeamHistory data = await readHistory(teamId);
            _footballerCache.history[teamId] = data;
        }
        return _footballerCache.history[teamId];
    }

    private async Task<TeamHistory> readHistory(int teamId) {
        if (teamId == 0) {
            return null;
        }
        var request = _generator.GenerateHistoryRequest(teamId);
        var dataString = await _executor.Execute(request);
        return JsonConvert.DeserializeObject<TeamHistory>(dataString);
    }

    public async Task<List<Club>> getClubs() {
        var clubs = new List<Club>();
        var request = _generator.GenerateFootballersRequest();
        var bootstrap = await _executor.Execute<Bootstrap>(request);
        foreach (var club in bootstrap.teams)
        {
            clubs.Add(club);
        }
        return clubs;
    }

    public async Task<ICollection<Match>> findMatches(int leagueId, int gameweek) {
        ProcessedLeagueFixtureList matchesInfo = await getLeagueEntriesAndMatches(leagueId);
        if (matchesInfo == null)
        {
            return null;
        }
        return matchesInfo.matches[gameweek];
    }

    public async Task<Match> findMatch(int leagueId, int teamId, int gameweek) {
        foreach (var match in await findMatches(leagueId, gameweek)) {
            if (match.entry_1_entry == teamId || match.entry_2_entry == teamId) {
                return match;
            }
        }
        return null;
    }

    public async Task<ProcessedLeagueFixtureList> getLeagueEntriesAndMatches(int leagueId)
    {
        if (!_footballerCache.leagueEntriesAndMatches.ContainsKey(leagueId))
        {
            await _footballerCache.leagueEntriesLock.WaitAsync();
            if (!_footballerCache.leagueEntriesAndMatches.ContainsKey(leagueId))
            {
                ProcessedLeagueFixtureList data = await readLeagueH2hMatches(leagueId);
                _footballerCache.leagueEntriesAndMatches[leagueId] = data;
            }
            _footballerCache.leagueEntriesLock.Release();
        }
        return _footballerCache.leagueEntriesAndMatches[leagueId];
    }

    public async Task<ProcessedLeagueFixtureList> readLeagueH2hMatches(int leagueId) {
        return await new S3JsonReader().Read<ProcessedLeagueFixtureList>(String.Format(GlobalConfig.DataRoot + "/{0}/fixtures/fixtures.json", leagueId));
    }

    public async Task<LeagueEntriesAndMatches> readLeagueH2hMatches(int leagueId, int pageNum) {
        var request = _generator.GenerateLeagueH2hMatchesRequest(leagueId, pageNum);
        try {
            return await _executor.Execute<LeagueEntriesAndMatches>(request);
        }
        catch (Exception ex) {
            _log.Error(ex);
            return null;
        }
    }

    public async Task<ICollection<int>> getTeamsInLeague(int leagueId) {
        var standings = await getStandings(leagueId);
        var teams = new List<int>();
        foreach (var standing in standings.standings.results) {
            teams.Add(standing.entry ?? 0);
        }
        return teams;
    }

    private FootballerDetails getCachedDetails(int id) {
        FootballerDetails data;
        return _footballerCache.footballerDetails.TryGetValue(id, out data) ? data : null;
    }

    private void initialize(IRequestExecutor executor)
    {
        _generator = new EPLRequestGenerator();
        _executor = executor;
        _footballerCache = new FootballerDataCache();
    }
}
