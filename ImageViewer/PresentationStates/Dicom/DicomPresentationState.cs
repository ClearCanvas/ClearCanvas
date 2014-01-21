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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// DICOM images without a real, concrete presentation state SOP should
	/// install this pseudo presentation state to recover shutter and overlay
	/// components from the image header.
	/// </summary>
	[Cloneable(true)]
    public sealed class DicomPresentationState : PresentationState
	{
        public static readonly DicomPresentationState Default = new DicomPresentationState { _isReadOnly = true };

        private bool _showGrayscaleInverted;
        private bool _isReadOnly;
        
        public DicomPresentationState()
        {
	        _isReadOnly = false;
        }

	    /// <summary>
	    /// Specifies whether or not to show grayscale images inverted.
	    /// </summary>
	    public bool ShowGrayscaleInverted
	    {
	        get { return _showGrayscaleInverted; }
	        set
	        {
                if (_isReadOnly)
                    throw new InvalidOperationException("Cannot change the properties of a read-only presentation state.");

	            _showGrayscaleInverted = value;
	        }
	    }

	    private static void DeserializeHeader(IDicomPresentationImage image)
		{
			bool anyFailures = false;

			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			if(dicomGraphicsPlane == null)
				throw new DicomGraphicsDeserializationException("Unknown exception.");

			// Check if the image header specifies a bitmap display shutter (bitmap shutters have built-in multiframe handling, and thus the attributes are not themselves frame-specific, unlike geometric shutters)
			BitmapDisplayShutterModuleIod bitmapShutterIod = new BitmapDisplayShutterModuleIod(image.ImageSop.DataSource);
			int bitmapShutterIndex = -1;
			if (bitmapShutterIod.ShutterShape == ShutterShape.Bitmap)
				bitmapShutterIndex = bitmapShutterIod.ShutterOverlayGroupIndex;
			if (bitmapShutterIndex < 0 || bitmapShutterIndex > 15)
				bitmapShutterIndex = -1;

			try
			{
				GeometricShuttersGraphic geometricShuttersGraphic = DicomGraphicsFactory.CreateGeometricShuttersGraphic(image.Frame);
				dicomGraphicsPlane.Shutters.Add(geometricShuttersGraphic);
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error occurred trying to create geometric shutter graphics from the image header.");
			}

			try
			{
				List<OverlayPlaneGraphic> overlayPlaneGraphics = DicomGraphicsFactory.CreateOverlayPlaneGraphics(image.Frame);
				foreach (OverlayPlaneGraphic overlay in overlayPlaneGraphics)
				{
					if (bitmapShutterIndex != -1 && overlay.Index == bitmapShutterIndex)
					{
						// Someday when we support CIELab colour, we should set presentation value/colour based on client display type
						if (bitmapShutterIod.ShutterPresentationValue != null)
							overlay.GrayPresentationValue = (ushort)bitmapShutterIod.ShutterPresentationValue;
						overlay.Color = null;

						// insert the bitmap shutter into the shutters graphic instead of with the other overlays
						dicomGraphicsPlane.Shutters.Add(overlay);
					}
					else if (overlay.Index >= 0 && overlay.Index < 16)
					{
						// otherwise just add the overlay to the default layer for overlays and activate immediately
						dicomGraphicsPlane.ImageOverlays.Add(overlay);
						dicomGraphicsPlane.ImageOverlays.ActivateAsLayer(overlay, "OVERLAY");
					}
					else
					{
						// otherwise just add the overlay to the default layer for overlays and activate immediately
						dicomGraphicsPlane.UserOverlays.Add(overlay);
						dicomGraphicsPlane.UserOverlays.ActivateAsLayer(overlay, "OVERLAY");
					}
				}
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error occurred trying to create overlay graphics from the image header.");
			}

			dicomGraphicsPlane.Shutters.ActivateFirst();

			if (anyFailures)
				throw new DicomGraphicsDeserializationException("At least one failure occurred in deserializing graphics from the image header.");
		}

		public override void Serialize(IEnumerable<IPresentationImage> images)
		{
			throw new NotSupportedException("The default presentation state cannot be serialized.");
		}

		public override void Deserialize(IEnumerable<IPresentationImage> images)
		{
			foreach (IPresentationImage image in images)
			{
				if (image is IDicomPresentationImage)
				{
					DeserializeHeader((IDicomPresentationImage) image);
				    DeserializeInvert((IDicomPresentationImage) image);
				}
			}
		}

        private void DeserializeInvert(IDicomPresentationImage image)
        {
            if (!ShowGrayscaleInverted)
                return;

            if (!(image.ImageGraphic.PixelData is GrayscalePixelData))
                return;

            if (!(image is IVoiLutProvider))
                return;

            IVoiLutManager manager = ((IVoiLutProvider) image).VoiLutManager;
            //Invert can be true by default depending on photometric interpretation,
            //so we'll just assume the current value is the default and flip it.
            manager.Invert = !manager.Invert;
        }

	    public override void Clear(IEnumerable<IPresentationImage> image)
		{
		}
	}
}
