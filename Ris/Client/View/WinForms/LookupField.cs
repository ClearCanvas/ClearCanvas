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
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// A type of field that allows the user to select from a potentially large list of items.
    /// </summary>
    /// <remarks>
    /// This type of field is useful when the list of items to choose from is too large to fit into a
    /// regular drop-down.  This type of field allows the user to choose an item by entering a text query,
    /// such as the first few letters of a name, and then clicking a button to launch an auxilliary dialog
    /// to resolve the query.  Alternatively, the user may resolve the query from a dynamically generated
    /// list of suggested items (this field makes use of the <see cref="SuggestComboField"/>).  The
    /// <see cref="LookupHandler"/> property must be set, as all user-interaction with the control will
    /// be directed to an underlying <see cref="ILookupHandler"/>.
    /// </remarks>
    public partial class LookupField : UserControl
    {
        private ILookupHandler _lookupHandler;

        public LookupField()
        {
            InitializeComponent();
        }

        #region Design-time properties

		[Localizable(true)]
        public string LabelText
        {
            get { return _inputField.LabelText; }
            set { _inputField.LabelText = value; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        [Browsable(false)]
        public object Value
        {
            get { return _inputField.Value; }
            set { _inputField.Value = value; }
        }

        /// <summary>
        /// Occurs when the <see cref="Value"/> property changes.
        /// </summary>
        [Browsable(false)]
        public event EventHandler ValueChanged
        {
            // use pass through event subscription
            add { _inputField.ValueChanged += value; }
            remove { _inputField.ValueChanged -= value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ILookupHandler"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILookupHandler LookupHandler
        {
            get { return _lookupHandler; }
            set
            {
                _lookupHandler = value;
                _inputField.SuggestionProvider = _lookupHandler.SuggestionProvider;
            }
        }

        private void _findButton_Click(object sender, EventArgs e)
        {
            try
            {
                object result;
                bool resolved = _lookupHandler.Resolve(_inputField.QueryText, true, out result);
                if (resolved)
                {
                    this.Value = result;
                }
            }
            catch (Exception ex)
            {
                // not much we can do here if Resolve throws an exception
                Platform.Log(LogLevel.Error, ex);
            }
        }

        private void _inputField_Format(object sender, ListControlConvertEventArgs e)
        {
            try
            {
                e.Value = _lookupHandler.FormatItem(e.ListItem);
            }
            catch (Exception ex)
            {
                // not much we can do here if FormatItem throws an exception
                Platform.Log(LogLevel.Error, ex);
            }
        }
    }
}
