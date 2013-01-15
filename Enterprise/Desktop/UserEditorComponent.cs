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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Enterprise.Desktop
{
    public class AuthorityGroupTableEntry
    {
        private readonly AuthorityGroupSummary _summary;
        private bool _selected;
        private event EventHandler SelectedChanged;

        public AuthorityGroupTableEntry(AuthorityGroupSummary authorityGroupSummary, EventHandler onChanged)
        {
            _summary = authorityGroupSummary;
            _selected = false;
            SelectedChanged += onChanged;
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    EventsHelper.Fire(SelectedChanged, this, EventArgs.Empty);
                }
            }
        }

        public AuthorityGroupSummary AuthorityGroupSummary
        {
            get { return _summary; }
        }
    }

    public class SelectableAuthorityGroupTable : Table<AuthorityGroupTableEntry>
    {
        public SelectableAuthorityGroupTable()
        {
            Columns.Add(new TableColumn<AuthorityGroupTableEntry, bool>(SR.ColumnActive,
                                                                        entry => entry.Selected,
                delegate(AuthorityGroupTableEntry entry, bool value) { entry.Selected = value; },
                0.5f));

            Columns.Add(new TableColumn<AuthorityGroupTableEntry, string>(SR.ColumnAuthorityGroup,
                                                                          entry => entry.AuthorityGroupSummary.Name,
                2.5f));
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="UserEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class UserEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// UserEditorComponent class
    /// </summary>
    [AssociateView(typeof(UserEditorComponentViewExtensionPoint))]
    public class UserEditorComponent : ApplicationComponent
    {
        private readonly bool _isNew;
        private readonly string _userName;
		private readonly SelectableAuthorityGroupTable _table;

		private UserDetail _userDetail;
        private List<AuthorityGroupTableEntry> _authorityGroups;

        private UserSummary _userSummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserEditorComponent()
        {
            _isNew = true;
            _userName = null;
            _table = new SelectableAuthorityGroupTable();
        }

        public UserEditorComponent(string userName)
        {
            Platform.CheckForNullReference(userName, "userName");

            _isNew = false;
            _userName = userName;
            _table = new SelectableAuthorityGroupTable();
        }

        /// <summary>
        /// Returns the user summary for use by the caller of this component
        /// </summary>
        public UserSummary UserSummary
        {
            get { return _userSummary; }
        }

		public override void Start()
        {
			// load all auth groups
			Platform.GetService(
				delegate(IAuthorityGroupAdminService service)
				{
					ListAuthorityGroupsResponse authorityGroupsResponse = service.ListAuthorityGroups(new ListAuthorityGroupsRequest());

					_authorityGroups = CollectionUtils.Map(
						authorityGroupsResponse.AuthorityGroups,
						delegate(AuthorityGroupSummary summary)
						{
							return new AuthorityGroupTableEntry(summary, OnAuthorityGroupChecked);
						});
				});

			// load user
            Platform.GetService(
                delegate(IUserAdminService service)
                {
                    if (_isNew)
                    {
                        _userDetail = new UserDetail
                                          { 
                        	// Force users to change the password when they log in
                        	PasswordExpiryTime = Platform.Time 
                         };
                    }
                    else
                    {
                        LoadUserForEditResponse response = service.LoadUserForEdit(new LoadUserForEditRequest(_userName));
                        _userDetail = response.UserDetail;
                    }
                });

			InitialiseTable();

            base.Start();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string UserId
        {
            get { return _userDetail.UserName; }
            set
            {
                _userDetail.UserName = value;
                Modified = true;
            }
        }

        [ValidateNotNull]
		public string DisplayName
		{
			get { return _userDetail.DisplayName; }
			set
			{
				_userDetail.DisplayName = value;
				Modified = true;
			}
		}

        public string EmailAddress
        {
            get { return _userDetail.EmailAddress; }
            set
            {
                _userDetail.EmailAddress = value;
                Modified = true;
            }
        }

        public bool IsUserIdReadOnly
        {
            get { return !_isNew; }
        }

        public DateTime? ValidFrom
        {
            get { return _userDetail.ValidFrom; }
            set
            {
                // set valid from to the start of the day
                _userDetail.ValidFrom = value == null ? value : value.Value.Date;
                Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _userDetail.ValidUntil; }
            set
            {
                // set valid unitl to the end of the day
                _userDetail.ValidUntil = value == null ? value : value.Value.Date.AddDays(1).AddTicks(-1);
                Modified = true;
             }
        }

        public DateTime? PasswordExpiryTime
        {
            get { return _userDetail.PasswordExpiryTime; }
            set
            {
                // set valid unitl to the end of the day
                _userDetail.PasswordExpiryTime = value == null ? value : value.Value.Date;
                Modified = true;
            }
        }

        public bool AccountEnabled
        {
            get { return _userDetail.Enabled; }
            set
            {
                _userDetail.Enabled = value;
                Modified = true;
            }
        }

        public ITable Groups
        {
            get { return _table; }
        }

        public void Accept()
        {
            if (HasValidationErrors)
            {
                ShowValidation(true);
                return;
            }

            try
            {
				// add or update the user account
                Platform.GetService(
                    delegate(IUserAdminService service)
                    {
                        if (_isNew)
                        {
                            AddUserResponse response = service.AddUser(new AddUserRequest(_userDetail));
                            _userSummary = response.UserSummary;
                        }
                        else
                        {
                            UpdateUserResponse response = service.UpdateUser(new UpdateUserRequest(_userDetail));
                            _userSummary = response.UserSummary;
                        }
                    });

                Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(
                    e, 
                    SR.ExceptionSaveUser, 
                    Host.DesktopWindow,
                    delegate
                    {
                        ExitCode = ApplicationComponentExitCode.Error;
                        Host.Exit();
                    });
            }
        }

        public bool AcceptEnabled
        {
            get { return Modified && string.IsNullOrEmpty(UserId) == false; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { ModifiedChanged += value; }
            remove { ModifiedChanged -= value; }
        }

        public void Cancel()
        {
            ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public void OnAuthorityGroupChecked(object sender, EventArgs e)
        {
            var changedEntry = (AuthorityGroupTableEntry)sender;

            if (changedEntry.Selected == false)
            {
                // Remove the 
                CollectionUtils.Remove(_userDetail.AuthorityGroups,
                                       summary => summary.Name == changedEntry.AuthorityGroupSummary.Name);
                Modified = true;
            }
            else
            {
                bool alreadyAdded = CollectionUtils.Contains(_userDetail.AuthorityGroups,
                                                             summary =>
                                                             summary.Name == changedEntry.AuthorityGroupSummary.Name);
                
                if (alreadyAdded == false)
                {
                    _userDetail.AuthorityGroups.Add(changedEntry.AuthorityGroupSummary);
                    Modified = true;
                }
            }
        }

        #endregion

        private void InitialiseTable()
        {
            _table.Items.Clear();
            _table.Items.AddRange(_authorityGroups);

            foreach (AuthorityGroupSummary selectedSummary in _userDetail.AuthorityGroups)
            {
                AuthorityGroupSummary summary = selectedSummary;
                AuthorityGroupTableEntry foundEntry = CollectionUtils.SelectFirst(_authorityGroups,
                                                                                  entry =>
                                                                                  summary.Name == entry.AuthorityGroupSummary.Name);

                if (foundEntry != null) foundEntry.Selected = true;
            }
        }
    }
}
