using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Sweater.Api
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Program
    {
        private const int Port = 8080;

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Run();
        }

        private static IWebHost CreateWebHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var currentDir = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(currentDir)
                .AddJsonFile("certificate.json", true, true)
                .AddJsonFile($"certificate.{environment}.json", true, true)
                .AddJsonFile("indexers.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseContentRoot(currentDir)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, Port, listenOptions =>
                    {
                        var certificateSettings = config.GetSection("certificateSettings");

                        listenOptions.UseHttps(new X509Certificate2(
                            certificateSettings.GetValue<string>("filename")
                            , certificateSettings.GetValue<string>("password")
                        ));
                    });
                })
                .Build();
        }
    }
}