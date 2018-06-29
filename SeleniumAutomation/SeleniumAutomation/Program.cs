using System;
using Serilog;

namespace SeleniumAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupLogger();
            var settings = new Settings
            {
                ExecutorUrl = Environment.GetEnvironmentVariable("SeleniumExecutorUrl", EnvironmentVariableTarget.User),
                SessionId = Environment.GetEnvironmentVariable("SeleniumSessionId", EnvironmentVariableTarget.User)
            };

            if (string.IsNullOrEmpty(settings.ExecutorUrl))
            {
                Console.WriteLine("Unable to read settings from Environment Variable");
                Console.WriteLine("Enter Selenium Executor Url...");
                settings.ExecutorUrl = Console.ReadLine();

                Console.WriteLine("Enter Selenium Session Id...");
                settings.SessionId = Console.ReadLine();
            }

            new Executor().Execute(settings.ExecutorUrl, settings.SessionId);

            Log.CloseAndFlush();
        }

        private static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }
    }
}