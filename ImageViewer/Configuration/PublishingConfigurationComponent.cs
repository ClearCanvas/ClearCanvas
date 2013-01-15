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
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class PublishingConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof (PublishingConfigurationComponentViewExtensionPoint))]
	public class PublishingConfigurationComponent : ConfigurationApplicationComponent
	{
		internal PublishingConfigurationComponent()
		{
		}

		private bool _publishLocalToSourceAE;

		public bool PublishLocalToSourceAE
		{
			get { return _publishLocalToSourceAE; }
			set
			{
				if (value == _publishLocalToSourceAE)
					return;

				_publishLocalToSourceAE = value;
				base.Modified = true;
				NotifyPropertyChanged("PublishLocalToSourceAE");
			}
		}

		public override void Start()
		{
			_publishLocalToSourceAE = DicomPublishingSettings.Default.SendLocalToStudySourceAE;

			base.Start();
		}

		public override void Save()
		{
            ApplicationSettingsExtensions.SetSharedPropertyValue(DicomPublishingSettings.Default, "SendLocalToStudySourceAE", PublishLocalToSourceAE);
		}
	}
}