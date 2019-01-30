using System.Collections.Generic;

public class CloudAppConfig {
    public int CurrentGameWeek {get; set;} = 1;
    public int day {get; set;} = 0;
    public bool finalPollOfDayCompleted {get; set;}  = false;
    public IDictionary<int, TeamIdName> AvailableTeams {get; set;} = new Dictionary<int, TeamIdName>();
}
