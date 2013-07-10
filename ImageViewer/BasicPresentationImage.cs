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
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Rendering.GDI;

namespace ClearCanvas.ImageViewer
{
	#region BasicPresentationImageRendererFactoryExtensionPoint class

	/// <summary>
	/// The <see cref="BasicPresentationImage"/> creates a single factory object that is then used to create
	/// an <see cref="IRenderer"/> for each <see cref="BasicPresentationImage"/>.  
	/// </summary>
	/// <remarks>
	/// The returned <see cref="IRenderer"/> need not be thread-safe as 
	/// the <see cref="BasicPresentationImage"/> itself is not thread-safe.
	/// </remarks>
	public sealed class BasicPresentationImageRendererFactoryExtensionPoint : ExtensionPoint<IRendererFactory>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public BasicPresentationImageRendererFactoryExtensionPoint()
		{
		}
	}

	#endregion

	/// <summary>
	/// A <see cref="PresentationImage"/> that encapsulates basic
	/// 2D image functionality.
	/// </summary>
	[Cloneable]
	public abstract class BasicPresentationImage :
		PresentationImage, 
		IImageGraphicProvider,
		ISpatialTransformProvider,
		IApplicationGraphicsProvider,
		IOverlayGraphicsProvider,
		IAnnotationLayoutProvider,
		IPresentationStateProvider
	{
		#region Private fields

		private static readonly object _rendererFactorySyncLock = new object();
		private static volatile IRendererFactory _rendererFactory;
		private static volatile RendererFactoryInitializationException _rendererFactoryInitializationException;

		[CloneIgnore]
		private CompositeGraphic _compositeImageGraphic;
		[CloneIgnore]
		private ImageGraphic _imageGraphic;
		[CloneIgnore]
		private CompositeGraphic _applicationGraphics;
		[CloneIgnore]
		private CompositeGraphic _overlayGraphics;
		[CloneIgnore]
		private PresentationStateGraphic _presentationStateGraphic;
		private IAnnotationLayout _annotationLayout;

		#endregion

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected BasicPresentationImage(BasicPresentationImage source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage"/>.
		/// </summary>
		protected BasicPresentationImage(ImageGraphic imageGraphic) 
			: this(imageGraphic, 1.0, 1.0, 1.0, 1.0)
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage"/>.
		/// </summary>
		protected BasicPresentationImage(
			ImageGraphic imageGraphic,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY)
		{
			Platform.CheckForNullReference(imageGraphic, "imageGraphic");

			InitializeSceneGraph(
				imageGraphic, 
				pixelSpacingX, 
				pixelSpacingY, 
				pixelAspectRatioX, 
				pixelAspectRatioY);
		}

		#region Public properties

		/// <summary>
		/// Gets the dimensions of the image.
		/// </summary>
		public override Size SceneSize
		{
			get { return new Size(_imageGraphic.Columns, _imageGraphic.Rows); }
		}

		#region IImageGraphicProvider

		/// <summary>
		/// Gets this presentation image's <see cref="ImageGraphic"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ImageGraphic"/> is the first <see cref="IGraphic"/>
		/// added to the <see cref="PresentationImage.SceneGraph"/> and thus is rendered first.
		/// </remarks>
		public ImageGraphic ImageGraphic
		{
			get { return _imageGraphic; }
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
		public ISpatialTransform SpatialTransform
		{
			get { return _compositeImageGraphic.SpatialTransform; }
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

		#region IPresentationStateProvider members

		/// <summary>
		/// Gets or sets the <see cref="PresentationStates.PresentationState"/> of the image.
		/// </summary>
		public PresentationState PresentationState
		{
			get { return _presentationStateGraphic.PresentationState; }
			set { _presentationStateGraphic.PresentationState = value; }
		}

		#endregion

		#endregion

		#region IAnnotationLayoutProvider Members

		/// <summary>
		/// Gets the <see cref="IAnnotationLayout"/> associated with this presentation image.
		/// </summary>
		public IAnnotationLayout AnnotationLayout
		{
			get
			{
				if (_annotationLayout == null)
					_annotationLayout = CreateAnnotationLayout();
				
				return _annotationLayout;
			}
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
			get
			{
				if (_rendererFactory == null)
				{
					lock (_rendererFactorySyncLock)
					{
						if (_rendererFactory == null)
						{
							try
							{
								_rendererFactory = (IRendererFactory)new BasicPresentationImageRendererFactoryExtensionPoint().CreateExtension();
								_rendererFactory.Initialize();
							}
							catch (RendererFactoryInitializationException e)
							{
								_rendererFactory = null;
								_rendererFactoryInitializationException = e;
								Platform.Log(LogLevel.Warn, e);
							}
							catch(NotSupportedException)
							{
								Platform.Log(LogLevel.Debug, SR.MessageNoRendererPluginsExist);
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Warn, e);
							}

							if (_rendererFactory == null)
								_rendererFactory = new GdiRendererFactory();
						}
					}
				}

				if (base.ImageRenderer == null)
					base.ImageRenderer = _rendererFactory.GetRenderer();

				return base.ImageRenderer;
			}
		}

		/// <summary>
		/// Raises the <see cref="EventBroker.ImageDrawing"/> event and
		/// renders the <see cref="PresentationImage"/>.
		/// </summary>
		/// <param name="drawArgs"></param>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		/// <exception cref="RenderingException">Thrown if any <see cref="Exception"/> is encountered while rendering the image.</exception>
		public override void Draw(DrawArgs drawArgs)
		{
			base.Draw(drawArgs);

			CheckRendererInitializationError();
		}

		#region Disposal

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

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

		private static void CheckRendererInitializationError()
		{
			if (_rendererFactoryInitializationException != null)
			{
				Exception e = _rendererFactoryInitializationException;
				_rendererFactoryInitializationException = null;
				throw e;
			}
		}

		private void InitializeSceneGraph(
			ImageGraphic imageGraphic,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY)
		{
			_imageGraphic = imageGraphic;

			_compositeImageGraphic = new CompositeImageGraphic(
				imageGraphic.Rows,
				imageGraphic.Columns,
				pixelSpacingX,
				pixelSpacingY,
				pixelAspectRatioX,
				pixelAspectRatioY);

			_applicationGraphics = new CompositeGraphic();
			_applicationGraphics.Name = "Application";

			_overlayGraphics = new CompositeGraphic();
			_overlayGraphics.Name = "Overlay";

			_presentationStateGraphic = new PresentationStateGraphic();
			_presentationStateGraphic.Name = "PresentationState";

			_compositeImageGraphic.Graphics.Add(_imageGraphic);
			_compositeImageGraphic.Graphics.Add(_applicationGraphics);
			_compositeImageGraphic.Graphics.Add(_overlayGraphics);

			this.SceneGraph.Graphics.Add(_presentationStateGraphic);
			this.SceneGraph.Graphics.Add(_compositeImageGraphic);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_presentationStateGraphic = CollectionUtils.SelectFirst(SceneGraph.Graphics,
				delegate(IGraphic test) { return test is PresentationStateGraphic; }) as PresentationStateGraphic;

			_compositeImageGraphic = CollectionUtils.SelectFirst(SceneGraph.Graphics,
				delegate(IGraphic test) { return test is CompositeImageGraphic; }) as CompositeImageGraphic;
			
			Platform.CheckForNullReference(_compositeImageGraphic, "_compositeImageGraphic");

			_imageGraphic = CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics,
				delegate(IGraphic test) { return test is ImageGraphic; }) as ImageGraphic;

			_applicationGraphics = CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics,
				delegate(IGraphic test) { return test.Name == "Application"; }) as CompositeGraphic;

			_overlayGraphics = CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics,
				delegate(IGraphic test) { return test.Name == "Overlay"; }) as CompositeGraphic;

			Platform.CheckForNullReference(_imageGraphic, "_imageGraphic");
			Platform.CheckForNullReference(_applicationGraphics, "_applicationGraphics");
			Platform.CheckForNullReference(_overlayGraphics, "_overlayGraphics");
			Platform.CheckForNullReference(_presentationStateGraphic, "_presentationStateGraphic");
		}
	}
}
