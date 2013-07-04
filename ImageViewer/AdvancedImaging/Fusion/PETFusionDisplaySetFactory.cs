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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Utilities;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public class PETFusionDisplaySetFactory : DisplaySetFactory
	{
		private const float _halfPi = (float) Math.PI/2;
		private const float _gantryTiltTolerance = 0.1f; // allowed tolerance for gantry tilt (in radians)
		private const float _orientationTolerance = 0.01f; // allowed tolerance for image orientation (direction cosine values)
		private const float _minimumSliceSpacing = 0.01f; // minimum spacing required between slices (in mm)
		private const float _sliceSpacingTolerance = 0.001f; // allowed tolerance for slice spacing (in mm)
		private const string _attenuationCorrectionCode = "ATTN";
		private const string _petModality = "PT";

		private readonly PETFusionType _fusionType;

		public PETFusionDisplaySetFactory(PETFusionType fusionType)
		{
			_fusionType = fusionType;
		}

		public PETFusionType FusionType
		{
			get { return _fusionType; }
		}

		private static bool IsAttenuationCorrected(Sop sop)
		{
			var correctionTypes = DicomStringHelper.GetStringArray(sop[DicomTags.CorrectedImage].ToString().ToUpperInvariant());
			return (Array.FindIndex(correctionTypes, s => s == _attenuationCorrectionCode) >= 0);
		}

		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();
			if (IsValidPETFusionSeries(series))
			{
				var fuseableBaseSeries = new List<Series>(FindFuseableBaseSeries(series));
				if (fuseableBaseSeries.Count > 0)
				{
					string error;
					if (!CheckPETFusionSeries(series, out error))
					{
						// if there is an error with the PET series, avoid trying to generate the volume entirely
						// instead, generate a placeholder series for each base series
						foreach (var baseSeries in fuseableBaseSeries)
							displaySets.Add(CreateFusionErrorDisplaySet(baseSeries, series, error));
						return displaySets;
					}

					using (var fusionOverlayData = new FusionOverlayData(GetFrames(series.Sops)))
					{
						foreach (var baseSeries in fuseableBaseSeries)
						{
							if (!CheckBaseSeries(baseSeries, out error))
							{
								// if there is an error with a single base series, generate a placeholder series
								displaySets.Add(CreateFusionErrorDisplaySet(baseSeries, series, error));
								continue;
							}

							var descriptor = new PETFusionDisplaySetDescriptor(baseSeries.GetIdentifier(), series.GetIdentifier(), IsAttenuationCorrected(series.Sops[0]));
							var displaySet = new DisplaySet(descriptor);
							foreach (var baseFrame in GetFrames(baseSeries.Sops))
							{
								using (var fusionOverlaySlice = fusionOverlayData.CreateOverlaySlice(baseFrame))
								{
									var fus = new FusionPresentationImage(baseFrame, fusionOverlaySlice);
									displaySet.PresentationImages.Add(fus);
								}
							}
							displaySet.PresentationImages.Sort();
							displaySets.Add(displaySet);
						}
					}
				}
			}
			return displaySets;
		}

		private static DisplaySet CreateFusionErrorDisplaySet(Series baseSeries, Series fusionSeries, string error)
		{
			// create a basic descriptor that's templated from the real PET fusion display descriptor
			var baseDescriptor = new PETFusionDisplaySetDescriptor(baseSeries.GetIdentifier(), fusionSeries.GetIdentifier(), IsAttenuationCorrected(fusionSeries.Sops[0]));
			var descriptor = new BasicDisplaySetDescriptor {Description = baseDescriptor.Description, Name = baseDescriptor.Name, Number = baseDescriptor.Number, Uid = baseDescriptor.Uid};
			var displaySet = new DisplaySet(descriptor);
			displaySet.PresentationImages.Add(new ErrorPresentationImage(SR.MessageFusionError + Environment.NewLine + string.Format(SR.FormatReason, error)));
			return displaySet;
		}

		public bool IsValidBaseSeries(Series series)
		{
			switch (_fusionType)
			{
				case PETFusionType.CT:
					if (series.Modality != _fusionType.ToString())
						return false;

					var frames = GetFrames(series.Sops);
					if (frames.Count < 5)
						return false;
					return true;
			}
			return false;
		}

		public bool CheckBaseSeries(Series series, out string error)
		{
			if (!IsValidBaseSeries(series))
			{
				error = SR.MessageInvalidSeries;
				return false;
			}

			var frames = GetFrames(series.Sops);

			if (!AssertSameSeriesFrameOfReference(frames))
			{
				error = SR.MessageInconsistentFramesOfReference;
				return false;
			}

			foreach (Frame frame in frames)
			{
				if (frame.ImageOrientationPatient.IsNull)
				{
					error = SR.MessageMissingImageOrientation;
					return false;
				}

				//TODO (CR Sept 2010): ImagePositionPatient?
				if (frame.NormalizedPixelSpacing.IsNull)
				{
					error = SR.MessageMissingPixelSpacing;
					return false;
				}
			}

			error = null;
			return true;
		}

		public bool IsValidPETFusionSeries(Series series)
		{
			if (series.Modality != _petModality)
				return false;

			var frames = GetFrames(series.Sops);
			if (frames.Count < 5)
				return false;

			return true;
		}

		public bool CheckPETFusionSeries(Series series, out string error)
		{
			if (!IsValidPETFusionSeries(series))
			{
				error = SR.MessageInvalidSeries;
				return false;
			}

			var frames = GetFrames(series.Sops);

			if (!AssertSameSeriesFrameOfReference(frames))
			{
				error = SR.MessageInconsistentFramesOfReference;
				return false;
			}

			// ensure all frames have the same orientation
			ImageOrientationPatient orient = frames[0].ImageOrientationPatient;
			double minColumnSpacing = double.MaxValue, minRowSpacing = double.MaxValue;
			double maxColumnSpacing = double.MinValue, maxRowSpacing = double.MinValue;
			foreach (Frame frame in frames)
			{
				if (frame.ImageOrientationPatient.IsNull)
				{
					error = SR.MessageMissingImageOrientation;
					return false;
				}

				if (!frame.ImageOrientationPatient.EqualsWithinTolerance(orient, _orientationTolerance))
				{
					error = SR.MessageInconsistentImageOrientation;
					return false;
				}

				PixelSpacing pixelSpacing = frame.NormalizedPixelSpacing;
				if (pixelSpacing.IsNull)
				{
					error = SR.MessageMissingPixelSpacing;
					return false;
				}

				minColumnSpacing = Math.Min(minColumnSpacing, pixelSpacing.Column);
				maxColumnSpacing = Math.Max(maxColumnSpacing, pixelSpacing.Column);
				minRowSpacing = Math.Min(minRowSpacing, pixelSpacing.Row);
				maxRowSpacing = Math.Max(maxRowSpacing, pixelSpacing.Row);
			}

			//Aren't many of these rules taken from MPR?  If so, we should create a rule object.

			// ensure all frames have consistent pixel spacing
			if (maxColumnSpacing - minColumnSpacing > _sliceSpacingTolerance || maxRowSpacing - minRowSpacing > _sliceSpacingTolerance)
			{
				error = SR.MessageInconsistentPixelSpacing;
				return false;
			}

			// ensure all frames are sorted by slice location
			frames.Sort(new SliceLocationComparer().Compare);

			// ensure all frames are equally spaced
			float? nominalSpacing = null;
			for (int i = 1; i < frames.Count; i++)
			{
				float currentSpacing = CalcSpaceBetweenPlanes(frames[i], frames[i - 1]);
				if (currentSpacing < _minimumSliceSpacing)
				{
					error = SR.MessageInconsistentImageSpacing;
					return false;
				}

				if (!nominalSpacing.HasValue)
					nominalSpacing = currentSpacing;
				if (!FloatComparer.AreEqual(currentSpacing, nominalSpacing.Value, _sliceSpacingTolerance))
				{
					error = SR.MessageInconsistentImageSpacing;
					return false;
				}
			}

			// ensure frames are not tilted about unsupposed axis combinations (the gantry correction algorithm only supports rotations about X)
			if (!IsSupportedGantryTilt(frames)) // suffices to check first one... they're all co-planar now!!
			{
				error = SR.MessageUnsupportedGantryTilt;
				return false;
			}

			error = null;
			return true;
		}

		public bool CanFuse(Series baseSeries, Series petSeries)
		{
			if (!IsValidBaseSeries(baseSeries))
				return false;
			if (!IsValidPETFusionSeries(petSeries))
				return false;

			var baseFrames = GetFrames(baseSeries.Sops);
			var petFrames = GetFrames(petSeries.Sops);
			if (baseFrames[0].StudyInstanceUid != petFrames[0].StudyInstanceUid)
				return false;

			return true;
		}

		private IEnumerable<Series> FindFuseableBaseSeries(Series petSeries)
		{
			var petStudy = petSeries.ParentStudy;
			if (petStudy == null)
				yield break;

			foreach (var series in petStudy.Series)
			{
				if (CanFuse(series, petSeries))
					yield return series;
			}
		}

		private static bool AssertSameSeriesFrameOfReference(IList<Frame> frames)
		{
			// ensure all frames have are from the same series, and have the same frame of reference
			string studyInstanceUid = frames[0].StudyInstanceUid;
			string seriesInstanceUid = frames[0].SeriesInstanceUid;
			string frameOfReferenceUid = frames[0].FrameOfReferenceUid;
			foreach (Frame frame in frames)
			{
				if (frame.StudyInstanceUid != studyInstanceUid)
					return false;
				if (frame.SeriesInstanceUid != seriesInstanceUid)
					return false;
				if (frame.FrameOfReferenceUid != frameOfReferenceUid)
					return false;
			}
			return true;
		}

		private static List<Frame> GetFrames(IEnumerable<Sop> sops)
		{
			List<Frame> list = new List<Frame>();
			foreach (var sop in sops)
				if (sop is ImageSop)
					list.AddRange(((ImageSop) sop).Frames);
			return list;
		}

		private static float CalcSpaceBetweenPlanes(Frame frame1, Frame frame2)
		{
			Vector3D point1 = frame1.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			Vector3D point2 = frame2.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			Vector3D delta = point1 - point2;

			return delta.IsNull ? 0f : delta.Magnitude;
		}

		private static bool IsSupportedGantryTilt(IList<Frame> frames)
		{
			try
			{
				// neither of these should return null since we already checked for image orientation and position (patient)
				var firstImagePlane = frames[0].ImagePlaneHelper;
				var lastImagePlane = frames[frames.Count - 1].ImagePlaneHelper;

				Vector3D stackZ = lastImagePlane.ImageTopLeftPatient - firstImagePlane.ImageTopLeftPatient;
				Vector3D imageX = firstImagePlane.ImageTopRightPatient - firstImagePlane.ImageTopLeftPatient;

				if (!stackZ.IsOrthogonalTo(imageX, _gantryTiltTolerance))
				{
					// this is a gantry slew (gantry tilt about Y axis)
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Unexpected exception encountered while checking for supported gantry tilts");
				return false;
			}
		}
	}
}