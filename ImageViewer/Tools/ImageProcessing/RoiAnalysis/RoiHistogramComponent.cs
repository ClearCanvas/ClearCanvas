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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiAnalysis
{
	[ExtensionPoint()]
	public class RoiHistogramToolExtensionPoint : ExtensionPoint<ITool>
    {

    }

	/// <summary>
	/// Extension point for views onto <see cref="RoiHistogramComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class RoiHistogramComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	public interface IRoiHistogramToolContext : IToolContext
    {
		Roi Roi { get; }

		RoiHistogramComponent Component { get; }
    }

	/// <summary>
	/// RoiHistogramComponent class
	/// </summary>
	[AssociateView(typeof (RoiHistogramComponentViewExtensionPoint))]
	public class RoiHistogramComponent : RoiAnalysisComponent
	{
		private int _minBin = 0;
		private int _maxBin = 1000;
		private int _numBins = 100;
		private int[] _binLabels;
		private int[] _bins;
		private bool _autoscale;
		private ToolSet _toolSet;
        private ActionModelRoot _toolbarModel;
        private Table<RoiInfoItem> _roiInfoItems;
		protected List<RoiGraphic> _roiGraphics;
		protected RoiGraphic _selectedRoiGraphic;
		private class RoiHistogramToolContext : ToolContext, IRoiHistogramToolContext
        {
			private readonly RoiHistogramComponent _component;

			public RoiHistogramToolContext(RoiHistogramComponent component)
            {
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			public Roi Roi
            {
                get { return _component.GetRoi(); }
            }

			public RoiHistogramComponent Component
            {
                get { return _component; }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RoiHistogramComponent(IImageViewerToolContext imageViewerToolContext)
			: base(imageViewerToolContext) 
		{
			_roiGraphics = new List<RoiGraphic>();
		
			
			_roiInfoItems = new Table<RoiInfoItem>();
			var setValueDelegate = new TableColumn<RoiInfoItem, string>.SetColumnValueDelegate<RoiInfoItem, string>((d, value) =>
			{
				d.SetValue(value);
			});
			_roiInfoItems.Columns.Add(new TableColumn<RoiInfoItem, string>("Data", delegate(RoiInfoItem i) { return i.Name; }));
			_roiInfoItems.Columns.Add(new TableColumn<RoiInfoItem, string>("Value", delegate (RoiInfoItem i) { return i.ValueAsString; },setValueDelegate));
		}

		public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
        }

        public override void Start()
        {
			
            base.Start();
			_toolSet = new ToolSet(new RoiHistogramToolExtensionPoint(), new RoiHistogramToolContext(this));
			_toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "roihistogram-toolbar", _toolSet.Actions);
			foreach(var r in (new RoiInfoItemExtensionPoint().CreateExtensions()))
            {
				if(r is IRoiInfoItem)
                {
					_roiInfoItems.Items.Add(r as IRoiInfoItem);
                }
            }
			GetRoiGraphics();
			_selectedRoiGraphic =  GetSelectedRoi();
			UpdateComponent();
        }

		public List<RoiGraphic> RoiGraphics
        {
            get { return _roiGraphics; }
        }

		public RoiGraphic SelectedRoiGraphic
        {
            get
            {
				return _selectedRoiGraphic;
            }
            set
            {
				_selectedRoiGraphic = value;
				ImageViewer.SelectedPresentationImage.SelectedGraphic = _selectedRoiGraphic;                
                UpdateComponent();
				OnAllPropertiesChanged();
				
            }
        }

		public void UpdateComponent()
        {
						
			foreach(var r in _roiInfoItems.Items)
            {
				var roi = GetRoi();
                try
                {
					r.SetRoi(roi);
					r.SetComponent(this);
				}
				catch(Exception setRoiException)
                {
					Platform.Log(LogLevel.Error, "Unable to Set Roi " + r.Name +": " + setRoiException.Message);
                }
                try
                {
					r.ComputeValue();
				}
				catch(Exception ex)
                {
					Platform.Log(LogLevel.Error, "Unable to compute roi info item " + r.Name +": " + ex.Message);
				}
				
            }
			NotifyPropertyChanged("RoiInfoItems");
        }

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

		public Table<RoiInfoItem> RoiInfoItems 
		{ 
			get { return _roiInfoItems; } 
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

		#region event
		/// <summary>
		/// Fires when an ROI is added to the <see cref="ImageViewer"/>
		/// </summary>
		public event EventHandler RoiAdded;

		private void OnRoiAdded()
        {
			EventsHelper.Fire(RoiAdded, this, new EventArgs());
		}

		/// <summary>
		/// Is thrown when any Roi has a change of name
		/// </summary>
		public event EventHandler RoiNameChanged;



		private void OnRoiNameChanged()
        {
			EventsHelper.Fire(RoiNameChanged, this, new EventArgs());
			UpdateComponent();
			//RoiNameChanged?.Invoke(this, new EventArgs());
        }
        #endregion

        #region private

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

        protected override void OnGraphicSelectionChanged(object sender, Graphics.GraphicSelectionChangedEventArgs e)
        {
            
			if(e.SelectedGraphic is RoiGraphic)
            {
                if (!RoiGraphics.Contains(e.SelectedGraphic as RoiGraphic))
                {
					RoiGraphics.Add(e.SelectedGraphic as RoiGraphic);
					(e.SelectedGraphic as RoiGraphic).NameChanged += Roi_NameChanged;					
					SelectedRoiGraphic = (e.SelectedGraphic as RoiGraphic);
					OnRoiAdded();
				}
				SelectedRoiGraphic = e.SelectedGraphic as RoiGraphic;
            }
			base.OnGraphicSelectionChanged(sender, e);
		}

		/// <summary>
		/// Gets the ROIGraphics in the current presentation image only
		/// </summary>
		/// <remarks>
		/// This method was supposed to query every RoiGraphic on every PrsentationImage in the viewer but for some reason
		/// it will only ever pick up the RoiGraphics on the current PresentationImage.
		/// </remarks>
        private void GetRoiGraphics()
        {
            if (ImageViewer.SelectedPresentationImage is GrayscalePresentationImage)
            {
                var gray = ImageViewer.SelectedPresentationImage as GrayscalePresentationImage;
                if (gray != null)
                {
					
					var y = GetRoiGraphics(gray.SceneGraph);
                    _roiGraphics.AddRange(y);
                }
            }
        }		

		/// <summary>
		/// Recursively scours a <see cref="CompositeGraphic"/> for any <see cref="RoiGraphic"/>s and returns them in a list
 		/// </summary>
		/// <param name="compositeGraphic"></param>
		/// <returns></returns>
		private List<RoiGraphic> GetRoiGraphics(Graphics.CompositeGraphic compositeGraphic)
        {			
			if (compositeGraphic.Graphics is null)
            {				
				return new List<RoiGraphic>();
			}				
			if(compositeGraphic.Graphics.Count == 0)
            {				
				return new List<RoiGraphic>();
			}			
            else
            {
				var result = new List<RoiGraphic>();				
				foreach (var g in compositeGraphic.Graphics)
                {					
					if (g is RoiGraphic)
					{						
						result.Add(g as RoiGraphic);
                        (g as RoiGraphic).NameChanged += Roi_NameChanged;
                    }
					else if(g is Graphics.CompositeGraphic)
                    {						
						result.AddRange(GetRoiGraphics(g as Graphics.CompositeGraphic));
					}					
                }			
				return result;
            }
        }

        private void Roi_NameChanged(object sender, EventArgs e)
        {
			OnRoiNameChanged();
        }
        #endregion
    }
}