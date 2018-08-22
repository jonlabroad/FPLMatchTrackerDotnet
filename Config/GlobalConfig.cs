using System.Collections.Generic;

public class GlobalConfig {
    public static readonly string EplBaseUrl = "https://fantasy.premierleague.com/drf";
    public static readonly string FootballersPath = "/bootstrap";
    public static readonly string BootstrapStaticPath = "/bootstrap-static";
    public static readonly string EntryPath = "/entry/{ENTRY_ID}";
    public static readonly string EventPath = EntryPath + "/event/{EVENT_ID}";
    public static readonly string PicksPath = EventPath + "/picks";
    public static readonly string LeagueH2hPath = "/leagues-h2h-standings/{LEAGUE_ID}";
    public static readonly string FootballerDetailsPath = "/element-summary/{FOOTBALLER_ID}";
    public static readonly string LivePath = "/event/{EVENT_ID}/live";
    public static readonly string LeagueH2hMatchesPath = "/leagues-entries-and-h2h-matches/league/{LEAGUE_ID}?page={PAGE}";
    public static readonly string HistoryPath = EntryPath + "/history";

    // Highlights
    public static string YoutubeChannelId = "UCqZQlzSHbVJrwrn5XvzrzcA";

    public static readonly string SeasonPath = "Season2018";
    public static readonly string DataRoot = "data/" + SeasonPath;
    public static string MatchInfoRoot = DataRoot;

    public static readonly string S3Bucket = "fantasyeplmatchtracker";
    public static readonly string PlayerDataRoot = DataRoot + "/players";
    public static readonly string TopicNamePrefix = "matchtrackeralert_";
    public static readonly string TopicArnFormat = string.Format("arn:aws:sns:us-east-1:796987500533:{0}", TopicNamePrefix) + "{1}";

    public static SecretConfig Secrets = new SecretConfigurator().ReadConfig().Result;
    public static CloudAppConfig CloudAppConfig = new CloudAppConfigProvider().read().Result;

    public static readonly string RECORDER_BASE_PATH = "recorder";
    public static readonly string RECORDER_PATH_FMT = RECORDER_BASE_PATH + "/{0}/{1}/{2}";

    public static readonly int NumberFootballersToProcessPerLambda = 800;
    public static readonly bool BinPlayerData = false;

    public static bool LocalLambdas = true;
    public static bool TestMode = false;
    public static bool PlaybackMode = false;
    public static int PlaybackGameweek = 10;
    public static int CurrentPlaybackSequence = 0;
    public static bool Record = false;

    public static IDictionary<string, DeviceConfig> DeviceConfig = new DeviceConfigurator().readAllConfig().Result;
}
