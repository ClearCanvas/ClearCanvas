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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using NPlot;
using Application = ClearCanvas.Desktop.Application;

namespace ClearCanvas.ImageViewer.Tools.Measurement.RoiAnalysis.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="RoiHistogramComponent"/>
	/// </summary>
	public partial class RoiHistogramComponentControl : ApplicationComponentUserControl
	{
		private RoiHistogramComponent _component;
		private BindingSource _bindingSource;

		/// <summary>
		/// Constructor
		/// </summary>
		public RoiHistogramComponentControl(RoiHistogramComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;
			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_minUpDown.Minimum = Decimal.MinValue;
			_minUpDown.Maximum = Decimal.MaxValue;
			_minUpDown.Increment = 10;
			_minUpDown.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
			_minUpDown.Accelerations.Add(new NumericUpDownAcceleration(5, 100));

			_maxUpDown.Minimum = Decimal.MinValue;
			_maxUpDown.Maximum = Decimal.MaxValue;
			_maxUpDown.Increment = 10;
			_maxUpDown.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
			_maxUpDown.Accelerations.Add(new NumericUpDownAcceleration(5, 100));

			_numBinsUpDown.Minimum = 1;
			_numBinsUpDown.Maximum = 200;
			_numBinsUpDown.Increment = 5;

			_minUpDown.DataBindings.Add("Value", _bindingSource, "MinBin", true, DataSourceUpdateMode.OnPropertyChanged);
			_minUpDown.DataBindings.Add("Enabled", _bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_maxUpDown.DataBindings.Add("Value", _bindingSource, "MaxBin", true, DataSourceUpdateMode.OnPropertyChanged);
			_maxUpDown.DataBindings.Add("Enabled", _bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_numBinsUpDown.DataBindings.Add("Value", _bindingSource, "NumBins", true, DataSourceUpdateMode.OnPropertyChanged);
			_numBinsUpDown.DataBindings.Add("Enabled", _bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);

			Refresh(null, EventArgs.Empty);
			_component.AllPropertiesChanged += new EventHandler(Refresh);
			_component.RoiAdded += _component_RoiAdded;
			_component.RoiNameChanged += _component_RoiNameChanged;

			_roiInfoTable.Table = _component.RoiInfoItems;
			_roiInfoTable.ToolbarModel = _component.ToolbarModel;		

			roiGraphicCombo.DataSource = _component.RoiGraphics;			
			roiGraphicCombo.DataBindings.Add("Value", _bindingSource, "SelectedRoiGraphic", true, DataSourceUpdateMode.OnPropertyChanged);			
            

			
		}

        private void _component_RoiNameChanged(object sender, EventArgs e)
        {
			
			setComboValues();
			//Refresh();
		}

        private void _component_RoiAdded(object sender, EventArgs e)
        {		
			setComboValues();
		}

		private void setComboValues()
        {
			var blah = roiGraphicCombo.DataSource;
			roiGraphicCombo.DataSource = null;
			roiGraphicCombo.DataSource = blah;
			roiGraphicCombo.DisplayMember = "Name";
			//roiGraphicCombo.Value = _component.SelectedRoiGraphic;
			//roiGraphicCombo.Refresh();

		}
        private void Refresh(object sender, EventArgs e)
		{
			_roiInfoTable.Refresh();

			_plotSurface.Clear();
			_plotSurface.BackColor = Color.White;			
			
			
			
			if (!_component.ComputeHistogram())
			{
				_plotSurface.Refresh();
				return;
			}
			
			HistogramPlot histogram = new HistogramPlot();
			histogram.AbscissaData = _component.BinLabels;
			histogram.OrdinateData = _component.Bins;
			histogram.Center = false;
			histogram.BaseWidth = 1.0f;
			histogram.Filled = true;
			histogram.Pen = new Pen(Color.Gray);
			histogram.RectangleBrush = new RectangleBrushes.Solid(Color.Gray);
			
			_plotSurface.Add(histogram);
			_plotSurface.PlotBackColor = Color.White;
			_plotSurface.XAxis1.Color = Color.Black;
			_plotSurface.YAxis1.Color = Color.Black;
			_plotSurface.Refresh();
			
		

		}
	}
}