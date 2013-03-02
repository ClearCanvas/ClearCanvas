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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;
using System.ComponentModel;
using Point=System.Drawing.Point;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ExtensionPoint]
	public sealed class ProbeToolDropDownToolExtensionPoint : ExtensionPoint<ITool> { }

	[MenuAction("activate", "imageviewer-contextmenu/MenuProbe", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuProbe", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarProbe", "Select", "DropDownMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.B)]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.ProbeToolSmall.png", "Icons.ProbeToolMedium.png", "Icons.ProbeToolLarge.png")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
    [GroupHint("activate", "Tools.Image.Inspection.Probe")]

	[MouseToolButton(XMouseButtons.Left, false)]

	#region Tool Settings Actions

	[MenuAction("showRawPix", "probetool-dropdown/MenuShowRawPixelValue", "ToggleShowRawPix")]
	[CheckedStateObserver("showRawPix", "ShowRawPix", "ShowRawPixChanged")]
	[GroupHint("showRawPix", "Tools.Image.Inspection.Probe.Modality.CT.ShowPixel")]

	[MenuAction("showVoiLut", "probetool-dropdown/MenuShowVoiPixelValue", "ToggleShowVoiLut")]
	[CheckedStateObserver("showVoiLut", "ShowVoiLut", "ShowVoiLutChanged")]
    [GroupHint("showVoiLut", "Tools.Image.Inspection.Probe.General.ShowVoiLut")]

	#endregion

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public partial class ProbeTool : MouseImageViewerTool
	{
		private Tile _selectedTile;
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
			this.CursorToken = new CursorToken("ProbeCursor.png", this.GetType().Assembly);
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
			if (this.SelectedImageGraphicProvider == null || SelectedImageSopProvider == null)
				return false;

			_selectedTile = mouseInformation.Tile as Tile;
			_selectedTile.InformationBox = new InformationBox();
			_selectedImageGraphic = this.SelectedImageGraphicProvider.ImageGraphic;
			_selectedFrame = ((IImageSopProvider) SelectedPresentationImage).Frame;

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

            ToolSettings settings = ToolSettings.DefaultInstance;
			bool showPixelValue = settings.ShowRawPixelValue;
			bool showVoiValue = settings.ShowVOIPixelValue;

			Probe(sourcePointRounded, showPixelValue, showVoiValue);
		}

		private void Probe(Point sourcePointRounded, bool showPixelValue, bool showVoiValue)
		{
			string probeString;
			string coordinateString = String.Format(SR.FormatProbeInfo, SR.LabelLocation, string.Format(SR.FormatCoordinates, SR.LabelNotApplicable, SR.LabelNotApplicable));
			string pixelValueString = String.Format(SR.FormatProbeInfo, SR.LabelRawPixel, SR.LabelNotApplicable);
			string modalityLutString = String.Format(SR.FormatProbeInfo, SR.LabelModalityLut, SR.LabelNotApplicable);
			string voiLutString = String.Format(SR.FormatProbeInfo, SR.LabelVOILut, SR.LabelNotApplicable);

			try
			{
				var displayString = new StringBuilder();
				if (_selectedImageGraphic.BoundingBox.Contains(sourcePointRounded))
				{
					coordinateString = String.Format(SR.FormatProbeInfo, SR.LabelLocation, string.Format(SR.FormatCoordinates, sourcePointRounded.X, sourcePointRounded.Y));

					if (_selectedImageGraphic is GrayscaleImageGraphic)
					{
						GrayscaleImageGraphic image = _selectedImageGraphic as GrayscaleImageGraphic;

						int pixelValue = 0;

						GetPixelValue(image, sourcePointRounded, ref pixelValue, ref pixelValueString);
						GetModalityLutValue(image, pixelValue, ref modalityLutString);
						GetVoiLutValue(image, pixelValue, ref voiLutString);

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

				// show the coordinate last, cause it's probably the least interesting information
				displayString.AppendLine(coordinateString);

				probeString = displayString.ToString().Trim();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				probeString = SR.MessageProbeToolError;
			}

			var destinationPoint = Point.Round(_selectedImageGraphic.SpatialTransform.ConvertToDestination(sourcePointRounded));
			_selectedTile.InformationBox.Update(probeString, destinationPoint);
		}

		private void GetPixelValue(
			GrayscaleImageGraphic grayscaleImage,
			Point sourcePointRounded,
			ref int pixelValue,
			ref string pixelValueString)
		{
			pixelValue = grayscaleImage.PixelData.GetPixel(sourcePointRounded.X, sourcePointRounded.Y);
			pixelValueString = String.Format(SR.FormatProbeInfo, SR.LabelRawPixel, pixelValue);
		}

		private void GetModalityLutValue(
			GrayscaleImageGraphic grayscaleImage,
			int pixelValue,
			ref string modalityLutString)
		{
			if (grayscaleImage.ModalityLut != null)
			{
				var modalityLutValue = grayscaleImage.ModalityLut[pixelValue];
				
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
			GrayscaleImageGraphic grayscaleImage,
			int pixelValue,
			ref string voiLutString)
		{
			if (grayscaleImage.VoiLut != null)
			{
				var voiLutValue = grayscaleImage.OutputLut[pixelValue];
				voiLutString = String.Format(SR.FormatProbeInfo, SR.LabelVOILut, voiLutValue);
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
			this.ShowRawPix = !this.ShowRawPix;
		}

		public void ToggleShowVoiLut()
		{
			this.ShowVoiLut = !this.ShowVoiLut;
		}

		#endregion
	}

	partial class ProbeTool : IProbe
	{
		void IProbe.Probe(PointF coordinate, CoordinateSystem coordinateSystem)
		{
			if (Context.Viewer.SelectedTile == null)
				throw new InvalidOperationException("No tile selected.");

			if (SelectedPresentationImage == null)
				throw new InvalidOperationException("No image selected.");

			if (SelectedImageGraphicProvider == null)
				throw new InvalidOperationException("Unsupported image type selected.");

			if (coordinateSystem == CoordinateSystem.Destination)
				coordinate = SelectedImageGraphicProvider.ImageGraphic.SpatialTransform.ConvertToSource(coordinate);

			_selectedTile = (Tile) Context.Viewer.SelectedTile;
			_selectedTile.InformationBox = new InformationBox();
			_selectedImageGraphic = SelectedImageGraphicProvider.ImageGraphic;
			_selectedFrame = ((IImageSopProvider) SelectedPresentationImage).Frame;

			Probe(Point.Truncate(coordinate), true, true);
		}

		void IProbe.ResetProbe()
		{
			Cancel();
		}
	}
}
