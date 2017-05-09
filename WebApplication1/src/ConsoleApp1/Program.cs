using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    using System.IO;

    using Microsoft.AspNetCore.Hosting;

    using WebApplication1;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseWebListener()
                //.UseKestrel()
                .UseUrls("http://localhost:9410/") // WTAULVSE0DX3W7L
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
