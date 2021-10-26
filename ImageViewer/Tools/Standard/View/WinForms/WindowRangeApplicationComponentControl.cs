using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NPlot;
using System.Drawing;
namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    public partial class WindowRangeApplicationComponentControl : UserControl
    {
        private readonly WindowRangeApplicationComponent _component;
        private BindingSource source;

        public WindowRangeApplicationComponentControl(WindowRangeApplicationComponent component)
        {
            _component = component;
            _component.PresentationImageChanged += _component_PresentationImageChanged;
            _component.HistogramAdjusted += _component_HistogramAdjusted;
            InitializeComponent();
            source = new BindingSource();
            source.DataSource = _component;
          
            _minTracker.DataBindings.Add(new Binding("Value", source, "WindowMin", true, DataSourceUpdateMode.OnPropertyChanged));          
            _maxTracker.DataBindings.Add(new Binding("Value", source, "WindowMax", true, DataSourceUpdateMode.OnPropertyChanged));
            _minTracker.DataBindings.Add(new Binding("Minimum", source, "MinPixelValue", true, DataSourceUpdateMode.OnPropertyChanged));
            _minTracker.DataBindings.Add(new Binding("Maximum", source, "MaxPixelValue", true, DataSourceUpdateMode.OnPropertyChanged));
            _maxTracker.DataBindings.Add(new Binding("Minimum", source, "MinPixelValue", true, DataSourceUpdateMode.OnPropertyChanged));
            _maxTracker.DataBindings.Add(new Binding("Maximum", source, "MaxPixelValue", true, DataSourceUpdateMode.OnPropertyChanged));
            _histMin.DataBindings.Add(new Binding("Value", source, "HistogramMin", true, DataSourceUpdateMode.OnPropertyChanged));
            _histMax.DataBindings.Add(new Binding("Value", source, "HistogramMax", true, DataSourceUpdateMode.OnPropertyChanged));
            _histBins.DataBindings.Add(new Binding("Value", source, "NumBins", true, DataSourceUpdateMode.OnPropertyChanged));
            RefreshPlot();
           
        }

        private void _component_HistogramAdjusted(object sender, EventArgs e)
        {
            RefreshPlot();
        }

        private void _component_PresentationImageChanged(object sender, EventArgs e)
        {
            RefreshPlot();
        }

        private void RefreshPlot()
        {
            _plotSurface.Clear();
            _plotSurface.BackColor = Color.White;



            if (!_component.ComputeHistogram())
            {
                _plotSurface.Refresh();
                return;
            }

            HistogramPlot histogram = new HistogramPlot();
            histogram.AbscissaData = _component.BinLabels;
            histogram.OrdinateData = _component.Bins;
            histogram.Center = false;
            histogram.BaseWidth = 1.0f;
            histogram.Filled = true;
            histogram.Pen = new Pen(Color.Gray);
            histogram.RectangleBrush = new RectangleBrushes.Solid(Color.Gray);

            _plotSurface.Add(histogram);
            _plotSurface.PlotBackColor = Color.White;
            _plotSurface.XAxis1.Color = Color.Black;
            _plotSurface.YAxis1.Color = Color.Black;
            _plotSurface.Refresh();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void WindowRangeApplicationComponentControl_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void CenterUpDown_Scroll(object sender, ScrollEventArgs e)
        {
           
        }
    }
}
