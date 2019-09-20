using System.Collections.Generic;

public class LiveElementBase {
    public LiveElementBase() {}
    
    public LiveElementBase(int elementId) {
        id = elementId;
    }

    public LiveElementBase(LiveElementBase other) {
        id = other.id;
        explain = new List<Explain>();
        foreach (var otherExplain in other.explain) {
            explain.Add(new Explain(otherExplain));
        }
    }

    public int id { get; set; }
    public List<Explain> explain {get;set;} = new List<Explain>();
}