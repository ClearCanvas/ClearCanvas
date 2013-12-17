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
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class StreamingPerformanceInfo
	{
		internal StreamingPerformanceInfo(DateTime start, DateTime end, long totalBytesTransferred)
		{
			StartTime = start;
			EndTime = end;
			TotalBytesTransferred = totalBytesTransferred;
		}

		public readonly DateTime StartTime;
		public readonly DateTime EndTime;
		public readonly long TotalBytesTransferred;

		public TimeSpan ElapsedTime
		{
			get { return EndTime.Subtract(StartTime); }
		}
	}

	public interface IStreamingSopFrameData : ISopFrameData
	{
		StreamingPerformanceInfo LastRetrievePerformanceInfo { get; }
		bool PixelDataRetrieved { get; }
		void RetrievePixelData();
	}

	partial class StreamingSopDataSource
	{
		protected class StreamingSopFrameData : StandardSopFrameData, IStreamingSopFrameData
		{
			private readonly FramePixelData _framePixelData;
			private readonly byte[][] _overlayData;

			public StreamingSopFrameData(int frameNumber, StreamingSopDataSource parent)
				: base(frameNumber, parent, LargeObjectContainerData.PresetNetworkLoadedData)
			{
				_framePixelData = new FramePixelData(this.Parent, frameNumber);
				_overlayData = new byte[16][];
			}

			public new StreamingSopDataSource Parent
			{
				get { return (StreamingSopDataSource) base.Parent; }
			}

			public bool PixelDataRetrieved
			{
				get { return _framePixelData.AlreadyRetrieved; }
			}

			public StreamingPerformanceInfo LastRetrievePerformanceInfo
			{
				get { return _framePixelData.LastRetrievePerformanceInfo; }
			}

			public void RetrievePixelData()
			{
				_framePixelData.Retrieve();
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				byte[] pixelData = _framePixelData.GetUncompressedPixelData();

				string photometricInterpretationCode = this.Parent[DicomTags.PhotometricInterpretation].ToString();
				PhotometricInterpretation pi = PhotometricInterpretation.FromCodeString(photometricInterpretationCode);

				TransferSyntax ts = TransferSyntax.GetTransferSyntax(this.Parent.TransferSyntaxUid);
				if (pi.IsColor)
				{
					if (ts == TransferSyntax.Jpeg2000ImageCompression ||
					    ts == TransferSyntax.Jpeg2000ImageCompressionLosslessOnly ||
					    ts == TransferSyntax.JpegExtendedProcess24 ||
					    ts == TransferSyntax.JpegBaselineProcess1)
						pi = PhotometricInterpretation.Rgb;

					pixelData = ToArgb(this.Parent, pixelData, pi);
				}
				else
				{
					var overlayPlaneModuleIod = new OverlayPlaneModuleIod(Parent);
					foreach (var overlayPlane in overlayPlaneModuleIod)
					{
						if (!overlayPlane.HasOverlayData && _overlayData[overlayPlane.Index] == null)
						{
							// if the overlay is embedded in pixel data and we haven't cached it yet, extract it now before we normalize the frame pixel data
							var overlayData = OverlayData.UnpackFromPixelData(overlayPlane.OverlayBitPosition, Parent[DicomTags.BitsAllocated].GetInt32(0, 0), false, pixelData);
							_overlayData[overlayPlane.Index] = overlayData;
						}
					}

					NormalizeGrayscalePixels(this.Parent, pixelData);
				}

				return pixelData;
			}

			/// <summary>
			/// Called by <see cref="StandardSopFrameData.GetNormalizedOverlayData"/> to create a new byte buffer containing normalized 
			/// overlay pixel data for a particular overlay plane.
			/// </summary>
			/// <remarks>
			/// See <see cref="StandardSopFrameData.GetNormalizedOverlayData"/> for details on the expected format of the byte buffer.
			/// </remarks>
			/// <param name="overlayNumber">The 1-based overlay plane number.</param>
			/// <returns>A new byte buffer containing the normalized overlay pixel data.</returns>
			protected override byte[] CreateNormalizedOverlayData(int overlayNumber)
			{
				var overlayIndex = overlayNumber - 1;

				byte[] overlayData = null;

				// check whether or not the overlay plane exists before attempting to ascertain
				// whether or not the overlay is embedded in the pixel data
				var overlayPlaneModuleIod = new OverlayPlaneModuleIod(Parent);
				if (overlayPlaneModuleIod.HasOverlayPlane(overlayIndex))
				{
					if (_overlayData[overlayIndex] == null)
					{
						var overlayPlane = overlayPlaneModuleIod[overlayIndex];
						if (!overlayPlane.HasOverlayData)
						{
							// if the overlay is embedded, trigger retrieval of pixel data which will populate the cache for us
							GetNormalizedPixelData();
						}
						else
						{
							// try to compute the offset in the OverlayData bit stream where we can find the overlay frame that applies to this image frame
							int overlayFrame;
							int bitOffset;
							if (overlayPlane.TryGetRelevantOverlayFrame(FrameNumber, Parent.NumberOfFrames, out overlayFrame) &&
							    overlayPlane.TryComputeOverlayDataBitOffset(overlayFrame, out bitOffset))
							{
								// offset found - unpack only that overlay frame
								var od = new OverlayData(bitOffset,
								                         overlayPlane.OverlayRows,
								                         overlayPlane.OverlayColumns,
								                         overlayPlane.IsBigEndianOW,
								                         overlayPlane.OverlayData);
								_overlayData[overlayIndex] = od.Unpack();
							}
							else
							{
								// no relevant overlay frame found - i.e. the overlay for this image frame is blank
								_overlayData[overlayIndex] = new byte[0];
							}
						}
					}

					overlayData = _overlayData[overlayIndex];
				}

				return overlayData;
			}

			protected override void OnUnloaded()
			{
				base.OnUnloaded();

				// dump pixel data retrieve results and stored overlays
				_framePixelData.Unload();
				_overlayData[0x0] = null;
				_overlayData[0x1] = null;
				_overlayData[0x2] = null;
				_overlayData[0x3] = null;
				_overlayData[0x4] = null;
				_overlayData[0x5] = null;
				_overlayData[0x6] = null;
				_overlayData[0x7] = null;
				_overlayData[0x8] = null;
				_overlayData[0x9] = null;
				_overlayData[0xA] = null;
				_overlayData[0xB] = null;
				_overlayData[0xC] = null;
				_overlayData[0xD] = null;
				_overlayData[0xE] = null;
				_overlayData[0xF] = null;
			}
		}

		private class FramePixelDataRetriever
		{
			private const int MaxRetryDelay = 10000; // 10 seconds

			public readonly string SopInstanceUid;
			public readonly string TransferSyntaxUid;

			private readonly string _studyInstanceUid;
			private readonly string _seriesInstanceUid;
			private readonly int _frameNumber;

			private readonly IDicomFileLoader _loader;

			public FramePixelDataRetriever(FramePixelData source)
			{
				_loader = source.Parent._loader;

				_studyInstanceUid = source.Parent.StudyInstanceUid;
				_seriesInstanceUid = source.Parent.SeriesInstanceUid;
				SopInstanceUid = source.Parent.SopInstanceUid;
				TransferSyntaxUid = source.Parent.TransferSyntaxUid;
				_frameNumber = source.FrameNumber;
			}

			public IFramePixelData Retrieve()
			{
				Exception retrieveException;
				var pixelData = TryClientRetrievePixelData(out retrieveException);

				if (pixelData != null)
					return pixelData;

				throw retrieveException;
			}

			private IFramePixelData TryClientRetrievePixelData(out Exception lastRetrieveException)
			{
				// retry parameters
				const int maxRetryCount = 10;
				const int retryTimeout = 1500;
				int retryDelay = 50;
				int retryCounter = 0;

				IFramePixelData result = null;
				lastRetrieveException = null;

				var timeoutClock = new CodeClock();
				timeoutClock.Start();

				while (true)
				{
					try
					{
						if (retryCounter > 0)
							Platform.Log(LogLevel.Info, "Retrying retrieve pixel data for Sop '{0}' (Attempt #{1})", this.SopInstanceUid, retryCounter);

						CodeClock statsClock = new CodeClock();
						statsClock.Start();

						result = _loader.LoadFramePixelData(new LoadFramePixelDataArgs(this._studyInstanceUid, this._seriesInstanceUid, this.SopInstanceUid, this._frameNumber));

						statsClock.Stop();

						Platform.Log(LogLevel.Debug, "[Retrieve Info] Sop/Frame: {0}/{1}, Transfer Syntax: {2}, Bytes transferred: {3}, Elapsed (s): {4}, Retries: {5}",
						             this.SopInstanceUid, this._frameNumber, this.TransferSyntaxUid,
						             result.BytesReceived, statsClock.Seconds, retryCounter);

						break;
					}
					catch (Exception ex)
					{
						lastRetrieveException = ex;

						timeoutClock.Stop();
						if (timeoutClock.Seconds*1000 >= retryTimeout || retryCounter >= maxRetryCount)
						{
							// log an alert that we are aborting (exception trace at debug level only)
							int elapsed = (int) (1000*timeoutClock.Seconds);
							Platform.Log(LogLevel.Warn, "Failed to retrieve pixel data for Sop '{0}'; Aborting after {1} attempts in {2} ms", this.SopInstanceUid, retryCounter, elapsed);
							Platform.Log(LogLevel.Debug, ex, "[Retrieve Fail-Abort] Sop/Frame: {0}/{1}, Retry Attempts: {2}, Elapsed: {3} ms", this.SopInstanceUid, this._frameNumber - 1, retryCounter,
							             elapsed);
							break;
						}
						timeoutClock.Start();

						retryCounter++;

						// log the retry (exception trace at debug level only)
						Platform.Log(LogLevel.Warn, "Failed to retrieve pixel data for Sop '{0}'; Retrying in {1} ms", this.SopInstanceUid, retryDelay);
						Platform.Log(LogLevel.Debug, ex, "[Retrieve Fail-Retry] Sop/Frame: {0}/{1}, Retry in: {2} ms", this.SopInstanceUid, this._frameNumber - 1, retryDelay);
						MemoryManager.Collect(retryDelay);
						retryDelay *= 2;

						if (retryDelay > MaxRetryDelay)
							retryDelay = MaxRetryDelay; // cap it to avoid overflow, which will cause exception when calling MemoryManager.Collect()
					}
				}

				return result;
			}
		}

		private class FramePixelData
		{
			private readonly object _syncLock = new object();
			private volatile bool _alreadyRetrieved;
			private IFramePixelData _framePixelData;
			private volatile StreamingPerformanceInfo _lastRetrievePerformanceInfo;

			public readonly StreamingSopDataSource Parent;
			public readonly int FrameNumber;

			private int _retrievesAttempted;
			private Exception _lastError;

			public FramePixelData(StreamingSopDataSource parent, int frameNumber)
			{
				Parent = parent;
				FrameNumber = frameNumber;
			}

			public bool AlreadyRetrieved
			{
				get { return _alreadyRetrieved; }
			}

			public StreamingPerformanceInfo LastRetrievePerformanceInfo
			{
				get { return _lastRetrievePerformanceInfo; }
			}

			public void Retrieve()
			{
				if (!_alreadyRetrieved)
				{
					//construct this object before the lock so there's no chance of deadlocking
					//with the parent data source (because we are accessing it's tags at the 
					//same time as it's trying to get the pixel data).
					var retriever = new FramePixelDataRetriever(this);

					lock (_syncLock)
					{
						if (!_alreadyRetrieved)
						{
							AbortAttemptIfNecessary();

							try
							{
								ResetAttemptData();
								_retrievesAttempted++;
								var start = DateTime.Now;
								_framePixelData = retriever.Retrieve();
								var end = DateTime.Now;
								_lastRetrievePerformanceInfo =
									new StreamingPerformanceInfo(start, end, _framePixelData.BytesReceived);

								_alreadyRetrieved = true;
							}
							catch (Exception ex)
							{
								_lastError = ex;
								throw;
							}
						}
					}
				}
			}

			public byte[] GetUncompressedPixelData()
			{
				try
				{

					//construct this object before the lock so there's no chance of deadlocking
					//with the parent data source (because we are accessing its tags at the 
					//same time as it's trying to get the pixel data).
					var retriever = new FramePixelDataRetriever(this);

					lock (_syncLock)
					{
						IFramePixelData framePixelData;

						if (_framePixelData == null)
						{
							AbortAttemptIfNecessary();

							ResetAttemptData();
							_retrievesAttempted++;

							framePixelData = retriever.Retrieve();
						}
						else
							framePixelData = _framePixelData;

						//free this memory up in case it's holding a compressed buffer.
						_framePixelData = null;

						var clock = new CodeClock();
						clock.Start();

						//synchronize the call to decompress; it's really already synchronized by
						//the parent b/c it's only called from CreateFrameNormalizedPixelData, but it doesn't hurt.
						byte[] pixelData = framePixelData.GetPixelData();

						clock.Stop();

						Platform.Log(LogLevel.Debug,
						             "[Decompress Info] Sop/Frame: {0}/{1}, Transfer Syntax: {2}, Uncompressed bytes: {3}, Elapsed (s): {4}",
						             retriever.SopInstanceUid, FrameNumber, retriever.TransferSyntaxUid,
						             pixelData.Length, clock.Seconds);

						return pixelData;
					}
				}
				catch (Exception ex)
				{
					_lastError = ex;
					throw;
				}
			}

			private void AbortAttemptIfNecessary()
			{
				if (_retrievesAttempted > 0 && _lastError != null)
				{
					Platform.Log(LogLevel.Warn, "Abort retrieving pixel data for Sop '{0}' Frame#{1}: attempt was made recently and failed with the following error", Parent.SopInstanceUid, FrameNumber);

                    // throw the same error so that same message is rendered
                    // TODO: avoid logging this in the renderer
					throw _lastError;
				}
			}

			private void ResetAttemptData()
			{
				_retrievesAttempted = 0;
				_lastError = null;
			}

			public void Unload()
			{
				lock (_syncLock)
				{
					_alreadyRetrieved = false;
					_framePixelData = null;
					ResetAttemptData();
				}
			}
		}
	}
}