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
	public interface IRichTextBoxOwner
	{
		/// <summary>
		/// Get the RichTextBox control
		/// </summary>
		RichTextBox GetRichTextBox();

		/// <summary>
		/// Use this method instead of the RichTextBox.SelectedText
		/// </summary>
		/// <param name="text"></param>
		void SetSelectedText(string text);
	}

	public partial class RichTextField : UserControl, IRichTextBoxOwner
	{
		public RichTextField()
		{
			InitializeComponent();
		}

		public string Value
		{
			get { return NullIfEmpty(_richTextBox.Text); }
			set { _richTextBox.Text = value; }
		}

		public event EventHandler ValueChanged
		{
			add { _richTextBox.TextChanged += value; }
			remove { _richTextBox.TextChanged -= value; }
		}

		[Localizable(true)]
		public string LabelText
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		[DefaultValue(false)]
		public bool AcceptsTab
		{
			get { return _richTextBox.AcceptsTab; }
			set { _richTextBox.AcceptsTab = value; }
		}

		[DefaultValue(false)]
		public bool ReadOnly
		{
			get { return _richTextBox.ReadOnly; }
			set { _richTextBox.ReadOnly = value; }
		}

		[DefaultValue(true)]
		public bool WordWrap
		{
			get { return _richTextBox.WordWrap; }
			set { _richTextBox.WordWrap = value; }
		}

		[DefaultValue(RichTextBoxScrollBars.None)]
		public RichTextBoxScrollBars ScrollBars
		{
			get { return _richTextBox.ScrollBars; }
			set { _richTextBox.ScrollBars = value; }
		}

		[DefaultValue(32767)]
		public int MaximumLength
		{
			get { return _richTextBox.MaxLength; }
			set { _richTextBox.MaxLength = value; }
		}

		private static string NullIfEmpty(string value)
		{
			return (value != null && value.Length == 0) ? null : value;
		}

		private void _richTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (_richTextBox.ReadOnly)
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

		private void _richTextBox_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				string dropString = (String)e.Data.GetData(typeof(String));

				// Insert string at the current keyboard cursor
				int currentIndex = _richTextBox.SelectionStart;
				_richTextBox.Text = string.Concat(
					_richTextBox.Text.Substring(0, currentIndex),
					dropString,
					_richTextBox.Text.Substring(currentIndex));
			}
		}

		#region IRichTextBoxOwner Members

		public RichTextBox GetRichTextBox()
		{
			return _richTextBox;
		}

		public void SetSelectedText(string text)
		{
			_richTextBox.SelectedText = text;
		}

		#endregion
	}
}
