using System.Collections.Generic;

public class Explain {
    public Explain() {}

    public Explain(Explain other) {
        fixture = other.fixture;
        stats = new List<ExplainElement>();
        foreach (var otherStat in other.stats) {
            stats.Add(new ExplainElement(otherStat));
        }
    }

    public int fixture { get; set; }
    public List<ExplainElement> stats { get; set; } = new List<ExplainElement>();
}