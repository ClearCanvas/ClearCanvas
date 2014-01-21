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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Enterprise.Desktop
{
    public class AuthorityTokenTableEntry
    {
        private readonly AuthorityTokenSummary _summary;
        private bool _selected;
        private event EventHandler _selectedChanged;
    	private readonly Path _path;

        public AuthorityTokenTableEntry(AuthorityTokenSummary authorityTokenSummary, EventHandler onChanged)
        {
            _summary = authorityTokenSummary;
            _selected = false;
            _selectedChanged = onChanged;
			_path = new Path(_summary.Name, new ResourceResolver(GetType().Assembly));
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    EventsHelper.Fire(_selectedChanged, this, EventArgs.Empty);
                }
            }
        }

    	public string Name
    	{
			get { return _summary.Name; }
    	}

    	public string Description
    	{
			get { return _summary.Description; }
    	}

    	public Path Path
    	{
			get { return _path; }
    	}

    	internal AuthorityTokenSummary Summary
    	{
			get { return _summary; }
    	}
    }

    public class SelectableAuthorityTokenTable : Table<AuthorityTokenTableEntry>
    {
        public SelectableAuthorityTokenTable ()
    	{
            Columns.Add(new TableColumn<AuthorityTokenTableEntry, bool>(SR.ColumnActive,
                delegate(AuthorityTokenTableEntry entry) { return entry.Selected; },
                delegate(AuthorityTokenTableEntry entry, bool value) { entry.Selected = value; },
                0.5f));

            Columns.Add(new TableColumn<AuthorityTokenTableEntry, string>(SR.ColumnAuthorityTokenName,
                delegate(AuthorityTokenTableEntry entry) { return entry.Name; },
                1.5f));

            Columns.Add(new TableColumn<AuthorityTokenTableEntry, string>(SR.ColumnAuthorityTokenDescription,
                delegate(AuthorityTokenTableEntry entry) { return entry.Description; },
                3.5f));
	    }
    }

    /// <summary>
    /// Extension point for views onto <see cref="AuthorityGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AuthorityGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AuthorityGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(AuthorityGroupEditorComponentViewExtensionPoint))]
    public class AuthorityGroupEditorComponent : ApplicationComponent
    {
        private readonly bool _isNew;
    	private readonly bool _duplicate;
        private AuthorityGroupSummary _authorityGroupSummary;
        private AuthorityGroupDetail _authorityGroupDetail;
		private List<AuthorityTokenTableEntry> _authorityTokens;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthorityGroupEditorComponent()
        {
            _isNew = true;
        	_duplicate = false;
        }

		/// <summary>
		/// Constructor for editing or duplicating an authority group.
		/// </summary>
        /// <param name="authorityGroup"></param>
		/// <param name="duplicate"></param>
        public AuthorityGroupEditorComponent(AuthorityGroupSummary authorityGroup, bool duplicate)
        {
            Platform.CheckForNullReference(authorityGroup, "authorityGroup");

        	_duplicate = duplicate;
            _isNew = _duplicate;
            _authorityGroupSummary = authorityGroup;
        }

    	public AuthorityGroupSummary AuthorityGroupSummary
    	{
            get { return _authorityGroupSummary; }
    	}

        public override void Start()
        {
            Platform.GetService<IAuthorityGroupAdminService>(
				delegate(IAuthorityGroupAdminService service)
                {
                    ListAuthorityTokensResponse authorityTokenResponse = service.ListAuthorityTokens(new ListAuthorityTokensRequest());

                    _authorityTokens = CollectionUtils.Map<AuthorityTokenSummary, AuthorityTokenTableEntry>(
                        CollectionUtils.Sort(authorityTokenResponse.AuthorityTokens,
                                             (x, y) => x.Name.CompareTo(y.Name)),
                        summary => new AuthorityTokenTableEntry(summary, OnAuthorityTokenChecked));

                    if (_isNew && !_duplicate)
                    {
                        _authorityGroupDetail = new AuthorityGroupDetail();
                    }
                    else
                    {
                        LoadAuthorityGroupForEditResponse response = service.LoadAuthorityGroupForEdit(new LoadAuthorityGroupForEditRequest(_authorityGroupSummary.AuthorityGroupRef));
                        _authorityGroupDetail = response.AuthorityGroupDetail;

						// if duplicating, append something to the name
						if (_duplicate)
						{
						    _authorityGroupDetail.AuthorityGroupRef = null;
                            _authorityGroupDetail.Name = _authorityGroupDetail.Name + " Copy";
							_authorityGroupDetail.BuiltIn = false;	// built-in groups duplicate to a regular group
						}
                    }

                    InitialiseTable();
                });

            base.Start();
        }

        #region Presentation Model

		[ValidateNotNull]
        public string Name
        {
            get { return _authorityGroupDetail.Name; }
            set
            {
                _authorityGroupDetail.Name = value;
                Modified = true;
            }
        }

        [ValidateNotNull]
        public string Description
        {
            get { return _authorityGroupDetail.Description; }
            set
            {
                _authorityGroupDetail.Description = value;
                Modified = true;
            }
        }

        public bool DataGroup
        {
            get { return _authorityGroupDetail.DataGroup; }
            set
            {
                _authorityGroupDetail.DataGroup = value;
                Modified = true;
                DataGroupModified = true;
            }
        }

        public bool DataGroupModified
        {
            get; set;
        }

        public List<AuthorityTokenTableEntry> AuthorityTokens
        {
            get { return _authorityTokens; }
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
                bool closeAborted = false;

				Platform.GetService<IAuthorityGroupAdminService>(
					delegate(IAuthorityGroupAdminService service)
                    {
                        if (_isNew)
                        {
                            AddAuthorityGroupResponse response = service.AddAuthorityGroup(new AddAuthorityGroupRequest(_authorityGroupDetail));
                            _authorityGroupSummary = response.AuthorityGroupSummary;
                        }
                        else
                        {
                            UpdateAuthorityGroupRequest request = new UpdateAuthorityGroupRequest(_authorityGroupDetail);
                            if (DataGroupModified && DataGroup == false)
                            {
                                var optionsComponent = new PasswordConfirmComponent
                                {
                                    Description = SR.DescriptionDataAccessGroupChange
                                };
                                if (ApplicationComponentExitCode.Accepted == LaunchAsDialog(Host.DesktopWindow, optionsComponent, SR.TitlePasswordConfirm))
                                {
                                    request.Password = optionsComponent.Password;
                                }
                                else
                                {
                                    closeAborted = true;
                                    return;
                                }
                            }

                            UpdateAuthorityGroupResponse response = service.UpdateAuthorityGroup(request);
                            _authorityGroupSummary = response.AuthorityGroupSummary;
                        }
                    });
                if (closeAborted)
                    return;
                Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (FaultException<UserAccessDeniedException>)
            {
                Host.ShowMessageBox(SR.ExceptionUserAccessDenied, MessageBoxActions.Ok);        
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionSaveAuthorityGroup, Host.DesktopWindow,
                    delegate
                    {
                        ExitCode = ApplicationComponentExitCode.Error;
                        Host.Exit();
                    });
            }
        }

        public bool AcceptEnabled
        {
            get { return Modified; }
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

        public void OnAuthorityTokenChecked(object sender, EventArgs e)
        {
            AuthorityTokenTableEntry changedEntry = (AuthorityTokenTableEntry)sender;

            if (changedEntry.Selected == false)
            {
                CollectionUtils.Remove(
                    _authorityGroupDetail.AuthorityTokens,
                    summary => summary.Name == changedEntry.Summary.Name);
                Modified = true;
            }
            else
            {
                bool alreadyAdded = CollectionUtils.Contains(
                    _authorityGroupDetail.AuthorityTokens,
                    summary => summary.Name == changedEntry.Summary.Name);

                if (alreadyAdded == false)
                {
					_authorityGroupDetail.AuthorityTokens.Add(changedEntry.Summary);
                    Modified = true;
                }
            }

        }

        #endregion

        private void InitialiseTable()
        {
            foreach (AuthorityTokenSummary selectedToken in _authorityGroupDetail.AuthorityTokens)
            {
                AuthorityTokenTableEntry foundEntry = CollectionUtils.SelectFirst(
                    _authorityTokens,
                    entry => selectedToken.Name == entry.Summary.Name);

                if (foundEntry != null) foundEntry.Selected = true;
            }
        }
    }
}
