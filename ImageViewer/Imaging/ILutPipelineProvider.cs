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

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides access to the LUT instances in a grayscale image's LUT pipeline.
	/// </summary>
	public interface ILutPipelineProvider
	{
		/// <summary>
		/// Gets the modality LUT in the pipeline. Value may be NULL if no LUT is present.
		/// </summary>
		IModalityLut ModalityLut { get; }

		/// <summary>
		/// Gets the VOI LUT in the pipeline. Value may be NULL if no LUT is present.
		/// </summary>
		IVoiLut VoiLut { get; }

		/// <summary>
		/// Computes the value of a given raw pixel at a specified stage of the LUT pipeline.
		/// </summary>
		/// <param name="rawPixelValue">The raw pixel value from the source pixel data.</param>
		/// <param name="outStage">The stage in the LUT pipeline for which the value should be computed.</param>
		/// <returns></returns>
		double LookupPixelValue(int rawPixelValue, LutPipelineStage outStage);
	}

	/// <summary>
	/// Identifies the type of pixel value based on the stage in a grayscale image's LUT pipeline.
	/// </summary>
	public enum LutPipelineStage
	{
		/// <summary>
		/// Represents untransformed values from the source data (i.e. the raw input values to the LUT pipeline).
		/// </summary>
		/// <remarks>
		/// Corresponds to the values stored in the image pixel data.
		/// </remarks>
		Source,

		/// <summary>
		/// Represents values after being transformed by a modality LUT, if one exists.
		/// </summary>
		/// <remarks>
		/// Corresponds to the output of the &quot;modality&quot; stage in the DICOM Grayscale Standard Display Function (PS 3.14, Section 6).
		/// </remarks>
		Modality,

		/// <summary>
		/// Represents values after being transformed by a VOI LUT, if one exists.
		/// </summary>
		/// <remarks>
		/// Corresponds to the output of the &quot;values of interest&quot; and &quot;polarity&quot; stages in the DICOM Grayscale Standard Display Function (PS 3.14, Section 6).
		/// </remarks>
		Voi
	}
}