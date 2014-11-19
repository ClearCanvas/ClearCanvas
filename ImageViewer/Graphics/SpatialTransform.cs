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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Graphics
{
	//TODO: the Matrix fields are not disposed although they are IDisposable

	/// <summary>
	/// Implements a 2D affine transformation
	/// </summary>
	[Cloneable]
	public class SpatialTransform : ISpatialTransform
	{
		#region Private fields

		private bool _updatingScaleParameters;
		private float _scale = 1.0f;
		private float _scaleX = 1.0f;
		private float _scaleY = 1.0f;
		private float _translationX;
		private float _translationY;
		private float _rotationXY;
		private bool _flipX;
		private bool _flipY;
		private PointF _centerOfRotationXY;
		private Matrix _cumulativeTransform;
		private Matrix _transform;
		private bool _recalculationRequired;

		[CloneIgnore]
		private IGraphic _ownerGraphic;

		private SpatialTransformValidationPolicy _validationPolicy;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="SpatialTransform"/>.
		/// </summary>
		public SpatialTransform(IGraphic ownerGraphic)
		{
			_ownerGraphic = ownerGraphic;
			_recalculationRequired = true;
			_updatingScaleParameters = false;
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected SpatialTransform(SpatialTransform source, ICloningContext context)
		{
			context.CloneFields(source, this);

			if (source._cumulativeTransform != null)
				_cumulativeTransform = source._cumulativeTransform.Clone();

			if (source._transform != null)
				_transform = source._transform.Clone();
		}

		#region Properties

		/// <summary>
		/// Gets whether or not we need to recalculate the <see cref="CumulativeTransform"/>.
		/// </summary>
		protected bool RecalculationRequired
		{
			get
			{
				//check if the scale values have changed before returning.
				UpdateScaleInternal();

				if (_recalculationRequired)
				{
					return true;
				}
				else if (OwnerGraphic != null && OwnerGraphic.ParentGraphic != null)
				{
					//If something above us in the hierarchy needs recalculating, so do we.
					return OwnerGraphic.ParentGraphic.SpatialTransform.RecalculationRequired;
				}

				return false;
			}
			private set
			{
				if (value && OwnerGraphic is CompositeGraphic)
				{
					//If we need recalculation, so does everything below us.
					foreach (var graphic in ((CompositeGraphic) OwnerGraphic).Graphics)
						graphic.SpatialTransform.ForceRecalculation();
				}

				_recalculationRequired = value;
			}
		}

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		public float Scale
		{
			get
			{
				UpdateScaleInternal();
				return _scale;
			}
			set
			{
				if (_scale == value)
					return;

				if (value < 0 || FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("value", "Cannot set Scale to zero.");

				_scale = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the scale in the x-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/>.
		/// However, when pixels are non-square, <see cref="ScaleX"/> and <see cref="ScaleY"/>
		/// will differ.  Note that <see cref="ScaleX"/> is not used to account for user flip and is
		/// thus generally positive. It can, however, be used to invert one or both axes to account
		/// for the native orientation of the coordinate system.</remarks>
		protected internal float ScaleX
		{
			get
			{
				UpdateScaleInternal();
				return _scaleX;
			}
			protected set
			{
				if (_scaleX == value)
					return;

				if (FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("value", "Cannot set ScaleX to zero.");

				_scaleX = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the scale in the y-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/>.
		/// However, when pixels are non-square, <see cref="ScaleX"/> and <see cref="ScaleY"/>
		/// will differ.  Note that <see cref="ScaleY"/> is not used to account for user flip and is
		/// thus generally positive. It can, however, be used to invert one or both axes to account
		/// for the native orientation of the coordinate system.</remarks>
		protected internal float ScaleY
		{
			get
			{
				UpdateScaleInternal();
				return _scaleY;
			}
			protected set
			{
				if (_scaleY == value)
					return;

				if (FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("value", "Cannot set ScaleY to zero.");

				_scaleY = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the translation along the x-axis.
		/// </summary>
		public float TranslationX
		{
			get { return _translationX; }
			set
			{
				if (_translationX == value)
					return;

				_translationX = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the translation along the y-axis.
		/// </summary>
		public float TranslationY
		{
			get { return _translationY; }
			set
			{
				if (_translationY == value)
					return;

				_translationY = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether images are flipped vertically
		/// (i.e., the x-axis as the axis of reflection)
		/// </summary>
		public bool FlipX
		{
			get { return _flipX; }
			set
			{
				if (_flipX == value)
					return;

				_flipX = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether images are flipped horizontally
		/// (i.e., the y-axis as the axis of reflection)
		/// </summary>
		public bool FlipY
		{
			get { return _flipY; }
			set
			{
				if (_flipY == value)
					return;

				_flipY = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the rotation in the XY plane in degrees.
		/// </summary>
		/// <remarks>
		/// Values less than 0 or greater than 360 are converted to the equivalent
		/// angle between 0 and 360. For example, -5 becomes 355 and 390 becomes 30.
		/// </remarks>
		public int RotationXY
		{
			get { return (int) _rotationXY; }
			set
			{
				value = value%360;

				if (value < 0)
					value += 360;

				if (_rotationXY == value)
					return;

				_rotationXY = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets or sets the center of rotation.
		/// </summary>
		/// <remarks>
		/// The point should be specified in terms of the coordinate system
		/// of the parent graphic, i.e. source coordinates.
		/// </remarks>
		public PointF CenterOfRotationXY
		{
			get { return _centerOfRotationXY; }
			set
			{
				if (_centerOfRotationXY == value)
					return;

				_centerOfRotationXY = value;
				RecalculationRequired = true;
			}
		}

		/// <summary>
		/// Gets the transform relative to the <see cref="IGraphic"/> object's
		/// immediate parent <see cref="IGraphic"/>.
		/// </summary>
		public Matrix Transform
		{
			get
			{
				if (_transform == null)
					_transform = new Matrix();

				_transform.Reset();
				_transform.Rotate(RotationXY);
				_transform.Scale(ScaleX*(FlipY ? -1 : 1), ScaleY*(FlipX ? -1 : 1));
				_transform.Translate(TranslationX, TranslationY);

				return _transform;
			}
		}

		/// <summary>
		/// Gets the cumulative scale.
		/// </summary>
		/// <remarks>
		/// Gets the scale relative to the root of the scene graph.
		/// </remarks>
		public virtual float CumulativeScale
		{
			get
			{
				float cumulativeScale = Scale;
				if (OwnerGraphic != null && OwnerGraphic.ParentGraphic != null)
					cumulativeScale *= OwnerGraphic.ParentGraphic.SpatialTransform.CumulativeScale;

				return cumulativeScale;
			}
		}

		/// <summary>
		/// Gets the cumulative transform.
		/// </summary>
		/// <remarks>
		/// The <see cref="CumulativeTransform"/> is the product of an
		/// <see cref="IGraphic"/>'s <see cref="Transform"/> and the
		/// <see cref="Transform"/> of all of nodes above it (i.e., all of its
		/// ancestors).
		/// </remarks>
		public Matrix CumulativeTransform
		{
			get
			{
				Calculate();
				return _cumulativeTransform;
			}
		}

		/// <summary>
		/// Gets or sets the associated <see cref="SpatialTransformValidationPolicy"/>.
		/// </summary>
		/// <remarks>
		/// It is not always desirable to allow an <see cref="IGraphic"/> to be transformed
		/// in arbitrary ways.  For example, at present, images can only be rotated in
		/// 90 degree increments.  This property essentially allows a validation policy to be set on a
		/// per graphic basis.  If validation fails, an <see cref="ArgumentException"/> is thrown.
		/// </remarks>
		public SpatialTransformValidationPolicy ValidationPolicy
		{
			get { return _validationPolicy; }
			set { _validationPolicy = value; }
		}

		protected internal IGraphic OwnerGraphic
		{
			get { return _ownerGraphic; }
			internal set { _ownerGraphic = value; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Resets all transform parameters to their defaults.
		/// </summary>
		public void Initialize()
		{
			Reset();
		}

		public void Reset()
		{
			ResetCore();
			ForceRecalculation();
		}

		/// <summary>
		/// Called to reset transform parameters to default values.
		/// </summary>
		protected virtual void ResetCore()
		{
			Scale = 1.0F;
			TranslationX = 0.0F;
			TranslationY = 0.0F;
			RotationXY = 0;
			FlipY = false;
			FlipX = false;
		}

		public void FlipHorizontal()
		{
			switch (RotationXY)
			{
				case 0:
				case 180:
					FlipY = !FlipY;
					return;
				case 90:
				case 270:
					FlipX = !FlipX;
					return;
			}

			var hVector = ConvertToSource(new SizeF(100, 0));
			FlipY = !FlipY;
			if (!FloatComparer.AreEqual(new SizeF(-100, 0), ConvertToDestination(hVector)))
				RotationXY += 180;
		}

		public void FlipVertical()
		{
			switch (RotationXY)
			{
				case 0:
				case 180:
					FlipX = !FlipX;
					return;
				case 90:
				case 270:
					FlipY = !FlipY;
					return;
			}

			var vVector = ConvertToSource(new SizeF(0, 100));
			FlipX = !FlipX;
			if (!FloatComparer.AreEqual(new SizeF(0, -100), ConvertToDestination(vVector)))
				RotationXY += 180;
		}

		public void Rotate(int degrees)
		{
			RotationXY += degrees;
		}

		public void Zoom(float scale)
		{
			Platform.CheckPositive(scale, "scale");
			Scale *= scale;
		}

		public void Translate(float dx, float dy)
		{
			var sourceIncrement = ConvertToSource(new SizeF(dx, dy));
			TranslationX += sourceIncrement.Width;
			TranslationY += sourceIncrement.Height;
		}

		/// <summary>
		/// Converts a <see cref="PointF"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourcePoint"></param>
		/// <returns></returns>
		public PointF ConvertToDestination(PointF sourcePoint)
		{
			PointF[] points = new PointF[1];
			points[0] = sourcePoint;
			this.CumulativeTransform.TransformPoints(points);

			return points[0];
		}

		/// <summary>
		/// Converts a <see cref="PointF"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationPoint"></param>
		/// <returns></returns>
		public PointF ConvertToSource(PointF destinationPoint)
		{
			Matrix inverse = this.CumulativeTransform.Clone();
			inverse.Invert();

			PointF[] points = new PointF[1];
			points[0] = destinationPoint;
			inverse.TransformPoints(points);

			return points[0];
		}

		/// <summary>
		/// Converts a <see cref="RectangleF"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourceRectangle"></param>
		/// <returns></returns>
		public RectangleF ConvertToDestination(RectangleF sourceRectangle)
		{
			PointF topLeft = ConvertToDestination(sourceRectangle.Location);
			PointF bottomRight = ConvertToDestination(new PointF(sourceRectangle.Right, sourceRectangle.Bottom));

			return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
		}

		/// <summary>
		/// Converts a <see cref="RectangleF"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationRectangle"></param>
		/// <returns></returns>
		public RectangleF ConvertToSource(RectangleF destinationRectangle)
		{
			PointF topLeft = ConvertToSource(destinationRectangle.Location);
			PointF bottomRight = ConvertToSource(new PointF(destinationRectangle.Right, destinationRectangle.Bottom));

			return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
		}

		/// <summary>
		/// Converts a <see cref="SizeF"/> from source to destination coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting sizes; this is equivalent
		/// to converting a direction vector, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		public SizeF ConvertToDestination(SizeF sourceDimensions)
		{
			PointF[] transformed = new PointF[] {sourceDimensions.ToPointF()};
			this.CumulativeTransform.TransformVectors(transformed);
			return new SizeF(transformed[0]);
		}

		/// <summary>
		/// Converts a <see cref="SizeF"/> from destination to source coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting sizes; this is equivalent
		/// to converting a direction vector, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		public SizeF ConvertToSource(SizeF destinationDimensions)
		{
			PointF[] transformed = new PointF[] {destinationDimensions.ToPointF()};
			Matrix inverse = this.CumulativeTransform.Clone();
			inverse.Invert();
			inverse.TransformVectors(transformed);
			return new SizeF(transformed[0]);
		}

		#region IMemorable members

		/// <summary>
		/// Creates a memento for this object.
		/// </summary>
		/// <remarks>Typically used in conjunction with <see cref="MemorableUndoableCommand"/>
		/// to support undo/redo.</remarks>
		public virtual object CreateMemento()
		{
			SpatialTransformMemento memento = new SpatialTransformMemento();
			memento.FlipX = this.FlipX;
			memento.FlipY = this.FlipY;
			memento.RotationXY = this.RotationXY;
			memento.Scale = this.Scale;
			memento.TranslationX = this.TranslationX;
			memento.TranslationY = this.TranslationY;

			return memento;
		}

		/// <summary>
		/// Sets a memento for this object.
		/// </summary>
		/// <remarks>Typically used in conjunction with <see cref="MemorableUndoableCommand"/>
		/// to support undo/redo.</remarks>
		/// <exception cref="ArgumentNullException"><b>memento</b>
		/// is <b>null</b>.</exception>
		/// <exception cref="InvalidCastException"><b>memento</b>
		/// is not of the type expected by the object.</exception>
		public virtual void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			SpatialTransformMemento spatialTransformMemento = (SpatialTransformMemento) memento;

			this.FlipX = spatialTransformMemento.FlipX;
			this.FlipY = spatialTransformMemento.FlipY;
			this.RotationXY = spatialTransformMemento.RotationXY;
			this.Scale = spatialTransformMemento.Scale;
			this.TranslationX = spatialTransformMemento.TranslationX;
			this.TranslationY = spatialTransformMemento.TranslationY;

			this.RecalculationRequired = true;
		}

		#endregion

		#endregion

		#region Protected methods

		/// <summary>
		/// Forces the <see cref="CumulativeTransform"/> to be recalculated.
		/// </summary>
		protected internal void ForceRecalculation()
		{
			RecalculationRequired = true;
		}

		/// <summary>
		/// Calculates the cumulative transform.
		/// </summary>
		/// <remarks>Once this method is executed, the <see cref="CumulativeTransform"/>
		/// property will reflect any changes in the transform parameters.</remarks>
		protected virtual void Calculate()
		{
			if (!this.RecalculationRequired)
				return;

			// The cumulative transform is the product of the transform of the
			// parent graphic and the transform of this graphic (i.e. the current transform)
			// If there is no parent graphic, then the cumulative transform = current transform
			if (_cumulativeTransform == null)
				_cumulativeTransform = new Matrix();

			_cumulativeTransform.Reset();

			IGraphic parentGraphic = OwnerGraphic != null ? OwnerGraphic.ParentGraphic : null;
			if (parentGraphic != null)
				_cumulativeTransform.Multiply(parentGraphic.SpatialTransform.CumulativeTransform);

			CalculatePreTransform(_cumulativeTransform);
			_cumulativeTransform.Multiply(this.Transform);
			CalculatePostTransform(_cumulativeTransform);

			this.RecalculationRequired = false;

			// Validate if there's a validation policy in place.  Otherwise, assume all is good.
			if (_validationPolicy != null)
				_validationPolicy.Validate(this);
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a pre-transform transformation.
		/// </summary>
		/// <param name="cumulativeTransform"></param>
		protected virtual void CalculatePreTransform(Matrix cumulativeTransform)
		{
			cumulativeTransform.Translate(_centerOfRotationXY.X, _centerOfRotationXY.Y);
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a post-transform transformation.
		/// </summary>
		protected virtual void CalculatePostTransform(Matrix cumulativeTransform)
		{
			cumulativeTransform.Translate(-_centerOfRotationXY.X, -_centerOfRotationXY.Y);
		}

		/// <summary>
		/// Gives derived classes an opportunity to update the scale parameters.
		/// </summary>
		/// <remarks>
		/// By default, sets <see cref="ScaleX"/> and <see cref="ScaleY"/> to the value of <see cref="Scale"/>.
		/// </remarks>
		protected virtual void UpdateScaleParameters()
		{
			float scale = Scale;
			this.ScaleX = scale;
			this.ScaleY = scale;
		}

		private void UpdateScaleInternal()
		{
			if (_updatingScaleParameters)
				return;

			_updatingScaleParameters = true;
			try
			{
				UpdateScaleParameters();
			}
			finally
			{
				_updatingScaleParameters = false;
			}
		}

		#endregion
	}
}