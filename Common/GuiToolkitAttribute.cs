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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Attribute used to mark a class as using a specific GUI toolkit.
    /// </summary>
    /// <remarks>
	/// Typically this attribute is used on an extension class (in addition to the <see cref="ExtensionOfAttribute"/>) 
	/// to allow plugin code to determine at runtime if the given extension is compatible with the GUI toolkit
	/// that is currently in use by the main window.
	/// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class GuiToolkitAttribute : Attribute
    {
        private string _toolkitID;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="toolkitID">A string identifier for the Gui Toolkit.</param>
		public GuiToolkitAttribute(string toolkitID)
        {
            _toolkitID = toolkitID;
        }

		/// <summary>
		/// Gets the Gui Toolkit ID.
		/// </summary>
        public string ToolkitID
        {
            get { return _toolkitID; }
        }

		/// <summary>
		/// Determines whether or not this attribute is a match for (or is the same as) <paramref name="obj"/>,
		/// which is itself an <see cref="Attribute"/>.
		/// </summary>
        public override bool Match(object obj)
        {
            if (obj != null && obj is GuiToolkitAttribute)
            {
                return (obj as GuiToolkitAttribute).ToolkitID == this.ToolkitID;
            }
            else
            {
                return false;
            }
        }
    }
}
