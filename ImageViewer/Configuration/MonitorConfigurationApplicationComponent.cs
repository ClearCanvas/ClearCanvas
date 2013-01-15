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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="MonitorConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public sealed class MonitorConfigurationApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// MonitorConfigurationApplicationComponent class
	/// </summary>
	[AssociateView(typeof(MonitorConfigurationApplicationComponentViewExtensionPoint))]
	public class MonitorConfigurationApplicationComponent : ConfigurationApplicationComponent
	{
		private WindowBehaviour _windowBehaviour;

		/// <summary>
		/// Constructor
		/// </summary>
		public MonitorConfigurationApplicationComponent()
		{
		}

		public bool SingleWindow
		{
			get
			{
				return _windowBehaviour == WindowBehaviour.Single;
			}
			set
			{
				if (value == true)
				{
					_windowBehaviour = WindowBehaviour.Single;
					this.Modified = true;
				}
			}
		}

		public bool SeparateWindow
		{
			get
			{
				return _windowBehaviour == WindowBehaviour.Separate;
			}
			set
			{
				if (value == true)
				{
					_windowBehaviour = WindowBehaviour.Separate;
					this.Modified = true;
				}
			}
		}

		public override void Start()
		{
			_windowBehaviour = (WindowBehaviour)MonitorConfigurationSettings.Default.WindowBehaviour;
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Save()
		{
			MonitorConfigurationSettings.Default.WindowBehaviour = (int)_windowBehaviour;
			MonitorConfigurationSettings.Default.Save();
		}
	}
}