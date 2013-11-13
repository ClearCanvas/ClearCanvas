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

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.InteractiveGraphics.Tests
{
	[MenuAction("ellipse", "global-menus/&Debug/Draw Ellipse", "DrawEllipse")]
	[MenuAction("rectangle", "global-menus/&Debug/Draw Rectangle", "DrawRectangle")]
	[MenuAction("polyline", "global-menus/&Debug/Draw Polyline", "DrawPolyline")]
	[MenuAction("spline", "global-menus/&Debug/Draw Spline", "DrawSpline")]
	[MenuAction("toggleEnabled", "global-menus/&Debug/Enabled", "ToggleEnabled")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class InteractiveGraphicsDemoTool : ImageViewerTool
	{
		public void DrawEllipse()
		{
			var rectangle = new Rectangle(new Point(), SelectedPresentationImage.SceneSize);
			rectangle.Inflate(-10, -10);
			DrawGraphic(new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new EllipsePrimitive {TopLeft = rectangle.Location, BottomRight = rectangle.Location + rectangle.Size}))));
		}

		public void DrawRectangle()
		{
			var rectangle = new Rectangle(new Point(), SelectedPresentationImage.SceneSize);
			rectangle.Inflate(-10, -10);
			DrawGraphic(new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new RectanglePrimitive {TopLeft = rectangle.Location, BottomRight = rectangle.Location + rectangle.Size}))));
		}

		public void DrawPolyline()
		{
			var rectangle = new Rectangle(new Point(), SelectedPresentationImage.SceneSize);
			rectangle.Inflate(-10, -10);
			var polyline = new PolylineGraphic {Points = {rectangle.Location, rectangle.Location + new Size(rectangle.Width/2, 0), rectangle.Location + new Size(rectangle.Width, 0), rectangle.Location + rectangle.Size, rectangle.Location + new Size(0, rectangle.Height), rectangle.Location + new Size(0, rectangle.Height/2)}};
			DrawGraphic(new VerticesControlGraphic(true, polyline));
		}

		public void DrawSpline()
		{
			var rectangle = new Rectangle(new Point(), SelectedPresentationImage.SceneSize);
			rectangle.Inflate(-10, -10);
			var spline = new SplinePrimitive {Points = {rectangle.Location, rectangle.Location + new Size(rectangle.Width/2, 0), rectangle.Location + new Size(rectangle.Width, 0), rectangle.Location + rectangle.Size, rectangle.Location + new Size(0, rectangle.Height), rectangle.Location + new Size(0, rectangle.Height/2)}};
			DrawGraphic(new VerticesControlGraphic(true, spline));
		}

		private void DrawGraphic(IGraphic graphic)
		{
			if (SelectedOverlayGraphicsProvider == null) return;
			SelectedOverlayGraphicsProvider.OverlayGraphics.Add(new XStatefulGraphic(graphic));
			SelectedPresentationImage.Draw();
		}

		public void ToggleEnabled()
		{
			if (SelectedPresentationImage == null) return;

			var statefulGraphic = SelectedPresentationImage.SelectedGraphic as XStatefulGraphic;
			if (statefulGraphic != null)
			{
				var controlGraphic = ((IDecoratorGraphic) statefulGraphic).DecoratedGraphic as IControlGraphic;
				while (controlGraphic != null)
				{
					controlGraphic.Enabled = !controlGraphic.Enabled;
					controlGraphic = controlGraphic.DecoratedGraphic as IControlGraphic;
				}
			}
		}

		[Cloneable]
		[DicomSerializableGraphicAnnotation(typeof (XStatefulGraphicSerializer))]
		private class XStatefulGraphic : StandardStatefulGraphic
		{
			public XStatefulGraphic(IGraphic subject) : base(subject) {}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected XStatefulGraphic(XStatefulGraphic source, ICloningContext context) : base(source, context)
			{
				context.CloneFields(source, this);
			}
		}

		private class XStatefulGraphicSerializer : GraphicAnnotationSerializer<XStatefulGraphic>
		{
			protected override void Serialize(XStatefulGraphic graphic, GraphicAnnotationSequenceItem serializationState)
			{
				SerializeGraphic(graphic.Subject, serializationState);
			}
		}
	}
}

#endif