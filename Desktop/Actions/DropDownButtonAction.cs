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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Models a toolbar item that has both a drop-down and a button, where each can behave independently.
	/// </summary>
	public class DropDownButtonAction : ButtonAction, IDropDownAction
	{
	    private Func<ActionModelNode> _menuModelDelegate;

	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionId">The fully qualified action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="flags">Flags that control the style of the action.</param>
		/// <param name="resolver">A resource resolver that will be used to resolve text and image resources.</param>
		public DropDownButtonAction(string actionId, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
			: base(actionId, path, flags, resolver)
		{
		}

		#region IDropDownButtonAction Members

		/// <summary>
		/// Gets the menu model for the dropdown.
		/// </summary>
		public ActionModelNode DropDownMenuModel
		{
			get { return _menuModelDelegate(); }
		}

		#endregion

        public void SetMenuModelDelegate(Func<ActionModelNode> menuModelDelegate)
		{
			_menuModelDelegate = menuModelDelegate;
		}
	}
}
