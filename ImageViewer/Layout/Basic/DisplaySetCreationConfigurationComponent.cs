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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExtensionPoint]
	public sealed class DisplaySetCreationConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(DisplaySetCreationConfigurationComponentViewExtensionPoint))]
	public class DisplaySetCreationConfigurationComponent : ConfigurationApplicationComponent
	{
		private BindingList<StoredDisplaySetCreationSetting> _settings;

		public DisplaySetCreationConfigurationComponent()
		{
		}

		public override void Start()
		{
			Initialize();
			base.Start();
		}

		public override void Save()
		{
			DisplaySetCreationSettings.DefaultInstance.Save(_settings);
		}

		private void Initialize()
		{
			List<StoredDisplaySetCreationSetting> sortedSettings = DisplaySetCreationSettings.DefaultInstance.GetStoredSettings();
			sortedSettings = CollectionUtils.Sort(sortedSettings,
			                                      (setting1, setting2) => System.String.CompareOrdinal(setting1.Modality, setting2.Modality));

			_settings = new BindingList<StoredDisplaySetCreationSetting>(sortedSettings);
			foreach (StoredDisplaySetCreationSetting setting in _settings)
			{
			    setting.PropertyChanged += (s,e) => OnPropertyChanged();
                foreach(var overlaySelection in setting.OverlaySelections)
                    overlaySelection.IsSelectedChanged += (s, e) => OnPropertyChanged();
			}
		}

		private void OnPropertyChanged()
		{
			this.Modified = true;
		}

		public BindingList<StoredDisplaySetCreationSetting> Options
		{
			get { return _settings; }
		}
	}
}
