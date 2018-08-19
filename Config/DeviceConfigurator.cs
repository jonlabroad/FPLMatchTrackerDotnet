using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;

public class DeviceConfigurator
{
    private static readonly string S3_CONFIG_DIR = "device_config";
    private static readonly string S3_PATH_FMT = S3_CONFIG_DIR + "/{0}";
    private static readonly string S3_KEY_NAME = "device.config";

    private S3JsonWriter _writer = new S3JsonWriter();
    private S3JsonReader _reader = new S3JsonReader();

    public DeviceConfigurator() : base() {

    }

    public async Task<DeviceConfig> readConfig(string uniqueDeviceId) {
        string s3Key = getDeviceConfigPath(uniqueDeviceId);
        var config = await _reader.Read<DeviceConfig>(s3Key);
        if (config == null)
        {
            config = new DeviceConfig(uniqueDeviceId);
        }
        return config;
    }

    public async Task<IDictionary<string, DeviceConfig>> readAllConfig() {
        var configs = new Dictionary<string, DeviceConfig>();
        var keys = await _reader.getKeys(S3_CONFIG_DIR);
        foreach (var key in keys) {
            var config = await readConfig(key);
            configs.Add(config.uniqueDeviceId, config);
        }
        return configs;
    }

    public async Task writeConfig(DeviceConfig config, string deviceId) {
        await _writer.write(getDeviceConfigPath(deviceId), config);
    }

    private string getDeviceConfigPath(string deviceId) {
        return string.Format(S3_PATH_FMT + "/" + S3_KEY_NAME, deviceId);
    }
}
