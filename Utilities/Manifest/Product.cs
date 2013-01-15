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

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Product definition for use in a <see cref="ClearCanvasManifest"/>.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The name of the product.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The component name.
        /// </summary>
        [DefaultValue(null)]
        public string Component { get; set; }
        /// <summary>
        /// The edition of the component.
        /// </summary>
        [DefaultValue(null)]
        public string Edition { get; set; }
        /// <summary>
        /// The version associated with the product.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The version suffix (e.g., SP1, etc.)
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// The name of the manifest associated with the product.
        /// </summary>
        [DefaultValue(null)]
        public string Manifest { get; set; }

    	//TODO (CR February 2011) - Low: Does not check Release.
		//We should just remove the product settings check from here, since it's done using critical.config now anyway.
    }
}
