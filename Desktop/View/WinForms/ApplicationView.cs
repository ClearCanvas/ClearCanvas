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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IApplicationView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.
    /// Reasons for subclassing may include: overriding the <see cref="CreateDesktopWindowView"/>
    /// factory method to supply a custom subclasses of <see cref="DesktopWindowView"/>,
    /// and overriding <see cref="ShowMessageBox"/> to customize the display of message boxes.
    /// </para>
    /// </remarks>
    [ExtensionOf(typeof(ApplicationViewExtensionPoint))]
    public class ApplicationView : WinFormsView, IApplicationView
    {
        /// <summary>
        /// No-args constructor required by extension point framework.
        /// </summary>
        public ApplicationView()
        {
			System.Windows.Forms.Application.ThreadException += (sender, e) => ExceptionHandler.ReportUnhandled(e.Exception);
        }

        #region IApplicationView Members

        /// <summary>
        /// Creates a new view for the specified <see cref="DesktopWindow"/>.
        /// </summary>
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IDesktopWindowView"/>.
        /// In practice, it is preferable to subclass <see cref="DesktopWindowView"/> rather than implement <see cref="IDesktopWindowView"/>
        /// directly.
        /// </remarks>
        /// <param name="window"></param>
        /// <returns></returns>
        public virtual IDesktopWindowView CreateDesktopWindowView(DesktopWindow window)
        {
            return new DesktopWindowView(window);
        }

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <remarks>
        /// Override this method if you need to customize the display of message boxes.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, MessageBoxActions actions)
        {
            MessageBox mb = new MessageBox();
            return mb.Show(message, actions);
        }

        #endregion

        /// <summary>
        /// Not used by this class.
        /// </summary>
        public override object GuiElement
        {
            // not used
            get { throw new NotSupportedException(); }
        }
    }
}
