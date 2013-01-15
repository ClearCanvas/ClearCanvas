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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Interface to an entity responsible for managing installation of color maps.
	/// </summary>
	public interface IColorMapInstaller
	{
		/// <summary>
		/// Gets the currently installed color map.
		/// </summary>
		IColorMap ColorMap { get; }

		/// <summary>
		/// Installs a color map by name.
		/// </summary>
		void InstallColorMap(string name);

		/// <summary>
		/// Installs a color map by <see cref="ColorMapDescriptor">descriptor</see>.
		/// </summary>
		void InstallColorMap(ColorMapDescriptor descriptor);

		/// <summary>
		/// Installs a color map.
		/// </summary>
		void InstallColorMap(IColorMap colorMap);

		/// <summary>
		/// Gets <see cref="ColorMapDescriptor"/>s for all the different types of available color maps.
		/// </summary>
		IEnumerable<ColorMapDescriptor> AvailableColorMaps { get; }
	}
}
