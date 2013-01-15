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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tools.Measurement.Tests
{
	[TestFixture]
	public class ShowAnglesToolTest
	{
		[Test]
		public void TestCloningShowAnglesToolCompositeGraphicLineSelection()
		{
			MockShowAnglesTool mockOwnerTool = new MockShowAnglesTool();
			using (MockPresentationImage image = new MockPresentationImage(100, 100))
			{
				ShowAnglesTool.ShowAnglesToolCompositeGraphic composite = new ShowAnglesTool.ShowAnglesToolCompositeGraphic(mockOwnerTool);
				image.OverlayGraphics.Add(composite);

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					Assert.IsNotNull(cloneComposite, "ShowAnglesToolCompositeGraphic should be cloneable.");
				}

				PolylineGraphic line1 = new PolylineGraphic();
				line1.Points.Add(new PointF(0, 0));
				line1.Points.Add(new PointF(10, 10));
				VerticesControlGraphic control1 = new VerticesControlGraphic(line1);
				image.OverlayGraphics.Add(control1);
				composite.Select(control1);
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);

					cloneComposite.CoordinateSystem = line1.CoordinateSystem;
					try
					{
						Assert.IsNotNull(cloneComposite.SelectedLine, "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection.");
						Assert.AreEqual(line1.Points[0], cloneComposite.SelectedLine.Points[0], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (X).");
						Assert.AreEqual(line1.Points[1], cloneComposite.SelectedLine.Points[1], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (Y).");
					}
					finally
					{
						cloneComposite.ResetCoordinateSystem();
					}
				}

				PolylineGraphic line2 = new PolylineGraphic();
				line2.Points.Add(new PointF(0, 10));
				line2.Points.Add(new PointF(10, 0));
				VerticesControlGraphic control2 = new VerticesControlGraphic(line2);
				image.OverlayGraphics.Add(control2);
				composite.Select(control2);
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);

					cloneComposite.CoordinateSystem = line2.CoordinateSystem;
					try
					{
						Assert.IsNotNull(cloneComposite.SelectedLine, "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (2).");
						Assert.AreEqual(line2.Points[0], cloneComposite.SelectedLine.Points[0], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (X2).");
						Assert.AreEqual(line2.Points[1], cloneComposite.SelectedLine.Points[1], "Cloned ShowAnglesToolCompositeGraphic should retain line graphic selection (Y2).");
					}
					finally
					{
						cloneComposite.ResetCoordinateSystem();
					}
				}
			}
		}

		[Test]
		public void TestCloningShowAnglesToolCompositeGraphicVisibility()
		{
			MockShowAnglesTool mockOwnerTool = new MockShowAnglesTool();
			using (MockPresentationImage image = new MockPresentationImage(100, 100))
			{
				ShowAnglesTool.ShowAnglesToolCompositeGraphic composite = new ShowAnglesTool.ShowAnglesToolCompositeGraphic(mockOwnerTool);
				image.OverlayGraphics.Add(composite);

				PolylineGraphic line1 = new PolylineGraphic();
				line1.Points.Add(new PointF(0, 0));
				line1.Points.Add(new PointF(10, 10));
				VerticesControlGraphic control1 = new VerticesControlGraphic(line1);
				image.OverlayGraphics.Add(control1);
				composite.Select(control1);
				composite.OnDrawing();

				PolylineGraphic line2 = new PolylineGraphic();
				line2.Points.Add(new PointF(0, 10));
				line2.Points.Add(new PointF(10, 0));
				VerticesControlGraphic control2 = new VerticesControlGraphic(line2);
				image.OverlayGraphics.Add(control2);
				composite.Select(control2);
				composite.OnDrawing();

				mockOwnerTool.ShowAngles = true;
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					Assert.IsTrue(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (true).");
					mockOwnerTool.ShowAngles = false;
					composite.OnDrawing();
					Assert.IsTrue(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (true) even when the original changes.");
				}

				mockOwnerTool.ShowAngles = false;
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					Assert.IsFalse(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (false).");
					mockOwnerTool.ShowAngles = true;
					composite.OnDrawing();
					Assert.IsFalse(cloneComposite.Visible, "Cloned ShowAnglesToolCompositeGraphic should retain visibility state when captured (false) even when the original changes.");
				}
			}
		}

		[Test]
		public void TestBug6614()
		{
			MockShowAnglesTool mockOwnerTool = new MockShowAnglesTool();
			using (MockPresentationImage image = new MockPresentationImage(1000, 1000))
			{
				ShowAnglesTool.ShowAnglesToolCompositeGraphic composite = new ShowAnglesTool.ShowAnglesToolCompositeGraphic(mockOwnerTool);
				image.OverlayGraphics.Add(composite);

				PolylineGraphic line1 = new PolylineGraphic();
				line1.Points.Add(new PointF(274.983246f, 483.976f));
				line1.Points.Add(new PointF(674.3086f, 490.196f));
				VerticesControlGraphic control1 = new VerticesControlGraphic(line1);
				image.OverlayGraphics.Add(control1);
				composite.Select(control1);
				composite.OnDrawing();

				using (IPresentationImage clone = image.Clone())
				{
					var cloneComposite = FindShowAnglesToolComposite(clone);
					var cloneAngles = FindShowAnglesTool(cloneComposite.Graphics);

					if (cloneAngles != null)
					{
						foreach (ICalloutGraphic calloutGraphic in cloneAngles.Graphics.Where(IsOfType<ICalloutGraphic>))
						{
							if (calloutGraphic.Visible)
							{
								Assert.AreNotEqual(string.Format(SR.ToolsMeasurementFormatDegrees, 0), calloutGraphic.Text, "ShowAnglesToolGraphic should not have spurious 0.0 degree callout with only one line.");
							}
						}
					}
				}
			}
		}

		private static ShowAnglesTool.ShowAnglesToolCompositeGraphic FindShowAnglesToolComposite(IPresentationImage image)
		{
			IOverlayGraphicsProvider imageOverlayGraphics = (IOverlayGraphicsProvider) image;
			IGraphic graphic = imageOverlayGraphics.OverlayGraphics.FirstOrDefault(IsOfType<ShowAnglesTool.ShowAnglesToolCompositeGraphic>);
			if (graphic is CompositeGraphic)
				((CompositeGraphic) graphic).OnDrawing();
			return (ShowAnglesTool.ShowAnglesToolCompositeGraphic) graphic;
		}

		private static ShowAnglesTool.ShowAnglesToolGraphic FindShowAnglesTool(IEnumerable<IGraphic> graphicCollection)
		{
			return (ShowAnglesTool.ShowAnglesToolGraphic) graphicCollection.FirstOrDefault(IsOfType<ShowAnglesTool.ShowAnglesToolGraphic>);
		}

		private static bool IsOfType<T>(object @object)
		{
			return @object is T;
		}

		[Cloneable]
		private class MockPresentationImage : GrayscalePresentationImage
		{
			private static readonly FieldInfo _clientRectangleField = typeof (PresentationImage).GetField("_clientRectangle", BindingFlags.NonPublic | BindingFlags.Instance);

			public MockPresentationImage(int rows, int columns) : base(rows, columns)
			{
				_clientRectangleField.SetValue(this, new Rectangle(0, 0, columns, rows));
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected MockPresentationImage(MockPresentationImage source, ICloningContext context) : base(source, context)
			{
				context.CloneFields(source, this);

				_clientRectangleField.SetValue(this, source.ClientRectangle);
			}
		}

		private class MockShowAnglesTool : ShowAnglesTool
		{
			public MockShowAnglesTool()
			{
				this.ShowAngles = true;
			}

			protected override void OnShowAnglesChanged()
			{
				// prevent actual drawing to the non-existent physical workspace
			}
		}
	}
}

#endif