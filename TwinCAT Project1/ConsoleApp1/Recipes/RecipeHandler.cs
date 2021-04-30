using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Recipes
{
    class RecipeHandler
    {
        public static List<BatteryRecipe> ReadInAllBatteriesRecipies(List<BatteryRecipe> list)
        {
            list.Add(new BatteryRecipe("AA Battery", 50.5, 50.3, 50.7, 14.0, 13.8, 14.2, 1.9, 1.7, 2.1, 5.3, 5.1, 5.5));
            list.Add(new BatteryRecipe("AAA Battery", 44.2, 44.0, 44.4, 10.5, 10.3, 10.7, 1.2, 1.0, 1.4, 3.4, 3.2, 3.6));
            return list;
        }

        public static BatteryRecipe ReadInRecipie(List<BatteryRecipe> list, int index)
        {
            String productName = list.ElementAt(index).ProductName;
            double startValueTotalLength = list.ElementAt(index).StartValueTotalLength;
            double minValueTotalLength = list.ElementAt(index).MinValueTotalLength;
            double maxValueTotalLength = list.ElementAt(index).MaxValueTotalLength;

            double startValueTotalDiameter = list.ElementAt(index).StartValueTotalDiameter;
            double minValueTotalDiameter = list.ElementAt(index).MinValueTotalDiameter;
            double maxValueTotalDiameter = list.ElementAt(index).MaxValueTotalDiameter;

            double startValueTerminalLength = list.ElementAt(index).StartValueTerminalLength;
            double minValueTerminalLength = list.ElementAt(index).MinValueTerminalLength;
            double maxValueTerminalLength = list.ElementAt(index).MaxValueTerminalLength;

            double startValueTerminalDiameter = list.ElementAt(index).StartValueTerminalDiameter;
            double minValueTerminalDiameter = list.ElementAt(index).MinValueTerminalDiameter;
            double maxValueTerminalDiameter = list.ElementAt(index).MaxValueTerminalDiameter;

            BatteryRecipe recepieInUse = new BatteryRecipe(productName, startValueTotalLength, minValueTotalLength,
                maxValueTotalLength, startValueTotalDiameter, minValueTotalDiameter, maxValueTotalDiameter, startValueTerminalLength,
                minValueTerminalLength, maxValueTerminalLength, startValueTerminalDiameter, minValueTerminalDiameter, maxValueTerminalDiameter);

            return recepieInUse;
        }
    }
}
