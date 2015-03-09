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

namespace ClearCanvas.ImageViewer.Graphics3D
{
	/// <summary>
	/// A <see cref="SpatialTransform"/> specific to the root model of the scene.
	/// </summary>
	/// <remarks>
	/// This <see cref="SpatialTransform"/> maps the transformation of the 3D scene
	/// as a projection on to the plane of the view port, thereby bridging
	/// the 3D and 2D coordinate systems.
	/// </remarks>
	[Cloneable]
	internal class RootModelSpatialTransform : SpatialTransform, IImageSpatialTransform
	{
		private readonly RootSpatialTransform3D _rootTransform3D;
		private readonly Size _sceneSize;
		private Rectangle _clientRectangle;
		private float _effectiveZ;
		private bool _scaleToFit = true;
		private Matrix _cumulativeTransform3D;

		public RootModelSpatialTransform(IGraphic ownerGraphic, Size sceneSize, Vector3D dimensions, Vector3D spacing, Vector3D aspectRatio)
			: base(ownerGraphic)
		{
			_sceneSize = sceneSize;
			_rootTransform3D = new RootSpatialTransform3D(this, dimensions, spacing, aspectRatio);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected RootModelSpatialTransform(RootModelSpatialTransform source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);

			_rootTransform3D.ParentTransform = this;

			if (source._cumulativeTransform3D != null)
				_cumulativeTransform3D = source._cumulativeTransform3D.Clone();
		}

		/// <summary>
		/// Gets the 3D spatial transform of the root model.
		/// </summary>
		public SpatialTransform3D SpatialTransform3D
		{
			get { return _rootTransform3D; }
		}

		/// <summary>
		/// Gets or sets the client rectangle of the view port.
		/// </summary>
		protected Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (_clientRectangle == value) return;
				_clientRectangle = value;
				ForceRecalculation();
			}
		}

		/// <summary>
		/// Gets or sets the origin of the root model in the scene.
		/// </summary>
		public Vector3D ModelOrigin
		{
			get { return _rootTransform3D.ModelOrigin; }
			set { _rootTransform3D.ModelOrigin = value; }
		}

		public float EffectiveZ
		{
			get { return _effectiveZ; }
			set
			{
// ReSharper disable CompareOfFloatsByEqualityOperator
				if (_effectiveZ == value) return;
				_effectiveZ = value;
				ForceRecalculation();
// ReSharper restore CompareOfFloatsByEqualityOperator
			}
		}

		public bool ScaleToFit
		{
			get { return _scaleToFit; }
			set
			{
				if (_scaleToFit == value) return;
				_scaleToFit = value;
				ForceRecalculation();
			}
		}

		protected Matrix EffectiveCumulativeTransform3D
		{
			get
			{
				Calculate();
				return _cumulativeTransform3D;
			}
		}

		public override float CumulativeScale
		{
			get { return _rootTransform3D.CumulativeScale; }
		}

		protected override void ResetCore()
		{
			base.ResetCore();
			ScaleToFit = true;
		}

		protected override void UpdateScaleParameters()
		{
			if (OwnerGraphic != null && OwnerGraphic.ParentPresentationImage != null)
				ClientRectangle = OwnerGraphic.ParentPresentationImage.ClientRectangle;

			if (!RecalculationRequired) return;

			// N.B.: no adjustment for anisotropic voxel spacing needed here.
			// The 3D transform and renderer is responsible for taking that into account,
			// so the result is projected on the view plane with isotropic spacing.
			float scale;

			if (ScaleToFit)
			{
				//don't ever calculate the 'scale to fit' for an empty rectangle.
				var destinationWidth = Math.Max(10, ClientRectangle.Width);
				var destinationHeight = Math.Max(10, ClientRectangle.Height);

				// compute the correct scale to fit based on current view rotation
				var dimensions = new PointF(_sceneSize.Width, _sceneSize.Height);
				if (RotationXY == 90 || RotationXY == 270)
				{
					var imageAspectRatio = dimensions.X/dimensions.Y;
					var clientAspectRatio = (float) destinationHeight/destinationWidth;
					scale = clientAspectRatio >= imageAspectRatio ? destinationWidth/dimensions.Y : destinationHeight/dimensions.X;
				}
				else
				{
					var imageAspectRatio = dimensions.Y/dimensions.X;
					var clientAspectRatio = (float) destinationHeight/destinationWidth;
					scale = clientAspectRatio >= imageAspectRatio ? destinationWidth/dimensions.X : destinationHeight/dimensions.Y;
				}

				// update the Scale property so that turning off scale to fit does not create a discontinuity in scale
				Scale = scale;
			}
			else
			{
				scale = Scale;
			}

			// the tile's +Y axis points down, while the 3D model's +Y axis points up
			ScaleX = scale;
			ScaleY = -scale;
		}

		protected override void Calculate()
		{
			if (!RecalculationRequired) return;

			base.Calculate();

			// the elements of the .NET 2D Drawing Matrix are arranged as such:
			//                      [  a  b 0 ]      [ a c Tx ]**T
			// [a b c d Tx Ty] <==> [  c  d 0 ] <==> [ b d Ty ]
			//                      [ Tx Ty 1 ]      [ 0 0  1 ]
			var transform2D = CumulativeTransform.Elements;

			// when we augment the 2D matrix for 3D space, we place the elements as such:
			// [ a c 0 Tx ]
			// [ b d 0 Ty ]
			// [ 0 0 1 Z0 ] where Z0 is the effective Z at which the 2D plane will be fixed
			// [ 0 0 0  1 ]
			_cumulativeTransform3D = Matrix.GetIdentity(4);
			_cumulativeTransform3D[0, 0] = transform2D[0];
			_cumulativeTransform3D[1, 0] = transform2D[1];
			_cumulativeTransform3D[0, 1] = transform2D[2];
			_cumulativeTransform3D[1, 1] = transform2D[3];
			_cumulativeTransform3D[0, 3] = transform2D[4];
			_cumulativeTransform3D[1, 3] = transform2D[5];
			_cumulativeTransform3D[2, 3] = _effectiveZ;
		}

		protected override void CalculatePostTransform(System.Drawing.Drawing2D.Matrix cumulativeTransform)
		{
			// no post-transform step - the origin of the view port source (3D model root projected on to the view plane) is the center of the projected image
		}

		protected override void CalculatePreTransform(System.Drawing.Drawing2D.Matrix cumulativeTransform)
		{
			cumulativeTransform.Translate(ClientRectangle.Width/2.0f, ClientRectangle.Height/2.0f);
		}

		public override object CreateMemento()
		{
			return new ImageSpatialTransformMemento(ScaleToFit, base.CreateMemento());
		}

		public override void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			var imageSpatialTransformMemento = (ImageSpatialTransformMemento) memento;
			ScaleToFit = imageSpatialTransformMemento.ScaleToFit;
			base.SetMemento(imageSpatialTransformMemento.SpatialTransformMemento);
		}

		#region RootSpatialTransform3D Class

		/// <summary>
		/// A <see cref="SpatialTransform3D"/> specific to the root model of the scene.
		/// </summary>
		/// <remarks>
		/// This <see cref="SpatialTransform3D"/> maps the transformation of the 3D scene
		/// as a projection on to the plane of the view port, thereby modelling the transition
		/// between the 3D and 2D coordinate systems.
		/// </remarks>
		[Cloneable]
		protected class RootSpatialTransform3D : SpatialTransform3D
		{
			#region Private Fields

			[CloneIgnore]
			private RootModelSpatialTransform _parentTransform;

			[CloneCopyReference]
			private readonly Vector3D _dimensions;

			[CloneCopyReference]
			private readonly Vector3D _spacing;

			[CloneCopyReference]
			private readonly Vector3D _adjustedDimensions;

			[CloneCopyReference]
			private Vector3D _modelOrigin = Vector3D.Null;

			#endregion

			/// <summary>
			/// Initializes a new instance of <see cref="RootSpatialTransform3D"/> with
			/// the specified owner <see cref="IGraphic"/>, dimensions, spacing
			/// and aspect ratio.
			/// </summary>
			/// <param name="parentTransform"></param>
			/// <param name="dimensionX"></param>
			/// <param name="dimensionY"></param>
			/// <param name="dimensionZ"></param>
			/// <param name="spacingX"></param>
			/// <param name="spacingY"></param>
			/// <param name="spacingZ"></param>
			/// <param name="aspectRatioX"></param>
			/// <param name="aspectRatioY"></param>
			/// <param name="aspectRatioZ"></param>
			public RootSpatialTransform3D(RootModelSpatialTransform parentTransform,
			                              float dimensionX, float dimensionY, float dimensionZ,
			                              float spacingX, float spacingY, float spacingZ,
			                              float aspectRatioX, float aspectRatioY, float aspectRatioZ)
				: this(parentTransform, new Vector3D(dimensionX, dimensionY, dimensionZ), new Vector3D(spacingX, spacingY, spacingZ), new Vector3D(aspectRatioX, aspectRatioY, aspectRatioZ)) {}

			/// <summary>
			/// Initializes a new instance of <see cref="RootSpatialTransform3D"/> with
			/// the specified owner <see cref="IGraphic"/>, dimensions, spacing
			/// and aspect ratio.
			/// </summary>
			/// <param name="parentTransform"></param>
			/// <param name="dimensions"></param>
			/// <param name="spacing"></param>
			/// <param name="aspectRatio"></param>
			public RootSpatialTransform3D(RootModelSpatialTransform parentTransform, Vector3D dimensions, Vector3D spacing, Vector3D aspectRatio)
				: base(null)
			{
				Platform.CheckForNullReference(dimensions, "dimensions");

				if (spacing != null && (FloatComparer.AreEqual(0, spacing.X) || FloatComparer.AreEqual(0, spacing.Y) || FloatComparer.AreEqual(0, spacing.Z)))
					spacing = null;

				if (aspectRatio != null && (FloatComparer.AreEqual(0, aspectRatio.X) || FloatComparer.AreEqual(0, aspectRatio.Y) || FloatComparer.AreEqual(0, aspectRatio.Z)))
					aspectRatio = null;

				_parentTransform = parentTransform;
				_dimensions = dimensions;
				_spacing = spacing;
				_adjustedDimensions = ComputeAdjustedDimensions(dimensions, spacing, aspectRatio);
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			protected RootSpatialTransform3D(RootSpatialTransform3D source, ICloningContext context)
				: base(source, context)
			{
				context.CloneFields(source, this);
			}

			/// <summary>
			/// Gets or sets the origin of the root model.
			/// </summary>
			public Vector3D ModelOrigin
			{
				get { return _modelOrigin; }
				set
				{
					if (value == null) value = Vector3D.Null;
					if (Equals(_modelOrigin, value)) return;
					_modelOrigin = value;
					ForceRecalculation();
				}
			}

			/// <summary>
			/// Gets the transform of the view port.
			/// </summary>
			public RootModelSpatialTransform ParentTransform
			{
				get { return _parentTransform; }
				internal set { _parentTransform = value; }
			}

			/// <summary>
			/// Gets raw dimensions of the root model.
			/// </summary>
			protected Vector3D SourceDimensions
			{
				get { return _dimensions; }
			}

			/// <summary>
			/// Gets the dimensions of the root model, adjusted to account for anisotropic spacing in the model.
			/// </summary>
			protected internal Vector3D AdjustedSourceDimensions
			{
				get { return _adjustedDimensions; }
			}

			/// <summary>
			/// Gets the spacing in mm/voxel of the model.
			/// </summary>
			protected Vector3D SourceSpacing
			{
				get { return _spacing; }
			}

			public override float CumulativeScale
			{
				get
				{
					var cumulativeScale = Scale;
					if (_parentTransform != null) cumulativeScale *= _parentTransform.Scale;
					return cumulativeScale;
				}
			}

			/// <summary>
			/// Moves the origin to center of Tile.
			/// </summary>
			protected override void CalculatePreTransform(Matrix cumulativeTransform)
			{
				// shift center of model to the origin before performing transform
				var offset = SourceDimensions/2f + ModelOrigin;
				cumulativeTransform.Translate(-offset.X, -offset.Y, -offset.Z);
			}

			/// <summary>
			/// Moves the origin to the center of the image.
			/// </summary>
			protected override void CalculatePostTransform(Matrix cumulativeTransform)
			{
				// shift center of model back from the origin after performing transform
				var offset = SourceDimensions/2f + ModelOrigin;
				cumulativeTransform.Translate(offset.X, offset.Y, offset.Z);
			}

			private static Vector3D ComputeAdjustedDimensions(Vector3D dimensions, Vector3D spacing, Vector3D aspectRatio)
			{
				// compute the effective dimensions, correcting any anisotropic voxel aspect ratio
				float ratioX, ratioY, ratioZ;
				if (aspectRatio != null)
				{
					// choose the smallest dimension to ensure that the model is never downsampled
					var ratioD = Math.Min(Math.Min(aspectRatio.X, aspectRatio.Y), aspectRatio.Z);
					ratioX = aspectRatio.X/ratioD;
					ratioY = aspectRatio.Y/ratioD;
					ratioZ = aspectRatio.Z/ratioD;
				}
				else if (spacing != null)
				{
					// choose the smallest dimension to ensure that the model is never downsampled
					var ratioD = Math.Min(Math.Min(spacing.X, spacing.Y), spacing.Z);
					ratioX = spacing.X/ratioD;
					ratioY = spacing.Y/ratioD;
					ratioZ = spacing.Z/ratioD;
				}
				else
				{
					ratioX = ratioY = ratioZ = 1;
				}
				return new Vector3D(ratioX*dimensions.X, ratioY*dimensions.Y, ratioZ*dimensions.Z);
			}
		}

		#endregion
	}
}