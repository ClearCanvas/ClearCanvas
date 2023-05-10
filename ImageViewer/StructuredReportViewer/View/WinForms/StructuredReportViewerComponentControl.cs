using ClearCanvas.Dicom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Tools.StructuredReportViewer.View.WinForms
{
    public partial class StructuredReportViewerComponentControl : UserControl
    {

        private StructuredReportViewerComponent _component;
        public StructuredReportViewerComponentControl(StructuredReportViewerComponent component)
        {
            InitializeComponent();
            _component = component;
            //CollectionBox.DataBindings.Add("Text", _component, "DumpString", false, DataSourceUpdateMode.Never, 0);
            PatientNameLabel.DataBindings.Add("Text", _component, "PatientName", false, DataSourceUpdateMode.Never, "Unknown");
            PatientBirthDateLabel.DataBindings.Add("Text", _component, "PatientBirthDate", false, DataSourceUpdateMode.Never, "Unknown");
            PatientIdLabel.DataBindings.Add("Text", _component, "PatientID", false, DataSourceUpdateMode.Never, "Unknown");
            LabelTitle.DataBindings.Add("Text", _component, "ReportTitle", false, DataSourceUpdateMode.Never, "Unknown");
            ReportDateLabel.DataBindings.Add("Text", _component, "ReportDate", false, DataSourceUpdateMode.Never, "Unknown");
            PatientSexLabel.DataBindings.Add("Text", _component, "PatientSex", false, DataSourceUpdateMode.Never, "Unknown");
            CompletionFlagLabel.DataBindings.Add("Text", _component, "CompletionFlag", false, DataSourceUpdateMode.Never, "Unknown");
            VerificationFlagLabel.DataBindings.Add("Text", _component, "VerificationFlag", false, DataSourceUpdateMode.Never, "Unknown");
            StudyDateLabel.DataBindings.Add("Text", _component, "StudyDate", false, DataSourceUpdateMode.Never, "Unknown");
            StudyLabel.DataBindings.Add("Text", _component, "Study", false, DataSourceUpdateMode.Never, "Unknown");
            AccessionNumberLabel.DataBindings.Add("Text", _component, "AccessionNumber", false, DataSourceUpdateMode.Never, "Unknown");
            ReferrerLabel.DataBindings.Add("Text", _component, "Referrer", false, DataSourceUpdateMode.Never, "Unknown");

            for(int i = 0; i < component.ContentSequnce.Count; i++)
            {
                DicomSequenceItem q = component.ContentSequnce[i];
                
                ContentPanel.Controls.Add(ConvertSequenceItem(q,i+1,0));
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FlowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LabelAccessionNumber_Click(object sender, EventArgs e)
        {

        }

        private void LabelStudyDate_Click(object sender, EventArgs e)
        {

        }

        private Control ConvertSequenceItem(DicomSequenceItem item, int counter, int indentLevel)
        {
            if (item.TryGetAttribute(DicomTags.ValueType, out var type))
            {
                FlowLayoutPanel panel = new FlowLayoutPanel();
                panel.AutoSize = true;
                //panel.MaximumSize = new Size(500, 1000);
                panel.FlowDirection = FlowDirection.LeftToRight;
                if (item.TryGetAttribute(DicomTags.ConceptNameCodeSequence, out var codeSequence))
                {
                    var conceptNameCodeSequenceItem = (codeSequence as DicomAttributeSQ)[0];
                    if (conceptNameCodeSequenceItem.TryGetAttribute(DicomTags.CodeMeaning, out DicomAttribute att))
                    {
                        //for (int c = 0; c < indentLevel; c++)
                        //{
                        //    //panel.Controls.Add(new Label { Text = "  ", AutoSize = true, Font = new Font("Segoe UI", 11) });
                        //}
                        panel.Controls.Add(new Label { Text = counter.ToString() + " " + att.ToString() + ": ", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), Margin = new Padding(0) });
                    }
                }
                switch (type.ToString())
                {
                    case "TEXT":
                        if(item.TryGetAttribute(DicomTags.TextValue, out var textValue))
                        {
                           
                            panel.Controls.Add(new Label { Text = textValue.ToString(), AutoSize = true, Font = new Font("Segoe UI", 11) });
                            return panel;
                        }
                        if (item.TryGetAttribute(DicomTags.ContentSequence, out var contentSequenceA))
                        {
                            FlowLayoutPanel master = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
                            master.Controls.Add(panel);
                            FlowLayoutPanel offset1 = new FlowLayoutPanel();
                            offset1.FlowDirection = FlowDirection.LeftToRight;
                            offset1.AutoSize = true;
                            offset1.Margin = new Padding(0);
                            FlowLayoutPanel vertical = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
                            //vertical.Controls.Add(panel);
                            var seq = contentSequenceA as DicomAttributeSQ;
                            for (int j = 0; j < seq.Count; j++)
                            {


                                vertical.Controls.Add(ConvertSequenceItem(seq[j], j + 1, indentLevel + 1));
                            }
                            offset1.Controls.Add(new Label { Text = " ", AutoSize = true });
                            offset1.Controls.Add(vertical);
                            master.Controls.Add(offset1);
                            return master;
                            //return vertical;
                        }
                        return null;
                    case "NUM":
                        if (item.TryGetAttribute(DicomTags.MeasuredValueSequence, out var numericSequence))
                        {
                            var measurement = (numericSequence as DicomAttributeSQ)[0];
                            if (measurement is null)
                                return panel;
                            if(measurement.TryGetAttribute(DicomTags.NumericValue,out var numericValue))
                            {
                                if (measurement.TryGetAttribute(DicomTags.MeasurementUnitsCodeSequence, out var measurementUnitsCodeSequence))
                                {
                                    if((measurementUnitsCodeSequence as DicomAttributeSQ)[0].TryGetAttribute(DicomTags.CodeMeaning,out var codeMeaning))
                                     {
                                        panel.Controls.Add(new Label { Text = numericValue.ToString() + " " +codeMeaning.ToString(), Font = new Font("Segoe UI", 11), AutoSize = true });
                                    }
                                }
                                else 
                                {
                                    panel.Controls.Add(new Label { Text = numericValue.ToString(), Font = new Font("Segoe UI", 11) });
                                }

                                

                            }
                            
                        }
                        if (item.TryGetAttribute(DicomTags.ContentSequence, out var contentSequenceB))
                        {
                            FlowLayoutPanel master = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
                            master.Controls.Add(panel);
                            FlowLayoutPanel offset1 = new FlowLayoutPanel();
                            offset1.FlowDirection = FlowDirection.LeftToRight;
                            offset1.AutoSize = true;
                            offset1.Margin = new Padding(0);
                            FlowLayoutPanel vertical = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
                            //vertical.Controls.Add(panel);
                            var seq = contentSequenceB as DicomAttributeSQ;
                            for (int j = 0; j < seq.Count; j++)
                            {


                                vertical.Controls.Add(ConvertSequenceItem(seq[j], j + 1, indentLevel + 1));
                            }
                            offset1.Controls.Add(new Label { Text = " ", AutoSize = true });
                            offset1.Controls.Add(vertical);
                            master.Controls.Add(offset1);
                            return master;
                            //return vertical;
                        }
                        return panel;
                    case "DATE":
                    case "TIME":
                        return panel;
                    case "DATETIME":
                        if(item.TryGetAttribute(DicomTags.Datetime,out var dateTimeValue))
                        {
                            if (dateTimeValue.TryGetDateTime(0, out var dateTime))
                            {
                                panel.Controls.Add(new Label { Text = dateTime.ToString(), AutoSize = true, Font = new Font("Segoe UI", 11) });
                            }
                        }
                        return panel;
                    case "UIDREF":
                        if (item.TryGetAttribute(DicomTags.Uid, out var uid))
                        {
                            
                            panel.Controls.Add(new Label { Text = uid.ToString(), AutoSize = true, Font = new Font("Segoe UI", 11) });
                           
                        }
                        return panel;
                    case "PNAME":
                    case "COMPOSITE":
                    case "IMAGE":
                    case "WAVEFORM":
                    case "SCOORD":
                    case "SCOORD3D":
                    case "TCOORD":
                        return panel;
                    case "CONTAINER":
                        FlowLayoutPanel offset = new FlowLayoutPanel();
                        offset.FlowDirection = FlowDirection.LeftToRight;
                        offset.AutoSize = true;
                        offset.Margin = new Padding(0);
                        //offset.BackColor = Color.Orange;
                        FlowLayoutPanel list = new FlowLayoutPanel();
                        list.AutoSize= true;
                        list.FlowDirection = FlowDirection.TopDown;
                        //list.Controls.Add(new Label { Text = "    "  });
                        //list.BackColor = Color.Green;
                        panel.FlowDirection = FlowDirection.TopDown;
                        var container = item.GetAttribute(DicomTags.ContentSequence) as DicomAttributeSQ;
                        for (int i = 0; i < container.Count; i++)
                        {
                            DicomSequenceItem q = container[i];
                            list.Controls.Add(ConvertSequenceItem(q, i + 1, indentLevel+1));
                        }
                        offset.Controls.Add(new Label { Text = " ", AutoSize = true });
                        offset.Controls.Add(list);
                        panel.Controls.Add(offset);
                        return panel;
                        //break;
                    case "CODE":
                       
                        if (item.TryGetAttribute(DicomTags.ConceptCodeSequence, out var codeConcept))
                        {
                            var conceptCodeSequenceItem = (codeConcept as DicomAttributeSQ)[0];
                            if (conceptCodeSequenceItem.TryGetAttribute(DicomTags.CodeMeaning, out DicomAttribute att))
                            {
                                panel.Controls.Add(new Label { Text = att.ToString(), AutoSize = true, Font = new Font("Segoe UI", 11) });
                            }
                        }
                        if(item.TryGetAttribute(DicomTags.ContentSequence,out var contentSequence))
                        {
                            FlowLayoutPanel master = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
                            master.Controls.Add(panel);
                            FlowLayoutPanel offset1 = new FlowLayoutPanel();
                            offset1.FlowDirection = FlowDirection.LeftToRight;
                            offset1.AutoSize = true;
                            offset1.Margin = new Padding(0);
                            FlowLayoutPanel vertical = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
                            //vertical.Controls.Add(panel);
                            var seq = contentSequence as DicomAttributeSQ;
                            for(int j = 0;j <seq.Count;j++ )
                            {
                                

                                vertical.Controls.Add(ConvertSequenceItem(seq[j], j+1, indentLevel + 1));
                            }
                            offset1.Controls.Add(new Label { Text = " ", AutoSize = true });
                            offset1.Controls.Add(vertical);
                            master.Controls.Add(offset1);
                            return master;
                            //return vertical;
                        }
                        return panel;
                        //break;
                    default:
                        return panel;
                }
            }
            else
                return null;
        }
    }
}
