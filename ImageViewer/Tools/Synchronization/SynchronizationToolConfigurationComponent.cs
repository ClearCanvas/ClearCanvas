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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[ExtensionOf(typeof (ConfigurationPageProviderExtensionPoint))]
	internal sealed class ConfigurationPageProvider : IConfigurationPageProvider
	{
		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(AuthorityTokens.ViewerVisible))
				yield return new ConfigurationPage<SynchronizationToolConfigurationComponent>("TitleTools/TitleSynchronization");
		}
	}

	[ExtensionPoint]
	public sealed class SynchronizationToolConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (SynchronizationToolConfigurationComponentViewExtensionPoint))]
	public class SynchronizationToolConfigurationComponent : ConfigurationApplicationComponent
	{
		private SynchronizationToolSettings _settings;
		private float _parallelPlaneToleranceAngle;

		[ValidateGreaterThan(0f, Inclusive = true)]
		[ValidateLessThan(15f, Inclusive = true)]
		public float ParallelPlanesToleranceAngle
		{
			get { return _parallelPlaneToleranceAngle; }
			set
			{
				if (_parallelPlaneToleranceAngle != value)
				{
					_parallelPlaneToleranceAngle = value;
					base.Modified = true;
					base.NotifyPropertyChanged("ParallelPlanesToleranceAngle");
				}
			}
		}

		public override void Start()
		{
			base.Start();

            _settings = SynchronizationToolSettings.DefaultInstance;
			_parallelPlaneToleranceAngle = _settings.ParallelPlanesToleranceAngle;
		}

		public override void Save()
		{
			_settings.ParallelPlanesToleranceAngle = _parallelPlaneToleranceAngle;
			_settings.Save();
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}
	}
}