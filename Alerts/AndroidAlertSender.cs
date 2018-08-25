using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using NLog;

public class AndroidAlertSender : IAlertSender
{
    private AmazonSimpleNotificationServiceClient _client;
    private Logger _log = LogManager.GetCurrentClassLogger();

    public AndroidAlertSender() {
        _client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast1);
    }

    public async Task SendAlert(int teamId, string title, string subtitle) {
        _log.Info("Sending alert for team {0}\n", teamId);
        var endpoints = await findTeamEndpoints(teamId);
        _log.Info(string.Format("Found {0} endpoints for team {1}\n", endpoints.Count, teamId));
        foreach (var endpointArn in endpoints)
        {
            var request = new PublishRequest()
            {
                TargetArn = endpointArn,
                Message = createNotification(title, subtitle),
                MessageStructure = "json"
            };
            await _client.PublishAsync(request);

            request = new PublishRequest()
            {
                TargetArn = endpointArn,
                Message = createDataMessage(title),
                MessageStructure = "json"
            };
            await _client.PublishAsync(request);
        }
    }

    protected async Task<List<string>> findTeamEndpoints(int teamId) {
        var endpoints = new List<string>();
        ListEndpointsByPlatformApplicationRequest request = new ListEndpointsByPlatformApplicationRequest()
        {
            PlatformApplicationArn = GlobalConfig.Secrets.platformApplicationArn
        };
        var result = await _client.ListEndpointsByPlatformApplicationAsync(request);
        var devices = findDevicesSubscribed(teamId);

        foreach (Endpoint endpoint in result.Endpoints) {
            if (endpoint.Attributes.GetValueOrDefault("Enabled").Equals("true", StringComparison.OrdinalIgnoreCase)) {
                EndpointUserData userData = readUserData(endpoint.Attributes.GetValueOrDefault("CustomUserData"));
                _log.Info(string.Format("Endpoint user playlistId: {0}, Firebase playlistId {1}\n", userData.uniqueUserId, userData.firebaseId));
                if (devices.Contains(userData.uniqueUserId)) {
                    _log.Info(string.Format("Found subscribed device! {0}\n", userData.uniqueUserId));
                    endpoints.Add(endpoint.EndpointArn);
                }
            }
        }
        return endpoints;
    }

    protected HashSet<string> findDevicesSubscribed(int teamId) {
        var devices = new HashSet<string>();
        foreach (DeviceConfig config in GlobalConfig.DeviceConfig.Values) {
            foreach (var subscriber in config.getSubscribers(teamId)) {
                devices.Add(subscriber);
            };
        }
        return devices;
    }

    protected string createNotification(string title, string subtitle) {
        string msg = "{ \"GCM\": \"{\\\"notification\\\": {\\\"title\\\": \\\"" + title + "\\\", \\\"body\\\": \\\"" + subtitle + "\\\", \\\"sound\\\": \\\"default\\\", \\\"tag\\\": \\\"0\\\"}}\"}";
        _log.Info(msg);
        return msg;
    }

    protected string createDataMessage(string title) {
        string msg = "{ \"GCM\": \"{\\\"data\\\": {\\\"title\\\": \\\"" + title + "\\\"}}\"}";
        _log.Info(msg);
        return msg;
    }

    protected EndpointUserData readUserData(string userDataRaw) {
        return new EndpointUserData(userDataRaw);
    }
}