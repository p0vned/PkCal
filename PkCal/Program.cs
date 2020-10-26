using PkCal.Tools;
using System;
using System.IO;
using System.Linq;
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
                ConsoleMessage.PrintSuccessMessage("[SUKCES] ");
                Console.WriteLine(string.Format("Utworzono folder w lokalizacji: {0}", configDirPathBuilder.Path));
            }

            calendarEndpointDataFile.CheckIfExists();

            Console.WriteLine("Sprawdzam, czy plik z URL do kalendarza istnieje, jeśli nie zostanie on utworzony...");
            if (!calendarEndpointDataFile.Exists)
            {
                var result = calendarEndpointDataFile.SaveContentToFile();
                
                if (!result.Success)
                    EndProgramError(result.ToString());

                ConsoleMessage.PrintSuccessMessage("[SUKCES] ");
                Console.WriteLine(string.Format("Utworzono plik z URL do kalendarza w lokalizacji: {0}", calendarEndpointDataFile.Path));
            }

            calendarDataFile.CheckIfExists();

            Console.WriteLine("Sprawdzam, czy plik z danymi kalendarza istnieje jeśli nie zostanie on utworzony...");
            if (!calendarDataFile.Exists)
            {
                calendarDataFile.SetContent("initial");
                var result = calendarDataFile.SaveContentToFile();

                if (!result.Success)
                    EndProgramError(result.ToString());

                ConsoleMessage.PrintSuccessMessage("[SUKCES] ");
                Console.WriteLine(string.Format("Utworzono plik z danymi kalendarza w lokalizacji: {0}", calendarDataFile.Path));
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

                        ConsoleMessage.PrintSuccessMessage("[SUKCES] ");
                        Console.WriteLine("\nPoprawnie utworzono plik z linkiem do kalendarza!");
                        break;
                    case "n":
                        isInputCorrect = true;
                        calendarEndpointDataFile.ReadContentFromFile();
                        break;
                    default:
                        Console.WriteLine("Prosze wprowadzić poprawny znak odpowiedzi!");
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

            foreach (var calendarEvent in calendarFile.Events)
            {
                Console.WriteLine("======");
                Console.WriteLine(string.Format("[KURS] {0}", calendarEvent.Categories.SingleOrDefault()));
                Console.WriteLine(string.Format("[NAZWA WYDARZENIA] {0}", calendarEvent.Summary));
                Console.WriteLine(string.Format("[OPIS] {0}", calendarEvent.Description));
                Console.WriteLine(string.Format("[TERMIN DO] {0}", calendarEvent.DtEnd));
            }

            Console.WriteLine("==================================");

            var eventsInCalendarFile = calendarFile.Events.ToHashSet();
            var eventsInCalendarFromWeb = calendarWeb.Events.ToHashSet();

            var newEvents = eventsInCalendarFromWeb.Except(eventsInCalendarFile);

            if (newEvents.Count() != 0)
            {
                ConsoleMessage.PrintWarningMessage("[UWAGA] ");
                Console.WriteLine("ZNALEZIONO NOWE ZMIANY W KALENDARZU!");

                foreach (var newEvent in newEvents)
                {
                    Console.WriteLine("======");
                    Console.WriteLine(string.Format("[KURS] {0}", newEvent.Categories.SingleOrDefault()));
                    Console.WriteLine(string.Format("[NAZWA WYDARZENIA] {0}", newEvent.Summary));
                    Console.WriteLine(string.Format("[OPIS] {0}", newEvent.Description));
                    Console.WriteLine(string.Format("[TERMIN DO] {0}", newEvent.DtEnd));

                    calendarDataFile.SetContent(calendarDataObtainer.Content);
                }
            }
            else
                Console.WriteLine("BRAK NOWYCH ZMIAN W KALENDARZU!");

            calendarDataFile.SaveContentToFile();

            Console.ReadKey();
        }

        private static void EndProgramError(string message)
        {
            const string tag = "[BŁĄD] ";
            
            ConsoleMessage.PrintErrorMessage(tag);
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine("Naciśnij przycisk aby zamknąć program");

            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
