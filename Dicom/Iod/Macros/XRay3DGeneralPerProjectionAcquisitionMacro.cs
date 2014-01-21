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

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// X-Ray 3D General Per Projection Acquisition Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3.1.2 (Table C.8.21.3.1.2-1)</remarks>
	public interface IXRay3DGeneralPerProjectionAcquisitionMacro : IXRayCollimatorDimensionsMacro
	{
		/// <summary>
		/// Gets or sets the value of Kvp in the underlying collection. Type 1C.
		/// </summary>
		double? Kvp { get; set; }

		/// <summary>
		/// Gets or sets the value of XRayTubeCurrentInMa in the underlying collection. Type 1C.
		/// </summary>
		double? XRayTubeCurrentInMa { get; set; }

		/// <summary>
		/// Gets or sets the value of FrameAcquisitionDuration in the underlying collection. Type 1C.
		/// </summary>
		double? FrameAcquisitionDuration { get; set; }

		/// <summary>
		/// Gets or sets the value of CollimatorShape in the underlying collection. Type 1C.
		/// </summary>
		string CollimatorShape { get; set; }
	}
}