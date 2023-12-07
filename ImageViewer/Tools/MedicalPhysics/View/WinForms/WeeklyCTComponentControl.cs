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
            HighContrastTextBox.DataBindings.Add("Text", _component, "HighContrastResolution", true, DataSourceUpdateMode.OnPropertyChanged,0,"#.0 HU");
            ContrastScaleTextBox.DataBindings.Add("Text", _component, "ContrastScale", true, DataSourceUpdateMode.OnPropertyChanged, 0, "#.0 HU");
            WaterValueTextBox.DataBindings.Add("Text", _component, "WaterValue", true, DataSourceUpdateMode.OnPropertyChanged, 0, "0.0 HU");
        }

        
    }
}
