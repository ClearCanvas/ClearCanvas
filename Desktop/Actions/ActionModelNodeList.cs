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

using System.Collections.Generic;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Used by <see cref="ActionModelNode"/> to hold the list of child nodes.
    /// </summary>
    public class ActionModelNodeList : List<ActionModelNode>
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ActionModelNodeList()
		{
		}

    	/// <summary>
        /// Returns the child node whose <see cref="ActionModelNode.PathSegment"/> 
        /// <see cref="ClearCanvas.Desktop.PathSegment.LocalizedText"/> property
        /// is equal to the specified value.
        /// </summary>
        /// <param name="name">The name of the node to retrieve.</param>
        /// <returns>The corresponding child node, or null if no such node exists.</returns>
        public ActionModelNode this[string name]
        {
            get
            {
                foreach (ActionModelNode node in this)
                {
                    // define node equality in terms of the localized text
                    // (eg two menu items with the same name should be the same menu item, 
                    // even if a different resource key was used)
                    if (node.PathSegment.LocalizedText == name)
                        return node;
                }
                return null;
            }
        }
    }
}
