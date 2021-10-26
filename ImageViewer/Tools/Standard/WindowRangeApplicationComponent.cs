using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [ExtensionPoint]
    public sealed class WindowRangeApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {

    }

    [AssociateView(typeof(WindowRangeApplicationComponentViewExtensionPoint))]
    public class WindowRangeApplicationComponent : ImageViewerToolComponent
    {
        #region privateFields
        double _windowWidth;
        double _windowCenter;

        double _windowMin;
        double _windowMax;

        IVoiLutProvider _voiLutProvider;
        WindowRangeTool _tool;
        private readonly VoiLutImageOperation _operation;
        private ToolModalityBehaviorHelper _toolBehavior;
        private int[] _binLabels;
        private int[] _bins;
        IPresentationImage oldImage;
        private int _histogramMin = 0;
        private int _histogramMax = 100;
        private int _numBins = 99;
        private int _minPixelValue;
        private int _maxPixelValue;
        IImageBox _referenceImageBox;
        #endregion

        #region constructor
        public WindowRangeApplicationComponent(IDesktopWindow desktopWindow, WindowRangeTool tool) : base(desktopWindow)
        {
            _tool = tool;
            _operation = new VoiLutImageOperation(SetWindow);
            _referenceImageBox = ImageViewer.SelectedImageBox;

            if (ImageViewer.SelectedPresentationImage is IVoiLutProvider)
            {
                _voiLutProvider = ImageViewer.SelectedPresentationImage as IVoiLutProvider;
                GetCurentWindowValues(true);
                
            }
            if(ImageViewer != null)
            {
                ImageViewer.EventBroker.PresentationImageSelected += PresentationImageSelected;
                _toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
            }
            if(ImageViewer.SelectedPresentationImage != null)
            {
                oldImage = this.ImageViewer.SelectedPresentationImage;
                
            }
            GetMinMaxValues();
            
        }
        #endregion

        #region public
        public double WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                _windowWidth = value;
                NotifyPropertyChanged("WindowWidth");
                SetMinMax();
                Apply();
            }
        }

        public double WindowCenter
        {
            get { return _windowCenter; }
            set
            {
                _windowCenter = value;
                NotifyPropertyChanged("WindowCenter");
                SetMinMax();
                Apply();
            }
        }

        public double WindowMin
        {
            get { return _windowMin; }
            set
            {
                Platform.Log(LogLevel.Debug, "WindowMin Set");
                _windowMin = value;
                //NotifyPropertyChanged("WindowMin");
                SetCentreWidth();
                Apply();
                //GetCurentWindowValues(false);
                Notify();
            }
        }

        public double WindowMax
        {
            get { return _windowMax; }
            set
            {
                Platform.Log(LogLevel.Debug, "WindowMax Set");
                _windowMax = value;
                //NotifyPropertyChanged("WindowMax");
                SetCentreWidth();
                Apply();
                //GetCurentWindowValues(false);
                Notify();
            }
        }

        public bool ComputeHistogram()
        {

            var grayDicom = this.ImageViewer.SelectedPresentationImage as DicomGrayscalePresentationImage;

            if (grayDicom != null)
            {
               
                    if (grayDicom.ImageGraphic != null)
                        return ComputeHistogram(grayDicom.ImageGraphic);
                
               
            }

            
            return false;
        }

        public int[] BinLabels
        {
            get { return _binLabels; }
        }

        public int[] Bins
        {
            get { return _bins; }
        }

        public int MinPixelValue
        {
            get { return _minPixelValue; }
            private set
            {
                _minPixelValue = value;
                NotifyPropertyChanged("MinPixelValue");
            }
        }

        public int MaxPixelValue
        {
            get { return _maxPixelValue; }
            private set
            {
                _maxPixelValue = value;
                NotifyPropertyChanged("MaxPixelValue");
            }
        }

        public int HistogramMin
        {
            get { return _histogramMin; }
            set
            {
                _histogramMin = value;
                checkNumBins();
                NotifyPropertyChanged("HistogramMin");
                OnHistogramAdjusted();
            }
        }

        public int HistogramMax
        {
            get { return _histogramMax; }
            set
            {
                _histogramMax = value;
                checkNumBins();
                NotifyPropertyChanged("HistogramMax");               
                OnHistogramAdjusted();
            }
        }

        public int NumBins
        {
            get { return _numBins; }
            set
            {
                _numBins = value;
                NotifyPropertyChanged("NumBins");
                OnHistogramAdjusted();
            }
        }

        #endregion

        #region override
        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            if (ImageViewer != null)
            {
                ImageViewer.EventBroker.PresentationImageSelected -= PresentationImageSelected;
                _toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
            }
        }
        #endregion

        #region private

        private bool ComputeHistogram(ImageGraphic image)
        {
            // For now, only allow ROIs of grayscale images
           
            if (image == null)
            {
                
                return false;
            }

            int left = 0;
            int right = image.Columns -1;
            int top = 0;
            int bottom = image.Rows -1;

            //// If any part of the ROI is outside the bounds of the image,
            //// don't allow a histogram to be displayed since it's invalid.
            //if (left < 0 || left > pixelData.Columns - 1 ||
            //    right < 0 || right > pixelData.Columns - 1 ||
            //    top < 0 || top > pixelData.Rows - 1 ||
            //    bottom < 0 || bottom > pixelData.Rows - 1)
            //{

            //    return false;
            //}

            var roiPixelData = new List<double>();
            
            image.PixelData.ForEachPixel(left, top, right, bottom, delegate (int j, int x, int y, int pixelIndex)
               {
                   //double pixelValue = mage.PixelData.GetPixel(pixelIndex);
                   //if (pixelValue > maxPixel)
                   //    maxPixel = pixelValue;
                   //if()
                   roiPixelData.Add(image.PixelData.GetPixel(pixelIndex));
               });
            
            if((int)roiPixelData.Max() < 100)
            {
                Histogram histogram = new Histogram(_histogramMin, _histogramMax, _numBins, roiPixelData.ToArray());

                _bins = histogram.Bins;
                _binLabels = histogram.BinLabels;
            }
            else
            {
                Histogram histogram = new Histogram(_histogramMin,_histogramMax, _numBins, roiPixelData.ToArray());

                _bins = histogram.Bins;
                _binLabels = histogram.BinLabels;
            }
           

            //NotifyPropertyChanged("MinBin");
            //NotifyPropertyChanged("MaxBin");
            //NotifyPropertyChanged("NumBins");

            //this.Enabled = true;
            return true;
        }
        private void SetMinMax()
        {
            _windowMin = _windowCenter - (_windowWidth / 2);
            _windowMax = _windowCenter + (_windowWidth / 2);
            Notify();
        }

        private void SetCentreWidth()
        {
            _windowWidth = Math.Round((_windowMax - _windowMin));
            _windowCenter = Math.Round(_windowMin + (_windowWidth/2));
           
        }

        public void Apply()
        {
            if (_operation.GetOriginator(_voiLutProvider as IPresentationImage) == null)
                return;

            ImageOperationApplicator applicator = new ImageOperationApplicator(_voiLutProvider as IPresentationImage, _operation);
            UndoableCommand historyCommand = _toolBehavior.Behavior.SelectedImageWindowLevelTool ? applicator.ApplyToReferenceImage() : applicator.ApplyToAllImages();
            if (historyCommand != null)
            {
                historyCommand.Name = SR.CommandInvert;
                if(_voiLutProvider is IPresentationImage)
                {
                    (_voiLutProvider as IPresentationImage).ImageViewer.CommandHistory.AddCommand(historyCommand);
                }
               
               
            }
        }
        private void SetWindow(IPresentationImage image)
        {
            //voiLut = null;
            if (!CanWindowLevel())
                return;
            


            IVoiLutManager manager = (IVoiLutManager)_operation.GetOriginator(image);
            var linearLut = manager.VoiLut as IVoiLutLinear;
            var standardLut = linearLut as IBasicVoiLutLinear;
            if (standardLut == null)
            {
                standardLut = new BasicVoiLutLinear(WindowWidth, WindowCenter);
                manager.InstallVoiLut(standardLut);
            }
            else
            {
                standardLut.WindowWidth = WindowWidth;
                standardLut.WindowCenter = WindowCenter;
            }

            //voiLut = standardLut;
            //_voiLutProvider.Draw();
        }

        private bool CanWindowLevel()
        {
            IVoiLutManager manager = GetSelectedImageVoiLutManager();
            return manager != null && manager.Enabled && manager.VoiLut is IVoiLutLinear;
        }

        private IVoiLutManager GetSelectedImageVoiLutManager()
        {
            return _voiLutProvider.VoiLutManager;
        }
        private void GetMinMaxValues()
        {
            var dicomGray = ImageViewer.SelectedPresentationImage as DicomGrayscalePresentationImage;
            if (dicomGray is null)
                return;

            (dicomGray.ImageGraphic.PixelData as GrayscalePixelData).CalculateMinMaxPixelValue(out _minPixelValue, out _maxPixelValue);
            NotifyPropertyChanged("MinPixelValue");
            NotifyPropertyChanged("MaxPixelValue");
            _histogramMin = _minPixelValue;
            _histogramMax = _maxPixelValue;
            NotifyPropertyChanged("HistogramMin");
            NotifyPropertyChanged("HistogramMax");
            checkNumBins();
        }

        private void checkNumBins()
        {
            if((_histogramMax - _histogramMin) < _numBins)
            {
                _numBins = _histogramMax - _histogramMin;
                NotifyPropertyChanged("NumBins");
            }
        }
        private void GetCurentWindowValues(bool initial)
        {
            if (CanWindowLevel())
            {
                var linearLut = GetSelectedImageVoiLutManager().VoiLut as IVoiLutLinear;
                _windowWidth = linearLut.WindowWidth;
                _windowCenter = linearLut.WindowCenter;
                if (initial)
                {
                    SetMinMax();
                    
                }
                    
            }
            else
            {
                _windowWidth = 0;
                _windowCenter = 0;
                _windowMax = 0;
                _windowMin = 0;
                _minPixelValue = 0;
                _maxPixelValue = 0;
            }
        }

        private void Notify()
        {
            NotifyPropertyChanged("WindowWidth");
            NotifyPropertyChanged("WindowCentre");
            NotifyPropertyChanged("WindowMin");
            NotifyPropertyChanged("WindowMax");
            
        }
        #endregion

        #region events
        public event EventHandler PresentationImageChanged;

        private void PresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
        {
            
            if (e.SelectedPresentationImage is null)
            {
                _voiLutProvider = null;
                WindowCenter = 0;
                WindowWidth = 0;
                return;
            }

            _voiLutProvider = e.SelectedPresentationImage as IVoiLutProvider;
            GetCurentWindowValues(true);
           if(ImageViewer.SelectedImageBox != _referenceImageBox)
            {
                GetMinMaxValues();
            }
            if (oldImage != e.SelectedPresentationImage)
            {
                PresentationImageChanged?.Invoke(this, new EventArgs());
                oldImage = e.SelectedPresentationImage;
            }

        }

        public event EventHandler HistogramAdjusted;
        
        private void OnHistogramAdjusted()
        {
            HistogramAdjusted?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}
