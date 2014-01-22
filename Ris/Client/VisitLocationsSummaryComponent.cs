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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="VisitLocationsSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class VisitLocationsSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// VisitLocationsSummaryComponent class
	/// </summary>
	[AssociateView(typeof(VisitLocationsSummaryComponentViewExtensionPoint))]
	public class VisitLocationsSummaryComponent : ApplicationComponent
	{
		private VisitDetail _visit;
		private readonly VisitLocationTable _locationsTable;
		private VisitLocationDetail _currentVisitLocationSelection;
		private readonly List<EnumValueInfo> _visitLocationRoleChoices;
		private readonly CrudActionModel _visitLocationActionHandler;

		/// <summary>
		/// Constructor
		/// </summary>
		public VisitLocationsSummaryComponent(List<EnumValueInfo> visitLocationRoleChoices)
		{
			_locationsTable = new VisitLocationTable();
			_visitLocationRoleChoices = visitLocationRoleChoices;

			_visitLocationActionHandler = new CrudActionModel();
			_visitLocationActionHandler.Add.SetClickHandler(AddVisitLocation);
			_visitLocationActionHandler.Edit.SetClickHandler(UpdateSelectedVisitLocation);
			_visitLocationActionHandler.Delete.SetClickHandler(DeleteSelectedVisitLocation);

			_visitLocationActionHandler.Add.Enabled = true;
		}

		public override void Start()
		{
			LoadVisitLocations();

			base.Start();
		}

		public VisitDetail Visit
		{
			get { return _visit; }
			set { _visit = value; }
		}

		public ITable Locations
		{
			get { return _locationsTable; }
		}

		public VisitLocationDetail CurrentVisitLocationSelection
		{
			get { return _currentVisitLocationSelection; }
			set
			{
				_currentVisitLocationSelection = value;
				VisitLocationSelectionChanged();
			}
		}

		public void SetSelectedVisitLocation(ISelection selection)
		{
			this.CurrentVisitLocationSelection = (VisitLocationDetail)selection.Item;
		}

		private void VisitLocationSelectionChanged()
		{
			if (_currentVisitLocationSelection != null)
			{
				_visitLocationActionHandler.Edit.Enabled = true;
				_visitLocationActionHandler.Delete.Enabled = true;
			}
			else
			{
				_visitLocationActionHandler.Edit.Enabled = false;
				_visitLocationActionHandler.Delete.Enabled = false;
			}
		}


		public ActionModelNode VisitLocationActionModel
		{
			get { return _visitLocationActionHandler; }
		}

		public void AddVisitLocation()
		{
			LoadVisitLocations();
			this.Modified = true;
		}

		public void UpdateSelectedVisitLocation()
		{
			LoadVisitLocations();
			this.Modified = true;
		}

		public void DeleteSelectedVisitLocation()
		{
			_visit.Locations.Remove(_currentVisitLocationSelection);

			LoadVisitLocations();
			this.Modified = true;
		}

		public void LoadVisitLocations()
		{
			_locationsTable.Items.Clear();
			_locationsTable.Items.AddRange(_visit.Locations);
		}
	}
}
