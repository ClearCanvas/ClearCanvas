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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Factory class to create the individual graphics components of a DICOM image for presentation.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	internal class DicomGraphicsFactory
	{
		#region DICOM Overlays (and Bitmap Shutters)

		public static List<OverlayPlaneGraphic> CreateOverlayPlaneGraphics(Frame frame)
		{
			return CreateOverlayPlaneGraphics(frame, null);
		}

		public static List<OverlayPlaneGraphic> CreateOverlayPlaneGraphics(Frame frame, OverlayPlaneModuleIod overlaysFromPresentationState)
		{
			ISopDataSource dataSource = frame.ParentImageSop.DataSource;
			OverlayPlaneModuleIod overlaysIod = new OverlayPlaneModuleIod(dataSource);

			List<OverlayPlaneGraphic> overlayPlaneGraphics = new List<OverlayPlaneGraphic>();

			bool failedOverlays = false;

			foreach (var overlayPlane in overlaysIod)
			{
				// DICOM 2009 PS 3.3 Section C.9.3.1.1 specifies the rule: NumberOfFramesInOverlay+ImageFrameOrigin-1 must be <= NumberOfFrames
				if (!overlayPlane.IsValidMultiFrameOverlay(frame.ParentImageSop.NumberOfFrames))
				{
					failedOverlays = true;
					Platform.Log(LogLevel.Warn, new DicomOverlayDeserializationException(overlayPlane.Group, OverlayPlaneSource.Image), "Encoding error encountered while reading overlay from image headers.");
					continue;
				}

				try
				{
					byte[] overlayData = dataSource.GetFrameData(frame.FrameNumber).GetNormalizedOverlayData(overlayPlane.Index + 1);
					overlayPlaneGraphics.Add(new OverlayPlaneGraphic(overlayPlane, overlayData, OverlayPlaneSource.Image));

					// if overlay data is null, the data source failed to retrieve the overlay data for some reason, so we also treat it as an encoding error
					// this is different from if the overlay data is zero-length, which indicates that the retrieval succeeded, but that the overlay data for the frame is empty
					if (overlayData == null)
						throw new NullReferenceException();
				}
				catch (Exception ex)
				{
					failedOverlays = true;
					Platform.Log(LogLevel.Warn, new DicomOverlayDeserializationException(overlayPlane.Group, OverlayPlaneSource.Image, ex), "Failed to load overlay from the image header.");
				}
			}

			if (overlaysFromPresentationState != null)
			{
				foreach (var overlayPlane in overlaysFromPresentationState)
				{
					// if overlay data is missing, treat as an encoding error
					if (!overlayPlane.HasOverlayData)
					{
						failedOverlays = true;
						Platform.Log(LogLevel.Warn, new DicomOverlayDeserializationException(overlayPlane.Group, OverlayPlaneSource.PresentationState), "Encoding error encountered while reading overlay from softcopy presentation state.");
						continue;
					}

					try
					{
						byte[] overlayData;

						// try to compute the offset in the OverlayData bit stream where we can find the overlay frame that applies to this image frame
						int overlayFrame, bitOffset;
						if (overlayPlane.TryGetRelevantOverlayFrame(frame.FrameNumber, frame.ParentImageSop.NumberOfFrames, out overlayFrame) &&
						    overlayPlane.TryComputeOverlayDataBitOffset(overlayFrame, out bitOffset))
						{
							// offset found - unpack only that overlay frame
							var od = new OverlayData(bitOffset,
							                         overlayPlane.OverlayRows,
							                         overlayPlane.OverlayColumns,
							                         overlayPlane.IsBigEndianOW,
							                         overlayPlane.OverlayData);

							overlayData = od.Unpack();
						}
						else
						{
							// no relevant overlay frame found - i.e. the overlay for this image frame is blank
							overlayData = new byte[0];
						}

						overlayPlaneGraphics.Add(new OverlayPlaneGraphic(overlayPlane, overlayData, OverlayPlaneSource.PresentationState));
					}
					catch (Exception ex)
					{
						failedOverlays = true;
						Platform.Log(LogLevel.Warn, new DicomOverlayDeserializationException(overlayPlane.Group, OverlayPlaneSource.PresentationState, ex), "Failed to load overlay from softcopy presentation state.");
					}
				}
			}

			if (failedOverlays)
			{
				// add an error graphic if any overlays are not being displayed due to deserialization errors.
				overlayPlaneGraphics.Add(new ErrorOverlayPlaneGraphic(SR.MessageErrorDisplayingOverlays));
			}

			return overlayPlaneGraphics;
		}

		[Cloneable]
		private class ErrorOverlayPlaneGraphic : OverlayPlaneGraphic
		{
			public ErrorOverlayPlaneGraphic(string errorMessage)
				: base(10, 10)
			{
				errorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage : string.Empty;
				Graphics.Add(new ErrorText {Color = System.Drawing.Color.WhiteSmoke, Text = errorMessage});
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private ErrorOverlayPlaneGraphic(OverlayPlaneGraphic source, ICloningContext context)
				: base(source, context) {}

			[Cloneable(true)]
			private class ErrorText : InvariantTextPrimitive
			{
				public override void OnDrawing()
				{
					// upon drawing, re-centre the text
					var bounds = base.ParentPresentationImage.ClientRectangle;
					var anchor = new PointF(bounds.Left + bounds.Width/2, bounds.Top + bounds.Height/2);

					CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						Location = anchor;
					}
					finally
					{
						ResetCoordinateSystem();
					}

					base.OnDrawing();
				}
			}
		}

		#endregion

		#region DICOM Shutters (Geometric)

		public static GeometricShuttersGraphic CreateGeometricShuttersGraphic(Frame frame)
		{
			DisplayShutterMacroIod shutterIod = new DisplayShutterMacroIod(frame);
			return CreateGeometricShuttersGraphic(shutterIod, frame.Rows, frame.Columns);
		}

		public static GeometricShuttersGraphic CreateGeometricShuttersGraphic(DisplayShutterMacroIod shutterModule, int imageRows, int imageColumns)
		{
			GeometricShuttersGraphic shuttersGraphic = new GeometricShuttersGraphic(imageRows, imageColumns);
			if ((shutterModule.ShutterShape & ShutterShape.Circular) == ShutterShape.Circular)
			{
				Point? center = shutterModule.CenterOfCircularShutter;
				int? radius = shutterModule.RadiusOfCircularShutter;
				if (center != null && radius != null)
					shuttersGraphic.AddDicomShutter(new CircularShutter(center.Value, radius.Value));
			}

			if ((shutterModule.ShutterShape & ShutterShape.Rectangular) == ShutterShape.Rectangular)
			{
				int? left = shutterModule.ShutterLeftVerticalEdge;
				int? right = shutterModule.ShutterRightVerticalEdge;
				int? top = shutterModule.ShutterUpperHorizontalEdge;
				int? bottom = shutterModule.ShutterLowerHorizontalEdge;

				if (left != null && right != null && top != null && bottom != null)
					shuttersGraphic.AddDicomShutter(new RectangularShutter(left.Value, right.Value, top.Value, bottom.Value));
			}

			if ((shutterModule.ShutterShape & ShutterShape.Polygonal) == ShutterShape.Polygonal)
			{
				Point[] points = shutterModule.VerticesOfThePolygonalShutter;
				shuttersGraphic.AddDicomShutter(new PolygonalShutter(points));
			}

			return shuttersGraphic;
		}

		#endregion

		#region DICOM Graphic Annotations

		public static IEnumerable<DicomGraphicAnnotation> CreateGraphicAnnotations(Frame frame, GraphicAnnotationModuleIod annotationsFromPresentationState, RectangleF displayedArea, bool interactive = false)
		{
			GraphicAnnotationSequenceItem[] annotationSequences = annotationsFromPresentationState.GraphicAnnotationSequence;
			if (annotationSequences == null) return Enumerable.Empty<DicomGraphicAnnotation>().ToList();
			return annotationSequences.Select(sqItem => CreateGraphicAnnotation(frame, sqItem, displayedArea, interactive)).Where(g => g != null).ToList();
		}

		public static DicomGraphicAnnotation CreateGraphicAnnotation(Frame frame, GraphicAnnotationSequenceItem sequenceItem, RectangleF displayedArea, bool interactive = false)
		{
			ImageSopInstanceReferenceDictionary dictionary = new ImageSopInstanceReferenceDictionary(sequenceItem.ReferencedImageSequence, true);
			return dictionary.ReferencesFrame(frame.ParentImageSop.SopInstanceUid, frame.FrameNumber) ? DicomGraphicAnnotation.Create(sequenceItem, displayedArea) : null;
		}

		#endregion
	}
}