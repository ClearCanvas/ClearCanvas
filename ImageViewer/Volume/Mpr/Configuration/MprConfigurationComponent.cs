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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[ExtensionPoint]
	public sealed class MprConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (MprConfigurationComponentViewExtensionPoint))]
	public class MprConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string Path = "Mpr";

		private MprSettings _settings;
		private float _sliceSpacingFactor = 1;
		private bool _automaticSliceSpacing = true;

		[ValidateGreaterThan(0.5f, Inclusive = true, Message = "MessageValidateSliceSpacing")]
		[ValidateLessThan(10f, Inclusive = true, Message = "MessageValidateSliceSpacing")]
		public float SliceSpacingFactor
		{
			get { return _sliceSpacingFactor; }
			set
			{
				if (_sliceSpacingFactor != value)
				{
					_sliceSpacingFactor = value;
					this.Modified = true;
					this.NotifyPropertyChanged("SliceSpacingFactor");
				}
			}
		}

		public bool ProportionalSliceSpacing
		{
			get { return !_automaticSliceSpacing; }	
			set
			{
				AutomaticSliceSpacing = !value;
			}
		}

		public bool AutomaticSliceSpacing
		{
			get { return _automaticSliceSpacing; }
			set
			{
				if (_automaticSliceSpacing != value)
				{
					_automaticSliceSpacing = value;
					this.Modified = true;
					this.NotifyPropertyChanged("AutoSliceSpacing");
					this.NotifyPropertyChanged("ProportionalSliceSpacing");
				}
			}
		}

		public override void Start()
		{
			base.Start();

            _settings = MprSettings.DefaultInstance;
			_sliceSpacingFactor = _settings.SliceSpacingFactor;
			_automaticSliceSpacing = _settings.AutoSliceSpacing;
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}

		public override void Save()
		{
			_settings.AutoSliceSpacing = _automaticSliceSpacing;
			_settings.SliceSpacingFactor = _sliceSpacingFactor;
			_settings.Save();
		}
	}
}