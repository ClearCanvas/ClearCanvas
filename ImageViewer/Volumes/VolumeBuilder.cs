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
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volumes
{
	partial class Volume
	{
		/// <summary>
		/// Utility class for creating a <see cref="Volume"/> from a list of <see cref="Frame"/>s.
		/// </summary>
		/// <remarks>
		/// For internal use by the Volume static factory methods (Create).
		/// You MUST call Dispose() on this class after you're done to release the source SOP cache locks.
		/// </remarks>
		private class VolumeBuilder : IDisposable
		{
			// Future improvements list
			// - Deal with overlapping frames
			// - Support uneven spacing

			#region Private fields

			private const float _skewAngleTolerance = 0.1f; // allowed tolerance for skew angle (in radians) = ~5.7 degrees
			private const float _orientationTolerance = 0.01f; // allowed tolerance for image orientation (direction cosine values)
			private const float _minimumSliceSpacing = 0.001f; // minimum spacing required between slices (in mm)
			private const float _sliceSpacingTolerance = 0.01f; // allowed tolerance for slice spacing (in mm)

			private readonly List<IFrameReference> _frames;
			private readonly CreateVolumeProgressCallback _callback;

			private VolumeHeaderData _volumeHeaderData;
			private Matrix3D _volumeOrientationPatient;
			private Vector3D _volumePositionPatient;
			private Vector3D _voxelSpacing;
			private Size3D _volumeSize;
			private double? _skewAngleY;
			private int? _paddingRows;

			#endregion

			public VolumeBuilder(IEnumerable<Frame> frames, CreateVolumeProgressCallback callback)
			{
				_callback = callback ?? delegate { };
				_frames = new List<IFrameReference>();
				foreach (Frame frame in frames)
					_frames.Add(frame.CreateTransientReference());
			}

			public VolumeBuilder(IEnumerable<IFrameReference> frames, CreateVolumeProgressCallback callback)
			{
				_callback = callback ?? delegate { };
				_frames = new List<IFrameReference>();
				foreach (IFrameReference frame in frames)
					_frames.Add(frame.Clone());
			}

			public void Dispose()
			{
				foreach (IFrameReference frame in _frames)
					frame.Dispose();
				_frames.Clear();
			}

			/// <summary>
			/// Computes the orientation of the volume in patient coordinates.
			/// </summary>
			private Matrix3D VolumeOrientationPatient
			{
				get
				{
					if (_volumeOrientationPatient == null)
					{
						var imageOrientationPatient = _frames[0].Frame.ImageOrientationPatient;
						var volumeRowOrientation = new Vector3D((float) imageOrientationPatient.RowX, (float) imageOrientationPatient.RowY, (float) imageOrientationPatient.RowZ);
						var volumeColumnOrientation = new Vector3D((float) imageOrientationPatient.ColumnX, (float) imageOrientationPatient.ColumnY, (float) imageOrientationPatient.ColumnZ);
						var volumeStackOrientation = volumeRowOrientation.Cross(volumeColumnOrientation);
						_volumeOrientationPatient = Matrix3D.FromRows(volumeRowOrientation.Normalize(), volumeColumnOrientation.Normalize(), volumeStackOrientation.Normalize());
					}
					return _volumeOrientationPatient;
				}
			}

			/// <summary>
			/// Computes the position of the volume origin in patient coordinates after adjusting for source skew padding.
			/// </summary>
			private Vector3D VolumePositionPatient
			{
				get
				{
					if (_volumePositionPatient == null)
					{
						var imagePositionPatient = _frames[0].Frame.ImagePositionPatient;
						_volumePositionPatient = new Vector3D((float) imagePositionPatient.X, (float) imagePositionPatient.Y, (float) imagePositionPatient.Z);

						// if the volume is padded in order to normalize the source data (parallelepiped -> rect cuboid)
						// and the skew angle is positive (so that the padding for the first frame is at the top)
						// then we must offset the computed volume position in patient coordinates
						var paddingRows = PaddingRows;
						if (paddingRows > 0 && SkewAngleY > 0)
						{
							// the offset in patient units is given by number of padding rows times the voxel row spacing
							// and the direction is along the actual volume column orientation (middle row of the orientation matrix)
							var mmOffsetY = VoxelSpacing.Y*PaddingRows;
							_volumePositionPatient = _volumePositionPatient - mmOffsetY*VolumeOrientationPatient.GetRow(1);
						}
					}
					return _volumePositionPatient;
				}
			}

			/// <summary>
			/// Computes the voxel size in patient units.
			/// </summary>
			private Vector3D VoxelSpacing
			{
				get
				{
					if (_voxelSpacing == null)
					{
						Frame frame0 = _frames[0].Frame;
						Frame frame1 = _frames[1].Frame;
						PixelSpacing pixelSpacing = frame0.PixelSpacing;
						_voxelSpacing = new Vector3D((float) pixelSpacing.Column, (float) pixelSpacing.Row, CalcSpaceBetweenPlanes(frame0, frame1));
					}
					return _voxelSpacing;
				}
			}

			/// <summary>
			/// Gets the skew angle (radians) of the Y-axis represented by the source frames (angle between Y-axis and XZ-plane of the parallelepiped formed by the source frames). For explanation, see <see cref="IsSupportedSourceSkew"/>.
			/// </summary>
			private double SkewAngleY
			{
				get
				{
					if (!_skewAngleY.HasValue)
					{
						const double halfPi = Math.PI/2;
						var firstImagePlane = _frames[0].Frame.ImagePlaneHelper;
						var lastImagePlane = _frames[_frames.Count - 1].Frame.ImagePlaneHelper;

						var sourceZ = lastImagePlane.ImagePositionPatientVector - firstImagePlane.ImagePositionPatientVector;
						var sourceX = firstImagePlane.ImageRowOrientationPatientVector;

						if (!sourceZ.IsOrthogonalTo(sourceX, _skewAngleTolerance))
						{
							// should have been caught before we even get here, but let's just double check
							// this indicates a skew in the X-axis (X-axis and the YZ-plane of the source frames are not orthogonal)
							// typically, this is a result of a gantry slew in the acquisition setup
							throw new InvalidOperationException("Non-zero skew angle of X-axis detected");
						}

						// to compute the skew angle, project the Z unit axis on to the Y unit axis
						// (the previous check has already verified that X is mutually orthogonal to Y and Z, so we can ignore X)
						var projectionZonY = sourceZ.Normalize().Dot(firstImagePlane.ImageColumnOrientationPatientVector.Normalize());

						// if the projection is not zero, then the result is the cosine of the angle between the two vectors
						// and the skew angle is simply that angle less 90 degrees
						_skewAngleY = !FloatComparer.AreEqual(0, projectionZonY) ? Math.Acos(projectionZonY) - halfPi : 0;
					}
					return _skewAngleY.Value;
				}
			}

			/// <summary>
			/// Gets the number of padding rows necessary to normalize the source frames such that the represented volume is a rectangular cuboid.
			/// </summary>
			private int PaddingRows
			{
				get
				{
					if (!_paddingRows.HasValue)
					{
						// if the source frames have a non-zero skew (i.e. the volume represented is not a rectangular cuboid but a general parallelepiped),
						// we will pad the frames as they're copied to the array so as to create a normalized cuboid volume that contains the skewed source data.
						// This is the total number of rows that we will pad for each frame, and it affects the overall dimensions of the volume.
						// It is a function of the Y-axis skew angle and the range along the Z-axis of the source frames.
						var firstImagePlane = _frames[0].Frame.ImagePlaneHelper;
						var lastImagePlane = _frames[_frames.Count - 1].Frame.ImagePlaneHelper;
						var skewAngleY = SkewAngleY;

						var padRowsMm = !FloatComparer.AreEqual(skewAngleY, 0) ? Math.Sin(skewAngleY)*((lastImagePlane.ImagePositionPatientVector - firstImagePlane.ImagePositionPatientVector).Magnitude) : 0;

						// ensure this pad is always positive for sizing calculations, and round to nearest row
						// (assumes that the row spacing is small enough to make rounding negligible - otherwise we would need to interpolate)
						_paddingRows = (int) (Math.Abs(padRowsMm/VoxelSpacing.Y) + 0.5f);
					}
					return _paddingRows.Value;
				}
			}

			private Size3D VolumeSize
			{
				get
				{
					if (_volumeSize == null)
					{
						var firstFrame = _frames[0].Frame;
						_volumeSize = new Size3D(firstFrame.Columns, firstFrame.Rows + PaddingRows, _frames.Count);
					}
					return _volumeSize;
				}
			}

			#region Builder

			/// <summary>
			/// Creates the <see cref="VolumeHeaderData"/> for the builder's source frames.
			/// </summary>
			/// <returns></returns>
			public VolumeHeaderData BuildVolumeHeader()
			{
				if (_volumeHeaderData != null) return _volumeHeaderData;

				PrepareFrames(_frames); // this also sorts the frames into order by slice location

				// compute modality LUT parameters normalized for all frames
				double normalizedSlope, normalizedIntercept;
				var normalizedUnits = _frames[0].Frame.RescaleUnits;
				ComputeNormalizedModalityLut(_frames, out normalizedSlope, out normalizedIntercept);

				// determine an appropriate pixel padding value
				var pixelPaddingValue = ComputePixelPaddingValue(_frames, normalizedSlope, normalizedIntercept);

				// get the laterality of the anatomy being constructed
				var laterality = _frames[0].Frame.Laterality;

				// Construct a model SOP data source based on the first frame's DICOM header
				var header = new VolumeHeaderData(_frames.Select(f => (IDicomAttributeProvider) f.Frame).ToList(), VolumeSize, VoxelSpacing, VolumePositionPatient, VolumeOrientationPatient, 16, 16, false, pixelPaddingValue, normalizedSlope, normalizedIntercept, normalizedUnits, laterality);

				// determine how the normalized modality LUT affects VOI windows and update the header
				VoiWindow.SetWindows(ComputeAggregateNormalizedVoiWindows(_frames, normalizedSlope, normalizedIntercept), header);

				return _volumeHeaderData = header;
			}

			/// <summary>
			/// Creates and populates a <see cref="Volume"/> from the builder's source frames.
			/// </summary>
			public Volume Build()
			{
				var header = BuildVolumeHeader();

				// N.B.: this method only creates ushort volumes because it normalizes all the source frames so that the modality LUTs, VOI LUTs, etc. are consistent
				// all pixel data is rescaled appropriately before filling the volume data array
				int minVolumeValue, maxVolumeValue;
				var volumeArray = BuildVolumeArray((ushort) header.PaddingValue, header.RescaleSlope, header.RescaleIntercept, out minVolumeValue, out maxVolumeValue);
				var volume = new U16Volume(volumeArray, header, minVolumeValue, maxVolumeValue);
				return volume;
			}

			/// <summary>
			/// Builds the volume array. Takes care of Gantry Tilt correction (pads rows at top/bottom accordingly)
			/// </summary>
			private ushort[] BuildVolumeArray(ushort pixelPadValue, double normalizedSlope, double normalizedIntercept, out int minVolumeValue, out int maxVolumeValue)
			{
				var volumeSize = VolumeSize;
				var volumeData = MemoryManager.Allocate<ushort>(volumeSize.Volume, TimeSpan.FromSeconds(10));

				var refFramePos = _frames[0].Frame.ImagePlaneHelper.ImagePositionPatientVector;
				var paddingRows = PaddingRows;
				var skewAngleY = SkewAngleY;

				var volumePaddedFrameSize = volumeSize.Width*volumeSize.Height;

				var progressCallback = new FrameCopyProgressTracker(_frames.Count, _callback);
				var frameRanges = Enumerable.Range(0, _frames.Count)
				                            .AsParallel2()
				                            .Select(n =>
				                                    {
					                                    var position = n*volumePaddedFrameSize;
					                                    var sourceFrame = _frames[n].Frame;

					                                    int paddingTopRows = 0, paddingBottomRows = 0;
					                                    if (paddingRows > 0)
					                                    {
						                                    // determine the number of rows to be padded at top and bottom
						                                    var framePadRowsMm = Math.Sin(skewAngleY)*(refFramePos - sourceFrame.ImagePlaneHelper.ImagePositionPatientVector).Magnitude;
						                                    var framePadRows = Math.Min(paddingRows, (int) (Math.Abs(framePadRowsMm/VoxelSpacing.Y) + 0.5f));

						                                    // when the skew angle is negative, the above calculation gives the rows to pad at the top of the frame
						                                    paddingTopRows = skewAngleY < 0 ? framePadRows : paddingRows - framePadRows;
						                                    paddingBottomRows = paddingRows - paddingTopRows;
					                                    }

					                                    int frameMinPixelValue, frameMaxPixelValue;
					                                    using (var lutFactory = LutFactory.Create()) // lut factory isn't thread safe, so we create one when needed in the worker thread
						                                    FillVolumeFrame(volumeData, position, volumeSize.Width, sourceFrame, pixelPadValue, normalizedSlope, normalizedIntercept, paddingTopRows, paddingBottomRows, lutFactory, out frameMinPixelValue, out frameMaxPixelValue);
					                                    progressCallback.IncrementAndNotify();
					                                    return new {Min = frameMinPixelValue, Max = frameMaxPixelValue};
				                                    }).ToList();

				minVolumeValue = frameRanges.Select(r => r.Min).Min();
				maxVolumeValue = frameRanges.Select(r => r.Max).Max();
				return volumeData;
			}

			private static void FillVolumeFrame(ushort[] volumeData, int position, int paddedColumns, Frame sourceFrame, ushort pixelPadValue, double normalizedSlope, double normalizedIntercept, int paddingRowsTop, int paddingRowsBottom, LutFactory lutFactory, out int frameMinPixelValue, out int frameMaxPixelValue)
			{
				if (paddingRowsTop > 0) // pad rows at the top if required
				{
					var count = paddingRowsTop*paddedColumns;
					FillVolumeData(volumeData, pixelPadValue, position, count);
					position += count; // update position so frame is copied after the top padding
				}

				// Copy frame data
				var frameData = sourceFrame.GetNormalizedPixelData();
				var frameBitsStored = sourceFrame.BitsStored;
				var frameBytesPerPixel = sourceFrame.BitsAllocated/8;
				var frameIsSigned = sourceFrame.PixelRepresentation != 0;
				var frameModalityLut = lutFactory.GetModalityLutLinear(frameBitsStored, frameIsSigned, sourceFrame.RescaleSlope, sourceFrame.RescaleIntercept);
				CopyFrameData(frameData, frameBytesPerPixel, frameIsSigned, frameModalityLut, volumeData, position, normalizedSlope, normalizedIntercept, out frameMinPixelValue, out frameMaxPixelValue);

				if (paddingRowsBottom > 0) // pad rows at the bottom if required
				{
					position += frameData.Length/frameBytesPerPixel; // update position so bottom padding is copied after the frame data
					var count = paddingRowsBottom*paddedColumns;
					FillVolumeData(volumeData, pixelPadValue, position, count);
				}
			}

			private class FrameCopyProgressTracker
			{
				private readonly object _syncroot = new object();

				private readonly SynchronizationContext _synchronizationContext;
				private readonly CreateVolumeProgressCallback _callback;
				private readonly int _count;
				private volatile int _current;

				public FrameCopyProgressTracker(int count, CreateVolumeProgressCallback callback)
				{
					_current = -1; // the callback notifies using frame index, not frame number, so the first callback should be 0
					_count = count;
					_callback = callback;
					_synchronizationContext = SynchronizationContext.Current;
				}

				public void IncrementAndNotify()
				{
					if (_callback == null) return;

					// locking is not ideal, but it's the easiest way to ensure the progress callback isn't called in any order other than 0, 1, 2, ...
					lock (_syncroot)
					{
						++_current;
						if (_synchronizationContext != null)
							_synchronizationContext.Post(s => _callback.Invoke((int) s, _count), _current);
						else
							_callback.Invoke(_current, _count);
					}
				}
			}

			private static unsafe void FillVolumeData(ushort[] volumeData, ushort fillValue, int start, int count)
			{
				fixed (ushort* pVolumeData = volumeData)
				{
					var pVolumeFill = pVolumeData + start;
					for (var i = 0; i < count; ++i)
						*pVolumeFill++ = fillValue;
				}
			}

			private static unsafe void CopyFrameData(byte[] frameData, int frameBytesPerPixel, bool frameIsSigned, IModalityLut frameModalityLut, ushort[] volumeData, int volumeStart, double normalizedSlope, double normalizedIntercept, out int minFramePixel, out int maxFramePixel)
			{
				var pixelCount = frameData.Length/frameBytesPerPixel;
				var min = int.MaxValue;
				var max = int.MinValue;

				fixed (byte* pFrameData = frameData)
				fixed (ushort* pVolumeData = volumeData)
				{
					var pVolumeFrame = pVolumeData + volumeStart;
					if (frameBytesPerPixel == 2)
					{
						if (frameIsSigned)
						{
							var pFrameDataS16 = (short*) pFrameData;
							for (var i = 0; i < pixelCount; ++i)
							{
								var frameValue = frameModalityLut[*pFrameDataS16++];
								var volumeValue = (frameValue - normalizedIntercept)/normalizedSlope;
								var volumePixel = (ushort) Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, Math.Round(volumeValue)));
								if (volumePixel < min) min = volumePixel;
								if (volumePixel > max) max = volumePixel;
								*pVolumeFrame++ = volumePixel;
							}
						}
						else
						{
							var pFrameDataU16 = (ushort*) pFrameData;
							for (var i = 0; i < pixelCount; ++i)
							{
								var frameValue = frameModalityLut[*pFrameDataU16++];
								var volumeValue = (frameValue - normalizedIntercept)/normalizedSlope;
								var volumePixel = (ushort) Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, Math.Round(volumeValue)));
								if (volumePixel < min) min = volumePixel;
								if (volumePixel > max) max = volumePixel;
								*pVolumeFrame++ = volumePixel;
							}
						}
					}
					else if (frameBytesPerPixel == 1)
					{
						if (frameIsSigned)
						{
							var pFrameDataS8 = (sbyte*) pFrameData;
							for (var i = 0; i < pixelCount; ++i)
							{
								var frameValue = frameModalityLut[*pFrameDataS8++];
								var volumeValue = (frameValue - normalizedIntercept)/normalizedSlope;
								var volumePixel = (ushort) Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, Math.Round(volumeValue)));
								if (volumePixel < min) min = volumePixel;
								if (volumePixel > max) max = volumePixel;
								*pVolumeFrame++ = volumePixel;
							}
						}
						else
						{
							var pFrameDataU8 = pFrameData;
							for (var i = 0; i < pixelCount; ++i)
							{
								var frameValue = frameModalityLut[*pFrameDataU8++];
								var volumeValue = (frameValue - normalizedIntercept)/normalizedSlope;
								var volumePixel = (ushort) Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, Math.Round(volumeValue)));
								if (volumePixel < min) min = volumePixel;
								if (volumePixel > max) max = volumePixel;
								*pVolumeFrame++ = volumePixel;
							}
						}
					}
					else
					{
						throw new ArgumentOutOfRangeException("frameBytesPerPixel");
					}
				}

				minFramePixel = min;
				maxFramePixel = max;
			}

			#endregion

			#region Misc

			private static void ComputeNormalizedModalityLut(IEnumerable<IFrameReference> frames, out double slope, out double intercept)
			{
				var rangeMin = double.MaxValue;
				var rangeMax = double.MinValue;
				foreach (var frameReference in frames)
				{
					var bitsStored = frameReference.Frame.BitsStored;
					var isSigned = frameReference.Frame.PixelRepresentation != 0;
					var rescaleSlope = frameReference.Frame.RescaleSlope;
					var rescaleIntercept = frameReference.Frame.RescaleIntercept;
					rangeMax = Math.Max(rangeMax, DicomPixelData.GetMaxPixelValue(bitsStored, isSigned)*rescaleSlope + rescaleIntercept);
					rangeMin = Math.Min(rangeMin, DicomPixelData.GetMinPixelValue(bitsStored, isSigned)*rescaleSlope + rescaleIntercept);
				}
				intercept = rangeMin;
				slope = (rangeMax - rangeMin)/65535;
			}

			private static IEnumerable<VoiWindow> ComputeAggregateNormalizedVoiWindows(IEnumerable<IFrameReference> frames, double normalizedSlope, double normalizedIntercept)
			{
				// aggregate all the VOI windows for the source frames by index - i.e. 1st window is aggregate of all 1st windows in the input, 2nd window is aggregate of all 2nd windows, etc.
				var frameWindows = frames.Select(f => ComputeNormalizedVoiWindows(f, normalizedSlope, normalizedIntercept).ToArray()).ToArray();
				return Enumerable.Range(0, frameWindows.Select(w => w.Length).Max())
				                 .Select(i => frameWindows.Select(f => f.Length > i ? f[i] : null))
				                 .Select(w => ComputeAggregateVoiWindow(w.Where(v => v != null).ToList())).ToList();
			}

			private static IEnumerable<VoiWindow> ComputeNormalizedVoiWindows(IImageSopProvider frame, double normalizedSlope, double normalizedIntercept)
			{
				var normalizedWindows = new List<VoiWindow>(VoiWindow.GetWindows(frame.Frame));
				if (frame.ImageSop.Modality == @"PT" && frame.Frame.IsSubnormalRescale)
				{
					// for PET images with subnormal rescale, the VOI window will always be applied directly to the original stored pixel values
					// since MPR will not have access to original stored pixel values, we compute the VOI window through original modality LUT and inverted normalized modality LUT
					var normalizedVoiSlope = frame.Frame.RescaleSlope/normalizedSlope;
					var normalizedVoiIntercept = (frame.Frame.RescaleIntercept - normalizedIntercept)/normalizedSlope;
					for (var i = 0; i < normalizedWindows.Count; ++i)
					{
						var window = normalizedWindows[i]; // round the computed windows - the extra precision is not useful for display anyway
						normalizedWindows[i] = new VoiWindow(Math.Ceiling(window.Width*normalizedVoiSlope), Math.Round(window.Center*normalizedVoiSlope + normalizedVoiIntercept), window.Explanation);
					}
				}
				return normalizedWindows;
			}

			private static VoiWindow ComputeAggregateVoiWindow(IList<VoiWindow> voiWindows)
			{
				// conservative way to aggregate each window - take the max of all upper bounds, and the min of all lower bounds
				var max = voiWindows.Select(w => w.Center + w.Width/2).Max();
				var min = voiWindows.Select(w => w.Center - w.Width/2).Min();
				var exp = voiWindows.Select(w => w.Explanation).FirstOrDefault();
				return new VoiWindow(max - min, (max + min)/2, exp);
			}

			/// <summary>
			/// Computes a pixel value suitable for volume padding purposes.
			/// </summary>
			/// <remarks>
			/// This function will try to use the value of either <see cref="DicomTags.PixelPaddingValue"/>,
			/// <see cref="DicomTags.SmallestPixelValueInSeries"/> or <see cref="DicomTags.SmallestImagePixelValue"/>
			/// (in order of preference). If none of these attributes exist in the dataset, then a suitable value is
			/// computed based on <see cref="DicomTags.BitsStored"/>, <see cref="DicomTags.PixelRepresentation"/>,
			/// and <see cref="DicomTags.PhotometricInterpretation"/>.
			/// </remarks>
			private static ushort ComputePixelPaddingValue(IList<IFrameReference> frames, double normalizedSlope, double normalizedIntercept)
			{
				var isSigned = frames[0].Frame.PixelRepresentation != 0;
				var isMonochrome1 = frames[0].Frame.PhotometricInterpretation.Equals(PhotometricInterpretation.Monochrome1);
				var bitsStored = Math.Max(1, Math.Min(16, frames[0].Frame.BitsStored));
				var pixelPaddingValue = isMonochrome1 ? DicomPixelData.GetMaxPixelValue(bitsStored, isSigned) : DicomPixelData.GetMinPixelValue(bitsStored, isSigned);

				DicomAttribute attribute;
				var frameAttributeProvider = (IDicomAttributeProvider) frames[0].Frame;
				if (frameAttributeProvider.TryGetAttribute(DicomTags.PixelPaddingValue, out attribute))
					pixelPaddingValue = attribute.GetInt32(0, pixelPaddingValue);
				else if (frameAttributeProvider.TryGetAttribute(DicomTags.SmallestPixelValueInSeries, out attribute))
					pixelPaddingValue = attribute.GetInt32(0, pixelPaddingValue);
				else if (frameAttributeProvider.TryGetAttribute(DicomTags.SmallestImagePixelValue, out attribute))
					pixelPaddingValue = attribute.GetInt32(0, pixelPaddingValue);

				// these next few lines may look positively stupid, but they are here for a good reason (See Ticket #7026)
				if (isSigned)
					pixelPaddingValue = (short) pixelPaddingValue;
				else
					pixelPaddingValue = (ushort) pixelPaddingValue;

				// since the volume now stores values with original modality LUT and normalizing function applied, we must apply same to this value (#9417)
				var rescaledPaddingValue = frames[0].Frame.RescaleSlope*pixelPaddingValue + frames[0].Frame.RescaleIntercept;
				var normalizedPaddingValue = (rescaledPaddingValue - normalizedIntercept)/normalizedSlope;

				return (ushort) Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, Math.Round(normalizedPaddingValue)));
			}

			#endregion

			#region Validation and Preparation Helper

			/// <summary>
			/// Validates and prepares the provided frames for the <see cref="VolumeBuilder"/>.
			/// </summary>
			/// <exception cref="CreateVolumeException">Thrown if something is wrong with the source frames.</exception>
			public static void PrepareFrames(List<IFrameReference> frames)
			{
				// ensure we have at least 3 frames
				if (frames.Count < 3)
					throw new InsufficientFramesException();

				// ensure all frames have are from the same series, and have the same frame of reference
				string studyInstanceUid = frames[0].Frame.StudyInstanceUid;
				string seriesInstanceUid = frames[0].Frame.SeriesInstanceUid;
				string frameOfReferenceUid = frames[0].Frame.FrameOfReferenceUid;

				if (string.IsNullOrEmpty(studyInstanceUid) || string.IsNullOrEmpty(seriesInstanceUid))
					throw new NullSourceSeriesException();
				if (string.IsNullOrEmpty(frameOfReferenceUid))
					throw new NullFrameOfReferenceException();

				foreach (IFrameReference frame in frames)
				{
					if (frame.Frame.StudyInstanceUid != studyInstanceUid)
						throw new MultipleSourceSeriesException();
					if (frame.Frame.SeriesInstanceUid != seriesInstanceUid)
						throw new MultipleSourceSeriesException();
					if (frame.Frame.FrameOfReferenceUid != frameOfReferenceUid)
						throw new MultipleFramesOfReferenceException();
				}

				// ensure all frames are of the same supported format
				if (frames.Any(frame => frame.Frame.BitsAllocated != 16))
					throw new UnsupportedPixelFormatSourceImagesException();

				// ensure all frames have the same rescale function units
				if (frames.Select(frame => frame.Frame.RescaleUnits).Distinct().Count() > 1)
					throw new InconsistentRescaleFunctionTypeException();

				// ensure all frames have the same orientation
				var orient = frames[0].Frame.ImageOrientationPatient;
				var laterality = frames[0].Frame.Laterality;
				foreach (IFrameReference frame in frames)
				{
					if (frame.Frame.ImageOrientationPatient.IsNull)
						throw new NullImageOrientationException();
					if (!frame.Frame.ImageOrientationPatient.EqualsWithinTolerance(orient, _orientationTolerance))
						throw new MultipleImageOrientationsException();
					if (frame.Frame.PixelSpacing.IsNull)
						throw new UncalibratedFramesException();
					if (Math.Abs(frame.Frame.PixelSpacing.AspectRatio - 1) > _sliceSpacingTolerance)
						throw new AnisotropicPixelAspectRatioException();
					if (laterality != frame.Frame.Laterality)
						throw new InconsistentImageLateralityException();
				}

				// ensure all frames are sorted by slice location
				SliceLocationComparer sliceLocationComparer = new SliceLocationComparer();
				frames.Sort((x, y) => sliceLocationComparer.Compare(x.Frame, y.Frame));

				// ensure all frames are evenly spaced
				var spacing = new float[frames.Count - 1];
				for (int i = 1; i < frames.Count; i++)
				{
					float currentSpacing = CalcSpaceBetweenPlanes(frames[i].Frame, frames[i - 1].Frame);
					if (currentSpacing < _minimumSliceSpacing)
						throw new UnevenlySpacedFramesException();

					spacing[i - 1] = currentSpacing;
				}
				if (spacing.Max() - spacing.Min() > 2*_sliceSpacingTolerance)
					throw new UnevenlySpacedFramesException();

				// ensure frames are not skewed about unsupported axis combinations (this volume builder only normalizes for skew between the Y-axis and XZ-plane in the source data)
				if (!IsSupportedSourceSkew(frames))
					throw new UnsupportedGantryTiltAxisException();
			}

			/// <summary>
			/// Checks for skew in the orientation of the source frames. Returns true if source frames are suitable.
			/// </summary>
			private static bool IsSupportedSourceSkew(IList<IFrameReference> frames)
			{
				// The Volume class requires that the data represents a rectangular cuboid region of the patient
				// If the source frames do not represent a rectangular cuboid but rather a general parallelepiped, we have to pad the data when filling the volume
				// At present, we only support row padding when filling the volume - which means we can take data where the Y-axis is not necessarily orthogonal to the XZ-plane
				// (here, X refers to the source row orientation vector, Y refers to the source column orientation vector, and Z refers to the source stack orientation vector)
				// We do NOT support column padding, so we must reject data where the X-axis is not orthogonal to the YZ-plane 
				// Typically, such data arises from gantry tilted and gantry slewed acquisitions - but breast tomosynthesis images have non-orthogonal skews too (i.e. medial-lateral *oblique*)

				// N.B. Definition of Tilt and Slew from DICOM PS 3.3 C.8.22.5.2 description of the tags Gantry/Detector Tilt (0018,1120) and Gantry/Detector Slew (0018,1121)
				// Nominal angle of tilt... Zero degrees means the gantry is not tilted, negative degrees are when the top of the gantry is tilted away from where the table enters the gantry.
				// Nominal angle of slew... Zero degrees means the gantry is not slewed. Positive slew is moving the gantry on the patient’s left toward the patient’s superior, when the patient is supine.
				//
				//                          Feed Direction: >>>
				//           View from Side                     View from Top
				//
				//              ANTERIOR                              LEFT
				//       -Tilt \         / +Tilt            -Slew \         / +Slew
				//              \       /                          \       /
				//     FEET }--------------}O HEAD       FEET }-----------------}O HEAD
				//                \   /                              \   /
				//                 \ /                                \ /
				//              POSTERIOR                            RIGHT

				try
				{
					// neither of these should return null since we already checked for image orientation and position (patient)
					var firstImagePlane = frames[0].Frame.ImagePlaneHelper;
					var lastImagePlane = frames[frames.Count - 1].Frame.ImagePlaneHelper;

					var sourceZ = lastImagePlane.ImagePositionPatientVector - firstImagePlane.ImagePositionPatientVector;
					var sourceX = firstImagePlane.ImageRowOrientationPatientVector;

					if (!sourceZ.IsOrthogonalTo(sourceX, _skewAngleTolerance))
					{
						// this indicates a skew in the X-axis (X-axis and the YZ-plane of the source frames are not orthogonal)
						// typically, this is a result of a gantry slew in the acquisition setup
						return false;
					}
					return true;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "Unexpected exception encountered while checking for skew in the source data");
					return false;
				}
			}

			#endregion

			#region Spacing and Orientation Helpers

			/// <summary>
			/// Calculates the inter-slice spacing in patient units, orthogonal to the image plane.
			/// </summary>
			private static float CalcSpaceBetweenPlanes(Frame frame1, Frame frame2)
			{
				var point1 = frame1.ImagePlaneHelper.ImageTopLeftPatient;
				var point2 = frame2.ImagePlaneHelper.ImageTopLeftPatient;
				var delta = point1 - point2;

				// spacing between images should be measured along normal to image plane, regardless of actual orientation of images! (e.g. consider gantry tiled images)
				return delta.IsNull ? 0f : Math.Abs(delta.Dot(frame1.ImagePlaneHelper.ImageNormalPatient));
			}

			#endregion
		}

		/// <summary>
		/// Builds only the header for a volume of the given frames without loading the volume itself.
		/// </summary>
		/// <param name="frames"></param>
		/// <returns></returns>
		internal static VolumeHeaderData BuildHeader(IEnumerable<IFrameReference> frames)
		{
			Platform.CheckForNullReference(frames, "frames");
			using (var builder = new VolumeBuilder(frames.ToList(), null))
			{
				return builder.BuildVolumeHeader();
			}
		}

		/// <summary>
		/// Validates the specified display set as suitable source data for <see cref="Volume"/>.
		/// </summary>
		/// <param name="displaySet">The display set to validate.</param>
		/// <exception cref="ArgumentNullException">Thrown if the parameter was NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified source data is not suitable for creating a <see cref="Volume"/>.</exception>
		public static void Validate(IDisplaySet displaySet)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			Validate(displaySet.PresentationImages.Cast<IImageSopProvider>().Select(i => i.Frame));
		}

		/// <summary>
		/// Validates the specified frames as suitable source data for <see cref="Volume"/>.
		/// </summary>
		/// <param name="frames">The frames to be validated.</param>
		/// <exception cref="ArgumentNullException">Thrown if the parameter was NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified source data is not suitable for creating a <see cref="Volume"/>.</exception>
		public static void Validate(IEnumerable<Frame> frames)
		{
			Platform.CheckForNullReference(frames, "frames");
			var frameReferences = frames.Select(f => f.CreateTransientReference()).ToList();
			try
			{
				VolumeBuilder.PrepareFrames(frameReferences);
			}
			finally
			{
				foreach (var reference in frameReferences)
					reference.Dispose();
			}
		}

		/// <summary>
		/// Validates the specified frames as suitable source data for <see cref="Volume"/>.
		/// </summary>
		/// <param name="frames">The frames to be validated.</param>
		/// <exception cref="ArgumentNullException">Thrown if the parameter was NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified source data is not suitable for creating a <see cref="Volume"/>.</exception>
		public static void Validate(IEnumerable<IFrameReference> frames)
		{
			Platform.CheckForNullReference(frames, "frames");
			VolumeBuilder.PrepareFrames(frames.ToList());
		}
	}
}