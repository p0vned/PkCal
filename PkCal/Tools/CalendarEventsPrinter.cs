using Ical.Net.CalendarComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PkCal.Tools
{
    class CalendarEventsPrinter
    {
        private IEnumerable<CalendarEvent> _events;

        public CalendarEventsPrinter(IEnumerable<CalendarEvent> events)
        {
            _events = events;
        }

        public bool PrintAllEvents()
        {
            bool atLeastPrintedOnce = false;

            foreach (var calendarEvent in _events)
            {
                atLeastPrintedOnce = true;

                Console.WriteLine("======");
                Console.WriteLine(string.Format("[KURS] {0}", calendarEvent.Categories.SingleOrDefault()));
                Console.WriteLine(string.Format("[NAZWA WYDARZENIA] {0}", calendarEvent.Summary));
                Console.WriteLine(string.Format("[OPIS] {0}", calendarEvent.Description));
                Console.WriteLine(string.Format("[TERMIN DO] {0}", calendarEvent.DtEnd));
            }

            return atLeastPrintedOnce;
        }
    }
}
