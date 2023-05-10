using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.StructuredReportViewer
{

    /// <summary>
    /// Extension point for views onto <see cref="CountRateComponent"/>
    /// </summary>
    [ExtensionPoint]
    public sealed class StructuredReportViewerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {

    }


    /// <summary>
    /// DailyQCComponentClass
    /// </summary>
    [AssociateView(typeof(StructuredReportViewerComponentViewExtensionPoint))]
    public class StructuredReportViewerComponent : ApplicationComponent
    {
        private DicomAttributeCollection _attributeCollection;
        public StructuredReportViewerComponent(IDesktopWindow desktopWindow, DicomAttributeCollection collection)
        {
            _attributeCollection = collection;
        }

        public DicomAttributeCollection AttributeCollection
        {
            get { return _attributeCollection; }
        }

        public string DumpString
        {
            get
            {
                return _attributeCollection.DumpString;
            }
        }

        public string PatientName
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.PatientsName, out var patientName))
                {
                    var s = patientName.ToString().Split('^');

                    StringBuilder sb = new StringBuilder();
                    if (s.Length > 3)
                    {
                        sb.Append(s[3]);
                        sb.Append(' ');
                    }
                    sb.Append(s[1]);
                    sb.Append(' ');
                    sb.Append(s[0]);
                    return sb.ToString();
                }
                return string.Empty;
            }
        }

        public string PatientID
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.PatientId, out var att))
                {
                    return att.ToString();
                }
                return string.Empty;
            }
        }

        public string PatientSex
        {
            get
            {
                if(_attributeCollection.TryGetAttribute(DicomTags.PatientsSex,out var att))
                {
                    if (att.ToString() == "M")
                        return "Male";
                    else if (att.ToString() == "F")
                        return "Female";
                    else
                        return "Other";
                }
                return string.Empty;
            }
        }

        public string PatientBirthDate
        {

            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.PatientsBirthDate, out var att))
                {
                    //return (att as DicomAttributeDA)?.GetDateTime(1).ToString();
                    if(DateTime.TryParseExact(att.ToString(),"yyyyMMdd",CultureInfo.InvariantCulture,DateTimeStyles.None,out DateTime date))
                    {
                        return date.ToString("dd MMM yyyy");
                    }
                    
                }
                return "Birth date unknown";
            }
        }

        public string ReportTitle
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.ConceptNameCodeSequence, out DicomAttribute att))
                {
                    var seq = att as DicomAttributeSQ;
                    if (seq != null)
                    {
                        if (seq[0].TryGetAttribute(DicomTags.CodeMeaning, out DicomAttribute meaning))
                        {
                            return meaning.ToString();
                        }
                        else
                            return "Structured Report";
                    }
                    else
                        return "Structured Report";
                }
                else
                    return "Structured Report";
            }
        }

        public string ReportDate
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.ContentDate, out var studyDate))
                {
                    if ((studyDate as DicomAttributeDA).TryGetDateTime(0, out var d))
                    {
                        if (_attributeCollection.TryGetAttribute(DicomTags.ContentTime, out var studyTime))
                        {
                            if ((studyTime as DicomAttributeTM).TryGetDateTime(0, out var t))
                            {
                                var result = d.Add(t.TimeOfDay);
                                return result.ToString("dd MMM yyyy h:mm tt");
                            }
                        }
                    }

                }
                return "Report date unknown";
            }
        }

        public string CompletionFlag
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.CompletionFlag, out var att))
                {
                    return att.ToString();
                }
                return string.Empty;
            }
        }
        public string VerificationFlag
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.VerificationFlag, out var att))
                {
                    return att.ToString();
                }
                return string.Empty;
            }
        }

        public string StudyDate
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.StudyDate, out var studyDate))
                {
                    if((studyDate as DicomAttributeDA).TryGetDateTime(0,out var d))
                    {
                        if (_attributeCollection.TryGetAttribute(DicomTags.StudyTime, out var studyTime))
                        {
                            if ((studyTime as DicomAttributeTM).TryGetDateTime(0, out var t))
                            {
                                var result = d.Add(t.TimeOfDay);
                                return result.ToString("dd MMM yyyy h:mm tt");
                            }
                        }
                    }
                    
                }
                return "???";
            }
        }

        public string StudyID
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.VerificationFlag, out var att))
                {
                    return att.ToString();
                }
                return string.Empty;
            }
        }

        public string Study
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.StudyDescription, out var att))
                {
                    return att.ToString();
                }
                return string.Empty;
            }
        }

        public string AccessionNumber
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.AccessionNumber, out var att))
                {
                    return att.ToString();
                }
                return string.Empty;
            }
        }

        public string Referrer
        {
            get
            {
                if (_attributeCollection.TryGetAttribute(DicomTags.ReferringPhysiciansName, out var att))
                {
                    return att.ToString();
                }
                return string.Empty;
            }
        }

        public DicomAttributeSQ ContentSequnce
        {
            get
            {
                if(_attributeCollection.TryGetAttribute(DicomTags.ContentSequence,out var att))
                {
                    var seq = att as DicomAttributeSQ;
                    if(seq != null)
                    {
                        return seq;
                    }

                }
                return new DicomAttributeSQ(DicomTags.ContentSequence);
            }
        }
    }
}
