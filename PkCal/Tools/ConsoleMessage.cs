using System;
using System.Threading;

namespace PkCal.Tools
{
    static class ConsoleMessage
    {
        public static void ShowWelcomeMessage()
        {
            Console.WriteLine("PK CALENDAR CHECKER V1.0");
            Console.WriteLine("MADE BY WOJCIECH STRZEBOŃSKI");
            Thread.Sleep(1000);
            Console.Clear();
        }

        public static void PrintSuccessMessage(string message)
        {
            PrintMessageWithChangedColor(message, ConsoleColor.Green);
        }

        public static void PrintWarningMessage(string message)
        {
            PrintMessageWithChangedColor(message, ConsoleColor.Yellow);
        }

        public static void PrintErrorMessage(string message)
        {
            PrintMessageWithChangedColor(message, ConsoleColor.Red);
        }

        private static void PrintMessageWithChangedColor(string message, ConsoleColor color)
        {
            var currentForegroundColor = Console.ForegroundColor;
            
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = currentForegroundColor;
        }
    }
}
