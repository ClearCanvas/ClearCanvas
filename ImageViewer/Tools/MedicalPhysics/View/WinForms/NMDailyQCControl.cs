using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    public partial class NMDailyQCControl : UserControl
    {
        private NMDailyQCComponent _component;
        public NMDailyQCControl(NMDailyQCComponent component)
        {
            InitializeComponent();
            _component = component;
            SourceCalibrationDateField.DataBindings.Add(new Binding("Value", _component, "SourceCalibrationDate",true,DataSourceUpdateMode.OnPropertyChanged,null,"ddddd d MMM yyyy"));
            SourceCalibrationActivityField.DataBindings.Add(new Binding("Value", _component, "SourceCalibrationValue", true, DataSourceUpdateMode.OnPropertyChanged, null, "# MBq"));
            AcquisitionDateField.DataBindings.Add(new Binding("Value", _component, "AcquisitionDate", true, DataSourceUpdateMode.OnPropertyChanged, null, "ddddd d MMM yyyy"));
            D1BackgroundField.DataBindings.Add(new Binding("Value", _component, "D1Background", true, DataSourceUpdateMode.OnPropertyChanged, null, "0.000 kcps"));
            D2BackgroundField.DataBindings.Add(new Binding("Value", _component, "D2Background", true, DataSourceUpdateMode.OnPropertyChanged, null, "0.000 kcps"));
            SourceCurrentField.DataBindings.Add(new Binding("Value", _component, "SourceCurrentActivity", true, DataSourceUpdateMode.OnPropertyChanged, null, "# MBq"));
            D1Sensitivity.DataBindings.Add(new Binding("Value", _component, nameof(NMDailyQCComponent.D1Sensitivity), true, DataSourceUpdateMode.OnPropertyChanged, null, "#.0 cps/MBq"));
            D2Sensitivity.DataBindings.Add(new Binding("Value", _component, nameof(NMDailyQCComponent.D2Sensitivity), true, DataSourceUpdateMode.OnPropertyChanged, null, "#.0 cps/MBq"));
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }
    }
}
