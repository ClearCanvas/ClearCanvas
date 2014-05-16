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
using System.ComponentModel;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class DescriptiveSpinControl : UserControl
	{
		// Default formatting is to return the numeric value.
		private Converter<decimal, string> _defaultFormatter = value => value.ToString();

		public DescriptiveSpinControl()
		{
			InitializeComponent();

			_numericUpDown.Maximum = int.MaxValue;
			_numericUpDown.Minimum = int.MinValue;
			_numericUpDown.ValueChanged += delegate { _textBox.Text = this.Format(_numericUpDown.Value); };

			// When the description is clicked, give away focus to the spin control.
			_textBox.Enter += delegate { _numericUpDown.Focus(); };

			// The textbox is always in read-only mode.  Therefore manually change colour with enablement.
			this.EnabledChanged += delegate { SetTextBoxBackColour(); };

			// Set initial textBox back colour.
			SetTextBoxBackColour();
		}

		public Converter<decimal, string> Format { get; set; }

		[DefaultValue(int.MaxValue)]
		public decimal Maximum
		{
			get { return _numericUpDown.Maximum; }
			set { _numericUpDown.Maximum = value; }
		}

		[DefaultValue(int.MinValue)]
		public decimal Minimum
		{
			get { return _numericUpDown.Minimum; }
			set { _numericUpDown.Minimum = value; }
		}

		[DefaultValue(0)]
		public decimal Value
		{
			get { return _numericUpDown.Value; }
			set
			{
				_numericUpDown.Value = value;
				_textBox.Text = FormatValue(value);
			}
		}

		public event EventHandler ValueChanged
		{
			add { _numericUpDown.ValueChanged += value; }
			remove { _numericUpDown.ValueChanged -= value; }
		}

		private void SetTextBoxBackColour()
		{
			_textBox.BackColor = this.Enabled ? SystemColors.Window : SystemColors.Control;
		}

		private string FormatValue(decimal value)
		{
			return (this.Format ?? _defaultFormatter)(value);
		}
	}
}
