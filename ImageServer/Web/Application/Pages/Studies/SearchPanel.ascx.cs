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
using System.Configuration;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens = ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens;
using Resources;
using SR=Resources.SR;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies
{
    public class SearchPanelButtonClickedEventArgs:EventArgs
    {
        public IEnumerable<StudySummary> SelectedStudies { get; set; }
    }

    [ClientScriptResource(ComponentType="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel", ResourcePath="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SearchPanel.js")]
    public partial class SearchPanel : AJAXScriptControl, IStudySearchPage
    {
        #region Private members

        private readonly StudyController _controller = new StudyController();
        private EventHandler<SearchPanelButtonClickedEventArgs> _deleteButtonClickedHandler;
        private EventHandler<SearchPanelButtonClickedEventArgs> _assignAuthorityGroupsButtonClickedHandler;
        private static IStudySearchPageExtension _customizer;
        #endregion Private members

        #region Class Constructor

        static SearchPanel()
        {
            LoadCustomzier();
        }

        #endregion

        #region Events
        public event EventHandler<SearchPanelButtonClickedEventArgs> DeleteButtonClicked
        {
            add { _deleteButtonClickedHandler += value; }
            remove { _deleteButtonClickedHandler -= value; }
        }

        public event EventHandler<SearchPanelButtonClickedEventArgs> AssignAuthorityGroupsButtonClicked
        {
            add { _assignAuthorityGroupsButtonClickedHandler += value; }
            remove { _assignAuthorityGroupsButtonClickedHandler -= value; }
        }
        #endregion

        #region Private Properties
        
        private bool DisplaySearchWarning
        {
            get
            {
                return String.IsNullOrEmpty(PatientId.TrimText) &&
                       String.IsNullOrEmpty(PatientName.TrimText) &&
                       String.IsNullOrEmpty(AccessionNumber.TrimText) &&
                       String.IsNullOrEmpty(ToStudyDate.Text) &&
                       String.IsNullOrEmpty(FromStudyDate.Text) &&
                       String.IsNullOrEmpty(StudyDescription.TrimText) &&
                       String.IsNullOrEmpty(ReferringPhysiciansName.TrimText) &&
                       String.IsNullOrEmpty(ResponsibleOrganization.TrimText) &&
                       String.IsNullOrEmpty(ResponsiblePerson.TrimText) &&
                       ModalityListBox.SelectedIndex < 0 &&
                       StatusListBox.SelectedIndex < 0 &&
                       ConfigurationManager.AppSettings["DisplaySearchWarning"].ToLower().Equals("true");
            }
        }

        #endregion

        #region Public Properties

        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenButtonClientID")]
        public string OpenButtonClientID
        {
            get { return ViewStudyDetailsButton.ClientID; }
        }

		[ExtenderControlProperty]
		[ClientPropertyName("RestoreButtonClientID")]
		public string RestoreButtonClientID
		{
			get { return RestoreStudyButton.ClientID; }
		}

        [ExtenderControlProperty]
        [ClientPropertyName("SendButtonClientID")]
        public string SendButtonClientID
        {
            get { return MoveStudyButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("AssignAuthorityGroupsButtonClientID")]
        public string AssignAuthorityGroupsButtonClientID
        {
            get { return AssignAuthorityGroupsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("StudyListClientID")]
        public string StudyListClientID
        {
            get { return StudyListGridView.TheGrid.ClientID; }
        }
        
        [ExtenderControlProperty]
        [ClientPropertyName("OpenStudyPageUrl")]
        public string OpenStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("SendStudyPageUrl")]
        public string SendStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.MoveStudyPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewImagePageUrl")]
        public string ViewImagePageUrl
        {
            get
            {
//                if (!Thread.CurrentPrincipal.IsInRole(ClearCanvas.ImageViewer.AuthorityTokens.ViewerClinical))
//                    return Page.ResolveClientUrl(ImageServerConstants.PageURLs.EmbeddedViewImagesPage);

                return Page.ResolveClientUrl(ConfigurationManager.AppSettings["WebViewerPage"] ?? ImageServerConstants.PageURLs.ErrorPage);
            }
        }
        
        [ExtenderControlProperty]
        [ClientPropertyName("CanViewImages")]
        public bool CanViewImages
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Study.ViewImages); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenNewWindow")]
        public bool OpenNewWindow
        {
            get { return false; }
        }


        [ExtenderControlProperty]
        [ClientPropertyName("ViewImageButtonClientID")]
        public string ViewImageButtonClientID
        {
            get { return ViewImagesButton.ClientID; }
        }

        public ServerPartition ServerPartition { get; set; }

        Control IStudySearchPage.SearchFieldsContainer
        {
            get { return SearchFieldsContainer; }
        }

        #endregion Public Properties  

        #region Private Methods

        private static void LoadCustomzier()
        {
            try
            {
                _customizer = new StudySearchPageExtensionPoint().CreateExtension() as IStudySearchPageExtension;
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Debug, ex);
            }
        }

        private void SetupChildControls()
        {
            foreach(StudyStatusEnum s in  StudyStatusEnum.GetAll())
            {
                StatusListBox.Items.Add(new ListItem { Text = ServerEnumDescription.GetLocalizedDescription(s), Value = s.Lookup});
            }

            ClearToStudyDateButton.Attributes["onclick"] = ScriptHelper.ClearDate(ToStudyDate.ClientID, ToStudyDateCalendarExtender.ClientID);
            ClearFromStudyDateButton.Attributes["onclick"] = ScriptHelper.ClearDate(FromStudyDate.ClientID, FromStudyDateCalendarExtender.ClientID);
			SearchButton.Attributes["onclick"] = ScriptHelper.CheckDateRange(FromStudyDate.ClientID, ToStudyDate.ClientID, "To Date must be greater than From Date");
            
            GridPagerTop.InitializeGridPager(SR.GridPagerStudySingleItem, SR.GridPagerStudyMultipleItems, StudyListGridView.TheGrid,
                                             () => StudyListGridView.ResultCount, ImageServerConstants.GridViewPagerPosition.Top);
            StudyListGridView.Pager = GridPagerTop;

            ConfirmStudySearchMessageBox.Confirmed += delegate
                                                          {
                                                              StudyListGridView.DataBindOnPreRender =
                                                                  true;
                                                              StudyListGridView.Refresh();
                                                              if (SearchUpdatePanel.UpdateMode ==
                                                                  UpdatePanelUpdateMode.Conditional)
                                                                  SearchUpdatePanel.Update();
                                                          };
            ConfirmStudySearchMessageBox.Cancel += delegate
                                                       {
                                                           StudyListGridView.DataBindOnPreRender = false;
                                                       };

            RestoreMessageBox.Confirmed += delegate(object data)
                            {
                                if (data is IList<Study>)
                                {
                                    var studies = data as IList<Study>;
                                    foreach (Study study in studies)
                                    {
                                        _controller.RestoreStudy(study);
                                    }
                                }
								else if (data is IList<StudySummary>)
								{
									var studies = data as IList<StudySummary>;
									foreach (StudySummary study in studies)
									{
                                        _controller.RestoreStudy(study.TheStudy);
									}
								}
                                else if (data is Study)
                                {
                                    var study = data as Study;
                                    _controller.RestoreStudy(study);
                                }

                                DataBind();
                                SearchUpdatePanel.Update(); // force refresh
                            };

            StudyListGridView.DataSourceCreated += delegate(StudyDataSource source)
                                        {
                                            source.Partition = ServerPartition;
                                            source.DateFormats = ToStudyDateCalendarExtender.Format;

                                            if (!String.IsNullOrEmpty(PatientId.TrimText))
                                                source.PatientId = SearchHelper.TrailingWildCard(PatientId.TrimText);
                                            if (!String.IsNullOrEmpty(PatientName.TrimText))
                                                source.PatientName = SearchHelper.NameWildCard(PatientName.TrimText);
                                            if (!String.IsNullOrEmpty(AccessionNumber.TrimText))
                                                source.AccessionNumber = SearchHelper.TrailingWildCard(AccessionNumber.TrimText);
                                            if (!String.IsNullOrEmpty(ToStudyDate.Text))
                                                source.ToStudyDate = ToStudyDate.Text;
                                            if (!String.IsNullOrEmpty(FromStudyDate.Text))
                                                source.FromStudyDate = FromStudyDate.Text;
                                            if (!String.IsNullOrEmpty(StudyDescription.TrimText))
                                                source.StudyDescription = SearchHelper.LeadingAndTrailingWildCard(StudyDescription.TrimText);
                                            if (!String.IsNullOrEmpty(ReferringPhysiciansName.TrimText))
                                                source.ReferringPhysiciansName = SearchHelper.NameWildCard(ReferringPhysiciansName.TrimText);
                                            if (!String.IsNullOrEmpty(ResponsiblePerson.TrimText))
                                                source.ResponsiblePerson = SearchHelper.NameWildCard(ResponsiblePerson.TrimText);
                                            if (!String.IsNullOrEmpty(ResponsibleOrganization.TrimText))
                                                source.ResponsibleOrganization = SearchHelper.NameWildCard(ResponsibleOrganization.TrimText);
                                            
                                            if (ModalityListBox.SelectedIndex > -1)
                                            {
                                                var modalities = new List<string>();
                                                foreach (ListItem item in ModalityListBox.Items)
                                                {
                                                    if (item.Selected)
                                                    {
                                                        modalities.Add(item.Value);
                                                    }
                                                }
                                                source.Modalities = modalities.ToArray();
                                            }

                                            if (StatusListBox.SelectedIndex > -1)
                                            {
                                                var statuses = new List<string>();
                                                foreach (ListItem status in StatusListBox.Items)
                                                {
                                                    if (status.Selected)
                                                    {
                                                        statuses.Add(status.Value);
                                                    }
                                                }
                                                source.Statuses = statuses.ToArray();
                                            }
                                        };

            //Set Roles
            ViewImagesButton.Roles = AuthorityTokens.Study.ViewImages;
            ViewStudyDetailsButton.Roles = AuthorityTokens.Study.View;
            MoveStudyButton.Roles = AuthorityTokens.Study.Move;
            DeleteStudyButton.Roles = AuthorityTokens.Study.Delete;
            RestoreStudyButton.Roles = AuthorityTokens.Study.Restore;
            AssignAuthorityGroupsButton.Roles = ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.Study.EditDataAccess;
        }

    	#endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Remove all filter settings.
        /// </summary>
        public void Clear()
        {
            PatientId.Text = string.Empty;
            PatientName.Text = string.Empty;
            AccessionNumber.Text = string.Empty;
            StudyDescription.Text = string.Empty;
            ToStudyDate.Text = string.Empty;
            FromStudyDate.Text = string.Empty;
            ReferringPhysiciansName.Text = string.Empty;

            foreach (ListItem item in StatusListBox.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                }
            }

            foreach (ListItem item in ModalityListBox.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                }
            }
        }

        public void Refresh()
        {
            if (!StudyListGridView.IsDataSourceSet()) StudyListGridView.SetDataSource();
            StudyListGridView.RefreshCurrentPage();
            SearchUpdatePanel.Update();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SetupChildControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_customizer!=null)
            {
                _customizer.OnPageLoad(this);
            }  
        }

        internal void Reset()
        {
            Clear();
            StudyListGridView.Reset(); 
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {   
            if(DisplaySearchWarning) {
                StudyListGridView.DataBindOnPreRender = false;
                ConfirmStudySearchMessageBox.Message = SR.NoFiltersSearchWarning;
                   ConfirmStudySearchMessageBox.MessageStyle = "font-weight: bold; color: #205F87;";
                ConfirmStudySearchMessageBox.Show();
            } 
            else
            {
                StudyListGridView.Refresh();    
            }
        	var sb = new StringBuilder();
			if(!String.IsNullOrEmpty(PatientId.TrimText))
				sb.AppendFormat("PatientId={0};", PatientId.TrimText);
            if(!String.IsNullOrEmpty(PatientName.TrimText))
				sb.AppendFormat("PatientsName={0};", PatientName.TrimText);
            if(!String.IsNullOrEmpty(AccessionNumber.TrimText))
				sb.AppendFormat("AccessionNumber={0};", AccessionNumber.TrimText);
            if(!String.IsNullOrEmpty(ToStudyDate.Text)||!String.IsNullOrEmpty(FromStudyDate.Text))
				sb.AppendFormat("StudyDate={0}-{1};", FromStudyDate.Text, ToStudyDate.Text);
            if(!String.IsNullOrEmpty(StudyDescription.TrimText))
				sb.AppendFormat("StudyDescription={0};", StudyDescription.TrimText);
			if (ModalityListBox.SelectedIndex < 0)
			{
				bool first = true;
				foreach (ListItem item in ModalityListBox.Items)
				{
					if (!item.Selected) continue;

					if (first)
					{
						sb.AppendFormat("ModalitiesInStudy={0}", item.Value);
						first = false;
					}
					else
					{
						sb.AppendFormat(",{0}", item.Value);
					}
				}
				if (!first)
					sb.Append(';');
			}

        	var helper = new QueryAuditHelper(ServerPlatform.AuditSource, EventIdentificationContentsEventOutcomeIndicator.Success,
				new AuditPersonActiveParticipant(SessionManager.Current.Credentials.UserName,
											 null,
											 SessionManager.Current.Credentials.DisplayName),
				ServerPartition.AeTitle,ServerPlatform.HostId,sb.ToString());
			ServerAuditHelper.LogAuditMessage(helper);
        }
        
		protected void RestoreStudyButton_Click(object sender, ImageClickEventArgs e)
		{
			IList<StudySummary> studies = StudyListGridView.SelectedStudies;

			if (studies != null && studies.Count > 0)
			{
			    string message = studies.Count > 1 ? string.Format(SR.MultipleStudyRestore):
				                                    string.Format(SR.SingleStudyRestore);

			    RestoreMessageBox.Message = DialogHelper.createConfirmationMessage(message);
                RestoreMessageBox.Message += DialogHelper.createStudyTable(studies);
				
			    RestoreMessageBox.Title = Titles.RestoreStudyConfirmation;
                RestoreMessageBox.MessageType = MessageBox.MessageTypeEnum.YESNO;
				IList<Study> studyList = new List<Study>();
				foreach (StudySummary summary in studies)
                    studyList.Add(summary.TheStudy);
				RestoreMessageBox.Data = studyList;
				RestoreMessageBox.Show();
			}
		}

        protected void DeleteStudyButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyListGridView.RefreshCurrentPage();
            var args = new SearchPanelButtonClickedEventArgs
                                                         {
                                                             SelectedStudies = StudyListGridView.SelectedStudies
                                                         };
            EventsHelper.Fire(_deleteButtonClickedHandler, this, args);
        }

        protected void AssignAuthorityGroupsButton_Click(object sender, ImageClickEventArgs e)
        {
            StudyListGridView.RefreshCurrentPage();
            var args = new SearchPanelButtonClickedEventArgs
                                                         {
                                                             SelectedStudies = StudyListGridView.SelectedStudies
                                                         };
            EventsHelper.Fire(_assignAuthorityGroupsButtonClickedHandler, this, args);
        }

        protected void OpenStudyButton_Click(object sender, ImageClickEventArgs e)
        {
            foreach(StudySummary study in StudyListGridView.SelectedStudies)
            {
                var url = String.Format("{0}?study={1}", ImageServerConstants.PageURLs.StudyDetailsPage, study.StudyInstanceUid);

                string script = String.Format("window.open(\"{0}\", \"{1}\");", Page.ResolveClientUrl(url), "_blank");

                ScriptManager.RegisterStartupScript(Page, typeof(Page), "Redirect", script, true);
            }
        }

        #endregion Protected Methods
    }
}