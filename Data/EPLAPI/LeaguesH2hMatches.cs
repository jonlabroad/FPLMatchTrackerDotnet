using Newtonsoft.Json.Linq;

public class LeaguesH2hMatches {
    public bool has_next { get; set; }
    public int page { get; set; }

    public JArray results { get; set; }
}