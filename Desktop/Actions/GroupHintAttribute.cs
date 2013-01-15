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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Declares a 'group hint' for an action.
	/// </summary>
	/// <remarks>
	/// Group Hints are used to determine as appropriate a place 
	/// as possible to place an action within an action model.
	/// </remarks>
	public class GroupHintAttribute : ActionDecoratorAttribute
	{
		private readonly string _groupHint;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The logical Id of the action.</param>
        /// <param name="groupHint">The action's group hint.</param>
		public GroupHintAttribute(string actionID, string groupHint)
			:base(actionID)
		{
			if (groupHint == null)
				groupHint = "";

			_groupHint = groupHint;
		}

		/// <summary>
		/// Sets the <see cref="IAction"/>'s <see cref="GroupHint"/>, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
        public override void Apply(IActionBuildingContext builder)
		{
			builder.Action.GroupHint = new GroupHint(_groupHint);
		}
	}
}
