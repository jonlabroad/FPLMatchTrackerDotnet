using System.Collections.Generic;

public class CloudAppConfig {
    public int CurrentGameWeek {get; set;} = 1;
    public string day {get; set;} = null;
    public bool finalPollOfDayCompleted {get; set;}  = false;
    public IDictionary<int, TeamIdName> AvailableTeams {get; set;} = new Dictionary<int, TeamIdName>();
}
