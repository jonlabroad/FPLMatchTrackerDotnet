using System.Collections.Generic;

public class Leagues
{
    public List<ClassicLeague> classic { get; set; } = new List<ClassicLeague>();
    public List<H2hLeague> h2h { get; set; } = new List<H2hLeague>();
    public Cup cup { get; set; } = new Cup();
}
