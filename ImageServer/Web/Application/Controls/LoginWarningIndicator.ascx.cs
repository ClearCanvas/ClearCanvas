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
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class LoginWarningIndicator : System.Web.UI.UserControl
    {     
        protected void Page_Load(object sender, EventArgs e)
        {

            WarningsCount.Text = SessionManager.Current.User.WarningMessages.Count.ToString(CultureInfo.InvariantCulture);

            if (SessionManager.Current.User.WarningMessages.Count > 0)
            {

                int rows = 0;
                foreach (string alert in SessionManager.Current.User.WarningMessages)
                {
                    var alertRow = new TableRow();

                    alertRow.Attributes.Add("class", "AlertTableCell");

                    var component = new TableCell
                        {
                            Text = alert, Wrap = true
                        };

                    alertRow.Cells.Add(component);

                    WarningTable.Rows.Add(alertRow);

                    rows++;
                    if (rows == 5) break;
                }
            }
        }

        protected void SwitchToEnglishClicked(object sender, EventArgs e)
        {
            HttpContext.Current.Items["Language"] = "en";
        }
    }
}