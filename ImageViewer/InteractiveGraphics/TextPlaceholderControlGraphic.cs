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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that displays a placeholder control point when the value of the subject's <see cref="ITextGraphic.Text"/> is empty or null.
	/// </summary>
	[Cloneable]
	public class TextPlaceholderControlGraphic : ControlPointsGraphic
	{
		/// <summary>
		/// Constructs a new <see cref="TextPlaceholderControlGraphic"/>.
		/// </summary>
		/// <param name="subject">An <see cref="ITextGraphic"/> or an <see cref="IControlGraphic"/> chain whose subject is an <see cref="ITextGraphic"/>.</param>
		public TextPlaceholderControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (ITextGraphic));

			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected TextPlaceholderControlGraphic(TextPlaceholderControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.VisualStateChanged += OnSubjectVisualStateChanged;
		}

		/// <summary>
		/// Releases all resources used by this <see cref="IControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			this.Subject.VisualStateChanged -= OnSubjectVisualStateChanged;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		public new ITextGraphic Subject
		{
			get { return base.Subject as ITextGraphic; }
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public override string CommandName
		{
			get { return SR.CommandChange; }
		}

		/// <summary>
		/// Captures the current state of this <see cref="TextPlaceholderControlGraphic"/>.
		/// </summary>
		public override object CreateMemento()
		{
			PointMemento pointMemento;

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				pointMemento = new PointMemento(this.Subject.Location);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}

			return pointMemento;
		}

		/// <summary>
		/// Restores the state of this <see cref="TextPlaceholderControlGraphic"/>.
		/// </summary>
		/// <param name="memento">The object that was originally created with <see cref="TextPlaceholderControlGraphic.CreateMemento"/>.</param>
		public override void SetMemento(object memento)
		{
			PointMemento pointMemento = memento as PointMemento;
			if (pointMemento == null)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Subject.Location = pointMemento.Point;
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}
		}

		private void OnSubjectVisualStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				base.ControlPoints.Clear();
				if (string.IsNullOrEmpty(this.Subject.Text))
				{
					base.ControlPoints.Add(this.Subject.Location);
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		/// <summary>
		/// Called to notify the derived class of a control point change event.
		/// </summary>
		/// <param name="index">The index of the point that changed.</param>
		/// <param name="point">The value of the point that changed.</param>
		protected override void OnControlPointChanged(int index, PointF point)
		{
			this.Subject.Location = point;
			this.Draw();
			base.OnControlPointChanged(index, point);
		}
	}
}