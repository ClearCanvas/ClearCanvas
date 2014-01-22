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
	/// X-Ray Grid Description Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.7.11 (Table C.8-36b)</remarks>
	public interface IXRayGridDescriptionMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of GridAbsorbingMaterial in the underlying collection. Type 3.
		/// </summary>
		string GridAbsorbingMaterial { get; set; }

		/// <summary>
		/// Gets or sets the value of GridSpacingMaterial in the underlying collection. Type 3.
		/// </summary>
		string GridSpacingMaterial { get; set; }

		/// <summary>
		/// Gets or sets the value of GridThickness in the underlying collection. Type 3.
		/// </summary>
		double? GridThickness { get; set; }

		/// <summary>
		/// Gets or sets the value of GridPitch in the underlying collection. Type 3.
		/// </summary>
		double? GridPitch { get; set; }

		/// <summary>
		/// Gets or sets the value of GridAspectRatio in the underlying collection. Type 3.
		/// </summary>
		int[] GridAspectRatio { get; set; }

		/// <summary>
		/// Gets or sets the value of GridPeriod in the underlying collection. Type 3.
		/// </summary>
		double? GridPeriod { get; set; }

		/// <summary>
		/// Gets or sets the value of GridFocalDistance in the underlying collection. Type 3.
		/// </summary>
		double? GridFocalDistance { get; set; }

		/// <summary>
		/// Gets or sets the value of GridId in the underlying collection. Type 3.
		/// </summary>
		string GridId { get; set; }
	}
}