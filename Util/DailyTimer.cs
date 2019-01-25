using System;

public class DailyTimer
{
    public DailyTimer()
    {

    }

    public bool IsNewDay()
    {
        var lastConfigTimestamp = Date.fromString(GlobalConfig.CloudAppConfig.day);
        var currentTime = DateTime.Now;
        return string.IsNullOrEmpty(GlobalConfig.CloudAppConfig.day) || lastConfigTimestamp.Day != currentTime.Day;
    }
}