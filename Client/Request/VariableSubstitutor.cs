public class VariableSubstitutor {
    private static readonly string ENTRY_ID_STRING = "ENTRY_ID";
    private static readonly string EVENT_ID_STRING = "EVENT_ID";
    private static readonly string LEAGUE_ID_STRING = "LEAGUE_ID";
    private static readonly string FOOTBALLER_ID_STRING = "FOOTBALLER_ID";
    private static readonly string LEAGUE_H2H_PAGE_STRING = "PAGE";

    public static string Substitute(string inputString, int entryId, int eventId) {
        string withEntry = SubstituteEntryId(inputString, entryId);
        return SubstituteEventId(withEntry, eventId);
    }

    public static string SubstituteLeague(string inputString, int leagueId) {
        return Substitute(inputString, LEAGUE_ID_STRING, leagueId);
    }

    public static string SubstituteFootballerId(string inputString, int footballerId) {
        return Substitute(inputString, FOOTBALLER_ID_STRING, footballerId);
    }

    public static string SubstitutePage(string inputString, int pageNum) {
        return Substitute(inputString, LEAGUE_H2H_PAGE_STRING, pageNum);
    }

    private static string SubstituteEventId(string inputString, int entryId) {
        return Substitute(inputString, EVENT_ID_STRING, entryId);
    }

    private static string SubstituteEntryId(string inputString, int entryId) {
        return Substitute(inputString, ENTRY_ID_STRING, entryId);
    }

    private static string Substitute(string inputString, string name, int value) {
        string toReplace = "{" + $"{name}" + "}";
        return inputString.Replace(toReplace, value.ToString());
    }
}
