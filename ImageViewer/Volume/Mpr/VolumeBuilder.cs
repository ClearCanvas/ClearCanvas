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
using System.Drawing;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
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

			private const float _halfPi = (float) Math.PI/2;
			private const float _gantryTiltTolerance = 0.1f; // allowed tolerance for gantry tilt (in radians)
			private const float _orientationTolerance = 0.01f; // allowed tolerance for image orientation (direction cosine values)
			private const float _minimumSliceSpacing = 0.01f; // minimum spacing required between slices (in mm)
			private const float _sliceSpacingTolerance = 0.01f; // allowed tolerance for slice spacing (in mm)

			private readonly List<IFrameReference> _frames;
			private readonly CreateVolumeProgressCallback _callback;

			private Matrix _imageOrientationPatient;
			private Vector3D _imagePositionPatient;
			private Vector3D _voxelSpacing;
			private Size3D _volumeSize;
			private double? _gantryTilt;
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

			private Matrix ImageOrientationPatient
			{
				get
				{
					if (_imageOrientationPatient == null)
						_imageOrientationPatient = ImageOrientationPatientToMatrix(_frames[0].Frame.ImageOrientationPatient);
					return _imageOrientationPatient;
				}
			}

			private Vector3D ImagePositionPatient
			{
				get
				{
					if (_imagePositionPatient == null)
						_imagePositionPatient = ImagePositionPatientToVector(_frames[0].Frame.ImagePositionPatient);
					return _imagePositionPatient;
				}
			}

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

			private double GantryTilt
			{
				get
				{
					if (!_gantryTilt.HasValue)
					{
						double aboutXradians = this.ComputeTiltAboutX();
						double aboutYradians = this.ComputeTiltAboutY();

						if (aboutXradians != 0 && aboutYradians != 0)
							// should never happen, since the validation should have caught this already
							throw new Exception("Patient orientation is tilted about X and Y, not supported");

						if (aboutXradians != 0)
							// This flips euler sign in prone position, so that tilt is correctly signed
							_gantryTilt = aboutXradians*this.ImageOrientationPatient[0, 0];
						else if (aboutYradians != 0)
							// This flips euler sign in Decubitus Left position
							_gantryTilt = aboutYradians*this.ImageOrientationPatient[0, 1];
						else
							_gantryTilt = 0;
					}
					return _gantryTilt.Value;
				}
			}

			private int PaddingRows
			{
				get
				{
					if (!_paddingRows.HasValue)
					{
						// If the series was obtained with a Gantry/Detector Tilt, we will pad the frames as they're
						//	 added to the volume so as to create a normalized cuboid volume that contains the tilted volume.
						// This is the number of rows that we will pad for each frame, and it affects the overall
						//	 dimensions of the volume.
						// It is a function of the tilt angle and the run from first to last slice.
						double padRowsMm = Math.Tan(this.GantryTilt)*(_frames[_frames.Count - 1].Frame.ImagePositionPatient.Z - _frames[0].Frame.ImagePositionPatient.Z);

						// ensure this pad is always positive for sizing calculations
						_paddingRows = Math.Abs((int) (padRowsMm/this.VoxelSpacing.Y + 0.5f));
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
						// Relying on the frames being uniform, so we'll base width/height off of first frame
						_volumeSize = new Size3D(_frames[0].Frame.Columns, _frames[0].Frame.Rows + this.PaddingRows, _frames.Count);
					}
					return _volumeSize;
				}
			}

			#region Builder

			/// <summary>
			/// Creates and populates a <see cref="Volume"/> from the builder's source frames.
			/// </summary>
			public Volume Build()
			{
				PrepareFrames(_frames); // this also sorts the frames into order by slice location

				// Construct a model SOP data source based on the first frame's DICOM header
				var sopDataSourcePrototype = VolumeSopDataSourcePrototype.Create(_frames.Select(f => (IDicomAttributeProvider) f.Sop.DataSource).ToList(), 16, 16, false);

				// compute normalized modality LUT
				double normalizedSlope, normalizedIntercept;
				ComputeNormalizedModalityLut(_frames, out normalizedSlope, out normalizedIntercept);
				sopDataSourcePrototype[DicomTags.RescaleSlope].SetFloat64(0, normalizedSlope);
				sopDataSourcePrototype[DicomTags.RescaleIntercept].SetFloat64(0, normalizedIntercept);
				sopDataSourcePrototype[DicomTags.RescaleType] = _frames[0].Sop.DataSource[DicomTags.RescaleType].Copy();
				sopDataSourcePrototype[DicomTags.Units] = _frames[0].Sop.DataSource[DicomTags.Units].Copy(); // PET series use this attribute to designate rescale units

				// compute normalized VOI windows
				VoiWindow.SetWindows(ComputeNormalizedVoiWindows(_frames, normalizedSlope, normalizedIntercept), sopDataSourcePrototype);

				// compute the volume padding value
				var pixelPaddingValue = ComputePixelPaddingValue(_frames, normalizedSlope, normalizedIntercept);

				int minVolumeValue, maxVolumeValue;
				var volumeArray = BuildVolumeArray(pixelPaddingValue, normalizedSlope, normalizedIntercept, out minVolumeValue, out maxVolumeValue);
				var volume = new Volume(null, volumeArray, VolumeSize, VoxelSpacing, ImagePositionPatient, ImageOrientationPatient, sopDataSourcePrototype, pixelPaddingValue, _frames[0].Frame.SeriesInstanceUid, minVolumeValue, maxVolumeValue);
				return volume;
			}

			// Builds the volume array. Takes care of Gantry Tilt correction (pads rows at top/bottom accordingly)
			private ushort[] BuildVolumeArray(ushort pixelPadValue, double normalizedSlope, double normalizedIntercept, out int minVolumeValue, out int maxVolumeValue)
			{
				var volumeData = MemoryManager.Allocate<ushort>(VolumeSize.Volume, TimeSpan.FromSeconds(10));
				var min = int.MaxValue;
				var max = int.MinValue;

				float lastFramePos = (float) _frames[_frames.Count - 1].Frame.ImagePositionPatient.Z;

				int position = 0;
				using (var lutFactory = LutFactory.Create())
				{
					for (var n = 0; n < _frames.Count; n++)
					{
						var sourceFrame = _frames[n].Frame;
						var frameDimensions = VolumeSize;
						var paddingRows = PaddingRows;
						var gantryTilt = GantryTilt;

						// PadTop takes care of padding rows for gantry tilt correction
						int countRowsPaddedAtTop = 0;
						if (paddingRows > 0)
						{
							// figure out how many rows need to be padded at the top
							float deltaMm = lastFramePos - (float) sourceFrame.ImagePositionPatient.Z;
							double padTopMm = Math.Tan(gantryTilt)*deltaMm;
							countRowsPaddedAtTop = (int) (padTopMm/this.VoxelSpacing.Y + 0.5f);

							//TODO (cr Oct 2009): verify that IPP of the first image is correct for the volume.
							// account for the tilt in negative radians: we start padding from the bottom first in this case
							if (gantryTilt < 0)
								countRowsPaddedAtTop += paddingRows;

							int stop = position + countRowsPaddedAtTop*frameDimensions.Width;
							for (int i = position; i < stop; i++)
								volumeData[i] = pixelPadValue;
							position = stop;
						}

						// Copy frame data
						var frameData = sourceFrame.GetNormalizedPixelData();
						var frameBitsStored = sourceFrame.BitsStored;
						var frameBytesPerPixel = sourceFrame.BitsAllocated/8;
						var frameIsSigned = sourceFrame.PixelRepresentation != 0;
						var frameModalityLut = lutFactory.GetModalityLutLinear(frameBitsStored, frameIsSigned, sourceFrame.RescaleSlope, sourceFrame.RescaleIntercept);
						int frameMin, frameMax;
						CopyFrameData(frameData, frameBytesPerPixel, frameIsSigned, frameModalityLut, volumeData, position, normalizedSlope, normalizedIntercept, out frameMin, out frameMax);
						position += frameData.Length/sizeof (ushort);
						min = Math.Min(min, frameMin);
						max = Math.Max(max, frameMax);

						// Finish out any padding left over from PadTop
						if (paddingRows > 0) // Pad bottom
						{
							int stop = position + ((paddingRows - countRowsPaddedAtTop)*frameDimensions.Width);
							for (int i = position; i < stop; i++)
								volumeData[i] = pixelPadValue;
							position = stop;
						}

						// update progress
						_callback(n, _frames.Count);
					}
				}

				minVolumeValue = min;
				maxVolumeValue = max;
				return volumeData;
			}

			private static void CopyFrameData(byte[] frameData, int frameBytesPerPixel, bool frameIsSigned, IModalityLut frameModalityLut, ushort[] volumeData, int volumeStart, double normalizedSlope, double normalizedIntercept, out int minFramePixel, out int maxFramePixel)
			{
				var pixelCount = frameData.Length/frameBytesPerPixel;
				unsafe
				{
					var min = int.MaxValue;
					var max = int.MinValue;

					fixed (byte* pFrameData = frameData)
					fixed (ushort* pVolumeData = volumeData)
					{
						if (frameBytesPerPixel == 2)
						{
							var pFrameData16 = (short*) pFrameData;
							for (var i = 0; i < pixelCount; ++i)
							{
							    /// TODO (CR Nov 2011): Although perhaps less pretty, it's a bit more efficient
							    /// to break this out into 2 loops.
								var frameValue = frameModalityLut[frameIsSigned ? (int) pFrameData16[i] : (ushort) pFrameData16[i]];
								var volumeValue = (frameValue - normalizedIntercept)/normalizedSlope;
								var volumePixel = (ushort) Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, Math.Round(volumeValue)));
                                /// TODO (CR Nov 2011): Writes are volatile, so slightly more efficient to only do the assignment when necessary
                                min = Math.Min(min, volumePixel);
								max = Math.Max(max, volumePixel);
								pVolumeData[volumeStart + i] = volumePixel;
							}
						}
						else if (frameBytesPerPixel == 1)
						{
							for (var i = 0; i < pixelCount; ++i)
							{
                                /// TODO (CR Nov 2011): Although perhaps less pretty, it's a bit more efficient
                                /// to break this out into 2 loops.
                                var frameValue = frameModalityLut[frameIsSigned ? (int)(sbyte)pFrameData[i] : pFrameData[i]];
								var volumeValue = (frameValue - normalizedIntercept)/normalizedSlope;
								var volumePixel = (ushort) Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, Math.Round(volumeValue)));
							    /// TODO (CR Nov 2011): Writes are volatile, so slightly more efficient to only do the assignment when necessary
                                min = Math.Min(min, volumePixel);
								max = Math.Max(max, volumePixel);
								pVolumeData[volumeStart + i] = volumePixel;
							}
						}
					}

					minFramePixel = min;
					maxFramePixel = max;
				}
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

			private static IEnumerable<VoiWindow> ComputeNormalizedVoiWindows(IList<IFrameReference> frames, double normalizedSlope, double normalizedIntercept)
			{
				var normalizedWindows = new List<VoiWindow>(VoiWindow.GetWindows(frames[0].Sop.DataSource));
				if (frames[0].ImageSop.Modality == @"PT" && frames[0].Frame.IsSubnormalRescale)
				{
					// for PET images with subnormal rescale, the VOI window will always be applied directly to the original stored pixel values
					// since MPR will not have access to original stored pixel values, we compute the VOI window through original modality LUT and inverted normalized modality LUT
					var normalizedVoiSlope = frames[0].Frame.RescaleSlope/normalizedSlope;
					var normalizedVoiIntercept = (frames[0].Frame.RescaleIntercept - normalizedIntercept)/normalizedSlope;
					for (var i = 0; i < normalizedWindows.Count; ++i)
					{
						var window = normalizedWindows[i]; // round the computed windows - the extra precision is not useful for display anyway
						normalizedWindows[i] = new VoiWindow(Math.Ceiling(window.Width*normalizedVoiSlope), Math.Round(window.Center*normalizedVoiSlope + normalizedVoiIntercept), window.Explanation);
					}
				}
				return normalizedWindows;
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
				if (frames[0].Sop.DataSource.TryGetAttribute(DicomTags.PixelPaddingValue, out attribute))
					pixelPaddingValue = attribute.GetInt32(0, pixelPaddingValue);
				else if (frames[0].Sop.DataSource.TryGetAttribute(DicomTags.SmallestPixelValueInSeries, out attribute))
					pixelPaddingValue = attribute.GetInt32(0, pixelPaddingValue);
				else if (frames[0].Sop.DataSource.TryGetAttribute(DicomTags.SmallestImagePixelValue, out attribute))
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

			private double ComputeTiltAboutX()
			{
				float aboutXradians = (float) GetXRotation(this.ImageOrientationPatient);

				// If within specified tolerance of 0, Pi/2, -Pi/2, then treat as no tilt (return 0)
				if (FloatComparer.AreEqual(aboutXradians, 0f, _gantryTiltTolerance) ||
				    FloatComparer.AreEqual(Math.Abs(aboutXradians), _halfPi, _gantryTiltTolerance))
					return 0f;

				return aboutXradians;
			}

			private double ComputeTiltAboutY()
			{
				float aboutYradians = (float) GetYRotation(this.ImageOrientationPatient);

				// If within specified tolerance of 0, Pi/2, -Pi/2, then treat as no tilt (return 0)
				if (FloatComparer.AreEqual(aboutYradians, 0f, _gantryTiltTolerance) ||
				    FloatComparer.AreEqual(Math.Abs(aboutYradians), _halfPi, _gantryTiltTolerance))
					return 0f;

				return aboutYradians;
			}

			#endregion

			#region Validation and Preparation Helper

			/// <summary>
			/// Validates and prepares the provided frames for the <see cref="VolumeBuilder"/>.
			/// </summary>
			/// <exception cref="CreateVolumeException">Thrown if something is wrong with the source frames.</exception>
			public static void PrepareFrames(List<IFrameReference> _frames)
			{
				// ensure we have at least 3 frames
				if (_frames.Count < 3)
					throw new InsufficientFramesException();

				// ensure all frames have are from the same series, and have the same frame of reference
				string studyInstanceUid = _frames[0].Frame.StudyInstanceUid;
				string seriesInstanceUid = _frames[0].Frame.SeriesInstanceUid;
				string frameOfReferenceUid = _frames[0].Frame.FrameOfReferenceUid;

				if (string.IsNullOrEmpty(studyInstanceUid) || string.IsNullOrEmpty(seriesInstanceUid))
					throw new NullSourceSeriesException();
				if (string.IsNullOrEmpty(frameOfReferenceUid))
					throw new NullFrameOfReferenceException();

				foreach (IFrameReference frame in _frames)
				{
					if (frame.Frame.StudyInstanceUid != studyInstanceUid)
						throw new MultipleSourceSeriesException();
					if (frame.Frame.SeriesInstanceUid != seriesInstanceUid)
						throw new MultipleSourceSeriesException();
					if (frame.Frame.FrameOfReferenceUid != frameOfReferenceUid)
						throw new MultipleFramesOfReferenceException();
				}

				// ensure all frames are of the same supported format
				foreach (IFrameReference frame in _frames)
				{
					if (frame.Frame.BitsAllocated != 16)
						throw new UnsupportedPixelFormatSourceImagesException();
				}

				// ensure all frames have the same orientation
				ImageOrientationPatient orient = _frames[0].Frame.ImageOrientationPatient;
				foreach (IFrameReference frame in _frames)
				{
					if (frame.Frame.ImageOrientationPatient.IsNull)
					{
						if (frame.ImageSop.NumberOfFrames > 1)
							throw new UnsupportedMultiFrameSourceImagesException(new NullImageOrientationException());
						throw new NullImageOrientationException();
					}
					if (!frame.Frame.ImageOrientationPatient.EqualsWithinTolerance(orient, _orientationTolerance))
						throw new MultipleImageOrientationsException();
					if (frame.Frame.PixelSpacing.IsNull)
						throw new UncalibratedFramesException();
					if (Math.Abs(frame.Frame.PixelSpacing.AspectRatio - 1) > _minimumSliceSpacing)
						throw new AnisotropicPixelAspectRatioException();
				}

				// ensure all frames are sorted by slice location
				SliceLocationComparer sliceLocationComparer = new SliceLocationComparer();
				_frames.Sort(delegate(IFrameReference x, IFrameReference y) { return sliceLocationComparer.Compare(x.Frame, y.Frame); });

				// ensure all frames are equally spaced
				float? nominalSpacing = null;
				for (int i = 1; i < _frames.Count; i++)
				{
					float currentSpacing = CalcSpaceBetweenPlanes(_frames[i].Frame, _frames[i - 1].Frame);
					if (currentSpacing < _minimumSliceSpacing)
					{
						if (_frames[i].ImageSop.NumberOfFrames > 1)
							throw new UnsupportedMultiFrameSourceImagesException(new UnevenlySpacedFramesException());
						throw new UnevenlySpacedFramesException();
					}

					if (!nominalSpacing.HasValue)
						nominalSpacing = currentSpacing;
					if (!FloatComparer.AreEqual(currentSpacing, nominalSpacing.Value, _sliceSpacingTolerance))
						throw new UnevenlySpacedFramesException();
				}

				// ensure frames are not tilted about unsupposed axis combinations (the gantry correction algorithm only supports rotations about X)
				if (!IsSupportedGantryTilt(_frames)) // suffices to check first one... they're all co-planar now!!
					throw new UnsupportedGantryTiltAxisException();
			}

			#endregion

			#region Gantry Tilt Helpers

			private static bool IsSupportedGantryTilt(IList<IFrameReference> frames)
			{
				try
				{
					using (IPresentationImage firstImage = PresentationImageFactory.Create(frames[0].Frame))
					{
						using (IPresentationImage lastImage = PresentationImageFactory.Create(frames[frames.Count - 1].Frame))
						{
							// neither of these should return null since we already checked for image orientation and position (patient)
							DicomImagePlane firstImagePlane = DicomImagePlane.FromImage(firstImage);
							DicomImagePlane lastImagePlane = DicomImagePlane.FromImage(lastImage);

							Vector3D stackZ = lastImagePlane.PositionPatientTopLeft - firstImagePlane.PositionPatientTopLeft;
							Vector3D imageX = firstImagePlane.PositionPatientTopRight - firstImagePlane.PositionPatientTopLeft;

							if (!stackZ.IsOrthogonalTo(imageX, _gantryTiltTolerance))
							{
								// this is a gantry slew (gantry tilt about Y axis)
								return false;
							}
						}
					}
					return true;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Debug, ex, "Unexpected exception encountered while checking for supported gantry tilts");
					return false;
				}
			}

			/// <summary>
			/// Gets the rotation about the X-axis in radians.
			/// </summary>
			private static double GetXRotation(Matrix orientationPatient)
			{
				return Math.Atan2(orientationPatient[2, 1], orientationPatient[2, 2]);
			}

			/// <summary>
			/// Gets the rotation about the Y-axis in radians.
			/// </summary>
			private static double GetYRotation(Matrix orientationPatient)
			{
				return -Math.Asin(orientationPatient[2, 0]);
			}

			/// <summary>
			/// Gets the rotation about the Z-axis in radians.
			/// </summary>
			private static double GetZRotation(Matrix orientationPatient)
			{
				return Math.Atan2(orientationPatient[1, 0], orientationPatient[0, 0]);
			}

			#endregion

			#region Spacing and Orientation Helpers

			private static float CalcSpaceBetweenPlanes(Frame frame1, Frame frame2)
			{
				Vector3D point1 = frame1.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
				Vector3D point2 = frame2.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
				Vector3D delta = point1 - point2;

				return delta.IsNull ? 0f : delta.Magnitude;
			}

			private static Matrix ImageOrientationPatientToMatrix(ImageOrientationPatient orientation)
			{
				Vector3D xOrient = new Vector3D((float) orientation.RowX, (float) orientation.RowY, (float) orientation.RowZ);
				Vector3D yOrient = new Vector3D((float) orientation.ColumnX, (float) orientation.ColumnY, (float) orientation.ColumnZ);
				Vector3D zOrient = xOrient.Cross(yOrient);

				Matrix orientationMatrix = Math3D.OrientationMatrixFromVectors(xOrient, yOrient, zOrient);
				return orientationMatrix;
			}

			private static Vector3D ImagePositionPatientToVector(ImagePositionPatient position)
			{
				return new Vector3D((float) position.X, (float) position.Y, (float) position.Z);
			}

			#endregion
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