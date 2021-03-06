using System;
using System.Linq;
using System.Threading.Tasks;

public class EventFinder
{
    EPLClient _client;

    public EventFinder(EPLClient client)
    {
        _client = client;
    }

    public async Task<Event> GetEvent(int gameweek)
    {
        var boot = await _client.getBootstrapStatic();
        return boot.events.Where(e => e.id == gameweek).First();
    }

    public async Task<Event> GetCurrentEvent()
    {
        var boot = await _client.getBootstrapStatic();
        var currentEvent = boot.events.FirstOrDefault(e => e.is_current);
        return currentEvent;
    }
    
    public DateTime GetEventStartTime(Event ev)
    {
        return Date.fromApiString(ev.deadline_time);
    }

}