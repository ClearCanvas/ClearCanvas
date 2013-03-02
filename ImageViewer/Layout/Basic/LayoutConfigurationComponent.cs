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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.ComponentModel;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Extension point for views onto <see cref="LayoutSettingsApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public sealed class LayoutConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// LayoutSettingsApplicationComponent class
	/// </summary>
	[AssociateView(typeof(LayoutConfigurationComponentViewExtensionPoint))]
	public class LayoutConfigurationComponent : ConfigurationApplicationComponent
	{
		private List<StoredLayout> _layouts;

		public LayoutConfigurationComponent()
		{
		}

		/// <summary>
		/// Gets the maximum allowable rows for image boxes.
		/// </summary>
		public int MaximumImageBoxRows
		{
			get { return LayoutSettings.MaximumImageBoxRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for image boxes.
		/// </summary>
		public int MaximumImageBoxColumns
		{
			get { return LayoutSettings.MaximumImageBoxColumns; }
		}

		/// <summary>
		/// Gets the maximum allowable rows for tiles.
		/// </summary>
		public int MaximumTileRows
		{
			get { return LayoutSettings.MaximumTileRows; }
		}

		/// <summary>
		/// Gets the maximum allowable columns for tiles.
		/// </summary>
		public int MaximumTileColumns
		{
			get { return LayoutSettings.MaximumTileColumns; }
		}
		
		public IList<StoredLayout> Layouts
		{
			get
			{
				if (_layouts == null)
				{
					_layouts = new List<StoredLayout>(LayoutSettings.DefaultInstance.Layouts);

					StoredLayout defaultLayout = _layouts.Find(delegate(StoredLayout layout) { return layout.IsDefault; });

					//make sure there is one for each available modality, don't worry about the default - there will always be one provided by the settings class.
					foreach (string modality in StandardModalities.Modalities)
					{
						if (!_layouts.Exists(delegate(StoredLayout layout) { return layout.Modality == modality; }))
							_layouts.Add(new StoredLayout(modality, defaultLayout.ImageBoxRows, defaultLayout.ImageBoxColumns, defaultLayout.TileRows, defaultLayout.TileColumns));
					}


					_layouts.Sort(new StoredLayoutSortByModality());
					foreach(StoredLayout layout in _layouts)
						layout.PropertyChanged += delegate { Modified = true; };
				}

				return _layouts;
			}
		}

		public static void Configure(IDesktopWindow desktopWindow)
		{
			ConfigurationDialog.Show(desktopWindow, ConfigurationPageProvider.BasicLayoutConfigurationPath);
		}

		public override void Save()
		{
            LayoutSettings.DefaultInstance.Layouts = _layouts;
		}
	}
}
