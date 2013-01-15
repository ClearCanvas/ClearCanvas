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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Extended.Common.Admin.ProtocolAdmin;
using ClearCanvas.Ris.Client.Admin;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Extension point for views onto <see cref="ProtocolGroupEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ProtocolGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProtocolGroupEditorComponent class
	/// </summary>
	[AssociateView(typeof(ProtocolGroupEditorComponentViewExtensionPoint))]
	public class ProtocolGroupEditorComponent : ApplicationComponent
	{
		#region Private fields

		private EntityRef _protocolGroupRef;
		private ProtocolGroupSummary _protocolGroupSummary;
		private ProtocolGroupDetail _protocolGroupDetail;

		private readonly bool _isNew;

		private ProtocolCodeTable _availableProtocolCodes;
		private ProtocolCodeTable _selectedProtocolCodes;
		private ProtocolCodeSummary _selectedProtocolCodesSelection;

		private SimpleActionModel _selectedProtocolCodesActionHandler;
		private readonly string _moveCodeUpKey = "MoveCodeUp";
		private readonly string _moveCodeDownKey = "MoveCodeDown";
		private readonly string _newCodeKey = "NewCode";

		private ProcedureTypeGroupSummaryTable _availableReadingGroups;
		private ProcedureTypeGroupSummaryTable _selectedReadingGroups;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ProtocolGroupEditorComponent()
		{
			_isNew = true;
		}

		public ProtocolGroupEditorComponent(EntityRef protocolGroupRef)
		{
			_isNew = false;
			_protocolGroupRef = protocolGroupRef;
		}

		#endregion

		#region ApplicationComponent overrides

		public override void Start()
		{
			_availableProtocolCodes = new ProtocolCodeTable();
			_selectedProtocolCodes = new ProtocolCodeTable();

			_selectedProtocolCodesActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			_selectedProtocolCodesActionHandler.AddAction(_moveCodeUpKey, SR.TitleMoveProtocolCodeUp, "Icons.UpToolSmall.png", SR.TitleMoveProtocolCodeUp, MoveProtocolCodeUp);
			_selectedProtocolCodesActionHandler.AddAction(_moveCodeDownKey, SR.TitleMoveProtocolCodeDown, "Icons.DownToolSmall.png", SR.TitleMoveProtocolCodeDown, MoveProtocolCodeDown);
			_selectedProtocolCodesActionHandler.AddAction(_newCodeKey, SR.TitleNewProtocolCode, "Icons.AddToolSmall.png", SR.TitleNewProtocolCode, AddNewProtocolCode);
			_selectedProtocolCodesActionHandler[_moveCodeUpKey].Enabled = false;
			_selectedProtocolCodesActionHandler[_moveCodeDownKey].Enabled = false;
			_selectedProtocolCodesActionHandler[_newCodeKey].Enabled = true;

			_availableReadingGroups = new ProcedureTypeGroupSummaryTable();
			_selectedReadingGroups = new ProcedureTypeGroupSummaryTable();

			Platform.GetService<IProtocolAdminService>(service =>
			{
				var editFormDataResponse = service.GetProtocolGroupEditFormData(new GetProtocolGroupEditFormDataRequest());

				_availableProtocolCodes.Items.AddRange(editFormDataResponse.ProtocolCodes);
				_availableReadingGroups.Items.AddRange(editFormDataResponse.ReadingGroups);

				if (_isNew)
				{
					_protocolGroupDetail = new ProtocolGroupDetail();
				}
				else
				{
					var response = service.LoadProtocolGroupForEdit(new LoadProtocolGroupForEditRequest(_protocolGroupRef));

					_protocolGroupDetail = response.Detail;

					_selectedProtocolCodes.Items.AddRange(_protocolGroupDetail.Codes);
					_selectedReadingGroups.Items.AddRange(_protocolGroupDetail.ReadingGroups);
				}

				foreach (var item in _selectedProtocolCodes.Items)
				{
					_availableProtocolCodes.Items.Remove(item);
				}

				foreach (var item in _selectedReadingGroups.Items)
				{
					_availableReadingGroups.Items.Remove(item);
				}
			});

			base.Start();
		}

		#endregion

		#region Public Properties

		public ProtocolGroupSummary ProtocolGroupSummary
		{
			get { return _protocolGroupSummary; }
		}
		#endregion

		#region Presentation Model

		[ValidateNotNull]
		public string Name
		{
			get { return _protocolGroupDetail.Name; }
			set
			{
				_protocolGroupDetail.Name = value;
				this.Modified = true;
			}
		}

		public string Description
		{
			get { return _protocolGroupDetail.Description; }
			set
			{
				_protocolGroupDetail.Description = value;
				this.Modified = true;
			}
		}

		public ITable AvailableProtocolCodes
		{
			get { return _availableProtocolCodes; }
		}

		public ITable SelectedProtocolCodes
		{
			get { return _selectedProtocolCodes; }
		}

		public ActionModelNode SelectedProtocolCodesActionModel
		{
			get { return _selectedProtocolCodesActionHandler; }
		}

		public ISelection SelectedProtocolCodesSelection
		{
			get { return new Selection(_selectedProtocolCodesSelection); }
			set
			{
				_selectedProtocolCodesSelection = (ProtocolCodeSummary)value.Item;
				SelectedProtocolCodesSelectionChanged();
			}
		}

		private void SelectedProtocolCodesSelectionChanged()
		{
			var somethingSelected = _selectedProtocolCodesSelection != null;

			_selectedProtocolCodesActionHandler[_moveCodeUpKey].Enabled = somethingSelected;
			_selectedProtocolCodesActionHandler[_moveCodeDownKey].Enabled = somethingSelected;
		}

		public ITable AvailableReadingGroups
		{
			get { return _availableReadingGroups; }
		}

		public ITable SelectedReadingGroups
		{
			get { return _selectedReadingGroups; }
		}

		public void Accept()
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
					ExceptionHandler.Report(e, SR.ExceptionSaveProtocolGroup, this.Host.DesktopWindow,
						delegate
						{
							this.ExitCode = ApplicationComponentExitCode.Error;
							this.Host.Exit();
						});
				}
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		public void ItemsAddedOrRemoved()
		{
			this.Modified = true;
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		#endregion

		#region Private methods

		private void SaveChanges()
		{
			_protocolGroupDetail.Codes.Clear();
			_protocolGroupDetail.Codes.AddRange(_selectedProtocolCodes.Items);

			_protocolGroupDetail.ReadingGroups.Clear();
			_protocolGroupDetail.ReadingGroups.AddRange(_selectedReadingGroups.Items);

			Platform.GetService<IProtocolAdminService>(service =>
			{
				if (_isNew)
				{
					var response = service.AddProtocolGroup(new AddProtocolGroupRequest(_protocolGroupDetail));
					_protocolGroupRef = response.Summary.ProtocolGroupRef;
					_protocolGroupSummary = response.Summary;
				}
				else
				{
					var response = service.UpdateProtocolGroup(new UpdateProtocolGroupRequest(_protocolGroupRef, _protocolGroupDetail));
					_protocolGroupRef = response.Summary.ProtocolGroupRef;
					_protocolGroupSummary = response.Summary;
				}
			});
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		#endregion

		public void AddNewProtocolCode()
		{
			try
			{
				var editor = new ProtocolCodeEditorComponent();
				var exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddProtocolCode);
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					_selectedProtocolCodes.Items.Add(editor.ProtocolCode);
					this.Modified = true;
				}
			}
			catch (Exception e)
			{
				// could not launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void MoveProtocolCodeUp()
		{
			if (_selectedProtocolCodesSelection == null)
				return;

			var index = _selectedProtocolCodes.Items.IndexOf(_selectedProtocolCodesSelection);
			if (index <= 0)
				return;

			// Swap selected item with preceding item
			_selectedProtocolCodes.Items[index] = _selectedProtocolCodes.Items[index - 1];
			_selectedProtocolCodes.Items[index - 1] = _selectedProtocolCodesSelection;

			// Ensures that UI updates and correct row is highlighted
			NotifyPropertyChanged("SelectedProtocolCodesSelection");

			this.Modified = true;
		}

		public void MoveProtocolCodeDown()
		{
			if (_selectedProtocolCodesSelection == null)
				return;

			var index = _selectedProtocolCodes.Items.IndexOf(_selectedProtocolCodesSelection);
			if (index >= _selectedProtocolCodes.Items.Count - 1)
				return;

			// Swap selected item with following item
			_selectedProtocolCodes.Items[index] = _selectedProtocolCodes.Items[index + 1];
			_selectedProtocolCodes.Items[index + 1] = _selectedProtocolCodesSelection;

			// Ensures that UI updates and correct row is highlighted
			NotifyPropertyChanged("SelectedProtocolCodesSelection");

			this.Modified = true;
		}
	}
}
