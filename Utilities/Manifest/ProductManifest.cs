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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Root element used for Product Manifests
    /// </summary>
    [XmlRoot(ElementName = "ProductManifest")]
    public class ProductManifest
    {
        private List<ManifestFile> _files;

        public Product Product { get; set;}

        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public List<ManifestFile> Files
        {
            get
            {
                if (_files == null)
                    _files = new List<ManifestFile>();
                return _files;
            }
            set { _files = value; }
        }
    }
}
