using PkCal.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ical.Net;
using System.Configuration;
using PkCal.Models;

namespace PkCal
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage.Show();

            var configDirectoryName = ConfigurationManager.AppSettings["ConfigDirectoryName"];
            var calendarEndpointFileName = ConfigurationManager.AppSettings["EndpointCalendarFileName"];
            var calendarDataFileName = ConfigurationManager.AppSettings["CalendarDataFileName"];

            var currentDirectory = Directory.GetCurrentDirectory();

            var configDirPathBuilder = new PathCreator(currentDirectory);
            configDirPathBuilder.AddDirectory(configDirectoryName);

            var calendarEndpointFilePathBuilder = new PathCreator(configDirPathBuilder.Path);
            calendarEndpointFilePathBuilder.AddFile(calendarDataFileName);

            var calendarDataFilePathBuilder = new PathCreator(configDirPathBuilder.Path);
            calendarDataFilePathBuilder.AddFile(calendarDataFileName);

            var calendarEndpointDataFile = new DataFile(calendarEndpointFilePathBuilder.Path);
            var calendarDataFile = new DataFile(calendarDataFilePathBuilder.Path);

            Console.WriteLine("Sprawdzam, czy folder z konfiguracją istnieje, jeśli nie zostanie on utworzony...");
            if (!Directory.Exists(configDirPathBuilder.Path))
            {
                Directory.CreateDirectory(configDirPathBuilder.Path);
                Console.WriteLine("Utworzono folder w lokalizacji: " + configDirPathBuilder.Path);
            }

            Console.WriteLine();

            calendarEndpointDataFile.CheckIfExists();

            Console.WriteLine(string.Format("Sprawdzam, czy plik z linkiem do kalendarza istnieje... {0}", calendarEndpointDataFile.Exists ? "TAK" : "NIE"));

            if (calendarEndpointDataFile.Exists)
            {
                bool isInputCorrect = false;

                while (!isInputCorrect)
                {
                    Console.WriteLine("Czy chcesz zmienić adres URL? [T/N] ");
                    var input = Console.ReadLine();

                    input = input.ToLower();

                    switch (input)
                    {
                        case "t":
                            isInputCorrect = true;
                            
                            Console.WriteLine("Wprowadź adres url:");

                            var url = Console.ReadLine();
                            calendarEndpointDataFile.SetContent(url);
                            var result = calendarEndpointDataFile.SaveContentToFile();

                            if (!result.Success)
                            {
                                Console.WriteLine("[BŁĄD] " + result);
                                Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                                Environment.Exit(-1);
                            }

                            Console.WriteLine("[SUKCES] Poprawnie utworzono plik z linkiem do kalendarza!");
                            break;
                        case "n":
                            isInputCorrect = true;
                            break;
                        default:
                            Console.WriteLine("Prosze wprowadzić poprawny znak odpowiedzi!");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Wprowadź adres url:");

                var url = Console.ReadLine();
                calendarEndpointDataFile.SetContent(url);
                var result = calendarEndpointDataFile.SaveContentToFile();

                if (!result.Success)
                {
                    Console.WriteLine("[BŁĄD] " + result);
                    Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                    Environment.Exit(-1);
                }

                Console.WriteLine("[SUKCES] Poprawnie utworzono plik z linkiem do kalendarza!");
            }

            Console.WriteLine();

            calendarDataFile.CheckIfExists();

            Console.WriteLine(string.Format("Sprawdzam, czy plik z starymi danymi kalendarza istnieje... {0}", calendarDataFile.Exists ? "TAK" : "NIE"));

            if (!calendarDataFile.Exists)
            {
                calendarDataFile.SetContent("initial");
                var result = calendarDataFile.SaveContentToFile();

                if (!result.Success)
                {
                    Console.WriteLine("[BŁĄD] " + result);
                    Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                    Environment.Exit(-1);
                }
            }

            var calendarUrl = File.ReadAllLines(calendarEndpointFilePathBuilder.Path);

            if (calendarUrl == null)
            {
                Console.WriteLine("[BŁĄD] Plik zawierający url do kalendarza jest pusty!");
                Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                Environment.Exit(-1);
            }

            if (!calendarUrl[0].Contains("http"))
            {
                Console.WriteLine("[BŁĄD] Adres do kalendarza jest nieprawidłowy!");
                Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                Environment.Exit(-1);
            }

            var fw = new FileFromWebObtainer(new Uri(calendarUrl[0]));

            Console.WriteLine("Pobieranie danych z kalendarza...");
            var dataCalendarResult = fw.GetCalendarData();

            if (!dataCalendarResult.Success)
            {
                Console.WriteLine(string.Format("[BŁĄD] {0}", dataCalendarResult));
                Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                Environment.Exit(-1);
            }

            var oldDataCalendarFromFile = File.ReadAllText(calendarEndpointFilePathBuilder.Path);

            if (oldDataCalendarFromFile.Contains("initial"))
            {
                var fs = new FileSaver(calendarEndpointFilePathBuilder.Path, fw.Content);
                fs.SaveFile();
                oldDataCalendarFromFile = File.ReadAllText(calendarEndpointFilePathBuilder.Path);
            }

            var oldCalendar = Calendar.Load(oldDataCalendarFromFile);
            var currentCalendar = Calendar.Load(fw.Content);

            Console.Clear();

            foreach (var calendarEvent in oldCalendar.Events)
            {
                Console.WriteLine("======");
                Console.WriteLine(string.Format("[KURS] {0}", calendarEvent.Categories.SingleOrDefault()));
                Console.WriteLine(string.Format("[NAZWA WYDARZENIA] {0}", calendarEvent.Summary));
                Console.WriteLine(string.Format("[OPIS] {0}", calendarEvent.Description));
                Console.WriteLine(string.Format("[TERMIN DO] {0}", calendarEvent.DtEnd));
            }

            Console.WriteLine("==================================");

            var oldCalendarEvetsUid = oldCalendar.Events.Select(s => s.Uid);
            var currentCallendarEventsUid = currentCalendar.Events.Select(s => s.Uid);

            var newEvents = currentCallendarEventsUid.Where(w => !oldCalendarEvetsUid.Any(c => c.Equals(w))).ToList();

            if (newEvents.Count != 0)
            {  
                Console.WriteLine("ZNALEZIONO NOWE ZMIANY W KALENDARZU!");
                Console.WriteLine("TODO");
            }
            else
            {
                Console.WriteLine("BRAK NOWYCH ZMIAN W KALENDARZU!");
            }

            var calendarSaver = new FileSaver(calendarEndpointFilePathBuilder.Path, fw.Content);
            calendarSaver.SaveFile();

            Console.ReadKey();
        }
    }
}
