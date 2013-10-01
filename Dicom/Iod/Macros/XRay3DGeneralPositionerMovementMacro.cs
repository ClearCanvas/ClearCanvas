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
	/// X-Ray 3D General Positioner Movement Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3.1.3 (Table C.8.21.3.1.3-1)</remarks>
	public interface IXRay3DGeneralPositionerMovementMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of PrimaryPositionerScanArc in the underlying collection. Type 1C.
		/// </summary>
		double? PrimaryPositionerScanArc { get; set; }

		/// <summary>
		/// Gets or sets the value of PrimaryPositionerScanStartAngle in the underlying collection. Type 1C.
		/// </summary>
		double? PrimaryPositionerScanStartAngle { get; set; }

		/// <summary>
		/// Gets or sets the value of PrimaryPositionerIncrement in the underlying collection. Type 1C.
		/// </summary>
		double? PrimaryPositionerIncrement { get; set; }

		/// <summary>
		/// Gets or sets the value of SecondaryPositionerScanArc in the underlying collection. Type 1C.
		/// </summary>
		double? SecondaryPositionerScanArc { get; set; }

		/// <summary>
		/// Gets or sets the value of SecondaryPositionerScanStartAngle in the underlying collection. Type 1C.
		/// </summary>
		double? SecondaryPositionerScanStartAngle { get; set; }

		/// <summary>
		/// Gets or sets the value of SecondaryPositionerIncrement in the underlying collection. Type 1C.
		/// </summary>
		double? SecondaryPositionerIncrement { get; set; }
	}
}