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
using System.ComponentModel;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// This class should be used the base class for all user controls, rather than directly inheriting
    /// from <see cref="UserControl"/>.  Provides a mechanism for handling default Accept and Cancel buttons.
    /// </summary>
    public class CustomUserControl : LocalizableUserControl
    {
        // N.B. do not make this class abstract, no matter how tempting it may look. You will break the VS Forms designer.

        private IButtonControl _acceptButton;
        private IButtonControl _cancelButton;

        /// <summary>
        /// Gets or sets the button that will be clicked when the Enter key is pressed
        /// </summary>
        public IButtonControl AcceptButton
        {
            get { return _acceptButton; }
            set { _acceptButton = value; }
        }

        /// <summary>
        /// Gets or sets the button that will be clicked when the Escape key is pressed
        /// </summary>
        public IButtonControl CancelButton
        {
            get { return _cancelButton; }
            set { _cancelButton = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ApplicationComponentUserControl"/> is currently in design mode.
        /// </summary>
        /// <remarks>
        /// This implementation solves the problem where <see cref="Component.DesignMode"/> property does not work when called in the control's constructor.
        /// </remarks>
        protected new bool DesignMode
        {
            get { return base.DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime; }
        }

        /// <summary>
        /// Overridden in order to subscribe to the <see cref="Control.Enter"/> event
        /// on all child controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            foreach (Control child in this.Controls)
            {
                child.Enter += ChildEnterEventHandler;
            }

            base.OnEnter(e);
        }

        /// <summary>
        /// Overridden in order to unsubscribe from the <see cref="Control.Enter"/> event
        /// on all child controls and hide the default button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLeave(EventArgs e)
        {
            foreach (Control child in this.Controls)
            {
                child.Enter -= ChildEnterEventHandler;
            }

            // hide the default button
            ShowDefaultButtonUICue(false);

            base.OnLeave(e);
        }

        /// <summary>
        /// Overridden to process Enter and Escape keys
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
			if (base.ProcessDialogKey(keyData))
				return true;

			//if none of the other controls handled it using default processing, 
			// then try our Accept and Cancel buttons, if they are assigned.
			if (keyData == Keys.Return && _acceptButton != null)
			{
					_acceptButton.PerformClick();
					return true;    // handled
			}
			else if (keyData == Keys.Escape && _cancelButton != null)
			{
					_cancelButton.PerformClick();
					return true;    // handled
			}

			return false;
        }

        /// <summary>
        /// Whenever the focused child control changes, the default button UI cues
        /// may need to be updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildEnterEventHandler(object sender, EventArgs e)
        {
            // show the default button iff the active control is not a button
            bool show = !(this.ActiveControl is IButtonControl);
            ShowDefaultButtonUICue(show);
        }

        /// <summary>
        /// Shows the "accept" button with a default button UI cue, if true
        /// </summary>
        /// <param name="show">True to show the UI cue, false to hide it</param>
        private void ShowDefaultButtonUICue(bool show)
        {
            if (_acceptButton != null)
            {
                _acceptButton.NotifyDefault(show);
            }
        }
    }
}
