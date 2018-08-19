using System;
using Newtonsoft.Json;

namespace FPLMatchTrackerDotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new EPLClient(new RequestExecutor());
            //var bootstrap = client.getBootstrap().Result;
            //Console.WriteLine(JsonConvert.SerializeObject(bootstrap));

            var processor = new PlayerProcessor();
            processor.process().Wait();
        }
    }
}
