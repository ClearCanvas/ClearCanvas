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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    public class PatientOrientationHelper
    {
        private readonly ISpatialTransform _imageTransform;
        private readonly ImageOrientationPatient _imageOrientationPatient;
        private readonly PatientOrientation _patientOrientation;

        public enum ImageEdge { Left = 0, Top = 1, Right = 2, Bottom = 3 };
        private static readonly SizeF[] _edgeVectors = new [] { new SizeF(-1, 0), new SizeF(0, -1), new SizeF(1, 0), new SizeF(0, 1) };

        public PatientOrientationHelper(ISpatialTransform imageTransform, ImageOrientationPatient imageOrientationPatient)
        {
            Platform.CheckForNullReference(imageTransform, "imageTransform");
            Platform.CheckForNullReference(imageOrientationPatient, "imageOrientationPatient");

            _imageTransform = imageTransform;
            _imageOrientationPatient = imageOrientationPatient;
            AngleTolerance = 1;
        }

        public PatientOrientationHelper(ISpatialTransform imageTransform, PatientOrientation patientOrientation)
        {
            Platform.CheckForNullReference(imageTransform, "imageTransform");
            Platform.CheckForNullReference(patientOrientation, "patientOrientation");

            _imageTransform = imageTransform;
            _patientOrientation = patientOrientation;
            AngleTolerance = 1;
        }

        /// <summary>
        /// Angle tolerance for the 
        /// </summary>
        private double AngleTolerance { get; set; }

        public PatientOrientation GetEffectiveOrientation()
        {
            return new PatientOrientation(GetEdgeDirection(ImageEdge.Right), GetEdgeDirection(ImageEdge.Bottom));
        }

        public PatientDirection GetEdgeDirection(ImageEdge viewportEdge)
        {
            var direction = GetPrimaryEdgeDirection(viewportEdge);
            if (!direction.IsEmpty)
            {
                direction += GetSecondaryEdgeDirection(viewportEdge);
                //TODO (CR June 2011): Tertiary?
            }

            return direction;
        }

        public PatientDirection GetPrimaryEdgeDirection(ImageEdge viewportEdge)
        {
            var imageEdgeVectors = GetImageEdgeVectors();
            //find out which source image edge got transformed to coincide with this viewport edge.
            var transformedEdge = GetTransformedEdge(imageEdgeVectors, viewportEdge);
            return GetEdgeDirection(transformedEdge, PatientDirection.Component.Primary);
        }

        public PatientDirection GetSecondaryEdgeDirection(ImageEdge viewportEdge)
        {
            var imageEdgeVectors = GetImageEdgeVectors();
            //find out which source image edge got transformed to coincide with this viewport edge.
            var transformedEdge = GetTransformedEdge(imageEdgeVectors, viewportEdge);
            return GetEdgeDirection(transformedEdge, PatientDirection.Component.Secondary);
        }

        private PatientDirection GetEdgeDirection(ImageEdge imageEdge, PatientDirection.Component component)
		{
			bool negativeDirection = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Top);
			bool rowValues = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Right);

            var direction = PatientDirection.Empty;
            if (rowValues)
			{
                //TODO (CR June 2011): tertiary?
                if (component == PatientDirection.Component.Primary)
                    direction = GetPrimaryRowDirection();
                else if (component == PatientDirection.Component.Secondary)
                    direction = GetSecondaryRowDirection();
			}
			else
			{
                //TODO (CR June 2011): tertiary?
                if (component == PatientDirection.Component.Primary)
                    direction = GetPrimaryColumnDirection();
                else if (component == PatientDirection.Component.Secondary)
                    direction = GetSecondaryColumnDirection();
            }

            return negativeDirection ? direction.OpposingDirection : direction;
		}

        private PatientDirection GetPrimaryRowDirection()
        {
            return _imageOrientationPatient != null ? _imageOrientationPatient.PrimaryRow : _patientOrientation.PrimaryRow;
        }

        private PatientDirection GetPrimaryColumnDirection()
        {
            return _imageOrientationPatient != null ? _imageOrientationPatient.PrimaryColumn : _patientOrientation.PrimaryColumn;
        }

        private PatientDirection GetSecondaryRowDirection()
        {
            return _imageOrientationPatient != null 
                ? _imageOrientationPatient.GetSecondaryRowDirection(false, AngleTolerance) : _patientOrientation.SecondaryRow;
        }

        private PatientDirection GetSecondaryColumnDirection()
        {
            return _imageOrientationPatient != null 
                ? _imageOrientationPatient.GetSecondaryColumnDirection(false, AngleTolerance): _patientOrientation.SecondaryColumn;
        }

        private SizeF[] GetImageEdgeVectors()
        {
            var imageEdgeVectors = new SizeF[4];
            for (int i = 0; i < 4; ++i)
                imageEdgeVectors[i] = _imageTransform.ConvertToDestination(_edgeVectors[i]);
            return imageEdgeVectors;
        }
        
        private static ImageEdge GetTransformedEdge(SizeF[] transformedVectors, ImageEdge viewportEdge)
        {
            //the original (untransformed) vector for this viewport edge.
            SizeF thisViewportEdge = _edgeVectors[(int)viewportEdge];

            //find out which edge in the source image has moved to this edge of the viewport.
            for (int index = 0; index < transformedVectors.Length; ++index)
            {
                //normalize the vector before comparing.
                SizeF transformedVector = transformedVectors[index];
                double magnitude = Math.Sqrt(transformedVector.Width * transformedVector.Width +
                                                transformedVector.Height * transformedVector.Height);

                transformedVector.Width = (float)Math.Round(transformedVector.Width / magnitude);
                transformedVector.Height = (float)Math.Round(transformedVector.Height / magnitude);

                //is it the same as the original vector for this edge?
                if (transformedVector == thisViewportEdge)
                {
                    //return the image edge that has now moved to this edge of the viewport.
                    return (ImageEdge)index;
                }
            }

            //this should never happen.
            throw new IndexOutOfRangeException("The transformed edge does not have a corresponding value.");
        }
    }
}
