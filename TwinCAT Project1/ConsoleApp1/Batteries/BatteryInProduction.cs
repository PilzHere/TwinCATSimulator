using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Batteries
{
    class BatteryInProduction
    {
        private double totalLength, minValueTotalLength, maxValueTotalLength;
        private double totalDiameter, minValueTotalDiameter, maxValueTotalDiameter;
        private double terminalLength, minValueTerminalLength, maxValueTerminalLength;
        private double terminalDiameter, minValueTerminalDiameter, maxValueTerminalDiameter;

        public BatteryInProduction(double totalLength, double minValueTotalLength, double maxValueTotalLength, double totalDiameter, double minValueTotalDiameter, double maxValueTotalDiameter, double terminalLength, double minValueTerminalLength, double maxValueTerminalLength, double terminalDiameter, double minValueTerminalDiameter, double maxValueTerminalDiameter)
        {
            this.TotalLength = totalLength;
            this.MinValueTotalLength = minValueTotalLength;
            this.MaxValueTotalLength = maxValueTotalLength;
            this.TotalDiameter = totalDiameter;
            this.MinValueTotalDiameter = minValueTotalDiameter;
            this.MaxValueTotalDiameter = maxValueTotalDiameter;
            this.TerminalLength = terminalLength;
            this.MinValueTerminalLength = minValueTerminalLength;
            this.MaxValueTerminalLength = maxValueTerminalLength;
            this.TerminalDiameter = terminalDiameter;
            this.MinValueTerminalDiameter = minValueTerminalDiameter;
            this.MaxValueTerminalDiameter = maxValueTerminalDiameter;
        }

        public double TotalLength { get => totalLength; set => totalLength = value; }
        public double MinValueTotalLength { get => minValueTotalLength; set => minValueTotalLength = value; }
        public double MaxValueTotalLength { get => maxValueTotalLength; set => maxValueTotalLength = value; }
        public double TotalDiameter { get => totalDiameter; set => totalDiameter = value; }
        public double MinValueTotalDiameter { get => minValueTotalDiameter; set => minValueTotalDiameter = value; }
        public double MaxValueTotalDiameter { get => maxValueTotalDiameter; set => maxValueTotalDiameter = value; }
        public double TerminalLength { get => terminalLength; set => terminalLength = value; }
        public double MinValueTerminalLength { get => minValueTerminalLength; set => minValueTerminalLength = value; }
        public double MaxValueTerminalLength { get => maxValueTerminalLength; set => maxValueTerminalLength = value; }
        public double TerminalDiameter { get => terminalDiameter; set => terminalDiameter = value; }
        public double MinValueTerminalDiameter { get => minValueTerminalDiameter; set => minValueTerminalDiameter = value; }
        public double MaxValueTerminalDiameter { get => maxValueTerminalDiameter; set => maxValueTerminalDiameter = value; }
    }
}
