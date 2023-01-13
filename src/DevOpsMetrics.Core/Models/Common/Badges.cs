namespace DevOpsMetrics.Core.Models.Common
{
    public class Badges
    {
        public static string BadgeURL(string description, string rating)
        {
            string color;
            switch (rating)
            {
                case "Elite":
                    color = "brightgreen";
                    break;
                case "High":
                    color = "green";
                    break;
                case "Medium":
                    color = "orange";
                    break;
                case "Low":
                    color = "red";
                    break;
                default:
                    color = "lightgrey";
                    break;
            }

            //Example: https://img.shields.io/badge/Deployment%20frequency-High-green
            return $"https://img.shields.io/badge/{description}-{rating}-{color}";
        }
    }
}
