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

    public ProcessedPlayerCollection copyApi() {
        var api = new ProcessedPlayerCollection();
        foreach (var elementId in players.Keys) {
            var element = players[elementId];
            if (element.events != null && element.events.Count > 0) {
                api.players.Add(elementId, ProcessedPlayer.ApiPlayer(element));
            }
        }
        return api;
    }
}
