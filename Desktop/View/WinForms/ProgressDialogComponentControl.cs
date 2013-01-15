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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProgressDialogComponent"/>
    /// </summary>
    public partial class ProgressDialogComponentControl : ApplicationComponentUserControl
    {
        private ProgressDialogComponent _component;
        private int _defaultProgressBarWidth;
        private bool _cancelButtonOriginallyVisible;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressDialogComponentControl(ProgressDialogComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            base.CancelButton = _cancelButton;
            base.AcceptButton = _cancelButton;

            _cancelButton.Visible = _component.ShowCancel;
            _cancelButton.Text = _component.ButtonText;
            _message.Text = _component.ProgressMessage;
            _progressBar.Value = _component.ProgressBar;
            _progressBar.MarqueeAnimationSpeed = _component.MarqueeSpeed;
            _progressBar.Style = (System.Windows.Forms.ProgressBarStyle)_component.ProgressBarStyle;
            _progressBar.Maximum = _component.ProgressBarMaximum;

            _component.ProgressUpdateEvent += OnProgressUpdate;
            _component.ProgressTerminateEvent += OnProgressTerminate;

            _cancelButtonOriginallyVisible = _cancelButton.Visible;
            _defaultProgressBarWidth = _progressBar.Width;
            UpdateProgressBarLength();
        }

        ~ProgressDialogComponentControl()
        {
            _component.ProgressUpdateEvent -= OnProgressUpdate;
            _component.ProgressTerminateEvent -= OnProgressTerminate;
        }

        private void OnProgressUpdate(object sender, EventArgs e)
        {
            _message.Text = _component.ProgressMessage;
            _progressBar.Value = _component.ProgressBar;
            _progressBar.Style = (System.Windows.Forms.ProgressBarStyle)_component.ProgressBarStyle;
        }

        private void OnProgressTerminate(object sender, EventArgs e)
        {
            _cancelButton.Visible = _component.ShowCancel;
            _cancelButton.Text = _component.ButtonText;
            _message.Text = _component.ProgressMessage;
            _progressBar.Value = _component.ProgressBar;
            _progressBar.Style = (System.Windows.Forms.ProgressBarStyle)_component.ProgressBarStyle;
            _progressBar.MarqueeAnimationSpeed = _component.MarqueeSpeed;

            UpdateProgressBarLength();
        }

        private void UpdateProgressBarLength()
        {
            if (_cancelButton.Visible)
            {
                // Resotre progressBar width if the cancel button becomes visible
                if (!_cancelButtonOriginallyVisible)
                    _progressBar.Width = _defaultProgressBarWidth;
            }
            else
                _progressBar.Width = _cancelButton.Right - _progressBar.Left;
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _message_Enter(object sender, EventArgs e)
        {
            // Give the current focus to the next control upon entering, so the caret never shows up
            Control nextControl = Parent.GetNextControl(_message, true);
            if (nextControl != null)
                nextControl.Focus();
        }
    }
}
