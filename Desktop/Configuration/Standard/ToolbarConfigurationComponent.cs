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

using System.Globalization;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	[ExtensionPoint]
	public sealed class ToolbarConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ToolbarConfigurationComponentViewExtensionPoint))]
	public sealed class ToolbarConfigurationComponent : ConfigurationApplicationComponent
	{
		private bool _wrap;
		private IconSize _iconSize;

		public bool Wrap
		{
			get { return _wrap; }
			set
			{
				if (_wrap != value)
				{
					_wrap = value;
					base.NotifyPropertyChanged("Wrap");
					base.Modified = true;
				}
			}
		}

		public IconSize IconSize
		{
			get { return _iconSize; }
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					base.NotifyPropertyChanged("IconSize");
					base.Modified = true;
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_wrap = ToolStripSettings.Default.WrapLongToolstrips;
			_iconSize = ToolStripSettings.Default.IconSize;
		}

		public override void Save()
		{
			var previousCulture = Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			try
			{
				ToolStripSettings.Default.WrapLongToolstrips = _wrap;
				ToolStripSettings.Default.IconSize = _iconSize;
				ToolStripSettings.Default.Save();
			}
			finally
			{
				Thread.CurrentThread.CurrentUICulture = previousCulture;
			}
		}
	}
}