using System;
using System.Threading.Tasks;
using NLog;

public class CloudConfigUpdater
{
    private EPLClient _client;
    private static Logger _log = LogManager.GetCurrentClassLogger();

    public CloudConfigUpdater(EPLClient client) {
        _client = client != null ? client : EPLClientFactory.createClient();
    }

    public async Task<bool> update() {
        var currentEvent = await getCurrentEvent();
        if (currentEvent == null) {
            _log.Error("Unable to find current event");
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
