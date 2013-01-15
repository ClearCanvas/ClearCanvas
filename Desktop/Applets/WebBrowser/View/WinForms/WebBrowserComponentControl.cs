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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Applets.WebBrowser;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Applets.WebBrowser.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BrowserComponent"/>
    /// </summary>
    public partial class WebBrowserComponentControl : ApplicationComponentUserControl
    {
        private WebBrowserComponent _component;
		private ActionModelNode _toolbarModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public WebBrowserComponentControl(WebBrowserComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			// Build the shortcut toolbar
			this.ToolbarModel = _component.ToolbarModel;

			_back.Enabled = _browser.CanGoBack;
			_forward.Enabled = _browser.CanGoForward;

			ListenForComponentEvents();
			ListenForUIEvents();
			ListenForBrowserEvents();
        }

		private ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
			set
			{
				_toolbarModel = value;
				ToolStripBuilder.Clear(_shortcutToolbar.Items);
				
				if (_toolbarModel != null)
				{
					// Use the toolbar model in the component to build the toolbar
					ToolStripBuilder.ToolStripBuilderStyle style = new ToolStripBuilder.ToolStripBuilderStyle(
						ToolStripItemDisplayStyle.ImageAndText,
						ToolStripItemAlignment.Left,
						TextImageRelation.ImageBeforeText);
					ToolStripBuilder.BuildToolbar(_shortcutToolbar.Items, _toolbarModel.ChildNodes, style);

					foreach (ToolStripItem item in _shortcutToolbar.Items)
					{
						item.TextImageRelation = TextImageRelation.ImageBeforeText;
						item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
					}
				}
			}
		}

		private void ListenForComponentEvents()
		{
			_component.UrlChanged +=
				delegate(object sender, EventArgs e) { _address.Text = _component.Url; };
			_component.GoInvoked +=
				delegate(object sender, EventArgs e)
				{
					_component.Url = _address.Text;
					_browser.Navigate(_component.Url);
				};
			_component.BackInvoked +=
				delegate(object sender, EventArgs e) { _browser.GoBack(); };
			_component.ForwardInvoked +=
				delegate(object sender, EventArgs e) { _browser.GoForward(); };
			_component.RefreshInvoked +=
				delegate(object sender, EventArgs e) { _browser.Refresh(); };
			_component.CancelInvoked +=
				delegate(object sender, EventArgs e) { _browser.Stop(); };
		}

		private void ListenForUIEvents()
		{
			_address.TextChanged +=
				delegate(object sender, EventArgs e) { _component.Url = _address.Text; };

			_address.KeyDown +=
				delegate(object sender, KeyEventArgs e)
				{
					if (e.KeyCode == Keys.Return)
						_component.Go();
				};

			_back.Click +=
				delegate(object sender, EventArgs e) { _component.Back(); };
			_forward.Click +=
				delegate(object sender, EventArgs e) { _component.Forward(); };
			_refresh.Click +=
				delegate(object sender, EventArgs e) { _component.Refresh(); };
			_stop.Click +=
				delegate(object sender, EventArgs e) { _component.Cancel(); };
			_go.Click +=
				delegate(object sender, EventArgs e) { _component.Go(); };
		}

		private void ListenForBrowserEvents()
		{
			_browser.CanGoBackChanged +=
				delegate(object sender, EventArgs e) { _back.Enabled = _browser.CanGoBack; };
			_browser.CanGoForwardChanged +=
				delegate(object sender, EventArgs e) { _forward.Enabled = _browser.CanGoForward; };
			_browser.DocumentTitleChanged +=
				delegate(object sender, EventArgs e) 
				{ 
					_component.SetDocumentTitle(_browser.DocumentTitle);
					_component.Url = _browser.Url.ToString();
				};
			_browser.ProgressChanged +=
				delegate(object sender, WebBrowserProgressChangedEventArgs e)
				{
					_browserProgress.Maximum = (int)e.MaximumProgress;
					_browserProgress.Value = (int)e.CurrentProgress;
				};
			_browser.StatusTextChanged +=
				delegate(object sender, EventArgs e) { _browserStatus.Text = _browser.StatusText; };
		}

    }
}
