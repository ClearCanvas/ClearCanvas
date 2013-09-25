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
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod
{
	public class LoadDicomFileArgs : LoadSopDicomFileArgs
	{
		public LoadDicomFileArgs(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, bool forceCompleteHeader, bool includePixelData)
			: base(forceCompleteHeader, includePixelData)
		{
			StudyInstanceUid = studyInstanceUid;
			SeriesInstanceUid = seriesInstanceUid;
			SopInstanceUid = sopInstanceUid;
		}

		public readonly string StudyInstanceUid;
		public readonly string SeriesInstanceUid;
		public readonly string SopInstanceUid;
	}

	public class LoadSopDicomFileArgs
	{
		public LoadSopDicomFileArgs(bool forceCompleteHeader, bool includePixelData)
		{
			ForceCompleteHeader = forceCompleteHeader;
			IncludePixelData = includePixelData;
		}

		public readonly bool ForceCompleteHeader;
		public readonly bool IncludePixelData;
	}

	public class LoadFramePixelDataArgs
	{
		public LoadFramePixelDataArgs(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, int frameNumber)
		{
			StudyInstanceUid = studyInstanceUid;
			SeriesInstanceUid = seriesInstanceUid;
			SopInstanceUid = sopInstanceUid;
			FrameNumber = frameNumber;
		}

		public readonly string StudyInstanceUid;
		public readonly string SeriesInstanceUid;
		public readonly string SopInstanceUid;
		public readonly int FrameNumber;
	}

	public class LoadSopFramePixelDataArgs
	{
		public LoadSopFramePixelDataArgs(int frameNumber)
		{
			FrameNumber = frameNumber;
		}

		public readonly int FrameNumber;
	}

	public interface IFramePixelData
	{
		long BytesReceived { get; }

		byte[] GetPixelData();
	}

	/// <summary>
	/// Interface for loading a complete or partial DICOM File.
	/// </summary>
	public interface IDicomFileLoader
	{
		bool CanLoadCompleteHeader { get; }
		bool CanLoadPixelData { get; }
		bool CanLoadFramePixelData { get; }
		DicomFile LoadDicomFile(LoadDicomFileArgs args);
		IFramePixelData LoadFramePixelData(LoadFramePixelDataArgs args);
	}

	/// <summary>
	/// Interface for loading a complete or partial DICOM File from a know SOP.
	/// </summary>
	public interface ISopDicomFileLoader
	{
		bool CanLoadCompleteHeader { get; }
		bool CanLoadPixelData { get; }
		bool CanLoadFramePixelData { get; }

		DicomFile LoadDicomFile(LoadSopDicomFileArgs args);
		IFramePixelData LoadFramePixelData(LoadSopFramePixelDataArgs args);
	}

	public class FramePixelData : IFramePixelData
	{
		private readonly byte[] _pixelData;

		public FramePixelData(byte[] pixelData)
		{
			_pixelData = pixelData;
		}
		#region IFramePixelData Members

		public long BytesReceived
		{
			get { return _pixelData.Length; }
		}

		public byte[] GetPixelData()
		{
			return _pixelData;
		}

		#endregion
	}

	public class DicomFileLoader : IDicomFileLoader
	{
		private readonly Func<LoadDicomFileArgs, DicomFile> _loadDicomFile;
		private readonly Func<LoadFramePixelDataArgs, IFramePixelData> _loadFramePixelData;

		public DicomFileLoader(bool canLoadCompleteHeader, bool canLoadPixelData, Func<LoadDicomFileArgs, DicomFile> loadDicomFile, Func<LoadFramePixelDataArgs, IFramePixelData> loadFramePixelData)
		{
			_loadDicomFile = loadDicomFile;
			_loadFramePixelData = loadFramePixelData;
			CanLoadCompleteHeader = canLoadCompleteHeader;
			CanLoadPixelData = canLoadPixelData;
		}

		#region IHeaderProvider

		public bool CanLoadCompleteHeader { get; private set; }
		public bool CanLoadPixelData { get; private set; }
		public bool CanLoadFramePixelData { get; private set; }

		public DicomFile LoadDicomFile(LoadDicomFileArgs args)
		{
			if (args.ForceCompleteHeader && !CanLoadCompleteHeader)
				throw new NotSupportedException("Provider doesn't support loading the complete header.");
			if (args.IncludePixelData && !CanLoadPixelData)
				throw new NotSupportedException("Provider doesn't support inclusion of pixel data.");

			return _loadDicomFile(args);
		}

		public IFramePixelData LoadFramePixelData(LoadFramePixelDataArgs args)
		{
			if (!CanLoadFramePixelData)
				throw new NotSupportedException("Provider doesn't support loading individual frame pixel data.");

			return _loadFramePixelData(args);
		}

		#endregion
	}

	// TODO (CR Jul 2013): replace the stuff in the viewer with this.

	/// <summary>
	/// Abstract representation of a study, with child series.
	/// </summary>
	public interface IStudy : IStudyData, IPatientData
	{
		new DateTime? PatientsBirthDate { get; }
		new TimeSpan? PatientsBirthTime { get; }

		new DateTime? StudyDate { get; }
		new TimeSpan? StudyTime { get; }

		IList<ISeries> Series { get; }
	}

	/// <summary>
	/// Abstract representation of a series with child sops.
	/// </summary>
	public interface ISeries : ISeriesData
	{
		IStudy ParentStudy { get; }
		IList<ISopInstance> SopInstances { get; }

		string StationName { get; }
		string Manufacturer { get; }
		string ManufacturersModelName { get; }

		string InstitutionName { get; }
		string InstitutionAddress { get; }
		string InstitutionalDepartmentName { get; }
	}

	/// <summary>
	/// Abstract representation of a sop instance, that provides <see cref="DicomAttribute"/> objects, or can construct and return an entire header.
	/// </summary>
	public interface ISopInstance : ISopInstanceData, ISopDicomFileLoader /*, IDicomAttributeProvider*/
	{
		ISeries ParentSeries { get; }

		SopClass SopClass { get; }

		string SourceApplicationEntityTitle { get; }

		DicomAttribute GetAttribute(uint dicomTag);
		DicomAttribute GetAttribute(DicomTag dicomTag);

		DicomFile GetHeader(bool forceComplete);
		byte[] GetFramePixelData(int frameNumber);
	}
}