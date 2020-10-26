using PkCal.Tools;
using System;
using System.IO;
using Ical.Net;
using System.Configuration;
using PkCal.Models;

namespace PkCal
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleMessage.ShowWelcomeMessage();

            var configDirectoryName = ConfigurationManager.AppSettings["ConfigDirectoryName"];
            var calendarEndpointFileName = ConfigurationManager.AppSettings["EndpointCalendarFileName"];
            var calendarDataFileName = ConfigurationManager.AppSettings["CalendarDataFileName"];

            var currentDirectory = Directory.GetCurrentDirectory();

            var configDirPathBuilder = new PathCreator(currentDirectory);
            configDirPathBuilder.AddDirectory(configDirectoryName);

            var calendarEndpointFilePathBuilder = new PathCreator(configDirPathBuilder.Path);
            calendarEndpointFilePathBuilder.AddFile(calendarEndpointFileName);

            var calendarDataFilePathBuilder = new PathCreator(configDirPathBuilder.Path);
            calendarDataFilePathBuilder.AddFile(calendarDataFileName);

            var calendarEndpointDataFile = new DataFile(calendarEndpointFilePathBuilder.Path);
            var calendarDataFile = new DataFile(calendarDataFilePathBuilder.Path);

            Console.WriteLine("Sprawdzam, czy folder z konfiguracją istnieje, jeśli nie zostanie on utworzony...\n");
            if (!Directory.Exists(configDirPathBuilder.Path))
            {
                Directory.CreateDirectory(configDirPathBuilder.Path);
                ConsoleMessage.PrintSuccessMessage(string.Format("Utworzono folder w lokalizacji: {0}", configDirPathBuilder.Path));
            }

            Console.WriteLine("Sprawdzam, czy plik z URL do kalendarza istnieje, jeśli nie zostanie on utworzony...");
            if (!calendarEndpointDataFile.Exists)
            {
                var result = calendarEndpointDataFile.SaveContentToFile();
                
                if (!result.Success)
                    EndProgramError(result.ToString());

                ConsoleMessage.PrintSuccessMessage(string.Format("Utworzono plik z URL do kalendarza w lokalizacji: {0}", calendarEndpointDataFile.Path));
            }

            Console.WriteLine("Sprawdzam, czy plik z danymi kalendarza istnieje jeśli nie zostanie on utworzony...");
            if (!calendarDataFile.Exists)
            {
                calendarDataFile.SetContent("initial");
                var result = calendarDataFile.SaveContentToFile();

                if (!result.Success)
                    EndProgramError(result.ToString());

                ConsoleMessage.PrintSuccessMessage(string.Format("Utworzono plik z danymi kalendarza w lokalizacji: {0}", calendarDataFile.Path));
            }

            Console.WriteLine("\nCzy chcesz zmienić adres URL? Jeśli to pierwsze uruchomienie programu wybierz T [T/N]");

            bool isInputCorrect = false;

            while (!isInputCorrect)
            {
                var input = Console.ReadLine();

                input = input.ToLower();

                switch (input)
                {
                    case "t":
                        isInputCorrect = true;

                        Console.WriteLine("Wprowadź adres URL do kalendarza:");

                        var url = Console.ReadLine();
                        calendarEndpointDataFile.SetContent(url);
                        var result = calendarEndpointDataFile.SaveContentToFile();

                        if (!result.Success)
                            EndProgramError(result.ToString());

                        ConsoleMessage.PrintSuccessMessage("\nPoprawnie utworzono plik z linkiem do kalendarza!");
                        break;
                    case "n":
                        isInputCorrect = true;
                        calendarEndpointDataFile.ReadContentFromFile();
                        break;
                    default:
                        ConsoleMessage.PrintErrorMessage("Prosze wprowadzić poprawny znak odpowiedzi!");
                        break;
                }
            }

            if (calendarEndpointDataFile.Content == null)
                EndProgramError("Plik zawierający url do kalendarza jest pusty!");

            if (!calendarEndpointDataFile.Content.Contains("http"))
                EndProgramError("Adres do kalendarza jest nieprawidłowy!");

            var calendarDataObtainer = new FileFromWebObtainer(new Uri(calendarEndpointDataFile.Content));

            Console.WriteLine("Pobieranie danych z kalendarza...");
            var dataCalendarResult = calendarDataObtainer.GetCalendarData();

            if (!dataCalendarResult.Success)
                EndProgramError(dataCalendarResult.ToString());

            calendarDataFile.ReadContentFromFile();

            if (calendarDataFile.Content.Contains("initial"))
            {
                calendarDataFile.SetContent(calendarDataObtainer.Content);
                calendarDataFile.SaveContentToFile();
            }

            var calendarFile = Calendar.Load(calendarDataFile.Content);
            var calendarWeb = Calendar.Load(calendarDataObtainer.Content);

            Console.Clear();

            var currentCalendarEventsPrinter = new CalendarEventsPrinter(calendarFile.Events);
            currentCalendarEventsPrinter.PrintAllEvents();

            Console.WriteLine("==================================");

            var newEventsFinder = new CalendarNewEventsFinder(calendarFile, calendarWeb);
            var newEventsResult = newEventsFinder.CheckNewEvents();

            if (!newEventsResult)
                ConsoleMessage.PrintSuccessMessage("NIE ZNALEZIONO ZMIAN W KALENDARZU!");
            else
            {
                ConsoleMessage.PrintWarningMessage("ZNALEZIONO NOWE ZMIANY W KALENDARZU!");

                var newEventsPrinter = new CalendarEventsPrinter(newEventsFinder.NewEvents);
                newEventsPrinter.PrintAllEvents();
                calendarDataFile.SetContent(calendarDataObtainer.Content);
            }

            calendarDataFile.SaveContentToFile();
            
            Console.WriteLine("\nNaciśnij przycisk aby zamknąć program...");
            Console.ReadKey();
        }

        private static void EndProgramError(string message)
        {
            ConsoleMessage.PrintErrorMessage(message);
            Console.WriteLine();
            Console.WriteLine("Naciśnij przycisk aby zamknąć program...");

            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
