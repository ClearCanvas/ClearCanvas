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
    /// Declares a tooltip message to associate with an action.
    /// </summary>
    public class TooltipAttribute : ActionDecoratorAttribute
    {
        private readonly string _tooltip;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to which this attribute applies.</param>
        /// <param name="tooltip">The tooltip message to associate with the action.</param>
        public TooltipAttribute(string actionID, string tooltip)
            :base(actionID)
        {
            _tooltip = tooltip;
        }

        /// <summary>
        /// The tooltip message.
        /// </summary>
        public string TooltipText { get { return _tooltip; } }

		/// <summary>
		/// Sets the <see cref="IAction.Tooltip"/> value for and <see cref="IAction"/> instance,
		/// via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
        public override void Apply(IActionBuildingContext builder)
        {
            // assert _action != null
            builder.Action.Tooltip = builder.ResourceResolver.LocalizeString(this.TooltipText);
        }
    }
}
