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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Interface to an entity responsible for managing installation of VOI LUTs.
	/// </summary>
	public interface IVoiLutInstaller
	{
		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		IVoiLut VoiLut { get; }

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="voiLut">The Lut to be installed.</param>
		void InstallVoiLut(IVoiLut voiLut);

		/// <summary>
		/// Gets or sets whether the output of the VOI LUT should be inverted for display.
		/// </summary>
		bool Invert { get; set; }

        /// <summary>
        /// Gets the default value of <see cref="Invert"/>.  In DICOM, this would be true
        /// for all MONOCHROME1 images.
        /// </summary>
        bool DefaultInvert { get; }
	}
}
