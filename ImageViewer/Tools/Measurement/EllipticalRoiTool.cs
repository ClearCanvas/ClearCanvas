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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.EllipticalRoiToolSmall.png", "Icons.EllipticalRoiToolMedium.png", "Icons.EllipticalRoiToolLarge.png")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Roi.Elliptical")]

	[MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public partial class EllipticalRoiTool : MeasurementTool
	{
		public EllipticalRoiTool()
			: base(SR.TooltipEllipticalRoi)
		{
		}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreateEllipiticalRoi; }
		}

		protected override string RoiNameFormat
		{
			get { return SR.FormatEllipseName; }
		}

		protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
		{
			return new InteractiveBoundableGraphicBuilder((IBoundableGraphic) graphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new DefaultRoiCalloutLocationStrategy();
		}

		protected override IGraphic CreateGraphic()
		{
			return new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new EllipsePrimitive())));
		}
    }

    #region Oto
    partial class EllipticalRoiTool : IDrawEllipse
    {
        AnnotationGraphic IDrawEllipse.Draw(CoordinateSystem coordinateSystem, string name, PointF topLeft, PointF bottomRight)
        {
            var image = Context.Viewer.SelectedPresentationImage;
            if (!CanStart(image))
                throw new InvalidOperationException("Can't draw an elliptical ROI at this time.");

            var imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
            if (coordinateSystem == CoordinateSystem.Destination)
            {
                //Use the image graphic to get the "source" coordinates because it's already in the scene.
                topLeft = imageGraphic.SpatialTransform.ConvertToSource(topLeft);
                bottomRight = imageGraphic.SpatialTransform.ConvertToSource(bottomRight);
            }

            var overlayProvider = (IOverlayGraphicsProvider) image;
            var roiGraphic = CreateRoiGraphic(false);
            var subject = (EllipsePrimitive)roiGraphic.Subject;
            roiGraphic.Name = name;
            AddRoiGraphic(image, roiGraphic, overlayProvider);

            subject.TopLeft = topLeft;
            subject.BottomRight = bottomRight;

            roiGraphic.Callout.Update();
            roiGraphic.State = roiGraphic.CreateSelectedState();
            //roiGraphic.Draw();
            return roiGraphic;
        }
    }
    #endregion
}