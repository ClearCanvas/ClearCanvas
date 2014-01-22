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
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Graphics3D;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A <see cref="PresentationImage"/> that encapsulates basic
	/// 3D image functionality.
	/// </summary>
	[Cloneable]
	public abstract class BasicPresentationImage3D :
		PresentationImage,
		ISpatialTransformProvider,
		ISpatialTransform3DProvider,
		IApplicationGraphicsProvider,
		IOverlayGraphicsProvider,
		IOverlayGraphics3DProvider,
		IAnnotationLayoutProvider
	{
		#region Private fields

		[CloneIgnore]
		private CompositeRootModel3D _compositeImageGraphic;

		[CloneIgnore]
		private CompositeGraphic _applicationGraphics;

		[CloneIgnore]
		private CompositeGraphic _overlayGraphics;

		[CloneIgnore]
		private CompositeGraphic3D _overlayGraphics3D;

		[CloneCopyReference]
		private readonly Vector3D _modelDimensions;

		[CloneCopyReference]
		private readonly Vector3D _voxelSpacing;

		private readonly Size _sceneSize;

		private IAnnotationLayout _annotationLayout;

		#endregion

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected BasicPresentationImage3D(BasicPresentationImage3D source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage3D"/>.
		/// </summary>
		protected BasicPresentationImage3D(float dimensionX, float dimensionY, float dimensionZ)
			: this(new Vector3D(dimensionX, dimensionY, dimensionZ), null, Size.Empty) {}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage3D"/>.
		/// </summary>
		protected BasicPresentationImage3D(float dimensionX, float dimensionY, float dimensionZ, int sceneWidth, int sceneHeight)
			: this(new Vector3D(dimensionX, dimensionY, dimensionZ), null, new Size(sceneWidth, sceneHeight)) {}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage3D"/>.
		/// </summary>
		protected BasicPresentationImage3D(Vector3D modelDimensions)
			: this(modelDimensions, null, Size.Empty) {}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage3D"/>.
		/// </summary>
		protected BasicPresentationImage3D(Vector3D modelDimensions, Size sceneSize)
			: this(modelDimensions, null, sceneSize) {}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage3D"/>.
		/// </summary>
		protected BasicPresentationImage3D(Vector3D modelDimensions, Vector3D voxelSpacing, Size sceneSize)
		{
			Platform.CheckForNullReference(modelDimensions, "modelDimensions");
			_modelDimensions = modelDimensions;
			_voxelSpacing = voxelSpacing ?? new Vector3D(1, 1, 1);
			_sceneSize = sceneSize;

			if (_sceneSize.IsEmpty)
			{
				var diagonal = (int) Math.Ceiling(new Vector3D(_modelDimensions.X*_voxelSpacing.X, _modelDimensions.Y*_voxelSpacing.Y, _modelDimensions.Z*_voxelSpacing.Z).Magnitude);
				_sceneSize = new Size(diagonal, diagonal);
			}

			InitializeSceneGraph(_sceneSize.Width, _sceneSize.Height, _modelDimensions.X, _modelDimensions.Y, _modelDimensions.Z, _voxelSpacing.X, _voxelSpacing.Y, _voxelSpacing.Z);
		}

		#region Public properties

		/// <summary>
		/// Gets the dimensions of the scene.
		/// </summary>
		/// <remarks>
		/// The dimensions of the scene effectively define the boundaries of the <see cref="IPresentationImage"/>.
		/// </remarks>
		public override Size SceneSize
		{
			get { return _sceneSize; }
		}

		public Vector3D Dimensions
		{
			get { return _modelDimensions; }
		}

		public Vector3D Spacing
		{
			get { return _voxelSpacing; }
		}

		public Vector3D Origin
		{
			get { return _compositeImageGraphic.Origin; }
			protected set { _compositeImageGraphic.Origin = value; }
		}

		#endregion

		#region ISpatialTransformProvider members

		/// <summary>
		/// Gets this presentation image's spatial transform.
		/// </summary>
		/// <remarks>
		/// The <see cref="ImageGraphic"/> and graphics added to the 
		/// <see cref="OverlayGraphics"/> collection are subject to this
		/// spatial transform.  Thus, the effect is that overlay graphics
		/// appear to be anchored to the underlying image.
		/// </remarks>
		public SpatialTransform ViewPortSpatialTransform
		{
			get { return _compositeImageGraphic.SpatialTransform; }
		}

		public SpatialTransform3D SceneRootSpatialTransform
		{
			get { return _compositeImageGraphic.SpatialTransform3D; }
		}

		ISpatialTransform3D ISpatialTransform3DProvider.SpatialTransform3D
		{
			get { return SceneRootSpatialTransform; }
		}

		ISpatialTransform ISpatialTransformProvider.SpatialTransform
		{
			get { return ViewPortSpatialTransform; }
		}

		#endregion

		#region IApplicationGraphicsProvider members

		/// <summary>
		/// Gets this presentation image's collection of application graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ApplicationGraphics"/> to add graphics that you want to
		/// overlay the image at the application-level. These graphics are rendered
		/// before any <see cref="OverlayGraphics"/>.
		/// </remarks>
		public GraphicCollection ApplicationGraphics
		{
			get { return _applicationGraphics.Graphics; }
		}

		#endregion

		#region IOverlayGraphicsProvider members

		/// <summary>
		/// Gets this presentation image's collection of overlay graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="OverlayGraphics"/> to add graphics that you want to
		/// overlay the image at the user-level. These graphics are rendered
		/// after any <see cref="ApplicationGraphics"/>.
		/// </remarks>
		public GraphicCollection OverlayGraphics
		{
			get { return _overlayGraphics.Graphics; }
		}

		#endregion

		#region IOverlayGraphics3DProvider members

		/// <summary>
		/// Gets this presentation image's collection of overlay graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="OverlayGraphics3D"/> to add graphics that you want to
		/// overlay the image at the user-level. These graphics are rendered
		/// after any <see cref="ApplicationGraphics"/>.
		/// </remarks>
		public GraphicCollection3D OverlayGraphics3D
		{
			get { return _overlayGraphics3D.Graphics; }
		}

		#endregion

		#region IAnnotationLayoutProvider Members

		/// <summary>
		/// Gets the <see cref="IAnnotationLayout"/> associated with this presentation image.
		/// </summary>
		public IAnnotationLayout AnnotationLayout
		{
			get { return _annotationLayout ?? (_annotationLayout = CreateAnnotationLayout()); }
			set { _annotationLayout = value; }
		}

		#endregion

		/// <summary>
		/// Gets an <see cref="IRenderer"/>.
		/// </summary>
		/// <remarks>
		/// In general, <see cref="ImageRenderer"/> should be considered an internal
		/// Framework property and should not be used.
		/// </remarks>
		public override IRenderer ImageRenderer
		{
			get { return base.ImageRenderer ?? (base.ImageRenderer = CreateImageRenderer()); }
		}

		protected abstract IRenderer CreateImageRenderer();

		#region Disposal

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary>
		/// Gets the <see cref="CompositeGraphic"/> at the root of the scene graphic containing the image itself and all additional graphics.
		/// </summary>
		protected CompositeGraphic CompositeImageGraphic
		{
			get { return _compositeImageGraphic; }
		}

		/// <summary>
		/// Creates an empty <see cref="AnnotationLayout"/> unless overridden.
		/// </summary>
		protected virtual IAnnotationLayout CreateAnnotationLayout()
		{
			//return an empty layout.
			return new AnnotationLayout();
		}

		private void InitializeSceneGraph(
			int sceneWidth,
			int sceneHeight,
			float dimensionX,
			float dimensionY,
			float dimensionZ,
			float pixelSpacingX,
			float pixelSpacingY,
			float pixelSpacingZ)
		{
			_compositeImageGraphic = new CompositeRootModel3D(sceneWidth, sceneHeight, dimensionX, dimensionY, dimensionZ, pixelSpacingX, pixelSpacingY, pixelSpacingZ)
			                         	{
			                         		Origin = new Vector3D(-dimensionX/2, -dimensionY/2, -dimensionZ/2)
			                         	};

			_applicationGraphics = new CompositeGraphic();
			_applicationGraphics.Name = "Application2D";

			_overlayGraphics = new CompositeGraphic();
			_overlayGraphics.Name = "Overlay2D";

			_overlayGraphics3D = new CompositeGraphic3D();
			_overlayGraphics3D.Name = "Overlay3D";

			_compositeImageGraphic.Graphics.Add(_applicationGraphics);
			_compositeImageGraphic.Graphics.Add(_overlayGraphics);
			_compositeImageGraphic.Graphics3D.Add(_overlayGraphics3D);

			SceneGraph.Graphics.Add(_compositeImageGraphic);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_compositeImageGraphic = (CompositeRootModel3D) CollectionUtils.SelectFirst(SceneGraph.Graphics, test => test is CompositeRootModel3D);

			Platform.CheckForNullReference(_compositeImageGraphic, "_compositeImageGraphic");

			_applicationGraphics = (CompositeGraphic) CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics, test => test.Name == "Application2D");
			_overlayGraphics = (CompositeGraphic) CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics, test => test.Name == "Overlay2D");
			_overlayGraphics3D = (CompositeGraphic3D) CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics3D, test => test.Name == "Overlay3D");

			Platform.CheckForNullReference(_applicationGraphics, "_applicationGraphics");
			Platform.CheckForNullReference(_overlayGraphics, "_overlayGraphics");
			Platform.CheckForNullReference(_overlayGraphics3D, "_overlayGraphics3D");
		}
	}
}