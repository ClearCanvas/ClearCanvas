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

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	/// <summary>
	/// This class is not necessarily intended to be used.  It is slightly modified from
	/// the original (temporary) code that was included in 0.95 for the text overlay.  Now 
	/// it simply serves as an example of how a hard-coded overlay can still be used 
	/// in the new model.
	/// </summary>
	internal sealed class DicomFixedAnnotationLayoutFactory
	{
		private string[] _AssignmentsTopLeft = 
		{
			"Dicom.Patient.PatientId",
			"Dicom.Patient.PatientsName",
			"Dicom.Patient.PatientsBirthDate",
			"Dicom.Patient.PatientsSex",
			"Dicom.PatientStudy.PatientsAge",
			"", "", "", "", "",
			"", "", "", "", ""
		};

		private string[] _AssignmentsTopRight =
		{
			"Dicom.GeneralStudy.AccessionNumber",
			"Dicom.GeneralStudy.StudyDescription",
			"Dicom.GeneralStudy.StudyId",
			"Dicom.GeneralStudy.StudyDate",
			"Dicom.GeneralStudy.StudyTime",
			"", "", "", "", "",
			"", "", "", "", ""
		};

		private string[] _AssignmentsBottomLeft = 
		{
			"", "", "", "", 
			"", "", "", "", 
			"Dicom.GeneralSeries.Laterality",
			"Dicom.GeneralSeries.SeriesNumber",
			"Dicom.GeneralImage.InstanceNumber",
			"Presentation.Zoom",
			"Presentation.AppliedLut",
			"Dicom.GeneralSeries.ProtocolName",
			"Dicom.GeneralSeries.SeriesDescription"
		};

		private string[] _AssignmentsBottomRight = 
		{
			"", "", "", "",
			"", "", "", "", "",
			"", "",
			"Dicom.GeneralSeries.OperatorsName",
			"Dicom.GeneralEquipment.StationName",
			"Dicom.GeneralStudy.ReferringPhysiciansName",
			"Dicom.PatientStudy.AdditionalPatientsHistory"
		};

		public AnnotationLayout Create()
		{
			AnnotationLayout layout = new AnnotationLayout();

			int numberOfBoxesPerQuadrant = 15;
			float boxheight = 1 / 32.0F;

			float x = 0F, y = 0F, dx = 0.5F, dy = boxheight;

			AnnotationBox newBox;
			AnnotationBox defaultBoxSettings = new AnnotationBox();
			defaultBoxSettings.Bold = true;
			defaultBoxSettings.Font = "Century Gothic";
			//TL
			for (int i = 0; i < numberOfBoxesPerQuadrant; ++i)
			{
				dx = (i > 0) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				newBox = defaultBoxSettings.Clone();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = AnnotationLayoutFactory.GetAnnotationItem(_AssignmentsTopLeft[i]);

				layout.AnnotationBoxes.Add(newBox);
				y += boxheight;
			}

			defaultBoxSettings = new AnnotationBox();
			defaultBoxSettings.Color = "OrangeRed";
			defaultBoxSettings.Font = "Century Gothic";
			defaultBoxSettings.Justification = AnnotationBox.JustificationBehaviour.Right;
			y = 0.0F;
			//TR
			for (int i = 0; i < numberOfBoxesPerQuadrant; ++i)
			{
				x = (i > 0) ? 0.5F : 0.6F; //make room for directional markers.
				dx = (i > 0) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				newBox = defaultBoxSettings.Clone();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = AnnotationLayoutFactory.GetAnnotationItem(_AssignmentsTopRight[i]);

				layout.AnnotationBoxes.Add(newBox);
				y += boxheight;
			}

			defaultBoxSettings = new AnnotationBox();
			defaultBoxSettings.Color = "Cyan";
			defaultBoxSettings.Font = "Century Gothic";
			x = 0F;
			y = 1.0F - boxheight;
			//BL
			for (int i = numberOfBoxesPerQuadrant - 1; i >= 0; --i)
			{
				dx = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				newBox = defaultBoxSettings.Clone();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = AnnotationLayoutFactory.GetAnnotationItem(_AssignmentsBottomLeft[i]);

				if (i < numberOfBoxesPerQuadrant - 4 && !String.IsNullOrEmpty(_AssignmentsBottomLeft[i]))
				{
					newBox.ConfigurationOptions = new AnnotationItemConfigurationOptions();
					newBox.ConfigurationOptions.ShowLabel = true;
				}

				layout.AnnotationBoxes.Add(newBox);
				y -= boxheight;
			}

			defaultBoxSettings = new AnnotationBox();
			defaultBoxSettings.Color = "Yellow";
			defaultBoxSettings.Font = "Century Gothic";
			defaultBoxSettings.NumberOfLines = 2;
			defaultBoxSettings.Justification = AnnotationBox.JustificationBehaviour.Right;

			y = 1.0F - boxheight;
			//BR
			for (int i = numberOfBoxesPerQuadrant - 1; i >= 0; --i)
			{
				x = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.6F; //make room for directional markers.
				dx = (i < (numberOfBoxesPerQuadrant - 1)) ? 0.5F : 0.4F; //make room for directional markers.

				RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
				newBox = defaultBoxSettings.Clone();
				newBox.NormalizedRectangle = normalizedRectangle;
				newBox.AnnotationItem = AnnotationLayoutFactory.GetAnnotationItem(_AssignmentsBottomRight[i]);
				if (!String.IsNullOrEmpty(_AssignmentsBottomRight[i]))
					newBox.NumberOfLines = 2;

				layout.AnnotationBoxes.Add(newBox);
				y -= boxheight;
			}

			defaultBoxSettings = new AnnotationBox();
			defaultBoxSettings.Color = "White";
			defaultBoxSettings.Bold = true;
			defaultBoxSettings.Font = "Century Gothic";
			defaultBoxSettings.NumberOfLines = 1;

			newBox = defaultBoxSettings.Clone();
			CreateDirectionalMarkerBox(0.00F, (1F - boxheight) / 2F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Left, "Presentation.DirectionalMarkers.Left", newBox);
			layout.AnnotationBoxes.Add(newBox);

			newBox = defaultBoxSettings.Clone();
			CreateDirectionalMarkerBox(0.90F, (1F - boxheight) / 2F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Right, "Presentation.DirectionalMarkers.Right", newBox);
			layout.AnnotationBoxes.Add(newBox);

			newBox = defaultBoxSettings.Clone();
			CreateDirectionalMarkerBox(0.45F, 0F, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Center, "Presentation.DirectionalMarkers.Top", newBox);
			layout.AnnotationBoxes.Add(newBox);

			newBox = defaultBoxSettings.Clone();
			CreateDirectionalMarkerBox(0.45F, 1F - boxheight, 0.1F, boxheight, AnnotationBox.JustificationBehaviour.Center, "Presentation.DirectionalMarkers.Bottom", newBox);
			layout.AnnotationBoxes.Add(newBox);

			return layout;
		}

		private static void CreateDirectionalMarkerBox(float x, float y, float dx, float dy, AnnotationBox.JustificationBehaviour justification, string identifier, AnnotationBox newBox)
		{
			RectangleF normalizedRectangle = new RectangleF(x, y, dx, dy);
			newBox.NormalizedRectangle = normalizedRectangle;
			newBox.AnnotationItem = AnnotationLayoutFactory.GetAnnotationItem(identifier);
			newBox.Justification = justification;
		}
	}
}
