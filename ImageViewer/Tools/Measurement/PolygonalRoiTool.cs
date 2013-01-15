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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarPolygonalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.PolygonalRoiToolSmall.png", "Icons.PolygonalRoiToolMedium.png", "Icons.PolygonalRoiToolLarge.png")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Roi.Polygonal")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class PolygonalRoiTool : MeasurementTool
	{
		public PolygonalRoiTool() : base(SR.TooltipPolygonalRoi) {}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreatePolygonalRoi; }
		}

		protected override string RoiNameFormat
		{
			get { return SR.FormatPolygonName; }
		}

		protected override IGraphic CreateGraphic()
		{
			return new PolygonControlGraphic(true, new MoveControlGraphic(new PolylineGraphic(true)));
		}

		protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
		{
			return new InteractivePolygonGraphicBuilder((IPointsGraphic) graphic);
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new PolygonalRoiCalloutLocationStrategy();
		}
    }

    #region Oto
    partial class PolygonalRoiTool : IDrawPolygon
    {
        AnnotationGraphic IDrawPolygon.Draw(CoordinateSystem coordinateSystem, string name, IList<PointF> vertices)
        {
            var image = Context.Viewer.SelectedPresentationImage;
            if (!CanStart(image))
                throw new InvalidOperationException("Can't draw a polygonal ROI at this time.");

            var imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
            if (coordinateSystem == CoordinateSystem.Destination)
                vertices = vertices.Select(v => imageGraphic.SpatialTransform.ConvertToSource(v)).ToList();

            var overlayProvider = (IOverlayGraphicsProvider) image;
            var roiGraphic = CreateRoiGraphic(false);
            roiGraphic.Name = name;
            AddRoiGraphic(image, roiGraphic, overlayProvider);

            var subject = (IPointsGraphic)roiGraphic.Subject;

            foreach (var vertex in vertices)
                subject.Points.Add(vertex);

            roiGraphic.Callout.Update();
            roiGraphic.State = roiGraphic.CreateSelectedState();
            //roiGraphic.Draw();
            return roiGraphic;
        }
    }
    #endregion
}