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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the staff editor.
	/// </summary>
	public interface IStaffEditorPageProvider : IExtensionPageProvider<IStaffEditorPage, IStaffEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IStaffEditorContext
	{
		EntityRef StaffRef { get; }

		IDictionary<string, string> StaffExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an interface to a custom staff editor page.
	/// </summary>
	public interface IStaffEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the staff editor.
	/// </summary>
	public class StaffEditorPageProviderExtensionPoint : ExtensionPoint<IStaffEditorPageProvider>
	{
	}



	/// <summary>
	/// Allows editing of staff person information.
	/// </summary>
	public class StaffEditorComponent : NavigatorComponentContainer
	{
		#region StaffEditorContext

		class EditorContext : IStaffEditorContext
		{
			private readonly StaffEditorComponent _owner;

			public EditorContext(StaffEditorComponent owner)
			{
				_owner = owner;
			}

			public EntityRef StaffRef
			{
				get { return _owner._staffRef; }
			}

			public IDictionary<string, string> StaffExtendedProperties
			{
				get { return _owner._staffDetail.ExtendedProperties; }
			}
		}

		#endregion


		private EntityRef _staffRef;
		private StaffDetail _staffDetail;

		private string _originalStaffUserName;

		// return values for staff
		private StaffSummary _staffSummary;

		private readonly bool _isNew;

		private EmailAddressesSummaryComponent _emailAddressesSummary;
		private PhoneNumbersSummaryComponent _phoneNumbersSummary;
		private AddressesSummaryComponent _addressesSummary;

		private StaffDetailsEditorComponent _detailsEditor;
		private StaffStaffGroupEditorComponent _nonElectiveGroupsEditor;
		private StaffStaffGroupEditorComponent _electiveGroupsEditor;

		private List<IStaffEditorPage> _extensionPages;

		/// <summary>
		/// Constructs an editor to edit a new staff
		/// </summary>
		public StaffEditorComponent()
		{
			_isNew = true;
		}

		/// <summary>
		/// Constructs an editor to edit an existing staff profile
		/// </summary>
		/// <param name="reference"></param>
		public StaffEditorComponent(EntityRef reference)
		{
			_isNew = false;
			_staffRef = reference;
		}

		/// <summary>
		/// Gets summary of staff that was added or edited
		/// </summary>
		public StaffSummary StaffSummary
		{
			get { return _staffSummary; }
		}

		public override void Start()
		{
			Platform.GetService<IStaffAdminService>(
				delegate(IStaffAdminService service)
				{
					LoadStaffEditorFormDataResponse formDataResponse = service.LoadStaffEditorFormData(new LoadStaffEditorFormDataRequest());

					this.ValidationStrategy = new AllComponentsValidationStrategy();

					if (_isNew)
					{
						_staffDetail = new StaffDetail();
						_staffDetail.StaffType = CollectionUtils.FirstElement(formDataResponse.StaffTypeChoices);
					}
					else
					{
						LoadStaffForEditResponse response = service.LoadStaffForEdit(new LoadStaffForEditRequest(_staffRef));
						_staffRef = response.StaffDetail.StaffRef;
						_staffDetail = response.StaffDetail;
					}

					_originalStaffUserName = _staffDetail.UserName;

					_detailsEditor = new StaffDetailsEditorComponent(formDataResponse.StaffTypeChoices, formDataResponse.SexChoices)
					                 	{StaffDetail = _staffDetail};
					this.Pages.Add(new NavigatorPage("NodeStaff", _detailsEditor));

					_phoneNumbersSummary = new PhoneNumbersSummaryComponent(formDataResponse.PhoneTypeChoices)
					                       	{
					                       		ReadOnly = !CanModifyStaffProfile,
					                       		SetModifiedOnListChange = true,
					                       		Subject = _staffDetail.TelephoneNumbers
					                       	};
					this.Pages.Add(new NavigatorPage("NodeStaff/NodePhoneNumbers", _phoneNumbersSummary));

					_addressesSummary = new AddressesSummaryComponent(formDataResponse.AddressTypeChoices)
					                    	{
					                    		ReadOnly = !CanModifyStaffProfile,
					                    		SetModifiedOnListChange = true,
					                    		Subject = _staffDetail.Addresses
					                    	};
					this.Pages.Add(new NavigatorPage("NodeStaff/NodeAddresses", _addressesSummary));

					_emailAddressesSummary = new EmailAddressesSummaryComponent
					                         	{
					                         		ReadOnly = !CanModifyStaffProfile,
					                         		SetModifiedOnListChange = true,
					                         		Subject = _staffDetail.EmailAddresses
					                         	};
					this.Pages.Add(new NavigatorPage("NodeStaff/NodeEmailAddresses", _emailAddressesSummary));



					// allow modification of non-elective groups only iff the user has StaffGroup admin permissions
					this.Pages.Add(new NavigatorPage("NodeStaff/NodeGroups/NodeNonElective", _nonElectiveGroupsEditor = new StaffNonElectiveStaffGroupEditorComponent(_staffDetail.Groups, formDataResponse.StaffGroupChoices, !CanModifyNonElectiveGroups)));
					this.Pages.Add(new NavigatorPage("NodeStaff/NodeGroups/NodeElective", _electiveGroupsEditor = new StaffElectiveStaffGroupEditorComponent(_staffDetail.Groups, formDataResponse.StaffGroupChoices, !CanModifyStaffProfile)));
				});

			// instantiate all extension pages
			_extensionPages = new List<IStaffEditorPage>();
			foreach (IStaffEditorPageProvider pageProvider in new StaffEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new EditorContext(this)));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (IStaffEditorPage page in _extensionPages)
			{
				this.Pages.Add(new NavigatorPage(page.Path, page.GetComponent()));
			}

			base.Start();
		}

		public override void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				// give extension pages a chance to save data prior to commit
				_extensionPages.ForEach(delegate(IStaffEditorPage page) { page.Save(); });

				// update non-elective groups
				_staffDetail.Groups = _nonElectiveGroupsEditor != null 
					? new List<StaffGroupSummary>(_nonElectiveGroupsEditor.SelectedItems)
					: new List<StaffGroupSummary>();

				// update elective groups
				if (_electiveGroupsEditor!= null)
				{
					_staffDetail.Groups.AddRange(_electiveGroupsEditor.SelectedItems);
				}

				// add or update staff
				Platform.GetService<IStaffAdminService>(
					delegate(IStaffAdminService service)
					{
						if (_isNew)
						{
							AddStaffResponse response = service.AddStaff(new AddStaffRequest(_staffDetail));
							_staffRef = response.Staff.StaffRef;
							_staffSummary = response.Staff;
						}
						else
						{
							UpdateStaffResponse response = service.UpdateStaff(new UpdateStaffRequest(_staffDetail));
							_staffRef = response.Staff.StaffRef;
							_staffSummary = response.Staff;
						}
					});

				// if necessary, update associated user account
				if(_originalStaffUserName != _staffDetail.UserName)
				{
					// clear staff from the existing user
					UpdateUserAccount(_originalStaffUserName, null);

					// update the current user with the staff name
					UpdateUserAccount(_staffDetail.UserName, _staffSummary);
				}

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionSaveStaff, this.Host.DesktopWindow,
					delegate
					{
						this.ExitCode = ApplicationComponentExitCode.Error;
						this.Host.Exit();
					});
			}
		}

		internal static void UpdateUserAccount(string userName, StaffSummary staff)
		{
			if(!string.IsNullOrEmpty(userName))
			{
				Platform.GetService<IUserAdminService>(
					delegate(IUserAdminService service)
					{
						// check if the user account exists
						ListUsersRequest request = new ListUsersRequest();
						request.UserName = userName;
						request.ExactMatchOnly = true;
						UserSummary user = CollectionUtils.FirstElement(service.ListUsers(request).Users);

						if(user != null)
						{
							// modify the display name on the user account
							UserDetail detail = service.LoadUserForEdit(
								new LoadUserForEditRequest(userName)).UserDetail;
							detail.DisplayName = (staff == null) ? null : staff.Name.ToString();

							service.UpdateUser(new UpdateUserRequest(detail));
						}
					});
			}
		}

		private static bool CanModifyStaffProfile
		{
			get
			{
				// require either Staff Admin or StaffProfile.Update token
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff)
					   || Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.StaffProfile.Update);
			}
		}

		private static bool CanModifyNonElectiveGroups
		{
			get
			{
				// require both Staff and StaffGroup admin tokens
				return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff)
					&& Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
			}
		}

	}
}
