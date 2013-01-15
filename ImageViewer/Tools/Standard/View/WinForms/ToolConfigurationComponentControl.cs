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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	public partial class ToolConfigurationComponentControl : UserControl
	{
		private readonly ToolModalityBehaviorCollection _collection;
		private readonly ToolConfigurationComponent _component;

		public ToolConfigurationComponentControl(ToolConfigurationComponent component)
		{
			InitializeComponent();

			_component = component;

			_collection = new ToolModalityBehaviorCollection(component.ModalityBehavior);
			_collection.CollectionChanged += HandleCollectionChanged;

			var modalities = StandardModalities.Modalities.Union(new[] {string.Empty}).ToList();
			modalities.Sort(StringComparer.InvariantCultureIgnoreCase);
			var bindingSource = new BindingSource {DataSource = new BindingList<ToolModalityBehaviorSettings>(modalities.Select(s => new ToolModalityBehaviorSettings(_collection[s], string.IsNullOrEmpty(s) ? SR.LabelDefault : s)).ToList())};

			_cboModality.DataSource = bindingSource;
			_cboModality.DisplayMember = "Modality";

			_tooltipProvider.SetToolTip(_lblSelectedImage, SR.TooltipToolAppliesToSelectedImage);
			_tooltipProvider.SetToolTip(_radWindowLevelImages, SR.TooltipToolAppliesToSelectedImage);
			_tooltipProvider.SetToolTip(_radFlipRotateImages, SR.TooltipToolAppliesToSelectedImage);
			_tooltipProvider.SetToolTip(_radZoomImages, SR.TooltipToolAppliesToSelectedImage);
			_tooltipProvider.SetToolTip(_radPanImages, SR.TooltipToolAppliesToSelectedImage);
			_tooltipProvider.SetToolTip(_radResetImages, SR.TooltipToolAppliesToSelectedImage);

			_tooltipProvider.SetToolTip(_lblEntireDisplaySet, SR.TooltipToolAppliesToEntireDisplaySet);
			_tooltipProvider.SetToolTip(_radWindowLevelDisplaySets, SR.TooltipToolAppliesToEntireDisplaySet);
			_tooltipProvider.SetToolTip(_radFlipRotateDisplaySets, SR.TooltipToolAppliesToEntireDisplaySet);
			_tooltipProvider.SetToolTip(_radZoomDisplaySets, SR.TooltipToolAppliesToEntireDisplaySet);
			_tooltipProvider.SetToolTip(_radPanDisplaySets, SR.TooltipToolAppliesToEntireDisplaySet);
			_tooltipProvider.SetToolTip(_radResetDisplaySets, SR.TooltipToolAppliesToEntireDisplaySet);

			_radWindowLevelImages.DataBindings.Add("Checked", bindingSource, "SelectedOnlyWindowLevel", false, DataSourceUpdateMode.OnPropertyChanged);
			_radFlipRotateImages.DataBindings.Add("Checked", bindingSource, "SelectedOnlyOrientation", false, DataSourceUpdateMode.OnPropertyChanged);
			_radZoomImages.DataBindings.Add("Checked", bindingSource, "SelectedOnlyZoom", false, DataSourceUpdateMode.OnPropertyChanged);
			_radPanImages.DataBindings.Add("Checked", bindingSource, "SelectedOnlyPan", false, DataSourceUpdateMode.OnPropertyChanged);
			_radResetImages.DataBindings.Add("Checked", bindingSource, "SelectedOnlyReset", false, DataSourceUpdateMode.OnPropertyChanged);

			_radWindowLevelDisplaySets.DataBindings.Add("Checked", bindingSource, "DisplaySetWindowLevel", false, DataSourceUpdateMode.OnPropertyChanged);
			_radFlipRotateDisplaySets.DataBindings.Add("Checked", bindingSource, "DisplaySetOrientation", false, DataSourceUpdateMode.OnPropertyChanged);
			_radZoomDisplaySets.DataBindings.Add("Checked", bindingSource, "DisplaySetZoom", false, DataSourceUpdateMode.OnPropertyChanged);
			_radPanDisplaySets.DataBindings.Add("Checked", bindingSource, "DisplaySetPan", false, DataSourceUpdateMode.OnPropertyChanged);
			_radResetDisplaySets.DataBindings.Add("Checked", bindingSource, "DisplaySetReset", false, DataSourceUpdateMode.OnPropertyChanged);

			_chkInvertZoomDirection.DataBindings.Add("Checked", _component, "InvertZoomDirection", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void HandleCollectionChanged(object sender, EventArgs e)
		{
			_component.ModalityBehavior = new ToolModalityBehaviorCollection(_collection);
		}

		private void HandleRadioButtonClick(object sender, EventArgs e)
		{
			var radioButton = sender as RadioButton;
			if (radioButton != null)
				radioButton.Checked = true;
		}

		#region ToolModalityBehaviorSettings Class

		// ReSharper disable UnusedMember.Local
		// ReSharper disable MemberCanBePrivate.Local

		/// <summary>
		/// Adapts the model side settings to the dual toggle form needed for this UI implementation
		/// </summary>
		private class ToolModalityBehaviorSettings : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private readonly ToolModalityBehavior _item;
			private readonly string _modality;

			private bool _selectedOnlyWindowLevel;
			private bool _selectedOnlyOrientation;
			private bool _selectedOnlyZoom;
			private bool _selectedOnlyPan;
			private bool _selectedOnlyReset;

			public ToolModalityBehaviorSettings(ToolModalityBehavior item, string modality)
			{
				_item = item;
				_modality = modality;
				_selectedOnlyWindowLevel = _item.SelectedImageWindowLevelTool || _item.SelectedImageWindowLevelPresetsTool || _item.SelectedImageInvertTool;
				_selectedOnlyOrientation = _item.SelectedImageFlipTool || _item.SelectedImageRotateTool;
				_selectedOnlyZoom = _item.SelectedImageZoomTool;
				_selectedOnlyPan = _item.SelectedImagePanTool;
				_selectedOnlyReset = _item.SelectedImageResetTool;
			}

			public string Modality
			{
				get { return _modality; }
			}

			public bool SelectedOnlyWindowLevel
			{
				get { return _selectedOnlyWindowLevel; }
				set
				{
					if (_selectedOnlyWindowLevel != value)
					{
						_selectedOnlyWindowLevel = value;
						NotifyPropertyChanged(@"SelectedOnlyWindowLevel");
						NotifyPropertyChanged(@"DisplaySetWindowLevel");

						_item.SelectedImageWindowLevelTool = _item.SelectedImageWindowLevelPresetsTool = _item.SelectedImageInvertTool = value;
					}
				}
			}

			public bool SelectedOnlyOrientation
			{
				get { return _selectedOnlyOrientation; }
				set
				{
					if (_selectedOnlyOrientation != value)
					{
						_selectedOnlyOrientation = value;
						NotifyPropertyChanged(@"SelectedOnlyOrientation");
						NotifyPropertyChanged(@"DisplaySetOrientation");

						_item.SelectedImageFlipTool = _item.SelectedImageRotateTool = value;
					}
				}
			}

			public bool SelectedOnlyZoom
			{
				get { return _selectedOnlyZoom; }
				set
				{
					if (_selectedOnlyZoom != value)
					{
						_selectedOnlyZoom = value;
						NotifyPropertyChanged(@"SelectedOnlyZoom");
						NotifyPropertyChanged(@"DisplaySetZoom");

						_item.SelectedImageZoomTool = value;
					}
				}
			}

			public bool SelectedOnlyPan
			{
				get { return _selectedOnlyPan; }
				set
				{
					if (_selectedOnlyPan != value)
					{
						_selectedOnlyPan = value;
						NotifyPropertyChanged(@"SelectedOnlyPan");
						NotifyPropertyChanged(@"DisplaySetPan");

						_item.SelectedImagePanTool = value;
					}
				}
			}

			public bool SelectedOnlyReset
			{
				get { return _selectedOnlyReset; }
				set
				{
					if (_selectedOnlyReset != value)
					{
						_selectedOnlyReset = value;
						NotifyPropertyChanged(@"SelectedOnlyReset");
						NotifyPropertyChanged(@"DisplaySetReset");

						_item.SelectedImageResetTool = value;
					}
				}
			}

			public bool DisplaySetWindowLevel
			{
				get { return !SelectedOnlyWindowLevel; }
				set { SelectedOnlyWindowLevel = !value; }
			}

			public bool DisplaySetOrientation
			{
				get { return !SelectedOnlyOrientation; }
				set { SelectedOnlyOrientation = !value; }
			}

			public bool DisplaySetZoom
			{
				get { return !SelectedOnlyZoom; }
				set { SelectedOnlyZoom = !value; }
			}

			public bool DisplaySetPan
			{
				get { return !SelectedOnlyPan; }
				set { SelectedOnlyPan = !value; }
			}

			public bool DisplaySetReset
			{
				get { return !SelectedOnlyReset; }
				set { SelectedOnlyReset = !value; }
			}

			private void NotifyPropertyChanged(string propertyName)
			{
				EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
			}
		}

		// ReSharper restore MemberCanBePrivate.Local
		// ReSharper restore UnusedMember.Local

		#endregion
	}
}