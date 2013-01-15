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
using System.Globalization;
using System.Security.Permissions;
using System.Web;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Alert.View)]
    public partial class AlertIndicator : System.Web.UI.UserControl
    {
        protected IList<Alert> alerts;
       
        protected void Page_Load(object sender, EventArgs e)
        {           
            var controller = new AlertController();
            var criteria = new AlertSelectCriteria();

            criteria.AlertLevelEnum.EqualTo(AlertLevelEnum.Critical);
            criteria.InsertTime.SortDesc(1);

            AlertsCount.Text = controller.GetAlertsCount(criteria).ToString(CultureInfo.InvariantCulture);

            alerts = controller.GetAlerts(criteria);

            if (alerts.Count > 0) {

                int rows = 0;
                foreach (Alert alert in alerts)
                {
                    var alertRow = new TableRow();

                    alertRow.Attributes.Add("class", "AlertTableCell");

                    var component = new TableCell();
                    var source = new TableCell();
                    var description = new TableCell();

                    description.Wrap = false;

                    component.Text = alert.Component;
                    component.Wrap = false;
                    source.Text = alert.Source;
                    source.Wrap = false;

                    string content = alert.Content.GetElementsByTagName("Message").Item(0).InnerText;
                    description.Text = content.Length < 50 ? content : content.Substring(0, 50);
                    description.Text += " ...";
                    description.Wrap = false;

                    alertRow.Cells.Add(component);
                    alertRow.Cells.Add(source);
                    alertRow.Cells.Add(description);

                    AlertTable.Rows.Add(alertRow);

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