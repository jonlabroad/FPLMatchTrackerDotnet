using System.Collections.Generic;

public class ProcessedLeagueFixtureList
{
    public League league {get; set;}
    public Dictionary<int, List<Match>> matches { get; set; } = new Dictionary<int, List<Match>>();
}
