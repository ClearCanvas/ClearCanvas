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
using System.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	public sealed class SynchronizationToolSettingsHelper : INotifyPropertyChanged
	{
        //TODO (Phoenix5): #10730 - remove this when it's fixed.
        [ThreadStatic]
        private static SynchronizationToolSettingsHelper _default;
	    
        public static SynchronizationToolSettingsHelper Default
	    {
	        get
	        {
	            return _default ?? (_default = new SynchronizationToolSettingsHelper(SynchronizationToolSettings.DefaultInstance));
	        }
	    }

		private event PropertyChangedEventHandler _propertyChanged;
		private readonly SynchronizationToolSettings _settings;
		private volatile float _parallelPlanesToleranceAngleRadians = -1;

		private SynchronizationToolSettingsHelper(SynchronizationToolSettings settings)
		{
			_settings = settings;
			_settings.PropertyChanged += OnSettingChanged;
		}

		private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "ParallelPlanesToleranceAngle")
				return;
			
			_parallelPlanesToleranceAngleRadians = -1;
			NotifyPropertyChanged("ParallelPlanesToleranceAngleRadians");
			NotifyPropertyChanged("ParallelPlanesToleranceAngleDegrees");
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		/// <summary>
		/// Gets the maximum angle difference, in degrees, between two planes for synchronization tools to treat the planes as parallel.
		/// </summary>
		/// <seealso cref="ParallelPlanesToleranceAngleRadians"/>
		public float ParallelPlanesToleranceAngleDegrees
		{
			get
			{
				var value = _settings.ParallelPlanesToleranceAngle;
				return value == 0F ? 100*float.Epsilon : value;
			}
		}

		/// <summary>
		/// Gets the maximum angle difference, in radians, between two planes for synchronization tools to treat the planes as parallel.
		/// </summary>
		/// <seealso cref="ParallelPlanesToleranceAngleDegrees"/>
		public float ParallelPlanesToleranceAngleRadians
		{
			get
			{
				if (_parallelPlanesToleranceAngleRadians < 0)
					_parallelPlanesToleranceAngleRadians = ParallelPlanesToleranceAngleDegrees*(float)Math.PI/180F;
				return _parallelPlanesToleranceAngleRadians;
			}
		}
	}
}