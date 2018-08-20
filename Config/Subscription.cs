using System.Collections.Generic;

public class Subscription {
    public IDictionary<int, TeamSubscription> teamsByTeamId {get; set;} = new Dictionary<int, TeamSubscription>();
}
