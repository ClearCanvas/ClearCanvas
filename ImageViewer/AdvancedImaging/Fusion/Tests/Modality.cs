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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	public enum Modality
	{
		CT,
		MR,
		PT,
		SC
	}

	public class ModalityConverter
	{
		public static string ToSopClassUid(Modality modality)
		{
			switch (modality)
			{
				case Modality.CT:
					return SopClass.CtImageStorageUid;
				case Modality.MR:
					return SopClass.MrImageStorageUid;
				case Modality.PT:
					return SopClass.PositronEmissionTomographyImageStorageUid;
				case Modality.SC:
				default:
					return SopClass.SecondaryCaptureImageStorageUid;
			}
		}
	}
}

#endif