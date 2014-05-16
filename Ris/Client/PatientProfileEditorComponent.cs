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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class PatientProfileEditorComponent : NavigatorComponentContainer
	{
		private EntityRef _profileRef;
		private PatientProfileDetail _profile;
		private PatientProfileSummary _result;
		private readonly bool _isNew;
		private readonly List<AttachmentSummary> _newAttachments;

		private PatientProfileDetailsEditorComponent _patientEditor;
		private AddressesSummaryComponent _addressesSummary;
		private PhoneNumbersSummaryComponent _phoneNumbersSummary;
		private EmailAddressesSummaryComponent _emailAddressesSummary;
		private ContactPersonsSummaryComponent _contactPersonsSummary;
		private PatientProfileAdditionalInfoEditorComponent _additionalPatientInfoSummary;
		private PatientNoteSummaryComponent _notesSummary;
		private AttachedDocumentPreviewComponent _documentSummary;

		/// <summary>
		/// Constructs an editor to edit the specified profile
		/// </summary>
		/// <param name="profileRef"></param>
		public PatientProfileEditorComponent(EntityRef profileRef)
		{
			_profileRef = profileRef;
			_isNew = false;
			_newAttachments = new List<AttachmentSummary>();
		}

		/// <summary>
		/// Constructs an editor to edit a new profile
		/// </summary>
		public PatientProfileEditorComponent()
		{
			_isNew = true;
			_newAttachments = new List<AttachmentSummary>();
		}

		/// <summary>
		/// Constructs an editor to edit a new profile with attachments
		/// </summary>
		public PatientProfileEditorComponent(EntityRef profileRef, List<AttachmentSummary> attachments)
		{
			Platform.CheckForNullReference(attachments, "attachments");

			_profileRef = profileRef;
			_isNew = false;
			_newAttachments = attachments;
		}

		/// <summary>
		/// Constructs an editor to edit a new profile with attachments
		/// </summary>
		public PatientProfileEditorComponent(List<AttachmentSummary> attachments)
		{
			Platform.CheckForNullReference(attachments, "attachments");

			_isNew = true;
			_newAttachments = attachments;
		}

		public PatientProfileSummary PatientProfile
		{
			get { return _result; }
		}

		public override void Start()
		{
			Platform.GetService<IPatientAdminService>(
				service =>
				{
					var formData = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());
					if (_isNew)
					{
						_profile = new PatientProfileDetail();
						_profile.Mrn.AssigningAuthority = formData.MrnAssigningAuthorityChoices.Count > 0
							? GetWorkingFacilityInformationAuthority(formData.MrnAssigningAuthorityChoices)
							: null;
						_profile.Healthcard.AssigningAuthority = formData.HealthcardAssigningAuthorityChoices.Count > 0
							? formData.HealthcardAssigningAuthorityChoices[0]
							: null;
						_profile.Sex = formData.SexChoices[0];
						_profile.DateOfBirth = Platform.Time.Date;
					}
					else
					{
						var response = service.LoadPatientProfileForEdit(new LoadPatientProfileForEditRequest(_profileRef));

						_profileRef = response.PatientProfileRef;
						_profile = response.PatientDetail;

						this.Host.Title =
							string.Format(SR.TitlePatientComponent, PersonNameFormat.Format(_profile.Name), MrnFormat.Format(_profile.Mrn));
					}

					if (_newAttachments.Count > 0)
					{
						_profile.Attachments.AddRange(_newAttachments);
						this.Modified = true;
						this.AcceptEnabled = true;
					}

					// if the user has permission to either a) create a new patient, or b) update the patient profile, then 
					// these pages should be displayed
					if (Thread.CurrentPrincipal.IsInRole(
							ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientProfile.Update)
						|| Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Create))
					{
						this.Pages.Add(
							new NavigatorPage("NodePatient",
								_patientEditor =
								new PatientProfileDetailsEditorComponent(
									_isNew,
									formData.MrnAutoGenerated,
									formData.SexChoices,
									formData.MrnAssigningAuthorityChoices,
									formData.HealthcardAssigningAuthorityChoices)));
						this.Pages.Add(
							new NavigatorPage("NodePatient/NodeAddresses",
								_addressesSummary = new AddressesSummaryComponent(formData.AddressTypeChoices)));
						this.Pages.Add(
							new NavigatorPage("NodePatient/NodePhoneNumbers",
								_phoneNumbersSummary = new PhoneNumbersSummaryComponent(formData.PhoneTypeChoices)));
						this.Pages.Add(
							new NavigatorPage("NodePatient/NodeEmailAddresses",
								_emailAddressesSummary = new EmailAddressesSummaryComponent()));
						this.Pages.Add(
							new NavigatorPage("NodePatient/NodeContactPersons",
								_contactPersonsSummary =
								new ContactPersonsSummaryComponent(formData.ContactPersonTypeChoices, formData.ContactPersonRelationshipChoices)));
						this.Pages.Add(
							new NavigatorPage("NodePatient/NodeCulture",
								_additionalPatientInfoSummary =
								new PatientProfileAdditionalInfoEditorComponent(formData.ReligionChoices, formData.PrimaryLanguageChoices)));

						_addressesSummary.SetModifiedOnListChange = true;
						_phoneNumbersSummary.SetModifiedOnListChange = true;
						_emailAddressesSummary.SetModifiedOnListChange = true;
						_contactPersonsSummary.SetModifiedOnListChange = true;

						_patientEditor.Subject = _profile;
						_addressesSummary.Subject = _profile.Addresses;
						_phoneNumbersSummary.Subject = _profile.TelephoneNumbers;
						_emailAddressesSummary.Subject = _profile.EmailAddresses;
						_contactPersonsSummary.Subject = _profile.ContactPersons;
						_additionalPatientInfoSummary.Subject = _profile;
					}

					// if the user has permission to either a) create a new patient, or b) update a patient, then
					// these pages should be displayed
					if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Create)
						|| Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Update))
					{
						this.Pages.Add(
							new NavigatorPage("NodePatient/NodeNotes",
								_notesSummary = new PatientNoteSummaryComponent(_profile.Notes, formData.NoteCategoryChoices)));
						_notesSummary.SetModifiedOnListChange = true;

						var patientDocumentsPage = new NavigatorPage("NodePatient/NodeAttachments",
							_documentSummary =
							new AttachedDocumentPreviewComponent(false, AttachmentSite.Patient));
						this.Pages.Add(patientDocumentsPage);
						_documentSummary.Attachments = _profile.Attachments;

						if (_newAttachments.Count > 0)
						{
							this.MoveTo(this.Pages.IndexOf(patientDocumentsPage));
							_documentSummary.SetInitialSelection(_newAttachments[0]);
						}
					}

					this.ValidationStrategy = new AllComponentsValidationStrategy();
				});


			base.Start();
		}

		public override void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
				try
				{
					SaveChanges();
					this.Exit(ApplicationComponentExitCode.Accepted);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow,
											delegate
											{
												this.ExitCode = ApplicationComponentExitCode.Error;
												this.Host.Exit();
											});
				}
			}
		}

		private void SaveChanges()
		{
			SynchronizeAttachedDocumentChanges();

			Platform.GetService<IPatientAdminService>(service =>
				{
					if (_isNew)
					{
						var response = service.AddPatient(new AddPatientRequest(_profile));
						_result = response.PatientProfile;
					}
					else
					{
						var response = service.UpdatePatientProfile(new UpdatePatientProfileRequest(_profileRef, _profile));
						_result = response.PatientProfile;
					}
				});
		}

		private void SynchronizeAttachedDocumentChanges()
		{
			if (_documentSummary == null)
				return;

			// resynchronize the profile's documents if the document summary was created
			// (the document summary does not use the same object for its list of attachments,
			// so changes must be manually synchronized.
			_profile.Attachments.Clear();
			_profile.Attachments.AddRange(_documentSummary.Attachments);
		}

		private static EnumValueInfo GetWorkingFacilityInformationAuthority(IEnumerable<EnumValueInfo> assigningAuthorityChoices)
		{
			return LoginSession.Current.WorkingFacility == null ?
				assigningAuthorityChoices.FirstOrDefault()
				: assigningAuthorityChoices.FirstOrDefault(ia => ia.Code == LoginSession.Current.WorkingFacility.InformationAuthority.Code);
		}

	}
}