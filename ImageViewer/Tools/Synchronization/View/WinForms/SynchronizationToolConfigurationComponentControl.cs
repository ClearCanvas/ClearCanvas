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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Synchronization.View.WinForms
{
	public partial class SynchronizationToolConfigurationComponentControl : UserControl, INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private readonly SynchronizationToolConfigurationComponent _component;
		private string _toleranceAngle;

		public SynchronizationToolConfigurationComponentControl(SynchronizationToolConfigurationComponent component)
		{
			InitializeComponent();

			_component = component;
			_toleranceAngle = _component.ParallelPlanesToleranceAngle.ToString();
			_txtToleranceAngle.DataBindings.Add("Text", this, "ToleranceAngle", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		public string ToleranceAngle
		{
			get { return _toleranceAngle; }
			set
			{
				if (_toleranceAngle != value)
				{
					_toleranceAngle = value;
					this.NotifyPropertyChanged("ToleranceAngle");

					float fValue;
					if (float.TryParse(_toleranceAngle, out fValue))
					{
						if (fValue >= 0 && fValue <= 15)
						{
							_component.ParallelPlanesToleranceAngle = fValue;
							_errorProvider.SetError(_pnlToleranceAngleControl, string.Empty);
						}
						else
						{
							// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
							_component.ParallelPlanesToleranceAngle = -1;
							_errorProvider.SetError(_pnlToleranceAngleControl, SR.ErrorAngleOutOfRange);
						}
					}
					else
					{
						// deliberately set a value out of range, so that the component will fail internal range validation and refuse to exit
						_component.ParallelPlanesToleranceAngle = -1;
						_errorProvider.SetError(_pnlToleranceAngleControl, SR.ErrorInvalidNumberFormat);
					}
				}
			}
		}
	}
}