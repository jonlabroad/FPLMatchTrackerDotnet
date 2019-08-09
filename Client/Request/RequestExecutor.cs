using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RestSharp;

public class RequestExecutor : IRequestExecutor
{
    bool _record;
    IRestClient _client;
    public CookieContainer Cookies { get; set; }
    private Logger _log = LogManager.GetCurrentClassLogger();
    private SemaphoreSlim requestLock = new SemaphoreSlim(100, 100);
    //RequestResponseRecorder _recorder;

    public RequestExecutor(string host) {
        initialize(host, false, 0);
    }

    public RequestExecutor(string host, bool record, int recordSequence)
    {
        initialize(host, record, recordSequence);
    }

    private void initialize(string host, bool record, int recordSequence) {
        Cookies = new CookieContainer();
        _client = new RestClient(host);
        _client.CookieContainer = Cookies;
        _record = record;
        if (_record) {
            //_recorder = new RequestResponseRecorder(GlobalConfig.CloudAppConfig.CurrentGameWeek, recordSequence);
        }
    }

    public async Task<T> Execute<T>(IRestRequest request) {
        try
        {
            _log.Info(request.Resource);
            await requestLock.WaitAsync();
            var data = await _client.ExecuteTaskAsync<T>(request);
            requestLock.Release();
            if (_record) {
                //_recorder.record(request.getUrl(), jsonResponse.getBody());
            }
            return data.Data;
        }
        catch (Exception ex) {
            _log.Error(ex);
            return default(T);
        }
    }

        public async Task<string> Execute(IRestRequest request) {
        try
        {
            _log.Info(request.Resource);
            await requestLock.WaitAsync();
            var data = await _client.ExecuteTaskAsync(request);
            requestLock.Release();
            if (_record) {
                //_recorder.record(request.getUrl(), jsonResponse.getBody());
            }
            return data.Content;
        }
        catch (Exception ex) {
            _log.Error(ex);
            return "";
        }
    }

    public async Task<IRestResponse> ExecuteWithResponse(IRestRequest request)
    {
        try
        {
            _log.Info(request.Resource);
            await requestLock.WaitAsync();
            var data = await _client.ExecuteTaskAsync(request);
            requestLock.Release();
            return data;
        }
        catch (Exception ex) {
            _log.Error(ex);
            return null;
        } 
    }
}
