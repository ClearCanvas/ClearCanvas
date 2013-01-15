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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a button action with the specifed action identifier and path hint.
    /// </summary>
    public class ButtonActionAttribute : ClickActionAttribute
    {
        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model.</param>
        public ButtonActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint)
        {
        }

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model.</param>
        /// <param name="clickHandler">Name of the method that will be invoked when the button is clicked.</param>
        public ButtonActionAttribute(string actionID, string pathHint, string clickHandler)
            : base(actionID, pathHint, clickHandler)
        {
        }

        /// <summary>
        /// Factory method to instantiate the action.
        /// </summary>
		/// <param name="actionID">The logical action identifier to associate with this action.</param>
        /// <param name="path">The path to the action in the toolbar model.</param>
        /// <param name="flags">Flags specifying how the button should respond to being clicked.</param>
        /// <param name="resolver">The action resource resolver used to resolve the action path and icons.</param>
        /// <returns>A <see cref="ClickAction"/>.</returns>
        protected override ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
        {
            return new ButtonAction(actionID, path, flags, resolver);
        }
    }
}
