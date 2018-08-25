using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Newtonsoft.Json;
using NLog;

public class S3JsonReader : SimpleS3Provider {
    private static Logger _log = LogManager.GetCurrentClassLogger();
    public async Task<T> Read<T>(string keyName)
    {
        try
        {
            var response = await _client.GetObjectAsync(_bucketName, keyName);
            if (response.ContentLength > 0)
            {
                var configStr = await new StreamReader(response.ResponseStream).ReadToEndAsync();
                var config = JsonConvert.DeserializeObject<T>(configStr, new JsonSerializerSettings {
                                                            NullValueHandling = NullValueHandling.Ignore
                                                        });
                return config;
            }
        }
        catch (Exception) {
            _log.Error(string.Format("Could not read {0}\n", keyName));
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
