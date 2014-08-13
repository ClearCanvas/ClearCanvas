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
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
	public partial class ApplicationLogSearchPanel : System.Web.UI.UserControl
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !Page.IsAsync)
            {
                string startTime = Request["From"];
                string endTime = Request["To"];
                string hostname = Request["HostName"];

                if (startTime != null && endTime != null)
                {
                    string[] start = startTime.Split(' ');
                    string[] end = endTime.Split(' ');

                    FromDateFilter.Text = start[0];
                    ToDateFilter.Text = end[0];
                    FromDateCalendarExtender.SelectedDate = DateTime.Parse(start[0]);
                    ToDateCalendarExtender.SelectedDate = DateTime.Parse(end[0]);
                    FromTimeFilter.Text = start[1] + ".000";
                    ToTimeFilter.Text = end[1] + ".000";
                    HostFilter.TrimText = hostname;
                    ApplicationLogGridView.SetDataSource();
                    ApplicationLogGridView.Refresh();
                }
            }
        }

	    protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            ClearToDateFilterButton.Attributes["onclick"] = ScriptHelper.ClearDate(ToDateFilter.ClientID, ToDateCalendarExtender.ClientID);
            ClearFromDateFilterButton.Attributes["onclick"] = ScriptHelper.ClearDate(FromDateFilter.ClientID, FromDateCalendarExtender.ClientID);
            ToDateFilter.Attributes["OnChange"] = ScriptHelper.PopulateDefaultToTime(ToTimeFilter.ClientID) + " return false;";
            FromDateFilter.Attributes["OnChange"] = ScriptHelper.PopulateDefaultFromTime(FromTimeFilter.ClientID) + " return false;";
			SearchButton.Attributes["onclick"] = ScriptHelper.CheckDateRange(FromDateFilter.ClientID, ToDateFilter.ClientID, SR.ToFromDateValidationError);
			GridPagerTop.InitializeGridPager(SR.GridPagerApplicationLogSingleItem, SR.GridPagerApplicationLogMultipleItems, ApplicationLogGridView.ApplicationLogListGrid, delegate { return ApplicationLogGridView.ResultCount; }, ImageServerConstants.GridViewPagerPosition.Top);
		    ApplicationLogGridView.Pager = GridPagerTop;

			ApplicationLogGridView.DataSourceCreated += delegate(ApplicationLogDataSource source)
			                                       	{
														if (!String.IsNullOrEmpty(HostFilter.TrimText))
															source.Host = SearchHelper.LeadingAndTrailingWildCard(HostFilter.TrimText);
														if (!String.IsNullOrEmpty(ThreadFilter.TrimText))
															source.Thread = SearchHelper.LeadingAndTrailingWildCard(ThreadFilter.TrimText);
														if (!String.IsNullOrEmpty(MessageFilter.TrimText))
															source.Message = SearchHelper.LeadingAndTrailingWildCard(MessageFilter.TrimText);
														if (!String.IsNullOrEmpty(LogLevelListBox.SelectedValue))
															if (!LogLevelListBox.SelectedValue.Equals("ANY"))
																source.LogLevel = LogLevelListBox.SelectedValue;
														if (!String.IsNullOrEmpty(FromDateFilter.Text) || !String.IsNullOrEmpty(FromTimeFilter.Text))
														{
															DateTime val;

															if (DateTime.TryParseExact(FromDateFilter.Text + " " + FromTimeFilter.Text,DateTimeFormatter.DefaultTimestampFormat,CultureInfo.CurrentCulture,DateTimeStyles.None, out val))
																source.StartDate = val;
															else if (DateTime.TryParse(FromDateFilter.Text + " " + FromTimeFilter.Text, out val))
																source.StartDate = val;
														}

			                                       		if (!String.IsNullOrEmpty(ToDateFilter.Text) || !String.IsNullOrEmpty(ToTimeFilter.Text))
														{
															DateTime val;
															if (DateTime.TryParseExact(ToDateFilter.Text + " " + ToTimeFilter.Text, DateTimeFormatter.DefaultTimestampFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out val))
																source.EndDate = val;
															else if (DateTime.TryParse(ToDateFilter.Text + " " + ToTimeFilter.Text, out val))
																source.EndDate = val;
														}
														
													};

		}

		protected void SearchButton_Click(object sender, ImageClickEventArgs e)
		{
            if(Page.IsValid) ApplicationLogGridView.Refresh();
		}
	}
}