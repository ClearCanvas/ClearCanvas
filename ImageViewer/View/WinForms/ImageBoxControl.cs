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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
    /// </summary>
    public partial class ImageBoxControl : UserControl
    {
        private ImageBox _imageBox;
		private Rectangle _parentRectangle;
		private bool _imageScrollerVisible;
		private CompositeUndoableCommand _historyCommand;
		private MemorableUndoableCommand _imageBoxCommand;
        private readonly Dictionary<IImageBoxExtension, IImageBoxExtensionView> _extensionViews = new Dictionary<IImageBoxExtension, IImageBoxExtensionView>();


        private IList<IImageBoxExtension> _extensions;

        /// <summary>
        /// Constructor
        /// </summary>
		internal ImageBoxControl(ImageBox imageBox, Rectangle parentRectangle)
        {
			_imageBox = imageBox;
            
			this.ParentRectangle = parentRectangle;
             
			InitializeComponent();

			_imageScrollerVisible = _imageScroller.Visible;
			_imageScroller.MouseDown += ImageScrollerMouseDown;
			_imageScroller.MouseUp += ImageScrollerMouseUp;
			_imageScroller.ValueChanged += ImageScrollerValueChanged;

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.BackColor = Color.Black;
			this.Dock = DockStyle.None;
			this.Anchor = AnchorStyles.None;

			_imageBox.Drawing += OnDrawing;
			_imageBox.SelectionChanged += OnImageBoxSelectionChanged;
			_imageBox.LayoutCompleted += OnLayoutCompleted;

            foreach (var extension in Extensions)
            {
                AttachExtension(extension);
            }
        }

		internal ImageBox ImageBox
		{
			get { return _imageBox; }
		}

		internal Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set 
			{
				_parentRectangle = value;

				int left = (int)(_imageBox.NormalizedRectangle.Left * _parentRectangle.Width);
				int top = (int)(_imageBox.NormalizedRectangle.Top * _parentRectangle.Height);
				int right = (int)(_imageBox.NormalizedRectangle.Right * _parentRectangle.Width);
				int bottom = (int)(_imageBox.NormalizedRectangle.Bottom * _parentRectangle.Height);

				this.SuspendLayout();

				this.Location = new Point(left, top);
				this.Size = new Size(right - left, bottom - top);

				this.ResumeLayout(false);
			}
		}


    	private IEnumerable<TileControl> TileControls
    	{
    		get
    		{
    			foreach (Control control in this.Controls)
    			{
    				if (control is TileControl)
    					yield return (TileControl) control;
    			}
    		}
    	}

		private TileControl GetTileControl(ITile tile)
		{
			foreach (TileControl tileControl in TileControls)
			{
				if (tileControl.Tile == tile)
					return tileControl;
			}

			return null;
		}

		internal void Draw()
		{
			_imageBox.Draw();	
		}

		private void DoDraw()
		{
            foreach (TileControl control in this.TileControls)
                control.Draw(); 
            Invalidate();
		}

        #region Protected methods

		protected override void OnLoad(EventArgs e)
		{
			AddTileControls(_imageBox);

			base.OnLoad(e);
		}


		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

            // Notify all extensions of the new size
            foreach (var view in _extensionViews.Values)
            {
                view.ParentSize = Size;
                if (view.Visible)
                {
                    // update the view so that we can decide whether or not we should suppress tile draw
                    view.UpdateLayout();
                }
            }

            // If an extension is being displayed and covers the entire image box, we don't want to draw the tile. 
            // Doing so will cause the image to briefly appear before being covered by the extension control.
            // We also don't want to suppress drawing if the extension control is visible but does not cover the entire image box 
            // (eg, an extension that draws a textbox on the image).
            bool suppressTileDraw = _imageBox != null && 
                                    CollectionUtils.Contains(_extensionViews.Values,
                                                        view => view.Visible &&
                                                        view.ActualSize.Width >= Size.Width &&
                                                        view.ActualSize.Height >= Size.Height);

			this.SuspendLayout();

            SetTileParentImageBoxRectangles(suppressTileDraw);

			this.ResumeLayout(false);

			if (ImageScrollerVisible)
			{
				_imageScroller.Location = new Point(this.Width - _imageScroller.Width, 0);
				_imageScroller.Size = new Size(_imageScroller.Width, this.Height);
			}

		    Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
            if (_imageBox == null)
                return;

			e.Graphics.Clear(Color.Black);

			DrawImageBoxBorder(e);
			DrawTileBorders(e);

			base.OnPaint(e);
		}

		#endregion

		#region Private methods

		private void OnImageBoxSelectionChanged(object sender, ItemEventArgs<IImageBox> e)
		{
			Invalidate();
			Update();
		}

		private void OnTileSelectionChanged(object sender, ItemEventArgs<ITile> e)
		{
			Invalidate();
			Update();
		}

		private void OnDrawing(object sender, EventArgs e)
		{
			DoDraw();
		}

		private void OnTileControlDrawing(object sender, EventArgs e)
		{
			//Perfectly efficient when there's only 1 tile ... a little less so when there's more than 1.
			//However, doing this makes sure the scroll bar is *always* up to date, even for things like sorting of images,
			//which currently doesn't actually fire any events.
			UpdateImageScroller();
		}

		private void DisposeControls(IEnumerable<TileControl> controls)
		{
			foreach (TileControl control in controls)
			{
				this.Controls.Remove(control);
			    control.MouseMove -= OnTileMouseMove;
				control.Tile.SelectionChanged -= OnTileSelectionChanged;
				control.Drawing -= OnTileControlDrawing;
				control.Dispose();
			}
		}

		private void PerformDispose()
		{

            DisposeExtensions();

			if (_imageBox != null)
			{
				_imageBox.Drawing -= OnDrawing;
				_imageBox.SelectionChanged -= OnImageBoxSelectionChanged;
				_imageBox.LayoutCompleted -= OnLayoutCompleted;
				_imageBox = null;
			}

			if (_imageScroller != null)
			{
				_imageScroller.ValueChanged -= ImageScrollerValueChanged;
				_imageScroller.MouseDown -= ImageScrollerMouseDown;
				_imageScroller.MouseUp -= ImageScrollerMouseUp;
			}

			DisposeControls(new List<TileControl>(this.TileControls));
		}

    	private void OnLayoutCompleted(object sender, EventArgs e)
		{
			List<TileControl> oldControls = new List<TileControl>(this.TileControls);

			this.SuspendLayout();

			// We add all the new tile controls to the image box control first,
			// then we remove the old ones. Removing them first then adding them
			// results in flickering, which we don't want.
			AddTileControls(_imageBox);

    		DisposeControls(oldControls);

			this.ResumeLayout(true);
		}

		private void DrawImageBoxBorder(PaintEventArgs e)
		{
			// Draw image box border
			DrawBorder(
				e.Graphics,
				this.ClientRectangle,
				_imageBox.BorderColor,
				ImageBox.BorderWidth,
				ImageBox.InsetWidth);
		}

		private void DrawTileBorders(PaintEventArgs e)
		{
			// Draw tile border, provided there's more than one tile
			if (this.Controls.Count > 1)
			{
				foreach (TileControl control in this.TileControls)
				{
					Rectangle rectangle = GetTileRectangle(control);

					DrawBorder(
						e.Graphics,
						rectangle,
						control.Tile.BorderColor,
						Tile.BorderWidth,
						Tile.InsetWidth);
				}
			}
		}

		private Rectangle GetTileRectangle(TileControl control)
		{
			Rectangle tileRectangle = new Rectangle(control.Location, control.Size);

			Rectangle borderRectangle =
				Rectangle.Inflate(
					tileRectangle,
					Tile.InsetWidth,
					Tile.InsetWidth);

			return borderRectangle;
		}

		private void DrawBorder(System.Drawing.Graphics graphics, Rectangle rectangle, Color borderColor, int borderWidth, int insetWidth)
		{
			int offset = insetWidth / 2;
			Rectangle borderRectangle = Rectangle.Inflate(rectangle, -offset, -offset);

			using (Pen pen = new Pen(borderColor, borderWidth))
			{
				graphics.DrawRectangle(pen, borderRectangle);
			}
		}

		private void AddTileControls(ImageBox imageBox)
		{
			this.SuspendLayout();

			foreach (Tile tile in imageBox.Tiles)
				AddTileControl(tile);

			// keep the image scroller at the forefront
			_imageScroller.BringToFront();

			this.ResumeLayout(false);
		}

		private void AddTileControl(Tile tile)
		{
			TileView view = ViewFactory.CreateAssociatedView(typeof(Tile)) as TileView;

			view.Tile = tile;
			view.ParentRectangle = this.AvailableClientRectangle;
			view.ParentImageBoxInsetWidth = ImageBox.InsetWidth;

			TileControl control = view.GuiElement as TileControl;
			control.Tile.SelectionChanged += OnTileSelectionChanged;
            control.MouseMove += OnTileMouseMove;
			control.Drawing += OnTileControlDrawing;

			control.SuspendLayout();
			this.Controls.Add(control);
			control.ResumeLayout(false);
		}

        void OnTileMouseMove(object sender, MouseEventArgs e)
        {
            TileControl tileControl = sender as TileControl;
            var screenPt = tileControl.PointToScreen(new Point(e.X, e.Y));
            var imageBoxPos = this.PointToClient(screenPt);
            MouseEventArgs @event = new MouseEventArgs(e.Button, e.Clicks, imageBoxPos.X, imageBoxPos.Y, e.Delta);
            OnMouseMove(@event);
        }

		private void SetTileParentImageBoxRectangles(bool suppressDraw)
		{
			foreach (TileControl control in this.TileControls)
				control.SetParentImageBoxRectangle(this.AvailableClientRectangle, ImageBox.InsetWidth, suppressDraw);
		}

		#endregion

		#region ImageBoxControl Scroll-stacking Support

    	private bool ImageScrollerVisible
    	{
			get { return _imageScrollerVisible; }
			set
			{
				//when we switch workspaces, _imageScroller.Visible changes to false.  But, for our calculations,
				//we don't care about whether or not it's really visible, just whether or not it should be visible.
				if (_imageScrollerVisible != value)
					_imageScroller.Visible = _imageScrollerVisible = value;
			}
    	}

		/// <summary>
		/// Gets the <see cref="Control.ClientRectangle"/> of this control, less any area dedicated to the ImageBoxControl scrollbar.
		/// </summary>
    	public Rectangle AvailableClientRectangle
    	{
    		get
    		{
				Rectangle clientRectangle = this.ClientRectangle;
				if (ImageScrollerVisible)
					clientRectangle.Width -= _imageScroller.Width - ImageBox.InsetWidth;
    			return clientRectangle;
    		}
    	}
        
    	private void ImageScrollerMouseDown(object sender, EventArgs e)
    	{
    		if(_imageBox != null)
    		{
				if (_imageBox.Tiles.Count > 0)
				{
					if (!_imageBox.Selected)
						_imageBox.SelectDefaultTile();

					TileControl tileControl = GetTileControl(_imageBox.SelectedTile);
					if (tileControl != null)
						tileControl.Focus();

					BeginCaptureUndo();
				}
    		}
    	}

		private void ImageScrollerMouseUp(object sender, EventArgs e)
		{
			EndCaptureUndo();
		}

		private void BeginCaptureUndo()
		{
			_historyCommand = new CompositeUndoableCommand();
			DrawableUndoableCommand drawableUndoableCommand = new DrawableUndoableCommand(_imageBox);
			_imageBoxCommand = new MemorableUndoableCommand(_imageBox);
			_imageBoxCommand.BeginState = _imageBox.CreateMemento();
			drawableUndoableCommand.Enqueue(_imageBoxCommand);
			_historyCommand.Enqueue(drawableUndoableCommand);
		}

		private void EndCaptureUndo()
		{
			if (_imageBoxCommand != null)
			{
				_imageBoxCommand.EndState = _imageBox.CreateMemento();
				if (!_imageBoxCommand.BeginState.Equals(_imageBoxCommand.EndState))
				{
					_historyCommand.Name = SR.CommandNameStackImageScroller;
					_imageBox.ImageViewer.CommandHistory.AddCommand(_historyCommand);
				}
			}

			_imageBoxCommand = null;
			_historyCommand = null;
		}

    	private void ImageScrollerValueChanged(object sender, TrackSlider.ValueChangedEventArgs e)
    	{
			if (e.UserAction == TrackSlider.UserAction.None)
			{
				//The change has occurred due to external forces ... so, drawing is up to the external force!
				_imageScroller.Update();
			}
			else
			{
				//we only draw the image box when focused because the user is actually dragging the scrollbar!
				_imageBox.TopLeftPresentationImageIndex = _imageScroller.Value;

				// make sure the scrollbar draws immediately!
				_imageScroller.Update();

				// this ordering of draw makes it look smoother for some reason.
				_imageBox.Draw();
			}
    	}

		private void UpdateImageScroller()
		{

			//This method can be called repeatedly and will essentially be a no-op if nothing needs to change.
			//In tiled mode, it could be a little inefficient to call repeatedly, but it's the lesser of the evils.
			//Otherwise, we're subscribing to a multitude of events and updating different things at different times.
			//Not to mention, that doesn't cover every case, like sorting images.  It's nothing compared to
			//the cost of updating the scroll control itself, anyway.

			CodeClock clock = new CodeClock();
			clock.Start();

			bool visibleBefore = ImageScrollerVisible;
			bool visibleNow = false;

			IDisplaySet displaySet = _imageBox.DisplaySet;
    		if (displaySet != null)
    		{
				int tileCount = _imageBox.Tiles.Count;
				int maximum = Math.Max(0, displaySet.PresentationImages.Count - tileCount);
				if (maximum > 0)
				{
					visibleNow = true;

					int topLeftIndex = Math.Max(0, _imageBox.TopLeftPresentationImageIndex);
					_imageScroller.SetValueAndRange(topLeftIndex, 0, maximum);
					_imageScroller.Increment = Math.Max(1, tileCount);
					_imageScroller.Value = topLeftIndex;
				}
    		}

			if (visibleBefore != visibleNow)
			{
				ImageScrollerVisible = visibleNow;
				//UpdateImageScroller is only called right before a Tile is drawn, so we suppress
				//the Tile drawing as a result of a size change here.
				SetTileParentImageBoxRectangles(true);
			}

			clock.Stop();
			//Trace.WriteLine(String.Format("UpdateScroller: {0}", clock));
		}

		#endregion

        #region ImageBox Extension support


        ///<summary>
        /// Gets a list of plugins for the image box that implementing <see cref="IImageBoxExtension"/>
        ///</summary>
        public IEnumerable<IImageBoxExtension> Extensions
        {
            get
            {

                LoadExtensions();
                return _extensions;
            }
        }


        private void LoadExtensions()
        {
            if (_extensions == null)
            {
                _extensions = new List<IImageBoxExtension>();
                foreach (IImageBoxExtension extension in new ImageBoxExtensionPoint().CreateExtensions())
                {
                    extension.ImageBox = this.ImageBox;
                    _extensions.Add(extension);
                }
            }

        }

        private void DisposeExtensions()
        {
            if (_extensions != null)
            {
                foreach (var ext in _extensions)
                {
                    try
                    {
                        DisposeExtension(ext);
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Warn, ex, "Error occurred while disposing {0} ImageBox extension", ext.Name);
                    }
                }

                _extensions.Clear();
            }

        }
        
        void AttachExtension(IImageBoxExtension extension)
        {
            IImageBoxExtensionView view;
            if (!_extensionViews.TryGetValue(extension, out view))
            {
                extension.PropertyChanged += OnExtensionPropertyChanged;

                view = extension.CreateView();
                if (view!=null)
                {
                    view.ParentSize = Size;
                    _extensionViews.Add(extension, view);

                    if (extension.Visible)
                    {
                        Control ctrl = view.GuiElement as Control;
                        if (ctrl != null)
                        {
                            AddExtensionControl(ctrl);
                        }
                    }
                }
            }
      
        }

        void AddExtensionControl(Control control)
        {
            if (Controls.Contains(control))
                return;

            // TODO: We should let extension decide if it wants to handle these events first
            control.MouseDown += OnExtensionMouseDown;
            control.KeyDown += OnExtensionKeyDown;
            control.KeyUp += OnExtensionKeyUp;
            Controls.Add(control);
        }


        IImageBoxExtensionView FindExtensionView(IImageBoxExtension extension)
        {
            IImageBoxExtensionView view;
            if (_extensionViews.TryGetValue(extension, out view))
            {
                return view;
            }

            return null;
        }
        
        void DisposeExtension(IImageBoxExtension extension)
        {
            extension.PropertyChanged -= OnExtensionPropertyChanged;

            // Dispose the extenion's view which was obtained using CreateView()
            // The extension itself will be disposed when the image box is disposed
            IImageBoxExtensionView view;
            if (_extensionViews.TryGetValue(extension, out view))
            {
                _extensionViews.Remove(extension);
                view.Dispose();
            }

            extension.Dispose();
        }

        void OnExtensionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var extension = sender as IImageBoxExtension;

            Platform.CheckForNullReference(extension, "extension");
            
            if (e.PropertyName == "Visible")
            {
                var view = FindExtensionView(extension);
                if (view!=null)
                {
                    Control ctrl = view.GuiElement as Control;
                    if (ctrl!=null)
                    {
                        if (extension.Visible)
                        {
                            AddExtensionControl(ctrl);

                            // make sure it's on top of the images or if the display set is empty, 
                            // it's on top of the empty tiles
                            ctrl.BringToFront();
                        }
                        else
                        {
                            Draw();
                            Update();
                        }
                    }
                    
                }
            }
        }

        void OnExtensionKeyUp(object sender, KeyEventArgs e)
        {
            // let tile controls handle it first
            foreach (var tileControl in TileControls)
            {
                tileControl.ProcessKeyUp(e);
                if (e.Handled)
                    return;
            }

            if (e.Handled)
                return;

            OnKeyUp(e);
        }

        void OnExtensionKeyDown(object sender, KeyEventArgs e)
        {
            // let tile controls handle it first
            foreach (var tileControl in TileControls)
            {
                tileControl.ProcessKeyDown(e);
                if (e.Handled)
                    return;
            }

            if (e.Handled)
                return;

            OnKeyDown(e);
        }

        void OnExtensionMouseDown(object sender, MouseEventArgs e)
        {
            _imageBox.SelectDefaultTile();
        }

        #endregion
    }

    
}
