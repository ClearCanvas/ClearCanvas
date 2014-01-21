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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.IO;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// An implementation of a <see cref="StandardSopDataSource"/> where a <see cref="DicomMessageBase"/>
	/// provides all the SOP instance data.
	/// </summary>
	public class DicomMessageSopDataSource : StandardSopDataSource, IDicomMessageSopDataSource
	{
		private readonly DicomAttributeCollection _dummy;
		private DicomMessageBase _sourceMessage;
		private bool _loaded = false;
		private bool _loading = false;

		/// <summary>
		/// Constructs a new <see cref="DicomMessageSopDataSource"/>.
		/// </summary>
		/// <param name="sourceMessage">The source <see cref="DicomMessageBase"/> which provides all the SOP instance data.</param>
		protected DicomMessageSopDataSource(DicomMessageBase sourceMessage)
		{
			_dummy = new DicomAttributeCollection();
			SetSourceMessage(sourceMessage);
		}

		/// <summary>
		/// Gets the source <see cref="DicomMessageBase"/>.
		/// </summary>
		/// <remarks>
		/// See the remarks for <see cref="IDicomMessageSopDataSource"/>.
		/// </remarks>
		public DicomMessageBase SourceMessage
		{
			// TODO: not actually thread-safe because client code can use indexers on "SourceMessage",
			// triggering empty attributes to be inserted.

			get
			{
				return GetSourceMessage(true);
			}
			protected set
			{
				lock (SyncLock)
				{
					SetSourceMessage(value);
				}
			}
		}

		protected DicomMessageBase GetSourceMessage(bool requireLoad)
		{
			lock (SyncLock)
			{
				if (requireLoad)
					Load();

				return _sourceMessage;
			}
		}

		private void SetSourceMessage(DicomMessageBase sourceMessage)
		{
			_sourceMessage = sourceMessage;
			_loaded = !_sourceMessage.DataSet.IsEmpty();
		}

		/// <summary>
		/// Called by the base class to ensure that all DICOM data attributes are loaded.
		/// </summary>
		protected virtual void EnsureLoaded()
		{
			//TODO: push up?
		}

		//TODO: is there a better way to do this?
		private void Load()
		{
			lock(SyncLock)
			{
				if (_loaded || _loading)
					return;

				try
				{
					_loading = true;
					EnsureLoaded();
					_loaded = true;
				}
				finally
				{
					_loading = false;
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				lock (SyncLock)
				{
					Load();

					DicomAttribute attribute;
					if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
						return attribute;

					if (_sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute))
						return attribute;

					return _dummy[tag];
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="DicomAttribute"/> for the given tag.
		/// </summary>
		public override DicomAttribute this[uint tag]
		{
			get
			{
				lock (SyncLock)
				{
					Load();

					DicomAttribute attribute;
					if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
						return attribute;

					if (_sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute))
						return attribute;

					return _dummy[tag];
				}
			}
		}

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				Load();

				if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
					return true;

				return _sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute);
			}
		}

		/// <summary>
		/// Attempts to get the attribute specified by <paramref name="tag"/>.
		/// </summary>
		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				Load();

				if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
					return true;

				return _sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute);
			}
		}

		#region Frame Data Handling

		/// <summary>
		/// Called by the base class to create a new <see cref="StandardSopDataSource.StandardSopFrameData"/>
		/// containing the data for a particular frame in the SOP instance.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
		/// <returns>A new <see cref="StandardSopDataSource.StandardSopFrameData"/> containing the data for a particular frame in the SOP instance.</returns>
		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			return new DicomMessageSopFrameData(frameNumber, this);
		}

		private class OverlayDataCache
		{
			private readonly byte[][] _data = new byte[16][];

			public byte[] this[int overlayIndex]
			{
				get { return _data[overlayIndex]; }
				set { _data[overlayIndex] = value; }
			}

			public void Clear()
			{
				_data[0x0] = null;
				_data[0x1] = null;
				_data[0x2] = null;
				_data[0x3] = null;
				_data[0x4] = null;
				_data[0x5] = null;
				_data[0x6] = null;
				_data[0x7] = null;
				_data[0x8] = null;
				_data[0x9] = null;
				_data[0xA] = null;
				_data[0xB] = null;
				_data[0xC] = null;
				_data[0xD] = null;
				_data[0xE] = null;
				_data[0xF] = null;
			}
		}

		/// <summary>
		/// An implementation of a <see cref="StandardSopDataSource.StandardSopFrameData"/>
		/// where a <see cref="DicomMessageBase"/> provides all the frame data.
		/// </summary>
		protected class DicomMessageSopFrameData : StandardSopFrameData
		{
			private readonly OverlayDataCache _overlayCache = new OverlayDataCache();
			private readonly int _frameIndex;

			/// <summary>
			/// Constructs a new <see cref="DicomMessageSopFrameData"/>
			/// </summary>
			/// <param name="frameNumber">The 1-based number of this frame.</param>
			/// <param name="parent">The parent <see cref="DicomMessageSopDataSource"/> that this frame belongs to.</param>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
			public DicomMessageSopFrameData(int frameNumber, DicomMessageSopDataSource parent)
				: base(frameNumber, parent)
			{
				_frameIndex = frameNumber - 1;
			}

            /// <summary>
            /// Constructs a new <see cref="DicomMessageSopFrameData"/>
            /// </summary>
            /// <param name="frameNumber">The 1-based number of this frame.</param>
            /// <param name="parent">The parent <see cref="DicomMessageSopDataSource"/> that this frame belongs to.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
            /// <param name="regenerationCost">The approximate cost to regenerate the pixel and/or overlay data.</param>
            public DicomMessageSopFrameData(int frameNumber, DicomMessageSopDataSource parent, RegenerationCost regenerationCost)
                : base(frameNumber, parent, regenerationCost)
            {
                _frameIndex = frameNumber - 1;
            }

			/// <summary>
			/// Gets the parent <see cref="DicomMessageSopDataSource"/> to which this frame belongs.
			/// </summary>
			public new DicomMessageSopDataSource Parent
			{
				get { return (DicomMessageSopDataSource) base.Parent; }
			}

			/// <summary>
			/// Called by the base class to create a new byte buffer containing normalized pixel data
			/// for this frame (8 or 16-bit grayscale, or 32-bit ARGB).
			/// </summary>
			/// <returns>A new byte buffer containing the normalized pixel data.</returns>
			protected override byte[] CreateNormalizedPixelData()
			{
				DicomMessageBase message = this.Parent.SourceMessage;

				CodeClock clock = new CodeClock();
				clock.Start();

				PhotometricInterpretation photometricInterpretation;
				byte[] rawPixelData = null;

				if (!message.TransferSyntax.Encapsulated)
				{
					DicomUncompressedPixelData pixelData = new DicomUncompressedPixelData(message);
					// DICOM library uses zero-based frame numbers
					MemoryManager.Execute(delegate { rawPixelData = pixelData.GetFrame(_frameIndex); });

					ExtractOverlayFrames(rawPixelData, pixelData.BitsAllocated);

					photometricInterpretation = PhotometricInterpretation.FromCodeString(message.DataSet[DicomTags.PhotometricInterpretation]);
				}
				else if (DicomCodecRegistry.GetCodec(message.TransferSyntax) != null)
				{
					DicomCompressedPixelData pixelData = new DicomCompressedPixelData(message);
					string pi = null;

					MemoryManager.Execute(delegate { rawPixelData = pixelData.GetFrame(_frameIndex, out pi); });

					photometricInterpretation = PhotometricInterpretation.FromCodeString(pi);
				}
				else
					throw new DicomCodecException("Unsupported transfer syntax");

				if (photometricInterpretation.IsColor)
					rawPixelData = ToArgb(message.DataSet, rawPixelData, photometricInterpretation);
				else
					NormalizeGrayscalePixels(message.DataSet, rawPixelData);

				clock.Stop();
				PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "CreateFrameNormalizedPixelData", clock.Seconds);

				return rawPixelData;
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
				//TODO (CR December 2010): make this a helper method somewhere, since it's now identical to the one in StreamingSopFrameData?

				var overlayIndex = overlayNumber - 1;

				byte[] overlayData = null;

				var clock = new CodeClock();
				clock.Start();

				// check whether or not the overlay plane exists before attempting to ascertain
				// whether or not the overlay is embedded in the pixel data
				var overlayPlaneModuleIod = new OverlayPlaneModuleIod(Parent);
				if (overlayPlaneModuleIod.HasOverlayPlane(overlayIndex))
				{
					if (_overlayCache[overlayIndex] == null)
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
								_overlayCache[overlayIndex] = od.Unpack();
							}
							else
							{
								// no relevant overlay frame found - i.e. the overlay for this image frame is blank
								_overlayCache[overlayIndex] = new byte[0];
							}
						}
					}

					overlayData = _overlayCache[overlayIndex];
				}

				clock.Stop();
				PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "CreateNormalizedOverlayData", clock.Seconds);

				return overlayData;
			}

			private void ExtractOverlayFrames(byte[] rawPixelData, int bitsAllocated)
			{
				// if any overlays have embedded pixel data, extract them now or forever hold your peace
				var overlayPlaneModuleIod = new OverlayPlaneModuleIod(Parent);
				foreach (var overlayPlane in overlayPlaneModuleIod)
				{
					if (!overlayPlane.HasOverlayData && _overlayCache[overlayPlane.Index] == null)
					{
						// if the overlay is embedded in pixel data and we haven't cached it yet, extract it now before we normalize the frame pixel data
						var overlayData = OverlayData.UnpackFromPixelData(overlayPlane.OverlayBitPosition, bitsAllocated, false, rawPixelData);
						_overlayCache[overlayPlane.Index] = overlayData;
					}
				}
			}

			/// <summary>
			/// Called by the base class when the cached byte buffers are being unloaded.
			/// </summary>
			protected override void OnUnloaded()
			{
				base.OnUnloaded();
				_overlayCache.Clear();
			}
		}

		#endregion

		#region Unit Test Entry Points

#if UNIT_TESTS

		internal static void TestNormalizeGrayscalePixels(IDicomAttributeProvider dicomAttributeProvider, byte[] pixelData, Endian endian)
		{
			NormalizeGrayscalePixels(dicomAttributeProvider, pixelData, endian);
		}

#endif

		#endregion

		#region Pixel Data Processing Functions

		/// <summary>
		/// Normalizes grayscale pixel data by masking out non-data bits and shifting the data bits to start at the lowest bit position.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The pixel data is normalized such that the effective high bit is precisely 1 less than bits stored.
		/// Filling of the high-order non-data bits is performed according to the sign of the pixel value when
		/// the pixel representation is signed, and always with 0 when the pixel reresentation is unsigned.
		/// </para>
		/// <para>
		/// The provided <paramref name="dicomAttributeProvider"/> is <b>not</b> updated with the effective high bit,
		/// nor is the <see cref="DicomTags.PixelData"/> attribute updated in any way. The only change is to the given
		/// pixel data buffer such that pixels can be read, 8 or 16 bits at a time, and interpreted immediately as
		/// <see cref="byte"/> or <see cref="ushort"/> (or their signed equivalents <see cref="sbyte"/> and <see cref="short"/>).
		/// </para>
		/// </remarks>
		/// <param name="dicomAttributeProvider">A dataset containing information about the representation of pixels in the <paramref name="pixelData"/>.</param>
		/// <param name="pixelData">The pixel data buffer to normalize.</param>
		protected static void NormalizeGrayscalePixels(IDicomAttributeProvider dicomAttributeProvider, byte[] pixelData)
		{
			NormalizeGrayscalePixels(dicomAttributeProvider, pixelData, ByteBuffer.LocalMachineEndian);
		}

		private static void NormalizeGrayscalePixels(IDicomAttributeProvider dicomAttributeProvider, byte[] pixelData, Endian endian)
		{
			if (dicomAttributeProvider[DicomTags.BitsAllocated].IsEmpty)
				throw new ArgumentNullException("dicomAttributeProvider", "BitsAllocated must not be empty.");
			if (dicomAttributeProvider[DicomTags.BitsStored].IsEmpty)
				throw new ArgumentNullException("dicomAttributeProvider", "BitsStored must not be empty.");

			int bitsAllocated = dicomAttributeProvider[DicomTags.BitsAllocated].GetInt32(0, -1);
			int bitsStored = dicomAttributeProvider[DicomTags.BitsStored].GetInt32(0, -1);
			int highBit = dicomAttributeProvider[DicomTags.HighBit].GetInt32(0, bitsStored - 1);
			bool isSigned = dicomAttributeProvider[DicomTags.PixelRepresentation].GetInt32(0, 0) > 0;

			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("BitsAllocated must be either 8 or 16.", "dicomAttributeProvider");
			if (highBit + 1 < bitsStored || highBit >= bitsAllocated)
				highBit = bitsStored - 1; // #10029 - if high bit is out of range for some reason, just assume bits stored - 1 (which is probably the case for 99% of all DICOM images)

			unsafe
			{
				// TODO (CR Aug 2013): Replace with DicomUncompressedPixelData.NormalizePixelData.

				//TODO (CR May 2011): this is not as efficient as it could be, and more confusing than it needs to be.
				//It could mostly be done with bit-wise operators if we cast the pixel data pointer to the correct type
				//right off the bat (short* or byte*), and we could optimize for the case where no right shift is necessary (high bit = bits stored - 1).
				//For signed data, there is also a trick that can be done where you left shift the value, then immediately
				//right shift it again in order to fill the high order bits with 1s.
				int shift = highBit + 1 - bitsStored;
				if (bitsAllocated == 16)
				{
					if (pixelData.Length%2 != 0)
						throw new ArgumentException("Pixel data length must be even.", "pixelData");

					ushort mask = (ushort) ((1 << bitsStored) - 1); // this is the mask of data bits when the LSB is at 0
					ushort inputMask = (ushort) (mask << shift); // this is the mask of data bits in the input window
					int length = pixelData.Length;
					fixed (byte* data = pixelData)
					{
						ushort window;

						if (isSigned)
						{
							ushort signMask = (ushort) (1 << (bitsStored - 1)); // this is the mask of the sign bit when the LSB is at 0
							ushort signFill = (ushort) ~mask; // this is the mask of the sign bits used to fill the high-order non-data bits

							if (endian == Endian.Little)
							{
								for (int n = 0; n < length; n += 2)
								{
									window = (ushort) ((data[n + 1] << 8) + data[n]);
									window = (ushort) ((window & inputMask) >> shift);
									if ((window & signMask) > 0)
										window = (ushort) (window | signFill);
									data[n] = (byte) (window & 0x00ff);
									data[n + 1] = (byte) ((window & 0xff00) >> 8);
								}
							}
							else
							{
								for (int n = 0; n < length; n += 2)
								{
									window = (ushort) ((data[n] << 8) + data[n + 1]);
									window = (ushort) ((window & inputMask) >> shift);
									if ((window & signMask) > 0)
										window = (ushort) (window | signFill);
									data[n + 1] = (byte) (window & 0x00ff);
									data[n] = (byte) ((window & 0xff00) >> 8);
								}
							}
						}
						else
						{
							if (endian == Endian.Little)
							{
								for (int n = 0; n < length; n += 2)
								{
									window = (ushort) ((data[n + 1] << 8) + data[n]);
									window = (ushort) ((window & inputMask) >> shift);
									data[n] = (byte) (window & 0x00ff);
									data[n + 1] = (byte) ((window & 0xff00) >> 8);
								}
							}
							else
							{
								for (int n = 0; n < length; n += 2)
								{
									window = (ushort) ((data[n] << 8) + data[n + 1]);
									window = (ushort) ((window & inputMask) >> shift);
									data[n + 1] = (byte) (window & 0x00ff);
									data[n] = (byte) ((window & 0xff00) >> 8);
								}
							}
						}
					}
				}
				else
				{
					byte mask = (byte) ((1 << bitsStored) - 1); // this is the mask of data bits when the LSB is at 0
					byte inputMask = (byte) (mask << shift); // this is the mask of data bits in the input window
					int length = pixelData.Length;
					fixed (byte* data = pixelData)
					{
						if (isSigned)
						{
							byte signMask = (byte) (1 << (bitsStored - 1)); // this is the mask of the sign bit when the LSB is at 0
							byte signFill = (byte) ~mask; // this is the mask of the sign bits used to fill the high-order non-data bits

							for (int n = 0; n < length; n++)
							{
								byte window = data[n];
								window = (byte) ((window & inputMask) >> shift);
								if ((window & signMask) > 0)
									window = (byte) (window | signFill);
								data[n] = window;
							}
						}
						else
						{
							for (int n = 0; n < length; n++)
							{
								data[n] = (byte) ((data[n] & inputMask) >> shift);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Converts colour pixel data to ARGB.
		/// </summary>
		protected static byte[] ToArgb(IDicomAttributeProvider dicomAttributeProvider, byte[] pixelData, PhotometricInterpretation photometricInterpretation)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			int rows = dicomAttributeProvider[DicomTags.Rows].GetInt32(0, 0);
			int columns = dicomAttributeProvider[DicomTags.Columns].GetInt32(0, 0);
			int sizeInBytes = rows * columns * 4;
			byte[] argbPixelData = MemoryManager.Allocate<byte>(sizeInBytes);

			// Convert palette colour images to ARGB so we don't get interpolation artifacts
			// when rendering.
			if (photometricInterpretation == PhotometricInterpretation.PaletteColor)
			{
				int bitsAllocated = dicomAttributeProvider[DicomTags.BitsAllocated].GetInt32(0, 0);
				int pixelRepresentation = dicomAttributeProvider[DicomTags.PixelRepresentation].GetInt32(0, 0);

				ColorSpaceConverter.ToArgb(
					bitsAllocated,
					pixelRepresentation != 0 ? true : false,
					pixelData,
					argbPixelData,
					PaletteColorMap.Create(dicomAttributeProvider));
			}
			// Convert RGB and YBR variants to ARGB
			else
			{
				int planarConfiguration = dicomAttributeProvider[DicomTags.PlanarConfiguration].GetInt32(0, 0);

				ColorSpaceConverter.ToArgb(
					photometricInterpretation,
					planarConfiguration,
					pixelData,
					argbPixelData);
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "ToArgb", clock.Seconds);

			return argbPixelData;
		}

		#endregion
	}
}
