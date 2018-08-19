using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

public class S3JsonWriter : SimpleS3Provider {

    public async Task write<T>(string key, T obj) {
        await write(key, obj, false);
    }

    public async Task write<T>(string key, T obj, bool pub) {
        var request = new PutObjectRequest()
        {
            BucketName = _bucketName,
            Key = key,
            ContentBody = toJson(obj),
            CannedACL = pub ? S3CannedACL.PublicRead : S3CannedACL.NoACL
        };

        var response = await _client.PutObjectAsync(request);
    }

    public async Task delete(string key) {
        await _client.DeleteObjectAsync(_bucketName, key);
    }

    private string toJson<T>(T data) {
        return JsonConvert.SerializeObject(data);
    }
}
