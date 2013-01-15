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
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class CannedTextInplaceLookupControl : UserControl
    {
        private ICannedTextLookupHandler _lookupHandler;

        private event EventHandler _committed;
        private event EventHandler _cancelled;

        public CannedTextInplaceLookupControl(ICannedTextLookupHandler lookupHandler)
        {
            InitializeComponent();
            _lookupHandler = lookupHandler;
            _suggestBox.SuggestionProvider = _lookupHandler.SuggestionProvider;
        }

        public event EventHandler Committed
        {
            add { _committed += value; }
            remove { _committed -= value; }
        }

        public event EventHandler Cancelled
        {
            add { _cancelled += value; }
            remove { _cancelled -= value; }
        }

        public object Value
        {
            get { return _suggestBox.Value; }
            set { _suggestBox.Value = value; }
        }

        public event EventHandler ValueChanged
        {
            add { _suggestBox.ValueChanged += value; }
            remove { _suggestBox.ValueChanged -= value; }
        }

        private void _suggestBox_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = _lookupHandler.FormatItem(e.ListItem);
        }

        private void _findButton_Click(object sender, EventArgs e)
        {
            try
            {
                object result;
                bool resolved = _lookupHandler.Resolve(_suggestBox.Text, true, out result);
                if (resolved)
                {
                    _suggestBox.Value = result;
                }
            }
            catch (Exception ex)
            {
                // not much we can do here if Resolve throws an exception
                Platform.Log(LogLevel.Error, ex);
            }
        }

        private void _suggestBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    EventsHelper.Fire(_committed, this, EventArgs.Empty);
                    break;
                case Keys.Escape:
                    EventsHelper.Fire(_cancelled, this, EventArgs.Empty);
                    break;
            }
        }

        private void _suggestBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
    }
}
