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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface IStreamingSopDataSource : IDicomMessageSopDataSource
	{
		new IStreamingSopFrameData GetFrameData(int frameNumber);
	}

	/// <summary>
	/// A <see cref="XmlSopDataSource"/> where the complete header and per-frame pixel data 
	/// can be retrieved on-demand via an <see cref="IDicomFileLoader"/>.
	/// </summary>
	/// <remarks>
	/// This class is optimized for the remote case, like streaming from an ImageServer, 
	/// as it has built-in mechanisms for retrying when pixel data fails to be retrieved.
	/// </remarks>
	public partial class StreamingSopDataSource : XmlSopDataSource, IStreamingSopDataSource
	{
		private readonly ISopDicomFileLoader _loader;

		public StreamingSopDataSource(InstanceXml instanceXml, IDicomFileLoader loader)
			: base(instanceXml)
		{
			_loader = ConvertLoader(loader);
			CheckLoaderCapabilities();
		}

		public StreamingSopDataSource(InstanceXml instanceXml, ISopDicomFileLoader loader)
			: base(instanceXml)
		{
			_loader = loader;
			CheckLoaderCapabilities();
		}

		private void CheckLoaderCapabilities()
		{
			if (!_loader.CanLoadCompleteHeader)
				throw new ArgumentException("Loader must be capable of retrieving the full image header.");
			if (!_loader.CanLoadFramePixelData)
				throw new ArgumentException("Loader must be capable of loading frame pixel data.");
		}

		private ISopDicomFileLoader ConvertLoader(IDicomFileLoader loader)
		{
			return new SopDicomFileLoader(loader.CanLoadCompleteHeader, loader.CanLoadPixelData, loader.CanLoadFramePixelData,
				args => loader.LoadDicomFile(new LoadDicomFileArgs(StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, args.ForceCompleteHeader, args.IncludePixelData)),
				args => loader.LoadFramePixelData(new LoadFramePixelDataArgs(StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, args.FrameNumber)));
		}

		#region IStreamingSopDataSource Members

		public new IStreamingSopFrameData GetFrameData(int frameNumber)
		{
			return (IStreamingSopFrameData) base.GetFrameData(frameNumber);
		}

		#endregion

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			return new StreamingSopFrameData(frameNumber, this);
		}

		protected override DicomFile GetFullHeader()
		{
			Exception retrieveException;
			DicomFile imageHeader = TryClientRetrieveImageHeader(out retrieveException);
			if (imageHeader != null)
				return imageHeader;
				
			throw retrieveException;
		}

		private DicomFile TryClientRetrieveImageHeader(out Exception lastRetrieveException)
		{
			// retry parameters
			const int retryTimeout = 1500;
			int retryDelay = 50;
			int retryCounter = 0;

			var timeoutClock = new CodeClock();
			timeoutClock.Start();

			lastRetrieveException = null;

			while (true)
			{
				try
				{
					if (retryCounter > 0)
						Platform.Log(LogLevel.Info, "Retrying retrieve headers for Sop '{0}' (Attempt #{1})", SopInstanceUid, retryCounter);

					return _loader.LoadDicomFile(new LoadDicomFileArgs(StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, true, false));
				}
				catch (Exception ex)
				{
					lastRetrieveException = ex;

					timeoutClock.Stop();
					if (timeoutClock.Seconds*1000 >= retryTimeout)
					{
						// log an alert that we are aborting (exception trace at debug level only)
						var elapsed = (int) (1000*timeoutClock.Seconds);
						Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Aborting after {1} attempts in {2} ms", SopInstanceUid, retryCounter, elapsed);
						Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Abort] Sop: {0}, Retry Attempts: {1}, Elapsed: {2} ms", SopInstanceUid, retryCounter, elapsed);
						break;
					}
					timeoutClock.Start();

					retryCounter++;

					// log the retry (exception trace at debug level only)
					Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Retrying in {1} ms", SopInstanceUid, retryDelay);
					Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Retry] Sop: {0}, Retry in: {1} ms", SopInstanceUid, retryDelay);

					MemoryManager.Collect(retryDelay);
					retryDelay *= 2;
				}
			}

			return null;
		}
	}
}