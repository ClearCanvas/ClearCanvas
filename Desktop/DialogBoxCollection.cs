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
    /// Represents the collection of <see cref="DialogBox"/> objects owned by a desktop window.
    /// </summary>
    internal class DialogBoxCollection : DesktopObjectCollection<DialogBox>
    {
        private DesktopWindow _owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The <see cref="DesktopWindow"/> that owns the dialog box.</param>
        internal DialogBoxCollection(DesktopWindow owner)
		{
            _owner = owner;
		}

        /// <summary>
        /// Creates a new dialog box with the specified arguments.
        /// </summary>
        internal DialogBox AddNew(DialogBoxCreationArgs args)
        {
            DialogBox dialog = CreateDialogBox(args);
            Open(dialog);
            return dialog;
        }

        /// <summary>
        /// Creates a new <see cref="DialogBox"/>.
        /// </summary>
        private DialogBox CreateDialogBox(DialogBoxCreationArgs args)
        {
            IDialogBoxFactory factory = CollectionUtils.FirstElement<IDialogBoxFactory>(
                (new DialogBoxFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDialogBoxFactory();

            return factory.CreateDialogBox(args, _owner);
        }

    }
}
