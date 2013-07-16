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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
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
            :base(component)
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

			_splitMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "SplitMixedMultiframes", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "SplitMixedMultiframesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_showOriginalMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMixedMultiframeSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMixedMultiframeSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_invertImages.DataBindings.Add("Checked", _bindingSource, "ShowGrayscaleInverted", false, DataSourceUpdateMode.OnPropertyChanged);
			_invertImages.DataBindings.Add("Enabled", _bindingSource, "ShowGrayscaleInvertedEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

            SetOverlayItems();
            _bindingSource.CurrentItemChanged += (sender, args) =>
                                                     {
                                                         SetOverlayItems();
                                                     };

            _listOverlays.ItemChecked += ListOverlaysOnItemChecked;
		}

        private void ListOverlaysOnItemChecked(object sender, ItemCheckedEventArgs itemCheckedEventArgs)
        {
            if (itemCheckedEventArgs.Item == null)return;

            var item = itemCheckedEventArgs.Item.Tag as OverlaySelectionSetting;
            if (item != null)
                item.IsSelected = itemCheckedEventArgs.Item.Checked;
        }

        private void SetOverlayItems()
        {
            _listOverlays.Items.Clear();

            var item = _bindingSource.Current as StoredDisplaySetCreationSetting;
            if (item == null)
                return;

            foreach (var overlaySelection in item.OverlaySelections)
            {
                var listItem = new ListViewItem(overlaySelection.DisplayName){Tag = overlaySelection, Checked = overlaySelection.IsSelected};
                _listOverlays.Items.Add(listItem);
            }
        }
    }
}
