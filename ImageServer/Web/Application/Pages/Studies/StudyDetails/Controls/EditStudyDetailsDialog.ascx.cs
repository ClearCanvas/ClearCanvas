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
using System.Web;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Authentication;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Security;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;
using SR = Resources.SR;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{


    public partial class EditStudyDetailsDialog : System.Web.UI.UserControl
    {
        #region Private Members

        private const string REASON_CANNEDTEXT_CATEGORY = "EditStudyReason";

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets/Gets the current editing device.
        /// </summary>
        public Study Study
        {
            set
            {
                // put into viewstate to retrieve later
                ViewState["loadedStudy"] = value;
            }
            get
            { 
                // put into viewstate to retrieve later
                return ViewState["loadedStudy"] as Study;
            }
        }

        #endregion
 
        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="EditStudyDetailsDialog.StudyEdited"/>.
        /// </summary>
        public delegate void OnOKClickedEventHandler();

        /// <summary>
        /// Occurs when users click on "OK".
        /// </summary>
        public event OnOKClickedEventHandler StudyEdited;

        #endregion Events
        
        #region Private Methods

        private void SetupJavascript()
        {
            ClearStudyDateTimeButton.OnClientClick = "document.getElementById('" + StudyDate.ClientID + "').value='';" +
                                         "document.getElementById('" + StudyTimeHours.ClientID + "').value='';" +
                                         "document.getElementById('" + StudyTimeMinutes.ClientID + "').value='';" +
                                         "document.getElementById('" + StudyTimeSeconds.ClientID + "').value='';" +
                                         " return false;";

            ClearPatientBirthDateButton.OnClientClick = "document.getElementById('" + PatientBirthDate.ClientID +
                                                        "').value=''; return false;";

        }

        private static bool AreDifferent(String v1, String v2)
        {
            if (v1 == null)
                return String.IsNullOrEmpty(v2) == false;

            //v1 is not null
            return !v1.Equals(v2, StringComparison.InvariantCulture);
        }

        private List<UpdateItem> GetChanges()
        {
            var changes = new List<UpdateItem>();
            var oldPatientName = new PersonName(Study.PatientsName);
            var newPatientName = PatientNamePanel.PersonName;

            if (!oldPatientName.AreSame(newPatientName, PersonNameComparisonOptions.CaseSensitive))
            {
                var item = new UpdateItem(DicomTags.PatientsName, Study.PatientsName, PatientNamePanel.PersonName);
                changes.Add(item);
            }

	        String dicomBirthDate = string.IsNullOrEmpty(PatientBirthDate.Text)
		        ? ""
		        : DateTime.Parse(PatientBirthDate.Text).ToString(DicomConstants.DicomDate, CultureInfo.InvariantCulture);

            if (AreDifferent(Study.PatientsBirthDate, dicomBirthDate))
            {
                var item = new UpdateItem(DicomTags.PatientsBirthDate, Study.PatientsBirthDate, dicomBirthDate);
                changes.Add(item);
            }

            string newPatientAge = String.IsNullOrEmpty(PatientAge.Text)? String.Empty:String.Format("{0}{1}", PatientAge.Text.PadLeft(3, '0'), PatientAgePeriod.SelectedValue);

            if (AreDifferent(Study.PatientsAge, newPatientAge))
            {
                var item = new UpdateItem(DicomTags.PatientsAge, Study.PatientsAge, newPatientAge);
                changes.Add(item);
            }

            // PatientGender is a required field.
            if (AreDifferent(Study.PatientsSex, PatientGender.Text))
            {
                var item = new UpdateItem(DicomTags.PatientsSex, Study.PatientsSex, PatientGender.Text);
                changes.Add(item);
            }

            //PatientID.Text is a required field.
            if (AreDifferent(Study.PatientId, PatientID.Text))
            {
                var item = new UpdateItem(DicomTags.PatientId, Study.PatientId, PatientID.Text);
                changes.Add(item);
            }

            if (AreDifferent(Study.StudyDescription, StudyDescription.Text))
            {
                var item = new UpdateItem(DicomTags.StudyDescription, Study.StudyDescription, StudyDescription.Text);
                changes.Add(item);
            }

            if (AreDifferent(Study.StudyId, StudyID.Text))
            {
                var item = new UpdateItem(DicomTags.StudyId, Study.StudyId, StudyID.Text);
                changes.Add(item);
            }

            if (AreDifferent(Study.AccessionNumber, AccessionNumber.Text))
            {
                var item = new UpdateItem(DicomTags.AccessionNumber, Study.AccessionNumber, AccessionNumber.Text);
                changes.Add(item);
            }

            var oldPhysicianName = new PersonName(Study.ReferringPhysiciansName);
            var newPhysicianName = ReferringPhysicianNamePanel.PersonName;

            if (!newPhysicianName.AreSame(oldPhysicianName, PersonNameComparisonOptions.CaseSensitive))
            {
                var item = new UpdateItem(DicomTags.ReferringPhysiciansName, Study.ReferringPhysiciansName, ReferringPhysicianNamePanel.PersonName.ToString());
                changes.Add(item);
            }

            string newDicomStudyDate=string.Empty;
            if (!string.IsNullOrEmpty(StudyDate.Text))
            {
                DateTime newStudyDate;
                newDicomStudyDate = InputDateParser.TryParse(StudyDate.Text, out newStudyDate)
                                        ? newStudyDate.ToString(DicomConstants.DicomDate, CultureInfo.InvariantCulture) /* to ISO yyyyMMdd */
                                        : string.Empty;
            }

            if (AreDifferent(Study.StudyDate, newDicomStudyDate))
            {
                var item = new UpdateItem(DicomTags.StudyDate, Study.StudyDate, newDicomStudyDate);
                changes.Add(item);
            }

            int hh = String.IsNullOrEmpty(StudyTimeHours.Text)? 0:int.Parse(StudyTimeHours.Text);
            int mm = String.IsNullOrEmpty(StudyTimeMinutes.Text) ? 0 : int.Parse(StudyTimeMinutes.Text);
            int ss = String.IsNullOrEmpty(StudyTimeSeconds.Text) ? 0 : int.Parse(StudyTimeSeconds.Text);
            String dicomStudyTime = String.Format("{0:00}{1:00}{2:00}", hh, mm, ss);

            // #9475 : if fraction is in the original time, it should be preserved unless the hours, minutes or seconds are modified.
            var originalTime = Study.StudyTime;
            if (!string.IsNullOrEmpty(originalTime) && originalTime.Contains("."))
            {
                originalTime = originalTime.Substring(0, originalTime.IndexOf(".", StringComparison.InvariantCultureIgnoreCase));
            }

            if (AreDifferent(originalTime, dicomStudyTime))
            {
                var item = new UpdateItem(DicomTags.StudyTime, Study.StudyTime, dicomStudyTime);
                changes.Add(item);
            }

            return changes;
        }

        private void UpdateFields()
        {
            if(Study == null) return;

            var patientName = new PersonName(Study.PatientsName);
            var physicianName = new PersonName(Study.ReferringPhysiciansName);
            PatientNamePanel.PersonName = patientName;
            ReferringPhysicianNamePanel.PersonName = physicianName;
            
            // Patient Information
            if (!string.IsNullOrEmpty(Study.PatientsSex))
            {
                switch(Study.PatientsSex)
                {
                    case "M":
                        PatientGender.SelectedIndex = 1;
                        break;
                    case "F":
                        PatientGender.SelectedIndex = 2;
                        break;
                    case "O":
                        PatientGender.SelectedIndex = 3;
                        break;
                    default:
                        PatientGender.SelectedIndex = 0;
                        break;
                }
            } else
            {
                PatientGender.SelectedIndex = 0;
            }

            PatientID.Text = Study.PatientId;
            DateTime? originalBirthDate = String.IsNullOrEmpty(Study.PatientsBirthDate)? (DateTime?) null:DateParser.Parse(Study.PatientsBirthDate);
	        PatientBirthDateCalendarExtender.SelectedDate = originalBirthDate;

            if (!String.IsNullOrEmpty(Study.PatientsAge))
            {
                PatientAge.Text = Study.PatientsAge.Substring(0, 3).TrimStart('0');
                switch (Study.PatientsAge.Substring(3))
                {
                    case "Y":
                        PatientAgePeriod.SelectedIndex = 0;
                        break;
                    case "M":
                        PatientAgePeriod.SelectedIndex = 1;
                        break;
                    case "W":
                        PatientAgePeriod.SelectedIndex = 2;
                        break;
                    default:
                        PatientAgePeriod.SelectedIndex = 3;
                        break;
                }
            }
            else
            {
                PatientAge.Text = string.Empty;
                PatientAgePeriod.SelectedIndex = 0;
            }

            // Study Information
            StudyDescription.Text = Study.StudyDescription;            
            StudyID.Text = Study.StudyId;
            AccessionNumber.Text = Study.AccessionNumber;

            if (!string.IsNullOrEmpty(Study.StudyDate))
            {
                DateTime? studyDate = DateParser.Parse(Study.StudyDate);
                StudyDateCalendarExtender.SelectedDate = studyDate;
            }
            else
            {
                StudyDateCalendarExtender.SelectedDate = null;
            }
            

            if (!string.IsNullOrEmpty(Study.StudyTime))
            {
                DateTime? studyTime = TimeParser.Parse(Study.StudyTime);
                if (studyTime!=null)
                {
                    StudyTimeHours.Text = String.Format("{0:00}",studyTime.Value.Hour);

                    StudyTimeMinutes.Text = String.Format("{0:00}", studyTime.Value.Minute);
                    StudyTimeSeconds.Text = String.Format("{0:00}", studyTime.Value.Second);
                }
                else
                {
                    // The time is invalid, display it in the boxes
                    StudyTimeHours.Text = "";
                    StudyTimeMinutes.Text = "";
                    StudyTimeSeconds.Text = "";
                }

            }
            else
            {
                StudyTimeHours.Text = "00";
                StudyTimeMinutes.Text = "00";
                StudyTimeSeconds.Text = "00";
            }

            ReasonListBox.SelectedIndex = 0;
            if(string.IsNullOrEmpty(ReasonListBox.SelectedValue))
            {
                Comment.Text = string.Empty;
            } else
            {
                Comment.Text = SR.CustomReasonComment;
            }
            SaveReasonAsName.Text = string.Empty;

            AttachmentExistWarning.Visible = this.Study.HasAttachment;
            
            DataBind();
        }

        private void EnsurePredefinedReasonsLoaded()
        {
            ReasonListBox.Items.Clear();

			var broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<ICannedTextEntityBroker>();
            var criteria = new CannedTextSelectCriteria();
            criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
            IList<CannedText> list = broker.Find(criteria);

            if (SessionManager.Current.User.IsInRole(AuthorityTokens.Study.SaveReason))
            {
                ReasonListBox.Items.Add(new ListItem(SR.CustomReason, SR.CustomReasonComment));
            } else
            {
                ReasonListBox.Items.Add(new ListItem(SR.SelectOne, string.Empty));    
            }

            foreach (CannedText text in list)
            {
                ReasonListBox.Items.Add(new ListItem(text.Label, text.Text));
            }

        }

        private static void AuditLog(Study study, List<UpdateItem> fields)
        {
            Platform.CheckForNullReference(study, "study");
            Platform.CheckForNullReference(fields, "fields");

            var helper =
                new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                                                      EventIdentificationContentsEventOutcomeIndicator.Success,
                                                      EventIdentificationContentsEventActionCode.U);
            helper.AddUser(new AuditPersonActiveParticipant(
                               SessionManager.Current.Credentials.UserName,
                               null,
                               SessionManager.Current.Credentials.DisplayName));

            var participant = new AuditStudyParticipantObject(study.StudyInstanceUid, study.AccessionNumber);

            string updateDescription = StringUtilities.Combine(
                fields, ";",
                item => String.Format("Tag=\"{0}\" Value=\"{1}\"", item.DicomTag.Name, item.Value)
                );

            participant.ParticipantObjectDetailString = updateDescription;
            helper.AddStudyParticipantObject(participant);
            ServerAuditHelper.LogAuditMessage(helper);
        }

        private void SaveCustomReason()
        {
            if (ReasonListBox.Items.FindByText(SaveReasonAsName.Text) != null)
            {
                // update
                var adaptor = new StudyEditReasonAdaptor();
                var criteria = new CannedTextSelectCriteria();
                criteria.Label.EqualTo(SaveReasonAsName.Text);
                criteria.Category.EqualTo(REASON_CANNEDTEXT_CATEGORY);
                IList<CannedText> reasons = adaptor.Get(criteria);
                foreach (CannedText reason in reasons)
                {
                    var rowColumns = new CannedTextUpdateColumns {Text = Comment.Text};
                    adaptor.Update(reason.Key, rowColumns);
                }

            }
            else
            {
                // add 
                var adaptor = new StudyDeleteReasonAdaptor();
                var rowColumns = new CannedTextUpdateColumns
                                     {
                                         Category = REASON_CANNEDTEXT_CATEGORY,
                                         Label = SaveReasonAsName.Text,
                                         Text = Comment.Text
                                     };
                adaptor.Add(rowColumns);
            }

        }

        #endregion

        #region Protected Methods
       
        protected override void OnInit(EventArgs e)
        {
            SetupJavascript();
            EditStudyDetailsValidationSummary.HeaderText = ErrorMessages.EditStudyValidationError;
            EnsurePredefinedReasonsLoaded();

			PatientBirthDate.Attributes.Add("readonly", "readonly");

            //Set up the control to handle custom reasons if the user has the authority.
            if (!SessionManager.Current.User.IsInRole(AuthorityTokens.Study.SaveReason))
            {
                ReasonSavePanel.Visible = false;
                SaveReasonAsName.Attributes.Add("display", "none");
                SaveReasonAsNameValidator.Enabled = false;
            } else
            {
                //Hide/Disable the "Save As Reason" textbox/validation depending on whether the user is using a custom reason or not.
                ReasonListBox.Attributes.Add("onchange", "if(document.getElementById('" + ReasonListBox.ClientID + "').options[document.getElementById('" + ReasonListBox.ClientID + "').selectedIndex].text != '" + SR.CustomReason + "') { document.getElementById('" + ReasonSavePanel.ClientID + "').style.display = 'none'; document.getElementById('" + SaveReasonAsName.ClientID + "').style.display = 'none'; } else { document.getElementById('" + ReasonSavePanel.ClientID + "').style.display = 'table-row'; document.getElementById('" + SaveReasonAsName.ClientID + "').style.display = 'table-cell'; }");
                ReasonListBox.TextChanged += delegate
                {
                    if (ReasonListBox.SelectedItem.Text == SR.CustomReason) SaveReasonAsNameValidator.Enabled = true;
                    else SaveReasonAsNameValidator.Enabled = false;
                };
            }

            string pattern = InputDateParser.DateFormat;
            StudyDateCalendarExtender.Format = pattern;

            var d = new DateTime(2008, 05, 27);
            DateExampleLabel.Text = string.Format("({0})", string.Format(SR.Example, d.ToString(pattern, CultureInfo.InstalledUICulture)));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Ensure the study date entered if invalid will not be erased by the calendar extender
            // Note: this code is actually needed for Firefox because the server validation is used.
            // For IE, because of client-side validation, all input is already valid on postback.
            if (!String.IsNullOrEmpty(StudyDate.Text))
            {
                DateTime result;
                if (InputDateParser.TryParse(StudyDate.Text, out result))
                {
                    // entered value is actually valid... update the calendar
                    StudyDateCalendarExtender.SelectedDate = result;
                }
                else
                {
                    StudyDateCalendarExtender.SelectedDate = null;
                }
            }
            else
            {
                // Prevents the calendar from copying its value into the textbox
                StudyDateCalendarExtender.SelectedDate = null;
            }

        }

        /// <summary>
        /// Handles event when user clicks on "OK" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string reason = ReasonListBox.SelectedItem.Text;

                if (!String.IsNullOrEmpty(SaveReasonAsName.Text))
                {
                    SaveCustomReason();
                    reason = SaveReasonAsName.Text;
                }
                
                if (StudyEdited != null)
                {
                    List<UpdateItem> modifiedFields = GetChanges();
                    if (modifiedFields!=null && modifiedFields.Count > 0)
                    {
                        var studyController = new StudyController();
                        studyController.EditStudy(Study, modifiedFields, reason + ImageServerConstants.ReasonCommentSeparator[0] + Comment.Text);
                        AuditLog(Study, modifiedFields);
                        StudyEdited();
                    }
                }

                Close();
            }
            else
            {
                EnsureDialogVisible();
            }
        }

        internal void EnsureDialogVisible()
        {
            EditStudyModalDialog.Show();
        }

        /// <summary>
        /// Handles event when user clicks on "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion 

        #region Public Methods

        /// <summary>
        /// Displays the edit Study Details dialog box.
        /// </summary>
        public void Show(bool updateFields)
        {            
            if(updateFields) UpdateFields();
            EditStudyModalDialog.Show();
        }

        /// <summary>
        /// Dismisses the dialog box.
        /// </summary>
        public void Close()
        {
            EditStudyModalDialog.Hide();
        }

        #endregion
    }
}