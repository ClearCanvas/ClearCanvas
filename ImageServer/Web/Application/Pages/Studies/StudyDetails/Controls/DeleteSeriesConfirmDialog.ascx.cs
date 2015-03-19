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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Authentication;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Security;
using SR = Resources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [Serializable]
    public class DeleteSeriesInfo
    {
        private Study _study;
        private Series _series;
        private ServerEntityKey _studyKey;
        private string _serverPartitionAE;
        private string _seriesNumber;
        private string _modality;
        private string _description;
        private int _numberOfRelatedSeries;
        private string _seriesInstanceUid;
        private string _performedProcedureStepStartDate;
        private string _performedProcedureStepStartTime;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        public string SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        public string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int NumberOfSeriesRelatedInstances
        {
            get { return _numberOfRelatedSeries; }
            set { _numberOfRelatedSeries = value; }
        }

        public string PerformedProcedureStepStartDate
        {
            get { return _performedProcedureStepStartDate; }
            set { _performedProcedureStepStartDate = value; }
        }

        public string PerformedProcedureStepStartTime
        {
            get { return _performedProcedureStepStartTime; }
            set { _performedProcedureStepStartTime = value; }
        }

        public ServerEntityKey StudyKey
        {
            get { return _studyKey; }
            set { _studyKey = value; }
        }

        public string ServerPartitionAE
        {
            get { return _serverPartitionAE; }
            set { _serverPartitionAE = value; }
        }
    }

    public class DeleteSeriesConfirmDialogSeriesDeletingEventArgs : EventArgs
    {
        private IList<DeleteSeriesInfo> _deletingSeries;
        private string _reasonForDeletion;

        public IList<DeleteSeriesInfo> DeletingSeries
        {
            get { return _deletingSeries; }
            set { _deletingSeries = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public class DeleteSeriesConfirmDialogSeriesDeletedEventArgs:EventArgs
    {
        private IList<DeleteSeriesInfo> _deletedSeries;
        private string _reasonForDeletion;

        public IList<DeleteSeriesInfo> DeletedSeries
        {
            get { return _deletedSeries; }
            set { _deletedSeries = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public partial class DeleteSeriesConfirmDialog : UserControl
    {
        private const string REASON_CANNEDTEXT_CATEGORY = "DeleteSeriesReason";
        private EventHandler<DeleteSeriesConfirmDialogSeriesDeletingEventArgs> _seriesDeletingHandler;
        private EventHandler<DeleteSeriesConfirmDialogSeriesDeletedEventArgs> _seriesDeletedHandler;

        public event EventHandler<DeleteSeriesConfirmDialogSeriesDeletingEventArgs> SeriesDeleting
        {
            add { _seriesDeletingHandler += value; }
            remove { _seriesDeletingHandler -= value; }
        }
        
        public event EventHandler<DeleteSeriesConfirmDialogSeriesDeletedEventArgs> SeriesDeleted
        {
            add { _seriesDeletedHandler += value; }
            remove { _seriesDeletedHandler -= value; }
        }
        
        public IList<DeleteSeriesInfo> DeletingSeries
        {
            get
            {
                return ViewState["DeletedSeries"] as IList<DeleteSeriesInfo>;
            }
            set { ViewState["DeletedSeries"] = value; }
        }

        public bool DeleteEntireStudy
        {
            get
            {
                if (ViewState["DeleteEntireStudy"] == null) return false;
                bool deleteEntireStudy;
                Boolean.TryParse(ViewState["DeleteEntireStudy"].ToString(), out deleteEntireStudy);
                return deleteEntireStudy;
            }
            set { ViewState["DeleteEntireStudy"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (DeleteEntireStudy) DeleteEntireStudyLabel.Visible = true;
            else DeleteEntireStudyLabel.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Set up the control to handle custom reasons if the user has the authority.
            if (!SessionManager.Current.User.IsInRole(AuthorityTokens.Study.SaveReason))
            {
                ReasonSavePanel.Visible = false;
                SaveReasonAsName.Attributes.Add("display", "none");
                SaveReasonAsNameValidator.Enabled = false;
            }
            else
            {
                //Hide/Disable the "Save As Reason" textbox/validation depending on whether the user is using a custom reason or not.
                ReasonListBox.Attributes.Add("onchange", "if(document.getElementById('" + ReasonListBox.ClientID + "').options[document.getElementById('" + ReasonListBox.ClientID + "').selectedIndex].text != '" + SR.CustomReason + "') { document.getElementById('" + ReasonSavePanel.ClientID + "').style.display = 'none'; document.getElementById('" + SaveReasonAsName.ClientID + "').style.display = 'none'; } else { document.getElementById('" + ReasonSavePanel.ClientID + "').style.display = 'table-row'; document.getElementById('" + SaveReasonAsName.ClientID + "').style.display = 'table-cell'; }");
                ReasonListBox.SelectedIndexChanged += delegate
                {
                    if (ReasonListBox.SelectedItem.Text == SR.CustomReason) SaveReasonAsNameValidator.Enabled = true;
                    else SaveReasonAsNameValidator.Enabled = false;
                };
            }
        }

        private void ClearInputs()
        {
            SaveReasonAsName.Text = "";
            if (ReasonListBox.Items.Count > 0)
            {
                ReasonListBox.SelectedIndex = 0;
                if (string.IsNullOrEmpty(ReasonListBox.SelectedValue))
                {
                    Comment.Text = string.Empty;
                }
                else
                {
                    Comment.Text = SR.CustomReasonComment;
                }
            } else
            {
                Comment.Text = string.Empty;
            }
        }

        public override void DataBind()
        {
            SeriesListing.DataSource = DeletingSeries;

            EnsurePredefinedReasonsLoaded();
            
            base.DataBind();
        }

        private void EnsurePredefinedReasonsLoaded()
        {
            ReasonListBox.Items.Clear();

			ICannedTextEntityBroker broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<ICannedTextEntityBroker>();
            CannedTextSelectCriteria criteria = new CannedTextSelectCriteria();
            criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
            IList<CannedText> list = broker.Find(criteria);

            if (SessionManager.Current.User.IsInRole(AuthorityTokens.Study.SaveReason))
            {
                ReasonListBox.Items.Add(new ListItem(SR.CustomReason, SR.CustomReasonComment));
            }
            else
            {
                ReasonListBox.Items.Add(new ListItem(SR.SelectOne, string.Empty));
            }
            foreach (CannedText text in list)
            {
                ReasonListBox.Items.Add(new ListItem(text.Label, text.Text));
            }                
        }

        protected void DeleteSeriesButton_Clicked(object sender, ImageClickEventArgs e)
        {

            if (DeletingSeries==null || DeletingSeries.Count==0)
                return;
                    
            Study study = DeletingSeries[0].Study;

            string serverPartitionAE = DeletingSeries[0].ServerPartitionAE;

            if (Page.IsValid)
            {
                try
                {
                    var reason = ReasonListBox.SelectedItem.Text;

                    if (!String.IsNullOrEmpty(SaveReasonAsName.Text))
                    {
                        reason = SaveReasonAsName.Text;
                        SaveCustomReason();
                    }

                    OnDeletingSeries();
                    StudyController controller = new StudyController();

                    if(DeleteEntireStudy)
                    {
                        try
                        {
                            
                            controller.DeleteStudy(DeletingSeries[0].StudyKey, reason + ImageServerConstants.ReasonCommentSeparator[0] + Comment.Text);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex);
                            StringBuilder log = new StringBuilder();
                            log.AppendLine(string.Format("Unable to delete all series in study {0} on partition {1}", study.StudyInstanceUid, serverPartitionAE));
                            log.AppendLine(ex.Message);
                            Platform.Log(LogLevel.Error, log.ToString());

                            throw;
                        }

                    } 
                    else
                    {
                        try
                        {
                            IList<Series> series = new List<Series>();
                            foreach (DeleteSeriesInfo seriesInfo in DeletingSeries)
                            {
                                series.Add((seriesInfo.Series));
                            }
                            controller.DeleteSeries(DeletingSeries[0].Study, series, reason + ImageServerConstants.ReasonCommentSeparator[0] + Comment.Text);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex);
                            StringBuilder log = new StringBuilder();
                            log.AppendLine(string.Format("Unable to delete the following series in study {0} on partition {1}", study.StudyInstanceUid, serverPartitionAE));
                            foreach (var series in DeletingSeries)
                            {
                                log.AppendLine(string.Format("\tSeries #{0}  {1}  {2}", series.SeriesNumber, series.Description, series.Modality));
                            }

                            log.AppendLine(ex.Message);
                            Platform.Log(LogLevel.Error, log.ToString());
                            throw;
                        }
                    }

                    OnSeriesDeleted(study);
                }
                finally
                {
                    Close();
                }
            }
            else
            {
                EnsureDialogVisible();
            }
        }

        private void SaveCustomReason()
        {
            if (ReasonListBox.Items.FindByText(SaveReasonAsName.Text)!=null)
            {
                // update
                StudyDeleteReasonAdaptor adaptor = new StudyDeleteReasonAdaptor();
                CannedTextSelectCriteria criteria = new CannedTextSelectCriteria();
                criteria.Label.EqualTo(SaveReasonAsName.Text);
                criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                IList<CannedText> reasons = adaptor.Get(criteria);
                foreach(CannedText reason in reasons)
                {
                    CannedTextUpdateColumns rowColumns = new CannedTextUpdateColumns();
                    rowColumns.Text = Comment.Text;
                    adaptor.Update(reason.Key, rowColumns);
                }
                
            }
            else
            {
                // add 
                StudyDeleteReasonAdaptor adaptor = new StudyDeleteReasonAdaptor();
                CannedTextUpdateColumns rowColumns = new CannedTextUpdateColumns();
                rowColumns.Category = REASON_CANNEDTEXT_CATEGORY;
                rowColumns.Label = SaveReasonAsName.Text;
                rowColumns.Text = Comment.Text;
                adaptor.Add(rowColumns);
            }
            
        }

        protected void CancelButton_Clicked(object sender, ImageClickEventArgs e)
        {
            Close();
        }

        private void AuditLog(Study affectedStudy)
        {
            // Audit log
            DicomStudyDeletedAuditHelper helper =
                new DicomStudyDeletedAuditHelper(
                    ServerPlatform.AuditSource,
                    EventIdentificationContentsEventOutcomeIndicator.Success);
            helper.AddUserParticipant(new AuditPersonActiveParticipant(
                                          SessionManager.Current.Credentials.
                                              UserName,
                                          null,
                                          SessionManager.Current.Credentials.
                                              DisplayName));
            helper.AddStudyParticipantObject(new AuditStudyParticipantObject(
                                                 affectedStudy.StudyInstanceUid,
                                                 affectedStudy.AccessionNumber ??
                                                 string.Empty));
            ServerAuditHelper.LogAuditMessage(helper);
        }

        private void OnSeriesDeleted(Study affectedStudy)
        {
            AuditLog(affectedStudy);

            DeleteSeriesConfirmDialogSeriesDeletedEventArgs args = new DeleteSeriesConfirmDialogSeriesDeletedEventArgs();
            args.DeletedSeries = DeletingSeries;
            args.ReasonForDeletion = Comment.Text;
            EventsHelper.Fire(_seriesDeletedHandler, this, args);
        }

        private void OnDeletingSeries()
        {
            DeleteSeriesConfirmDialogSeriesDeletingEventArgs args = new DeleteSeriesConfirmDialogSeriesDeletingEventArgs();
            args.DeletingSeries = DeletingSeries;
            args.ReasonForDeletion = Comment.Text;
            EventsHelper.Fire(_seriesDeletingHandler, this, args);
        }

        internal void EnsureDialogVisible()
        {
            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        public void Initialize(List<DeleteSeriesInfo> list)
        {
            DeletingSeries = list;
        }

        internal void Show()
        {
            DataBind();
            ClearInputs(); 
            EnsureDialogVisible();
        }
    }
}