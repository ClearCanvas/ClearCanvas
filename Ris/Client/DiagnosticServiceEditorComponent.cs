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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{

	/// <summary>
	/// Extension point for views onto <see cref="DiagnosticServiceEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DiagnosticServiceEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DiagnosticServiceEditorComponent class
	/// </summary>
	[AssociateView(typeof(DiagnosticServiceEditorComponentViewExtensionPoint))]
	public class DiagnosticServiceEditorComponent : ApplicationComponent
	{
		private readonly bool _isNew;

		private EntityRef _editedItemEntityRef;
		private DiagnosticServiceDetail _editedItemDetail;
		private DiagnosticServiceSummary _editedItemSummary;

		private ProcedureTypeSummaryTable _availableProcedureTypes;
		private ProcedureTypeSummaryTable _selectedProcedureTypes;

		public DiagnosticServiceEditorComponent()
		{
			_isNew = true;
		}

		public DiagnosticServiceEditorComponent(EntityRef entityRef)
		{
			_editedItemEntityRef = entityRef;
			_isNew = false;
		}

		public override void Start()
		{
			_availableProcedureTypes = new ProcedureTypeSummaryTable();
			_selectedProcedureTypes = new ProcedureTypeSummaryTable();

			Platform.GetService<IDiagnosticServiceAdminService>(
				service =>
					{
						var formDataResponse =
							service.LoadDiagnosticServiceEditorFormData(new LoadDiagnosticServiceEditorFormDataRequest());
						_availableProcedureTypes.Items.AddRange(formDataResponse.ProcedureTypeChoices);

						if (_isNew)
						{
							_editedItemDetail = new DiagnosticServiceDetail();
						}
						else
						{
							var response = service.LoadDiagnosticServiceForEdit(new LoadDiagnosticServiceForEditRequest(_editedItemEntityRef));

							_editedItemDetail = response.DiagnosticService;
							_selectedProcedureTypes.Items.AddRange(_editedItemDetail.ProcedureTypes);
						}

						foreach (var selectedSummary in _selectedProcedureTypes.Items)
						{
							_availableProcedureTypes.Items.Remove(selectedSummary);
						}
					});

			base.Start();
		}

		public DiagnosticServiceSummary DiagnosticServiceSummary
		{
			get { return _editedItemSummary; }
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Name
		{
			get { return _editedItemDetail.Name; }
			set
			{
				_editedItemDetail.Name = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string ID
		{
			get { return _editedItemDetail.Id; }
			set
			{
				_editedItemDetail.Id = value;
				this.Modified = true;
			}
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public ITable AvailableProcedureTypes
		{
			get { return _availableProcedureTypes; }
		}

		public ITable SelectedProcedureTypes
		{
			get { return _selectedProcedureTypes; }
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		public void ItemsAddedOrRemoved()
		{
			this.Modified = true;
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			// can't seem to get validation working on the ListItemSelector control that is bound here,
			// so quick fix is we just manually check and show a message box
			if(_selectedProcedureTypes.Items.Count == 0)
			{
				this.Host.ShowMessageBox(SR.MessageDiagnosticServiceMustHaveAssociatedProcedureTypes, MessageBoxActions.Ok);
				return;
			}

			try
			{
				Platform.GetService<IDiagnosticServiceAdminService>(
					service =>
						{
							if (_isNew)
							{
								_editedItemDetail.ProcedureTypes.AddRange(_selectedProcedureTypes.Items);
								var response = service.AddDiagnosticService(new AddDiagnosticServiceRequest(_editedItemDetail));
								_editedItemEntityRef = response.DiagnosticService.DiagnosticServiceRef;
								_editedItemSummary = response.DiagnosticService;
							}
							else
							{
								_editedItemDetail.ProcedureTypes.Clear();
								_editedItemDetail.ProcedureTypes.AddRange(_selectedProcedureTypes.Items);
								var response = service.UpdateDiagnosticService(new UpdateDiagnosticServiceRequest(_editedItemDetail));
								_editedItemEntityRef = response.DiagnosticService.DiagnosticServiceRef;
								_editedItemSummary = response.DiagnosticService;
							}
						});

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionSaveDiagnosticService, this.Host.DesktopWindow,
				                        () => Exit(ApplicationComponentExitCode.Error));
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

	}
}
