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

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Defines the interface to a tree, which provides a presentation model for viewing hierarchical data.
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// Obtains the <see cref="ITreeItemBinding"/> that defines how items in this tree are mapped to the view.
        /// </summary>
        ITreeItemBinding Binding { get; }

        /// <summary>
        /// Obtains a reference to the collection of items in this tree.
        /// </summary>
        /// <remarks>
		/// <para>
		/// Note that this collection contains only the immediate items.  Each 
		/// item may provide a sub-tree, which can be obtained via the
		/// <see cref="ITreeItemBinding.GetSubTree"/> method.
		/// </para>
		/// <para>
		/// In general, it is advisable that the implementation of the root <see cref="ITree"/> should encapsulate
		/// a single ancestor root tree item, whose <see cref="IItemCollection"/> is returned in <see cref="Items"/>.
		/// Because only the root's children are returned in this interface, the tree view will still show them
		/// as "top-level" nodes, but they will still be related to each other through a common ancestor item.
		/// This is important, because a number of <see cref="ITree"/> features, such as check states, reordering,
		/// and view updates triggered from the model side depend on the existence of a parent node.
		/// </para>
		/// </remarks>
        IItemCollection Items { get; }
    }
}
