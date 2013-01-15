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

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an extension point for views onto a <see cref="LookupHandlerCellEditor"/>.
	/// </summary>
	[ExtensionPoint]
	public class LookupHandlerCellEditorViewExtensionPoint : ExtensionPoint<ITableCellEditorView>
	{
	}

	/// <summary>
	/// Implements a <see cref="ITableCellEditor"/> in terms of a <see cref="ILookupHandler"/>.
	/// </summary>
	[AssociateView(typeof(LookupHandlerCellEditorViewExtensionPoint))]
	public class LookupHandlerCellEditor : TableCellEditor
	{
		private readonly ILookupHandler _lookupHandler;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="lookupHandler"></param>
		public LookupHandlerCellEditor(ILookupHandler lookupHandler)
		{
			_lookupHandler = lookupHandler;
		}

		/// <summary>
		/// Gets the lookup handler associated with this cell editor.
		/// </summary>
		public ILookupHandler LookupHandler
		{
			get { return _lookupHandler; }
		}
	}
}
