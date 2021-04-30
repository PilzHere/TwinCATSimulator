namespace ConsoleApp1.Batteries
{
    abstract class Battery
    {
        private float terminalDiameter, outerDiameter; // milimeter
        private float totalLength, terminalLength; // milimeter

        public float TerminalDiameter
        {
            get { return terminalDiameter; }
            set { terminalDiameter = value; }
        }

        public float OuterDiameter
        {
            get { return outerDiameter; }
            set { outerDiameter = value; }
        }

        public float TotalLength
        {
            get { return totalLength; }
            set { totalLength = value; }
        }

        public float TerminalLength
        {
            get { return terminalLength; }
            set { terminalLength = value; }
        }
    }
}
