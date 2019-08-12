using Newtonsoft.Json;
using RestSharp;

public class EPLRequestGenerator {
    public IRestRequest GenerateLeagueH2hStandingsRequest(int leagueId) {
        var resolvedLeaguePath = VariableSubstitutor.SubstituteLeague(GlobalConfig.LeagueH2hPath, leagueId);
        return Build(resolvedLeaguePath);
    }

    public IRestRequest GenerateFootballersRequest() {
        return Build(GlobalConfig.FootballersPath);
    }

    public IRestRequest GenerateBootstrapStaticRequest() {
        return Build(GlobalConfig.BootstrapStaticPath);
    }

    public IRestRequest GenerateLiveDataRequest(int eventId) {
        var resolvedUrl = VariableSubstitutor.Substitute(GlobalConfig.LivePath, 0, eventId);
        return Build(resolvedUrl);
    }

    public IRestRequest GenerateEntryRequest(int teamId) {
        var resolvedUrl = VariableSubstitutor.Substitute(GlobalConfig.EntryPath, teamId, 0);
        return Build(resolvedUrl);
    }

    public IRestRequest GenerateHistoryRequest(int teamId) {
        var resolvedUrl = VariableSubstitutor.Substitute(GlobalConfig.HistoryPath, teamId, 0);
        return Build(resolvedUrl);
    }

    public IRestRequest GeneratePicksRequest(int teamId, int eventId) {
        var resolvedUrl = VariableSubstitutor.Substitute(GlobalConfig.PicksPath, teamId, eventId);
        return Build(resolvedUrl);
    }

    public IRestRequest GenerateFootballerDetailRequest(int footballerId) {
        var resolvedUrl = VariableSubstitutor.SubstituteFootballerId(GlobalConfig.FootballerDetailsPath, footballerId);
        return Build(resolvedUrl);
    }

    public IRestRequest GenerateLeagueH2hMatchesRequest(int leagueId, int pageNum) {
        var resolvedUrl = VariableSubstitutor.SubstituteLeague(GlobalConfig.LeagueH2hMatchesPath, leagueId);
        resolvedUrl = VariableSubstitutor.SubstitutePage(resolvedUrl, pageNum);
        return Build(resolvedUrl);
    }

    public IRestRequest GenerateMyTeamRequest(int teamId) {
        var path = $"/my-team/{teamId}/";
        return Build(path);
    }

    private IRestRequest Build(string path) {
        return new RestRequest(path, Method.GET);
    }

    private IRestRequest BuildPost(string path, string body) {
        var request = new RestRequest(path, Method.POST);
        request.AddBody(body);
        return request;
    }
}
