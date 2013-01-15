#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.Samples
{
    public class EditSop
    {

        private readonly string _sourceFilename;
        private DicomFile _dicomFile;

        public EditSop(string file)
        {
            _sourceFilename = file;
        }

        public DicomFile DicomFile
        {
            get { return _dicomFile; }
        }

        public void Load()
        {
            _dicomFile = new DicomFile(_sourceFilename);
            try
            {
                _dicomFile.Load();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception loading DICOM file: {0}", _sourceFilename);
            }
        }

        public string GetXmlRepresentation()
        {
            var theDoc = GetXmlDoc();

            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Document,
                Indent = true,
                NewLineOnAttributes = false,
                CheckCharacters = true,
                IndentChars = "  "
            };

            StringBuilder sb = new StringBuilder();

            XmlWriter tw = XmlWriter.Create(sb, xmlSettings);
            theDoc.WriteTo(tw);
            tw.Flush();
            tw.Close();

            return sb.ToString();
        }

        private XmlDocument GetXmlDoc()
        {
            var theDocument = new XmlDocument();

            XmlElement instance = theDocument.CreateElement("Instance");
            

            XmlAttribute sopInstanceUid = theDocument.CreateAttribute("UID");
            sopInstanceUid.Value = _dicomFile.MediaStorageSopInstanceUid;
            instance.Attributes.Append(sopInstanceUid);

            XmlAttribute sopClassAttribute = theDocument.CreateAttribute("SopClassUID");
            sopClassAttribute.Value = _dicomFile.SopClass.Uid;
            instance.Attributes.Append(sopClassAttribute);

            theDocument.AppendChild(instance);

            foreach (DicomAttribute attribute in _dicomFile.DataSet)
            {
                XmlElement instanceElement = CreateDicomAttributeElement(theDocument, attribute.Tag, "Attribute");
                if (attribute is DicomAttributeSQ || attribute is DicomAttributeOW || attribute is DicomAttributeUN ||
                    attribute is DicomAttributeOF || attribute is DicomAttributeOB)
                {
                    continue;
                }
                instanceElement.InnerText = XmlEscapeString(attribute);

                instance.AppendChild(instanceElement);
            }

            return theDocument;
        }

        private static XmlElement CreateDicomAttributeElement(XmlDocument document, DicomTag dicomTag, string name)
        {
            XmlElement dicomAttributeElement = document.CreateElement(name);

            XmlAttribute tag = document.CreateAttribute("Tag");
            tag.Value = "$" + dicomTag.VariableName;

            XmlAttribute vr = document.CreateAttribute("VR");
            vr.Value = dicomTag.VR.ToString();

            dicomAttributeElement.Attributes.Append(tag);
            dicomAttributeElement.Attributes.Append(vr);

            return dicomAttributeElement;
        }

        private static string XmlEscapeString(string input)
        {
            string result = input ?? string.Empty;

            result = SecurityElement.Escape(result);

            // Do the regular expression to escape out other invalid XML characters in the string not caught by the above.
            // NOTE: the \x sequences you see below are C# escapes, not Regex escapes
            result = Regex.Replace(result, "[^\x9\xA\xD\x20-\xFFFD]", m => string.Format("&#x{0:X};", (int)m.Value[0]));

            return result;
        }

        public void UpdateTags(string xml)
        {
            var theDoc = new XmlDocument();

            try
            {
                theDoc.LoadXml(xml);
                var instanceXml = new InstanceXml(theDoc.DocumentElement, null);
                DicomAttributeCollection queryMessage = instanceXml.Collection;

                if (queryMessage == null)
                {
                    Platform.Log(LogLevel.Error, "Unexpected error parsing move message");
                    return;
                }

                foreach (var attribute in queryMessage)
                {
                    _dicomFile.DataSet[attribute.Tag] = attribute.Copy();
                }

            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unable to perform update");
            }		
        }

        public void Save(string filename)
        {
            try
            {
                _dicomFile.Save(filename);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception saving dicom file: {0}", filename);
            }
        }      
    }
}
