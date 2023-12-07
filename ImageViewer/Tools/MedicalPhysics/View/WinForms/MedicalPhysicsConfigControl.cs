using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    public partial class MedicalPhysicsConfigControl : UserControl
    {
        private MedicalPhysicsConfigComponent _component;

        public MedicalPhysicsConfigControl(MedicalPhysicsConfigComponent component)
        {
            _component = component;
            InitializeComponent();
            FloodCalibrationDateField.DataBindings.Add("Value", _component, "FloodSourceCalibrationDate",
                true, DataSourceUpdateMode.OnPropertyChanged, DateTime.Now);

            FloodSourceActivityField.DataBindings.Add("Value", _component, "FloodSourceCalibrationActivity",
                true, DataSourceUpdateMode.OnPropertyChanged, 0, "# MBq");

            FloodSourceSerialNumberField.DataBindings.Add("Value", _component, "FloodSourceSerialNumber", false, DataSourceUpdateMode.OnPropertyChanged);
            IsotopesField.DataBindings.Add(nameof(ComboBoxField.DataSource), _component, nameof(MedicalPhysicsConfigComponent.Isotopes));
            IsotopesField.DataBindings.Add(nameof(ComboBoxField.Value), _component, nameof(MedicalPhysicsConfigComponent.Isotope));

        }
    }
}
