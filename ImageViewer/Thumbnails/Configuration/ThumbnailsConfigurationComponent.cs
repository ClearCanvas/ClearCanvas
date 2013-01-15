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

namespace ClearCanvas.ImageViewer.Thumbnails.Configuration
{
	[ExtensionPoint]
	public sealed class ThumbnailsConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ThumbnailsConfigurationComponentViewExtensionPoint))]
	public class ThumbnailsConfigurationComponent : ConfigurationApplicationComponent
	{
		public static readonly string Path = "LabelThumbnails";

		private ThumbnailsSettings _settings;
		private bool _autoOpenThumbnails;

		public ThumbnailsConfigurationComponent() {}

		public bool AutoOpenThumbnails
		{
			get { return _autoOpenThumbnails; }
			set
			{
				if (_autoOpenThumbnails != value)
				{
					_autoOpenThumbnails = value;
					this.Modified = true;
					this.NotifyPropertyChanged("AutoOpenThumbnails");
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_settings = ThumbnailsSettings.Default;
			_autoOpenThumbnails = _settings.AutoOpenThumbnails;
		}

		public override void Save()
		{
			_settings.AutoOpenThumbnails = _autoOpenThumbnails;
			_settings.Save();
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}
	}
}