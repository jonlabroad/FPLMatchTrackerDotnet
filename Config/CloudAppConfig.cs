using System.Collections.Generic;

public class CloudAppConfig {
    public int CurrentGameWeek = 1;
    public string day = null;
    public bool finalPollOfDayCompleted = false;
    public IDictionary<int, TeamIdName> AvailableTeams = new Dictionary<int, TeamIdName>();
}
