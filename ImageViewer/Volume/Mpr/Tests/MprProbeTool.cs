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

#if UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	[MenuAction("activate", "global-menus/MenuDebug/MenuMpr/MPR Probe", "Select", Flags = ClickActionFlags.CheckAction)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[GroupHint("activate", "Tools.Volume.MPR.Reslicing")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint), Enabled = false)]
	public class MprProbeTool : MouseImageViewerTool
	{
		private Tile _selectedTile;
		private ImageGraphic _selectedImageGraphic;
		private ImageSop _selectedImageSop;

		public MprProbeTool() : base(SR.TooltipProbe)
		{
			this.CursorToken = new CursorToken(CursorToken.SystemCursors.Cross);
			Behaviour |= MouseButtonHandlerBehaviour.ConstrainToTile;
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.SelectedImageGraphicProvider == null)
				return false;

			_selectedTile = mouseInformation.Tile as Tile;
			_selectedTile.InformationBox = new InformationBox();
			_selectedImageGraphic = this.SelectedImageGraphicProvider.ImageGraphic;
			_selectedImageSop = (this.SelectedPresentationImage as IImageSopProvider).ImageSop;

			Probe(mouseInformation.Location);

			return true;
		}

		/// <summary>
		/// Called by the framework as the mouse moves while the assigned mouse button
		/// is pressed.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_selectedTile == null || _selectedImageGraphic == null)
				return false;

			Probe(mouseInformation.Location);

			return true;
		}

		/// <summary>
		/// Called by the framework when the assigned mouse button is released.
		/// </summary>
		/// <param name="e">Mouse event args</param>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();
			return false;
		}

		public override void Cancel()
		{
			if (_selectedTile == null || _selectedImageGraphic == null)
				return;

			_selectedImageGraphic = null;

			_selectedTile.InformationBox.Visible = false;
			_selectedTile.InformationBox = null;
			_selectedTile = null;
		}

		private void Probe(Point destinationPoint)
		{
			Point sourcePointRounded = Point.Truncate(_selectedImageGraphic.SpatialTransform.ConvertToSource(destinationPoint));

			bool showPixelValue = true;
			bool showModalityValue = true;
			bool showVoiValue = true;

			string probeString = String.Format(SR.FormatLocation, SR.LabelNotApplicable, SR.LabelNotApplicable, SR.LabelNotApplicable);
			string pixelValueString = String.Format(SR.FormatPixelValue, SR.LabelNotApplicable);
			string modalityLutString = String.Format(SR.FormatModalityLutValue, SR.LabelNotApplicable);
			string voiLutString = String.Format(SR.FormatVoiLutValue, SR.LabelNotApplicable);

			try
			{
				DicomImagePlane dip = DicomImagePlane.FromImage(this.SelectedPresentationImage);
				if (_selectedImageGraphic.HitTest(destinationPoint))
				{
					Vector3D voxelCoordinate = dip.ConvertToPatient(new PointF(sourcePointRounded.X, sourcePointRounded.Y));
					probeString = String.Format(SR.FormatLocation, voxelCoordinate.X, voxelCoordinate.Y, voxelCoordinate.Z);

					if (_selectedImageGraphic is GrayscaleImageGraphic)
					{
						GrayscaleImageGraphic image = _selectedImageGraphic as GrayscaleImageGraphic;

						int pixelValue = 0;
						double modalityLutValue = 0;
						double voiLutValue = 0;

						GetPixelValue(image, sourcePointRounded, ref pixelValue, ref pixelValueString);
						GetModalityLutValue(image, pixelValue, ref modalityLutValue, ref modalityLutString);
						GetVoiLutValue(image, modalityLutValue, ref voiLutValue, ref voiLutString);
					}
					else if (_selectedImageGraphic is ColorImageGraphic)
					{
						showModalityValue = false;
						showVoiValue = false;

						ColorImageGraphic image = _selectedImageGraphic as ColorImageGraphic;
						Color color = image.PixelData.GetPixelAsColor(sourcePointRounded.X, sourcePointRounded.Y);
						string rgbFormatted = String.Format(SR.FormatRgb, color.R, color.G, color.B);
						pixelValueString = String.Format(SR.FormatPixelValue, rgbFormatted);
					}
					else
					{
						showPixelValue = false;
						showModalityValue = false;
						showVoiValue = false;
					}
				}

				if (showPixelValue)
					probeString += "\n" + pixelValueString;
				if (showModalityValue)
					probeString += "\n" + modalityLutString;
				if (showVoiValue)
					probeString += "\n" + voiLutString;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				probeString = SR.MessageProbeToolError;
			}

			_selectedTile.InformationBox.Update(probeString, destinationPoint);
		}

		private void GetPixelValue(
			GrayscaleImageGraphic grayscaleImage,
			Point sourcePointRounded,
			ref int pixelValue,
			ref string pixelValueString)
		{
			pixelValue = grayscaleImage.PixelData.GetPixel(sourcePointRounded.X, sourcePointRounded.Y);
			pixelValueString = String.Format(SR.FormatPixelValue, pixelValue);
		}

		private void GetModalityLutValue(
			GrayscaleImageGraphic grayscaleImage,
			int pixelValue,
			ref double modalityLutValue,
			ref string modalityLutString)
		{
			if (grayscaleImage.ModalityLut != null)
			{
				modalityLutValue = grayscaleImage.ModalityLut[pixelValue];
				modalityLutString = String.Format(SR.FormatModalityLutValue, modalityLutValue);

				if (_selectedImageSop != null)
				{
					if (String.Compare(_selectedImageSop.Modality, "CT", true) == 0)
						modalityLutString += String.Format(" ({0})", SR.LabelHounsfieldUnitsAbbreviation);
				}
			}
		}

		private void GetVoiLutValue(
			GrayscaleImageGraphic grayscaleImage,
			double modalityLutValue,
			ref double voiLutValue,
			ref string voiLutString)
		{
			if (grayscaleImage.VoiLut != null)
			{
				voiLutValue = grayscaleImage.VoiLut[modalityLutValue];
				voiLutString = String.Format(SR.FormatVoiLutValue, voiLutValue);
			}
		}

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming
		private static class SR
		{
			public static readonly string MessageProbeToolError = "Error";
			public static readonly string TooltipProbe = "Spatial Probe";
			public static readonly string FormatRgb = "{0:x2}{1:x2}{2:x2}";
			public static readonly string FormatLocation = "LOC: x={0}, y={1}, z={2}";
			public static readonly string FormatVoiLutValue = "VOI: {0}";
			public static readonly string FormatPixelValue = "VOX: {0}";
			public static readonly string FormatModalityLutValue = "MOD: {0}";
			public static readonly string LabelHounsfieldUnitsAbbreviation = "HU";
			public static readonly string LabelNotApplicable = "n/a";
		}

// ReSharper restore InconsistentNaming
// ReSharper restore ConvertToConstant.Local
	}
}

#endif