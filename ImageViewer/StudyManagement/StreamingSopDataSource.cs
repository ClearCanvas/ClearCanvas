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
	/// <see cref="DicomMessageSopDataSource"/> for a <see cref="StudyXml"/>-based study,
	/// where the complete header and per-frame pixel data can be retrieved on-demand via
	/// an <see cref="IDicomFileLoader"/>.
	/// </summary>
	/// <remarks>
	/// This class is optimized for the remote case, like streaming from an ImageServer, 
	/// as it has built-in mechanisms for retrying when pixel data fails to be retrieved.
	/// </remarks>
	public partial class StreamingSopDataSource : DicomMessageSopDataSource, IStreamingSopDataSource
	{
		private readonly IDicomFileLoader _loader;
		private volatile bool _fullHeaderRetrieved = false;

		public StreamingSopDataSource(DicomFile file, IDicomFileLoader loader)
			: base(file)
		{
			if (!loader.CanLoadCompleteHeader)
				throw new ArgumentException("Loader must be capable of retrieving the full image header.", "loader");
			if (!loader.CanLoadFramePixelData)
				throw new ArgumentException("Loader must be capable of loading frame pixel data.", "loader");

			_loader = loader;
			//Have to assume this to be the case.
			_fullHeaderRetrieved = true;
		}

		public StreamingSopDataSource(InstanceXml instanceXml, IDicomFileLoader loader)
			: base(new DicomFile("", new DicomAttributeCollection(), instanceXml.Collection))
		{
			if (!loader.CanLoadCompleteHeader)
				throw new ArgumentException("Loader must be capable of retrieving the full image header.", "loader");
			if (!loader.CanLoadFramePixelData)
				throw new ArgumentException("Loader must be capable of loading frame pixel data.", "loader");

			_loader = loader;
			//These don't get set properly for instance xml.
			var sourceFile = (DicomFile)SourceMessage;
			sourceFile.TransferSyntaxUid = instanceXml.TransferSyntax.UidString;
			sourceFile.MediaStorageSopInstanceUid = instanceXml.SopInstanceUid;

		    sourceFile.MetaInfo[DicomTags.SopClassUid].SetString(0,
		                                                         instanceXml.SopClass == null
		                                                             ? instanceXml[DicomTags.SopClassUid].ToString()
		                                                             : instanceXml.SopClass.Uid);
		}

		private InstanceXmlDicomAttributeCollection AttributeCollection
		{
			get { return (InstanceXmlDicomAttributeCollection)SourceMessage.DataSet; }
		}

		#region IStreamingSopDataSource Members

		public new IStreamingSopFrameData GetFrameData(int frameNumber)
		{
			return (IStreamingSopFrameData) base.GetFrameData(frameNumber);
		}

		#endregion

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				lock(SyncLock)
				{
					if (NeedFullHeader(tag.TagValue))
						GetFullHeader();

					return base[tag];
				}
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				lock (SyncLock)
				{
					if (NeedFullHeader(tag))
						GetFullHeader();

					return base[tag];
				}
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock(SyncLock)
			{
				if (NeedFullHeader(tag.TagValue))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			return new StreamingSopFrameData(frameNumber, this);
		}

		private bool NeedFullHeader(uint tag)
		{
			if (_fullHeaderRetrieved)
				return false;

			if (AttributeCollection.IsTagExcluded(tag))
				return true;

			DicomAttribute attribute = base[tag];
			if (attribute is DicomAttributeSQ)
			{
				DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
				if (items != null)
				{
					foreach (DicomSequenceItem item in items)
					{
						if (item is InstanceXmlDicomSequenceItem)
						{
							if (((InstanceXmlDicomSequenceItem) item).HasExcludedTags(true))
								return true;
						}
					}
				}
			}

			return false;
		}

		private void GetFullHeader()
		{
			if (!_fullHeaderRetrieved)
			{
				Exception retrieveException;
				DicomFile imageHeader = TryClientRetrieveImageHeader(out retrieveException);

				if (imageHeader != null)
				{
					base.SourceMessage = imageHeader;
					_fullHeaderRetrieved = true;
				}
				else
				{
					throw retrieveException;
				}
			}
		}

		private DicomFile TryClientRetrieveImageHeader(out Exception lastRetrieveException)
		{
			// retry parameters
			const int retryTimeout = 1500;
			int retryDelay = 50;
			int retryCounter = 0;

			CodeClock timeoutClock = new CodeClock();
			timeoutClock.Start();

			lastRetrieveException = null;

			while (true)
			{
				try
				{
					if (retryCounter > 0)
						Platform.Log(LogLevel.Info, "Retrying retrieve headers for Sop '{0}' (Attempt #{1})", this.SopInstanceUid, retryCounter);

					return _loader.LoadDicomFile(new LoadDicomFileArgs(this.StudyInstanceUid, this.SeriesInstanceUid, this.SopInstanceUid, true, false));
				}
				catch (Exception ex)
				{
					lastRetrieveException = ex;

					timeoutClock.Stop();
					if (timeoutClock.Seconds*1000 >= retryTimeout)
					{
						// log an alert that we are aborting (exception trace at debug level only)
						int elapsed = (int) (1000*timeoutClock.Seconds);
						Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Aborting after {1} attempts in {2} ms", this.SopInstanceUid, retryCounter, elapsed);
						Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Abort] Sop: {0}, Retry Attempts: {1}, Elapsed: {2} ms", this.SopInstanceUid, retryCounter, elapsed);
						break;
					}
					timeoutClock.Start();

					retryCounter++;

					// log the retry (exception trace at debug level only)
					Platform.Log(LogLevel.Warn, "Failed to retrieve headers for Sop '{0}'; Retrying in {1} ms", this.SopInstanceUid, retryDelay);
					Platform.Log(LogLevel.Debug, ex, "[GetHeaders Fail-Retry] Sop: {0}, Retry in: {1} ms", this.SopInstanceUid, retryDelay);
					
					MemoryManager.Collect(retryDelay);
					retryDelay *= 2;
				}
			}

			return null;
		}
	}
}
