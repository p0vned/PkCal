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
            PrintColoredString("[SUKCES] ", ConsoleColor.Green);
            Console.WriteLine(message);
        }

        public static void PrintWarningMessage(string message)
        {
            PrintColoredString("[UWAGA] ", ConsoleColor.Yellow);
            Console.WriteLine(message);
        }

        public static void PrintErrorMessage(string message)
        {
            PrintColoredString("[UWAGA] ", ConsoleColor.Red);
            Console.WriteLine(message);
        }

        private static void PrintColoredString(string message, ConsoleColor color)
        {
            var currentForegroundColor = Console.ForegroundColor;
            
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = currentForegroundColor;
        }
    }
}
