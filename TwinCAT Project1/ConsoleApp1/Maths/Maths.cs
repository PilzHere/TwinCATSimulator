using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Maths
{
    class Maths
    {
        /// <summary>
        /// Method to generate a random value within a range. 
        /// Ex.value=8 and range = 0.2, gives a random return value between 7.8 and 8.2
        /// Return value rounded off to three decimals.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <param name="absoluteMinValue"></param>
        /// <param name="absoluteMaxValue"></param>
        /// <returns>double</returns>
        public static double GetRandomNumberWithRange(double value, double range, double absoluteMinValue, double absoluteMaxValue)
        {
            Random random = new Random();
            double minimum;
            double maximum;

            if (value < absoluteMinValue || value > absoluteMaxValue)
            {
                if (value < absoluteMinValue)
                {
                    minimum = value - range / 8;
                    maximum = value + range;
                }
                else
                {
                    minimum = value - range;
                    maximum = value + range / 8;
                }
            }
            else
            {
                minimum = value - range;
                maximum = value + range;
            }

            double answer = (random.NextDouble() * (maximum - minimum) + minimum);

            return Math.Round(answer, 3, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns wether value is withing in range or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns>true or false</returns>
        public static bool CheckIfValueIsWithinRange(double value, double minValue, double maxValue)
        {
            if (value >= minValue && value <= maxValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIfBatteryValuesAreOK(Batteries.BatteryInProduction batteryInProduction)
        {
            if (CheckIfValueIsWithinRange(batteryInProduction.TotalLength, batteryInProduction.MinValueTotalLength, batteryInProduction.MaxValueTotalLength) &&
            CheckIfValueIsWithinRange(batteryInProduction.TerminalLength, batteryInProduction.MinValueTerminalLength, batteryInProduction.MaxValueTerminalLength) &&
            CheckIfValueIsWithinRange(batteryInProduction.TotalDiameter, batteryInProduction.MinValueTotalDiameter, batteryInProduction.MaxValueTotalDiameter) &&
            CheckIfValueIsWithinRange(batteryInProduction.TerminalDiameter, batteryInProduction.MinValueTerminalDiameter, batteryInProduction.MaxValueTerminalDiameter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
