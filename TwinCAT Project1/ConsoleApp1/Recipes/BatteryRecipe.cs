using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Recipes
{
    class BatteryRecipe
    {
        private String productName;
        private double startValueTotalLength, minValueTotalLength, maxValueTotalLength;
        private double startValueTotalDiameter, minValueTotalDiameter, maxValueTotalDiameter;
        private double startValueTerminalLength, minValueTerminalLength, maxValueTerminalLength;
        private double startValueTerminalDiameter, minValueTerminalDiameter, maxValueTerminalDiameter;

        private double tachometerMinValueTotalLength, tachometerMaxValueTotalLength;
        private double tachometerMinValueTotalDiameter, tachometerMaxValueTotalDiameter;
        private double tachometerMinValueTerminalLength, tachometerMaxValueTerminalLength;
        private double tachometerMinValueTerminalDiameter, tachometerMaxValueTerminalDiameter;

        public BatteryRecipe(String productName, double startValueTotalLength, double minValueTotalLength, double maxValueTotalLength, double startValueTotalDiameter, double minValueTotalDiameter, double maxValueTotalDiameter, double startValueTerminalLength, double minValueTerminalLength, double maxValueTerminalLength, double startValueTerminalDiameter, double minValueTerminalDiameter, double maxValueTerminalDiameter)
        {
            this.ProductName = productName;
            this.StartValueTotalLength = startValueTotalLength;
            this.MinValueTotalLength = minValueTotalLength;
            this.MaxValueTotalLength = maxValueTotalLength;
            this.StartValueTotalDiameter = startValueTotalDiameter;
            this.MinValueTotalDiameter = minValueTotalDiameter;
            this.MaxValueTotalDiameter = maxValueTotalDiameter;
            this.StartValueTerminalLength = startValueTerminalLength;
            this.MinValueTerminalLength = minValueTerminalLength;
            this.MaxValueTerminalLength = maxValueTerminalLength;
            this.StartValueTerminalDiameter = startValueTerminalDiameter;
            this.MinValueTerminalDiameter = minValueTerminalDiameter;
            this.MaxValueTerminalDiameter = maxValueTerminalDiameter;

            double outOfRangeValue = 0.2;
            this.tachometerMinValueTotalLength = minValueTotalLength - outOfRangeValue;
            this.tachometerMaxValueTotalLength = maxValueTotalLength + outOfRangeValue;
            this.tachometerMinValueTotalDiameter = minValueTotalDiameter - outOfRangeValue;
            this.tachometerMaxValueTotalDiameter = maxValueTotalDiameter + outOfRangeValue;
            this.tachometerMinValueTerminalLength = minValueTerminalLength - outOfRangeValue;
            this.tachometerMaxValueTerminalLength = maxValueTerminalLength + outOfRangeValue;
            this.tachometerMinValueTerminalDiameter = minValueTerminalDiameter - outOfRangeValue;
            this.tachometerMaxValueTerminalDiameter = maxValueTerminalDiameter + outOfRangeValue;
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
