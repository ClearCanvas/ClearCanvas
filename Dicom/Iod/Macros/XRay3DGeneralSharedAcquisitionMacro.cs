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

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// X-Ray 3D General Shared Acquisition Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2011, Part 3, Section C.8.21.3.1.1 (Table C.8.21.3.1.1-1)</remarks>
	public interface IXRay3DGeneralSharedAcquisitionMacro : IXRayGridDescriptionMacro
	{
		/// <summary>
		/// Gets or sets the value of SourceImageSequence in the underlying collection. Type 1C.
		/// </summary>
		ImageSopInstanceReferenceMacro[] SourceImageSequence { get; set; }

		/// <summary>
		/// Gets or sets the value of FieldOfViewDimensionsInFloat in the underlying collection. Type 1C.
		/// </summary>
		double[] FieldOfViewDimensionsInFloat { get; set; }

		/// <summary>
		/// Gets or sets the value of FieldOfViewOrigin in the underlying collection. Type 1C.
		/// </summary>
		double[] FieldOfViewOrigin { get; set; }

		/// <summary>
		/// Gets or sets the value of FieldOfViewRotation in the underlying collection. Type 1C.
		/// </summary>
		double? FieldOfViewRotation { get; set; }

		/// <summary>
		/// Gets or sets the value of FieldOfViewHorizontalFlip in the underlying collection. Type 1C.
		/// </summary>
		string FieldOfViewHorizontalFlip { get; set; }

		/// <summary>
		/// Gets or sets the value of Grid in the underlying collection. Type 1C.
		/// </summary>
		string Grid { get; set; }

		/// <summary>
		/// Gets or sets the value of Kvp in the underlying collection. Type 1C.
		/// </summary>
		double? Kvp { get; set; }

		/// <summary>
		/// Gets or sets the value of XRayTubeCurrentInMa in the underlying collection. Type 1C.
		/// </summary>
		double? XRayTubeCurrentInMa { get; set; }

		/// <summary>
		/// Gets or sets the value of ExposureTimeInMs in the underlying collection. Type 1C.
		/// </summary>
		double? ExposureTimeInMs { get; set; }

		/// <summary>
		/// Gets or sets the value of ExposureInMas in the underlying collection. Type 1C.
		/// </summary>
		double? ExposureInMas { get; set; }

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgent in the underlying collection. Type 1C.
		/// </summary>
		string ContrastBolusAgent { get; set; }

		/// <summary>
		/// Gets or sets the value of ContrastBolusAgentSequence in the underlying collection. Type 1C.
		/// </summary>
		CodeSequenceMacro[] ContrastBolusAgentSequence { get; set; }

		/// <summary>
		/// Gets or sets the value of StartAcquisitionDateTime in the underlying collection.  Type 1C.
		/// </summary>
		DateTime? StartAcquisitionDateTime { get; set; }

		/// <summary>
		/// Gets or sets the value of EndAcquisitionDatetime in the underlying collection.  Type 1C.
		/// </summary>
		DateTime? EndAcquisitionDateTime { get; set; }

		/// <summary>
		/// Creates a single instance of a SourceImageSequence item. Does not modify the SourceImageSequence in the underlying collection.
		/// </summary>
		ImageSopInstanceReferenceMacro CreateSourceImageSequenceItem();

		/// <summary>
		/// Creates a single instance of a ContrastBolusAgentSequence item. Does not modify the ContrastBolusAgentSequence in the underlying collection.
		/// </summary>
		CodeSequenceMacro CreateContrastBolusAgentSequenceItem();
	}
}