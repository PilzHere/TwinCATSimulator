using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        private TwinCAT.TwinCATVariable<Int32> productCount = null;
        private TwinCAT.TwinCATVariable<Int32> productPassed = null;
        private TwinCAT.TwinCATVariable<Int32> productFailed = null;

        private TwinCAT.TwinCATVariable<Byte[]> imageOfProduct = null;

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

            productCount = varBuilder.Build<Int32>("Task.Outputs.productCount");
            productPassed = varBuilder.Build<Int32>("Task.Outputs.productPassed");
            productFailed = varBuilder.Build<Int32>("Task.Outputs.productFailed");

            imageOfProduct = varBuilder.Build<Byte[]>("Task.Outputs.picture");

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

        private void ClearProductCounters()
        {
            productCount.Value = 0;
            productPassed.Value = 0;
            productFailed.Value = 0; // Does not update sometimes?
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

                int randomSleepTime = 50; // ms
                int productSleepTime = 800; //ms

                double randomRange = 0.05; // sets the range in method Maths.Maths.GetRandomNumberWithRange()

                // set product only when changed.
                if (productOption != oldProductOption)
                {
                    ClearProductCounters();

                    // Blank image to clear the image box
                    List<int> blankImageList = new List<int> { 857 };
                    String path = "Images/Blank/";
                    ShowImage(blankImageList, path);

                    switch (productOption)
                    {

                        case 0: // Simulate AA Batteries
                            Console.WriteLine("Chosen product: AA Battery.");
                            recepieInUse = Recipes.RecipeHandler.ReadInRecipie(allBatteriesRecipiesList, 0); // AA
                            SetStartValues(recepieInUse);

                            firstStart = true;
                            SetNewSimState(false);

                            // Update tachometer's values.
                            SetTachometersRange(recepieInUse);

                            break;
                        case 1:
                            Console.WriteLine("Chosen product: AAA Battery.");

                            recepieInUse = Recipes.RecipeHandler.ReadInRecipie(allBatteriesRecipiesList, 1); // AAA
                            SetStartValues(recepieInUse);

                            firstStart = true;
                            SetNewSimState(false);

                            // Update tachometer's values.
                            SetTachometersRange(recepieInUse);

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
                    case 0: // Simulate AA Batteries
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
                                    ClearProductCounters();
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
                            bool toBreakIfPaused = false;

                            while (true)
                            {
                                simulatorState.Polling = true;
                                simState = simulatorState.Value;
                                if (!simState)
                                {
                                    toBreakIfPaused = true;
                                    break;
                                }

                                // Get a random number for the newly produced battery
                                double tempValueTotalLength = Maths.Maths.GetRandomNumberWithRange(startValueTotalLength, randomRange, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength);
                                Thread.Sleep(randomSleepTime); // Sleeping for better random values within one product
                                double tempValueTotalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTotalDiameter, randomRange, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter);
                                Thread.Sleep(randomSleepTime);
                                double tempValueTerminalLength = Maths.Maths.GetRandomNumberWithRange(startValueTerminalLength, randomRange, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength);
                                Thread.Sleep(randomSleepTime);
                                double tempValueTerminalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTerminalDiameter, randomRange, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);
                                Thread.Sleep(randomSleepTime);

                                // Produce one battery

                                List<int> AAbatteryList = new List<int> { 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848, 849, 850, 851, 852, 853, 854, 855, 856 };
                                String path = "Images/AA/";
                                ShowImage(AAbatteryList, path);

                                Batteries.BatteryInProduction batteryInProduction = new Batteries.BatteryInProduction(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength, tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter, tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength, tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);

                                tachom1RealValue.Value = batteryInProduction.TotalLength;
                                tachom2RealValue.Value = batteryInProduction.TotalDiameter;
                                tachom3RealValue.Value = batteryInProduction.TerminalLength;
                                tachom4RealValue.Value = batteryInProduction.TerminalDiameter;

                                Thread.Sleep(productSleepTime);

                                // Check if battery is OK or NOK
                                if (Maths.Maths.CheckIfBatteryValuesAreOK(batteryInProduction))
                                {
                                    okBatteriesProducedList.Add(batteryInProduction);
                                    productPassed.Value = okBatteriesProducedList.Count;
                                }
                                else
                                {
                                    nokBatteriesProducedList.Add(batteryInProduction);
                                    productFailed.Value = nokBatteriesProducedList.Count;
                                }

                                productCount.Value = okBatteriesProducedList.Count + nokBatteriesProducedList.Count;

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
                            if (toBreakIfPaused) break;

                            SetNewSimState(false);
                            firstStart = true;

                            //Console.WriteLine("This line should NOT be printed if paused...");
                        }

                        break;
                    case 1: // Simulate AAA Batteries
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
                                    ClearProductCounters();
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
                            bool toBreakIfPaused = false;

                            while (true)
                            {
                                simulatorState.Polling = true;
                                simState = simulatorState.Value;
                                if (!simState)
                                {
                                    toBreakIfPaused = true;
                                    break;
                                }

                                // Get a random number for the newly produced battery
                                double tempValueTotalLength = Maths.Maths.GetRandomNumberWithRange(startValueTotalLength, randomRange, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength);
                                Thread.Sleep(randomSleepTime); // Sleeping for better random values within one product
                                double tempValueTotalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTotalDiameter, randomRange, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter);
                                Thread.Sleep(randomSleepTime);
                                double tempValueTerminalLength = Maths.Maths.GetRandomNumberWithRange(startValueTerminalLength, randomRange, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength);
                                Thread.Sleep(randomSleepTime);
                                double tempValueTerminalDiameter = Maths.Maths.GetRandomNumberWithRange(startValueTerminalDiameter, randomRange, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);
                                Thread.Sleep(randomSleepTime);

                                // Produce one battery

                                List<int> AAAbatteryList = new List<int> { 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823 };
                                String path = "Images/AAA/";
                                ShowImage(AAAbatteryList, path);

                                Batteries.BatteryInProduction batteryInProduction = new Batteries.BatteryInProduction(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength, tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter, tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength, tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter);

                                tachom1RealValue.Value = batteryInProduction.TotalLength;
                                tachom2RealValue.Value = batteryInProduction.TotalDiameter;
                                tachom3RealValue.Value = batteryInProduction.TerminalLength;
                                tachom4RealValue.Value = batteryInProduction.TerminalDiameter;

                                Thread.Sleep(productSleepTime);

                                // Check if battery is OK or NOK
                                if (Maths.Maths.CheckIfBatteryValuesAreOK(batteryInProduction))
                                {
                                    okBatteriesProducedList.Add(batteryInProduction);
                                    productPassed.Value = okBatteriesProducedList.Count;
                                }
                                else
                                {
                                    nokBatteriesProducedList.Add(batteryInProduction);
                                    productFailed.Value = nokBatteriesProducedList.Count;
                                }

                                productCount.Value = okBatteriesProducedList.Count + nokBatteriesProducedList.Count;

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
                            if (toBreakIfPaused) break;

                            SetNewSimState(false);
                            firstStart = true;

                            //Console.WriteLine("This line should NOT be printed if paused...");
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

        /// <summary>
        /// Sets the tachometers range when chosing a product
        /// </summary>
        /// <param name="recepieInUse"></param>
        private void SetTachometersRange(Recipes.BatteryRecipe recepieInUse)
        {
            int numberOfDecimals = 1; // sets the number of decimals that will be returned

            tachom1MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom1MinValue.Value = Math.Round(recepieInUse.MinValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom1RealValue.Value = 0;
            tachom1MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom1MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);

            tachom2MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom2MinValue.Value = Math.Round(recepieInUse.MinValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom2RealValue.Value = 0;
            tachom2MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom2MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);

            tachom3MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom3MinValue.Value = Math.Round(recepieInUse.MinValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom3RealValue.Value = 0;
            tachom3MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom3MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);

            tachom4MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom4MinValue.Value = Math.Round(recepieInUse.MinValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom4RealValue.Value = 0;
            tachom4MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachom4MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);

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
        /// Sends imagepath to HMI
        /// </summary>
        /// <param name="list"></param>
        /// <param name="path"></param>
        public void ShowImage(List<int> list, String path)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, list.Count);
            string value = path + "DSC05" + list[randomIndex] + ".JPG";
            byte[] temp = Encoding.UTF8.GetBytes(value);
            byte[] toBeSended = new byte[256];

            for (int i = 0; i < temp.Length; i++)
            {
                toBeSended[i] = temp[i];
            }

            imageOfProduct.Value = toBeSended;

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
