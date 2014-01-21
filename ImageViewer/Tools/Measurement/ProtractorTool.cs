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
	[MenuAction("activate", "imageviewer-contextmenu/MenuProtractor", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuProtractor", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarProtractor", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.ProtractorToolSmall.png", "Icons.ProtractorToolMedium.png", "Icons.ProtractorToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Annotations.Measurement.Angle")]
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class ProtractorTool : MeasurementTool, IDrawProtractor
	{
		public ProtractorTool()
			: base(SR.TooltipProtractor) {}

		protected override string CreationCommandName
		{
			get { return SR.CommandCreateProtractor; }
		}

		protected override string RoiNameFormat
		{
			get { return SR.FormatProtractorName; }
		}

		protected override InteractiveGraphicBuilder CreateGraphicBuilder(IGraphic graphic)
		{
			return new InteractivePolylineGraphicBuilder(3, (IPointsGraphic) graphic);
		}

		protected override IGraphic CreateGraphic()
		{
			return new VerticesControlGraphic(new MoveControlGraphic(new ProtractorGraphic()));
		}

		protected override IAnnotationCalloutLocationStrategy CreateCalloutLocationStrategy()
		{
			return new ProtractorRoiCalloutLocationStrategy();
		}

		#region IDrawProtractor Members

		AnnotationGraphic IDrawProtractor.Draw(CoordinateSystem coordinateSystem, string name, PointF point1, PointF vertex, PointF point2)
		{
			var image = Context.Viewer.SelectedPresentationImage;
			if (!CanStart(image))
				throw new InvalidOperationException("Can't draw a protractor at this time.");

			var imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
			if (coordinateSystem == CoordinateSystem.Destination)
			{
				point1 = imageGraphic.SpatialTransform.ConvertToSource(point1);
				vertex = imageGraphic.SpatialTransform.ConvertToSource(vertex);
				point2 = imageGraphic.SpatialTransform.ConvertToSource(point2);
			}

			var overlayProvider = (IOverlayGraphicsProvider) image;
			var roiGraphic = CreateRoiGraphic(false);
			roiGraphic.Name = name;
			AddRoiGraphic(image, roiGraphic, overlayProvider);

			var subject = (IPointsGraphic) roiGraphic.Subject;
			subject.Points.Add(point1);
			subject.Points.Add(vertex);
			subject.Points.Add(point2);

			roiGraphic.Callout.Update();
			roiGraphic.State = roiGraphic.CreateSelectedState();
			return roiGraphic;
		}

		#endregion
	}
}