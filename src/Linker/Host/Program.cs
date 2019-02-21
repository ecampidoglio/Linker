using System.IO;
using Linker.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Linker.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseIISIntegration()
                .ConfigureLogging(log => log.AddConsole())
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
