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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="DisplaySetOptionsApplicationComponent"/>.
	/// </summary>
	public partial class DisplaySetCreationConfigurationComponentControl : ApplicationComponentUserControl
	{
		private readonly DisplaySetCreationConfigurationComponent _component;
		private readonly BindingSource _bindingSource;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DisplaySetCreationConfigurationComponentControl(DisplaySetCreationConfigurationComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_bindingSource = new BindingSource {DataSource = _component.Options};

			_modality.DataSource = _bindingSource;
			_modality.DisplayMember = "Modality";

			_createSingleImageDisplaySets.DataBindings.Add("Checked", _bindingSource, "CreateSingleImageDisplaySets", false, DataSourceUpdateMode.OnPropertyChanged);
			_createSingleImageDisplaySets.DataBindings.Add("Enabled", _bindingSource, "CreateSingleImageDisplaySetsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_createAllImagesDisplaySet.DataBindings.Add("Checked", _bindingSource, "CreateAllImagesDisplaySet", false, DataSourceUpdateMode.OnPropertyChanged);
			_createAllImagesDisplaySet.DataBindings.Add("Enabled", _bindingSource, "CreateAllImagesDisplaySetEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_showOriginalSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_splitEchos.DataBindings.Add("Checked", _bindingSource, "SplitMultiEchoSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitEchos.DataBindings.Add("Enabled", _bindingSource, "SplitMultiEchoSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_showOriginalMultiEchoSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMultiEchoSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMultiEchoSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMultiEchoSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_splitStacks.DataBindings.Add("Checked", _bindingSource, "SplitMultiStackSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitStacks.DataBindings.Add("Enabled", _bindingSource, "SplitMultiStackSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_showOriginalMultiStackSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMultiStackSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMultiStackSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMultiStackSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_splitMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "SplitMixedMultiframes", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "SplitMixedMultiframesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_showOriginalMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMixedMultiframeSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMixedMultiframeSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_invertImages.DataBindings.Add("Checked", _bindingSource, "ShowGrayscaleInverted", false, DataSourceUpdateMode.OnPropertyChanged);
			_invertImages.DataBindings.Add("Enabled", _bindingSource, "ShowGrayscaleInvertedEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			SetOverlayItems();
			_bindingSource.CurrentItemChanged += (sender, args) => SetOverlayItems();
		}

		private IconCheckBox AddOverlayCheckBox()
		{
			var check = new IconCheckBox() {Margin = new Padding(3, 2, 3, 2)};
			check.CheckedChanged += (sender, args) => ((OverlaySelectionSetting) check.Tag).IsSelected = check.Checked;
			_overlaysPanel.Controls.Add(check);
			return check;
		}

		private void SetOverlayItems()
		{
			_overlaysPanel.SuspendLayout();

			var item = _bindingSource.Current as StoredDisplaySetCreationSetting;
			if (item != null)
			{
				var applicableOverlays = item.OverlaySelections;
				while (_overlaysPanel.Controls.Count > applicableOverlays.Count)
				{
					var last = _overlaysPanel.Controls[_overlaysPanel.Controls.Count - 1];
					last.Dispose(); //automatically removes itself.
				}

				for (int i = 0; i < applicableOverlays.Count; ++i)
				{
					IconCheckBox checkBox;
					if (_overlaysPanel.Controls.Count > i)
						checkBox = (IconCheckBox) _overlaysPanel.Controls[i];
					else
						checkBox = AddOverlayCheckBox();

					var overlay = applicableOverlays[i];
					Image icon = null;
					var manager = OverlayHelper.OverlayManagers.FirstOrDefault(m => m.Name == overlay.Name);
					if (manager != null && manager.IconSet != null)
						icon = manager.IconSet.CreateIcon(IconSize.Small, manager.ResourceResolver);

					checkBox.Image = icon;
					checkBox.Tag = overlay;
					checkBox.Name = overlay.Name;
					checkBox.Text = overlay.DisplayName;
					checkBox.CheckEnabled = overlay.IsConfigurable;
					checkBox.Checked = overlay.IsSelected;
				}
			}
			else
			{
				_overlaysPanel.Controls.Clear();
			}

			_overlaysPanel.ResumeLayout(true);
		}
	}
}