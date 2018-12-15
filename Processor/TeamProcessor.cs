using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TeamProcessor
{
    EPLClient _client;
    IEnumerable<int> _teamIds;
    int _gameweek;
    int _leagueId;
    List<SingleTeamProcessor> _processors = new List<SingleTeamProcessor>();
    ProcessedPlayerCollection _processedPlayers = null;

    public TeamProcessor(EPLClient client, IEnumerable<int> teamIds, int gameweek, int leagueId, ProcessedPlayerCollection processedPlayers = null) {
        _client = client != null ? client : EPLClientFactory.createClient();
        _teamIds = teamIds;
        _gameweek = gameweek;
        _leagueId = leagueId;
        _processedPlayers = processedPlayers;
    }

    public async Task<IDictionary<int, ProcessedTeam>> process() {
        var tasks = new List<Task<ProcessedTeam>>();
        foreach (var teamId in _teamIds)
        {
            ProcessedPlayerProvider playerProvider = new ProcessedPlayerProvider(_processedPlayers);
            SingleTeamProcessor processor = new SingleTeamProcessor(playerProvider, teamId, _gameweek, _leagueId, _client);
            tasks.Add(processor.process());
        }
        await Task.WhenAll(tasks);

        var processedTeams = new Dictionary<int, ProcessedTeam>();
        foreach (var task in tasks) {
            var team = await task;
            processedTeams.Add(team.id, team);
        }
        return processedTeams;
    }
}
