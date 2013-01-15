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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TimestampField : UserControl
    {
        public event EventHandler ValueChanged;

        public TimestampField()
        {
            InitializeComponent();
            _datePicker.ValueChanged += OnValueChanged;
            _timePicker.ValueChanged += OnValueChanged;

            // System.Component.DesignMode does not work in control constructors
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                try
                {
                    _datePicker.CustomFormat = Format.DateFormat;
                    _timePicker.CustomFormat = Format.TimeFormat;
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to set custom date/time format");
                }
            }
        }

        [Browsable(true)]
        [Category("Label")]
		[Localizable(true)]
        public string TextLabel
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value == string.Empty ? null : value;
            }
        }

        public DateTime MaxValue
        {
            get { return _datePicker.MaxDate; }
            set { _datePicker.MaxDate = value; }
        }

        public DateTime MinValue
        {
            get { return _datePicker.MinDate; }
            set { _datePicker.MinDate = value; }
        }

        public DateTime? Value
        {
            get
            {
                if (!_datePicker.Checked)
                    return null;

                return _datePicker.Value.Date + _timePicker.Value.TimeOfDay;
                
            }

            set
            {
                _datePicker.Checked = value != null;
                if (value != null)
                {
                    _datePicker.Value = value.Value;
                    _timePicker.Value = value.Value;
                    _timePicker.Enabled = true;
                }
                else
                {
                    _timePicker.Enabled = false;
                }

                if (ValueChanged != null)
                    EventsHelper.Fire(ValueChanged, this, EventArgs.Empty);
            }
        }

        private void OnDatePickerMouseUp(object sender, MouseEventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (!_datePicker.Checked)
            {
                Value = null;
            } 
            else
            {
                Value = _datePicker.Value.Date + _timePicker.Value.TimeOfDay;
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            UpdateState();
        }
    }
}
