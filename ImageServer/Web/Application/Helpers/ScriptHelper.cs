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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
    public class ScriptHelper
    {
        public static string ClearDate(string textBoxID, string calendarExtenderID)
        {
            return "document.getElementById('" + textBoxID + "').value='';" +
                         "$find('" + calendarExtenderID + "').set_selectedDate(null);" +
                         "return false;";
        }

        public static string CheckDateRange(string fromDateTextBoxID, string toDateTextBoxID, string message)
        {
            return
                "return CheckDateRange(document.getElementById('" + fromDateTextBoxID + "').value, document.getElementById('" +
                toDateTextBoxID + "').value, '" + message + "');";
        }

        public static string PopulateDefaultFromTime(string fromTimeTextBoxID)
        {
            return "if(document.getElementById('" + fromTimeTextBoxID + "').value == '') { document.getElementById('" + fromTimeTextBoxID + "').value = '00:00:00.000'; }";
        }

        public static string PopulateDefaultToTime(string toTimeTextBoxID)
        {
            return "if(document.getElementById('" + toTimeTextBoxID + "').value == '') { document.getElementById('" + toTimeTextBoxID + "').value = '23:59:59.999'; }";
        }

    }
}
