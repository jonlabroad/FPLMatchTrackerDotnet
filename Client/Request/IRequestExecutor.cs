using System.Net;
using System.Threading.Tasks;
using RestSharp;

public interface IRequestExecutor {
    CookieContainer Cookies { get; set; }

    Task<T> Execute<T>(IRestRequest request);
    Task<string> Execute(IRestRequest request);
    Task<IRestResponse> ExecuteWithResponse(IRestRequest request);
}
