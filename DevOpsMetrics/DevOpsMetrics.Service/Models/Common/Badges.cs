namespace DevOpsMetrics.Service.Models.Common
{
    public class Badges
    {
        public static string BadgeURL(string description, string rating)
        {
            string color = Badges.BadgeColor(rating);
            //Example: https://img.shields.io/badge/Deployment%20frequency-Elite-brightgreen
            return $"https://img.shields.io/badge/{description}-{rating}-{color}";
        }

        private static string BadgeColor(string rating)
        {
            switch (rating)
            {
                case "Elite":
                    return "brightgreen";
                case "High":
                    return "green";
                case "Medium":
                    return "orange";
                case "Low":
                    return "red";
                default: 
                    return "white";
            }
        }
    }
}
