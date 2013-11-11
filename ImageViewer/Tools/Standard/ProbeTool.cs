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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionPoint]
	public sealed class ProbeToolDropDownToolExtensionPoint : ExtensionPoint<ITool> {}

	[MenuAction("activate", "imageviewer-contextmenu/MenuProbe", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuProbe", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarProbe", "Select", "DropDownMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.B)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.ProbeToolSmall.png", "Icons.ProbeToolMedium.png", "Icons.ProbeToolLarge.png")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[GroupHint("activate", "Tools.Image.Inspection.Probe")]
	//
	[MouseToolButton(XMouseButtons.Left, false)]
	//
	[MenuAction("showRawPix", "probetool-dropdown/MenuShowRawPixelValue", "ToggleShowRawPix")]
	[CheckedStateObserver("showRawPix", "ShowRawPix", "ShowRawPixChanged")]
	[GroupHint("showRawPix", "Tools.Image.Inspection.Probe.Modality.CT.ShowPixel")]
	//
	[MenuAction("showVoiLut", "probetool-dropdown/MenuShowVoiPixelValue", "ToggleShowVoiLut")]
	[CheckedStateObserver("showVoiLut", "ShowVoiLut", "ShowVoiLutChanged")]
	[GroupHint("showVoiLut", "Tools.Image.Inspection.Probe.General.ShowVoiLut")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ProbeTool : MouseImageViewerTool, IProbe
	{
		private Tile _selectedTile;
		private ISpatialTransform _selectedSpatialTransform;
		private IPatientCoordinateMapping _selectedCoordinateMapping;
		private ImageGraphic _selectedImageGraphic;
		private Frame _selectedFrame;
		private ActionModelNode _actionModel;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public ProbeTool()
			: base(SR.TooltipProbe)
		{
			CursorToken = new CursorToken("ProbeCursor.png", this.GetType().Assembly);
			Behaviour |= MouseButtonHandlerBehaviour.ConstrainToTile;
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public ActionModelNode DropDownMenuModel
		{
			get
			{
				if (_actionModel == null)
				{
					_actionModel = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Tools.Standard", "probetool-dropdown", this.Actions);
				}

				return _actionModel;
			}
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (SelectedSpatialTransformProvider == null || SelectedImageSopProvider == null)
				return false;

			_selectedTile = mouseInformation.Tile as Tile;
			if (_selectedTile == null) return false; // if it's not Tile, then we can't do anything

			_selectedTile.InformationBox = new InformationBox();
			_selectedImageGraphic = SelectedImageGraphicProvider != null ? SelectedImageGraphicProvider.ImageGraphic : null;
			_selectedFrame = ((IImageSopProvider) SelectedPresentationImage).Frame;
			_selectedCoordinateMapping = SelectedPresentationImage is IPatientCoordinateMappingProvider ? ((IPatientCoordinateMappingProvider) SelectedPresentationImage).PatientCoordinateMapping : null;
			_selectedSpatialTransform = SelectedSpatialTransformProvider.SpatialTransform;

			Probe(mouseInformation.Location);

			return true;
		}

		/// <summary>
		/// Called by the framework as the mouse moves while the assigned mouse button
		/// is pressed.
		/// </summary>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_selectedTile == null || _selectedSpatialTransform == null)
				return false;

			Probe(mouseInformation.Location);

			return true;
		}

		/// <summary>
		/// Called by the framework when the assigned mouse button is released.
		/// </summary>
		/// <returns>True if the event was handled, false otherwise</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();
			return false;
		}

		public override void Cancel()
		{
			if (_selectedTile == null || _selectedSpatialTransform == null)
				return;

			_selectedImageGraphic = null;
			_selectedSpatialTransform = null;
			_selectedFrame = null;
			_selectedCoordinateMapping = null;
			_selectedTile.InformationBox.Visible = false;
			_selectedTile.InformationBox = null;
			_selectedTile = null;
		}

		private void Probe(Point destinationPoint)
		{
			Point sourcePointRounded = Point.Truncate(_selectedSpatialTransform.ConvertToSource(destinationPoint));

			ToolSettings settings = ToolSettings.DefaultInstance;
			bool showPixelValue = settings.ShowRawPixelValue;
			bool showVoiValue = settings.ShowVOIPixelValue;

			Probe(sourcePointRounded, showPixelValue, showVoiValue);
		}

		private void Probe(Point sourcePointRounded, bool showPixelValue, bool showVoiValue)
		{
			string probeString;
			string pixelValueString = String.Format(SR.FormatProbeInfo, SR.LabelRawPixel, SR.LabelNotApplicable);
			string modalityLutString = String.Format(SR.FormatProbeInfo, SR.LabelModalityLut, SR.LabelNotApplicable);
			string voiLutString = String.Format(SR.FormatProbeInfo, SR.LabelVOILut, SR.LabelNotApplicable);

			try
			{
				var displayString = new StringBuilder();

				if (_selectedImageGraphic != null && _selectedImageGraphic.BoundingBox.Contains(sourcePointRounded))
				{
					if (_selectedImageGraphic is ILutPipelineProvider)
					{
						var luts = _selectedImageGraphic as ILutPipelineProvider;
						var pixelValue = _selectedImageGraphic.PixelData.GetPixel(sourcePointRounded.X, sourcePointRounded.Y);

						GetPixelValue(luts, pixelValue, ref pixelValueString);
						GetModalityLutValue(luts, pixelValue, ref modalityLutString);
						GetVoiLutValue(luts, pixelValue, ref voiLutString);

						// the modality LUT value is always shown
						displayString.AppendLine(modalityLutString);

						if (showPixelValue) displayString.AppendLine(pixelValueString);
						if (showVoiValue) displayString.AppendLine(voiLutString);
					}
					else if (_selectedImageGraphic is ColorImageGraphic)
					{
						ColorImageGraphic image = _selectedImageGraphic as ColorImageGraphic;
						Color color = image.PixelData.GetPixelAsColor(sourcePointRounded.X, sourcePointRounded.Y);
						string rgbFormatted = String.Format(SR.FormatRGB, color.R, color.G, color.B);
						pixelValueString = String.Format(SR.FormatProbeInfo, SR.LabelRGBPixel, rgbFormatted);
						displayString.AppendLine(pixelValueString);
					}
				}

				// show the coordinates last, cause it's probably the least interesting information
				var coordinateString = String.Format(SR.FormatProbeInfo, SR.LabelLocation, string.Format(SR.FormatCoordinates, sourcePointRounded.X, sourcePointRounded.Y));
				displayString.AppendLine(coordinateString);

				var patientPoint = _selectedCoordinateMapping != null && _selectedCoordinateMapping.IsValid
				                   	? _selectedCoordinateMapping.ConvertToPatient(sourcePointRounded)
				                   	: _selectedFrame.ImagePlaneHelper.ConvertToPatient(sourcePointRounded);
				if (patientPoint != null)
				{
					var patientString = String.Format(SR.FormatProbeInfo, SR.LabelPatientLocation, string.Format(SR.FormatCoordinates3D, patientPoint.X.ToString("f3"), patientPoint.Y.ToString("f3"), patientPoint.Z.ToString("f3")));
					displayString.AppendLine(patientString);
				}

				probeString = displayString.ToString().Trim();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				probeString = SR.MessageProbeToolError;
			}

			var destinationPoint = Point.Round(_selectedSpatialTransform.ConvertToDestination(sourcePointRounded));
			_selectedTile.InformationBox.Update(probeString, destinationPoint);
		}

		private void GetPixelValue(
			ILutPipelineProvider luts,
			int pixelValue,
			ref string pixelValueString)
		{
			var value = luts.LookupPixelValue(pixelValue, LutPipelineStage.Source);
			pixelValueString = String.Format(SR.FormatProbeInfo, SR.LabelRawPixel, value);
		}

		private void GetModalityLutValue(
			ILutPipelineProvider luts,
			int pixelValue,
			ref string modalityLutString)
		{
			if (luts.ModalityLut != null)
			{
				var modalityLutValue = luts.LookupPixelValue(pixelValue, LutPipelineStage.Modality);

				var modalityLutValueDisplay = modalityLutValue.ToString(_selectedFrame != null && _selectedFrame.IsSubnormalRescale ? @"G3" : @"F1");
				if (_selectedFrame != null)
				{
					var units = (_selectedFrame.RescaleUnits ?? RescaleUnits.None).Label;
					if (!string.IsNullOrEmpty(units))
						modalityLutValueDisplay = string.Format(SR.FormatValueUnits, modalityLutValueDisplay, units);
				}

				modalityLutString = String.Format(SR.FormatProbeInfo, SR.LabelModalityLut, modalityLutValueDisplay);
			}
		}

		private void GetVoiLutValue(
			ILutPipelineProvider luts,
			int pixelValue,
			ref string voiLutString)
		{
			if (luts.VoiLut != null)
			{
				var voiLutValue = luts.LookupPixelValue(pixelValue, LutPipelineStage.Voi);
				voiLutString = String.Format(SR.FormatProbeInfo, SR.LabelVOILut, voiLutValue.ToString("F1"));
			}
		}

		#region Probe Tool Settings

		private event EventHandler _showRawPixChanged;
		private event EventHandler _showVoiLutChanged;
		private ToolSettings _settings;

		public override void Initialize()
		{
			base.Initialize();

			_settings = ToolSettings.DefaultInstance;
			_settings.PropertyChanged += OnPropertyChanged;
		}

		protected override void Dispose(bool disposing)
		{
			_settings.PropertyChanged -= OnPropertyChanged;
			_settings = null;

			base.Dispose(disposing);
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "ShowRawPixelValue":
					EventsHelper.Fire(_showRawPixChanged, this, EventArgs.Empty);
					break;
				case "ShowVOIPixelValue":
					EventsHelper.Fire(_showVoiLutChanged, this, EventArgs.Empty);
					break;
			}
		}

		public event EventHandler ShowRawPixChanged
		{
			add { _showRawPixChanged += value; }
			remove { _showRawPixChanged -= value; }
		}

		public event EventHandler ShowVoiLutChanged
		{
			add { _showVoiLutChanged += value; }
			remove { _showVoiLutChanged -= value; }
		}

		public bool ShowRawPix
		{
			get
			{
				try
				{
					return _settings.ShowRawPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				_settings.ShowRawPixelValue = value;
				_settings.Save();
			}
		}

		public bool ShowVoiLut
		{
			get
			{
				try
				{
					return _settings.ShowVOIPixelValue;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				_settings.ShowVOIPixelValue = value;
				_settings.Save();
			}
		}

		public void ToggleShowRawPix()
		{
			ShowRawPix = !ShowRawPix;
		}

		public void ToggleShowVoiLut()
		{
			ShowVoiLut = !ShowVoiLut;
		}

		#endregion

		#region IProbe Implementation

		void IProbe.Probe(PointF coordinate, CoordinateSystem coordinateSystem)
		{
			if (Context.Viewer.SelectedTile == null)
				throw new InvalidOperationException("No tile selected.");

			if (SelectedPresentationImage == null)
				throw new InvalidOperationException("No image selected.");

			if (SelectedSpatialTransformProvider == null)
				throw new InvalidOperationException("Unsupported image type selected.");

			if (coordinateSystem == CoordinateSystem.Destination)
				coordinate = SelectedSpatialTransformProvider.SpatialTransform.ConvertToSource(coordinate);

			_selectedTile = (Tile) Context.Viewer.SelectedTile;
			_selectedTile.InformationBox = new InformationBox();
			_selectedImageGraphic = SelectedImageGraphicProvider != null ? SelectedImageGraphicProvider.ImageGraphic : null;
			_selectedSpatialTransform = SelectedSpatialTransformProvider.SpatialTransform;
			_selectedFrame = ((IImageSopProvider) SelectedPresentationImage).Frame;
			_selectedCoordinateMapping = SelectedPresentationImage is IPatientCoordinateMappingProvider ? ((IPatientCoordinateMappingProvider) SelectedPresentationImage).PatientCoordinateMapping : null;

			Probe(Point.Truncate(coordinate), true, true);
		}

		void IProbe.ResetProbe()
		{
			Cancel();
		}

		#endregion
	}
}