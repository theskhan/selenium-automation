using System;
using System.IO;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json.Serialization;

namespace SeleniumAutomationStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new StreamReader("url.txt");
            var url = reader.ReadToEnd();
            reader.Close();

            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            var settings = new Settings
            {
                SessionId = driver.SessionId.ToString(),
                ExecutorUrl = GetExecutorURLFromDriver(driver).ToString()
            };

            Console.WriteLine("SessionId: " + driver.SessionId);
            Console.WriteLine("Executor Url: " + GetExecutorURLFromDriver(driver));

            var settingsSerialized = JsonConvert.SerializeObject(settings, Formatting.Indented);
            var writer = new StreamWriter("selenium-settings.json");
            writer.Write(settingsSerialized);
            writer.Flush();
            writer.Close();
            writer.Dispose();

            Environment.SetEnvironmentVariable("SeleniumExecutorUrl", settings.ExecutorUrl);
            Environment.SetEnvironmentVariable("SeleniumSessionId", settings.SessionId);

            Console.ReadLine();
        }

        public static Uri GetExecutorURLFromDriver(OpenQA.Selenium.Remote.RemoteWebDriver driver)
        {
            var executorField = typeof(OpenQA.Selenium.Remote.RemoteWebDriver)
                .GetField("executor",
                    System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);

            object executor = executorField.GetValue(driver);

            var internalExecutorField = executor.GetType()
                .GetField("internalExecutor",
                    System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);
            object internalExecutor = internalExecutorField.GetValue(executor);

            //executor.CommandInfoRepository
            var remoteServerUriField = internalExecutor.GetType()
                .GetField("remoteServerUri",
                    System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance);
            var remoteServerUri = remoteServerUriField.GetValue(internalExecutor) as Uri;

            return remoteServerUri;
        }
    }
}
