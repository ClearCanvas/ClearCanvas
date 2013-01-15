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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="PatientSearchComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PatientSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[ExtensionPoint]
	public class PatientSearchToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IPatientSearchToolContext : IToolContext
	{
		event EventHandler SelectedProfileChanged;
		PatientProfileSummary SelectedProfile { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	/// <summary>
	/// PatientSearchComponent class
	/// </summary>
	[AssociateView(typeof(PatientSearchComponentViewExtensionPoint))]
	public class PatientSearchComponent : ApplicationComponent
	{
		class PatientSearchToolContext : ToolContext, IPatientSearchToolContext
		{
			private readonly PatientSearchComponent _component;

			public PatientSearchToolContext(PatientSearchComponent component)
			{
				_component = component;
			}

			public event EventHandler SelectedProfileChanged
			{
				add { _component.SelectedProfileChanged += value; }
				remove { _component.SelectedProfileChanged -= value; }
			}

			public PatientProfileSummary SelectedProfile
			{
				get { return (PatientProfileSummary)_component.SelectedProfile.Item; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}
		}

		private string _searchString;

		private PatientProfileTable _profileTable;
		private PatientProfileSummary _selectedProfile;
		private event EventHandler _selectedProfileChanged;

		private ToolSet _toolSet;

		public override void Start()
		{
			_profileTable = new PatientProfileTable();
			_toolSet = new ToolSet(new PatientSearchToolExtensionPoint(), new PatientSearchToolContext(this));

			base.Start();
		}

		public override void Stop()
		{
			_toolSet.Dispose();

			base.Stop();
		}

		#region Presentation Model

		public string SearchString
		{
			get { return _searchString; }
			set { _searchString = value; }
		}

		public bool SearchEnabled
		{
			get { return !String.IsNullOrEmpty(_searchString); }
		}

		public ITable Profiles
		{
			get { return _profileTable; }
		}

		public ActionModelNode ItemsContextMenuModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-contextmenu", _toolSet.Actions); }
		}

		public ActionModelNode ItemsToolbarModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-toolbar", _toolSet.Actions); }
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		public ISelection SelectedProfile
		{
			get { return new Selection(_selectedProfile); }
			set
			{
				if (Equals(_selectedProfile, value.Item))
					return;

				_selectedProfile = (PatientProfileSummary)value.Item;
				EventsHelper.Fire(_selectedProfileChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler SelectedProfileChanged
		{
			add { _selectedProfileChanged += value; }
			remove { _selectedProfileChanged -= value; }
		}

		public void Search()
		{
			try
			{
				_profileTable.Items.Clear();

				TextQueryResponse<PatientProfileSummary> response = null;

				Platform.GetService(
					delegate(IRegistrationWorkflowService service)
					{
						var request = new TextQueryRequest();
						request.TextQuery = _searchString;
						request.SpecificityThreshold = PatientSearchComponentSettings.Default.SearchCriteriaSpecificityThreshold;
						response = service.PatientProfileTextQuery(request);
					});

				if (response.TooManyMatches)
					throw new WeakSearchCriteriaException();

				_profileTable.Items.AddRange(response.Matches);
				this.SelectedProfile = new Selection(_profileTable.Items.Count > 0 ? _profileTable.Items[0] : null);

				if (_profileTable.Items.Count == 0)
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageResultsNotFound, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void OpenPatient()
		{
			try
			{
				var document = DocumentManager.Get<PatientBiographyDocument>(_selectedProfile.PatientProfileRef);
				if (document == null)
				{
					document = new PatientBiographyDocument(_selectedProfile, this.Host.DesktopWindow);
					document.Open();
				}
				else
				{
					document.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion
	}
}
