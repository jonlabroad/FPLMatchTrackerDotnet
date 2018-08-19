using System.Collections.Generic;
using System.Threading.Tasks;

public class EPLClient
{
    private EPLRequestGenerator _generator;
    private IRequestExecutor _executor;
    //private FootballerDataCache _footballerCache;

    public EPLClient(IRequestExecutor executor)  {
        initialize(executor);
    }

    public async Task<Bootstrap> getBootstrap() {
        //if (_footballerCache.footballers.size() <= 0) {
            var request = _generator.GenerateFootballersRequest();
            var bootstrap = await _executor.Execute<Bootstrap>(request);
            //_footballerCache.setFootballers(bootstrap.elements);
        //}
        return bootstrap;
    }


    private void initialize(IRequestExecutor executor)
    {
        _generator = new EPLRequestGenerator();
        _executor = executor;
        //_footballerCache = new FootballerDataCache();
    }
}
