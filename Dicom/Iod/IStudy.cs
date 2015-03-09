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
	// TODO (CR Jul 2013): replace the stuff in the viewer with this.

	/// <summary>
	/// Abstract representation of a study, with child series.
	/// </summary>
	public interface IStudy : IStudyData, IPatientData
	{
		/// <summary>
		/// Gets the collection of series in this study.
		/// </summary>
		ISeriesCollection Series { get; }

		/// <summary>
		/// Gets the first SOP instance in this study.
		/// </summary>
		ISopInstance FirstSopInstance { get; }

		new DateTime? PatientsBirthDate { get; }
		new TimeSpan? PatientsBirthTime { get; }

		new DateTime? StudyDate { get; }
		new TimeSpan? StudyTime { get; }
	}

	/// <summary>
	/// Args for loading a <see cref="DicomFile"/> for a particular SOP in a study.
	/// </summary>
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

	/// <summary>
	/// Args for loading a <see cref="DicomFile"/> for a known SOP.
	/// </summary>
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

	/// <summary>
	/// Args for loading pixel data for a particular frame, from a SOP in a known study.
	/// </summary>
	public class LoadFramePixelDataArgs : LoadSopFramePixelDataArgs
	{
		public LoadFramePixelDataArgs(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, int frameNumber)
			: base(frameNumber)
		{
			StudyInstanceUid = studyInstanceUid;
			SeriesInstanceUid = seriesInstanceUid;
			SopInstanceUid = sopInstanceUid;
		}

		public readonly string StudyInstanceUid;
		public readonly string SeriesInstanceUid;
		public readonly string SopInstanceUid;
	}

	/// <summary>
	/// Args for loading frame pixel data from a known SOP.
	/// </summary>
	public class LoadSopFramePixelDataArgs
	{
		public LoadSopFramePixelDataArgs(int frameNumber)
		{
			FrameNumber = frameNumber;
		}

		public readonly int FrameNumber;
	}

	/// <summary>
	/// Interface for pixel data, possibly compressed, that then handles decompression automatically.
	/// </summary>
	public interface IFramePixelData
	{
		/// <summary>
		/// Gets the number of bytes loaded or received.
		/// </summary>
		long BytesReceived { get; }

		/// <summary>
		/// Gets the uncompressed, but otherwise unaltered, pixel data.
		/// </summary>
		/// <param name="photometricInterpretation">The output photometric interpretation,
		/// which can differ from the header value when the pixel data is compressed.
		/// For example, some codes may convert YBR to RGB automatically on decompressing.
		/// </param>
		byte[] GetPixelData(out string photometricInterpretation);
	}

	/// <summary>
	/// Interface for loading a complete or partial DICOM File.
	/// </summary>
	public interface IDicomFileLoader
	{
		/// <summary>
		/// Can the entire header be retrieved?
		/// </summary>
		bool CanLoadCompleteHeader { get; }

		/// <summary>
		/// Can the pixel data be retrieved, along with the header, via <see cref="LoadDicomFile"/>?
		/// </summary>
		bool CanLoadPixelData { get; }

		/// <summary>
		/// Can pixel data for individual frame be retrieved via <see cref="LoadFramePixelData"/>?
		/// </summary>
		bool CanLoadFramePixelData { get; }

		/// <summary>
		/// Gets the SOP header, which may be complete or incomplete, and may or may not contain pixel data, given the input options.
		/// </summary>
		/// <param name="args">Instructions for what exactly to load.</param>
		/// <exception cref="NotSupportedException">If the arguments passed in are not consistent with
		/// <see cref="CanLoadCompleteHeader"/>, <see cref="CanLoadPixelData"/>. For example, <see cref="LoadDicomFileArgs.ForceCompleteHeader"/> cannot
		/// be true if <see cref="CanLoadCompleteHeader"/> is false. Nor can <see cref="LoadDicomFileArgs.IncludePixelData"/> if
		/// <see cref="CanLoadPixelData"/> is false.
		/// </exception>
		DicomFile LoadDicomFile(LoadDicomFileArgs args);
		/// <summary>
		/// Loads the pixel data for an individual frame.
		/// </summary>
		/// <param name="args">Instructions for what to load, which is essentially only the frame#.</param>
		/// <exception cref="NotSupportedException">If <see cref="CanLoadFramePixelData"/> is false.</exception>
		IFramePixelData LoadFramePixelData(LoadFramePixelDataArgs args);
	}

	/// <summary>
	/// Interface for loading a complete or partial DICOM File from a know SOP.
	/// </summary>
	public interface ISopDicomFileLoader
	{
		/// <summary>
		/// Can the entire header be retrieved?
		/// </summary>
		bool CanLoadCompleteHeader { get; }

		/// <summary>
		/// Can the pixel data be retrieved, along with the header, via <see cref="LoadDicomFile"/>?
		/// </summary>
		bool CanLoadPixelData { get; }

		/// <summary>
		/// Can pixel data for individual frame be retrieved via <see cref="LoadFramePixelData"/>?
		/// </summary>
		bool CanLoadFramePixelData { get; }

		/// <summary>
		/// Gets the SOP header, which may be complete or incomplete, and may or may not contain pixel data, given the input options.
		/// </summary>
		/// <param name="args">Instructions for what exactly to load.</param>
		/// <exception cref="NotSupportedException">If the arguments passed in are not consistent with
		/// <see cref="CanLoadCompleteHeader"/>, <see cref="CanLoadPixelData"/>. For example, <see cref="LoadDicomFileArgs.ForceCompleteHeader"/> cannot
		/// be true if <see cref="CanLoadCompleteHeader"/> is false. Nor can <see cref="LoadDicomFileArgs.IncludePixelData"/> if
		/// <see cref="CanLoadPixelData"/> is false.
		/// </exception>
		DicomFile LoadDicomFile(LoadSopDicomFileArgs args);
		/// <summary>
		/// Loads the pixel data for an individual frame.
		/// </summary>
		/// <param name="args">Instructions for what to load, which is essentially only the frame#.</param>
		/// <exception cref="NotSupportedException">If <see cref="CanLoadFramePixelData"/> is false.</exception>
		IFramePixelData LoadFramePixelData(LoadSopFramePixelDataArgs args);
	}

	public class SopDicomFileLoader : ISopDicomFileLoader
	{
		private readonly Func<LoadSopDicomFileArgs, DicomFile> _loadDicomFile;
		private readonly Func<LoadSopFramePixelDataArgs, IFramePixelData> _loadFramePixelData;

		public SopDicomFileLoader(bool canLoadCompleteHeader, bool canLoadPixelData, bool canLoadFramePixelData,
			Func<LoadSopDicomFileArgs, DicomFile> loadDicomFile, Func<LoadSopFramePixelDataArgs, IFramePixelData> loadFramePixelData)
		{
			_loadDicomFile = loadDicomFile;
			_loadFramePixelData = loadFramePixelData;
		
			CanLoadCompleteHeader = canLoadCompleteHeader;
			CanLoadPixelData = canLoadPixelData;
			CanLoadFramePixelData = canLoadFramePixelData;
		}

		#region ISopDicomFileLoader Members

		public bool CanLoadCompleteHeader { get; private set; }
		public bool CanLoadPixelData { get; private set; }
		public bool CanLoadFramePixelData { get; private set; }

		public DicomFile LoadDicomFile(LoadSopDicomFileArgs args)
		{
			if (args.ForceCompleteHeader && !CanLoadCompleteHeader)
				throw new NotSupportedException("Provider doesn't support loading the complete header.");
			if (args.IncludePixelData && !CanLoadPixelData)
				throw new NotSupportedException("Provider doesn't support inclusion of pixel data.");

			return _loadDicomFile(args);
		}

		public IFramePixelData LoadFramePixelData(LoadSopFramePixelDataArgs args)
		{
			if (!CanLoadFramePixelData)
				throw new NotSupportedException("Provider doesn't support loading individual frame pixel data.");

			return _loadFramePixelData(args);
		}

		#endregion
	}

	public delegate byte[] GetCompressedPixelData(out string photometricInterpretation);

	public class FramePixelData : IFramePixelData
	{
		private byte[] _uncompressedPixelData;
		private string _photometricInterpretation;
		private readonly GetCompressedPixelData _getUncompressedPixelData;

		public FramePixelData(byte[] uncompressedPixelData, string photometricInterpretation = null)
		{
			BytesReceived = uncompressedPixelData.Length;
			_uncompressedPixelData = uncompressedPixelData;
			_photometricInterpretation = photometricInterpretation;
		}

		public FramePixelData(long bytesReceived, GetCompressedPixelData getUncompressedPixelData)
		{
			BytesReceived = bytesReceived;
			_getUncompressedPixelData = getUncompressedPixelData;
		}

		#region IFramePixelData Members

		public long BytesReceived { get; private set; }

		public byte[] GetPixelData(out string photometricInterpretation)
		{
			if (_uncompressedPixelData != null)
			{
				photometricInterpretation = _photometricInterpretation;
				return _uncompressedPixelData;
			}

			_uncompressedPixelData = _getUncompressedPixelData(out _photometricInterpretation);
			photometricInterpretation = _photometricInterpretation;
			return _uncompressedPixelData;
		}

		#endregion
	}

	public class DicomFileLoader : IDicomFileLoader
	{
		private readonly Func<LoadDicomFileArgs, DicomFile> _loadDicomFile;
		private readonly Func<LoadFramePixelDataArgs, IFramePixelData> _loadFramePixelData;

		public DicomFileLoader(bool canLoadCompleteHeader, bool canLoadPixelData, bool canLoadFramePixelData,
			Func<LoadDicomFileArgs, DicomFile> loadDicomFile, Func<LoadFramePixelDataArgs, IFramePixelData> loadFramePixelData)
		{
			_loadDicomFile = loadDicomFile;
			_loadFramePixelData = loadFramePixelData;

			CanLoadCompleteHeader = canLoadCompleteHeader;
			CanLoadPixelData = canLoadPixelData;
			CanLoadFramePixelData = canLoadFramePixelData;
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
}