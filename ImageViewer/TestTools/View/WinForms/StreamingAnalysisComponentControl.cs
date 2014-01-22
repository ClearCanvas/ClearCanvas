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

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.StudyLoaders.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;
using NPlot;
using System.Threading;
using Timer=System.Windows.Forms.Timer;

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StreamingAnalysisComponent"/>.
    /// </summary>
    public partial class StreamingAnalysisComponentControl : ApplicationComponentUserControl
    {
        private StreamingAnalysisComponent _component;
    	private Timer _timer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StreamingAnalysisComponentControl(StreamingAnalysisComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_addSelectedStudies.DataBindings.Add("Enabled", component, "CanAddSelectedStudies", true,
        	                                     DataSourceUpdateMode.OnPropertyChanged);

			_decompressActive.DataBindings.Add("Checked", component, "DecompressActive", true,
										 DataSourceUpdateMode.OnPropertyChanged);

        	_decompressActive.Click += delegate { _component.DecompressActive = !_component.DecompressActive; };
			
			_retrieveActive.DataBindings.Add("Checked", component, "RetrieveActive", true,
										 DataSourceUpdateMode.OnPropertyChanged);

			_retrieveActive.Click += delegate { _component.RetrieveActive = !_component.RetrieveActive; };
			
			_retrieveItems.DataBindings.Add("Value", component, "NumberOfRetrieveItems", true,
        	                                DataSourceUpdateMode.OnPropertyChanged);

			_retrieveConcurrency.DataBindings.Add("Value", component, "RetrieveConcurrency", true,
										 DataSourceUpdateMode.OnPropertyChanged);

			_decompressConcurrency.DataBindings.Add("Value", component, "DecompressConcurrency", true,
								 DataSourceUpdateMode.OnPropertyChanged);

			_decompressItems.DataBindings.Add("Value", component, "NumberOfDecompressItems", true,
									DataSourceUpdateMode.OnPropertyChanged);

			_retrievePriority.DataSource = GetThreadPriorities();
			//_retrievePriority.DataBindings.Add("SelectedValue", component, "RetrieveThreadPriority", true,
			//                                   DataSourceUpdateMode.OnPropertyChanged);

        	_retrievePriority.SelectedValueChanged +=
        		delegate { _component.RetrieveThreadPriority = (ThreadPriority)_retrievePriority.SelectedValue; };
			_retrievePriority.SelectedItem = _component.RetrieveThreadPriority;

			_decompressPriority.DataSource = GetThreadPriorities();
			//_decompressPriority.DataBindings.Add("SelectedValue", component, "DecompressThreadPriority", true,
			//                       DataSourceUpdateMode.OnPropertyChanged);

			_decompressPriority.SelectedValueChanged +=
				delegate { _component.DecompressThreadPriority = (ThreadPriority)_decompressPriority.SelectedValue; };

			_decompressPriority.SelectedItem = _component.DecompressThreadPriority;
        	

        	//_plotAverage.ValueChanged += delegate { RefreshRetrievePlot(); };

			_timer = new Timer();
        	_timer.Interval = 5000;
			_timer.Tick += new EventHandler(OnTimer);
        	_timer.Enabled = true;
		}

		private List<ThreadPriority> GetThreadPriorities()
		{
			List<ThreadPriority> priorities = new List<ThreadPriority>();
			priorities.Add(ThreadPriority.Highest);
			priorities.Add(ThreadPriority.AboveNormal);
			priorities.Add(ThreadPriority.Normal);
			priorities.Add(ThreadPriority.BelowNormal);
			priorities.Add(ThreadPriority.Lowest);
			return priorities;
		}

		void OnTimer(object sender, EventArgs e)
		{
			RefreshRetrievePlot();
		}

		private void RefreshRetrievePlot()
		{
			_retrieveSpeedPlot.Clear();
			PointPlot plot = new PointPlot();

			List<DateTime> timePoints;
			List<double> retrieveMbPerSecond;
			ComputePlotAverage(out timePoints, out retrieveMbPerSecond);

			plot.AbscissaData = timePoints;
			plot.DataSource = retrieveMbPerSecond;

			Grid grid = new Grid();
			grid.HorizontalGridType = Grid.GridType.Coarse;
			grid.VerticalGridType = Grid.GridType.Coarse;

			_retrieveSpeedPlot.Add(grid);
			_retrieveSpeedPlot.Add(plot);

			_retrieveSpeedPlot.ShowCoordinates = true;
			_retrieveSpeedPlot.YAxis1.Label = "Mb/sec";
			_retrieveSpeedPlot.YAxis1.LabelOffsetAbsolute = true;
			_retrieveSpeedPlot.YAxis1.LabelOffset = 40;
			_retrieveSpeedPlot.Padding = 5;

			//Align percent plot axes.
			DateTimeAxis ax = new DateTimeAxis(_retrieveSpeedPlot.XAxis1);
			ax.HideTickText = false;
			_retrieveSpeedPlot.XAxis1 = ax;

			_retrieveSpeedPlot.Refresh();
		}

		private void ComputePlotAverage(out List<DateTime> timePoints, out List<double> retrieveMbPerSecond)
		{
			timePoints = new List<DateTime>();
			retrieveMbPerSecond = new List<double>();

			for (int i = 0; i < _component.PerformanceInfo.Count; ++i)
			{
				DateTime minStartTime = DateTime.MaxValue;
				DateTime maxEndTime = DateTime.MinValue;
				long totalBytes = 0;
				
				for (int j = 0; j < (int)_plotAverage.Value && i < _component.PerformanceInfo.Count; ++j, ++i)
				{
					StreamingPerformanceInfo performanceInfo = _component.PerformanceInfo[i];
					totalBytes += performanceInfo.TotalBytesTransferred;

					if (performanceInfo.StartTime < minStartTime)
						minStartTime = performanceInfo.StartTime;
					if (performanceInfo.EndTime > maxEndTime)
						maxEndTime = performanceInfo.EndTime;
				}

				long averageTicks = minStartTime.Ticks + (maxEndTime.Ticks - minStartTime.Ticks)/2;
				double timeSpanSeconds = maxEndTime.Subtract(minStartTime).TotalSeconds;
				double averageMbPerSecond = totalBytes / timeSpanSeconds / 1024 / 1024 * 8;
				timePoints.Add(new DateTime(averageTicks));
				retrieveMbPerSecond.Add(averageMbPerSecond);
			}
		}

    	private void _addSelectedStudies_Click(object sender, EventArgs e)
		{
			_component.AddSelectedStudies();
		}

		private void _clearRetrieve_Click(object sender, EventArgs e)
		{
			_component.ClearRetrieveItems();
		}

		private void _clearDecompress_Click(object sender, EventArgs e)
		{
			_component.ClearDecompressItems();
		}

		private void _refreshPlot_Click(object sender, EventArgs e)
		{
			RefreshRetrievePlot();
		}
    }
}
