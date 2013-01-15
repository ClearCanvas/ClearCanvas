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
using System.Windows.Forms;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class HtmlComponentControl : CustomUserControl
    {
        private IApplicationComponent _component;
        private ActiveTemplate _template;
        private string _cachedHtml;


        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlComponentControl(IApplicationComponent component, ActiveTemplate template)
        {
            InitializeComponent();

            _component = component;
            _template = template;
#if DEBUG
            _webBrowser.IsWebBrowserContextMenuEnabled = true;
#else
            _webBrowser.IsWebBrowserContextMenuEnabled = false;
#endif

            _component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
            this.Disposed += new EventHandler(DisposedEventHandler);
            ReloadPage();
        }

        public event WebBrowserNavigatingEventHandler Navigating
        {
            add { _webBrowser.Navigating += value; }
            remove { _webBrowser.Navigating -= value; }
        }

        internal void ReloadPage()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["Component"] = _component;
            _cachedHtml = _template.Evaluate(context);

            if (this.Visible)
            {
                _webBrowser.DocumentText = _cachedHtml;
            }
        }

        private void DisposedEventHandler(object sender, EventArgs e)
        {
            _component.AllPropertiesChanged -= AllPropertiesChangedEventHandler;
        }

        private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
        {
            ReloadPage();
        }

        private void _initialRefreshTimer_Tick(object sender, EventArgs e)
        {
            _initialRefreshTimer.Enabled = false;
            _webBrowser.DocumentText = _cachedHtml;
        }

        private void HtmlComponentControl_Load(object sender, EventArgs e)
        {
        }

        private void HtmlComponentControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                _initialRefreshTimer.Enabled = true;
            }
        }
    }
}
