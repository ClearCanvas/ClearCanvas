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
	/// X-Ray Collimator Dimensions Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.7.3 (Table C.8-28b)</remarks>
	public interface IXRayCollimatorDimensionsMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of CollimatorLeftVerticalEdge in the underlying collection. Type 1C.
		/// </summary>
		int? CollimatorLeftVerticalEdge { get; set; }

		/// <summary>
		/// Gets or sets the value of CollimatorRightVerticalEdge in the underlying collection. Type 1C.
		/// </summary>
		int? CollimatorRightVerticalEdge { get; set; }

		/// <summary>
		/// Gets or sets the value of CollimatorUpperHorizontalEdge in the underlying collection. Type 1C.
		/// </summary>
		int? CollimatorUpperHorizontalEdge { get; set; }

		/// <summary>
		/// Gets or sets the value of CollimatorLowerHorizontalEdge in the underlying collection. Type 1C.
		/// </summary>
		int? CollimatorLowerHorizontalEdge { get; set; }

		/// <summary>
		/// Gets or sets the value of CenterOfCircularCollimator in the underlying collection. Type 1C.
		/// </summary>
		int[] CenterOfCircularCollimator { get; set; }

		/// <summary>
		/// Gets or sets the value of RadiusOfCircularCollimator in the underlying collection. Type 1C.
		/// </summary>
		int? RadiusOfCircularCollimator { get; set; }

		/// <summary>
		/// Gets or sets the value of VerticesOfThePolygonalCollimator in the underlying collection. Type 1C.
		/// </summary>
		int[] VerticesOfThePolygonalCollimator { get; set; }
	}
}