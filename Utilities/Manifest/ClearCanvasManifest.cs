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

using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// A manifest file for a ClearCanvas product.
    /// </summary>
    [XmlRoot(ElementName = "ClearCanvasManifest", Namespace = "http://www.clearcanvas.ca")]
    public class ClearCanvasManifest
    {
        #region Public Properties

        /// <summary>
        /// If the manifest is for a Package, the actual Package Manifest
        /// </summary>
        [XmlElement("PackageManifest")]
        [DefaultValue(null)]
        public PackageManifest PackageManifest { get; set; }

        /// <summary>
        /// If the manifest is for a Product, the actual Product Manifest
        /// </summary>
        [XmlElement("ProductManifest")]
        [DefaultValue(null)]
        public ProductManifest ProductManifest { get; set; }

        #endregion Public Properties

        #region Public Static Methods

        public static void Serialize(string filename, ClearCanvasManifest manifest)
        {
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
            {
                Serialize(fs, manifest);
                fs.Close();
            }
        }

        public static void Serialize(Stream stream, ClearCanvasManifest manifest)
        {
            XmlSerializer theSerializer = new XmlSerializer(typeof (ClearCanvasManifest));

            XmlWriterSettings settings = new XmlWriterSettings
                                             {
                                                 Indent = true,
                                                 IndentChars = "  ",
                                                 Encoding = Encoding.UTF8,
                                             };

            XmlWriter writer = XmlWriter.Create(stream, settings);
            if (writer != null)
                theSerializer.Serialize(writer, manifest);
            stream.Flush();
        }

        public static XmlDocument Serialize(ClearCanvasManifest manifest)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                Serialize(fs, manifest);

                fs.Seek(0, SeekOrigin.Begin);
                fs.Flush();

                XmlDocument doc = new XmlDocument();
                doc.Load(fs);
                return doc;
            }
        }


        public static ClearCanvasManifest Deserialize(string filename)
        {
            XmlSerializer theSerializer = new XmlSerializer(typeof(ClearCanvasManifest));

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ClearCanvasManifest input = (ClearCanvasManifest)theSerializer.Deserialize(fs);

                return input;
            }
        }

        #endregion Public Static Methods
    }
}
