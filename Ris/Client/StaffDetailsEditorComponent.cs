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

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="StaffDetailsEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class StaffDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// StaffDetailsEditorComponent class
	/// </summary>
	[AssociateView(typeof(StaffDetailsEditorComponentViewExtensionPoint))]
	public class StaffDetailsEditorComponent : ApplicationComponent
	{
		private UserLookupData _selectedUser = null;
		private ILookupHandler _userLookupHandler;
		private string _userNameNoLongerExist;

		private StaffDetail _staffDetail;
		private readonly List<EnumValueInfo> _staffTypeChoices;
		private readonly List<EnumValueInfo> _sexChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public StaffDetailsEditorComponent(List<EnumValueInfo> staffTypeChoices, List<EnumValueInfo> sexChoices)
		{
			_staffDetail = new StaffDetail();
			_staffTypeChoices = staffTypeChoices;
			_sexChoices = sexChoices;
		}

		public override void Start()
		{
			_selectedUser = GetUserForStaff(_staffDetail); 
			_userLookupHandler = new UserLookupHandler(this.Host.DesktopWindow);

			this.Validation.Add(new ValidationRule("SelectedUser",
				delegate
				{
					bool userNoLongerExist = !string.IsNullOrEmpty(_userNameNoLongerExist) 
						&& (_selectedUser != null && _selectedUser.UserName == _userNameNoLongerExist);
					return new ValidationResult(!userNoLongerExist, string.Format(SR.MessageAssociatedUserNoLongerExist, _userNameNoLongerExist));
				}));

			base.Start();
		}

		public StaffDetail StaffDetail
		{
			get { return _staffDetail; }
			set { _staffDetail = value; }
		}

		#region Presentation Model

		public bool ReadOnly
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Staff) == false; }
		}

		public bool IsUserAdmin
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User); }	
		}

		public ILookupHandler UserLookupHandler
		{
			get { return _userLookupHandler; }
		}

		public object SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				if (_selectedUser != value)
				{
					_selectedUser = (UserLookupData)value;
					_staffDetail.UserName = _selectedUser == null ? null : _selectedUser.UserName;
					this.Modified = true;
				}
			}
		}

        [ValidateNotNull]
        public EnumValueInfo StaffType
		{
			get { return _staffDetail.StaffType; }
			set
			{
				_staffDetail.StaffType = value;

				this.Modified = true;
			}
		}

		public IList StaffTypeChoices
		{
			get { return _staffTypeChoices; }
		}

        [ValidateNotNull]
        public string StaffId
		{
			get { return _staffDetail.StaffId; }
			set
			{
				_staffDetail.StaffId = value;
				this.Modified = true;
			}
		}

        [ValidateNotNull]
        public string FamilyName
		{
			get { return _staffDetail.Name.FamilyName; }
			set
			{
				_staffDetail.Name.FamilyName = value;
				this.Modified = true;
			}
		}

        [ValidateNotNull]
        public string GivenName
		{
			get { return _staffDetail.Name.GivenName; }
			set
			{
				_staffDetail.Name.GivenName = value;
				this.Modified = true;
			}
		}

		public string MiddleName
		{
			get { return _staffDetail.Name.MiddleName; }
			set
			{
				_staffDetail.Name.MiddleName = value;
				this.Modified = true;
			}
		}

		public string Prefix
		{
			get { return _staffDetail.Name.Prefix; }
			set
			{
				_staffDetail.Name.Prefix = value;
				this.Modified = true;
			}
		}

		public string Suffix
		{
			get { return _staffDetail.Name.Suffix; }
			set
			{
				_staffDetail.Name.Suffix = value;
				this.Modified = true;
			}
		}

		public string Degree
		{
			get { return _staffDetail.Name.Degree; }
			set
			{
				_staffDetail.Name.Degree = value;
				this.Modified = true;
			}
		}

        [ValidateNotNull]
        public EnumValueInfo Sex
		{
			get { return _staffDetail.Sex; }
			set
			{
				_staffDetail.Sex = value;
				this.Modified = true;
			}
		}

		public IList SexChoices
		{
			get { return _sexChoices; }
		}

		public string Title
		{
			get { return _staffDetail.Title; }
			set
			{
				_staffDetail.Title = value;
				this.Modified = true;
			}
		}

		public string LicenseNumber
		{
			get { return _staffDetail.LicenseNumber; }
			set
			{
				_staffDetail.LicenseNumber = value;
				this.Modified = true;
			}
		}

		public string BillingNumber
		{
			get { return _staffDetail.BillingNumber; }
			set
			{
				_staffDetail.BillingNumber = value;
				this.Modified = true;
			}
		}

		#endregion

		private UserLookupData GetUserForStaff(StaffDetail staff)
		{
			UserLookupData user = null;

			if (!string.IsNullOrEmpty(staff.UserName))
			{
				// If this staff is associated with an user, get the most updated user using IUserAdminService
				if (this.IsUserAdmin)
				{
					Platform.GetService<IUserAdminService>(
						delegate(IUserAdminService service)
						{
							ListUsersRequest request = new ListUsersRequest();
							request.UserName = staff.UserName;
							request.ExactMatchOnly = true;
							ListUsersResponse response = service.ListUsers(request);
							UserSummary summary = CollectionUtils.FirstElement(response.Users);

							// If there is no return user, it means that the user was deleted without updating staff.
							user = summary == null ? null : new UserLookupData(summary.UserName);
						});

					if (user == null)
					{
						_userNameNoLongerExist = staff.UserName;

						// The user no longer exist, delete user name.
						staff.UserName = null;
						
						// Nevertheless, display this info.
						user = new UserLookupData(_userNameNoLongerExist);						
					}
				}
				else
				{
					// if user is not a UserAdmin, don't hit the server, use the data we already have.
					// unfortunately, we cannot detect the case where user is deleted without staff updated.
					user = new UserLookupData(staff.UserName);
				}
			}

			return user;
		}
	}
}
