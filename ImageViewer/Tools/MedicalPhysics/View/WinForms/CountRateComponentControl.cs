using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.ImageViewer.Tools.MedicalPhysics;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.View.WinForms
{
    public partial class CountRateComponentControl : UserControl
    {
        private readonly CountRateComponent _component;
        public CountRateComponentControl(CountRateComponent component)
        {
            InitializeComponent();
            _component = component;
            DurationBox.DataBindings.Add("Text", _component, "AcquisitionDuration",true,DataSourceUpdateMode.OnPropertyChanged,0,"#,# msec");
            TotalCountsBox.DataBindings.Add("Text", _component, "TotalCounts", true, DataSourceUpdateMode.OnPropertyChanged,null,"#,# cnts");
            CountRateBox.DataBindings.Add("Text", _component, "CountRate", true, DataSourceUpdateMode.OnPropertyChanged, null, "0.000 kcnts/sec");
            _referenceActivityBox.DataBindings.Add("Text", _component, "SourceReferenceActivity", true, DataSourceUpdateMode.OnPropertyChanged, null, "0 MBq");
            _currentActivityBox.DataBindings.Add("Text", _component, "SourceCurrentActivity", true, DataSourceUpdateMode.OnPropertyChanged, null, "0 MBq");
            _referenceDateBox.DataBindings.Add("Text", _component, "SourceReferenceDate", true, DataSourceUpdateMode.OnPropertyChanged, null, "MMMMM d, yyyy");
            _sensitivityBox.DataBindings.Add("Text", _component, "Sensitivity", true, DataSourceUpdateMode.OnPropertyChanged, null, "0.0 cps/MBq");
            _acquisitionDateBox.DataBindings.Add("Text", _component, "ImageAcquisitionDate", true, DataSourceUpdateMode.OnPropertyChanged, null, "MMMMM d, yyyy");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
