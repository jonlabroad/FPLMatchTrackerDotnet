using System.Collections.Generic;

public class SecretConfig {
    public IDictionary<int, User> users {get; set;} = new Dictionary<int, User>();
    public string platformApplicationArn {get; set;} = "";
    public string googleApiKey {get; set;} = "";

    public void AddUser(int teamId, string teamName, string phoneNumber) {
        User user = new User();
        user.teamId = teamId;
        user.teamName = teamName;
        user.alertPhoneNumber = phoneNumber;
        users.Add(teamId, user);
    }

    public User GetUserByTeamId(int teamId) {
        User user;
        return users.TryGetValue(teamId, out user) ? user : null;
    }
}
