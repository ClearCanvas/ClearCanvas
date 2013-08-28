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
    /// Declares an observer binding for the checked state of an action.
    /// </summary>
    /// <remarks>
    /// This attribute causes the checked state of the action specified by the action ID to be
    /// bound to the state of the specified property on the class to which this attribute applies.
    /// The property name must refer to a public boolean property on the target class that has get access.
    /// The change event name must refer to a public event on the class that will fire whenever the state of the property
    /// changes.
    /// </remarks>
    public class CheckedStateObserverAttribute : StateObserverAttribute
    {
        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to which this attribute applies.</param>
        /// <param name="propertyName">The name of the property to bind to.</param>
        /// <param name="changeEventName">The name of the property change notification event to bind to.</param>
        public CheckedStateObserverAttribute(string actionID, string propertyName, string changeEventName)
            : base(actionID, propertyName, changeEventName)
        {
        }

		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="actionID">The logical action identifier to which this attribute applies.</param>
		/// <param name="propertyName">The name of the property to bind to.</param>
		public CheckedStateObserverAttribute(string actionID, string propertyName)
			: base(actionID, propertyName)
		{
		}

		/// <summary>
		/// Binds the <see cref="IClickAction.Checked"/> property and <see cref="IClickAction.CheckedChanged"/> event 
		/// to the corresponding items on the target object, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public override void Apply(IActionBuildingContext builder)
        {
            Bind<bool>(builder, "Checked", "CheckedChanged");
        }
    }
}
