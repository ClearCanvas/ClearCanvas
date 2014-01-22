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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a property to get a collection of DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
	/// </summary>
	public interface IDicomVoiLutsProvider
	{
		/// <summary>
		/// Gets a collection of DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
		/// </summary>
		IDicomVoiLuts DicomVoiLuts { get; }
	}

	/// <summary>
	/// Defines properties to get the various DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
	/// </summary>
	public interface IDicomVoiLuts
	{
		/// <summary>
		/// Gets a list of linear VOI LUTs (i.e. value windows) defined in the image header.
		/// </summary>
		IList<VoiWindow> ImageVoiLinearLuts { get; }

		/// <summary>
		/// Gets a list of data VOI LUTs defined in the image header.
		/// </summary>
		IList<VoiDataLut> ImageVoiDataLuts { get; }

		/// <summary>
		/// Gets the SOP instance UID of the image.
		/// </summary>
		string ImageSopInstanceUid { get; }

		/// <summary>
		/// Gets the frame number of the frame associated with the image.
		/// </summary>
		int ImageSopFrameNumber { get; }

		/// <summary>
		/// Gets a list of linear VOI LUTs (i.e. value windows) defined in the presentation state.
		/// </summary>
		IList<VoiWindow> PresentationVoiLinearLuts { get; }

		/// <summary>
		/// Gets a list of data VOI LUTs defined in the presentation state.
		/// </summary>
		IList<VoiDataLut> PresentationVoiDataLuts { get; }

		/// <summary>
		/// Gets the SOP instance UID of the presentation state.
		/// </summary>
		string PresentationStateSopInstanceUid { get; }
	}

	public sealed class DicomVoiLuts : IDicomVoiLuts
	{
		private readonly IImageSopProvider _image;

		public DicomVoiLuts(IImageSopProvider image)
		{
			_image = image;
		}

		#region Presentation Luts

		[CloneIgnore]
		private readonly List<VoiWindow> _presentationVoiLinearLuts = new List<VoiWindow>();

		[CloneIgnore]
		private readonly List<VoiDataLut> _presentationVoiDataLuts = new List<VoiDataLut>();

		[CloneIgnore]
		private string _sourcePresentationSopUid = "";

		public string PresentationStateSopInstanceUid
		{
			get { return _sourcePresentationSopUid; }
		}

		public IList<VoiWindow> PresentationVoiLinearLuts
		{
			get { return _presentationVoiLinearLuts.AsReadOnly(); }
		}

		public IList<VoiDataLut> PresentationVoiDataLuts
		{
			get { return _presentationVoiDataLuts.AsReadOnly(); }
		}

		internal void ReinitializePresentationLuts(string sourceSopUid)
		{
			_sourcePresentationSopUid = sourceSopUid;
			_presentationVoiLinearLuts.Clear();
			_presentationVoiDataLuts.Clear();
		}

		internal void AddPresentationLinearLut(double width, double center, string explanation)
		{
			_presentationVoiLinearLuts.Add(new VoiWindow(width, center, explanation));
		}

		internal void AddPresentationDataLut(VoiDataLut dataLut)
		{
			_presentationVoiDataLuts.Add(dataLut);
		}

		#endregion

		#region Image Luts

		public string ImageSopInstanceUid
		{
			get { return _image.ImageSop.SopInstanceUid; }
		}

		public int ImageSopFrameNumber
		{
			get { return _image.Frame.FrameNumber; }
		}

		public IList<VoiWindow> ImageVoiLinearLuts
		{
			get { return new List<VoiWindow>(VoiWindow.GetWindows(_image.Frame)).AsReadOnly(); }
		}

		public IList<VoiDataLut> ImageVoiDataLuts
		{
			get { return _image.Frame.VoiDataLuts; }
		}

		#endregion
	}
}