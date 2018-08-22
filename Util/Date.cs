using System;
using System.Runtime.Serialization;

public class Date
{
    //public static readonly DateTimeFormatter _formatter = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss z");
    //public static readonly DateTimeFormatter _apiFormatter = DateTimeFormat.forPattern("YYYY-MM-dd'T'HH:mm:ss'Z'").withZone(DateTimeZone.forID("Europe/London"));

    public static string toString(DateTime time) {
        return time.ToString("yyyy-MM-dd HH:mm:ss z");
    }

    public static DateTime fromString(string dateString) {
        //var date = DateTime.ParseExact(dateString.Replace(" EDT", "").Replace(" EST",""), "yyyy-MM-dd HH:mm:ss", null);
        var date = DateTime.Parse(dateString.Replace(" EDT", "").Replace(" EST",""));
        return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
    }

    public static DateTime fromApiString(string dateString) {
        var dateTime = DateTime.Parse(dateString);
        //var estDateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        return dateTime;
        //TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard"));
    }
}
