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
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	/// <summary>
	/// Calculates the directional marker for a particular given edge of a viewport.  The algorithm works based
	/// on the fact that the source and destination (image & viewport) edges coincide exactly when there is no 
	/// transform applied (e.g. Left Edge corresponds to Left Edge, etc).  So, when a 2D transform is applied
	/// to a Dicom image (90 degree rotations only!) the marker at a particular edge will move to a different edge.
	/// For example, if the marker at the Left edge of the viewport is "Anterior" when there is no transformation
	/// applied, a horizontal flip will cause the "Anterior" marker to move to the Right Edge of the viewport.
	/// 
	/// The algorithm used here is quite simple:
	/// - 4 vectors, pointing to the 4 image edges (in source coordinates) are transformed to destination
	///   (viewport) coordinates.
	/// - Once in destination coordinates, the (image) edge that has now effectively moved to the viewport edge
	///   represented by this object is determined and its marker becomes the new marker for this viewport edge. 
	/// </summary>
	internal sealed class DirectionalMarkerAnnotationItem : AnnotationItem
	{
		public enum ImageEdge
		{
			Left = 0,
			Top = 1,
			Right = 2,
			Bottom = 3
		} ;

		private static readonly SizeF[] _edgeVectors = new SizeF[] {new SizeF(-1, 0), new SizeF(0, -1), new SizeF(1, 0), new SizeF(0, 1)};

		private ImageEdge _viewportEdge;

		public DirectionalMarkerAnnotationItem(ImageEdge viewportEdge)
			: base("Presentation.DirectionalMarkers." + viewportEdge.ToString(), new AnnotationResourceResolver(typeof (DirectionalMarkerAnnotationItem).Assembly))
		{
			_viewportEdge = viewportEdge;
		}

		/// <summary>
		/// Gets the annotation text.
		/// </summary>
		/// <param name="presentationImage">the input presentation image.</param>
		/// <returns>the annotation text.</returns>
		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			string markerText = "";

			if (presentationImage != null)
			{
				ISpatialTransformProvider associatedTransform = presentationImage as ISpatialTransformProvider;
				IImageSopProvider associatedDicom = presentationImage as IImageSopProvider;

				if (associatedDicom != null && associatedTransform != null)
				{
					var spatialTransform = associatedTransform.SpatialTransform as ISpatialTransform;
					if (spatialTransform != null)
					{
						var imageOrientationPatient = associatedDicom.Frame.ImageOrientationPatient;
						var patientOrientation = associatedDicom.Frame.PatientOrientation;

						if (imageOrientationPatient != null && !imageOrientationPatient.IsNull)
							markerText = GetAnnotationTextInternal(spatialTransform, imageOrientationPatient);
						else if (patientOrientation != null && patientOrientation.IsValid)
							markerText = GetAnnotationTextInternal(spatialTransform, patientOrientation);
					}
				}
			}

			return markerText;
		}

		/// <summary>
		/// Called by GetAnnotationText (and also by Unit Test code).  Making this function internal simply makes it easier
		/// to write unit tests for this class (don't have to implement a fake PresentationImage).
		/// </summary>
		/// <param name="imageTransform">the image transform</param>
		/// <param name="imageOrientationPatient">the image orientation patient (direction cosines)</param>
		/// <returns></returns>
		internal string GetAnnotationTextInternal(ISpatialTransform imageTransform, ImageOrientationPatient imageOrientationPatient)
		{
			SizeF[] imageEdgeVectors = new SizeF[4];
			for (int i = 0; i < 4; ++i)
				imageEdgeVectors[i] = imageTransform.ConvertToDestination(_edgeVectors[i]);

			//find out which source image edge got transformed to coincide with this viewport edge.
			ImageEdge transformedEdge = GetTransformedEdge(imageEdgeVectors);

			//get the marker for the appropriate (source) image edge.
			return GetMarker(transformedEdge, imageOrientationPatient);
		}

		/// <summary>
		/// Called by GetAnnotationText (and also by Unit Test code).  Making this function internal simply makes it easier
		/// to write unit tests for this class (don't have to implement a fake PresentationImage).
		/// </summary>
		/// <param name="imageTransform">the image transform</param>
		/// <param name="patientOrientation">the image orientation patient (direction cosines)</param>
		/// <returns></returns>
		internal string GetAnnotationTextInternal(ISpatialTransform imageTransform, PatientOrientation patientOrientation)
		{
			SizeF[] imageEdgeVectors = new SizeF[4];
			for (int i = 0; i < 4; ++i)
				imageEdgeVectors[i] = imageTransform.ConvertToDestination(_edgeVectors[i]);

			//find out which source image edge got transformed to coincide with this viewport edge.
			ImageEdge transformedEdge = GetTransformedEdge(imageEdgeVectors);

			//get the marker for the appropriate (source) image edge.
			return GetMarker(transformedEdge, patientOrientation);
		}

		/// <summary>
		/// After undergoing the transformation, the input vectors are now in Destination (viewport) coordinates.
		/// This function determines which edge in the image has moved to this edge of the viewport.
		/// That edge's original marker (when there is no transform), becomes this edge's marker.
		/// </summary>
		/// <param name="transformedVectors">source vectors transformed to the destination coordinate system</param>
		/// <returns>the source image edge that has effectively moved to this edge</returns>
		private ImageEdge GetTransformedEdge(SizeF[] transformedVectors)
		{
			//the original (untransformed) vector for this viewport edge.
			SizeF thisViewportEdge = _edgeVectors[(int) _viewportEdge];

			//find out which edge in the source image has moved to this edge of the viewport.
			for (int index = 0; index < transformedVectors.Length; ++index)
			{
				//normalize the vector before comparing.
				SizeF transformedVector = transformedVectors[index];
				double magnitude = Math.Sqrt(transformedVector.Width*transformedVector.Width +
				                             transformedVector.Height*transformedVector.Height);

				transformedVector.Width = (float) Math.Round(transformedVector.Width/magnitude);
				transformedVector.Height = (float) Math.Round(transformedVector.Height/magnitude);

				//is it the same as the original vector for this edge?
				if (transformedVector == thisViewportEdge)
				{
					//return the image edge that has now moved to this edge of the viewport.
					return (ImageEdge) index;
				}
			}

			//this should never happen.
            throw new IndexOutOfRangeException(SR.ExceptionTransformedEdgeDoesNotMatch);
		}

		/// <summary>
		/// Determines the (untransformed) marker for a particular image edge.
		/// </summary>
		/// <param name="imageEdge">the edge (image coordinates)</param>
		/// <param name="imageOrientationPatient">the direction cosines of the image</param>
		/// <returns>a string representation of the direction (a 'marker')</returns>
		private string GetMarker(ImageEdge imageEdge, ImageOrientationPatient imageOrientationPatient)
		{
			bool negativeDirection = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Top);
			bool rowValues = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Right);

			string markerText = "";

			if (rowValues)
			{
				ImageOrientationPatient.Directions primary = imageOrientationPatient.GetPrimaryRowDirection(negativeDirection);
				ImageOrientationPatient.Directions secondary = imageOrientationPatient.GetSecondaryRowDirection(negativeDirection, 1);
				markerText += GetMarkerText(primary);
				markerText += GetMarkerText(secondary);
			}
			else
			{
				ImageOrientationPatient.Directions primary = imageOrientationPatient.GetPrimaryColumnDirection(negativeDirection);
				ImageOrientationPatient.Directions secondary = imageOrientationPatient.GetSecondaryColumnDirection(negativeDirection, 1);
				markerText += GetMarkerText(primary);
				markerText += GetMarkerText(secondary);
			}

			return markerText;
		}

		/// <summary>
		/// Determines the (untransformed) marker for a particular image edge.
		/// </summary>
		/// <param name="imageEdge">the edge (image coordinates)</param>
		/// <param name="patientOrientation">the patient orientation construct of the image</param>
		/// <returns>a string representation of the direction (a 'marker')</returns>
		private string GetMarker(ImageEdge imageEdge, PatientOrientation patientOrientation)
		{
			bool negativeDirection = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Top);
			bool rowValues = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Right);

			var direction = (rowValues ? patientOrientation.Row : patientOrientation.Column) ?? PatientDirection.Empty;
			if (negativeDirection)
				direction = direction.OpposingDirection;

			string markerText = "";
			markerText += GetMarkerText(direction.Primary);
			markerText += GetMarkerText(direction.Secondary);

			return markerText;
		}

		/// <summary>
		/// Converts an <see cref="PatientDirection"/> to a marker string.
		/// </summary>
		/// <param name="direction">the direction (patient based system)</param>
		/// <returns>marker text</returns>
		private static string GetMarkerText(PatientDirection direction)
		{
		    // TODO (CR Mar 2012): Add a "short description" to PatientDirection class and return this from there.
            // Then we're not finding patient direction resources all over the place.

			if (direction == PatientDirection.QuadrupedLeft)
				return SR.ValueDirectionalMarkersQuadrupedLeft;
			else if (direction == PatientDirection.QuadrupedRight)
				return SR.ValueDirectionalMarkersQuadrupedRight;
			else if (direction == PatientDirection.QuadrupedCranial)
				return SR.ValueDirectionalMarkersQuadrupedCranial;
			else if (direction == PatientDirection.QuadrupedCaudal)
				return SR.ValueDirectionalMarkersQuadrupedCaudal;
			else if (direction == PatientDirection.QuadrupedRostral)
				return SR.ValueDirectionalMarkersQuadrupedRostral;
			else if (direction == PatientDirection.QuadrupedDorsal)
				return SR.ValueDirectionalMarkersQuadrupedDorsal;
			else if (direction == PatientDirection.QuadrupedVentral)
				return SR.ValueDirectionalMarkersQuadrupedVentral;
			else if (direction == PatientDirection.QuadrupedLateral)
				return SR.ValueDirectionalMarkersQuadrupedLateral;
			else if (direction == PatientDirection.QuadrupedMedial)
				return SR.ValueDirectionalMarkersQuadrupedMedial;
			else if (direction == PatientDirection.QuadrupedProximal)
				return SR.ValueDirectionalMarkersQuadrupedProximal;
			else if (direction == PatientDirection.QuadrupedDistal)
				return SR.ValueDirectionalMarkersQuadrupedDistal;
			else if (direction == PatientDirection.QuadrupedPalmar)
				return SR.ValueDirectionalMarkersQuadrupedPalmar;
			else if (direction == PatientDirection.QuadrupedPlantar)
				return SR.ValueDirectionalMarkersQuadrupedPlantar;
			else if (direction == PatientDirection.Left)
				return SR.ValueDirectionalMarkersLeft;
			else if (direction == PatientDirection.Right)
				return SR.ValueDirectionalMarkersRight;
			else if (direction == PatientDirection.Head)
				return SR.ValueDirectionalMarkersHead;
			else if (direction == PatientDirection.Foot)
				return SR.ValueDirectionalMarkersFoot;
			else if (direction == PatientDirection.Anterior)
				return SR.ValueDirectionalMarkersAnterior;
			else if (direction == PatientDirection.Posterior)
				return SR.ValueDirectionalMarkersPosterior;
			return string.Empty;
		}
	}
}