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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Extension point for views of the <see cref="LayoutChangerAction"/>.
	/// </summary>
	[ExtensionPoint]
	public class LayoutChangerActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	/// <summary>
	/// A custom action that allows the user to select a layout size.
	/// </summary>
	[AssociateView(typeof (LayoutChangerActionViewExtensionPoint))]
	public class LayoutChangerAction : Action
	{
		private readonly SetLayoutCallback _setLayoutCallback;
		private readonly int _maxRows;
		private readonly int _maxColumns;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="maxRows">The maximum number of rows that the user can select.</param>
		/// <param name="maxColumns">The maximum number of columns that the user can select.</param>
		/// <param name="callback">A <see cref="SetLayoutCallback"/> delegate that will be called when the user selects a layout size.</param>
		/// <param name="path">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve icons associated with this action.</param>
		public LayoutChangerAction(string actionID, int maxRows, int maxColumns, SetLayoutCallback callback, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{
			Platform.CheckForNullReference(callback, "callback");

			base.Label = path.LastSegment.LocalizedText;
			_setLayoutCallback = callback;
			_maxRows = maxRows;
			_maxColumns = maxColumns;
		}

		/// <summary>
		/// Gets the maximum number of rows that the user can select.
		/// </summary>
		public int MaxRows
		{
			get { return _maxRows; }
		}

		/// <summary>
		/// Gets the maximum number of columns that the user can select.
		/// </summary>
		public int MaxColumns
		{
			get { return _maxColumns; }
		}

		/// <summary>
		/// The method called by the view to set the user-selected layout size.
		/// </summary>
		/// <remarks>
		/// This method invokes the callback delegate provided in the
		/// <see cref="LayoutChangerAction(string, int, int, SetLayoutCallback, ActionPath, IResourceResolver)"/> constructor.
		/// </remarks>
		/// <param name="rows">The number of rows that the user selected.</param>
		/// <param name="columns">The number of columns that the user selected.</param>
		public void SetLayout(int rows, int columns)
		{
			_setLayoutCallback(rows, columns);
		}
	}

	/// <summary>
	/// Callback method to set the layout size.
	/// </summary>
	/// <param name="rows">The number of rows that the user selected.</param>
	/// <param name="columns">The number of columns that the user selected.</param>
	public delegate void SetLayoutCallback(int rows, int columns);
}