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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	//TODO: change how this manager/provider relationship works ... it just doesn't feel right.

	/// <summary>
	/// A VOI LUT Manager, which is responsible for managing installation and restoration
	/// of VOI LUTs via the Memento pattern.
	/// </summary>
	/// <remarks>
	/// Implementors must not return null from the <see cref="IVoiLutInstaller.VoiLut"/> method.
	/// </remarks>
	/// <seealso cref="IVoiLutProvider"/>
	/// <seealso cref="IVoiLut"/>
	public interface IVoiLutManager : IVoiLutInstaller, IMemorable
	{
		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		[Obsolete("Use the VoiLut property instead.")]
		IVoiLut GetLut();

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="voiLut">The Lut to be installed.</param>
		[Obsolete("Use the InstallVoiLut method instead.")]
		void InstallLut(IVoiLut voiLut);

		/// <summary>
		/// Toggles the state of the <see cref="IVoiLutInstaller.Invert"/> property.
		/// </summary>
		[Obsolete("Use the Invert property instead.")]
		void ToggleInvert();

		/// <summary>
		/// Gets or sets a value indicating whether the LUT should be used in rendering the parent object.
		/// </summary>
		bool Enabled { get; set; }
	}
}
