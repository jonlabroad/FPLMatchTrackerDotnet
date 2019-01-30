using System;

public class DailyTimer
{
    public DailyTimer()
    {

    }

    public bool IsNewDay()
    {
        var lastConfigDay = GlobalConfig.CloudAppConfig.day;
        var currentTime = DateTime.Now;
        return GlobalConfig.CloudAppConfig.day > 0 || lastConfigDay != currentTime.Day;
    }
}