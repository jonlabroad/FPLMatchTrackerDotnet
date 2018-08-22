using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ScoutingProcessor
{
    ISet<int> _processedTeams = new HashSet<int>();
    IDictionary<int, ProcessedTeam> _teams = new Dictionary<int, ProcessedTeam>();
    int _leagueId;
    EPLClient _client = null;

    public ScoutingProcessor(int leagueId, EPLClient client, IDictionary<int, ProcessedTeam> teams) {
        initialize(leagueId, client, teams);
    }

    protected void initialize(int leagueId, EPLClient client, IDictionary<int, ProcessedTeam> teams) {
        if (client == null) {
            client = EPLClientFactory.createClient();
        }
        _client = client;
        _teams = teams;
        _leagueId = leagueId;
    }

    public async Task<IDictionary<int, ScoutingReport>> Process() {
        IDictionary<int, ScoutingReport> reports = new Dictionary<int, ScoutingReport>();
        Standings standings = await _client.getStandings(_leagueId);
        for (int gameweek = GlobalConfig.CloudAppConfig.CurrentGameWeek; gameweek <= 38; gameweek++) {
            foreach (ProcessedTeam team in _teams.Values) {
                if (_processedTeams.Contains(team.id)) {
                    Console.WriteLine(string.Format("Team scouting report already processed (gw={0}): {1}\n", gameweek, team.id));
                    continue;
                }

                ScoutingReport report = new ScoutingReport();
                report.gameweek = gameweek;
                await findMatch(team.id, report, gameweek);

                processTeams(_teams, report.match, report, standings);

                findDifferential(report);
                generateStats(report);
                await simulateH2h(report);
                await writeReports(report, gameweek);

                reports[report.match.entry_1_entry] = report;
                reports[report.match.entry_2_entry] = report;
                _processedTeams.Add(report.match.entry_1_entry);
                _processedTeams.Add(report.match.entry_2_entry);
            }
            _processedTeams.Clear();
        }

        return reports;
    }

    private void generateStats(ScoutingReport report)
    {
        foreach (var team in report.teams.Values)
        {
            addBestPlayer(team, report);
            addInformPlayer(team, report);
        }
    }

    private TeamStats getStats(ProcessedTeam team, ScoutingReport report) {
        if (!report.stats.ContainsKey(team.id)) {
            report.stats.Add(team.id, new TeamStats());
        }
        return report.stats[team.id];
    }

    private void addBestPlayer(ProcessedTeam team, ScoutingReport report)
    {
        ProcessedPlayer player = findBestPlayer(team);
        TeamStats stats = getStats(team, report);
        stats.bestPlayer = player;
    }

    private void addInformPlayer(ProcessedTeam team, ScoutingReport report)
    {
        ProcessedPlayer player = findInformPlayer(team);
        TeamStats stats = getStats(team, report);
        stats.informPlayer = player;
    }

    private ProcessedPlayer findBestPlayer(ProcessedTeam team)
    {
        double maxPts = 0.0;
        ProcessedPlayer bestPlayer = null;
        foreach (var pick in team.picks)
        {
            try {
                double pointsPerGame = Double.Parse(pick.footballer.rawData.footballer.points_per_game);
                if (pointsPerGame > 0.0 && pointsPerGame > maxPts) {
                    maxPts = pointsPerGame;
                    bestPlayer = pick.footballer;
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
        return bestPlayer;
    }

    private ProcessedPlayer findInformPlayer(ProcessedTeam team)
    {
        double maxPts = 0.0;
        ProcessedPlayer bestPlayer = null;
        foreach (var pick in team.picks)
        {
            try {
                double form = Double.Parse(pick.footballer.rawData.footballer.form);
                if (form > 0.0 && form > maxPts) {
                    maxPts = form;
                    bestPlayer = pick.footballer;
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
        return bestPlayer;
    }

    private async Task writeReports(ScoutingReport report, int gameweek) {
        S3JsonWriter writer = new S3JsonWriter();
        foreach (var team in report.teams.Values)
        {
            if (team == null) {
                continue;
            }
            await writer.write(string.Format(GlobalConfig.DataRoot + "/{0}/{1}/{2}/ScoutingReport", _leagueId, team.id, gameweek), report, true);
        }
    }

    private async Task simulateH2h(ScoutingReport report) {
        if (report.teams.Keys.Contains(0)) {
            return;
        }
        H2hSimulator simulator = new H2hSimulator(_client, report.match.entry_1_entry, report.match.entry_2_entry);
        report.simulatedH2h = await simulator.simulate();
    }

    private async Task findMatch(int teamId, ScoutingReport report, int gameweek) {
        Match match = await findMatch(teamId, gameweek);
        report.match = match;
    }

    private async Task<Match> findMatch(int teamId, int gameweek) {
        Match match = await _client.findMatch(_leagueId, teamId, gameweek);
        if (match == null) {
            Console.WriteLine(string.Format("Unable to find match for team {0}, gameweek {1}\n", teamId, gameweek));
        }
        return match;
    }

    private void findDifferential(ScoutingReport report) {
        var teams = new List<ProcessedTeam>();
        foreach (var team in report.teams.Values) {
            teams.Add(team);
        }

        DifferentialFinder diffFinder = new DifferentialFinder(teams[0], teams[1]);
        report.differentials = diffFinder.find();
    }

    protected Standing getStanding(Standings standings, int teamId) {
        if (standings == null) {
            return null;
        }

        foreach (var standing in standings.standings.results)
        {
            if (standing.entry == teamId) {
                return standing;
            }
        }
        return null;
    }

    private void processTeams(IDictionary<int, ProcessedTeam> teams, Match match, ScoutingReport report, Standings standings) {
        var teamIds = new HashSet<int>();
        teamIds.Add(match.entry_1_entry);
        teamIds.Add(match.entry_2_entry);
        foreach (var teamId in teamIds)
        {
            ProcessedTeam team = teams[teamId];
            ProcessedMatchTeam matchTeam = new ProcessedMatchTeam(team, getStanding(standings, team.id));
            report.teams[teamId] = matchTeam;
        }
    }
}
