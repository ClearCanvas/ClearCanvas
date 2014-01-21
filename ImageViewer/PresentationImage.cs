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
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The final image that is presented to the user in a <see cref="Tile"/>.
	/// </summary>
	/// <remarks>
	/// An <see cref="IPresentationImage"/> can be thought of as a <i>scene</i> 
	/// composed of many parts, be they images, lines, text, etc.  It is the
	/// image that is presented to the user in a <see cref="Tile"/>.
	/// </remarks>
	[Cloneable(true)]
	public abstract class PresentationImage : IPresentationImage
	{
		#region Private Fields

		[CloneIgnore]
		private ImageViewerComponent _imageViewer;
		[CloneIgnore]
		private DisplaySet _parentDisplaySet;
		[CloneIgnore]
		private Tile _tile;
		private Rectangle _clientRectangle;

		[CloneIgnore]
		private bool _selected = false;
		[CloneIgnore]
		private bool _linked = true;
		private string _uid;
		
		private CompositeGraphic _sceneGraph;
		
		[CloneIgnore]
		private ISelectableGraphic _selectedGraphic;
		[CloneIgnore]
		private IFocussableGraphic _focussedGraphic;

		[CloneIgnore]
		private IRenderer _renderer;
		private event EventHandler _drawing;

	    private ExtensionData _extensionData;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="PresentationImage"/>.
		/// </summary>
		protected PresentationImage()
		{
		}
		
		#region Public Properties

		/// <summary>
		/// Gets the dimensions of the scene.
		/// </summary>
		/// <remarks>
		/// The dimensions of the scene effectively define the boundaries of the <see cref="IPresentationImage"/>.
		/// </remarks>
		public abstract Size SceneSize { get; }

		/// <summary>
		/// Gets the parent <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="PresentationImage"/> is not part of the 
		/// logical workspace yet.</value>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			internal set 
			{ 
				_imageViewer = value as ImageViewerComponent;
				this.SceneGraph.SetImageViewer(_imageViewer);
			}
		}

		/// <summary>
		/// Gets the parent <see cref="IDisplaySet"/>.
		/// </summary>
		/// <value>The parent <see cref="IDisplaySet"/> or <b>null</b> if the 
		/// <see cref="PresentationImage"/> has not
		/// been added to the <see cref="IDisplaySet"/> yet.</value>
		public IDisplaySet ParentDisplaySet
		{
			get { return _parentDisplaySet; }
			internal set { _parentDisplaySet = value as DisplaySet; }
		}

		/// <summary>
		/// Gets the associated <see cref="ITile"/>.
		/// </summary>
		/// <value>The <see cref="ITile"/> that currently contains the
		/// <see cref="PresentationImage"/> or <b>null</b> if the 
		/// <see cref="PresentationImage"/> is not currently visible.</value>
		public ITile Tile
		{
			get { return _tile; }
			internal set 
			{
				if (_tile != value)
				{
					_tile = value as Tile;

					if (_tile != null)
						_tile.PresentationImage = this;
					else
						_clientRectangle = Rectangle.Empty;
				}
			}
		}

		/// <summary>
		/// Gets the client rectangle of the surface on which the
		/// presentation image will be rendered.
		/// </summary>
		public Rectangle ClientRectangle
		{
			get { return !_clientRectangle.IsEmpty ? _clientRectangle : new Rectangle(new Point(), SceneSize); }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="PresentationImage"/> is visible.
		/// </summary>
		public bool Visible
		{
			get { return this.Tile != null; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="PresentationImage"/> is selected.
		/// </summary>
		public bool Selected
		{
			get { return _selected; }
			internal set
			{
				if (_selected == value)
					return;

				_selected = value;

				if (!_selected)
					return;

				if (this.ImageViewer == null)
					return;

				this.ImageViewer.EventBroker.OnPresentationImageSelected(
				    new PresentationImageSelectedEventArgs(this));
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="PresentationImage"/> is linked.
		/// </summary>
		public bool Linked
		{
			get { return _linked; }
			set
			{
				if (_linked == value)
					return;

				_linked = value;
			}
		}

		/// <summary>
		/// Gets or sets unique identifier for this <see cref="IPresentationImage"/>.
		/// </summary>
		public string Uid
		{
			get { return _uid; }
			set { _uid = value; }
		}

		/// <summary>
		/// Gets the <see cref="SceneGraph"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="SceneGraph"/> is a tree of <see cref="IGraphic"/> objects
		/// that represents what the user sees in a <see cref="Tile"/>.  If you're writing
		/// tools, you should avoid accessing the <see cref="SceneGraph"/> directly as it 
		/// is intended only for the renderer to traverse.  Instead, add and remove
		/// from the <see cref="SceneGraph"/> through interfaces on <see cref="PresentationImage"/>
		/// subclasses.
		/// </remarks>
		public CompositeGraphic SceneGraph
		{
			get 
			{
				if (_sceneGraph == null)
				{
					_sceneGraph = new CompositeGraphic();
					_sceneGraph.SetParentPresentationImage(this);
				}

				return _sceneGraph; 
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently selected.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		public virtual ISelectableGraphic SelectedGraphic
		{
			get { return _selectedGraphic; }
			set
			{
				// If it's the same graphic, then just return
				if (_selectedGraphic == value)
					return;

				// Deselect the previously selected graphic
				if (_selectedGraphic != null)
					_selectedGraphic.Selected = false;

				ISelectableGraphic deselectedGraphic = _selectedGraphic;
				_selectedGraphic = value;

				// Let everyone know
				if (this.ImageViewer != null)
				{
					if (this.ImageViewer.EventBroker != null)
					{
						this.ImageViewer.EventBroker.OnGraphicSelectionChanged(new GraphicSelectionChangedEventArgs(_selectedGraphic, deselectedGraphic));
					}
				}
			}
		}

		/// <summary>
		/// Gets the currently focussed <see cref="IGraphic"/>.
		/// </summary>
		/// <value>The currently selected <see cref="IGraphic"/> or <b>null</b>
		/// if no <see cref="IGraphic"/> is currently focussed.</value>
		/// <remarks>
		/// It is possible for an <see cref="IGraphic"/> to be selected,
		/// focussed or selected and focussed.
		/// </remarks>
		public virtual IFocussableGraphic FocussedGraphic
		{
			get { return _focussedGraphic; }
			set
			{
				// If it's the same graphic, then just return
				if (_focussedGraphic == value)
					return;

				// Defocus the previously focussed graphic
				if (_focussedGraphic != null)
					_focussedGraphic.Focussed = false;

				IFocussableGraphic unfocusedGraphic = _focussedGraphic;
				_focussedGraphic = value;

				// Let everyone know
				if (this.ImageViewer != null)
				{
					if (this.ImageViewer.EventBroker != null)
					{
						this.ImageViewer.EventBroker.OnGraphicFocusChanged(new GraphicFocusChangedEventArgs(_focussedGraphic, unfocusedGraphic));
					}
				}
			}
		}

		/// <summary>
		/// Gets this <see cref="PresentationImage"/>'s image renderer.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The creation of the image renderer is left to the subclass.
		/// This allows the greatest flexibility, since it is sometimes the case
		/// that a subclass of <see cref="PresentationImage"/> needs 
		/// a specialized <see cref="IRenderer"/>.
		/// </para>
		/// <para>
		/// In general, <see cref="ImageRenderer"/> should be considered an internal
		/// Framework property and should not be used.
		/// </para>
		/// </remarks>
		public virtual IRenderer ImageRenderer
		{
			get { return _renderer; }
			protected set { _renderer = value; }
		}

	    public ExtensionData ExtensionData
	    {
	        get
	        {
	            if (_extensionData == null)
                    _extensionData = new ExtensionData();
	            return _extensionData;
	        }    
	    }

		#endregion

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="PresentationImage"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_renderer != null)
				{
					_renderer.Dispose();
					_renderer = null;
				}

				if (_sceneGraph != null)
				{
					_sceneGraph.Dispose();
					_sceneGraph = null;
				}

				if (_extensionData != null)
				{
					_extensionData.Dispose();
					_extensionData = null;
				}
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Creates a fresh copy of the <see cref="IPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="IPresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>
		public abstract IPresentationImage CreateFreshCopy();

		/// <summary>
		/// Creates a deep copy of the <see cref="IPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IPresentationImage"/>s should never return null from this method.
		/// </remarks>
		public IPresentationImage Clone()
		{
			try
			{
				PresentationImage clone = CloneBuilder.Clone(this) as PresentationImage;
				if (clone != null)
				{
					clone.SceneGraph.SetParentPresentationImage(clone);
					if (ImageViewer != null)
						ImageViewer.EventBroker.OnCloneCreated(new CloneCreatedEventArgs(this, clone));
				}

				return clone;
			}
			catch (Exception e)
			{
				throw new PresentationImageCloningException(this, e);
			}
		}

		/// <summary>
		/// Fires just before the <see cref="PresentationImage"/> is actually drawn/rendered.
		/// </summary>
		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		/// <summary>
		/// Draws the <see cref="PresentationImage"/>.
		/// </summary>
		public void Draw()
		{
			if (this.Visible && this.Tile != null)
				this.Tile.Draw();
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
		public virtual void Draw(DrawArgs drawArgs)
		{
			drawArgs.SceneGraph = this.SceneGraph;
			_clientRectangle = drawArgs.RenderingSurface.ClientRectangle;

			// Let others know that we're about to draw
			ImageDrawingEventArgs args = new ImageDrawingEventArgs(this);
			if (this.ImageViewer != null && this.ImageViewer.EventBroker != null)
				this.ImageViewer.EventBroker.OnImageDrawing(args);

			OnDrawing();

			this.ImageRenderer.Draw(drawArgs);
		}

		/// <summary>
		/// Renders the <see cref="PresentationImage"/> to an offscreen <see cref="Bitmap"/>.
		/// </summary>
		/// <param name="width">Bitmap width.</param>
		/// <param name="height">Bitmap height.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method can be used anywhere an offscreen bitmap is required, such as 
		/// paper/DICOM printing, thumbnail generation, creation of new DICOM images, etc.
		/// </remarks>
		/// <exception cref="ArgumentException">Thrown if invalid image dimensions are supplied.</exception>
		/// <exception cref="RenderingException">Thrown if any <see cref="Exception"/> is encountered while rendering the image.</exception>
		public virtual Bitmap DrawToBitmap(int width, int height)
		{
			Platform.CheckPositive(width, "width");
			Platform.CheckPositive(height, "height");

			Bitmap bmp = new Bitmap(width, height);
			try
			{
				DrawToBitmap(bmp);
				return bmp;
			}
			catch (Exception)
			{
				// if an exception is thrown, then the calling code won't get the reference to the bitmap, so we need to dispose of it here
				bmp.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Renders the <see cref="PresentationImage"/> to a provided offscreen <see cref="Bitmap"/>.
		/// </summary>
		/// <param name="bmp">The offscreen bitmap to render to.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method can be used anywhere an offscreen bitmap is required, such as 
		/// paper/DICOM printing, thumbnail generation, creation of new DICOM images, etc.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="bmp"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="bmp"/> has invalid dimensions.</exception>
		/// <exception cref="RenderingException">Thrown if any <see cref="Exception"/> is encountered while rendering the image.</exception>
		public virtual void DrawToBitmap(Bitmap bmp)
		{
			Platform.CheckForNullReference(bmp, "bmp");

			Platform.CheckPositive(bmp.Width, "bmp.Width");
			Platform.CheckPositive(bmp.Height, "bmp.Height");

			var graphics = System.Drawing.Graphics.FromImage(bmp);
			var contextId = graphics.GetHdc();
			try
			{
				using (var surface = ImageRenderer.CreateRenderingSurface(IntPtr.Zero, bmp.Width, bmp.Height, RenderingSurfaceType.Offscreen))
				{
					surface.ContextID = contextId;
					surface.ClipRectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);

                    var drawArgs = new DrawArgs(surface, null, DrawMode.Render) { Tag = bmp };
					DrawNoEvents(drawArgs);
                    drawArgs = new DrawArgs(surface, null, DrawMode.Refresh) { Tag = bmp };
					DrawNoEvents(drawArgs);
				}
			}
			finally
			{
				graphics.ReleaseHdc(contextId);
				graphics.Dispose();
			}
		}

		#endregion

		/// <summary>
		/// Renders the <see cref="PresentationImage"/> without firing any events.
		/// </summary>
		/// <exception cref="RenderingException">Thrown if any <see cref="Exception"/> is encountered while rendering the image.</exception>
		protected void DrawNoEvents(DrawArgs drawArgs)
		{
			drawArgs.SceneGraph = this.SceneGraph;
			Rectangle oldRectangle = _clientRectangle;
			_clientRectangle = drawArgs.RenderingSurface.ClientRectangle;

			try
			{
				this.ImageRenderer.Draw(drawArgs);
			}
			finally
			{
				_clientRectangle = oldRectangle;
			}
		}

		/// <summary>
		/// Raises the <see cref="Drawing"/> event.
		/// </summary>
		protected virtual void OnDrawing()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);
		}
	}
}
