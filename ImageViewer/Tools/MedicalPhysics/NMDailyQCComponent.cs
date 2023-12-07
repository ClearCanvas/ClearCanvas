using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layout.Basic;


namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{
    [ExtensionPoint]
    public sealed class NMDailyQCComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    { }

    [AssociateView(typeof(NMDailyQCComponentViewExtensionPoint))]
    public class NMDailyQCComponent : ApplicationComponent
    {
        private Isotope _isotope;
        private IImageViewer _viewer;
        private double _d1Background, _d2Background;
        private double _d1Sensitivity, _d2Sensitivity;

        private DicomGrayscalePresentationImage _d1BackgroundImage, _d2BackgroundImage;
        private DicomGrayscalePresentationImage _d1FloodImage, _d2FloodImage;
        private IDisplaySet _background, _flood;

        private DateTime _sourceCalibrationDate;
        private float _sourceCalibrationValue;
        private DateTime _acquisitionDate;
        private float _sourceCurrentActivity;

        public NMDailyQCComponent(IImageViewer viewer) 
        {
            _viewer = viewer;
            _viewer.Closing += Viewer_Closing;

        }

        public DateTime SourceCalibrationDate
        {
            get
            {
                return _sourceCalibrationDate;
            }
        }

        public float SourceCalibrationValue
        {
            get { return _sourceCalibrationValue; }
        }

        public DateTime AcquisitionDate
        {
            get { return _acquisitionDate; }
        }

        public double D1Background
        {
            get { return _d1Background; }
        } 
        public double D2Background 
        { get { return _d2Background; } }

        public double D1Sensitivity
        {
            get { return _d1Sensitivity; }
        }
        public double D2Sensitivity
        {
            get { return (double)_d2Sensitivity; }
        }

        public float SourceCurrentActivity
        {
            get { return _sourceCurrentActivity; }
        }
        private void Viewer_Closing(object sender, EventArgs e)
        {
            _viewer.Closing -= Viewer_Closing;
            Exit(ApplicationComponentExitCode.None);
        }

        public override void Start()
        {
            base.Start();
            SortDisplaySets();
            _sourceCalibrationValue = Properties.Settings.Default.GammaCameraSourceActivity;
            _sourceCalibrationDate = Properties.Settings.Default.GammaCameraSourceRerefenceDate;
            _acquisitionDate = new DateTime(Convert.ToInt16(_viewer.StudyTree.Studies.First().StudyDate.Substring(0,4)),
                Convert.ToInt16(_viewer.StudyTree.Studies.First().StudyDate.Substring(4, 2)),
                Convert.ToInt16(_viewer.StudyTree.Studies.First().StudyDate.Substring(6, 2)));
            LayoutComponent.SetImageBoxLayout(_viewer, 1, 3);
            _viewer.PhysicalWorkspace.ImageBoxes[0].SetTileGrid(2, 1);
            _viewer.PhysicalWorkspace.ImageBoxes[0].SetDisplaySet(_background);
            _viewer.PhysicalWorkspace.ImageBoxes[1].SetTileGrid(2, 1);
            _viewer.PhysicalWorkspace.ImageBoxes[1].SetDisplaySet(_flood);

            CalculateValues();
            _viewer.PhysicalWorkspace.ImageBoxes[2].Tiles[0].Select();
        }

        public override void Stop()
        {
            base.Stop();
            _viewer.Closing -= Viewer_Closing;
        }

        private void CalculateValues()
        {
            _isotope = Isotope.Isotopes.Where(x => x.FullName == Properties.Settings.Default.GammaCameraSourceIsotope).First();
            _sourceCurrentActivity = (float)_isotope.GetActivity(_sourceCalibrationValue, _sourceCalibrationDate, _acquisitionDate);
            
            
            _d1Background = GetCountRate(_d1BackgroundImage);
            NotifyPropertyChanged(nameof(D1Background));
            
            _d2Background = GetCountRate(_d2BackgroundImage);
            NotifyPropertyChanged(nameof(D2Background));

            

            _d1Sensitivity = (GetCountRate(_d1FloodImage) * 1000.0) / _sourceCurrentActivity;
            _d2Sensitivity = (GetCountRate(_d2FloodImage) * 1000.0) / _sourceCurrentActivity;

        }

        private double GetCountRate(DicomGrayscalePresentationImage image)
        {
            int totalCounts = 0;
            int acquisitionDuration = 0;
            if (image.ImageSop.TryGetAttribute(DicomTags.ActualFrameDuration, out DicomAttribute dur))
            {
                acquisitionDuration = dur.GetInt32(0, 1);
            }
            var modalityLut = image.ModalityLut;
            var pixelData = image.ImageGraphic.PixelData;
            pixelData.ForEachPixel(
                        delegate (int i, int x, int y, int pixelIndex)
                        {
                            int storedValue = pixelData.GetPixel(pixelIndex);
                            double realValue = modalityLut != null ? modalityLut[storedValue] : storedValue;
                            totalCounts += Convert.ToInt32(realValue);
                        });
            return ((double)totalCounts / ((double)acquisitionDuration/1000))/1000;
        }

        private void SortDisplaySets()
        {
            double rate = GetCountRate(_viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0].PresentationImages[0] as DicomGrayscalePresentationImage);
            double rate2 = GetCountRate(_viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1].PresentationImages[0] as DicomGrayscalePresentationImage);
            if(rate < rate2)
            {
                _d1BackgroundImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0].PresentationImages[0] as DicomGrayscalePresentationImage;
                _d2BackgroundImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0].PresentationImages[1] as DicomGrayscalePresentationImage;

                _d1FloodImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1].PresentationImages[0] as DicomGrayscalePresentationImage;
                _d2FloodImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1].PresentationImages[1] as DicomGrayscalePresentationImage;

                _background = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0];
                _flood = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1];
            }
            else
            {
                _d1BackgroundImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1].PresentationImages[0] as DicomGrayscalePresentationImage;
                _d2BackgroundImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1].PresentationImages[1] as DicomGrayscalePresentationImage;

                _d1FloodImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0].PresentationImages[0] as DicomGrayscalePresentationImage;
                _d2FloodImage = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0].PresentationImages[1] as DicomGrayscalePresentationImage;

                _background = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[1];
                _flood = _viewer.LogicalWorkspace.ImageSets[0].DisplaySets[0];
            }
            
        }

    }
}
