using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.DataAccess.Common
{
    public class FractionConverter
    {
        public FractionModel ConvertToFraction(int percentComplete)
        {
            FractionModel model = new FractionModel();

            //Using the percent, convert it to a fraction
            switch (percentComplete)
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
                case 100:
                    model.Numerator = 1;
                    model.Denominator = 1;
                    break;
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
        public int Numerator { get; set; }
        public int Denominator { get; set; }
    }
}
