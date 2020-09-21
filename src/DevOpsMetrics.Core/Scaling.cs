namespace DevOpsMetrics.Core
{
    public class Scaling
    {
        //scale the number, so that the lowest number is visible on the charts
        public static int ScaleNumberToRange(float number, float currentMin, float currentMax, float targetMin, float targetMax)
        {
            //https://stats.stackexchange.com/questions/281162/scale-a-number-between-a-range/281164
            int result = (int)(((number - currentMin) / (currentMax - currentMin) * (targetMax - targetMin)) + targetMin);
            return result;
        }

    }
}
