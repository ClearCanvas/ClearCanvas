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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics3D
{
	[Cloneable]
	public class SpatialTransform3D : ISpatialTransform3D
	{
		private bool _updatingScaleParameters;
		private float _scale = 1.0f;
		private float _scaleX = 1.0f;
		private float _scaleY = 1.0f;
		private float _scaleZ = 1.0f;
		private float _translationX;
		private float _translationY;
		private float _translationZ;
		private Matrix3D _rotation = Matrix3D.GetIdentity();
		private bool _flipYz;
		private bool _flipXz;
		private bool _flipXy;
		private Vector3D _centerOfRotation = Vector3D.Null;
		private Matrix _cumulativeTransform;
		private Matrix _transform;
		private bool _recalculationRequired;

		[CloneIgnore]
		private IGraphic3D _ownerGraphic;

		public SpatialTransform3D(IGraphic3D ownerGraphic)
		{
			_ownerGraphic = ownerGraphic;
			_recalculationRequired = true;
			_updatingScaleParameters = false;
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected SpatialTransform3D(SpatialTransform3D source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_centerOfRotation = source._centerOfRotation ?? Vector3D.Null;

			if (source._rotation != null)
				_rotation = source._rotation.Clone();

			if (source._cumulativeTransform != null)
				_cumulativeTransform = source._cumulativeTransform.Clone();

			if (source._transform != null)
				_transform = source._transform.Clone();
		}

		protected internal IGraphic3D OwnerGraphic
		{
			get { return _ownerGraphic; }
			internal set { _ownerGraphic = value; }
		}

		public bool FlipYZ
		{
			get { return _flipYz; }
			set
			{
				if (_flipYz == value)
					return;

				_flipYz = value;
				ForceRecalculation();
			}
		}

		public bool FlipXZ
		{
			get { return _flipXz; }
			set
			{
				if (_flipXz == value)
					return;

				_flipXz = value;
				ForceRecalculation();
			}
		}

		public bool FlipXY
		{
			get { return _flipXy; }
			set
			{
				if (_flipXy == value)
					return;

				_flipXy = value;
				ForceRecalculation();
			}
		}

		public Matrix3D Rotation
		{
			get { return _rotation; }
			set
			{
				if (_rotation == value)
					return;

				_rotation = value ?? Matrix3D.GetIdentity();
				ForceRecalculation();
			}
		}

		public float Scale
		{
			get
			{
				UpdateScaleInternal();
				return _scale;
			}
			set
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_scale == value)
					return;
				// ReSharper restore CompareOfFloatsByEqualityOperator

				if (value < 0 || FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("value", string.Format("Cannot set {0} to zero.", "Scale"));

				_scale = value;
				ForceRecalculation();
			}
		}

		/// <summary>
		/// Gets or sets the scale in the x-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/> = <see cref="ScaleZ"/>.
		/// However, when voxels are non-square, <see cref="ScaleX"/>, <see cref="ScaleY"/> and <see cref="ScaleZ"/>
		/// will differ.  Note that <see cref="ScaleX"/> is not used to account for user flip and is
		/// thus generally positive. It can, however, be used to invert one or more axes to account
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
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_scaleX == value)
					return;
				// ReSharper restore CompareOfFloatsByEqualityOperator

				if (FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("value", string.Format("Cannot set {0} to zero.", "ScaleX"));

				_scaleX = value;
				ForceRecalculation();
			}
		}

		/// <summary>
		/// Gets or sets the scale in the y-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/> = <see cref="ScaleZ"/>.
		/// However, when voxels are non-square, <see cref="ScaleX"/>, <see cref="ScaleY"/> and <see cref="ScaleZ"/>
		/// will differ.  Note that <see cref="ScaleY"/> is not used to account for user flip and is
		/// thus generally positive. It can, however, be used to invert one or more axes to account
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
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_scaleY == value)
					return;
				// ReSharper restore CompareOfFloatsByEqualityOperator

				if (FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("value", string.Format("Cannot set {0} to zero.", "ScaleY"));

				_scaleY = value;
				ForceRecalculation();
			}
		}

		/// <summary>
		/// Gets or sets the scale in the z-direction.
		/// </summary>
		/// <remarks>Usually, <see cref="Scale"/> = <see cref="ScaleX"/> = <see cref="ScaleY"/> = <see cref="ScaleZ"/>.
		/// However, when voxels are non-square, <see cref="ScaleX"/>, <see cref="ScaleY"/> and <see cref="ScaleZ"/>
		/// will differ.  Note that <see cref="ScaleZ"/> is not used to account for user flip and is
		/// thus generally positive. It can, however, be used to invert one or more axes to account
		/// for the native orientation of the coordinate system.</remarks>
		protected internal float ScaleZ
		{
			get
			{
				UpdateScaleInternal();
				return _scaleZ;
			}
			protected set
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_scaleZ == value)
					return;
				// ReSharper restore CompareOfFloatsByEqualityOperator

				if (FloatComparer.AreEqual(value, 0F))
					throw new ArgumentOutOfRangeException("value", string.Format("Cannot set {0} to zero.", "ScaleZ"));

				_scaleZ = value;
				ForceRecalculation();
			}
		}

		public float TranslationX
		{
			get { return _translationX; }
			set
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_translationX == value)
					return;
				// ReSharper restore CompareOfFloatsByEqualityOperator

				_translationX = value;
				ForceRecalculation();
			}
		}

		public float TranslationY
		{
			get { return _translationY; }
			set
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_translationY == value)
					return;
				// ReSharper restore CompareOfFloatsByEqualityOperator

				_translationY = value;
				ForceRecalculation();
			}
		}

		public float TranslationZ
		{
			get { return _translationZ; }
			set
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_translationZ == value)
					return;
				// ReSharper restore CompareOfFloatsByEqualityOperator

				_translationZ = value;
				ForceRecalculation();
			}
		}

		public Vector3D CenterOfRotation
		{
			get { return _centerOfRotation; }
			set
			{
				if (_centerOfRotation == value)
					return;

				_centerOfRotation = value ?? Vector3D.Null;
				ForceRecalculation();
			}
		}

		public virtual float CumulativeScale
		{
			get
			{
				var cumulativeScale = Scale;
				if (OwnerGraphic != null && OwnerGraphic.ParentGraphic != null)
					cumulativeScale *= OwnerGraphic.ParentGraphic.SpatialTransform.CumulativeScale;
				return cumulativeScale;
			}
		}

		public virtual object CreateMemento()
		{
			return new Memento
			       {
				       FlipXY = FlipXY,
				       FlipXZ = FlipXZ,
				       FlipYZ = FlipYZ,
				       Rotation = Rotation.Clone(),
				       Scale = Scale,
				       TranslationX = TranslationX,
				       TranslationY = TranslationY,
				       TranslationZ = TranslationZ
			       };
		}

		public virtual void SetMemento(object memento)
		{
			var memento3D = memento as Memento;
			if (memento3D == null) return;

			FlipXY = memento3D.FlipXY;
			FlipXZ = memento3D.FlipXZ;
			FlipYZ = memento3D.FlipYZ;
			Rotation = memento3D.Rotation != null ? memento3D.Rotation.Clone() : null;
			Scale = memento3D.Scale;
			TranslationX = memento3D.TranslationX;
			TranslationY = memento3D.TranslationY;
			TranslationZ = memento3D.TranslationZ;
			ForceRecalculation();
		}

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
				if (value && OwnerGraphic is CompositeGraphic3D)
				{
					//If we need recalculation, so does everything below us.
					foreach (Graphic3D graphic in ((CompositeGraphic3D) OwnerGraphic).Graphics)
						graphic.SpatialTransform.ForceRecalculation();
				}

				_recalculationRequired = value;
			}
		}

		/// <summary>
		/// Gets the transform relative to the <see cref="IGraphic3D"/> object's
		/// immediate parent <see cref="IGraphic3D"/>.
		/// </summary>
		public Matrix Transform
		{
			get
			{
				_transform = Matrix.GetIdentity(4);
				_transform.Rotate(Rotation);
				_transform.Scale(ScaleX*(FlipYZ ? -1 : 1), ScaleY*(FlipXZ ? -1 : 1), ScaleZ*(FlipXY ? -1 : 1));
				_transform.Translate(TranslationX, TranslationY, TranslationZ);
				return _transform;
			}
		}

		/// <summary>
		/// Gets the cumulative transform.
		/// </summary>
		/// <remarks>
		/// The <see cref="CumulativeTransform"/> is the product of an
		/// <see cref="IGraphic3D"/>'s <see cref="Transform"/> and the
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
			TranslationZ = 0.0F;
			Rotation = null;
			FlipYZ = false;
			FlipXZ = false;
			FlipXY = false;
		}

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
			if (!RecalculationRequired)
				return;

			_cumulativeTransform = Matrix.GetIdentity(4);

			// The cumulative transform is the product of the transform of the
			// parent graphic and the transform of this graphic (i.e. the current transform)
			// If there is no parent graphic, then the cumulative transform = current transform
			var parentGraphic = OwnerGraphic != null ? OwnerGraphic.ParentGraphic : null;
			if (parentGraphic != null)
				_cumulativeTransform.Multiply(parentGraphic.SpatialTransform.CumulativeTransform);

			CalculatePreTransform(_cumulativeTransform);
			_cumulativeTransform.Multiply(Transform);
			CalculatePostTransform(_cumulativeTransform);

			RecalculationRequired = false;

			//// Validate if there's a validation policy in place.  Otherwise, assume all is good.
			//if (_validationPolicy != null)
			//    _validationPolicy.Validate(this);
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a pre-transform transformation.
		/// </summary>
		/// <param name="cumulativeTransform"></param>
		protected virtual void CalculatePreTransform(Matrix cumulativeTransform)
		{
			var cor = CenterOfRotation;
			cumulativeTransform.Translate(cor.X, cor.Y, cor.Z);
		}

		/// <summary>
		/// Gives subclasses an opportunity to perform a post-transform transformation.
		/// </summary>
		protected virtual void CalculatePostTransform(Matrix cumulativeTransform)
		{
			var cor = CenterOfRotation;
			cumulativeTransform.Translate(-cor.X, -cor.Y, -cor.Z);
		}

		/// <summary>
		/// Gives derived classes an opportunity to update the scale parameters.
		/// </summary>
		/// <remarks>
		/// By default, sets <see cref="ScaleX"/> and <see cref="ScaleY"/> to the value of <see cref="Scale"/>.
		/// </remarks>
		protected virtual void UpdateScaleParameters()
		{
			var scale = Scale;
			ScaleX = scale;
			ScaleY = scale;
			ScaleZ = scale;
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

		#region Coordinate Conversion Methods

		/// <summary>
		/// Converts a <see cref="Vector3D"/> point from source to destination coordinates.
		/// </summary>
		/// <param name="sourcePoint"></param>
		/// <returns></returns>
		public Vector3D ConvertPointToDestination(Vector3D sourcePoint)
		{
			return CumulativeTransform.TransformPoint(sourcePoint);
		}

		/// <summary>
		/// Converts a <see cref="Vector3D"/> point from destination to source coordinates.
		/// </summary>
		/// <param name="destinationPoint"></param>
		/// <returns></returns>
		public Vector3D ConvertPointToSource(Vector3D destinationPoint)
		{
			return CumulativeTransform.Invert().TransformPoint(destinationPoint);
		}

		/// <summary>
		/// Converts a <see cref="Rectangle3D"/> from source to destination coordinates.
		/// </summary>
		/// <param name="sourceRectangle"></param>
		/// <returns></returns>
		public Rectangle3D ConvertRectToDestination(Rectangle3D sourceRectangle)
		{
			var transform = CumulativeTransform;
			var topLeft = transform.TransformPoint(sourceRectangle.Location);
			var bottomRight = transform.TransformPoint(new Vector3D(sourceRectangle.Right, sourceRectangle.Bottom, sourceRectangle.Back));
			return new Rectangle3D(topLeft.X, topLeft.Y, topLeft.Z, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y, bottomRight.Z - topLeft.Z);
		}

		/// <summary>
		/// Converts a <see cref="Rectangle3D"/> from destination to source coordinates.
		/// </summary>
		/// <param name="destinationRectangle"></param>
		/// <returns></returns>
		public Rectangle3D ConvertRectToSource(Rectangle3D destinationRectangle)
		{
			var transform = CumulativeTransform.Invert();
			var topLeft = transform.TransformPoint(destinationRectangle.Location);
			var bottomRight = transform.TransformPoint(new Vector3D(destinationRectangle.Right, destinationRectangle.Bottom, destinationRectangle.Back));
			return new Rectangle3D(topLeft.X, topLeft.Y, topLeft.Z, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y, bottomRight.Z - topLeft.Z);
		}

		/// <summary>
		/// Converts a <see cref="Vector3D"/> vector from source to destination coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting vectors, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		public Vector3D ConvertVectorToDestination(Vector3D sourceVector)
		{
			return CumulativeTransform.TransformVector(sourceVector);
		}

		/// <summary>
		/// Converts a <see cref="Vector3D"/> vector from destination to source coordinates.
		/// </summary>
		/// <remarks>
		/// Only scale and rotation are applied when converting vectors, as direction vectors have only magnitude
		/// and direction information, but no position.
		/// </remarks>
		public Vector3D ConvertVectorToSource(Vector3D destinationVector)
		{
			return CumulativeTransform.Invert().TransformVector(destinationVector);
		}

		#endregion

		#region Memento Class

		private class Memento : IEquatable<Memento>
		{
// ReSharper disable InconsistentNaming
			public float Scale { get; set; }
			public float TranslationX { get; set; }
			public float TranslationY { get; set; }
			public float TranslationZ { get; set; }
			public bool FlipXY { get; set; }
			public bool FlipXZ { get; set; }
			public bool FlipYZ { get; set; }
			public Matrix3D Rotation { get; set; }
// ReSharper restore InconsistentNaming

			public override int GetHashCode()
			{
				// hash code is used in some cases to short circuit calls to Equals, so at the very least return a non-zero constant even if it will never be used to key a hash table
				return -0x3C816414;
			}

			public bool Equals(Memento other)
			{
// ReSharper disable CompareOfFloatsByEqualityOperator
				return other != null &&
				       Scale == other.Scale &&
				       TranslationX == other.TranslationX &&
				       TranslationY == other.TranslationY &&
				       TranslationZ == other.TranslationZ &&
				       FlipXY == other.FlipXY &&
				       FlipXZ == other.FlipXZ &&
				       FlipYZ == other.FlipYZ &&
				       Matrix3D.AreEqual(Rotation, other.Rotation);
// ReSharper restore CompareOfFloatsByEqualityOperator
			}

			public override bool Equals(object obj)
			{
				return obj == this || Equals(obj as Memento);
			}
		}

		#endregion
	}
}