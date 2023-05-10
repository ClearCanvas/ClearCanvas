#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Samples.Google.Calendar
{
    /// <summary>
    /// Provides access to a Google calendar.
    /// </summary>
    public class Calendar
    {

        const string CALENDAR_URI = "http://www.google.com/calendar/feeds/default/private/full";

        private CalendarService _service;

        public Calendar()
        {
            _service = new CalendarService("ClearCanvas-Workstation-1.0");
            _service.setUserCredentials("clearcanvas.demo", "clearcanvas1");
        }

        /// <summary>
        /// Queries the calendar for events matching the specified criteria.
        /// </summary>
        /// <param name="fullTextQuery"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public CalendarEvent[] GetEvents(string fullTextQuery, DateTime? from, DateTime? until)
        {
            EventQuery query = new EventQuery();

            query.Uri = new Uri(CALENDAR_URI);

            query.Query = fullTextQuery;

            if (from != null)
            {
                query.StartTime = from.Value;
            }

            if (until != null)
            {
                query.EndTime = until.Value;
            }

            EventFeed calFeed = _service.Query(query);

            List<CalendarEvent> events = CollectionUtils.Map<EventEntry, CalendarEvent, List<CalendarEvent>>(calFeed.Entries,
                delegate(EventEntry e) { return new CalendarEvent(e); });
            events.Sort();
            return events.ToArray();
        }

        /// <summary>
        /// Adds a new event to the calendar using the specified information.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public CalendarEvent AddEvent(string title, string description, DateTime? start, DateTime? end)
        {
            EventEntry entry = new EventEntry();

            // Set the title and content of the entry.
            entry.Title.Text = title;
            entry.Content.Content = description;

            if (start != null || end != null)
            {
                When eventTime = new When();
                if(start != null)
                    eventTime.StartTime = (DateTime)start;

                if(end != null)
                    eventTime.EndTime = (DateTime)end;

                entry.Times.Add(eventTime);
            }

            // Send the request and receive the response:
            entry = _service.Insert(new Uri(CALENDAR_URI), entry) as EventEntry;

            return new CalendarEvent(entry);
        }
    }
}
