using System;

public class Date
{
    //public static readonly DateTimeFormatter _formatter = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss z");
    //public static readonly DateTimeFormatter _apiFormatter = DateTimeFormat.forPattern("YYYY-MM-dd'T'HH:mm:ss'Z'").withZone(DateTimeZone.forID("Europe/London"));

    public static string toString(DateTime time) {
        return time.ToString("yyyy-MM-dd HH:mm:ss z");
    }

    public static DateTime fromString(string dateString) {
        return DateTime.Parse(dateString);
    }

    public static DateTime fromApiString(string dateString) {
        return DateTime.Parse(dateString);
    }
}
