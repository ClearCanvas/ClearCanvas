using ClearCanvas.Common;
using NPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Tools.Measurement.RoiAnalysis.View.WinForms
{
    public partial class RoiAnalysisComponentContainerControl : UserControl
    {
        RoiAnalysisComponentContainer _component;
        private BindingSource _bindingSource;
        
        public RoiAnalysisComponentContainerControl(RoiAnalysisComponentContainer component)
        {
           
            _component = component;
           
            _component.Stopped += Component_Stopped;
            _component.AllPropertiesChanged += Component_AllPropertiesChanged;
           

            InitializeComponent();
            //_bindingSource = new BindingSource();
            //_bindingSource.DataSource = _component;
           
            _minUpDown.Minimum = Decimal.MinValue;
            _minUpDown.Maximum = Decimal.MaxValue;
            _minUpDown.Increment = 10;
            _minUpDown.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
            _minUpDown.Accelerations.Add(new NumericUpDownAcceleration(5, 100));

            _maxUpDown.Minimum = Decimal.MinValue;
            _maxUpDown.Maximum = Decimal.MaxValue;
            _maxUpDown.Increment = 10;
            _maxUpDown.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
            _maxUpDown.Accelerations.Add(new NumericUpDownAcceleration(5, 100));

            _numBinsUpDown.Minimum = 1;
            _numBinsUpDown.Maximum = 200;
            _numBinsUpDown.Increment = 5;

            _minUpDown.DataBindings.Add("Value", _component, "MinBin", true, DataSourceUpdateMode.OnPropertyChanged);           
            _maxUpDown.DataBindings.Add("Value", _component, "MaxBin", true, DataSourceUpdateMode.OnPropertyChanged);          
            _numBinsUpDown.DataBindings.Add("Value", _component, "NumBins", true, DataSourceUpdateMode.OnPropertyChanged);          

            _roiInfoTable.Table = _component.RoiInfoItems;
            RefreshPlot();
        }

        private void Component_Stopped(object sender, EventArgs e)
        {
            _component.AllPropertiesChanged -= Component_AllPropertiesChanged;
            
        }

        private void Component_AllPropertiesChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_component.IsStarted)
                    return;

                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        RefreshView();
                        RefreshPlot();
                    }));
                }
                else
                {
                    RefreshView();
                    RefreshPlot();
                }
            }
            catch(Exception ex)
            {

            }
            
            
            

        }

        private void RefreshView()
        {
           
            _roiInfoTable.Refresh();

        }

        private void RefreshPlot()
        {
            _plotSurface.Clear();
            _plotSurface.BackColor = Color.White;



            //if (!_component.ComputeHistogram())
            //{
            //    _plotSurface.Refresh();
            //    return;
            //}

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
        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
