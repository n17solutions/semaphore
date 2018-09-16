using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace N17Solutions.Semaphore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"Directory Exists: {Directory.Exists("/data/semaphore")}");
            Console.WriteLine($"Public Key Exists: {File.Exists("/data/semaphore/public.key")}");
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}