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
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Editing
{
	public interface IEditContext : IDisposable
	{
		DicomAttributeCollection DataSet { get; }
		bool Excluded { get; }
		void Exclude();

		int NumberOfFrames { get; }
		IEditPixelDataContext GetPixelDataContext(int frameNumber);
	}

	public interface IEditPixelDataContext
	{
		DicomAttributeCollection DataSet { get; }
		int FrameNumber { get; }
		byte[] PixelData { get; }
		int Rows { get; }
		int Columns { get; }
		int BitsAllocated { get; }
		int BitsStored { get; }
		int HighBit { get; }
		int PixelRepresentation { get; }
		
		PhotometricInterpretation PhotometricInterpretation { get; }
		int PlanarConfiguration { get; }
	}
}
