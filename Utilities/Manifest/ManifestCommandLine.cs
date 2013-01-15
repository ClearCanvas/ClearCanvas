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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// <see cref="CommandLine"/> for the <see cref="ManifestGenerationApplication"/> and 
    /// <see cref="ManifestInputGenerationApplication"/> applications.
    /// </summary>
    public class ManifestCommandLine : CommandLine
    {
        [CommandLineParameter("dist", "d", "Specifies the root directory of the distribution to generate a manifest for.")]
        public string DistributionDirectory { get; set; }

        [CommandLineParameter("manifest", "m", "The path to the generated manifest file.")]
        public string Manifest { get; set; }

        [CommandLineParameter("package", "p", "True if the manifest is for a package.")]
        [DefaultValue(false)]
        public bool Package { get; set; }

        [CommandLineParameter("productmanifest", "pm", "The path to the product manifest the package works against (only used when /p is specified).")]
        public string ProductManifest { get; set; }

        [CommandLineParameter("packagename", "pn", "The name of the package (only used when /p is specified).")]
        public string PackageName { get; set; }

        [CommandLineParameter("certificate", "c", "The x509 certificate for signing the manifest.")]
        public string Certificate { get; set; }

        [CommandLineParameter("password", "pw", "The x509 certificate password for signing the manifest.")]
        public string Password { get; set; }

    }
}
