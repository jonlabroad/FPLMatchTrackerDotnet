using System.Threading.Tasks;

public class ConfigProvider
{
    public async Task<AlertProcessorConfig> read() {
        S3JsonReader reader = new S3JsonReader();
        return await reader.Read<AlertProcessorConfig>(createKey());
    }

    public async Task write(AlertProcessorConfig config) {
        S3JsonWriter writer = new S3JsonWriter();
        await writer.write(createKey(), config);

    }

    private string createKey() {
        return "alertprocessorconfig.json";
    }
}
