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

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	public abstract class SopInstanceReference
	{
		private readonly string _sourceApplicationEntityTitle;
		private readonly string _studyInstanceUid;
		private readonly string _seriesInstanceUid;
		private readonly string _sopInstanceUid;
		private readonly string _sopClassUid;

		protected SopInstanceReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid, string sourceApplicationEntityTitle = null)
		{
			_studyInstanceUid = studyInstanceUid ?? string.Empty;
			_seriesInstanceUid = seriesInstanceUid ?? string.Empty;
			_sopClassUid = sopClassUid ?? string.Empty;
			_sopInstanceUid = sopInstanceUid ?? string.Empty;
			_sourceApplicationEntityTitle = sourceApplicationEntityTitle ?? string.Empty;
		}

		public string SourceApplicationEntityTitle
		{
			get { return _sourceApplicationEntityTitle; }
		}

		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
		}

		public string SopInstanceUid
		{
			get { return _sopInstanceUid; }
		}

		public string SopClassUid
		{
			get { return _sopClassUid; }
		}
	}

	public class KeyImageReference : SopInstanceReference
	{
		private readonly int? _frameNumber;

		public int? FrameNumber
		{
			get { return _frameNumber; }
		}

		public KeyImageReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid, int? frameNumber, string sourceApplicationEntityTitle = null)
			: base(studyInstanceUid, seriesInstanceUid, sopClassUid, sopInstanceUid, sourceApplicationEntityTitle)
		{
			_frameNumber = frameNumber;
		}

		public KeyImageReference(Frame frame)
			: this(frame.StudyInstanceUid,
			       frame.SeriesInstanceUid,
			       frame.ParentImageSop.SopClassUid,
			       frame.SopInstanceUid,
			       frame.ParentImageSop.NumberOfFrames > 1 ? (int?) frame.FrameNumber : null,
			       frame.ParentImageSop.DataSource[DicomTags.SourceApplicationEntityTitle].ToString()) {}

		public static implicit operator KeyImageReference(Frame frame)
		{
			return new KeyImageReference(frame);
		}
	}

	public class PresentationStateReference : SopInstanceReference
	{
		public PresentationStateReference(string studyInstanceUid, string seriesInstanceUid, string sopClassUid, string sopInstanceUid, string sourceApplicationEntityTitle = null)
			: base(studyInstanceUid, seriesInstanceUid, sopClassUid, sopInstanceUid, sourceApplicationEntityTitle) {}

		public PresentationStateReference(DicomSoftcopyPresentationState presentationState)
			: base(presentationState.DicomFile.DataSet[DicomTags.StudyInstanceUid].ToString(),
			       presentationState.PresentationSeriesInstanceUid,
			       presentationState.PresentationSopClassUid,
			       presentationState.PresentationSopInstanceUid,
			       presentationState.DicomFile.MetaInfo[DicomTags.SourceApplicationEntityTitle].ToString()) {}

		public static implicit operator PresentationStateReference(DicomSoftcopyPresentationState presentationState)
		{
			return new PresentationStateReference(presentationState);
		}
	}
}