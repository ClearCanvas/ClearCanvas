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
using ClearCanvas.Desktop.Actions;
using Action=ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions
{
	[ExtensionPoint]
	public class ListFilterMenuActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	public interface IListFilterDataSource
	{
		IEnumerable<object> Values { get; }
		bool GetSelectedState(object value);
		void SetSelectedState(object value, bool selected);
		void SetAllSelectedState(bool selected);
	}

	[AssociateView(typeof (ListFilterMenuActionViewExtensionPoint))]
	public class ListFilterMenuAction : Action
	{
		private readonly IListFilterDataSource _dataSource;

		public ListFilterMenuAction(string actionID, ActionPath actionPath, IListFilterDataSource dataSource, IResourceResolver resourceResolver)
			: base(actionID, actionPath, resourceResolver)
		{
			Platform.CheckForNullReference(dataSource, "dataSource");
			_dataSource = dataSource;
		}

		public IListFilterDataSource DataSource
		{
			get { return _dataSource; }
		}

		public static ListFilterMenuAction CreateAction(Type callingType, string actionID, string actionPath, IListFilterDataSource dataSource, IResourceResolver resourceResolver)
		{
			ListFilterMenuAction action = new ListFilterMenuAction(
				string.Format("{0}:{1}", callingType.FullName, actionID),
				new ActionPath(actionPath, resourceResolver),
				dataSource, resourceResolver);
			action.Label = action.Path.LastSegment.LocalizedText;
			action.Persistent = true;
			return action;
		}
	}
}