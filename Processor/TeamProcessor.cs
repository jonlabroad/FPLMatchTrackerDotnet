using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TeamProcessor
{
    EPLClient _client;
    ICollection<int> _teamIds;
    int _gameweek;
    int _leagueId;
    List<SingleTeamProcessor> _processors = new List<SingleTeamProcessor>();

    public TeamProcessor(EPLClient client, ICollection<int> teamIds, int gameweek, int leagueId) {
        _client = client != null ? client : EPLClientFactory.createClient();
        _teamIds = teamIds;
        _gameweek = gameweek;
        _leagueId = leagueId;
    }

    public async Task<IDictionary<int, ProcessedTeam>> process() {
        var tasks = new List<Task<ProcessedTeam>>();
        foreach (var teamId in _teamIds)
        {
            ProcessedPlayerProvider playerProvider = new ProcessedPlayerProvider();
            SingleTeamProcessor processor = new SingleTeamProcessor(playerProvider, teamId, _gameweek, _leagueId, _client);
            tasks.Add(processor.process());
        }
        await Task.WhenAll(tasks);

        var processedTeams = new Dictionary<int, ProcessedTeam>();
        foreach (var task in tasks) {
            var team = task.Result;
            processedTeams.Add(team.id, team);
        }
        return processedTeams;
    }
}
