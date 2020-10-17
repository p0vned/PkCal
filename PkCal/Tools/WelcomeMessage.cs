using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PkCal.Tools
{
    public static class WelcomeMessage
    {
        public static void Show()
        {
            Console.WriteLine("PK CALENDAR CHECKER V1.0");
            Console.WriteLine("MADE BY WOJCIECH STRZEBOŃSKI");
            Thread.Sleep(1000);
            Console.Clear();
        }
    }
}
