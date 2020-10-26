using Ical.Net;
using Ical.Net.CalendarComponents;
using System.Collections.Generic;
using System.Linq;

namespace PkCal.Tools
{
    class CalendarNewEventsFinder
    {
        private Calendar _baseCalendar;
        private Calendar _checkedCalendar;

        public IEnumerable<CalendarEvent> NewEvents { get; private set; }

        public CalendarNewEventsFinder(Calendar baseCalendar, Calendar checkedCalendar)
        {
            _baseCalendar = baseCalendar;
            _checkedCalendar = checkedCalendar;
        }

        public bool CheckNewEvents()
        {
            var baseEvents = _baseCalendar.Events.ToHashSet();
            var checkedEvent = _checkedCalendar.Events.ToHashSet();

            NewEvents = baseEvents.Except(checkedEvent);

            return NewEvents.Count() != 0;
        }
    }
}
