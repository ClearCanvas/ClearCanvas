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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="WorklistDetailEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class WorklistDetailEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorklistDetailEditorComponent class
	/// </summary>
	[AssociateView(typeof(WorklistDetailEditorComponentViewExtensionPoint))]
	public class WorklistDetailEditorComponent : WorklistDetailEditorComponentBase
	{
		private readonly WorklistAdminDetail _worklistDetail;
		private readonly WorklistEditorMode _editorMode;
		private readonly bool _dialogMode;
		private readonly bool _adminMode;
		private readonly List<StaffGroupSummary> _groupChoices;

		private bool _isPersonal;

		private bool _hasWorklistCountError;
		private string _worklistCountErrorMessage;

		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistDetailEditorComponent(WorklistAdminDetail detail, List<WorklistClassSummary> worklistClasses, List<StaffGroupSummary> ownerGroupChoices, WorklistEditorMode editorMode, bool adminMode, bool dialogMode)
			:base(worklistClasses, GetDefaultWorklistClass(worklistClasses, detail))
		{
			_worklistDetail = detail;
			_dialogMode = dialogMode;
			_editorMode = editorMode;
			_adminMode = adminMode;
			_groupChoices = ownerGroupChoices;

			if(_editorMode == WorklistEditorMode.Add)
			{
				// default to "personal" if user has authority
				_isPersonal = HasPersonalAdminAuthority;
			}
			else
			{
				// default to "personal" if not a user worklist (this could happen when duplicating from an admin worklist)
				_isPersonal = !_worklistDetail.IsUserWorklist || _worklistDetail.IsStaffOwned;
			}

			// update the class to the default (if this is a new worklist)
			_worklistDetail.WorklistClass = GetDefaultWorklistClass(worklistClasses, detail);

			this.Validation.Add(
				new ValidationRule("SelectedGroup",
					delegate
					{
						var success = _adminMode || this.IsPersonal || (this.IsGroup && this.SelectedGroup != null);
						return new ValidationResult(success, SR.MessageValueRequired);
					}));

			this.Validation.Add(new ValidationRule("IsPersonal",
				delegate
					{
						var showValidation = this.IsPersonalGroupSelectionEnabled && this.IsPersonal && _hasWorklistCountError;
						return new ValidationResult(!showValidation, _worklistCountErrorMessage);
					}));

			this.Validation.Add(new ValidationRule("SelectedGroup",
				delegate
					{
						var showValidation = this.IsPersonalGroupSelectionEnabled && this.IsGroup && _worklistDetail.OwnerGroup != null && _hasWorklistCountError;
						return new ValidationResult(!showValidation, _worklistCountErrorMessage);
					}));
		}

		public override void Start()
		{
			ValidateWorklistCount();

			base.Start();
		}

		#region Presentation Model

		public bool IsOwnerPanelVisible
		{
			get { return !_adminMode; }
		}

		public bool IsPersonalGroupSelectionEnabled
		{
			get { return _editorMode != WorklistEditorMode.Edit && HasGroupAdminAuthority && HasPersonalAdminAuthority; }
		}

		public bool IsPersonal
		{
			get { return _isPersonal; }
			set
			{
				if(value != _isPersonal)
				{
					_isPersonal = value;
					if (_isPersonal)
						this.SelectedGroup = null;

					this.Modified = true;
					ValidateWorklistCount();
					NotifyPropertyChanged("IsPersonal");
					NotifyPropertyChanged("IsGroup");
				}
			}
		}

		public bool IsGroup
		{
			get { return !this.IsPersonal; }
			set { this.IsPersonal = !value; }
		}

		public bool IsGroupChoicesEnabled
		{
			get { return _editorMode != WorklistEditorMode.Edit && HasGroupAdminAuthority && IsGroup; }
		}

		public IList GroupChoices
		{
			get { return _groupChoices; }
		}

		public string FormatGroup(object item)
		{
			var group = (StaffGroupSummary)item;
			return group.Name;
		}

		public StaffGroupSummary SelectedGroup
		{
			get { return _worklistDetail.OwnerGroup; }
			set
			{
				if (Equals(_worklistDetail.OwnerGroup, value))
					return;

				_worklistDetail.OwnerGroup = value;
				this.Modified = true;
				ValidateWorklistCount();
				NotifyPropertyChanged("SelectedGroup");
			}
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _worklistDetail.Name; }
			set
			{
				_worklistDetail.Name = value;
				this.Modified = true;
			}
		}

		public string Description
		{
			get { return _worklistDetail.Description; }
			set
			{
				_worklistDetail.Description = value;
				this.Modified = true;
			}
		}

		public bool IsWorklistClassReadOnly
		{
			get { return _editorMode == WorklistEditorMode.Edit; }
		}

		[ValidateNotNull]
		public WorklistClassSummary WorklistClass
		{
			get { return _worklistDetail.WorklistClass; }
			set
			{
				if(!Equals(_worklistDetail.WorklistClass, value))
				{
					_worklistDetail.WorklistClass = value;
					if(_worklistDetail.WorklistClass != null)
					{
						// update settings, but don't save
						WorklistEditorComponentSettings.Default.DefaultWorklistClass = _worklistDetail.WorklistClass.ClassName;
					}
					this.Modified = true;
					NotifyWorklistClassChanged();
					NotifyPropertyChanged("WorklistClassDescription");
				}
			}
		}

		public string FormatWorklistClass(object item)
		{
			var summary = (WorklistClassSummary) item;
			return summary.DisplayName;
		}

		public string WorklistClassDescription
		{
			get { return _worklistDetail.WorklistClass == null ? null : _worklistDetail.WorklistClass.Description; }
		}

		public bool AcceptButtonVisible
		{
			get { return _dialogMode; }
		}

		public bool CancelButtonVisible
		{
			get { return _dialogMode; }
		}

		public void Accept()
		{
			Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		protected override void UpdateWorklistClassChoices()
		{
			// blank out the selected worklist class if not in the new set of choices
			if(!this.WorklistClassChoices.Contains(_worklistDetail.WorklistClass))
				_worklistDetail.WorklistClass = null;

			base.UpdateWorklistClassChoices();
		}

		private static bool HasGroupAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Worklist.Group); }
		}

		private static bool HasPersonalAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Worklist.Personal); }
		}

		private static WorklistClassSummary GetDefaultWorklistClass(IEnumerable<WorklistClassSummary> worklistClasses, WorklistAdminDetail detail)
		{
			return detail.WorklistClass
				?? CollectionUtils.SelectFirst(worklistClasses, w => w.ClassName == WorklistEditorComponentSettings.Default.DefaultWorklistClass);
		}


		private void ValidateWorklistCount()
		{
			Async.CancelPending(this);

			if (_editorMode == WorklistEditorMode.Edit && !this.IsPersonalGroupSelectionEnabled)
				return;

			_hasWorklistCountError = false;
			_worklistCountErrorMessage = null;

			Async.Request(this,
				 (IWorklistAdminService service) =>
				 {
             		var request = new GetWorklistEditFormDataRequest
             	              		{
             	              			GetWorklistEditValidationRequest =
             	              				new GetWorklistEditValidationRequest(!_adminMode, _worklistDetail.OwnerGroup)
             	              		};

             		return service.GetWorklistEditFormData(request);
				 },
				 response =>
				 {
             		_hasWorklistCountError = response.GetWorklistEditValidationResponse.HasError;
             		_worklistCountErrorMessage = response.GetWorklistEditValidationResponse.ErrorMessage;
             		if (_hasWorklistCountError)
             			ShowValidation(true);
				 });
		}

	}
}
