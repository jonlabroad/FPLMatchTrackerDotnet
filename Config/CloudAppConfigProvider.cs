using System.Threading.Tasks;

public class CloudAppConfigProvider {

    private static readonly string FILENAME = "appconfig.json";

    private S3JsonReader _reader;
    private S3JsonWriter _writer;
    private CloudAppConfig _config;

    public CloudAppConfigProvider() {
        _reader = new S3JsonReader();
        _writer = new S3JsonWriter();
    }

    public async Task<CloudAppConfig> read() {
        var newConfig = await _reader.Read<CloudAppConfig>(FILENAME);
        return newConfig != null ? newConfig : new CloudAppConfig();
    }

    public async Task write(CloudAppConfig config) {
        _config = config;
        await _writer.write(FILENAME, config, true);
    }
}
