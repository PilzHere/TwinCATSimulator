using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleApp1
{
    class PLCSimulator
    {
        private bool exit;

        public bool Exit { get => exit; set => exit = value; }

        public PLCSimulator() // TODO: maybe change name?
        {
            ConnectToPLC();
            SetupVariables();
            RunLoop();
        }

        List<Recipes.BatteryRecipe> allBatteriesRecipiesList = new List<Recipes.BatteryRecipe>();
        List<Batteries.BatteryInProduction> okBatteriesProducedList = new List<Batteries.BatteryInProduction>();
        List<Batteries.BatteryInProduction> nokBatteriesProducedList = new List<Batteries.BatteryInProduction>();

        private TwinCAT.TwinCATVariableBuilder varBuilder = null;
        private TwinCAT.TwinCATVariable<Boolean> simulatorState = null;
        private TwinCAT.TwinCATVariable<Int32> productOpt = null;

        private TwinCAT.TwinCATVariable<Double> tachom1MinOutOfRange = null;
        private TwinCAT.TwinCATVariable<Double> tachom1MinValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom1RealValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom1MaxValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom1MaxOutOfRange = null;

        private TwinCAT.TwinCATVariable<Double> tachom2MinOutOfRange = null;
        private TwinCAT.TwinCATVariable<Double> tachom2MinValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom2RealValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom2MaxValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom2MaxOutOfRange = null;

        private TwinCAT.TwinCATVariable<Double> tachom3MinOutOfRange = null;
        private TwinCAT.TwinCATVariable<Double> tachom3MinValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom3RealValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom3MaxValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom3MaxOutOfRange = null;

        private TwinCAT.TwinCATVariable<Double> tachom4MinOutOfRange = null;
        private TwinCAT.TwinCATVariable<Double> tachom4MinValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom4RealValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom4MaxValue = null;
        private TwinCAT.TwinCATVariable<Double> tachom4MaxOutOfRange = null;

        Recipes.BatteryRecipe recepieInUse = null;

        // Set start values for product in use
        double startValueTotalLength, startValueTotalDiameter, startValueTerminalLength, startValueTerminalDiameter;

        int itemsToProduce = 10; //TODO: should be input from HMI.

        private void SetupVariables()
        {
            simulatorState = varBuilder.Build<Boolean>("Task.Outputs.Out1");
            productOpt = varBuilder.Build<Int32>("Task.Outputs.Out2");

            tachom1MinOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom1MinOutOfRange");
            tachom1MinValue = varBuilder.Build<Double>("Task.Outputs.tachom1MinValue");
            tachom1RealValue = varBuilder.Build<Double>("Task.Outputs.tachom1RealValue");
            tachom1MaxValue = varBuilder.Build<Double>("Task.Outputs.tachom1MaxValue");
            tachom1MaxOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom1MaxOutOfRange");

            tachom2MinOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom2MinOutOfRange");
            tachom2MinValue = varBuilder.Build<Double>("Task.Outputs.tachom2MinValue");
            tachom2RealValue = varBuilder.Build<Double>("Task.Outputs.tachom2RealValue");
            tachom2MaxValue = varBuilder.Build<Double>("Task.Outputs.tachom2MaxValue");
            tachom2MaxOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom2MaxOutOfRange");

            tachom3MinOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom3MinOutOfRange");
            tachom3MinValue = varBuilder.Build<Double>("Task.Outputs.tachom3MinValue");
            tachom3RealValue = varBuilder.Build<Double>("Task.Outputs.tachom3RealValue");
            tachom3MaxValue = varBuilder.Build<Double>("Task.Outputs.tachom3MaxValue");
            tachom3MaxOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom3MaxOutOfRange");

            tachom4MinOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom4MinOutOfRange");
            tachom4MinValue = varBuilder.Build<Double>("Task.Outputs.tachom4MinValue");
            tachom4RealValue = varBuilder.Build<Double>("Task.Outputs.tachom4RealValue");
            tachom4MaxValue = varBuilder.Build<Double>("Task.Outputs.tachom4MaxValue");
            tachom4MaxOutOfRange = varBuilder.Build<Double>("Task.Outputs.tachom4MaxOutOfRange");

            // Read in all recipies
            Recipes.RecipeHandler.ReadInAllBatteriesRecipies(allBatteriesRecipiesList);
        }

        private void SetStartValues(Recipes.BatteryRecipe recepieInUse)
        {
            startValueTotalLength = recepieInUse.StartValueTotalLength;
            startValueTotalDiameter = recepieInUse.StartValueTotalDiameter;
            startValueTerminalLength = recepieInUse.StartValueTerminalLength;
            startValueTerminalDiameter = recepieInUse.StartValueTerminalDiameter;
        }

        /// <summary>
        /// Use last battery's data as a starting point for the next battery.
        /// </summary>
        /// <param name="totalLength"></param>
        /// <param name="totalDiameter"></param>
        /// <param name="terminalLength"></param>
        /// <param name="terminalDiameter"></param>
        private void SetStartValues(double totalLength, double totalDiameter, double terminalLength, double terminalDiameter)
        {
            startValueTotalLength = totalLength;
            startValueTotalDiameter = totalDiameter;
            startValueTerminalLength = terminalLength;
            startValueTerminalDiameter = terminalDiameter;
        }

        private void ClearOKAndNOKBatteriesLists()
        {
            okBatteriesProducedList.Clear();
            nokBatteriesProducedList.Clear();
        }

        bool simState = false;
        bool oldSimState = false;

        /// <summary>
        /// Set a new simstate to HMI output and local state and old local state.
        /// </summary>
        /// <param name="newSimState"></param>
        private void SetNewSimState(bool newSimState)
        {
            simulatorState.Value = newSimState;
            simState = simulatorState.Value;
            oldSimState = simState;
        }

        /// <summary>
        /// The main loop of the program.
        /// This is where items are produced.
        /// </summary>
        private void RunLoop()
        {
            int productOption = 0; //TODO: Use an Enum for this. Current supported values are 0-2.
            int oldProductOption = -1;

            bool firstStart = true;

            while (!Exit)
            {
                productOpt.Polling = true;
                productOption = productOpt.Value;

                // set product only when changed.
                if (productOption != oldProductOption)
                {
                    switch (productOption)
                    {
                        case 0:
                            // TODO: remove this. Option will be removed from HMI.
                            Console.WriteLine("Chosen product: nothing.");

                            firstStart = true;
                            SetNewSimState(false);

                            break;
                        case 1: // Simulate AA Batteries
                            Console.WriteLine("Chosen product: AA Battery.");
                            recepieInUse = Recipes.RecipeHandler.ReadInRecipie(allBatteriesRecipiesList, 0); // AA
                            SetStartValues(recepieInUse);

                            firstStart = true;
                            SetNewSimState(false);

                            // Update tachometer's values.
                            tachom1MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalLength, 1, MidpointRounding.AwayFromZero);
                            tachom1MinValue.Value = Math.Round(recepieInUse.MinValueTotalLength, 1, MidpointRounding.AwayFromZero);
                            tachom1RealValue.Value = 0;
                            tachom1MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalLength, 1, MidpointRounding.AwayFromZero);
                            tachom1MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalLength, 1, MidpointRounding.AwayFromZero);

                            tachom2MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom2MinValue.Value = Math.Round(recepieInUse.MinValueTotalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom2RealValue.Value = 0;
                            tachom2MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom2MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalDiameter, 1, MidpointRounding.AwayFromZero);

                            tachom3MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalLength, 1, MidpointRounding.AwayFromZero);
                            tachom3MinValue.Value = Math.Round(recepieInUse.MinValueTerminalLength, 1, MidpointRounding.AwayFromZero);
                            tachom3RealValue.Value = 0;
                            tachom3MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalLength, 1, MidpointRounding.AwayFromZero);
                            tachom3MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalLength, 1, MidpointRounding.AwayFromZero);

                            tachom4MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom4MinValue.Value = Math.Round(recepieInUse.MinValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom4RealValue.Value = 0;
                            tachom4MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom4MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);

                            break;
                        case 2:
                            Console.WriteLine("Chosen product: AAA Battery.");

                            recepieInUse = Recipes.RecipeHandler.ReadInRecipie(allBatteriesRecipiesList, 1); // AAA
                            SetStartValues(recepieInUse);

                            firstStart = true;
                            SetNewSimState(false);

                            // Update tachometer's values.
                            tachom1MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalLength, 1, MidpointRounding.AwayFromZero);
                            tachom1MinValue.Value = Math.Round(recepieInUse.MinValueTotalLength, 1, MidpointRounding.AwayFromZero);
                            tachom1RealValue.Value = 0;
                            tachom1MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalLength, 1, MidpointRounding.AwayFromZero);
                            tachom1MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalLength, 1, MidpointRounding.AwayFromZero);

                            tachom2MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom2MinValue.Value = Math.Round(recepieInUse.MinValueTotalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom2RealValue.Value = 0;
                            tachom2MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom2MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalDiameter, 1, MidpointRounding.AwayFromZero);

                            tachom3MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalLength, 1, MidpointRounding.AwayFromZero);
                            tachom3MinValue.Value = Math.Round(recepieInUse.MinValueTerminalLength, 1, MidpointRounding.AwayFromZero);
                            tachom3RealValue.Value = 0;
                            tachom3MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalLength, 1, MidpointRounding.AwayFromZero);
                            tachom3MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalLength, 1, MidpointRounding.AwayFromZero);

                            tachom4MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom4MinValue.Value = Math.Round(recepieInUse.MinValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom4RealValue.Value = 0;
                            tachom4MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);
                            tachom4MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalDiameter, 1, MidpointRounding.AwayFromZero);

                            break;
                        default:
                            Console.WriteLine("That is not an option.");

                            firstStart = true;
                            SetNewSimState(false);

                            break;
                    }

                    oldProductOption = productOption;
                }

                // run manufacturing process of chosen product
                switch (productOption)
                {
                    case 0:

                        break;
                    case 1: // Simulate AA Batteries
                        simulatorState.Polling = true;
                        simState = simulatorState.Value;

                        if (simState != oldSimState)
                        {
                            if (simState)
                            {
                                Console.WriteLine("STARTED AA Batteries!");

                                if (firstStart)
                                {
                                    ClearOKAndNOKBatteriesLists();
                                    firstStart = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("PAUSED AA Batteries!");
                            }

                            oldSimState = simState;
                        }

                        if (simState)
                        {
                            int sleepTime = 50; // ms

                            while (true)
                            {
                                // Get a random number for the newly produced battery
                                double tempValueTotalLength = Maths.Maths.GetRandomNumberWithRange(startValueTotalLength, 0.1, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength);
                                Thread.Sleep(sleepTime); // Sleeping for better random values within one product
                                double tempValueTotalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTotalDiameter, 0.1, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter);
                                Thread.Sleep(sleepTime);
                                double tempValueTerminalLength = Maths.Maths.GetRandomNumberWithRange(startValueTerminalLength, 0.1, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength);
                                Thread.Sleep(sleepTime);
                                double tempValueTerminalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTerminalDiameter, 0.1, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);
                                Thread.Sleep(sleepTime);

                                // Produce one battery
                                Batteries.BatteryInProduction batteryInProduction = new Batteries.BatteryInProduction(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength, tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter, tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength, tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);

                                tachom1RealValue.Value = batteryInProduction.TotalLength;
                                tachom2RealValue.Value = batteryInProduction.TotalDiameter;
                                tachom3RealValue.Value = batteryInProduction.TerminalLength;
                                tachom4RealValue.Value = batteryInProduction.TerminalDiameter;

                                // Check if battery is OK or NOK
                                if (Maths.Maths.CheckIfBatteryValuesAreOK(batteryInProduction))
                                {
                                    okBatteriesProducedList.Add(batteryInProduction);
                                }
                                else
                                {
                                    nokBatteriesProducedList.Add(batteryInProduction);
                                }

                                // Print out values for each measure of the battery
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength), tempValueTotalLength);
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter), tempValueTotalDiameter);
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength), tempValueTerminalLength);
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter), tempValueTerminalDiameter);

                                // New line for the next products printout
                                Console.WriteLine();

                                SetStartValues(tempValueTotalLength, tempValueTotalDiameter, tempValueTerminalLength, tempValueTerminalDiameter);

                                if (okBatteriesProducedList.Count == itemsToProduce)
                                {
                                    tachom1RealValue.Value = 0;
                                    tachom2RealValue.Value = 0;
                                    tachom3RealValue.Value = 0;
                                    tachom4RealValue.Value = 0;
                                    break;
                                }
                            }

                            PrintProductOKNOKData();
                            SetNewSimState(false);
                            firstStart = true;
                        }

                        break;
                    case 2: // Simulate AAA Batteries
                        simulatorState.Polling = true;
                        simState = simulatorState.Value;

                        if (simState != oldSimState)
                        {
                            if (simState)
                            {
                                Console.WriteLine("STARTED AAA Batteries!");

                                if (firstStart)
                                {
                                    ClearOKAndNOKBatteriesLists();
                                    firstStart = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("PAUSED AAA Batteries!");
                            }

                            oldSimState = simState;
                        }

                        if (simState)
                        {
                            int sleepTime = 50; // ms

                            while (true)
                            {
                                // Get a random number for the newly produced battery
                                double tempValueTotalLength = Maths.Maths.GetRandomNumberWithRange(startValueTotalLength, 0.1, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength);
                                Thread.Sleep(sleepTime); // Sleeping for better random values within one product
                                double tempValueTotalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTotalDiameter, 0.1, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter);
                                Thread.Sleep(sleepTime);
                                double tempValueTerminalLength = Maths.Maths.GetRandomNumberWithRange(startValueTerminalLength, 0.1, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength);
                                Thread.Sleep(sleepTime);
                                double tempValueTerminalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTerminalDiameter, 0.1, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);
                                Thread.Sleep(sleepTime);

                                // Produce one battery
                                Batteries.BatteryInProduction batteryInProduction = new Batteries.BatteryInProduction(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength, tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter, tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength, tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);

                                tachom1RealValue.Value = batteryInProduction.TotalLength;
                                tachom2RealValue.Value = batteryInProduction.TotalDiameter;
                                tachom3RealValue.Value = batteryInProduction.TerminalLength;
                                tachom4RealValue.Value = batteryInProduction.TerminalDiameter;

                                // Check if battery is OK or NOK
                                if (Maths.Maths.CheckIfBatteryValuesAreOK(batteryInProduction))
                                {
                                    okBatteriesProducedList.Add(batteryInProduction);
                                }
                                else
                                {
                                    nokBatteriesProducedList.Add(batteryInProduction);
                                }

                                // Print out values for each measure of the battery
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength), tempValueTotalLength);
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter), tempValueTotalDiameter);
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength), tempValueTerminalLength);
                                PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter), tempValueTerminalDiameter);

                                // New line for the next products printout
                                Console.WriteLine();

                                SetStartValues(tempValueTotalLength, tempValueTotalDiameter, tempValueTerminalLength, tempValueTerminalDiameter);

                                if (okBatteriesProducedList.Count == itemsToProduce)
                                {
                                    tachom1RealValue.Value = 0;
                                    tachom2RealValue.Value = 0;
                                    tachom3RealValue.Value = 0;
                                    tachom4RealValue.Value = 0;
                                    break;
                                }
                            }

                            PrintProductOKNOKData();
                            SetNewSimState(false);
                            firstStart = true;
                        }
                        break;
                    default:
                        Console.WriteLine("That is not an option. 2");

                        break;
                }
            }

            Console.WriteLine("Exiting program...");
            Environment.Exit(0);
        }

        private void PrintProductOKNOKData()
        {
            Console.WriteLine();
            Console.WriteLine("Total OK: " + okBatteriesProducedList.Count);
            Console.WriteLine("Total NOK: " + nokBatteriesProducedList.Count);
        }

        /// <summary>
        /// Prints data in color depending on boolean condition. Green = true, else red.
        /// </summary>
        /// <param name="myBool"></param>
        /// <param name="value"></param>
        public void PrintOut(bool myBool, double value)
        {
            if (myBool)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(value + " ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(value + " ");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Connects to PLC at localhost via ADS.
        /// </summary>
        private void ConnectToPLC()
        {
            Console.WriteLine("Connecting to PLC...");
            varBuilder = new TwinCAT.TwinCATVariableBuilder(); // Connects to PLC.
        }

        private void StopSimulation()
        {
            Exit = true;
        }
    }
}
