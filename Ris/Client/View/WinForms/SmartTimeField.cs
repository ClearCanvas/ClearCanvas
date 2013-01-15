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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// This class is experimental, does not entirely work right now.
    /// </summary>
    public partial class SmartTimeField : UserControl
    {
        private List<TimeSpan> _choices = new List<TimeSpan>();

        private TimeSpan _minimum = TimeSpan.Zero + TimeSpan.FromHours(7);      // 7am
        private TimeSpan _maximum = TimeSpan.Zero + TimeSpan.FromHours(7+24);   // 7am tomorrow
        private TimeSpan _interval = TimeSpan.FromMinutes(30);  // 30 mins

        public SmartTimeField()
        {
            InitializeComponent();
            _input.SuggestionProvider = new DefaultSuggestionProvider<TimeSpan>(_choices, FormatTimeSpan);
            _input.Format += new ListControlConvertEventHandler(InputFormatEventHandler);
        }


        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        public TimeSpan Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        public TimeSpan Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                if (!DesignMode)
                {
                    GenerateChoices();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if(!this.DesignMode)
            {
                GenerateChoices();
            }

            base.OnLoad(e);
        }

        private void GenerateChoices()
        {
            _choices.Clear();
            for (TimeSpan value = _minimum; value < _maximum; value += _interval)
                _choices.Add(value);
        }

        private string FormatTimeSpan(TimeSpan ts)
        {
            DateTime time = DateTime.Today + ts;
            return Format.Time(time);
        }

        private void InputFormatEventHandler(object sender, ListControlConvertEventArgs e)
        {
            e.Value = FormatTimeSpan((TimeSpan)e.ListItem);
        }

    }
}
