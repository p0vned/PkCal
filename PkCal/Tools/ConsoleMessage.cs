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
            var color = ConsoleColor.Green;

            PrintMessageWithChangedColour(message, color);
        }

        public static void PrintWarningMessage(string message)
        {
            var color = ConsoleColor.Yellow;

            PrintMessageWithChangedColour(message, color);
        }

        public static void PrintErrorMessage(string message)
        {
            var color = ConsoleColor.Red;

            PrintMessageWithChangedColour(message, color);
        }

        private static void PrintMessageWithChangedColour(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
