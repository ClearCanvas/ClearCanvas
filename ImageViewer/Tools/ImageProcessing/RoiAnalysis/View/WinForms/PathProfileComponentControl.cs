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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using NPlot;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PathProfileComponent"/>
	/// </summary>
	public partial class PathProfileComponentControl : ApplicationComponentUserControl
	{
		private PathProfileComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PathProfileComponentControl(PathProfileComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			Refresh(null, EventArgs.Empty);
			_component.AllPropertiesChanged += new EventHandler(Refresh);
		}

		private void Refresh(object sender, EventArgs e)
		{
			_plotSurface.Clear();
			_plotSurface.BackColor = Color.Black;

			if (!_component.ComputeProfile())
			{
				_plotSurface.Refresh();
				return;
			}

			LinePlot linePlot = new LinePlot();
			linePlot.AbscissaData = _component.PixelIndices;
			linePlot.OrdinateData = _component.PixelValues;
			linePlot.Pen = new Pen(Application.CurrentUITheme.Colors.StandardColorBase);

			_plotSurface.Add(linePlot);
			_plotSurface.PlotBackColor = Color.Black;
			_plotSurface.XAxis1.Color = Color.White;
			_plotSurface.YAxis1.Color = Color.White;
			_plotSurface.Refresh();
		}
	}
}