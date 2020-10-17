using PkCal.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ical.Net;

namespace PkCal
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage.Show();

            var directoryWithConfigName = "config";
            var uriCalendarFileName = "calendar.ws";
            var oldCalendarDataFileName = "oldCalendarData.ws";
            var currentCalendarDataFileName = "currentCalendarData.ws";

            var currentDirectory = Directory.GetCurrentDirectory();
            var sb = new StringBuilder();

            sb.Append(currentDirectory);
            sb.Append("\\");
            sb.Append(directoryWithConfigName);

            var configDirectoryPath = sb.ToString();

            sb.Append("\\");

            var uriCalendarFilePath = sb.ToString() + uriCalendarFileName;
            var oldCalendarDataFilePath = sb.ToString() + oldCalendarDataFileName;
            var currentCalendarDataFilePath = sb.ToString() + currentCalendarDataFileName;


            Console.WriteLine("Sprawdzam, czy folder z konfiguracją istnieje, jeśli nie zostanie on utworzony...");
            if (!Directory.Exists(configDirectoryPath))
            {
                Directory.CreateDirectory(configDirectoryPath);
                Console.WriteLine("Utworzono folder w lokalizacji: " + configDirectoryPath);
            }

            Console.WriteLine();

            sb.Clear();
            sb.Append(configDirectoryPath);
            sb.Append("\\"); 
            sb.Append(uriCalendarFileName);

            bool isFileWithUriCalendarExists = File.Exists(uriCalendarFilePath);

            Console.WriteLine(string.Format("Sprawdzam, czy plik z linkiem do kalendarza istnieje... {0}", isFileWithUriCalendarExists ? "TAK" : "NIE"));

            if (isFileWithUriCalendarExists)
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
                            var fs = new FileSaver(uriCalendarFilePath, url);
                            var result = fs.SaveFile();

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
                var fs = new FileSaver(uriCalendarFilePath, url);
                var result = fs.SaveFile();

                if (!result.Success)
                {
                    Console.WriteLine("[BŁĄD] " + result);
                    Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                    Environment.Exit(-1);
                }

                Console.WriteLine("[SUKCES] Poprawnie utworzono plik z linkiem do kalendarza!");
            }

            Console.WriteLine();

            bool isFileWithOldCalendarDataExists = File.Exists(oldCalendarDataFilePath);

            Console.WriteLine(string.Format("Sprawdzam, czy plik z starymi danymi kalendarza istnieje... {0}", isFileWithOldCalendarDataExists ? "TAK" : "NIE"));

            if (!isFileWithOldCalendarDataExists)
            {
                var fs = new FileSaver(oldCalendarDataFilePath, "initial");
                var result = fs.SaveFile();

                if (!result.Success)
                {
                    Console.WriteLine("[BŁĄD] " + result);
                    Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                    Environment.Exit(-1);
                }
            }

            bool isFileWithCurrentCalendarDataExists = File.Exists(currentCalendarDataFilePath);

            Console.WriteLine(string.Format("Sprawdzam, czy plik z nowymi danymi kalendarza istnieje... {0}", isFileWithCurrentCalendarDataExists ? "TAK" : "NIE"));

            if (!isFileWithCurrentCalendarDataExists)
            {
                var fs = new FileSaver(currentCalendarDataFilePath, "initial");
                var result = fs.SaveFile();

                if (!result.Success)
                {
                    Console.WriteLine("[BŁĄD] " + result);
                    Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                    Environment.Exit(-1);
                }
            }

            var calendarUrl = File.ReadAllLines(uriCalendarFilePath);

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

            if (!dataCalendarResult)
            {
                Console.WriteLine("[BŁĄD] Błąd pobierania danych z url kalendarza!");
                Console.WriteLine("Naciśnij przycisk aby zamknąć program");
                Environment.Exit(-1);
            }

            var oldDataCalendarFromFile = File.ReadAllText(oldCalendarDataFilePath);

            if (oldDataCalendarFromFile.Contains("initial"))
            {
                var fs = new FileSaver(oldCalendarDataFilePath, fw.Content);
                fs.SaveFile();
                oldDataCalendarFromFile = File.ReadAllText(oldCalendarDataFilePath);
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

            var calendarSaver = new FileSaver(oldCalendarDataFilePath, fw.Content);
            calendarSaver.SaveFile();

            Console.ReadKey();
        }
    }
}
