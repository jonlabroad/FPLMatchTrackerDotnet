using System;
using System.Globalization;
using System.Runtime.Serialization;

public class Date
{
    //public static readonly DateTimeFormatter _formatter = DateTimeFormat.forPattern("yyyy-MM-dd HH:mm:ss z");
    //public static readonly DateTimeFormatter _apiFormatter = DateTimeFormat.forPattern("YYYY-MM-dd'T'HH:mm:ss'Z'").withZone(DateTimeZone.forID("Europe/London"));

    public static string toString(DateTime time) {
        return time.ToString("yyyy-MM-dd HH:mm:ss z");
    }

    public static DateTime parseConfigTime(string dateStr)
    {
        return DateTime.ParseExact(dateStr, "yyyy-MM-dd HH:mm:ss z", CultureInfo.InvariantCulture);
    }

    public static DateTime fromString(string dateString, bool utc=false) {
        //var date = DateTime.ParseExact(dateString.Replace(" EDT", "").Replace(" EST",""), "yyyy-MM-dd HH:mm:ss", null);
        var date = DateTime.Parse(dateString.Replace(" EDT", "").Replace(" EST","").Replace(" UTC", ""));
        try {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }
        catch (Exception)
        {
            return  TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("America/New_York"));
        }

    }

    public static DateTime fromApiString(string dateString) {
        var dateTime = DateTime.Parse(dateString);
        //var estDateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        return dateTime;
        //TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard"));
    }
}
