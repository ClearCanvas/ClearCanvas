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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class ActiveMenuItem : ToolStripMenuItem
    {
        private IClickAction _action;
        private EventHandler _actionEnabledChangedHandler;
        private EventHandler _actionCheckedChangedHandler;
		private EventHandler _actionVisibleChangedHandler;
    	private EventHandler _actionAvailableChangedHandler;
		private EventHandler _actionLabelChangedHandler;
    	private EventHandler _actionIconSetChangedHandler;

		private IconSize _iconSize;

        public ActiveMenuItem(IClickAction action)
			: this(action, Desktop.IconSize.Small)
        {
        }

        public ActiveMenuItem(IClickAction action, IconSize iconSize)
        {
            _action = action;
			_iconSize = iconSize;
            _actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
            _actionCheckedChangedHandler = new EventHandler(OnActionCheckedChanged);
			_actionVisibleChangedHandler = new EventHandler(OnActionVisibleChanged);
			_actionAvailableChangedHandler = new EventHandler(OnActionAvailableChanged);
			_actionLabelChangedHandler = new EventHandler(OnActionLabelChanged);
			_actionIconSetChangedHandler = new EventHandler(OnActionIconSetChanged);

            _action.EnabledChanged += _actionEnabledChangedHandler;
            _action.CheckedChanged += _actionCheckedChangedHandler;
			_action.VisibleChanged += _actionVisibleChangedHandler;
        	_action.AvailableChanged += _actionAvailableChangedHandler;
			_action.LabelChanged += _actionLabelChangedHandler;
        	_action.IconSetChanged += _actionIconSetChangedHandler;

            this.Text = _action.Label;
            this.Checked = _action.Checked;

            UpdateVisibility();
            UpdateEnablement();
        	UpdateIcon();

            this.Click += delegate(object sender, EventArgs e)
            {
                _action.Click();
            };

            try
            {
                this.ShortcutKeys = (Keys)_action.KeyStroke;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
            }
        }

		public IconSize IconSize
		{
			get { return _iconSize; }
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					UpdateIcon();
				}
			}
		}

        private void OnActionCheckedChanged(object sender, EventArgs e)
        {
            this.Checked = _action.Checked;
        }

        private void OnActionEnabledChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

		private void OnActionVisibleChanged(object sender, EventArgs e)
		{
            UpdateVisibility();
		}

		private void OnActionAvailableChanged(object sender, EventArgs e)
		{
			UpdateEnablement();
			UpdateVisibility();
		}

		private void OnActionLabelChanged(object sender, EventArgs e)
		{
			this.Text = _action.Label;
		}

		private void OnActionIconSetChanged(object sender, EventArgs e)
		{
			UpdateIcon();
		}

    	protected override void Dispose(bool disposing)
        {
            if (disposing && _action != null)
            {
                // VERY IMPORTANT: instances of this class will be created and discarded frequently
                // throughout the lifetime of the application
                // therefore is it extremely important that the event handlers are disconnected
                // from the underlying _action events
                // otherwise, this object will hang around for the entire lifetime of the _action object,
                // even though this object is no longer needed
                _action.EnabledChanged -= _actionEnabledChangedHandler;
                _action.CheckedChanged -= _actionCheckedChangedHandler;
				_action.VisibleChanged -= _actionVisibleChangedHandler;
				_action.AvailableChanged -= _actionAvailableChangedHandler;
				_action.LabelChanged -= _actionLabelChangedHandler;
				_action.IconSetChanged -= _actionIconSetChangedHandler;

                _action = null;
            }
            base.Dispose(disposing);
        }

        private void UpdateVisibility()
        {
			base.Available = _action.Available && _action.Visible && (_action.Permissible || DesktopViewSettings.Default.ShowNonPermissibleActions);
        }

        private void UpdateEnablement()
        {
			this.Enabled = _action.Available && _action.Enabled && (_action.Permissible || DesktopViewSettings.Default.EnableNonPermissibleActions);
        }

		private void UpdateIcon()
		{
			ActionViewUtils.SetIcon(this, _action, _iconSize);
		}
    }
}
