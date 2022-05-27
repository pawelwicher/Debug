using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DDM.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var appUrl = configurationBuilder.GetSection("Urls").GetValue<string>("AppUrl");
            var host = WebHost.CreateDefaultBuilder(args).UseUrls(appUrl).UseStartup<Startup>().Build();

            host.Run();
        }
    }
}