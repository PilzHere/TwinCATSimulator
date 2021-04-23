using System;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace TwinCAT
{
    /// <summary>
    /// The representation of an object used for creating replicas of variables
    /// exported by a TwinCAT ADS server.
    /// </summary>
    public class TwinCATVariableBuilder
    {
        // This should be a readonly variable in the Build-method, however due
        // to C# stupidity it is only possible to have readonly fields. 
        /// <summary>
        /// Dictionary to translate from TwinCAT ADS datatypes to native ones.
        /// </summary>
        /// <remarks>
        /// Based on <see href="https://infosys.beckhoff.com/content/1033/tcsystemmanager/basics/TcSysMgr_DatatypeComparison.htm">Data Type Comparison</see>.
        /// ADST_STRING, ADST_WSTRING, ADST_REAL80 ADST_MAXTYPES and
        /// ADST_BIGTYPE are not translated.
        /// </remarks>
        private static readonly System.Collections.Generic.Dictionary<AdsDatatypeId, Type> typeConversion =
            new System.Collections.Generic.Dictionary<AdsDatatypeId, Type>
            {
                { AdsDatatypeId.ADST_VOID, typeof(void) },
                { AdsDatatypeId.ADST_INT16, typeof(short) },
                { AdsDatatypeId.ADST_INT32, typeof(int) },
                { AdsDatatypeId.ADST_REAL32, typeof(float) },
                { AdsDatatypeId.ADST_REAL64, typeof(double) },
                { AdsDatatypeId.ADST_INT8, typeof(sbyte) },
                { AdsDatatypeId.ADST_UINT8, typeof(byte) },
                { AdsDatatypeId.ADST_UINT16, typeof(ushort) },
                { AdsDatatypeId.ADST_UINT32, typeof(uint) },
                { AdsDatatypeId.ADST_INT64, typeof(long) },
                { AdsDatatypeId.ADST_UINT64, typeof(ulong) },
                { AdsDatatypeId.ADST_BIT, typeof(bool) }
            };

        /// <summary>TwinCAT ADS communication client.</summary>
        private TcAdsClient adsClient;
        /// <summary>
        /// Information about all declared variables exported by the ADS
        /// server.
        /// </summary>
        private TcAdsSymbolInfoLoader symbolInfoLoader;
        /// <summary>
        /// TwinCAT ADS server scans variables for updates this often.
        /// Expressed in milliseconds.
        /// </summary>
        private int cycleTime;
        /// <summary>
        /// The maximum period during which the TwinCAT ADS server can
        /// aggregate notification events originating from variable updates.
        /// Expressed in milliseconds.
        /// </summary>
        private int maxDelay;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TwinCATVariableBuilder"/> class. A connection is
        /// established to the TwinCAT ADS server on the local host using the
        /// supplied port number.
        /// </summary>
        /// <param name="serverPort">
        /// Port number of TwinCAT ADS server to connect to.
        /// </param>
        /// <param name="cycleTime">
        /// TwinCAT ADS server scans variables for updates this often.
        /// Expressed in milliseconds.
        /// </param>
        /// <param name="maxDelay">
        /// The maximum period during which the TwinCAT ADS server can
        /// aggregate notification events originating from variable updates.
        /// Expressed in milliseconds.
        /// </param>
        public TwinCATVariableBuilder(int serverPort = 301,
                                      int cycleTime = 10,
                                      int maxDelay = 0)
        {
            this.cycleTime = cycleTime;
            this.maxDelay = maxDelay;

            adsClient = new TcAdsClient();
            // Subscribe to events whenever (registered) variables are changed.
            adsClient.AdsNotificationEx += VariableUpdateEventHandler;

            adsClient.Connect(serverPort);

            symbolInfoLoader = adsClient.CreateSymbolInfoLoader();
        }

        /// <summary>
        /// Print information about symbols advertised by the TwinCAT ADS
        /// server.
        /// </summary>
        private void PrintSymbols()
        {
            foreach (TcAdsSymbolInfo symbol in symbolInfoLoader)
            {
                Console.WriteLine(symbol.Name + " " + symbol.DataType.FullName);
            }
        }

        /// <summary>
        /// Create an instance of <see cref="TwinCATVariable<T>"/> representing
        /// a replica of a variable on the TwinCAT ADS server identified by the
        /// supplied <see cref="variableName"/>.
        /// </summary>
        /// <typeparam name="T">Native type of the variable.</typeparam>
        /// <param name="variableName">Full name of the variable.</param>
        /// <returns>The representation of the replica.</returns>
        public TwinCATVariable<T> Build<T>(string variableName)
        {
            // Get symbol information to verify the type parameter T.
            TcAdsSymbolInfo symbolInformation = 
                symbolInfoLoader.FindSymbol(variableName);

            if (symbolInformation == null)
            {
                throw new TwinCATException(variableName + " was not found.");
            }

            if (symbolInformation.DataType.Category == DataTypeCategory.Array &&
                !typeof(T).IsArray)
            {
                throw new TwinCATException(variableName + " is an array while the value of the type parameter (" + typeof(T) + ") isn't.");
            }
            else if (symbolInformation.DataType.Category != DataTypeCategory.Array &&
                     typeof(T).IsArray)
            {
                throw new TwinCATException(variableName + " is not an array while the value of the type parameter (" + typeof(T) + ") is.");
            }

            Type elementType;
            if (symbolInformation.DataType.Category == DataTypeCategory.Array)
            {
                elementType = typeof(T).GetElementType();
            }
            else
            {
                elementType = typeof(T);
            }

            if (!typeConversion.ContainsKey(symbolInformation.DataType.DataTypeId))
            {
                throw new TwinCATException(variableName + " is of type " + symbolInformation.DataType.DataTypeId + " which can't be converted to a native type.");
            }

            if (typeConversion[symbolInformation.DataType.DataTypeId] != elementType &&
                !typeConversion[symbolInformation.DataType.DataTypeId].IsSubclassOf(typeof(T)))
            {
                throw new TwinCATException(variableName + " is of type " + symbolInformation.DataType.DataTypeId + " which can't be converted to " + elementType + ".");
            }

            if (typeof(T).IsArray)
            {
                return new TwinCATArrayVariable<T>(adsClient,
                                                   variableName,
                                                   symbolInformation.Size,
                                                   cycleTime,
                                                   maxDelay);
            }
            else
            {
                return new TwinCATScalarVariable<T>(adsClient,
                                                    variableName,
                                                    cycleTime,
                                                    maxDelay);
            }
        }

        /// <summary>
        /// Event handler for events from the TwinCAT ADS client.
        /// </summary>
        /// <param name="sender">Origin of event.</param>
        /// <param name="e">Event arguments.</param>
        private void VariableUpdateEventHandler(object sender, AdsNotificationExEventArgs e)
        {
            ((SetValueDelegate)e.UserData)(e.Value);
        }
    }
}
