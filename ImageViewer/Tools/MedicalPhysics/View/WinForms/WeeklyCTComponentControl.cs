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
    public partial class WeeklyCTComponentControl : UserControl
    {
        private WeeklyCTComponent _component;
        public WeeklyCTComponentControl(WeeklyCTComponent component)
        {
            InitializeComponent();
            _component = component;
            HighContrastTextField.DataBindings.Add("Value", _component, "HighContrastResolution", true, DataSourceUpdateMode.OnPropertyChanged,0,"#.0 HU");
            
            ContrastScaleTextField.DataBindings.Add("Value", _component, "ContrastScale", true, DataSourceUpdateMode.OnPropertyChanged, 0, "#.0 HU");
            WaterValueTextField.DataBindings.Add("Value", _component, "WaterValue", true, DataSourceUpdateMode.OnPropertyChanged, 0, "0.0 HU");
        }

        private void textField1_Load(object sender, EventArgs e)
        {

        }
    }
}
