using System.Collections.Generic;

public class ScoutingReport
{
    public int gameweek {get; set;}
    public Match match {get; set;}
    public string deadlineTimeFormatted { get; set; }
    public IDictionary<int, ProcessedMatchTeam> teams  {get; set;} = new Dictionary<int, ProcessedMatchTeam>();
    public IDictionary<int, Record> simulatedH2h  {get; set;} = new Dictionary<int, Record>();
    public HashSet<int> differentials  {get; set;} = new HashSet<int>();
    public IDictionary<int, TeamStats> stats {get; set;} = new Dictionary<int, TeamStats>();
}
