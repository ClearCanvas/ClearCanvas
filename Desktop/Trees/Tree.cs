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

using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// A useful generic implementation of <see cref="ITree"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class Tree<TItem> : ITree
    {
        private ITreeItemBinding _binding;
        private ItemCollection<TItem> _items;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="binding">The tree item binding.</param>
        public Tree(ITreeItemBinding binding)
            :this(binding, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binding">The tree item binding.</param>
        /// <param name="items">The set of items that are initially contained in this tree.</param>
        public Tree(ITreeItemBinding binding, IEnumerable<TItem> items)
        {
            _binding = binding;
            _items = new ItemCollection<TItem>();
            if (items != null)
            {
                _items.AddRange(items);
            }
        }

        /// <summary>
		/// Gets the <see cref="IItemCollection{TItem}"/> associated with this tree.
        /// </summary>
        public ItemCollection<TItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the item binding associated with this tree.
        /// </summary>
        public ITreeItemBinding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        #region ITree Members

		/// <summary>
		/// Gets the <see cref="IItemCollection"/> associated with this tree.
		/// </summary>
		IItemCollection ITree.Items
        {
            get { return _items; }
        }

        #endregion
    }
}
