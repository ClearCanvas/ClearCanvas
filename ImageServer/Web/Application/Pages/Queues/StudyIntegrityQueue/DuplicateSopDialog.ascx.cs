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
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using Resources;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    //
    // Dialog for handling duplicate sop.
    //
    public partial class DuplicateSopDialog : UserControl
    {
        private const string HighlightCssClass = " ConflictField ";

        protected void OKButton_Click(object sender, ImageClickEventArgs e)
        {
            try
            {            
                var itemKey = ViewState["QueueItem"] as ServerEntityKey;
                var controller = new DuplicateSopEntryController();
                ProcessDuplicateAction action = ProcessDuplicateAction.OverwriteAsIs;
                if (UseExistingSopRadioButton.Checked)
                    action = ProcessDuplicateAction.OverwriteUseExisting;
                else if (UseDuplicateRadioButton.Checked)
                    action = ProcessDuplicateAction.OverwriteUseDuplicates;
                else if (DeleteDuplicateRadioButton.Checked)
                    action = ProcessDuplicateAction.Delete;
                else if (ReplaceAsIsRadioButton.Checked)
                    action = ProcessDuplicateAction.OverwriteAsIs;

                controller.Process(itemKey, action);
            }
            catch (Exception ex)
            {
                MessageBox.Message = String.Format(ErrorMessages.ActionNotAllowedAtThisTime, ex.Message);
                MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Show();
            }

            //((Default) Page).UpdateUI();
            Close();
        }

        #region Nested type: ComparisonCallback

        private delegate void ComparisonCallback(bool different);

        #endregion

        #region private variables

        private bool _consistentData;
        private Model.StudyIntegrityQueue _item;

        #endregion

        #region public members

        /// <summary>
        /// Sets or gets the StudyIntegrity Item Value
        /// </summary>
        public Model.StudyIntegrityQueue StudyIntegrityQueueItem
        {
            get { return _item; }
            set
            {
                _item = value;
                ViewState["QueueItem"] = _item.GetKey();
            }
        }

        /// <summary>
        /// Sets or gets the Reconcile Item Value
        /// </summary>
        public DuplicateEntryDetails DuplicateEntryDetails { get; set; }

        public bool DataIsConsistent
        {
            get { return _consistentData; }
        }

        #endregion // public members

        #region Events

        #endregion Events

        #region Public delegates

        #endregion // public delegates

        #region Protected methods

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Protected methods

        #region Public methods

        /// <summary>
        /// Displays the add/edit device dialog box.
        /// </summary>
        public void Show()
        {
            DataBind();
            _consistentData = true;
            HighlightDifferences();
            Page.Validate();
            CheckDataForPossibleTruncation();
            DuplicateSopReconcileModalDialog.Show();
        }

        public override void DataBind()
        {
            ExistingPatientSeriesGridView.DataSource = DuplicateEntryDetails.ExistingStudy.Series;
            ConflictingPatientSeriesGridView.DataSource = DuplicateEntryDetails.ConflictingImageSet.StudyInfo.Series;
            StudyStorage storage =
                StudyStorage.Load(HttpContext.Current.GetSharedPersistentContext(), StudyIntegrityQueueItem.StudyStorageKey);

            IList<StudyStorageLocation> studyLocations = StudyStorageLocation.FindStorageLocations(storage);
            StudyLocation.Text = studyLocations[0].GetStudyPath();

            var entry = new DuplicateSopReceivedQueue(StudyIntegrityQueueItem);

			DuplicateSopLocation.Text = entry.GetFolderPath(HttpContext.Current.GetSharedPersistentContext());

            ComparisonResultGridView.DataSource = DuplicateEntryDetails.QueueData.ComparisonResults;
            base.DataBind();
        }

        private void CheckDataForPossibleTruncation()
        {
            List<uint> tagsMustBeTruncated = DataLengthValidation.CheckDataLength(DuplicateEntryDetails.ConflictingStudyInfo);
            FieldsMayTruncate.Value = tagsMustBeTruncated.Count > 0 ? "true" : "false";
        }

        private void HighlightDifferences()
        {
            if (DuplicateEntryDetails != null)
            {
                Compare(DuplicateEntryDetails.ExistingStudy.Patient.Name,
                        DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name,
                        different => Highlight(ConflictingNameLabel, different));

                Compare(DuplicateEntryDetails.ExistingStudy.Patient.PatientID,
                        DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientId,
                        different => Highlight(ConflictingPatientIDLabel, different));
                Compare(DuplicateEntryDetails.ExistingStudy.Patient.IssuerOfPatientID,
                        DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId,
                        different => Highlight(ConflictingPatientIssuerOfPatientID, different));
                Compare(DuplicateEntryDetails.ExistingStudy.Patient.BirthDate,
                        DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate,
                        different => Highlight(ConflictingPatientBirthDate, different));
                Compare(DuplicateEntryDetails.ExistingStudy.Patient.Sex,
                        DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex,
                        different => Highlight(ConflictingPatientSex, different));
                Compare(DuplicateEntryDetails.ExistingStudy.StudyDate,
                        DuplicateEntryDetails.ConflictingImageSet.StudyInfo.StudyDate,
                        different => Highlight(ConflictingStudyDate, different));
                Compare(DuplicateEntryDetails.ExistingStudy.AccessionNumber,
                        DuplicateEntryDetails.ConflictingImageSet.StudyInfo.AccessionNumber,
                        different => Highlight(ConflictingAccessionNumberLabel, different));

                UnknownSexWarning.Visible = !DicomValueValidator.IsValidDicomPatientSex(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex);
            }
        }

        private void Highlight(WebControl control, bool highlight)
        {
            if (highlight)
            {
                _consistentData = false;
                HtmlUtility.AddCssClass(control, HighlightCssClass);
            }
            else
                HtmlUtility.RemoveCssClass(control, HighlightCssClass);
        }

        private static void Compare(string value1, string value2, ComparisonCallback del)
        {
            if (!StringUtils.AreEqual(value1, value2, StringComparison.InvariantCultureIgnoreCase))
                del(true);
            else
                del(false);
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            TabContainer.ActiveTabIndex = 0;
            DuplicateSopReconcileModalDialog.Hide();
        }

        #endregion Public methods
    }
}