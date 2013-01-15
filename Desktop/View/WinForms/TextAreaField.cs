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
    public partial class TextAreaField : UserControl
    {
        public TextAreaField()
        {
            InitializeComponent();
        }

        
        public string Value
        {
            get { return NullIfEmpty(_textBox.Text); }
            set { _textBox.Text = value; }
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

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _textBox.ReadOnly; }
            set { _textBox.ReadOnly = value; }
        }

        [DefaultValue(true)]
        public bool WordWrap
        {
            get { return _textBox.WordWrap; }
            set { _textBox.WordWrap = value; }
        }

        [DefaultValue(ScrollBars.None)]
        public ScrollBars ScrollBars
        {
            get { return _textBox.ScrollBars; }
            set { _textBox.ScrollBars = value; }
        }

		[DefaultValue(32767)]
		public int MaximumLength
		{
			get { return _textBox.MaxLength; }
			set { _textBox.MaxLength = value; }
		}

        private static string NullIfEmpty(string value)
        {
            return (value != null && value.Length == 0) ? null : value;
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
    }
}
