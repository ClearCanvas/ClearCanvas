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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	/// <summary>
	/// Extension point for views onto <see cref="RoiHistogramComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class RoiHistogramComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// RoiHistogramComponent class
	/// </summary>
	[AssociateView(typeof (RoiHistogramComponentViewExtensionPoint))]
	public class RoiHistogramComponent : RoiAnalysisComponent
	{
		private int _minBin = -200;
		private int _maxBin = 1000;
		private int _numBins = 100;
		private int[] _binLabels;
		private int[] _bins;
		private bool _autoscale;

		/// <summary>
		/// Constructor
		/// </summary>
		public RoiHistogramComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext) {}

		public int MinBin
		{
			get { return _minBin; }
			set
			{
				_minBin = value;
				this.NotifyPropertyChanged("MinBin");
				OnAllPropertiesChanged();
			}
		}

		public int MaxBin
		{
			get { return _maxBin; }
			set
			{
				_maxBin = value;
				this.NotifyPropertyChanged("MaxBin");
				OnAllPropertiesChanged();
			}
		}

		public int NumBins
		{
			get { return _numBins; }
			set
			{
				_numBins = value;
				this.NotifyPropertyChanged("NumBins");
				OnAllPropertiesChanged();
			}
		}

		public bool AutoScale
		{
			get { return _autoscale; }
			set
			{
				_autoscale = value;
				this.NotifyPropertyChanged("AutoScale");
				OnAllPropertiesChanged();
			}
		}

		public int[] BinLabels
		{
			get { return _binLabels; }
		}

		public int[] Bins
		{
			get { return _bins; }
		}

		public bool ComputeHistogram()
		{
			Roi roi = GetRoi();

			if (roi != null)
			{
				return ComputeHistogram(roi);
			}

			this.Enabled = false;
			return false;
		}

		private bool ComputeHistogram(Roi roi)
		{
			// For now, only allow ROIs of grayscale images
			GrayscalePixelData pixelData = roi.PixelData as GrayscalePixelData;
			if (pixelData == null)
			{
				this.Enabled = false;
				return false;
			}

			int left = (int) Math.Round(roi.BoundingBox.Left);
			int right = (int) Math.Round(roi.BoundingBox.Right);
			int top = (int) Math.Round(roi.BoundingBox.Top);
			int bottom = (int) Math.Round(roi.BoundingBox.Bottom);

			// If any part of the ROI is outside the bounds of the image,
			// don't allow a histogram to be displayed since it's invalid.
			if (left < 0 || left > pixelData.Columns - 1 ||
			    right < 0 || right > pixelData.Columns - 1 ||
			    top < 0 || top > pixelData.Rows - 1 ||
			    bottom < 0 || bottom > pixelData.Rows - 1)
			{
				this.Enabled = false;
				return false;
			}

			var roiPixelData = new List<double>(roi.GetPixelValues()).ToArray();

			Histogram histogram = new Histogram(
				_minBin, _maxBin, _numBins, roiPixelData);

			_bins = histogram.Bins;
			_binLabels = histogram.BinLabels;

			NotifyPropertyChanged("MinBin");
			NotifyPropertyChanged("MaxBin");
			NotifyPropertyChanged("NumBins");

			this.Enabled = true;
			return true;
		}

		private Roi GetRoi()
		{
			RoiGraphic graphic = GetSelectedRoi();
			if (graphic == null)
				return null;

			return graphic.Roi;
		}

		protected override bool CanAnalyzeSelectedRoi()
		{
			return GetRoi() != null;
		}
	}
}