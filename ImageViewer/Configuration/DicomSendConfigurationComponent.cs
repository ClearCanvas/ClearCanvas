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
using System.Collections;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common.WorkItem;


namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomSendConfigurationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public sealed class DicomSendConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DicomSendConfigurationComponent class
	/// </summary>
	[AssociateView(typeof(DicomSendConfigurationComponentViewExtensionPoint))]
	public class DicomSendConfigurationComponent : ConfigurationApplicationComponent
	{
		private const int MaxRetryDelaySeconds = 3600;
		private const int MaxRetryDelayMinutes = 60;

		private DicomSendSettings _settings;
		private int _maxNumberOfRetries;
		private int _retryDelayValue;
		private RetryDelayTimeUnit _retryDelayUnit;

		private int _maxRetryDelayValue;

		public override void Start()
		{
			_settings = new DicomSendSettings();
			_maxNumberOfRetries = _settings.RetryCount;
			_retryDelayUnit = _settings.RetryDelayUnits;
			_retryDelayValue = _settings.RetryDelay;

			UpdateMaxRetryDelayValue();

			base.Start();
		}

		public override void Save()
		{
			try
			{
				// todo: this is rather inefficient - ideally there would be a method that would accept updates to muliple properties
				_settings.SetSharedPropertyValue(s => s.RetryCount, _maxNumberOfRetries);
				_settings.SetSharedPropertyValue(s => s.RetryDelayUnits, _retryDelayUnit);
				_settings.SetSharedPropertyValue(s => s.RetryDelay, _retryDelayValue);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#region Presentation Model

		public int MaxNumberOfRetries
		{
			get { return _maxNumberOfRetries; }
			set
			{
				if (_maxNumberOfRetries != value)
				{
					_maxNumberOfRetries = value;
					Modified = true;
					NotifyPropertyChanged("MaxNumberOfRetries");
				}
			}
		}

		public int MaxRetryDelayValue
		{
			get { return _maxRetryDelayValue; }
		}

		public int RetryDelayValue
		{
			get { return _retryDelayValue; }
			set
			{
				if (_retryDelayValue != value)
				{
					_retryDelayValue = value;
					Modified = true;
					NotifyPropertyChanged("RetryDelayValue");
				}
			}
		}

		public IList RetryDelayUnits
		{
			get { return Enum.GetValues(typeof(RetryDelayTimeUnit)); }
		}

		public object FormatRetryDelayUnit(object obj)
		{
			var unit = (RetryDelayTimeUnit)obj;
			return unit.GetDescription();
		}

		public RetryDelayTimeUnit RetryDelayUnit
		{
			get { return _retryDelayUnit; }
			set
			{
				if (_retryDelayUnit != value)
				{
					_retryDelayUnit = value;
					Modified = true;
					UpdateMaxRetryDelayValue();
					NotifyPropertyChanged("RetryDelayUnits");
				}
			}
		}

		#endregion

		private void UpdateMaxRetryDelayValue()
		{
			_maxRetryDelayValue = _retryDelayUnit == RetryDelayTimeUnit.Seconds ? MaxRetryDelaySeconds : MaxRetryDelayMinutes;
			NotifyPropertyChanged("MaxRetryDelayValue");
		}


	}
}
