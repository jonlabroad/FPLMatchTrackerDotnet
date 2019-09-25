using System.Collections.Generic;

public class HomeAwayStats
{
    public string identifier { get; set; }
    public List<HomeAwayStatElement> a { get; set; } = new List<HomeAwayStatElement>();
    public List<HomeAwayStatElement> h { get; set; } = new List<HomeAwayStatElement>();
}
