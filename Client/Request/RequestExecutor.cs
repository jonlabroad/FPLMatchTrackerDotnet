using System;
using System.Threading.Tasks;
using NLog;
using RestSharp;

public class RequestExecutor : IRequestExecutor
{
    bool _record;
    IRestClient _client;
    private Logger _log = LogManager.GetCurrentClassLogger();
    //RequestResponseRecorder _recorder;

    public RequestExecutor() {
        initialize(false, 0);
    }

    public RequestExecutor(bool record, int recordSequence)
    {
        initialize(record, recordSequence);
    }

    private void initialize(bool record, int recordSequence) {
        _client = new RestClient(GlobalConfig.EplBaseUrl);
        _record = record;
        if (_record) {
            //_recorder = new RequestResponseRecorder(GlobalConfig.CloudAppConfig.CurrentGameWeek, recordSequence);
        }
    }

    public async Task<T> Execute<T>(IRestRequest request) {
        try
        {
            _log.Info(request.Resource);
            var data = await _client.ExecuteTaskAsync<T>(request);
            if (_record) {
                //_recorder.record(request.getUrl(), jsonResponse.getBody());
            }
            return data.Data;
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            return default(T);
        }
    }

        public async Task<string> Execute(IRestRequest request) {
        try
        {
            _log.Info(request.Resource);
            var data = await _client.ExecuteTaskAsync(request);
            if (_record) {
                //_recorder.record(request.getUrl(), jsonResponse.getBody());
            }
            return data.Content;
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            return "";
        }
    }
}
