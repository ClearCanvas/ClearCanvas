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
	/// Models a toolbar item that, when clicked, displays a menu containing other <see cref="IAction"/>s.
	/// </summary>
	/// <remarks>
	/// The <see cref="DropDownAction"/> is not itself an <see cref="IClickAction"/>, in that the action of
	/// clicking it is not customizable; it can only show the associated <see cref="DropDownMenuModel"/> items.
	/// </remarks>
	public class DropDownAction : Action, IDropDownAction
	{
        private Func<ActionModelNode> _menuModelDelegate;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve resources associated with this action.</param>
		public DropDownAction(string actionID, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{
		}

		#region IDropDownAction Members

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