using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Serilog;

namespace SeleniumAutomation
{
    public class Executor
    {
        private List<Alert> _triggeredAlerts = new List<Alert>();
        private DateTime _exitProgramAt = DateTime.Parse("2:45 PM");
        private int _probeDelayInMilliseconds = 2000;

        public void Execute(string url, string sessionId)
        {
            var driver = new ReuseRemoteWebDriver(new Uri(url), sessionId);

            while (true)
            {
                try
                {
                    // if (DateTime.Now > _exitProgramAt) break;
                    System.Threading.Thread.Sleep(_probeDelayInMilliseconds);

                    Console.WriteLine("\nProcessing....");

                    var alertListItemElement = driver.FindElementsByCssSelector("#notification_details > li");

                    foreach (var alertElement in alertListItemElement)
                    {
                        var notifTimeStamp = alertElement.FindElement(By.ClassName("notif_time_stamp")).Text;
                        var alertTimeStamp = DateTime.Parse(notifTimeStamp);

                        var alertObject = new Alert
                        {
                            GeneratedOn = alertTimeStamp,
                            OrderPlacedOn = DateTime.Now
                        };

                        // Get the scrip name
                        var notifDescription = alertElement.FindElement(By.ClassName("notif_desc")).Text;

                        if (notifDescription.Contains(" has expired")) continue;
                        if (notifDescription.StartsWith("Bought ")) continue;

                        if (notifDescription.Contains(" of "))
                        {
                            var split1 = notifDescription.Split(" of ");
                            alertObject.Scrip = split1[1].Split(" ")[0];
                        }

                        if (notifDescription.StartsWith("SL-M"))
                            alertObject.OrderType = "SL-M";
                        else if (notifDescription.StartsWith("BUY"))
                            alertObject.OrderType = "BUY";
                        else if (notifDescription.StartsWith("SELL"))
                            alertObject.OrderType = "SELL";

                        // check if this is not already executed
                        var triggeredAlert = GetAlertByTimeStampAndScrip(alertObject.Scrip, alertTimeStamp);
                        if (triggeredAlert != null && triggeredAlert.OrderPlaced) continue;

                        // allow 15 seconds grace period
                        //if ((DateTime.Now -  alertTimeStamp).TotalSeconds > 15) continue;

                        //var notifWindow = webElement.FindElement(By.ClassName("notif_window"));
                        //if (notifWindow == null) continue;

                        alertObject.OrderPlacedOn = DateTime.Now;
                        alertObject.OrderPlaced = true;

                        Log.Information(alertObject.ToString());
                        _triggeredAlerts.Add(alertObject);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n\nERROR: " + ex.ToString() + "\n\n");
                    Log.Error(ex.ToString());
                }
            }
        }

        public Alert GetAlertByTimeStampAndScrip(string scrip, DateTime timeStamp)
        {
            foreach (var triggeredAlert in _triggeredAlerts)
            {
                if (triggeredAlert.Scrip == scrip && triggeredAlert.GeneratedOn == timeStamp) return triggeredAlert;
            }

            return null;
        }
    }
}
