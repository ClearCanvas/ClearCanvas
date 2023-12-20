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
using System.Text;
using System.Timers;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using System.ComponentModel;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Linq;

namespace ClearCanvas.ImageViewer.Tools.Measurement.RoiAnalysis
{
    [ExtensionPoint]
    public class RoiAnalysisComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

    public interface IRoiAnalysisToolContext : IToolContext
    {
        Roi Roi { get; }

        RoiAnalysisComponentContainer Component { get; }
    }


    [AssociateView(typeof(RoiAnalysisComponentContainerViewExtensionPoint))]
	public class RoiAnalysisComponentContainer : ImageViewerToolComponent
	{
        private class RoiAnalysisToolContext : ToolContext, IRoiAnalysisToolContext
        {
            private readonly RoiAnalysisComponentContainer _component;
            public RoiAnalysisToolContext(RoiAnalysisComponentContainer component)
            {
                _component = component;
            }

            public Roi Roi
            {
                get { return _component._roiGraphic?.GetRoi(); }
            }

            public RoiAnalysisComponentContainer Component => _component;
        }

        #region private
        private int _minBin = 0;
        private int _maxBin = 1000;
        private int _numBins = 100;
        private int[] _binLabels;
        private int[] _bins;
        private bool _autoscale;
        private Table<RoiInfoItem> _roiInfoItems;
        private ToolSet _toolSet;
        private ActionModelRoot _toolbarModel;
        private RoiGraphic _roiGraphic;
        private bool _resizeInProgress = false;
        private Timer _timer = new Timer(100);
        private Roi _mementoRoi;
      
        #endregion

        #region constructor
        public RoiAnalysisComponentContainer(IDesktopWindow desktopWindow): base(desktopWindow)
		{
            _timer.Enabled = true;
            _timer.Elapsed += Timer_Elapsed;
            _roiInfoItems = new Table<RoiInfoItem>();
            
            ImageViewer.Closing += ImageViewer_Closing;
            ImageViewer.EventBroker.GraphicSelectionChanged += OnGraphicSelectedChanged;
            ImageViewer.EventBroker.GraphicFocusChanged += OnGraphicFocusChanged;
            
          
            var setValueDelegate = new TableColumn<RoiInfoItem, string>.SetColumnValueDelegate<RoiInfoItem, string>((d, value) =>
            {
                d.SetValue(value);
            });
            _roiInfoItems.Columns.Add(new TableColumn<RoiInfoItem, string>("Data", delegate (RoiInfoItem i) { return i.Name; }));
            _roiInfoItems.Columns.Add(new TableColumn<RoiInfoItem, string>("Value", delegate (RoiInfoItem i) { return i.ValueAsString; }, setValueDelegate));
            
        }

       




        #endregion

        #region publicProperties
        public int MinBin
        {
            get { return _minBin; }
            set
            {
                _minBin = value;
                this.NotifyPropertyChanged("MinBin");
                
            }
        }

        public int MaxBin
        {
            get { return _maxBin; }
            set
            {
                _maxBin = value;
                this.NotifyPropertyChanged("MaxBin");
                
            }
        }

        public int NumBins
        {
            get { return _numBins; }
            set
            {
                _numBins = value;
                this.NotifyPropertyChanged("NumBins");
                
            }
        }

        public bool AutoScale
        {
            get { return _autoscale; }
            set
            {
                _autoscale = value;
                this.NotifyPropertyChanged("AutoScale");
                
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

        public RoiGraphic SelectedRoiGraphic
        {
            get
            {
                return _roiGraphic;
            }
        }

        public Table<RoiInfoItem> RoiInfoItems
        {
            get { return _roiInfoItems; }
        }
        #endregion

        #region publicMethods
        public bool ComputeHistogram()
        {
            Roi roi = _roiGraphic?.GetRoi();

            if (roi != null)
            {
                return ComputeHistogram(roi);
            }

            //this.Enabled = false;
            return false;
        }
        #endregion

        #region overrideMethods
        protected override void OnActiveImageViewerChanging(ActiveImageViewerChangedEventArgs e)
        {
            base.OnActiveImageViewerChanging(e);
            if(e.DeactivatedImageViewer != null)
            {
                e.DeactivatedImageViewer.Closing -= ImageViewer_Closing;
                e.DeactivatedImageViewer.EventBroker.GraphicSelectionChanged -= OnGraphicSelectedChanged;
            }
           if(e.ActivatedImageViewer != null)
            {
                e.ActivatedImageViewer.Closing += ImageViewer_Closing;
                e.ActivatedImageViewer.EventBroker.GraphicSelectionChanged += OnGraphicSelectedChanged;
            }
           

        }
        protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
        {
            base.OnActiveImageViewerChanged (e);
        }

        public override void Start()
		{
			base.Start();
            _toolSet = new ToolSet(new RoiHistogramToolExtensionPoint(), new RoiAnalysisToolContext(this));
            SetRoi(ImageViewer?.SelectedPresentationImage?.SelectedGraphic as RoiGraphic);
            foreach (var r in (new RoiInfoItemExtensionPoint().CreateExtensions()))
            {
                if (r is IRoiInfoItem)
                {
                    _roiInfoItems.Items.Add(r as IRoiInfoItem);
                }
            }
            UpdateComponent(_roiGraphic?.GetRoi());
        }

        public override void Stop()
        {
            if(ImageViewer != null)
            {
                ImageViewer.Closing -= ImageViewer_Closing;
                ImageViewer.EventBroker.GraphicSelectionChanged -= OnGraphicSelectedChanged;
            }
            
            base.Stop();
        }
        #endregion

        #region privateMethods
        private void ImageViewer_Closing(object sender, EventArgs e)
        {
            if(ImageViewer !=null)
                ImageViewer.Closing -= ImageViewer_Closing;
            Exit(ApplicationComponentExitCode.None);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            UpdateComponent(_mementoRoi);

        }
        private void OnGraphicSelectedChanged(object sender, GraphicSelectionChangedEventArgs e)
        {
            //if (e.SelectedGraphic is null)
            //    return;
            //var roi = e.SelectedGraphic as RoiGraphic;
            //if (roi is null)
            //    return;
            //if (roi != _roiGraphic)
            //{
            //    SetRoi(roi);
            //    UpdateComponent(_roiGraphic?.GetRoi());
            //}

        }
        private void OnGraphicFocusChanged(object sender, GraphicFocusChangedEventArgs e)
        {
            if (e.FocusedGraphic is RoiGraphic)
            {
                SetRoi(e.FocusedGraphic as RoiGraphic);
                UpdateComponent(_roiGraphic?.GetRoi());
            }
        }
        private void RoiChanged(object sender, EventArgs e)
        {
            _mementoRoi = _roiGraphic.GetRoi();
            _timer.Stop();
            _timer.Enabled = true;
            _timer.Start();
            //UpdateComponent(_roiGraphic.GetRoi());
        }

        private void UpdateComponent(Roi roi)
        {
            
            if (roi is null)
                return;
            foreach (var r in _roiInfoItems.Items)
            {
                //var roi = _roiGraphic.GetRoi();
                try
                {
                    r.SetRoi(roi);
                    r.SetComponent(this);
                }
                catch (Exception setRoiException)
                {
                    Platform.Log(LogLevel.Error, "Unable to Set Roi " + r.Name + ": " + setRoiException.Message);
                }
                try
                {
                    r.ComputeValue();
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, "Unable to compute roi info item " + r.Name + ": " + ex.Message);
                }

            }
            ComputeHistogram(roi);
            NotifyAllPropertiesChanged();
        }
        private bool ComputeHistogram(Roi roi)
        {
            // For now, only allow ROIs of grayscale images
            GrayscalePixelData pixelData = roi.PixelData as GrayscalePixelData;
            if (pixelData == null)
            {
                //this.Enabled = false;
                return false;
            }

            int left = (int)Math.Round(roi.BoundingBox.Left);
            int right = (int)Math.Round(roi.BoundingBox.Right);
            int top = (int)Math.Round(roi.BoundingBox.Top);
            int bottom = (int)Math.Round(roi.BoundingBox.Bottom);

            // If any part of the ROI is outside the bounds of the image,
            // don't allow a histogram to be displayed since it's invalid.
            if (left < 0 || left > pixelData.Columns - 1 ||
                right < 0 || right > pixelData.Columns - 1 ||
                top < 0 || top > pixelData.Rows - 1 ||
                bottom < 0 || bottom > pixelData.Rows - 1)
            {
                //this.Enabled = false;
                return false;
            }

            var roiPixelData = new List<double>(roi.GetPixelValues()).ToArray();
            _maxBin = (int)(roiPixelData.Max() * 1.5);
            if(roiPixelData.Min() >= 0)
            {
                _minBin = 0;
            }
            else
            {
                _minBin = (int)(roiPixelData.Min() * 0.5);
            }


            if(_minBin < 0)
            {
                _numBins = (int)((Math.Abs(_maxBin) + Math.Abs(_minBin)) / 10);
            }            
            else
            {
                _numBins = (int)((_maxBin - _minBin) / 10);
            }
            if (_numBins < 10)
                _numBins = 10;
            else if(_numBins > 100)
                _numBins = 100;
            
            Histogram histogram = new Histogram(
                _minBin, _maxBin, _numBins, roiPixelData);

            _bins = histogram.Bins;
            _binLabels = histogram.BinLabels;

            NotifyPropertyChanged("MinBin");
            NotifyPropertyChanged("MaxBin");
            NotifyPropertyChanged("NumBins");

            //this.Enabled = true;
            return true;
        }

        private void SetRoi(RoiGraphic graphic)
        {
            if (graphic is null)
                return;
           
            if (_roiGraphic != null)
            {
                _roiGraphic.RoiChanged -= RoiChanged;               
            }
            _roiGraphic = graphic;
            _roiGraphic.RoiChanged += RoiChanged;
            UpdateComponent(_roiGraphic.GetRoi());
        }

        private void OnHistogramChanged()
        {
            HistogramChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public event EventHandler HistogramChanged;

    }
}
