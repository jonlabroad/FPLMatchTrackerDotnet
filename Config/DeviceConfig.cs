using System.Collections.Generic;

public class DeviceConfig {
    public string uniqueDeviceId {get; set;}
    public Subscription subscriptions {get; set;} = new Subscription();

    public DeviceConfig(string uId) {
        uniqueDeviceId = uId;
    }

    public void addSubscription(int teamId, string teamName) {
        subscriptions.teamsByTeamId.Add(teamId, new TeamSubscription(teamId, teamName));
    }

    public bool isSubscribed(int teamId) {
        return subscriptions.teamsByTeamId.ContainsKey(teamId);
    }

    public ISet<string> getSubscribers(int teamId) {
        var devices = new HashSet<string>();
        if (subscriptions.teamsByTeamId.ContainsKey(teamId) && !devices.Contains(uniqueDeviceId)) {
            devices.Add(uniqueDeviceId);
        }
        return devices;
    }

    public HashSet<int> getAllTeamIds() {
        var teams = new HashSet<int>();
        foreach (var teamId in subscriptions.teamsByTeamId.Keys) {
            teams.Add(teamId);
        }
        return teams;
    }
}
