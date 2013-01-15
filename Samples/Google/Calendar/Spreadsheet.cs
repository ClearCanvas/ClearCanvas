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
using Google.GData.Spreadsheets;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using Google.GData.Client;

namespace ClearCanvas.Samples.Calendar
{
    [MenuAction("show", "global-menus/Google/Spreadsheet")]
    [ClickHandler("show", "Show")]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class SpreadsheetTool : Tool<IDesktopToolContext>
    {
        public void Show()
        {
            Spreadsheet sheet = Spreadsheet.GetByTitle("DevConf");
            Worksheet worksheet = sheet.FirstWorksheet;
            List<WorksheetRow> rows = worksheet.ListRows(new Dictionary<string, string>());
            foreach (WorksheetRow row in rows)
            {
                Console.WriteLine(row["Name"] + row["Age"]);
                row["Age"] += "1";
                row.Save();
            }
        }
    }

    public class Spreadsheet
    {
        const string URL = "http://spreadsheets.google.com/feeds/spreadsheets/private/full";

        private SpreadsheetEntry _entry;
        private SpreadsheetsService _service;


        public static List<Spreadsheet> ListAll()
        {
            SpreadsheetsService service = new SpreadsheetsService("ClearCanvas-Workstation-1.0");
            service.setUserCredentials("jresnick", "bl00b0lt");
            
            SpreadsheetQuery query = new SpreadsheetQuery();
            SpreadsheetFeed feed = service.Query(query);

            return CollectionUtils.Map<SpreadsheetEntry, Spreadsheet, List<Spreadsheet>>(
                feed.Entries,
                delegate(SpreadsheetEntry entry) { return new Spreadsheet(service, entry); });
        }

        public static Spreadsheet GetByTitle(string title)
        {
            SpreadsheetsService service = new SpreadsheetsService("ClearCanvas-Workstation-1.0");
            service.setUserCredentials("jresnick", "bl00b0lt");

            SpreadsheetQuery query = new SpreadsheetQuery();
            query.Title = title;

            SpreadsheetFeed feed = service.Query(query);
            return feed.Entries.Count > 0 ? new Spreadsheet(service, (SpreadsheetEntry)feed.Entries[0]) : null;
        }


        internal Spreadsheet(SpreadsheetsService service, SpreadsheetEntry entry)
        {
            _entry = entry;
            _service = service;
        }

        public string Title
        {
            get { return _entry.Title.Text; }
        }

        public Worksheet FirstWorksheet
        {
            get
            {
                List<Worksheet> worksheets = ListWorksheets();
                return worksheets.Count > 0 ? worksheets[0] : null;
            }
        }

        public List<Worksheet> ListWorksheets()
        {
            AtomLink link = _entry.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);

            WorksheetQuery query = new WorksheetQuery(link.HRef.ToString());
            WorksheetFeed feed = _service.Query(query);

            return CollectionUtils.Map<WorksheetEntry, Worksheet, List<Worksheet>>(
                feed.Entries,
                delegate(WorksheetEntry entry) { return new Worksheet(_service, entry); });
        }
    }

    public class Worksheet
    {
        private WorksheetEntry _entry;
        private SpreadsheetsService _service;

        internal Worksheet(SpreadsheetsService service, WorksheetEntry entry)
        {
            _entry = entry;
            _service = service;
        }

        public string Title
        {
            get { return _entry.Title.Text; }
        }

        public List<WorksheetRow> ListRows(Dictionary<string, string> parameters)
        {
            AtomLink listFeedLink = _entry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            ListQuery query = new ListQuery(listFeedLink.HRef.ToString());

            string sq = CollectionUtils.Reduce<string, string>(parameters.Keys, "",
                delegate(string key, string memo)
                {
                    return (string.IsNullOrEmpty(memo) ? "" : " and ")
                        + string.Format("{0}={1}", key, parameters[key]);
                });

            query.SpreadsheetQuery = sq;

            ListFeed feed = _service.Query(query);

            return CollectionUtils.Map<ListEntry, WorksheetRow, List<WorksheetRow>>(
                feed.Entries,
                delegate(ListEntry entry) { return new WorksheetRow(feed, entry); });
        }
    }

    public class WorksheetRow
    {
        private bool _unsaved;
        private ListEntry _entry;
        private ListFeed _feed;

        internal WorksheetRow(ListFeed feed, ListEntry entry)
        {
            _entry = entry;
            _feed = feed;
        }

        internal WorksheetRow(ListFeed feed)
        {
            // TODO need to pre-init listEntry columns
            _feed = feed;
            _entry = new ListEntry();
            _unsaved = true;
        }

        public void Save()
        {
            if (_unsaved)
            {
                _entry = _feed.Insert(_entry) as ListEntry;
                _unsaved = false;
            }
            else
            {
                _entry = _entry.Update() as ListEntry;
            }
        }

        public string this[string key]
        {
            get
            {
                ListEntry.Custom entry = CollectionUtils.SelectFirst<ListEntry.Custom>(_entry.Elements,
                    delegate(ListEntry.Custom e) { return e.LocalName.Equals(key, StringComparison.CurrentCultureIgnoreCase); });
                return entry == null ? null : entry.Value;
            }
            set
            {
                ListEntry.Custom entry = CollectionUtils.SelectFirst<ListEntry.Custom>(_entry.Elements,
                    delegate(ListEntry.Custom e) { return e.LocalName.Equals(key, StringComparison.CurrentCultureIgnoreCase); });
                if (entry == null)
                {
                    entry = new ListEntry.Custom();
                    _entry.Elements.Add(entry);
                }
                entry.Value = value;
            }
        }
    }

}
