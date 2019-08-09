public class LoginPayload {
    public string password { get; set; }
    public string login { get; set; }
    public string redirect_uri { get; set; } = "https://fantasy.premierleague.com/a/login";
    public string app { get; set; } = "plfpl-web";
}