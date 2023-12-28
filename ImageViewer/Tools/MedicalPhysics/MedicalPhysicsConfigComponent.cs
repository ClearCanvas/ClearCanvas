using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{

    [ExtensionPoint]
    public class MedicalPhysicsConfigComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

    [AssociateView(typeof(MedicalPhysicsConfigComponentViewExtensionPoint))]
    public class MedicalPhysicsConfigComponent : ConfigurationApplicationComponent
    {
        private DateTime _floodSourceCalibrationDate;
        private float _floodSourceCalibrationActivity;
        private string _floodSourceSerialNumber;
        private Isotope _isotope;
        private List<Isotope> _isotopes;

        public DateTime FloodSourceCalibrationDate
        {
            get { return _floodSourceCalibrationDate; }
            set 
            { 
                if(_floodSourceCalibrationDate != value)
                {
                    _floodSourceCalibrationDate = value;
                    base.NotifyPropertyChanged("FloodSourceCalibrationDate");
                    base.Modified = true;
                }
               
            }
        }

        public float FloodSourceCalibrationActivity
        {
            get { return _floodSourceCalibrationActivity; }
            set 
            { 
                if(_floodSourceCalibrationActivity != value)
                {
                    _floodSourceCalibrationActivity = value;
                    base.NotifyPropertyChanged("FloodSourceCalibrationActivity");
                    base.Modified = true;
                }
               
            }
        }

        public string FloodSourceSerialNumber
        {
            get { return _floodSourceSerialNumber; }
            set
            {
                if(_floodSourceSerialNumber != value)
                {
                    _floodSourceSerialNumber = value;
                    NotifyPropertyChanged("FloodSourceSerialNumber");
                    Modified = true;
                }
            }
        }

        public Isotope Isotope
        {
            get { return _isotope; }
            set 
            { 
                if(_isotope != value)
                {
                    _isotope = value;
                    NotifyPropertyChanged(nameof(Isotope));
                    Modified = true;
                }
            }
        }

        public List<Isotope> Isotopes
        {
            get
            {
                if (_isotopes == null)
                    _isotopes = Isotope.Isotopes.OrderBy(x => x.FullName).ToList();
                return _isotopes;
            }
        }

        public override void Start()
        {
            base.Start();
            _floodSourceCalibrationDate = Properties.Settings.Default.GammaCameraSourceRerefenceDate;
            _floodSourceCalibrationActivity = Properties.Settings.Default.GammaCameraSourceActivity;
            _floodSourceSerialNumber = Properties.Settings.Default.GammaCameraSourceSerialNumber;
            _isotope = Isotope.Isotopes.Where(x => x.FullName == Properties.Settings.Default.GammaCameraSourceIsotope).First();
        }

       
        public override void Save()
        {
            Properties.Settings.Default.GammaCameraSourceRerefenceDate = _floodSourceCalibrationDate;
            Properties.Settings.Default.GammaCameraSourceActivity = _floodSourceCalibrationActivity;
            Properties.Settings.Default.GammaCameraSourceSerialNumber = _floodSourceSerialNumber;
            Properties.Settings.Default.GammaCameraSourceIsotope = _isotope.FullName;
            Properties.Settings.Default.Save();
        }
    }
}
