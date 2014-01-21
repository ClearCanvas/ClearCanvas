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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuMatchScale", "Activate", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuMatchScale", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarMatchScale", "Activate", KeyStroke = XKeys.M)]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
    [IconSet("activate", "Icons.MatchScaleToolSmall.png", "Icons.MatchScaleToolMedium.png", "Icons.MatchScaleToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.MatchScale")]
	[Tooltip("activate", "TooltipMatchScale")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class MatchScaleTool : ImageViewerTool, IUndoableOperation<IPresentationImage>
	{
		#region Private Fields

		private const float _oneDegreeInRadians = (float)(Math.PI / 180);

		private bool _matchScaleNonParallelImages = true;
		private float _referenceDisplayedWidth;
		private RectangleF _referenceDisplayRectangle;

		#endregion

		public MatchScaleTool()
		{
            _matchScaleNonParallelImages = ToolSettings.DefaultInstance.MatchScaleForNonParallelImages;
		}

		#region Private Properties

		private IPresentationImage ReferenceImage
		{
			get { return base.ImageViewer.SelectedPresentationImage; }
		}

		#endregion

		#region Public Methods

		public void Activate()
		{
			if (!AppliesTo(ReferenceImage))
				return;

			CalculateReferenceDisplayValues();
			if (Math.Abs(_referenceDisplayRectangle.Width) < 50)
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageNotEnoughImageVisible, MessageBoxActions.Ok);
				return;
			}

			var historyCommand = new DrawableUndoableOperationCommand<IPresentationImage>(this, GetAllImages());
			historyCommand.Execute();
			if (historyCommand.Count > 0)
			{
				historyCommand.Name = SR.CommandMatchScale;
				base.ImageViewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		#endregion

		#region IUndoableOperation<IPresentationImage> Members

		public IMemorable GetOriginator(IPresentationImage image)
		{
			return GetImageTransform(image);
		}

		public bool AppliesTo(IPresentationImage image)
		{
			IImageSpatialTransform transform = GetImageTransform(image);
			if (transform == null)
				return false;

			//mustn't be rotated at a non-right angle to the viewport.
			if (transform.RotationXY % 90 != 0)
				return false;

			Frame frame = GetFrame(image);
			if (frame == null || frame.NormalizedPixelSpacing.IsNull)
				return false;
			
			if (!_matchScaleNonParallelImages && image != this.ReferenceImage)
			{
				Frame referenceFrame = GetFrame(this.ReferenceImage);
				if (!IsInSameFrameOfReference(referenceFrame, frame))
					return false;

				Vector3D referenceNormal = referenceFrame.ImagePlaneHelper.GetNormalVector();
				Vector3D normal = frame.ImagePlaneHelper.GetNormalVector();

				//If the reference image has no spatial info, then match scale with other such images
				if (referenceNormal == null && normal == null)
					return true;

				//if only one is null, don't do it
				if (referenceNormal == null || normal == null)
					return false;

				//if they're not parallel, don't do it
				if (!referenceNormal.IsParallelTo(normal, _oneDegreeInRadians))
					return false;
			}

			return true;
		}

		public void Apply(IPresentationImage image)
		{
			if (image == ReferenceImage)
				return;

			//Turn off scale to fit and start with scale=1, then adjust it.
			//We do this because images that have been "scaled to fit", but have not been shown yet,
			//have no client rectangle and their scale is often very small.  This is safer
			//and could produce a more accurate result.
			IImageSpatialTransform matchTransform = GetImageTransform(image);
			matchTransform.ScaleToFit = false;
			matchTransform.Scale = 1;

			//get the displayed width (in mm) for the same size display rectangle in the image to be matched.
			float matchDisplayedWidth = GetDisplayedWidth(image, _referenceDisplayRectangle);
			float rescaleAmount = matchDisplayedWidth / _referenceDisplayedWidth;

			matchTransform.Scale *= rescaleAmount;
		}

		#endregion

		#region Private Methods

		private void CalculateReferenceDisplayValues()
		{
			IImageSpatialTransform transform = GetImageTransform(ReferenceImage);
			Frame frame = GetFrame(ReferenceImage);

			//calculate the width (in mm) of the portion of the image that is visible on the display,
			//as well as the display rectangle it occupies.

			RectangleF sourceRectangle = new RectangleF(0, 0, frame.Columns, frame.Rows);
			_referenceDisplayRectangle = transform.ConvertToDestination(sourceRectangle);
			_referenceDisplayRectangle = RectangleUtilities.Intersect(_referenceDisplayRectangle, ReferenceImage.ClientRectangle);

			_referenceDisplayedWidth = GetDisplayedWidth(ReferenceImage, _referenceDisplayRectangle);
		}

		#region Private Helper Methods

		private static bool IsInSameFrameOfReference(Frame thisFrame, Frame otherFrame)
		{
			if (thisFrame.ParentImageSop.StudyInstanceUid != otherFrame.ParentImageSop.StudyInstanceUid)
				return false;

			return thisFrame.FrameOfReferenceUid == otherFrame.FrameOfReferenceUid;
		}

		private static float GetDisplayedWidth(IPresentationImage presentationImage, RectangleF referenceDisplayedRectangle)
		{
			IImageSpatialTransform transform = GetImageTransform(presentationImage);
			Frame frame = GetFrame(presentationImage);

			//Convert the displayed width to source dimensions
			SizeF sourceSize = transform.ConvertToSource(new SizeF(referenceDisplayedRectangle.Width, 0));
			float x = Math.Abs(sourceSize.Width);
			float y = Math.Abs(sourceSize.Height);

			//The displayed width is the magnitude of the line in source coordinates,
			//but one of xLength or yLength will always be zero, so we can optimize.
			if (x > y)
				return x * (float)frame.NormalizedPixelSpacing.Column;
			
			return y * (float)frame.NormalizedPixelSpacing.Row;
		}

		private static IImageSpatialTransform GetImageTransform(IPresentationImage image)
		{
			if (image != null && image is ISpatialTransformProvider)
				return ((ISpatialTransformProvider)image).SpatialTransform as IImageSpatialTransform;

			return null;
		}

		private static Frame GetFrame(IPresentationImage image)
		{
			if (image != null && image is IImageSopProvider)
				return ((IImageSopProvider)image).Frame;

			return null;
		}

		private IEnumerable<IPresentationImage> GetAllImages()
		{
			foreach (IImageBox imageBox in base.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet == null)
					continue;
				
				foreach (IPresentationImage image in imageBox.DisplaySet.PresentationImages)
					yield return image;
			}
		}

		#endregion
		#endregion
	}
}
