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
using System.Collections;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	[MenuAction("show", "global-menus/MenuView/MenuAlertViewer", "Show")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class AlertViewerTool : Tool<IDesktopToolContext>
	{
		private const string ShelfName = "DesktopAlertViewerShelf";
		private Shelf _shelf;

		/// <summary>
		/// Show the alert viewer shelf.
		/// </summary>
		public void Show()
		{
			if(_shelf == null)
			{
				_shelf = ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					new AlertViewerComponent(),
					SR.TitleAlertViewer,
					ShelfName,
					ShelfDisplayHint.DockRight|ShelfDisplayHint.DockAutoHide);
				_shelf.Closed += (sender, args) => _shelf = null;
			}
			else
			{
				_shelf.Activate();
			}
		}
	}
	
	/// <summary>
    /// Extension point for views onto <see cref="AlertViewerComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class AlertViewerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// A test component not intended for production use.
	/// </summary>
	[AssociateView(typeof (AlertViewerComponentViewExtensionPoint))]
	public class AlertViewerComponent : ApplicationComponent
	{
		enum Filters
		{
			All,
			ErrorsAndWarnings,
			Errors
		}


		private readonly Table<Alert> _alertTable;
		private readonly AlertLog _log = AlertLog.Instance;
		private Filters _filter;

		private readonly SimpleActionModel _alertActions;

		public AlertViewerComponent()
		{
			_alertTable = new Table<Alert>();
			_alertActions = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
		}

		public override void Start()
		{
			base.Start();

			_alertTable.Columns.Add(new TableColumn<Alert, IconSet>(SR.ColumnLevel, GetAlertIcon, 0.1f));
			_alertTable.Columns.Add(new TableColumn<Alert, string>(SR.ColumnMessage, a => a.Message, 0.9f));
			_alertTable.Columns.Add(new TableColumn<Alert, string>(SR.ColumnTime, a => Format.DateTime(a.Time), 0.4f));

			Refresh();

			_log.AlertLogged += AlertLoggedEventHandler;
		}

		public override void Stop()
		{
			_log.AlertLogged -= AlertLoggedEventHandler;

			base.Stop();
		}

		#region Presentation Model

		public ActionModelNode AlertActions
		{
			get { return _alertActions; }
		}

		public ITable Alerts
		{
			get { return _alertTable; }
		}

		public IList FilterChoices
		{
			get { return Enum.GetValues(typeof(Filters)); }
		}

		public string FormatFilter(object item)
		{
			var filter = (Filters) item;
			switch (filter)
			{
				case Filters.All:
					return SR.AlertFilterAll;
				case Filters.ErrorsAndWarnings:
					return SR.AlertFilterErrorsAndWarnings;
				case Filters.Errors:
					return SR.AlertFilterErrorsOnly;
			}
			throw new ArgumentOutOfRangeException();
		}

		public object Filter
		{
			get { return _filter; }
			set
			{
				var f = (Filters) value;
				if(f != _filter)
				{
					_filter = f;
					NotifyPropertyChanged("Filter");
					Refresh();
				}
			}
		}

		#endregion

		private void Refresh()
		{
			_alertTable.Items.Clear();
			_alertTable.Items.AddRange(_log.Entries.Where(Include).Reverse());
		}

		private bool Include(Alert alert)
		{
			return _filter == Filters.All
			       || (_filter == Filters.ErrorsAndWarnings && alert.Level != AlertLevel.Info)
			       || (_filter == Filters.Errors && alert.Level == AlertLevel.Error);
		}

		private void AlertLoggedEventHandler(object sender, ItemEventArgs<Alert> e)
		{
			_alertTable.Items.Insert(0, e.Item);
		}

		private static IconSet GetAlertIcon(Alert alert)
		{
			return alert.Level.GetIcon();
		}
	}
}
