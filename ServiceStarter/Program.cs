using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ServiceStarter
{
    class Program
    {
        static IConfiguration? Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();
            Configuration = builder.Build();

            var isProduction = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Production";

            var authPath = isProduction ? 
                Path.Combine(AppContext.BaseDirectory, "Auth", "Auth.exe") :
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\Auth\bin\Debug\net7.0\Auth.exe"));
            var dataProcessingPath = isProduction ? 
                Path.Combine(AppContext.BaseDirectory, "DataProcessing", "DataProcessing.exe") :
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\DataProcessing\bin\Debug\net7.0\DataProcessing.exe"));
            var apiGatewayPath = isProduction ? 
                Path.Combine(AppContext.BaseDirectory, "ApiGateway", "ApiGateway.exe") :
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\ApiGateway\bin\Debug\net7.0\ApiGateway.exe"));

            TryStartService("Auth", authPath);
            TryStartService("DataProcessing", dataProcessingPath);
            TryStartService("ApiGateway", apiGatewayPath);

            Console.WriteLine("All services have been started. Press any key to exit...");
            Console.ReadKey();
        }

        private static void TryStartService(string serviceName, string exePath)
        {
            if (!File.Exists(exePath))
            {
                Console.WriteLine($"Executable path for {serviceName} does not exist: {exePath}");
                return;
            }

            Console.WriteLine($"Attempting to start {serviceName} from {exePath}...");
            StartService(serviceName, exePath);
        }

        private static void StartService(string serviceName, string exePath)
        {
            Console.WriteLine($"Starting {serviceName} from {exePath}...");
            var processStartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                try
                {
                    process.Start();
                    Console.WriteLine($"{serviceName} started successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start {serviceName}. Error: {ex.Message}");
                }
            }
        }
    }
}
