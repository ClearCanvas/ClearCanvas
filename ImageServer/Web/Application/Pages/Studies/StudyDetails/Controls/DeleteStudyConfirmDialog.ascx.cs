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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Security;
using AuthorityTokens = ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens;
using SR = Resources.SR;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [Serializable]
    public class DeleteStudyInfo
    {
        private ServerEntityKey _studyKey;
        private string _studyInstanceUid;
        private string _accessionNumber;
        private string _patientId;
        private string _patientsName;
        private string _studyDescription;
        private string _modalities;
        private string _studyDate;
        private string _serverPartitionAE;

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }

        public string PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

        public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

        public string Modalities
        {
            get { return _modalities; }
            set { _modalities = value; }
        }

        public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
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

    public class DeleteStudyConfirmDialogStudyDeletingEventArgs : EventArgs
    {
        private IList<DeleteStudyInfo> _deletingStudies;
        private string _reasonForDeletion;

        public IList<DeleteStudyInfo> DeletingStudies
        {
            get { return _deletingStudies; }
            set { _deletingStudies = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public class DeleteStudyConfirmDialogStudyDeletedEventArgs:EventArgs
    {
        private IList<DeleteStudyInfo> _deletedStudies;
        private string _reasonForDeletion;

        public IList<DeleteStudyInfo> DeletedStudies
        {
            get { return _deletedStudies; }
            set { _deletedStudies = value; }
        }

        public string ReasonForDeletion
        {
            get { return _reasonForDeletion; }
            set { _reasonForDeletion = value; }
        }
    }

    public partial class DeleteStudyConfirmDialog : UserControl
    {
        private const string REASON_CANNEDTEXT_CATEGORY = "DeleteStudyReason";
        private EventHandler<DeleteStudyConfirmDialogStudyDeletingEventArgs> _studyDeletingHandler;
        private EventHandler<DeleteStudyConfirmDialogStudyDeletedEventArgs> _studyDeletedHandler;

        public event EventHandler<DeleteStudyConfirmDialogStudyDeletingEventArgs> StudyDeleting
        {
            add { _studyDeletingHandler += value; }
            remove { _studyDeletingHandler -= value; }
        }
        
        public event EventHandler<DeleteStudyConfirmDialogStudyDeletedEventArgs> StudyDeleted
        {
            add { _studyDeletedHandler += value; }
            remove { _studyDeletedHandler -= value; }
        }
        
        public IList<DeleteStudyInfo> DeletingStudies
        {
            get
            {
                return ViewState["DeletedStudies"] as IList<DeleteStudyInfo>;
            }
            set { ViewState["DeletedStudies"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
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
                ReasonListBox.Attributes.Add("onchange", "if(document.getElementById('" + ReasonListBox.ClientID + "').options[document.getElementById('" + ReasonListBox.ClientID + "').selectedIndex].text != '" + SR.CustomReason + "') { document.getElementById('" + ReasonSavePanel.ClientID + "').style.display = 'none'; document.getElementById('" + SaveReasonAsName.ClientID + "').style.display = 'none'; } else { document.getElementById('" + ReasonSavePanel.ClientID + "').style.display = 'table-row'; document.getElementById('" + SaveReasonAsName.ClientID + "').style.display = 'inline'; }");
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
            }
            else
            {
                Comment.Text = string.Empty;
            }
        }

        public override void DataBind()
        {
            StudyListing.DataSource = DeletingStudies;

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

        protected void DeleteButton_Clicked(object sender, ImageClickEventArgs e)
        {

            if (Page.IsValid)
            {
                try
                {
                    string reason = ReasonListBox.SelectedItem.Text;
                    if (!String.IsNullOrEmpty(SaveReasonAsName.Text))
                    {
                        SaveCustomReason();
                        reason = SaveReasonAsName.Text;
                    }

                    OnDeletingStudies();
                    StudyController controller = new StudyController();
                    foreach (DeleteStudyInfo study in DeletingStudies)
                    {
                        try
                        {
                            controller.DeleteStudy(study.StudyKey, reason + "::" + Comment.Text);

							// Audit log
                        	DicomStudyDeletedAuditHelper helper = new DicomStudyDeletedAuditHelper(
                        										ServerPlatform.AuditSource, 
																EventIdentificationContentsEventOutcomeIndicator.Success);
							helper.AddUserParticipant(new AuditPersonActiveParticipant(
																SessionManager.Current.Credentials.UserName, 
																null, 
																SessionManager.Current.Credentials.DisplayName));
                        	helper.AddStudyParticipantObject(new AuditStudyParticipantObject(
																	study.StudyInstanceUid, 
																	study.AccessionNumber ?? string.Empty));
                            ServerAuditHelper.LogAuditMessage(helper);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex, "DeleteClicked failed: Unable to delete studies");
                            throw;
                        }
                    }

                    OnStudiesDeleted();
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

        private void OnStudiesDeleted()
        {
            DeleteStudyConfirmDialogStudyDeletedEventArgs args = new DeleteStudyConfirmDialogStudyDeletedEventArgs();
            args.DeletedStudies = DeletingStudies;
            args.ReasonForDeletion = Comment.Text;
            EventsHelper.Fire(_studyDeletedHandler, this, args);
        }

        private void OnDeletingStudies()
        {
            DeleteStudyConfirmDialogStudyDeletingEventArgs args = new DeleteStudyConfirmDialogStudyDeletingEventArgs();
            args.DeletingStudies = DeletingStudies;
            args.ReasonForDeletion = Comment.Text;
            EventsHelper.Fire(_studyDeletingHandler, this, args);
        }

        internal void EnsureDialogVisible()
        {
            ModalDialog.Show();
        }

        public void Close()
        {
            ModalDialog.Hide();
        }

        public void Initialize(List<DeleteStudyInfo> list)
        {
            DeletingStudies = list;
        }

        internal void Show()
        {
            DataBind();
            ClearInputs(); 
            EnsureDialogVisible();
        }
    }
}