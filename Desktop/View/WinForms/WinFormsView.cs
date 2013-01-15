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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Abstract base class for all WinForms-based views.  Any class that implements a view using
    /// WinForms as the underlying GUI toolkit should subclass this class.
    /// </summary>
    [GuiToolkit(ClearCanvas.Common.GuiToolkitID.WinForms)]
    public abstract class WinFormsView
    {
        protected WinFormsView()
        {
        }

        /// <summary>
        /// Gets the toolkit ID, which is always <see cref="ClearCanvas.Common.GuiToolkitID.WinForms"/>.
        /// </summary>
        public string GuiToolkitID
        {
            get { return ClearCanvas.Common.GuiToolkitID.WinForms; }
        }

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.Control"/> that implements this view, allowing
        /// a parent view to insert the control as one of its children.
        /// </summary>
        public abstract object GuiElement
        {
            get;
        }
	}
}