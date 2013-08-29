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
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Clipboard.ImageExport;
using ClearCanvas.ImageViewer.Clipboard.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ImageExportComponent"/>.
	/// </summary>
	public partial class ImageExportComponentControl : ApplicationComponentUserControl, INotifyPropertyChanged
	{
		private ImageExportComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ImageExportComponentControl(ImageExportComponent component)
			: base(component)
		{
			_component = component;
			_component.PropertyChanged += OnComponentPropertyChanged;
			InitializeComponent();

			base.CancelButton = _buttonCancel;
			base.AcceptButton = _buttonOk;

			_imageExporters.DataSource = _component.ExporterInfoList;
			_imageExporters.DisplayMember = "Description";
			_imageExporters.DataBindings.Add("Value", _component, "SelectedExporterInfo", true, DataSourceUpdateMode.OnPropertyChanged);

			_checkOptionWysiwyg.DataBindings.Add("Checked", this, "ExportOptionWysiwyg", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkOptionCompleteImage.DataBindings.Add("Checked", this, "ExportOptionCompleteImage", true, DataSourceUpdateMode.OnPropertyChanged);

			_checkOptionScale.DataBindings.Add("Checked", this, "SizeModeScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkOptionFixed.DataBindings.Add("Checked", this, "SizeModeFixed", true, DataSourceUpdateMode.OnPropertyChanged);
			_pnlScale.DataBindings.Add("Enabled", this, "SizeModeScale", true, DataSourceUpdateMode.Never);
			_pnlFixedSize.DataBindings.Add("Enabled", this, "SizeModeFixed", true, DataSourceUpdateMode.Never);

			_scale.DataBindings.Add("Maximum", this, "MaximumScalePercent", true, DataSourceUpdateMode.Never);
			_scale.DataBindings.Add("Minimum", this, "MinimumScalePercent", true, DataSourceUpdateMode.Never);
			_scale.DataBindings.Add("Value", this, "ScalePercent", true, DataSourceUpdateMode.OnPropertyChanged);

			_imageWidth.DataBindings.Add("Maximum", _component, "MaximumDimension", true, DataSourceUpdateMode.Never);
			_imageWidth.DataBindings.Add("Minimum", _component, "MinimumDimension", true, DataSourceUpdateMode.Never);
			_imageWidth.DataBindings.Add("Value", _component, "Width", true, DataSourceUpdateMode.OnPropertyChanged);
			_imageHeight.DataBindings.Add("Maximum", _component, "MaximumDimension", true, DataSourceUpdateMode.Never);
			_imageHeight.DataBindings.Add("Minimum", _component, "MinimumDimension", true, DataSourceUpdateMode.Never);
			_imageHeight.DataBindings.Add("Value", _component, "Height", true, DataSourceUpdateMode.OnPropertyChanged);
			_outputDpi.DataBindings.Add("Value", _component, "OutputDpi", true, DataSourceUpdateMode.OnPropertyChanged);
			_backgroundColorSwatch.DataBindings.Add("BackColor", _component, "BackgroundColor", true, DataSourceUpdateMode.Never);

			_buttonConfigure.DataBindings.Add("Visible", _component, "ConfigureVisible", true, DataSourceUpdateMode.Never);
			_buttonConfigure.DataBindings.Add("Enabled", _component, "ConfigureEnabled", true, DataSourceUpdateMode.Never);
		}

		private void DoDispose(bool disposing)
		{
			if (disposing)
			{
				if (_component != null)
				{
					_component.PropertyChanged -= OnComponentPropertyChanged;
					_component = null;
				}
			}
		}

		#region INotifyPropertyChanged Members

		private event PropertyChangedEventHandler _propertyChanged;

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private void OnConfigureExporter(object sender, EventArgs e)
		{
			_component.Configure();
		}

		private void OnOk(object sender, EventArgs e)
		{
			if (_component.NumberOfImagesToExport > 1)
			{
				using (FolderBrowserDialog dialog = new FolderBrowserDialog())
				{
					if (DialogResult.OK == dialog.ShowDialog())
						_component.ExportFilePath = dialog.SelectedPath;
					else
						return;
				}
			}
			else
			{
				using (SaveFileDialog dialog = new SaveFileDialog())
				{
					dialog.Filter = _component.SelectedExporterInfo.FileExtensionFilter;
					dialog.DefaultExt = _component.SelectedExporterInfo.DefaultFileExtension;
					dialog.RestoreDirectory = true;
					dialog.AddExtension = true;

					if (DialogResult.OK == dialog.ShowDialog())
						_component.ExportFilePath = dialog.FileName;
					else
						return;
				}
			}

			_component.Accept();
		}

		private void OnCancel(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void OnBackgroundColorSwatchClick(object sender, EventArgs e)
		{
			Settings settings = Settings.Default;
			using (ColorDialog dlg = new ColorDialog())
			{
				dlg.AllowFullOpen = true;
				dlg.AnyColor = true;
				dlg.Color = _component.BackgroundColor;
				dlg.CustomColors = settings.CustomColorsArray;
				dlg.FullOpen = settings.ExpandColorDialog;
				dlg.ShowHelp = false;
				dlg.SolidColorOnly = false;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					_component.BackgroundColor = dlg.Color;
					settings.CustomColorsArray = dlg.CustomColors;
					settings.Save();
				}
			}
		}

		private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "ExportOption":
					this.ExportOptionWysiwyg = (_component.ExportOption == ExportOption.Wysiwyg);
					this.ExportOptionCompleteImage = (_component.ExportOption == ExportOption.CompleteImage);
					break;
				case "SizeMode":
					this.SizeModeScale = (_component.SizeMode == SizeMode.Scale);
					this.SizeModeFixed = (_component.SizeMode == SizeMode.Fixed);
					break;
			}
		}

		public float ScalePercent
		{
			get { return 100*_component.Scale; }
			set
			{
				value = value/100;
				if (value != _component.Scale)
				{
					_component.Scale = value;
					this.NotifyPropertyChanged("ScalePercent");
				}
			}
		}

		public float MaximumScalePercent
		{
			get { return 100*_component.MaximumScale; }
		}

		public float MinimumScalePercent
		{
			get { return 100*_component.MinimumScale; }
		}

		public bool ExportOptionWysiwyg
		{
			get { return _component.ExportOption == ExportOption.Wysiwyg; }
			set
			{
				if (_component.ExportOption == ExportOption.Wysiwyg && !value)
				{
					_component.ExportOption = ExportOption.CompleteImage;
					this.NotifyPropertyChanged("ExportOptionWysiwyg");
				}
			}
		}

		public bool ExportOptionCompleteImage
		{
			get { return _component.ExportOption == ExportOption.CompleteImage; }
			set
			{
				if (_component.ExportOption == ExportOption.CompleteImage && !value)
				{
					_component.ExportOption = ExportOption.Wysiwyg;
					this.NotifyPropertyChanged("ExportOptionCompleteImage");
				}
			}
		}

		public bool SizeModeScale
		{
			get { return _component.SizeMode == SizeMode.Scale; }
			set
			{
				if (_component.SizeMode == SizeMode.Scale && !value)
				{
					_component.SizeMode = SizeMode.Fixed;
					this.NotifyPropertyChanged("SizeModeScale");
				}
			}
		}

		public bool SizeModeFixed
		{
			get { return _component.SizeMode == SizeMode.Fixed; }
			set
			{
				if (_component.SizeMode == SizeMode.Fixed && !value)
				{
					_component.SizeMode = SizeMode.Scale;
					this.NotifyPropertyChanged("SizeModeFixed");
				}
			}
		}
	}
}