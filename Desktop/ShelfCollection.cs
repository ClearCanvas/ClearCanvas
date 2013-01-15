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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="Shelf"/> objects for a desktop window.
    /// </summary>
    public sealed class ShelfCollection : DesktopObjectCollection<Shelf>
    {
        private DesktopWindow _owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ShelfCollection(DesktopWindow owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Opens a new shelf.
        /// </summary>
        /// <param name="component">The <see cref="IApplicationComponent"/> that is to be hosted in the returned <see cref="Shelf"/>.</param>
        /// <param name="title">The title of the <see cref="Shelf"/>.</param>
        /// <param name="displayHint">A hint for how the <see cref="Shelf"/> should be initially displayed.</param>
        public Shelf AddNew(IApplicationComponent component, string title, ShelfDisplayHint displayHint)
        {
            return AddNew(component, title, null, displayHint);
        }

		/// <summary>
		/// Opens a new shelf.
		/// </summary>
		/// <param name="component">The <see cref="IApplicationComponent"/> that is to be hosted in the returned <see cref="Shelf"/>.</param>
		/// <param name="title">The title of the <see cref="Shelf"/>.</param>
		/// <param name="name">A name/identifier for the <see cref="Shelf"/>.</param>
		/// <param name="displayHint">A hint for how the <see cref="Shelf"/> should be initially displayed.</param>
		public Shelf AddNew(IApplicationComponent component, string title, string name, ShelfDisplayHint displayHint)
        {
            return AddNew(new ShelfCreationArgs(component, title, name, displayHint));
        }

        /// <summary>
        /// Opens a new shelf given the input <see cref="ShelfCreationArgs"/>.
        /// </summary>
        public Shelf AddNew(ShelfCreationArgs args)
        {
            Shelf shelf = CreateShelf(args);
            Open(shelf);
            return shelf;
        }

        /// <summary>
        /// Creates a new shelf.
        /// </summary>
        private Shelf CreateShelf(ShelfCreationArgs args)
        {
            IShelfFactory factory = CollectionUtils.FirstElement<IShelfFactory>(
                (new ShelfFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultShelfFactory();

            return factory.CreateShelf(args, _owner);
        }
    }
}
