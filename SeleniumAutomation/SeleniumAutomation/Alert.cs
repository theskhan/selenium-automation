using System;

namespace SeleniumAutomation
{
    public class Alert
    {
        public string Scrip { get; set; }
        public DateTime GeneratedOn { get; set; }
        public DateTime? OrderPlacedOn { get; set; }
        public bool OrderPlaced { get; set; }
        public string OrderType { get; set; }

        public override string ToString()
        {
            return $"Order {OrderType} placed for {Scrip} on {OrderPlacedOn} ({GeneratedOn})";
        }
    }
}