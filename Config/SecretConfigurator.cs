using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

public class SecretConfigurator {
    private static readonly string S3_BUCKET_NAME = GlobalConfig.S3Bucket;
    private static readonly string S3_KEY_NAME = "secrets.config";

    private AmazonS3Client _s3;

    public SecretConfigurator() {
        _s3 = new AmazonS3Client(RegionEndpoint.USEast1);
    }

    public async Task<SecretConfig> ReadConfig() {
        var response = await _s3.GetObjectAsync(S3_BUCKET_NAME, S3_KEY_NAME);
        if (response.ContentLength > 0)
        {
            var configStr = await new StreamReader(response.ResponseStream).ReadToEndAsync();
            var config = JsonConvert.DeserializeObject<SecretConfig>(configStr);
            return config;
        }
        return new SecretConfig();
    }

    public async Task WriteConfig(SecretConfig config) {
        var request = new PutObjectRequest()
        {
            BucketName = S3_BUCKET_NAME,
            Key = S3_KEY_NAME,
            ContentBody = JsonConvert.SerializeObject(config)
        };        
        await _s3.PutObjectAsync(request);
    }
}
