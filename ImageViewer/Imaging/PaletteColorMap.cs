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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// N.B. To be used only for colorspace conversions of a palette color image - do not install as a regular color map!
	/// </summary>
	internal class PaletteColorMap : ColorMap
	{
		private readonly PaletteColorLut _lut;

		public PaletteColorMap(PaletteColorLut lut)
		{
			this.MinInputValue = lut.FirstMappedPixelValue;
			this.MaxInputValue = lut.FirstMappedPixelValue + lut.CountEntries - 1;
			_lut = lut;
		}

		protected override void Create()
		{
			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				this[i] = _lut[i].ToArgb();
			}
		}

		public override string GetDescription()
		{
			return SR.DescriptionPaletteColorMap;
		}

		public static PaletteColorMap Create(IDicomAttributeProvider dataSource)
		{
			PaletteColorLut paletteColorLut = PaletteColorLut.Create(dataSource);
			return new PaletteColorMap(paletteColorLut);
		}
	}
}