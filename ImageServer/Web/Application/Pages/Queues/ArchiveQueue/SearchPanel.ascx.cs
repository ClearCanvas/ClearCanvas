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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;
using Resources;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Queues.ArchiveQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        private readonly ArchiveQueueController _controller = new ArchiveQueueController();

        #endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("OpenButtonClientID")]
		public string OpenButtonClientID
		{
			get { return ViewStudyDetailsButton.ClientID; }
		}

		[ExtenderControlProperty]
		[ClientPropertyName("ResetButtonClientID")]
		public string ResetButtonClientID
		{
			get { return ResetItemButton.ClientID; }
		}

		[ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return ArchiveQueueItemList.ArchiveQueueGrid.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("OpenStudyPageUrl")]
		public string OpenStudyPageUrl
		{
			get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
		}

        public Default EnclosingPage { get; set; }

        /// <summary>
        /// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
        /// </summary>
        public ServerPartition ServerPartition { get; set; }

        #endregion Public Properties  

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            ScheduleDate.Text = string.Empty;
            StatusFilter.SelectedIndex = 0;
        }

        public void Refresh()
        {
            
        }

        internal void Reset()
        {
            Clear();
            ArchiveQueueItemList.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearScheduleDateButton.OnClientClick = ScriptHelper.ClearDate(ScheduleDate.ClientID, ScheduleDateCalendarExtender.ClientID);
                           
            // setup child controls
            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem, Labels.GridPagerQueueMultipleItems, ArchiveQueueItemList.ArchiveQueueGrid,
                                             () => ArchiveQueueItemList.ResultCount, ImageServerConstants.GridViewPagerPosition.Top);
            ArchiveQueueItemList.Pager = GridPagerTop;

            MessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Model.ArchiveQueue>)
                                {
                                    var items = data as IList<Model.ArchiveQueue>;
                                    foreach (Model.ArchiveQueue item in items)
                                    {
                                        _controller.DeleteArchiveQueueItem(item);
                                    }
                                }
                                else if (data is Model.ArchiveQueue)
                                {
                                    var item = data as Model.ArchiveQueue;
                                    _controller.DeleteArchiveQueueItem(item);
                                }

                                ArchiveQueueItemList.RefreshCurrentPage();
                                SearchUpdatePanel.Update(); // force refresh

                            };

			ArchiveQueueItemList.DataSourceCreated += delegate(ArchiveQueueDataSource source)
										{
											source.Partition = ServerPartition;
                                            source.DateFormats = ScheduleDateCalendarExtender.Format;

                                            if (!String.IsNullOrEmpty(StatusFilter.SelectedValue) && StatusFilter.SelectedIndex > 0)
                                                source.StatusEnum = ArchiveQueueStatusEnum.GetEnum(StatusFilter.SelectedValue);
                                            if (!String.IsNullOrEmpty(PatientId.TrimText))
												source.PatientId = SearchHelper.TrailingWildCard(PatientId.TrimText);
											if (!String.IsNullOrEmpty(PatientName.TrimText))
												source.PatientName = SearchHelper.NameWildCard(PatientName.TrimText);
											if (!String.IsNullOrEmpty(ScheduleDate.Text))
												source.ScheduledDate = ScheduleDate.Text;
										};
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IList<ArchiveQueueStatusEnum> statusItems = ArchiveQueueStatusEnum.GetAll();

            int prevSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem(SR.All, "All"));
            foreach (ArchiveQueueStatusEnum s in statusItems)
                StatusFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(s), s.Lookup));
            StatusFilter.SelectedIndex = prevSelectedIndex;

            DeleteItemButton.Roles = AuthorityTokens.ArchiveQueue.Delete;
        	ViewStudyDetailsButton.Roles = AuthorityTokens.Study.View;

			if (!IsPostBack && !Page.IsAsync)
			{
				var patientId = Server.UrlDecode(Request["PatientID"]);
				var patientName = Server.UrlDecode(Request["PatientName"]);
				if (patientId != null || patientName != null)
				{
					PatientId.TrimText = patientId;
					PatientName.TrimText = patientName;

					ArchiveQueueItemList.SetDataSource();
					ArchiveQueueItemList.Refresh();
				}
			}
        }
       
        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            ArchiveQueueItemList.Refresh();
        }

        protected void DeleteItemButton_Click(object sender, EventArgs e)
        {            
            IList<Model.ArchiveQueue> items = ArchiveQueueItemList.SelectedItems;

            if (items != null && items.Count>0)
            {
                if (items.Count > 1) MessageBox.Message = string.Format(SR.MultipleArchiveQueueDelete);
                else MessageBox.Message = string.Format(SR.SingleArchiveQueueDelete);

                MessageBox.Message += "<table style=\"border: solid #CCCCCC 2px; margin-top: 5px;\">";
                foreach (Model.ArchiveQueue item in items)
                {
                    MessageBox.Message += String.Format("<tr><td style=\"font-weight: bold; color: #618FAD\">{0}:</td><td style=\"font-weight: normal; color: black;\">{1}</td></tr>", 
                        SR.StudyInstanceUID,            
                        StudyStorage.Load(item.StudyStorageKey).StudyInstanceUid);
                }
                MessageBox.Message += "</table>";

                MessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
                MessageBox.MessageStyle = "color: #FF0000; font-weight: bold;";
                MessageBox.Data = items;
                MessageBox.Show();
            }
        }

        #endregion Protected Methods

    	protected void ResetItemButton_Click(object sender, ImageClickEventArgs e)
    	{
			if (ArchiveQueueItemList.SelectedItems == null)
				DataBind();

			if (ArchiveQueueItemList.SelectedItems.Count > 0)
			{
				EnclosingPage.ResetArchiveQueueItem(ArchiveQueueItemList.SelectedItems);
				ArchiveQueueItemList.RefreshCurrentPage();
			}
    	}
    }
}