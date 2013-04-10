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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class WindowLevelSynchronicityTool : ImageViewerTool
	{
		private readonly IList<IDisplaySet> _fusionDisplaySets = new List<IDisplaySet>();
		private double _previousWindowWidth;
		private double _previousWindowCenter;
		private DelayedEventPublisher _publisher;
		private SynchronizationContext _synchronizationContext;

		public override void Initialize()
		{
			base.Initialize();

			ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;
			ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;

			_publisher = new DelayedEventPublisher((o, args) => UpdateFusion((IPresentationImage)o));
			_synchronizationContext = SynchronizationContext.Current;
		}

		protected override void Dispose(bool disposing)
		{
			_synchronizationContext = null;

			if (_publisher != null)
			{
				_publisher.Dispose();
				_publisher = null;
			}

			ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;

			base.Dispose(disposing);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);

			if (_publisher != null)
				_publisher.PublishNow();
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			if (e.OldDisplaySet != null)
			{
				_fusionDisplaySets.Remove(e.OldDisplaySet);
			}

			if (e.NewDisplaySet != null && e.NewDisplaySet.Descriptor is PETFusionDisplaySetDescriptor)
			{
				if (e.NewDisplaySet.ImageBox != null)
				{
					_fusionDisplaySets.Add(e.NewDisplaySet);

					// no point doing all this to find an appropriate VOI LUT if there are no images in the display set - but do update the fusionDisplaySets list!
					if (e.NewDisplaySet.PresentationImages.Count == 0)
						return;

					// find any available display set containing the same series as the individual layers and capture its VOI LUT
					IVoiLut baseVoiLut = null, overlayVoiLut = null;
					Frame baseFrame = null, overlayFrame = null;
					var descriptor = (PETFusionDisplaySetDescriptor) e.NewDisplaySet.Descriptor;
					foreach (IImageBox imageBox in ImageViewer.PhysicalWorkspace.ImageBoxes)
					{
						var selectedImage = imageBox.TopLeftPresentationImage;
						if (imageBox.DisplaySet == null || imageBox.DisplaySet.Descriptor is PETFusionDisplaySetDescriptor
						    || !(selectedImage is IImageSopProvider && selectedImage is IVoiLutProvider))
							continue;

						var seriesUid = ((IImageSopProvider) selectedImage).ImageSop.SeriesInstanceUid;
						if (baseVoiLut == null && seriesUid == descriptor.SourceSeries.SeriesInstanceUid)
						{
							baseVoiLut = ((IVoiLutProvider) selectedImage).VoiLutManager.VoiLut;
							baseFrame = ((IImageSopProvider) selectedImage).Frame;
						}
						else if (overlayVoiLut == null && seriesUid == descriptor.PETSeries.SeriesInstanceUid)
						{
							overlayVoiLut = ((IVoiLutProvider) selectedImage).VoiLutManager.VoiLut;
							overlayFrame = ((IImageSopProvider)selectedImage).Frame;
						}

						if (baseVoiLut != null && overlayVoiLut != null)
							break;
					}

					if (baseVoiLut == null || overlayVoiLut == null)
					{
						var fusionImage = (FusionPresentationImage) e.NewDisplaySet.PresentationImages[0];
						if (baseVoiLut == null)
							baseVoiLut = GetInitialVoiLut(baseFrame = fusionImage.Frame);
						if (overlayVoiLut == null)
							overlayVoiLut = GetInitialVoiLut(overlayFrame = fusionImage.OverlayFrameData.OverlayData.Frames[0]);
					}

					// replicate the captured VOI LUTs to the fusion images
					foreach (FusionPresentationImage image in e.NewDisplaySet.PresentationImages)
					{
						if (baseVoiLut != null)
							InstallVoiLut(image, baseVoiLut, baseFrame, false);
						if (overlayVoiLut != null)
							InstallVoiLut(image, overlayVoiLut, overlayFrame, true);
					}
				}
			}
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			if (_fusionDisplaySets.Count == 0)
				return;

			if ((e.PresentationImage is FusionPresentationImage))
				return;

			if (!(e.PresentationImage is IImageSopProvider) || !(e.PresentationImage is IVoiLutProvider))
				return;

			// only synchronize the VOI LUTs if the source LUT is linear - otherwise, leave it alone
			var sourceVoiLut = ((IVoiLutProvider) e.PresentationImage).VoiLutManager.VoiLut as IVoiLutLinear;
			if (sourceVoiLut == null)
				return;

			if (Equals(_previousWindowCenter, sourceVoiLut.WindowCenter) &&
				Equals(_previousWindowWidth, sourceVoiLut.WindowWidth))
				return;

			_previousWindowCenter = sourceVoiLut.WindowCenter;
			_previousWindowWidth = sourceVoiLut.WindowWidth;

			_publisher.Publish(e.PresentationImage, EventArgs.Empty);
		}

		private void UpdateFusion(IPresentationImage sourceImage)
		{
			var sourceVoiLut = ((IVoiLutProvider)sourceImage).VoiLutManager.VoiLut as IVoiLutLinear;
			var imageSopProvider = (IImageSopProvider) sourceImage;

			// find any available display set containing the same series as the individual layers and capture its VOI LUT
			var seriesInstanceUid = imageSopProvider.ImageSop.SeriesInstanceUid;
			foreach (IDisplaySet displaySet in _fusionDisplaySets)
			{
				var anyVisibleChange = false;

				var descriptor = (PETFusionDisplaySetDescriptor)displaySet.Descriptor;
				if (descriptor.SourceSeries.SeriesInstanceUid == seriesInstanceUid)
				{
					// replicate the captured VOI LUT to the fusion images
					foreach (FusionPresentationImage image in displaySet.PresentationImages)
					{
						InstallVoiLut(image, sourceVoiLut, imageSopProvider.Frame, false);
						anyVisibleChange |= (image.Visible);
					}
				}
				else if (descriptor.PETSeries.SeriesInstanceUid == seriesInstanceUid)
				{
					// replicate the captured VOI LUT to the fusion images
					foreach (FusionPresentationImage image in displaySet.PresentationImages)
					{
						InstallVoiLut(image, sourceVoiLut, imageSopProvider.Frame, true);
						anyVisibleChange |= (image.Visible);
					}
				}

				// force a draw only if we replicated the VOI LUT to a visible image somewhere
				if (anyVisibleChange)
				{
					if (_synchronizationContext != null)
						_synchronizationContext.Post(s => ((IDisplaySet)s).Draw(), displaySet);
					else
						displaySet.Draw();
				}
			}
		}

		/// <summary>
		/// Attempts to install an appropriate equivalent of the specified <paramref name="sourceVoiLut"/> to a fusion image. If the LUT is not linear, computes a dummy LUT.
		/// </summary>
		private static void InstallVoiLut(FusionPresentationImage fusionImage, IVoiLut sourceVoiLut, Frame sourceFrame, bool applyToOverlay)
		{
			IVoiLut newVoiLut;
			if (sourceVoiLut is MinMaxPixelCalculatedLinearLut)
			{
				if (applyToOverlay)
				{
					// if the overlay source image is using a min/max calculated LUT, install a custom calculated LUT that delay-computes min/max from the fusion data
					// we need to delay-compute this because the fusion image graphic is delay-generated, and thus not necessarily available until just before rendering!
					var skipModalityLut = sourceFrame.ParentImageSop.Modality == @"PT" && sourceFrame.IsSubnormalRescale;
					newVoiLut = new FusionOverlayMinMaxVoiLutLinear(fusionImage, !skipModalityLut);
				}
				else
				{
					// if the base source image is using a min/max calculated LUT, install a similarly min/max calculated LUT for the base of the fusion image
					newVoiLut = new MinMaxPixelCalculatedLinearLut(fusionImage.ImageGraphic.PixelData);
				}
			}
			else if (sourceVoiLut is IVoiLutLinear)
			{
				var voiLutLinear = (IVoiLutLinear) sourceVoiLut;
				var normalizedVoiSlope = 1.0;
				var normalizedVoiIntercept = 0.0;
				if (applyToOverlay && sourceFrame.ParentImageSop.Modality == @"PT" && sourceFrame.IsSubnormalRescale)
				{
					// for subnormal PET rescale slope cases, the original VOI windows must be transformed through the same process as what MPR did to the pixel data
					normalizedVoiSlope = sourceFrame.RescaleSlope/fusionImage.OverlayFrameData.OverlayRescaleSlope;
					normalizedVoiIntercept = (sourceFrame.RescaleIntercept - fusionImage.OverlayFrameData.OverlayRescaleIntercept)/fusionImage.OverlayFrameData.OverlayRescaleSlope;
				}
				newVoiLut = new BasicVoiLutLinear(voiLutLinear.WindowWidth*normalizedVoiSlope, voiLutLinear.WindowCenter*normalizedVoiSlope + normalizedVoiIntercept);
			}
			else
			{
				// if the source image is using some non-linear LUT, just install a default pass-through LUT
				newVoiLut = new IdentityVoiLinearLut();
			}

			if (applyToOverlay)
				fusionImage.OverlayVoiLutManager.InstallVoiLut(newVoiLut);
			else
				fusionImage.BaseVoiLutManager.InstallVoiLut(newVoiLut);
		}

		/// <summary>
		/// Gets the initial VOI LUT for the source frames (base or overlay). This is NOT the LUT used on the fusion image! See <see cref="InstallVoiLut"/>.
		/// </summary>
		private static IVoiLut GetInitialVoiLut(Frame frame)
		{
			if (frame != null)
			{
				using (var image = PresentationImageFactory.Create(frame))
				{
					var voiLut = InitialVoiLutProvider.Instance.GetLut(image);
					if (voiLut == null && image is IImageGraphicProvider)
					{
						var pixelData = ((IImageGraphicProvider) image).ImageGraphic.PixelData;
						if (pixelData is GrayscalePixelData)
							voiLut = new MinMaxPixelCalculatedLinearLut((GrayscalePixelData) pixelData);
					}
					return voiLut;
				}
			}
			return null;
		}
	}
}