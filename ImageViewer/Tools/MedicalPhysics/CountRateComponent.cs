using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;


namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{
    /// <summary>
    /// Extension point for views onto <see cref="CountRateComponent"/>
    /// </summary>
    [ExtensionPoint]
    public sealed class CountRateComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {

    }

    /// <summary>
    /// DailyQCComponentClass
    /// </summary>
    [AssociateView(typeof(CountRateComponentViewExtensionPoint))]
    public class CountRateComponent : ApplicationComponent
    {
        private int _acquisitionDuration;
        private int _totalCounts;
       
        private double _countRate;
        private readonly IDesktopWindow _desktopWindow;
        private IImageViewer _viewer;
        private string _sourceSerialNumber;
        private Isotope _isotope;
        private DateTime _sourceReferenceDate;
        private DateTime _acquisitionDate;
        private double _referenceActivity;
        private double _sourceCurrentActivity;
        private double _sensitivity;

        /// <summary>
        /// Constructor
        /// </summary>
        public CountRateComponent(IDesktopWindow desktopWindow)
        {
            _totalCounts = 0;

            _desktopWindow = desktopWindow;
            _isotope = Isotope.Isotopes.Where(x => x.FullName == Properties.Settings.Default.GammaCameraSourceIsotope).First();
            _referenceActivity = Properties.Settings.Default.GammaCameraSourceActivity;
            _sourceReferenceDate = Properties.Settings.Default.GammaCameraSourceRerefenceDate;
            _sourceSerialNumber = Properties.Settings.Default.GammaCameraSourceSerialNumber;
            _sourceCurrentActivity = SourceIsotope.GetCurrentActivity(_referenceActivity, _sourceReferenceDate);
        }

        /// <summary>
        /// The total acqusition time in msec
        /// </summary>
        public int AcquisitionDuration
        {
            get { return _acquisitionDuration; }
        }

        /// <summary>
        /// The total number of counts in the acquisition
        /// </summary>
        public int TotalCounts
        {
            get { return _totalCounts; }
        }

        /// <summary>
        /// The total counts divided by the acquisition duration in kcnts/sec
        /// </summary>
        public double CountRate
        {
            get
            {
                return _countRate;
            }
        }

        /// <summary>
        /// The serial numver of the source used in the daily QC
        /// </summary>
        public string SourceSerialNumber
        {
            get { return _sourceSerialNumber; }
        }

        /// <summary>
        /// The Isotope in the source used for daily QC
        /// </summary>
        public Isotope SourceIsotope
        {
            get { return _isotope; }
        }

        /// <summary>
        /// The reference Date of the source
        /// </summary>
        public DateTime SourceReferenceDate
        {
            get { return _sourceReferenceDate; }
        }

        /// <summary>
        /// The date time at which the image was acquired.
        /// </summary>
        public DateTime ImageAcquisitionDate
        {
            get { return _acquisitionDate; }
        }

        /// <summary>
        /// The reference activity in the source
        /// </summary>
        public double SourceReferenceActivity
        {
            get { return _referenceActivity; }
        }

        /// <summary>
        /// The current activity in the source
        /// </summary>
        public double SourceCurrentActivity
        {
            get { return _sourceCurrentActivity; }
        }

        public double Sensitivity
        {
            get { return _sensitivity; }
        }

        #region overrides
        public override void Start()
        {
            SetImageViewer(_desktopWindow.ActiveWorkspace);
            _desktopWindow.Workspaces.ItemActivationChanged += OnActiveWorkspaceChanged;
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();

            _desktopWindow.Workspaces.ItemActivationChanged -= OnActiveWorkspaceChanged;
            SetImageViewer(null);

        }
        #endregion

        #region private

        private void UpdateData(IPresentationImage presentationImage)
        {
            Platform.Log(LogLevel.Debug, "UpdateData");
            if (presentationImage is null)
            {
                _totalCounts = 0;
                _acquisitionDuration = 0;

                NotifyAllPropertiesChanged();
            }
            else
            {
                if (presentationImage is DicomGrayscalePresentationImage)
                {
                    var dicomImage = presentationImage as DicomGrayscalePresentationImage;
                    if (dicomImage.ImageSop.TryGetAttribute(DicomTags.ActualFrameDuration, out DicomAttribute dur))
                    {
                        _acquisitionDuration = dur.GetInt32(0, 1);

                    }
                    if (dicomImage.ImageSop.TryGetAttribute(DicomTags.AcquisitionDate, out DicomAttribute acqDate))
                    {
                        _acquisitionDate = acqDate.GetDateTime(0, new DateTime(2022,07,01));
                        _sourceCurrentActivity = SourceIsotope.GetActivity(_referenceActivity, _sourceReferenceDate, _acquisitionDate);
                        NotifyPropertyChanged("SourceCurrentActivity");
                        
                        
                    }
                    var pixelData = dicomImage.ImageGraphic.PixelData;
                    var modalityLut = dicomImage.ModalityLut;
                    _totalCounts = 0;
                    pixelData.ForEachPixel(
                        delegate (int i, int x, int y, int pixelIndex)
                        {
                            int storedValue = pixelData.GetPixel(pixelIndex);
                            double realValue = modalityLut != null ? modalityLut[storedValue] : storedValue;
                            _totalCounts += Convert.ToInt32(realValue);
                        });
                    _countRate = (_totalCounts / 1000.0) / (_acquisitionDuration / 1000.0);
                    NotifyPropertyChanged("AcquisitionDuration");
                    NotifyPropertyChanged("TotalCounts");
                    NotifyPropertyChanged("CountRate");
                  
                    _sensitivity = (_countRate*1000.0) / _sourceCurrentActivity;
                    NotifyPropertyChanged("Sensitivity");
                }
                else
                {
                    _totalCounts = 0;
                    _acquisitionDuration = 0;

                    NotifyAllPropertiesChanged();
                }
            }
        }
        private void OnActiveWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
        {
            SetImageViewer(e.Item);
        }

        private static IImageViewer CastToImageViewer(Workspace workspace)
        {
           
            IImageViewer viewer = null;
            if (workspace != null)
                viewer = ImageViewerComponent.GetAsImageViewer(workspace);

            return viewer;
        }

        private void SetImageViewer(Workspace workspace)
        {
           
            IImageViewer viewer = CastToImageViewer(workspace);
            if (viewer != _viewer)
            {
                if (_viewer != null)
                {
                    _viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;
                    _viewer.EventBroker.TileSelected -= OnTileSelected;
                    
                    
                }
                _viewer = viewer;
                if (_viewer != null)
                {
                    _viewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
                    _viewer.EventBroker.TileSelected += OnTileSelected;
                   
                    
                    UpdateData(viewer.SelectedPresentationImage);
                }
                else
                {
                    UpdateData(null);
                }
            }
        }

        private void OnTileSelected(object sender, TileSelectedEventArgs e)
        {
            
            //if (e.SelectedTile.PresentationImage != null)
                //UpdateData(e.SelectedTile.PresentationImage);
           
        }

        private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
        {
            Platform.Log(LogLevel.Debug, "OnPresentationImageSelected " +e.SelectedPresentationImage.Uid);
            UpdateData(e.SelectedPresentationImage);
        }
        #endregion
    }
}
