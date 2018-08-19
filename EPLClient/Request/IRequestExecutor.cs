using System.Threading.Tasks;
using RestSharp;

public interface IRequestExecutor {
    Task<T> Execute<T>(IRestRequest request);
    Task<string> Execute(IRestRequest request);
}
