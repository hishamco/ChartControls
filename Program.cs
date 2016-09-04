using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ChartControls
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().Build();
            
            var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseConfiguration(config)
                        .UseIISIntegration()
                        .UseStartup<Startup>()
                        .Build();

            host.Run();
        }
    }
}