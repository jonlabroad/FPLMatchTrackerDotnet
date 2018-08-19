using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Newtonsoft.Json;

public class S3JsonReader : SimpleS3Provider {
    public async Task<T> Read<T>(string keyName)
    {
        try
        {
            var response = await _client.GetObjectAsync(_bucketName, keyName);
            if (response.ContentLength > 0)
            {
                var configStr = await new StreamReader(response.ResponseStream).ReadToEndAsync();
                var config = JsonConvert.DeserializeObject<T>(configStr);
                return config;
            }
        }
        catch (Exception ex) {
            Console.WriteLine(string.Format("Could not read {0}\n", keyName));
        }
        return default(T);
    }

    public async Task<ICollection<string>> getKeys(string path) {
        ISet<string> keys = new HashSet<string>();
        var response = await _client.ListObjectsAsync(_bucketName, path);
        foreach (S3Object entry in response.S3Objects) {
            keys.Add(entry.Key);
        }
        return keys;
    }
}
