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

using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that controls the vertices of an <see cref="IPointsGraphic"/> as if it were a closed polygon.
	/// </summary>
	[Cloneable]
	public class PolygonControlGraphic : VerticesControlGraphic
	{
		/// <summary>
		/// Constructs a new <see cref="PolygonControlGraphic"/>.
		/// </summary>
		/// <param name="subject">An <see cref="IPointsGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="IPointsGraphic"/>.</param>
		public PolygonControlGraphic(IGraphic subject)
			: base(subject)
		{
			ResyncEndPoints();
		}

		/// <summary>
		/// Constructs a new <see cref="PolygonControlGraphic"/>.
		/// </summary>
		/// <param name="canAddRemoveVertices">A value indicating whether or not the user can dynamically add or remove vertices on the subject.</param>
		/// <param name="subject">An <see cref="IPointsGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="IPointsGraphic"/>.</param>
		public PolygonControlGraphic(bool canAddRemoveVertices, IGraphic subject)
			: base(canAddRemoveVertices, subject)
		{
			ResyncEndPoints();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected PolygonControlGraphic(PolygonControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		private void ResyncEndPoints()
		{
			IPointsGraphic pointsGraphic = this.Subject;
			if (pointsGraphic.Points.Count > 1)
			{
				pointsGraphic.Points[pointsGraphic.Points.Count - 1] = pointsGraphic.Points[0];
			}
		}

		/// <summary>
		/// Called to insert a vertex at the point where the context menu was last invoked.
		/// </summary>
		protected override void InsertVertex()
		{
			base.InsertVertex();
			ResyncEndPoints();
		}

		/// <summary>
		/// Called to remove the vertex at the point where the context menu was last invoked.
		/// </summary>
		protected override void DeleteVertex()
		{
			base.DeleteVertex();
			ResyncEndPoints();
		}

		/// <summary>
		/// Restores the state of this <see cref="VerticesControlGraphic"/>.
		/// </summary>
		/// <param name="memento">The object that was originally created with <see cref="VerticesControlGraphic.CreateMemento"/>.</param>
		public override void SetMemento(object memento)
		{
			base.SetMemento(memento);
			ResyncEndPoints();
		}

		/// <summary>
		/// Called to notify that a vertex in the subject graphic has changed.
		/// </summary>
		/// <param name="index">The index of the vertex that changed.</param>
		protected override void OnSubjectPointChanged(int index)
		{
			base.OnSubjectPointChanged(index);

			IPointsGraphic pointsGraphic = this.Subject;
			if (pointsGraphic.Points.Count > 1)
			{
				if (index == 0)
					base.OnSubjectPointChanged(pointsGraphic.Points.Count - 1);
				if (index == pointsGraphic.Points.Count - 1)
					base.OnSubjectPointChanged(0);
			}
		}

		/// <summary>
		/// Called to notify the derived class of a control point change event.
		/// </summary>
		/// <param name="index">The index of the point that changed.</param>
		/// <param name="point">The value of the point that changed.</param>
		protected override void OnControlPointChanged(int index, PointF point)
		{
			base.OnControlPointChanged(index, point);

			IPointsGraphic pointsGraphic = this.Subject;
			if (pointsGraphic.Points.Count > 1)
			{
				if (index == 0)
					base.OnControlPointChanged(pointsGraphic.Points.Count - 1, point);
				if (index == pointsGraphic.Points.Count - 1)
					base.OnControlPointChanged(0, point);
			}
		}
	}
}