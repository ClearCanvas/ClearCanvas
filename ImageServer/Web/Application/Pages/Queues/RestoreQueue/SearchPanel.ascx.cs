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

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue
{
    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl
    {
        #region Private members

        private readonly RestoreQueueController _controller = new RestoreQueueController();
    	private ServerPartition _serverPartition;

    	#endregion Private members

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteItemButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ItemListClientID")]
        public string ItemListClientID
        {
            get { return RestoreQueueItemList.RestoreQueueGrid.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("OpenButtonClientID")]
		public string OpenButtonClientID
		{
			get { return ViewStudyDetailsButton.ClientID; }
		}

		/// <summary>
		/// Gets the <see cref="Model.ServerPartition"/> associated with this search panel.
		/// </summary>
		public ServerPartition ServerPartition
		{
			get { return _serverPartition; }
			set { _serverPartition = value; }
		}

		[ExtenderControlProperty]
		[ClientPropertyName("OpenStudyPageUrl")]
		public string OpenStudyPageUrl
		{
			get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
		}
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
            RestoreQueueItemList.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearScheduleDateButton.OnClientClick = ScriptHelper.ClearDate(ScheduleDate.ClientID, ScheduleDateCalendarExtender.ClientID);
                          
            // setup child controls
            GridPagerTop.InitializeGridPager(Labels.GridPagerQueueSingleItem, Labels.GridPagerQueueMultipleItems, RestoreQueueItemList.RestoreQueueGrid,
                                             () => RestoreQueueItemList.ResultCount, ImageServerConstants.GridViewPagerPosition.Top);
            RestoreQueueItemList.Pager = GridPagerTop;

            MessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Model.RestoreQueue>)
                                {
                                    var items = data as IList<Model.RestoreQueue>;
                                    foreach (Model.RestoreQueue item in items)
                                    {
                                        _controller.DeleteRestoreQueueItem(item);
                                    }
                                }
                                else if (data is Model.RestoreQueue)
                                {
                                    var item = data as Model.RestoreQueue;
                                    _controller.DeleteRestoreQueueItem(item);
                                }

                                DataBind();
                                SearchUpdatePanel.Update(); // force refresh

                            };

			RestoreQueueItemList.DataSourceCreated += delegate(RestoreQueueDataSource source)
										{
											source.Partition = ServerPartition;
                                            source.DateFormats = ScheduleDateCalendarExtender.Format;

                                            if (!String.IsNullOrEmpty(StatusFilter.SelectedValue) && StatusFilter.SelectedIndex > 0)
                                                source.StatusEnum = RestoreQueueStatusEnum.GetEnum(StatusFilter.SelectedValue);
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
            IList<RestoreQueueStatusEnum> statusItems = RestoreQueueStatusEnum.GetAll();

            int prevSelectedIndex = StatusFilter.SelectedIndex;
            StatusFilter.Items.Clear();
            StatusFilter.Items.Add(new ListItem(SR.All, "All"));
            foreach (RestoreQueueStatusEnum s in statusItems)
                StatusFilter.Items.Add(new ListItem(ServerEnumDescription.GetLocalizedDescription(s), s.Lookup));
            StatusFilter.SelectedIndex = prevSelectedIndex;

            DeleteItemButton.Roles = AuthorityTokens.RestoreQueue.Delete;
			ViewStudyDetailsButton.Roles = AuthorityTokens.Study.View;

			if (!IsPostBack && !Page.IsAsync)
			{
				var patientId = Server.UrlDecode(Request["PatientID"]);
				var patientName = Server.UrlDecode(Request["PatientName"]);
				if (patientId != null || patientName != null)
				{
					PatientId.TrimText = patientId;
					PatientName.TrimText = patientName;

					RestoreQueueItemList.SetDataSource();
					RestoreQueueItemList.Refresh();
				}
			}
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            RestoreQueueItemList.Refresh();
        }

        protected void DeleteItemButton_Click(object sender, EventArgs e)
        {
            RestoreQueueItemList.RefreshCurrentPage();
            
            IList<Model.RestoreQueue> items = RestoreQueueItemList.SelectedItems;

            if (items != null && items.Count>0)
            {
                if (items.Count > 1) MessageBox.Message = string.Format(SR.MultipleRestoreQueueDelete);
                else MessageBox.Message = string.Format(SR.SingleRestoreQueueDelete);

                MessageBox.Message += "<table>";
                foreach (Model.RestoreQueue item in items)
                {
                    String text = "";
                    String.Format("<tr align='left'><td>{0}:{1}</td></tr>",
                                    SR.StudyInstanceUID,
                                    StudyStorage.Load(item.StudyStorageKey).StudyInstanceUid);
                    MessageBox.Message += text;
                }
                MessageBox.Message += "</table>";

                MessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
                MessageBox.Data = items;
                MessageBox.Show();
            }
        }

        #endregion Protected Methods
    }
}