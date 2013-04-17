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
    public partial class TextField : UserControl
    {
        private string _toolTip;

        public TextField()
        {
            InitializeComponent();
        }

        /*
        public string Value
        {
            get { return NullIfEmpty(_textBox.Text); }
            set { _textBox.Text = value; }
        }
        */
        [Category("Text Field")]
        public string Value
        {
            get 
            {
                return _textBox.IsNull == false ? 
                    (string)_textBox.Value : 
                    _textBox.NullTextReturnValue; 
            } 
            set 
            { 
                _textBox.Value = 
                    value == string.Empty ? null : value; 
            }
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                return _textBox.ReadOnly;
            }
            set
            {
                _textBox.ReadOnly = value;
            }
        }

		[Localizable(true)]
        public char PasswordChar
        {
            get { return _textBox.PasswordChar; }
            set { _textBox.PasswordChar = value; }
        }

        [DefaultValue("")]
		[Localizable(true)]
        public string ToolTip
        {
            get
            {
                return _toolTip;
            }
            set
            {
                _toolTip = value;
            }
        }

        public event EventHandler ValueChanged
        {
            add { _textBox.TextChanged += value; }
            remove { _textBox.TextChanged -= value; }
        }

		[Localizable(true)]
        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }
        
        /// <summary>
        /// Set/Get the text field mask.   See System.Windows.Forms.MaskedTextBox.Mask for details on setting the Mask value
        /// </summary>
        /// <seealso cref="System.Windows.Forms.MaskedTextBox.Mask"/>
        [Category("Masked Text Field")]
        [Description("See System.Windows.Forms.MaskedTextBox.Mask Property")]
        public string Mask
        {
            get { return _textBox.EditMask; }
            set { _textBox.EditMask = value; }
        }

        /// <summary>
        /// Gets or sets the current selection in the <see cref="TextField"/> control.
        /// </summary>
        /// <seealso cref="TextBoxBase.SelectedText"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get { return _textBox.SelectedText; }
            set { _textBox.SelectedText = value; }
        }

        /// <summary>
        /// Gets or sets the starting point of text selected in the <see cref="TextField"/> control.
        /// </summary>
        /// <seealso cref="TextBoxBase.SelectionStart"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart
        {
            get { return _textBox.SelectionStart; }
            set { _textBox.SelectionStart = value; }
        }

        /// <summary>
        /// Gets or sets the number of characters selected in the <see cref="TextField"/> control.
        /// </summary>
        /// <seealso cref="TextBoxBase.SelectionLength"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get { return _textBox.SelectionLength; }
            set { _textBox.SelectionLength = value; }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _textBox.Focus();
        }

        private void _textBox_MouseHover(object sender, EventArgs e)
        {
            ShowToolTip(true);
        }

        private void _textBox_MouseLeave(object sender, EventArgs e)
        {
            ShowToolTip(false);
        }

        // Hide the tooltip if the user starts typing again before the five-second display limit on the tooltip expires.
        private void _textBox_KeyDown(object sender, KeyEventArgs e)
        {
            ShowToolTip(false);
        }

        private void ShowToolTip(bool show)
        {
            if (show)
                _textFieldToolTip.Show(_toolTip, _textBox, _textBox.Location.X, _textBox.Location.Y, 5000);
            else
                _textFieldToolTip.Hide(_textBox);       
        }

		private void _textBox_DragEnter(object sender, DragEventArgs e)
		{
			if (_textBox.ReadOnly)
			{
				e.Effect = DragDropEffects.None;
				return;
			}

			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void _textBox_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				string dropString = (String)e.Data.GetData(typeof(String));

				// Insert string at the current keyboard cursor
				int currentIndex = _textBox.SelectionStart;
				_textBox.Text = string.Concat(
					_textBox.Text.Substring(0, currentIndex),
					dropString,
					_textBox.Text.Substring(currentIndex));
			}
		}

        // private static string NullIfEmpty(string value)
        // {
            // return (value != null && value.Length == 0) ? null : value;
        // }
    }
}
