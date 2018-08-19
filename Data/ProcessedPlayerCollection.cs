using System.Collections.Generic;

public class ProcessedPlayerCollection
{
    public IDictionary<int, ProcessedPlayer> players {get; set;} = new Dictionary<int, ProcessedPlayer>();

    public ProcessedPlayerCollection() {
    }

    public void merge(ProcessedPlayerCollection other) {
        foreach (var col in other.players)
        {
            players.Add(col);
        }
    }
}
