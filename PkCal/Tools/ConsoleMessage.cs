using System;
using System.Threading;

namespace PkCal.Tools
{
    public static class ConsoleMessage
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
            PrintMessageWithChangedColour(message, ConsoleColor.Green);
        }

        public static void PrintWarningMessage(string message)
        {
            PrintMessageWithChangedColour(message, ConsoleColor.Yellow);
        }

        public static void PrintErrorMessage(string message)
        {
            PrintMessageWithChangedColour(message, ConsoleColor.Red);
        }

        private static void PrintMessageWithChangedColour(string message, ConsoleColor color)
        {
            var currentForegroundColor = Console.ForegroundColor;
            
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = currentForegroundColor;
        }
    }
}
