using System;
using System.Threading.Tasks;

public class CloudConfigUpdater
{
    private EPLClient _client;

    public CloudConfigUpdater(EPLClient client) {
        _client = client != null ? client : EPLClientFactory.createClient();
    }

    public async Task<bool> update() {
        var currentEvent = await getCurrentEvent();
        if (currentEvent == null) {
            Console.WriteLine("Unable to find current event");
            return false;
        }

        if (currentEvent.id != GlobalConfig.CloudAppConfig.CurrentGameWeek) {
            GlobalConfig.CloudAppConfig.CurrentGameWeek = currentEvent.id;
            await new CloudAppConfigProvider().write(GlobalConfig.CloudAppConfig);
            return true;
        }
        return false;
    }

    private async Task<Event> getCurrentEvent() {
        var boot = await _client.getBootstrapStatic();
        int currentEvent = boot.currentEvent;
        foreach (Event ev in boot.events) {
            if (ev.id == currentEvent) {
                return ev;
            }
        }
        return null;
    }
}
