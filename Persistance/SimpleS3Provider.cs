using Amazon;
using Amazon.S3;

public class SimpleS3Provider {
    protected AmazonS3Client _client;
    protected string _bucketName;

    public SimpleS3Provider() {
        Init(GlobalConfig.S3Bucket);
    }

    public SimpleS3Provider(string bucketName) {
        Init(bucketName);
    }

    private void Init(string bucketName) {
        _bucketName = bucketName;
        _client = new AmazonS3Client(RegionEndpoint.USEast1);
    }
}
