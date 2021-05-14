using System;

namespace ConsoleApp1.Recipes
{
    class BatteryRecipe
    {
        private string productName;
        private double startValueTotalLength, minValueTotalLength, maxValueTotalLength;
        private double startValueTotalDiameter, minValueTotalDiameter, maxValueTotalDiameter;
        private double startValueTerminalLength, minValueTerminalLength, maxValueTerminalLength;
        private double startValueTerminalDiameter, minValueTerminalDiameter, maxValueTerminalDiameter;

        private double tachometerMinValueTotalLength, tachometerMaxValueTotalLength;
        private double tachometerMinValueTotalDiameter, tachometerMaxValueTotalDiameter;
        private double tachometerMinValueTerminalLength, tachometerMaxValueTerminalLength;
        private double tachometerMinValueTerminalDiameter, tachometerMaxValueTerminalDiameter;

        public BatteryRecipe(string productName, double startValueTotalLength, double minValueTotalLength, double maxValueTotalLength, double startValueTotalDiameter, double minValueTotalDiameter, double maxValueTotalDiameter, double startValueTerminalLength, double minValueTerminalLength, double maxValueTerminalLength, double startValueTerminalDiameter, double minValueTerminalDiameter, double maxValueTerminalDiameter)
        {
            ProductName = productName;
            StartValueTotalLength = startValueTotalLength;
            MinValueTotalLength = minValueTotalLength;
            MaxValueTotalLength = maxValueTotalLength;
            StartValueTotalDiameter = startValueTotalDiameter;
            MinValueTotalDiameter = minValueTotalDiameter;
            MaxValueTotalDiameter = maxValueTotalDiameter;
            StartValueTerminalLength = startValueTerminalLength;
            MinValueTerminalLength = minValueTerminalLength;
            MaxValueTerminalLength = maxValueTerminalLength;
            StartValueTerminalDiameter = startValueTerminalDiameter;
            MinValueTerminalDiameter = minValueTerminalDiameter;
            MaxValueTerminalDiameter = maxValueTerminalDiameter;

            double outOfRangeValue = 0.2;
            tachometerMinValueTotalLength = minValueTotalLength - outOfRangeValue;
            tachometerMaxValueTotalLength = maxValueTotalLength + outOfRangeValue;
            tachometerMinValueTotalDiameter = minValueTotalDiameter - outOfRangeValue;
            tachometerMaxValueTotalDiameter = maxValueTotalDiameter + outOfRangeValue;
            tachometerMinValueTerminalLength = minValueTerminalLength - outOfRangeValue;
            tachometerMaxValueTerminalLength = maxValueTerminalLength + outOfRangeValue;
            tachometerMinValueTerminalDiameter = minValueTerminalDiameter - outOfRangeValue;
            tachometerMaxValueTerminalDiameter = maxValueTerminalDiameter + outOfRangeValue;
        }

        public string ProductName { get; private set; }
        public double StartValueTotalLength { get => startValueTotalLength; set => startValueTotalLength = value; }
        public double MinValueTotalLength { get => minValueTotalLength; set => minValueTotalLength = value; }
        public double MaxValueTotalLength { get => maxValueTotalLength; set => maxValueTotalLength = value; }
        public double StartValueTotalDiameter { get => startValueTotalDiameter; set => startValueTotalDiameter = value; }
        public double MinValueTotalDiameter { get => minValueTotalDiameter; set => minValueTotalDiameter = value; }
        public double MaxValueTotalDiameter { get => maxValueTotalDiameter; set => maxValueTotalDiameter = value; }
        public double StartValueTerminalLength { get => startValueTerminalLength; set => startValueTerminalLength = value; }
        public double MinValueTerminalLength { get => minValueTerminalLength; set => minValueTerminalLength = value; }
        public double MaxValueTerminalLength { get => maxValueTerminalLength; set => maxValueTerminalLength = value; }
        public double StartValueTerminalDiameter { get => startValueTerminalDiameter; set => startValueTerminalDiameter = value; }
        public double MinValueTerminalDiameter { get => minValueTerminalDiameter; set => minValueTerminalDiameter = value; }
        public double MaxValueTerminalDiameter { get => maxValueTerminalDiameter; set => maxValueTerminalDiameter = value; }
        public double TachometerMinValueTotalLength { get => tachometerMinValueTotalLength; set => tachometerMinValueTotalLength = value; }
        public double TachometerMaxValueTotalLength { get => tachometerMaxValueTotalLength; set => tachometerMaxValueTotalLength = value; }
        public double TachometerMinValueTotalDiameter { get => tachometerMinValueTotalDiameter; set => tachometerMinValueTotalDiameter = value; }
        public double TachometerMaxValueTotalDiameter { get => tachometerMaxValueTotalDiameter; set => tachometerMaxValueTotalDiameter = value; }
        public double TachometerMinValueTerminalLength { get => tachometerMinValueTerminalLength; set => tachometerMinValueTerminalLength = value; }
        public double TachometerMaxValueTerminalLength { get => tachometerMaxValueTerminalLength; set => tachometerMaxValueTerminalLength = value; }
        public double TachometerMinValueTerminalDiameter { get => tachometerMinValueTerminalDiameter; set => tachometerMinValueTerminalDiameter = value; }
        public double TachometerMaxValueTerminalDiameter { get => tachometerMaxValueTerminalDiameter; set => tachometerMaxValueTerminalDiameter = value; }
    }
}
