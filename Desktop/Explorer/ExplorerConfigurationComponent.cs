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

namespace ClearCanvas.Desktop.Explorer
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	internal class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (!ExplorerLocalSettings.Default.ExplorerIsPrimary && ExplorerTool.GetExplorers().Count > 0)
				yield return new ConfigurationPage<ExplorerConfigurationComponent>("PathExplorer");
		}

		#endregion
	}

	[ExtensionPoint]
	public sealed class ExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ExplorerConfigurationComponentViewExtensionPoint))]
	public class ExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		private bool _launchAsShelf;
		private bool _launchAtStartup;

		public ExplorerConfigurationComponent()
		{
			_launchAsShelf = ExplorerSettings.Default.LaunchAsShelf;
			_launchAtStartup = ExplorerSettings.Default.LaunchAtStartup;
		}

		#region Presentation Model

		public new bool LaunchAsWorkspace
		{
			get { return !_launchAsShelf; }
			set
			{
				if (value)
					LaunchAsShelf = false;
				else
					LaunchAsShelf = true;
			}
		}

		public new bool LaunchAsShelf
		{
			get { return _launchAsShelf; }
			set
			{
				if (_launchAsShelf != value)
				{
					_launchAsShelf = value;
					NotifyPropertyChanged("LaunchAsShelf");
					NotifyPropertyChanged("LaunchAsWorkspace");

					Modified = true;
				}
			}
		}

		public bool LaunchAtStartup
		{
			get { return _launchAtStartup; }
			set
			{
				if (_launchAtStartup != value)
				{
					_launchAtStartup = value;
					NotifyPropertyChanged("LaunchAtStartup");

					Modified = true;
				}
			}
		}

		#endregion

		public override void Save()
		{
			ExplorerSettings.Default.LaunchAsShelf = _launchAsShelf;
			ExplorerSettings.Default.LaunchAtStartup = _launchAtStartup;

			ExplorerSettings.Default.Save();
		}
	}
}
