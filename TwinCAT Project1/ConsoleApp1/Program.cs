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
        static void Main(string[] args)
        {
            Console.WriteLine("This is a test!");
            TcAdsClient ads = new TcAdsClient();
            ads.Connect(541);

            if (ads.IsConnected)
            {
                Console.WriteLine("I have connection!");
            } else
            {
                Console.WriteLine("I cant connect!");
            }
        }
    }
}
