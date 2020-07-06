using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace REO
{
    public class Program
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        private string projectRootFolder;
        public static string serverAddress = "";
        public Program(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;
            projectRootFolder = env.ContentRootPath.Substring(0,
                env.ContentRootPath.LastIndexOf(@"\ProjectRoot\", StringComparison.Ordinal) + @"\ProjectRoot\".Length);

        }
        public static void Main(string[] args)
        {
            var b = CreateHostBuilder(args).Build();
            //b.Start ();
            //serverAddress = b.Services.GetService(typeof(IServerAddressesFeature)).ToString();
            //b.WaitForShutdown();
            b.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
