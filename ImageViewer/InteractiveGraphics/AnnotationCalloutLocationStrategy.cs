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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A default implementation of <see cref="IAnnotationCalloutLocationStrategy"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This implementation sets the initial callout position to be a location offset
	/// from the top left corner of the ROI measurement's bounding box. Once set, the
	/// callout is not moved along with the measurement.
	/// </para>
	/// <para>
	/// Implementors of <see cref="IAnnotationCalloutLocationStrategy"/> may wish to
	/// derive from this class to take advantage of built-in functionality and only
	/// override methods to implement the desired strategy.
	/// </para>
	/// </remarks>
	[Cloneable(true)]
	public class AnnotationCalloutLocationStrategy : IAnnotationCalloutLocationStrategy
	{
		[CloneIgnore]
		private IAnnotationGraphic _annotationGraphic;

		private bool _initialLocationSet;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AnnotationCalloutLocationStrategy()
		{
			_initialLocationSet = false;
		}

		/// <summary>
		/// Releases unmanaged resources held by this object.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}
		}

		/// <summary>
		/// Gets the owning <see cref="AnnotationGraphic"/>.
		/// </summary>
		protected IAnnotationGraphic AnnotationGraphic
		{
			get { return _annotationGraphic; }
		}

		[Obsolete("Access this via AnnotationSubject.")]
		protected IGraphic Roi
		{
			get { return _annotationGraphic.Subject; }
		}

		protected IGraphic AnnotationSubject
		{
			get { return _annotationGraphic.Subject; }
		}

		/// <summary>
		/// Gets the <see cref="AnnotationGraphic"/>'s Callout.
		/// </summary>
		protected ICalloutGraphic Callout
		{
			get { return _annotationGraphic.Callout; }
		}

		/// <summary>
		/// Called to release any unmanaged resources held by this object.
		/// </summary>
		/// <param name="disposing">True if <see cref="IDisposable.Dispose"/> was called; False if the object is being finalized.</param>
		protected virtual void Dispose(bool disposing) {}

		/// <summary>
		/// Called when the <see cref="AnnotationGraphic"/> changes.
		/// </summary>
		/// <param name="oldAnnotationGraphic">The former value of <see cref="AnnotationGraphic"/>.</param>
		/// <param name="annotationGraphic">The new value of <see cref="AnnotationGraphic"/>.</param>
		protected virtual void OnAnnotationGraphicChanged(IAnnotationGraphic oldAnnotationGraphic, IAnnotationGraphic annotationGraphic) {}

		#region IRoiCalloutLocationStrategy Members

		/// <summary>
		/// Sets the <see cref="AnnotationGraphic"/> that owns this strategy.
		/// </summary>
		public void SetAnnotationGraphic(IAnnotationGraphic annotationGraphic)
		{
			if (_annotationGraphic != annotationGraphic)
			{
				var oldAnnotationGraphic = _annotationGraphic;
				_annotationGraphic = annotationGraphic;
				OnAnnotationGraphicChanged(oldAnnotationGraphic, annotationGraphic);
			}
		}

		/// <summary>
		/// Called when the <see cref="IAnnotationGraphic"/>'s callout location has been changed externally; for example, by the user.
		/// </summary>
		public virtual void OnCalloutLocationChangedExternally() {}

		/// <summary>
		/// Calculates the initial callout location only; returns false thereafter.
		/// </summary>
		public virtual bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem)
		{
			location = PointF.Empty;
			coordinateSystem = CoordinateSystem.Destination;

			if (!_initialLocationSet)
			{
				_initialLocationSet = true;

				SizeF offset = new SizeF(0, 55);

				// Setup the callout
				AnnotationSubject.CoordinateSystem = CoordinateSystem.Destination;
				location = RectangleUtilities.ConvertToPositiveRectangle(AnnotationSubject.BoundingBox).Location - offset;
				AnnotationSubject.ResetCoordinateSystem();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Calculates the callout endpoint using the <see cref="IGraphic.GetClosestPoint"/> method.
		/// </summary>
		public virtual void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem)
		{
			coordinateSystem = AnnotationGraphic.CoordinateSystem;
			endPoint = AnnotationGraphic.Subject.GetClosestPoint(AnnotationGraphic.Callout.TextLocation);
		}

		/// <summary>
		/// Creates a deep copy of this strategy object.
		/// </summary>
		/// <remarks>
		/// Implementations should never return null from this method.
		/// </remarks>
		public IAnnotationCalloutLocationStrategy Clone()
		{
			return CloneBuilder.Clone(this) as IAnnotationCalloutLocationStrategy;
		}

		#endregion
	}
}