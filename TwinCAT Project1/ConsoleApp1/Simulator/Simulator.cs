using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    class Simulator
    {
        private bool exit;

        public bool Exit { get => exit; set => exit = value; }

        public Simulator()
        {
            ConnectToADS();
            SetupVariables();
            Run();
        }

        private Recipes.BatteryRecipe recepieInUse = null;

        private List<Recipes.BatteryRecipe> allBatteriesRecipiesList = new List<Recipes.BatteryRecipe>();
        private List<Batteries.BatteryInProduction> okBatteriesProducedList = new List<Batteries.BatteryInProduction>(), nokBatteriesProducedList = new List<Batteries.BatteryInProduction>();

        private TwinCAT.TwinCATVariableBuilder variableBuilder = null;

        private TwinCAT.TwinCATVariable<Boolean> simulatorState = null;
        private TwinCAT.TwinCATVariable<Int32> productOption = null;

        private TwinCAT.TwinCATVariable<Double> tachometer1MinOutOfRange = null, tachometer1MinValue = null, tachometer1RealValue = null, tachometer1MaxValue = null, tachometer1MaxOutOfRange = null;
        private TwinCAT.TwinCATVariable<Double> tachometer2MinOutOfRange = null, tachometer2MinValue = null, tachometer2RealValue = null, tachometer2MaxValue = null, tachometer2MaxOutOfRange = null;
        private TwinCAT.TwinCATVariable<Double> tachometer3MinOutOfRange = null, tachometer3MinValue = null, tachometer3RealValue = null, tachometer3MaxValue = null, tachometer3MaxOutOfRange = null;
        private TwinCAT.TwinCATVariable<Double> tachometer4MinOutOfRange = null, tachometer4MinValue = null, tachometer4RealValue = null, tachometer4MaxValue = null, tachometer4MaxOutOfRange = null;

        private TwinCAT.TwinCATVariable<Int32> amountOfProducts = null, amountOfProductsPassed = null, amountOfProductsFailed = null, amountOfProductsToCreate = null;

        private TwinCAT.TwinCATVariable<Int32> mousePositionX = null;

        private TwinCAT.TwinCATVariable<Byte[]> imageOfProduct = null;

        // Set start values for product in use
        private double startValueTotalLength, startValueTotalDiameter, startValueTerminalLength, startValueTerminalDiameter;

        private void SetupVariables()
        {
            simulatorState = variableBuilder.Build<Boolean>("Task.Outputs.Out1");
            productOption = variableBuilder.Build<Int32>("Task.Outputs.Out2");

            tachometer1MinOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom1MinOutOfRange");
            tachometer1MinValue = variableBuilder.Build<Double>("Task.Outputs.tachom1MinValue");
            tachometer1RealValue = variableBuilder.Build<Double>("Task.Outputs.tachom1RealValue");
            tachometer1MaxValue = variableBuilder.Build<Double>("Task.Outputs.tachom1MaxValue");
            tachometer1MaxOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom1MaxOutOfRange");

            tachometer2MinOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom2MinOutOfRange");
            tachometer2MinValue = variableBuilder.Build<Double>("Task.Outputs.tachom2MinValue");
            tachometer2RealValue = variableBuilder.Build<Double>("Task.Outputs.tachom2RealValue");
            tachometer2MaxValue = variableBuilder.Build<Double>("Task.Outputs.tachom2MaxValue");
            tachometer2MaxOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom2MaxOutOfRange");

            tachometer3MinOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom3MinOutOfRange");
            tachometer3MinValue = variableBuilder.Build<Double>("Task.Outputs.tachom3MinValue");
            tachometer3RealValue = variableBuilder.Build<Double>("Task.Outputs.tachom3RealValue");
            tachometer3MaxValue = variableBuilder.Build<Double>("Task.Outputs.tachom3MaxValue");
            tachometer3MaxOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom3MaxOutOfRange");

            tachometer4MinOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom4MinOutOfRange");
            tachometer4MinValue = variableBuilder.Build<Double>("Task.Outputs.tachom4MinValue");
            tachometer4RealValue = variableBuilder.Build<Double>("Task.Outputs.tachom4RealValue");
            tachometer4MaxValue = variableBuilder.Build<Double>("Task.Outputs.tachom4MaxValue");
            tachometer4MaxOutOfRange = variableBuilder.Build<Double>("Task.Outputs.tachom4MaxOutOfRange");

            amountOfProducts = variableBuilder.Build<Int32>("Task.Outputs.productCount");
            amountOfProductsPassed = variableBuilder.Build<Int32>("Task.Outputs.productPassed");
            amountOfProductsFailed = variableBuilder.Build<Int32>("Task.Outputs.productFailed");
            amountOfProductsToCreate = variableBuilder.Build<Int32>("Task.Outputs.productsToCreate");

            mousePositionX = variableBuilder.Build<Int32>("Task.Outputs.mousePosX");

            imageOfProduct = variableBuilder.Build<Byte[]>("Task.Outputs.picture");

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
            amountOfProducts.Value = 0;
            amountOfProductsPassed.Value = 0;
            amountOfProductsFailed.Value = 0; // Does not update sometimes?
        }

        bool simulationState = false;
        bool oldSimulationState = false;

        /// <summary>
        /// Set a new simstate to HMI output and local state and old local state.
        /// </summary>
        /// <param name="newSimState"></param>
        private void SetNewSimState(bool newSimState)
        {
            simulatorState.Value = newSimState;
            simulationState = simulatorState.Value;
            oldSimulationState = simulationState;
        }

        /// <summary>
        /// The main loop of the program.
        /// This is where items are produced.
        /// </summary>
        private void Run()
        {
            int productOption = 0; //TODO: Use an Enum for this. Current supported values are 0 -> 1.
            int oldProductOption = -1;
            bool firstStart = true;

            while (!Exit)
            {
                mousePositionX.Polling = true;
                //Console.WriteLine("Mouse pos X: " + mousePositionX.Value);

                amountOfProductsToCreate.Polling = true;

                this.productOption.Polling = true;
                productOption = this.productOption.Value;

                int randomSleepTime = 50; // ms
                int productSleepTime = 800; // ms

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

                if (amountOfProductsToCreate.Value > 0) // Cant create products less than 1.
                {
                    // run manufacturing process of chosen product
                    switch (productOption)
                    {
                        case 0: // Simulate AA Batteries
                            simulatorState.Polling = true;
                            simulationState = simulatorState.Value;

                            if (simulationState != oldSimulationState)
                            {
                                if (simulationState)
                                {
                                    Console.WriteLine("STARTED AA Batteries!");
                                    amountOfProductsToCreate.Polling = true;
                                    Console.WriteLine("Products to create: " + amountOfProductsToCreate.Value);

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

                                oldSimulationState = simulationState;
                            }

                            if (simulationState)
                            {
                                bool breakWhenPaused = false;

                                while (true)
                                {
                                    simulatorState.Polling = true;
                                    simulationState = simulatorState.Value;

                                    if (!simulationState)
                                    {
                                        breakWhenPaused = true;
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

                                    tachometer1RealValue.Value = batteryInProduction.TotalLength;
                                    tachometer2RealValue.Value = batteryInProduction.TotalDiameter;
                                    tachometer3RealValue.Value = batteryInProduction.TerminalLength;
                                    tachometer4RealValue.Value = batteryInProduction.TerminalDiameter;

                                    Thread.Sleep(productSleepTime);

                                    // Check if battery is OK or NOK
                                    if (Maths.Maths.CheckIfBatteryValuesAreOK(batteryInProduction))
                                    {
                                        okBatteriesProducedList.Add(batteryInProduction);
                                        amountOfProductsPassed.Value = okBatteriesProducedList.Count;
                                    }
                                    else
                                    {
                                        nokBatteriesProducedList.Add(batteryInProduction);
                                        amountOfProductsFailed.Value = nokBatteriesProducedList.Count;
                                    }

                                    amountOfProducts.Value = okBatteriesProducedList.Count + nokBatteriesProducedList.Count;

                                    // Print out values for each measure of the battery
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength), tempValueTotalLength);
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter), tempValueTotalDiameter);
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength), tempValueTerminalLength);
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter), tempValueTerminalDiameter);

                                    // New line for the next products printout
                                    Console.WriteLine();

                                    SetStartValues(tempValueTotalLength, tempValueTotalDiameter, tempValueTerminalLength, tempValueTerminalDiameter);

                                    if (okBatteriesProducedList.Count == amountOfProductsToCreate.Value)
                                    {
                                        tachometer1RealValue.Value = 0;
                                        tachometer2RealValue.Value = 0;
                                        tachometer3RealValue.Value = 0;
                                        tachometer4RealValue.Value = 0;
                                        break;
                                    }
                                }

                                PrintProductOKNOKData();
                                if (breakWhenPaused) break;

                                SetNewSimState(false);
                                firstStart = true;
                            }

                            break;
                        case 1: // Simulate AAA Batteries
                            simulatorState.Polling = true;
                            simulationState = simulatorState.Value;

                            if (simulationState != oldSimulationState)
                            {
                                if (simulationState)
                                {
                                    Console.WriteLine("STARTED AAA Batteries!");
                                    amountOfProductsToCreate.Polling = true;
                                    Console.WriteLine("Products to create: " + amountOfProductsToCreate.Value);

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

                                oldSimulationState = simulationState;
                            }

                            if (simulationState)
                            {
                                bool breakWhenPaused = false;

                                while (true)
                                {
                                    simulatorState.Polling = true;
                                    simulationState = simulatorState.Value;

                                    if (!simulationState)
                                    {
                                        breakWhenPaused = true;
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

                                    tachometer1RealValue.Value = batteryInProduction.TotalLength;
                                    tachometer2RealValue.Value = batteryInProduction.TotalDiameter;
                                    tachometer3RealValue.Value = batteryInProduction.TerminalLength;
                                    tachometer4RealValue.Value = batteryInProduction.TerminalDiameter;

                                    Thread.Sleep(productSleepTime);

                                    // Check if battery is OK or NOK
                                    if (Maths.Maths.CheckIfBatteryValuesAreOK(batteryInProduction))
                                    {
                                        okBatteriesProducedList.Add(batteryInProduction);
                                        amountOfProductsPassed.Value = okBatteriesProducedList.Count;
                                    }
                                    else
                                    {
                                        nokBatteriesProducedList.Add(batteryInProduction);
                                        amountOfProductsFailed.Value = nokBatteriesProducedList.Count;
                                    }

                                    amountOfProducts.Value = okBatteriesProducedList.Count + nokBatteriesProducedList.Count;

                                    // Print out values for each measure of the battery
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalLength, recepieInUse.MinValueTotalLength, recepieInUse.MaxValueTotalLength), tempValueTotalLength);
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTotalDiameter, recepieInUse.MinValueTotalDiameter, recepieInUse.MaxValueTotalDiameter), tempValueTotalDiameter);
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalLength, recepieInUse.MinValueTerminalLength, recepieInUse.MaxValueTerminalLength), tempValueTerminalLength);
                                    PrintOut(Maths.Maths.CheckIfValueIsWithinRange(tempValueTerminalDiameter, recepieInUse.MinValueTerminalDiameter, recepieInUse.MaxValueTerminalDiameter), tempValueTerminalDiameter);

                                    // New line for the next products printout
                                    Console.WriteLine();

                                    SetStartValues(tempValueTotalLength, tempValueTotalDiameter, tempValueTerminalLength, tempValueTerminalDiameter);

                                    if (okBatteriesProducedList.Count == amountOfProductsToCreate.Value)
                                    {
                                        tachometer1RealValue.Value = 0;
                                        tachometer2RealValue.Value = 0;
                                        tachometer3RealValue.Value = 0;
                                        tachometer4RealValue.Value = 0;
                                        break;
                                    }
                                }

                                PrintProductOKNOKData();
                                if (breakWhenPaused) break;

                                SetNewSimState(false);
                                firstStart = true;
                            }

                            break;
                        default:
                            Console.WriteLine("That is not an option. 2");

                            break;
                    }
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
            const int numberOfDecimals = 1; // Sets the number of decimals that will be returned.

            tachometer1MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer1MinValue.Value = Math.Round(recepieInUse.MinValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer1RealValue.Value = 0;
            tachometer1MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer1MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalLength, numberOfDecimals, MidpointRounding.AwayFromZero);

            tachometer2MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer2MinValue.Value = Math.Round(recepieInUse.MinValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer2RealValue.Value = 0;
            tachometer2MaxValue.Value = Math.Round(recepieInUse.MaxValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer2MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTotalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);

            tachometer3MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer3MinValue.Value = Math.Round(recepieInUse.MinValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer3RealValue.Value = 0;
            tachometer3MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer3MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalLength, numberOfDecimals, MidpointRounding.AwayFromZero);

            tachometer4MinOutOfRange.Value = Math.Round(recepieInUse.TachometerMinValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer4MinValue.Value = Math.Round(recepieInUse.MinValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer4RealValue.Value = 0;
            tachometer4MaxValue.Value = Math.Round(recepieInUse.MaxValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);
            tachometer4MaxOutOfRange.Value = Math.Round(recepieInUse.TachometerMaxValueTerminalDiameter, numberOfDecimals, MidpointRounding.AwayFromZero);

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
            const string fileNameBeginning = "DSC05", fileNameExtension = ".JPG";
            const int arraySize = 256;

            Random random = new Random();
            int randomIndex = random.Next(0, list.Count);
            string value = path + fileNameBeginning + list[randomIndex] + fileNameExtension;
            byte[] temp = Encoding.UTF8.GetBytes(value);
            byte[] toBeSended = new byte[arraySize];

            for (int i = 0; i < temp.Length; i++)
            {
                toBeSended[i] = temp[i];
            }

            imageOfProduct.Value = toBeSended;
        }

        /// <summary>
        /// Connects to ADS at localhost.
        /// </summary>
        private void ConnectToADS()
        {
            Console.WriteLine("Connecting using ADS...");
            variableBuilder = new TwinCAT.TwinCATVariableBuilder(); // Connect using ADS.
        }

        private void StopSimulation()
        {
            Exit = true;
        }
    }
}
