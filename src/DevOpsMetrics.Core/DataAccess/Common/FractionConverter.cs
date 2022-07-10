namespace DevOpsMetrics.Core.DataAccess.Common
{
    public class FractionConverter
    {
        public static FractionModel ConvertToFraction(int percent)
        {
            FractionModel model = new();

            //Using the percent, convert it to a fraction
            switch (percent)
            {
                case 0:
                    model.Numerator = 0;
                    model.Denominator = 1;
                    break;
                case 10:
                    model.Numerator = 1;
                    model.Denominator = 10;
                    break;
                case 25:
                    model.Numerator = 1;
                    model.Denominator = 4;
                    break;
                case 50:
                    model.Numerator = 1;
                    model.Denominator = 2;
                    break;
                case 75:
                    model.Numerator = 3;
                    model.Denominator = 4;
                    break;
                case 98:
                    model.Numerator = 49;
                    model.Denominator = 50;
                    break;
                case 100: //when it's 100% or something else 
                default:
                    model.Numerator = 1;
                    model.Denominator = 1;
                    break;
            }

            return model;
        }

    }

    public class FractionModel
    {
        public int Numerator
        {
            get; set;
        }
        public int Denominator
        {
            get; set;
        }
    }
}
