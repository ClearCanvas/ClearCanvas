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
using System.ComponentModel;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// A type of field that allows the user to select an item from a list of suggested items
    /// that is provided dynamically from a <see cref="ISuggestionProvider"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="SuggestionProvider"/> property must be set.  Also, the <see cref="Format"/> event
    /// should be handled to correctly format items for display.
    /// </remarks>
    public partial class SuggestComboField : UserControl
    {
        public SuggestComboField()
        {
            InitializeComponent();
        }

        #region Design-time properties and events

        /// <summary>
        /// Gets or sets the associated label text.
        /// </summary>
		[Localizable(true)]
        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        /// <summary>
        /// Occurs to allow formatting of the item for display in the user-interface.
        /// </summary>
        public event ListControlConvertEventHandler Format
        {
            add { _comboBox.Format += value; }
            remove { _comboBox.Format -= value; }
        }

        #endregion

        [Browsable(false)]
        public object Value
        {
            get { return _comboBox.Value; }
            set { _comboBox.Value = value; }
        }

        [Browsable(false)]
        public event EventHandler ValueChanged
        {
            // use pass through event subscription
            add { _comboBox.ValueChanged += value; }
            remove { _comboBox.ValueChanged -= value; }
        }

        /// <summary>
        /// Gets the current query text (the text that is in the edit portion of the control).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string QueryText
        {
            get { return _comboBox.Text; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ISuggestionProvider"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISuggestionProvider SuggestionProvider
        {
            get { return _comboBox.SuggestionProvider; }
            set { _comboBox.SuggestionProvider = value; }
        }

    }
}
