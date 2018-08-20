public class TeamSubscription {
    public int teamId {get; set;}
    public string teamName {get; set;}

    public TeamSubscription(int id, string name) {
        teamId = id;
        teamName = name;
    }
}
