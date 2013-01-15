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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A VOI LUT Manager, which is responsible for managing installation and restoration
	/// of VOI LUTs via the Memento pattern.
	/// </summary>
	public sealed class VoiLutManager : IVoiLutManager
	{
		#region Private Fields
		
		private readonly IVoiLutInstaller _voiLutInstaller;
		private bool _enabled = true;
		private bool _allowDisable;
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public VoiLutManager(IVoiLutInstaller voiLutInstaller, bool allowDisable)
		{
			Platform.CheckForNullReference(voiLutInstaller, "voiLutInstaller");
			_voiLutInstaller = voiLutInstaller;
			_allowDisable = allowDisable;
		}

		#region IVoiLutManager Members

		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		public IVoiLut GetLut()
		{
			return _voiLutInstaller.VoiLut;
		}

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="lut">The Lut to be installed.</param>
		public void InstallLut(IVoiLut lut)
		{
			InstallVoiLut(lut);
		}

		#endregion

		#region IVoiLutInstaller Members

		/// <summary>
		/// Gets the currently installed Voi Lut.
		/// </summary>
		/// <returns>The Voi Lut as an <see cref="IVoiLut"/>.</returns>
		public IVoiLut VoiLut
		{
			get { return _voiLutInstaller.VoiLut; }	
		}

		/// <summary>
		/// Installs a new Voi Lut.
		/// </summary>
		/// <param name="lut">The Lut to be installed.</param>
		public void InstallVoiLut(IVoiLut lut)
		{
			IVoiLut existingLut = GetLut();
			if (existingLut is IGeneratedDataLut)
			{
				//Clear the data in the data lut so it's not hanging around using up memory.
				((IGeneratedDataLut)existingLut).Clear();
			}
			
			_voiLutInstaller.InstallVoiLut(lut);
		}

		/// <summary>
		/// Gets or sets whether the output of the VOI LUT should be inverted for display.
		/// </summary>
		public bool Invert
		{
			get { return _voiLutInstaller.Invert; }
			set { _voiLutInstaller.Invert = value; }
		}

	    /// <summary>
	    /// Gets the default value of <see cref="Invert"/>.  In DICOM, this would be true
	    /// for all MONOCHROME1 images.
	    /// </summary>
	    public bool DefaultInvert
        {
            get { return _voiLutInstaller.DefaultInvert; }
        }

		/// <summary>
		/// Toggles the state of the <see cref="IVoiLutInstaller.Invert"/> property.
		/// </summary>
		[Obsolete("Use the Invert property instead.")]
		public void ToggleInvert()
		{
			_voiLutInstaller.Invert = !_voiLutInstaller.Invert;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the LUT should be used in rendering the parent object.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if LUTs may not be disabled for rendering the parent object.</exception>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (!_allowDisable)
					throw new InvalidOperationException();

				_enabled = value;
			}
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Captures enough information to restore the currently installed lut.
		/// </summary>
		public object CreateMemento()
		{
			return new VoiLutMemento(_voiLutInstaller.VoiLut, _voiLutInstaller.Invert);
		}

		/// <summary>
		/// Restores the previously installed lut and/or it's state.
		/// </summary>
		public void SetMemento(object memento)
		{
			VoiLutMemento lutMemento = (VoiLutMemento) memento;

			if (_voiLutInstaller.VoiLut != lutMemento.OriginatingLut)
				this.InstallLut(lutMemento.OriginatingLut);

			if (lutMemento.InnerMemento != null)
				_voiLutInstaller.VoiLut.SetMemento(lutMemento.InnerMemento);

			_voiLutInstaller.Invert = lutMemento.Invert;
		}

		#endregion
	}
}
