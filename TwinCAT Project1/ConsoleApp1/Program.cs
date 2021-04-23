using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace ConsoleApp1
{
    class Program
    {
        //private static AdsStream adsReadStream;
        //private static AdsStream adsWriteStream;

        

        public static int someValue1 = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("This is a test!");
            TcAdsClient ads = new TcAdsClient();
            ads.Connect("127.0.0.1.1.1", 851);

            //adsReadStream = new AdsStream(4);
            //adsWriteStream = new AdsStream(4);

            if (ads.IsConnected)
            {
                Console.WriteLine("I have connection!");
                Console.WriteLine("Hello from the other side!");

                // READ value from PLC adress
                /*AdsStream ds = new AdsStream(4);
                AdsBinaryReader br = new AdsBinaryReader(ds);

                // reads a DINT from PLC
                ads.Read(0x4020, 0, ds);
                ds.Position = 0;
                
                Console.WriteLine(br.ReadInt32().ToString());

                ds.Position = 0; // important, before doing anything with br!
                someValue1 = br.ReadInt32();
                Console.WriteLine("Value saved in someValue1.");

                // WRITE value to PLC adress

                // creates a stream with a length of 4 byte
                AdsStream ds2 = new AdsStream(4);
                AdsBinaryWriter bw = new AdsBinaryWriter(ds2);

                ds2.Position = 0;
                bw.Write((int)0); // restart from 0.

                // writes a DINT to PLC
                ads.Write(0x4020, 0, ds2);

                ds.Dispose();
                bw.Dispose();*/

                //ads.WriteSymbol("Task 4.Outputs.Out5", true, true);
                //var val = (bool)ads.ReadSymbol("Task 4.Outputs.Out5", typeof(Boolean), true);
                //var val2 = ads.ReadSymbolByName("Task 4.Outputs.Out5", typeof(Boolean), true);
                //var val = (int)ads.ReadSymbol("test", typeof(Int32), true);
                //Console.WriteLine(val2.ToString());                         

                //var valueToRead = (bool)ads.ReadAny(0x4044, 0x0, typeof(Boolean));

                TwinCAT.TwinCATVariableBuilder varBuilder = new TwinCAT.TwinCATVariableBuilder();
                TwinCAT.TwinCATVariable<Boolean> b = varBuilder.Build<Boolean>("Task.Outputs.Out5");
                Console.WriteLine(b);

                //Console.WriteLine(ads.ReadSymbol("Task 4.Outputs.Out5", typeof(Boolean), true).ToString());

                ads.Dispose();
            } else {
                Console.WriteLine("I cant connect!");
            }
        }
    }
}
