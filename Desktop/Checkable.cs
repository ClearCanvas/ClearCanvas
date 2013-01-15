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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Wrapper class for items that are "checkable".
	/// </summary>
	/// <typeparam name="TItem">The type of the checkable item.</typeparam>
    public class Checkable<TItem>
    {
        private bool _isChecked;
        private TItem _item;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The checkable item.</param>
		/// <param name="isChecked">The initial check state of the item.</param>
        public Checkable(TItem item, bool isChecked)
        {
            _isChecked = isChecked;
            _item = item;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// The initial check state is false by default.
		/// </remarks>
		/// <param name="item">The checkable item.</param>
        public Checkable(TItem item)
            : this(item, false)
        {
        }

		/// <summary>
		/// Gets or sets the checkable item.
		/// </summary>
        public TItem Item
        {
            get { return _item; }
        }

		/// <summary>
		/// Gets or sets the check state of the item.
		/// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }
    }
}
