using System;
using System.Threading.Tasks;
using NLog;

public class PlayerProcessorConfig
{
    public int recorderSequence = 0;
    public bool record = true;
    private static Logger _log = LogManager.GetCurrentClassLogger();

    private static readonly string ConfigKey = "playerprocessorconfig.json";
    private static PlayerProcessorConfig instance = null;

    protected PlayerProcessorConfig() {
    }

    public static PlayerProcessorConfig getInstance() {
        if(instance == null) {
            try {
                instance = readCloud().Result;
            }
            catch (Exception) {
                _log.Error(string.Format("PlayerProcessorConfig not found at {0}\n", ConfigKey));
                instance = new PlayerProcessorConfig();
            }
        }
        return instance;
    }

    private static async Task<PlayerProcessorConfig> readCloud() {
        S3JsonReader reader = new S3JsonReader();
        _log.Info(String.Format("Reading {0} {1}\n", GlobalConfig.S3Bucket, ConfigKey));
        return await reader.Read<PlayerProcessorConfig>(ConfigKey);
    }

    public async Task write() {
        S3JsonWriter writer = new S3JsonWriter();
        await writer.write(ConfigKey, this);
    }

    public async Task<PlayerProcessorConfig> refresh() {
        instance = await readCloud();
        return instance;
    }
}
