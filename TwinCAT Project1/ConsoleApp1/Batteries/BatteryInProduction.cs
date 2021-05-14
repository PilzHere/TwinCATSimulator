

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
            TotalLength = totalLength;
            MinValueTotalLength = minValueTotalLength;
            MaxValueTotalLength = maxValueTotalLength;
            TotalDiameter = totalDiameter;
            MinValueTotalDiameter = minValueTotalDiameter;
            MaxValueTotalDiameter = maxValueTotalDiameter;
            TerminalLength = terminalLength;
            MinValueTerminalLength = minValueTerminalLength;
            MaxValueTerminalLength = maxValueTerminalLength;
            TerminalDiameter = terminalDiameter;
            MinValueTerminalDiameter = minValueTerminalDiameter;
            MaxValueTerminalDiameter = maxValueTerminalDiameter;
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
