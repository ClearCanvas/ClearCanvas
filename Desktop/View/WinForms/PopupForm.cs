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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using vbAccelerator.Components.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Form that acts as pop-up window, similar to a calendar popup or an intellisense dropdown, that can host an arbitary
    /// content control.
    /// </summary>
    public partial class PopupForm : Form
    {
        private Control _popupOwner;
        private Point _location;
        private PopupWindowHelper _helper;

        private event EventHandler _popupClosed;
        private event CancelEventHandler _popupCancelled;

        /// <summary>
        /// Constructor that creates a new popup form with specified content, owner, and location.
        /// </summary>
        /// <param name="content">The content to show in the popup</param>
        /// <param name="popupOwner">The control that owns the popup.</param>
        /// <param name="location">The location of the popup, in screen coordinates.</param>
        public PopupForm(Control content, Control popupOwner, Point location)
        {
            InitializeComponent();

            _popupOwner = popupOwner;
            _location = location;

            _helper = new PopupWindowHelper();
            _helper.PopupClosed += delegate
            {
                if (_popupClosed != null)
                    _popupClosed(this, EventArgs.Empty);
            };
            _helper.PopupCancel += delegate(object sender, PopupCancelEventArgs e)
            {
                if (_popupCancelled != null)
                {
                    CancelEventArgs c = new CancelEventArgs(e.Cancel);
                    _popupCancelled(this, c);
                    e.Cancel = c.Cancel;
                }
            };

            // size the content appropriately, and size this form to match the content
            content.MinimumSize = content.Size;
            content.Dock = DockStyle.Fill;
            this.ClientSize = content.Size;

            // add content to form
            this.Controls.Add(content);
        }

        /// <summary>
        /// Occurs when the popup has closed.
        /// </summary>
        public event EventHandler PopupClosed
        {
            add { _popupClosed += value; }
            remove { _popupClosed -= value; }
        }

        /// <summary>
        /// Occurs when the popup is about to close, because the user has clicked outside of it.
        /// </summary>
        public event CancelEventHandler PopupCancelled
        {
            add { _popupCancelled += value; }
            remove { _popupCancelled -= value; }
        }

        /// <summary>
        /// Shows the popup on the screen.  This method can only be called once in the lifetime of this object.
        /// </summary>
        public void ShowPopup()
        {
            Form owner = GetRootForm(_popupOwner);
            _helper.AssignHandle(owner.Handle);
            _helper.ShowPopup(owner, this, _location);
        }

        /// <summary>
        /// Programatically closes the popup.  It is not usually necessary to call this method.
        /// </summary>
        public void ClosePopup()
        {
            _helper.ClosePopup();
        }

        private Form GetRootForm(Control control)
        {
            if (control == null)
                return null;
            else if (control is Form)
                return control as Form;
            else
                return GetRootForm(control.Parent);
        }
    }
}