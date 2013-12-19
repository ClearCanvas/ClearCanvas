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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Rendering;
using DrawMode = ClearCanvas.ImageViewer.Rendering.DrawMode;
using Screen = System.Windows.Forms.Screen;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
	/// </summary>
	public partial class TileControl : UserControl
	{
		#region Private fields

		private Tile _tile;
		private TileInputTranslator _inputTranslator;
		private TileController _tileController;

		private InformationBox _currentInformationBox;

		private EditBoxControl _editBoxControl;

		private IRenderingSurface _surface;
		private IMouseButtonHandler _currentMouseButtonHandler;
		private CursorWrapper _currentCursorWrapper;

		private bool _suppressDrawOnSizeChanged = false;
		private string _lastRenderExceptionMessage = null;

		[ThreadStatic]
		private static float? _dpi;

		[ThreadStatic]
		private static bool _isDrawing = false;

		[ThreadStatic]
		private static bool _painting = false;

		[ThreadStatic]
		private static readonly List<TileControl> _tilesToRepaint = new List<TileControl>();

		private event EventHandler _drawing;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public TileControl(Tile tile, Rectangle parentRectangle, int parentImageBoxInsetWidth)
		{
			_tile = tile;
			_tileController = new TileController(_tile, (_tile.ImageViewer as ImageViewerComponent).ShortcutManager);
			_inputTranslator = new TileInputTranslator(this);

			SetParentImageBoxRectangle(parentRectangle, parentImageBoxInsetWidth, true);
			InitializeComponent();

			this.BackColor = Color.Black;
			this.Dock = DockStyle.None;
			this.Anchor = AnchorStyles.None;
			this.AllowDrop = true;

			_tile.Drawing += new EventHandler(OnTileDrawing);
			_tile.RendererChanged += new EventHandler(OnRendererChanged);
			_tile.InformationBoxChanged += new EventHandler<InformationBoxChangedEventArgs>(OnInformationBoxChanged);
			_tile.EditBoxChanged += new EventHandler(OnEditBoxChanged);
			_tile.SelectionChanged += new EventHandler<ItemEventArgs<ITile>>(OnTileSelectionChanged);

			_contextMenuStrip.ImageScalingSize = new Size(24, 24);
			_contextMenuStrip.Opening += new CancelEventHandler(OnContextMenuStripOpening);

			_tileController.CursorTokenChanged += new EventHandler(OnCursorTokenChanged);
			_tileController.ContextMenuRequested += new EventHandler<ItemEventArgs<Point>>(OnContextMenuRequested);
			_tileController.CaptureChanging += new EventHandler<ItemEventArgs<IMouseButtonHandler>>(OnCaptureChanging);

			_editBoxControl = new EditBoxControl();
			this.Controls.Add(_editBoxControl);

			this.DoubleBuffered = false;
			this.SetStyle(ControlStyles.DoubleBuffer, false);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
		}

		public Tile Tile
		{
			get { return _tile; }
		}

		private float Dpi
		{
			get
			{
				if (!_dpi.HasValue)
				{
					using (var g = CreateGraphics())
						_dpi = (g.DpiX + g.DpiY)/2;
				}
				return _dpi.Value;
			}
		}

		private IRenderingSurface Surface
		{
			get
			{
				if (_surface == null)
				{
					// TileControl should *always* have an associated Tile
					if (this.Tile == null)
						throw new Exception(SR.ExceptionTileControlNoAssociatedTile);

					// Legitimate case; Tile maybe empty
					if (this.Tile.PresentationImage == null)
						return null;

					IRenderer renderer = ((PresentationImage) Tile.PresentationImage).ImageRenderer;

					// PresntationImage should *always* have a renderer
					if (renderer == null)
						throw new Exception(SR.ExceptionPresentationImageNotAssociatedWithARenderer);

					_surface = renderer.CreateRenderingSurface(this.Handle, this.Width, this.Height, RenderingSurfaceType.Onscreen);
					_surface.Invalidated += OnSurfaceInvalidated;
				}

				return _surface;
			}
		}

		private void OnSurfaceInvalidated(object sender, EventArgs e)
		{
			Invalidate();
		}

		public void SetParentImageBoxRectangle(Rectangle parentImageBoxRectangle, int parentImageBoxBorderWidth, bool suppressDraw)
		{
			int insetImageBoxWidth = parentImageBoxRectangle.Width - 2*parentImageBoxBorderWidth;
			int insetImageBoxHeight = parentImageBoxRectangle.Height - 2*parentImageBoxBorderWidth;

			int left = (int) (_tile.NormalizedRectangle.Left*insetImageBoxWidth + Tile.InsetWidth);
			int top = (int) (_tile.NormalizedRectangle.Top*insetImageBoxHeight + Tile.InsetWidth);
			int right = (int) (_tile.NormalizedRectangle.Right*insetImageBoxWidth - Tile.InsetWidth);
			int bottom = (int) (_tile.NormalizedRectangle.Bottom*insetImageBoxHeight - Tile.InsetWidth);

			_suppressDrawOnSizeChanged = suppressDraw;

			try
			{
				this.SuspendLayout();

				this.Location = new Point(left + parentImageBoxBorderWidth, top + parentImageBoxBorderWidth);
				this.Size = new Size(right - left, bottom - top);
				this.ResumeLayout(false);
			}
			finally
			{
				_suppressDrawOnSizeChanged = false;
			}
		}

		public event EventHandler Drawing
		{
			add { _drawing += value; }
			remove { _drawing -= value; }
		}

		public void Draw()
		{
			_tile.Draw();
		}

		private void DoDraw()
		{
			EventsHelper.Fire(_drawing, this, EventArgs.Empty);

			CodeClock clock = new CodeClock();
			clock.Start();

			if (this.Surface != null)
			{
				System.Drawing.Graphics graphics = this.CreateGraphics();

				this.Surface.WindowID = this.Handle;
				this.Surface.ContextID = graphics.GetHdc();
				this.Surface.ClientRectangle = this.ClientRectangle;
				this.Surface.ClipRectangle = this.ClientRectangle;

				DrawArgs args = new DrawArgs(this.Surface,
				                             new WinFormsScreenProxy(Screen.FromControl(this)),
				                             DrawMode.Render) {Dpi = Dpi};

				_isDrawing = true;

				try
				{
					_tile.Draw(args);

					_lastRenderExceptionMessage = null;
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "An error has occured while rendering the contents of a tile.");

					_lastRenderExceptionMessage = ex is RenderingException ? ((RenderingException) ex).UserMessage : ex.Message;

					// we cannot simply pass the existing Graphics because we haven't released its hDC yet
					// if we do, we'll get a "Object is currently in use elsewhere" exception
					DrawErrorMessage(_lastRenderExceptionMessage, Surface.ContextID, ClientRectangle);
				}
				finally
				{
					graphics.ReleaseHdc(this.Surface.ContextID);
					graphics.Dispose();

					_isDrawing = false;
				}
			}

			//Cause the tile to paint/refresh.
			Invalidate();
			Update();

			clock.Stop();
			string str = String.Format("TileControl.Draw: {0}, {1}\n", clock.ToString(), this.Size.ToString());
			Trace.Write(str);
		}

		private void DisposeSurface()
		{
			try
			{
				if (_surface != null)
				{
					_surface.Invalidated -= OnSurfaceInvalidated;
					_surface.Dispose();
				}
			}
			finally
			{
				_surface = null;
			}
		}

		private static void DrawErrorMessage(string errorMessage, IntPtr hDC, Rectangle bounds)
		{
			using (var errorGraphics = System.Drawing.Graphics.FromHdc(hDC))
			{
				using (var format = new StringFormat
				                    	{
				                    		Trimming = StringTrimming.EllipsisCharacter,
				                    		Alignment = StringAlignment.Center,
				                    		LineAlignment = StringAlignment.Center,
				                    		FormatFlags = StringFormatFlags.NoClip
				                    	})
				{
					// use the system-determined default font to ensure we can't fail at drawing error messages (cause some systems might not have Arial)
					using (var font = new Font(SystemFonts.DefaultFont.Name, 12.0f))
					{
						// we don't clear the background before drawing the error message, so give it a drop shadow effect
						errorGraphics.DrawString(errorMessage, font, Brushes.Black, new Rectangle(new Point(1, 1), bounds.Size), format);
						errorGraphics.DrawString(errorMessage, font, Brushes.WhiteSmoke, bounds, format);
					}
				}
			}
		}

		#region Overrides

		private void OnTileSelectionChanged(object sender, ItemEventArgs<ITile> e)
		{
			if (e.Item.Selected)
			{
				bool successful = Focus();
				if (!successful)
					Trace.WriteLine("Failed to set focus on tile - there must be a modal dialog in front!");
			}
		}

		private void OnTileDrawing(object sender, EventArgs e)
		{
			DoDraw();
		}

		private void OnRendererChanged(object sender, EventArgs e)
		{
			DisposeSurface();
		}

		private bool IsVistaOrLater()
		{
			return Environment.OSVersion.Version.Major >= 6;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//Assume anything Vista or later has the same issues.
			if (IsVistaOrLater())
			{
				//Windows Vista is opportunistic when it comes to wait conditions (e.g. locks, Mutexes, etc)
				//in that it will actually process WM_PAINT messages on the current thread, even though
				//it is supposed to be blocking in a WaitSleepJoin state.  This behaviour can actually
				//break rendering for a couple of reasons:
				//  1. We do custom double-buffering, and it's possible that we could process a paint message
				//     for an image that hasn't actually been rendered to the back buffer yet.
				//  2. The renderer itself accesses the pixel data of the ImageSops, which is a synchronized operation.
				//     In the case where 2 threads try to load the pixel data of an image simultaneously, the renderer can end up
				//     blocking execution on the main UI thread in the middle of a rendering operation.  If we
				//     allow another tile to paint in this situation, it actually causes some GDI errors because
				//     the previous rendering operation has not yet completed.
				if (_isDrawing || _painting)
				{
					e.Graphics.Clear(Color.Black);

					//Queue this tile up for deferred painting and return.
					if (!_tilesToRepaint.Contains(this))
						_tilesToRepaint.Add(this);

					return;
				}

				//We're about to paint this tile, so remove it from the queue.
				_tilesToRepaint.Remove(this);
			}

			if (_tile.PresentationImage == null)
				DisposeSurface();

			if (this.Surface == null)
			{
				// Make sure tile gets blacked out if there's
				// no presentation image in it
				e.Graphics.Clear(Color.Black);
			}
			else
			{
				this.Surface.WindowID = this.Handle;
				this.Surface.ContextID = e.Graphics.GetHdc();
				this.Surface.ClientRectangle = this.ClientRectangle;
				this.Surface.ClipRectangle = e.ClipRectangle;

				DrawArgs args = new DrawArgs(this.Surface,
				                             new WinFormsScreenProxy(Screen.FromControl(this)),
				                             DrawMode.Refresh) {Dpi = Dpi};

				_painting = true;

				try
				{
					_tile.Draw(args);

					// if an exception was encountered the last time we rendered the buffer, refresh the error text now
					if (!string.IsNullOrEmpty(_lastRenderExceptionMessage))
					{
						// we cannot simply pass the Graphics because we haven't released its hDC yet
						// if we do, we'll get a "Object is currently in use elsewhere" exception
						DrawErrorMessage(_lastRenderExceptionMessage, Surface.ContextID, ClientRectangle);
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "An error has occured while refreshing the contents of a tile.");

					var exceptionMessage = ex is RenderingException ? ((RenderingException) ex).UserMessage : ex.Message;

					// we cannot simply pass the existing Graphics because we haven't released its hDC yet
					// if we do, we'll get a "Object is currently in use elsewhere" exception
					DrawErrorMessage(exceptionMessage, Surface.ContextID, ClientRectangle);
				}
				finally
				{
					e.Graphics.ReleaseHdc(this.Surface.ContextID);

					_painting = false;
				}
			}

			// Now that we've finished painting this tile, we can process the deferred paint jobs.
			// The code below is self-fulfilling, in that we remove one tile from the queue and
			// invalidate it, causing it to paint.  When it's done painting, it will remove and
			// invalidate the next one, and so on.
			if (IsVistaOrLater() && _tilesToRepaint.Count > 0)
			{
				TileControl tileToRepaint = _tilesToRepaint[0];
				_tilesToRepaint.RemoveAt(0);

				tileToRepaint.Invalidate();
				tileToRepaint.Update();
			}

			//base.OnPaint(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// We're double buffering manually, so override this and do nothing
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			Trace.Write("TileControl.OnSizeChanged()\n");

			if (_tileController != null)
				_tileController.TileClientRectangle = this.ClientRectangle;

			if (!_suppressDrawOnSizeChanged)
				DoDraw();
		}

		#region Mouse/Keyboard Overrides

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			object message = _inputTranslator.OnLostFocus();
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			object message = _inputTranslator.OnMouseLeave();
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();

			object message = _inputTranslator.OnMouseDown(e);
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			bool handled = false;
			object message = _inputTranslator.OnMouseMove(e);
			if (message != null)
			{
				if (_tileController != null)
					handled = _tileController.ProcessMessage(message);
			}

			if (!handled)
			{
				base.OnMouseMove(e);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseUp(e);
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseWheel(e);
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			base.OnMouseCaptureChanged(e);

			//This feels bad to me, but it's the only way to accomplish
			//keeping the capture when the mouse has come up.  .NET automatically handles
			//capture for you by turning it on on mouse down and off on mouse up, but
			//it does not allow you to keep capture when the mouse is not down.  Even
			// if you take out the calls to the base class OnMouseX handlers, it still
			// turns Capture back off although it has been turned on explicitly.
			if (this._currentMouseButtonHandler != null)
				this.Capture = true;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			object message = _inputTranslator.OnKeyDown(e);
			if (message == null)
				return;

			if (_tileController != null && _tileController.ProcessMessage(message))
				e.Handled = true;

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			object message = _inputTranslator.OnKeyUp(e);
			if (message == null)
				return;

			if (_tileController != null && _tileController.ProcessMessage(message))
				e.Handled = true;

			base.OnKeyUp(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			//We want the tile control to receive keydown messages for *all* keys.
			return true;
		}

		public override bool PreProcessMessage(ref Message msg)
		{
			object message = _inputTranslator.PreProcessMessage(msg);
			if (message != null && _tileController != null)
				_tileController.ProcessMessage(message);

			bool returnValue = base.PreProcessMessage(ref msg);

			message = _inputTranslator.PostProcessMessage(msg, returnValue);
			if (message != null && _tileController != null)
				_tileController.ProcessMessage(message);

			return returnValue;
		}

		#endregion

		#region Drag/Drop Overrides

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(typeof (DisplaySet)) && !_tile.ParentImageBox.DisplaySetLocked)
				drgevent.Effect = DragDropEffects.Move;
			else
				drgevent.Effect = DragDropEffects.None;

			base.OnDragOver(drgevent);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			_tile.Select();

			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(_tile.ParentImageBox);
			memorableCommand.BeginState = _tile.ParentImageBox.CreateMemento();

			IDisplaySet displaySet = (IDisplaySet) drgevent.Data.GetData(typeof (DisplaySet));
			_tile.ParentImageBox.DisplaySet = displaySet.CreateFreshCopy();
			_tile.ParentImageBox.Draw();

			memorableCommand.EndState = _tile.ParentImageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(_tile.ParentImageBox);
			historyCommand.Enqueue(memorableCommand);
			_tile.ImageViewer.CommandHistory.AddCommand(historyCommand);

			base.OnDragDrop(drgevent);
		}

		#endregion

		protected override void OnHandleDestroyed(EventArgs e)
		{
			// Notify the surface that the tile control's window handle is
			// about to be destroyed so that any objects using the handle have
			// a chance to deal with it
			if (_surface != null)
				_surface.WindowID = IntPtr.Zero;

			base.OnHandleDestroyed(e);
		}

		#endregion

		private void OnContextMenuRequested(object sender, ItemEventArgs<Point> e)
		{
			_contextMenuStrip.Show(this, e.Item);
		}

		private void OnCaptureChanging(object sender, ItemEventArgs<IMouseButtonHandler> e)
		{
			if (_currentMouseButtonHandler == e.Item)
				return;

			_currentMouseButtonHandler = e.Item;
			this.Capture = (_currentMouseButtonHandler != null);
		}

		private void OnCursorTokenChanged(object sender, EventArgs e)
		{
			if (_tileController == null)
				return;

			if (_tileController.CursorToken == null)
			{
				this.Cursor = this.DefaultCursor;

				if (_currentCursorWrapper != null)
				{
					_currentCursorWrapper.Dispose();
					_currentCursorWrapper = null;
				}
			}
			else
			{
				try
				{
					CursorWrapper oldCursorWrapper = _currentCursorWrapper;
					_currentCursorWrapper = CursorFactory.CreateCursor(_tileController.CursorToken);
					this.Cursor = _currentCursorWrapper.Cursor;

					if (oldCursorWrapper != null)
						oldCursorWrapper.Dispose();
				}
				catch (Exception exception)
				{
					Platform.Log(LogLevel.Error, exception);
					this.Cursor = this.DefaultCursor;
					_currentCursorWrapper = null;
				}
			}
		}

		private void OnContextMenuStripOpening(object sender, CancelEventArgs e)
		{
			if (_tileController == null || _tileController.ContextMenuProvider == null)
			{
				e.Cancel = true;
				return;
			}

			if (_tileController.ContextMenuEnabled)
			{
				ActionModelNode menuModel = _tileController.ContextMenuProvider.GetContextMenuModel(_tileController);
				if (menuModel != null && menuModel.ChildNodes.Count > 0)
				{
					ToolStripBuilder.Clear(_contextMenuStrip.Items);
					ToolStripBuilder.BuildMenu(_contextMenuStrip.Items, menuModel.ChildNodes);

					// filter unavailable items out of list since they wreck the display of the overflow and scroll portions of the menu dropdown
					// (ClearCanvas Ticket #4775, Microsoft Connect Issue #136061)
					FilterUnavailableItems(_contextMenuStrip.Items);

					// cancel list if, after filtering, no items are left to display.
					e.Cancel = (_contextMenuStrip.Items.Count == 0);
				}
				else
				{
					e.Cancel = true;
				}
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void OnInformationBoxChanged(object sender, InformationBoxChangedEventArgs e)
		{
			if (_currentInformationBox != null)
				_currentInformationBox.Updated -= new EventHandler(OnUpdateInformationBox);

			_currentInformationBox = e.InformationBox;

			_toolTip.Active = false;
			_toolTip.Hide(this);

			if (e.InformationBox != null)
				_currentInformationBox.Updated += new EventHandler(OnUpdateInformationBox);
		}

		private void OnUpdateInformationBox(object sender, EventArgs e)
		{
			if (!_currentInformationBox.Visible)
			{
				_toolTip.Active = false;
				_toolTip.Hide(this);
			}
			else
			{
				_toolTip.Active = true;
				Point point = new Point(_currentInformationBox.DestinationPoint.X, _currentInformationBox.DestinationPoint.Y);
				point.Offset(5, 5);
				_toolTip.Show(_currentInformationBox.Data, this, point);
			}
		}

		private void OnEditBoxChanged(object sender, EventArgs e)
		{
			_editBoxControl.EditBox = _tile.EditBox;
		}

		private static void FilterUnavailableItems(ToolStripItemCollection items)
		{
			List<ToolStripItem> unavailableItems = new List<ToolStripItem>();
			foreach (ToolStripItem toolStripItem in items)
			{
				if (!toolStripItem.Available)
				{
					unavailableItems.Add(toolStripItem);
				}
				else if (toolStripItem is ToolStripMenuItem)
				{
					ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem) toolStripItem;
					if (toolStripMenuItem.DropDownItems.Count > 0)
					{
						FilterUnavailableItems(toolStripMenuItem.DropDownItems);
						if (toolStripMenuItem.DropDownItems.Count == 0)
							unavailableItems.Add(toolStripItem);
					}
				}
			}

			foreach (ToolStripItem unavailableItem in unavailableItems)
			{
				items.Remove(unavailableItem);
				unavailableItem.Dispose();
			}
		}

		internal void ProcessKeyUp(KeyEventArgs ev)
		{
			OnKeyUp(ev);
		}

		internal void ProcessKeyDown(KeyEventArgs args)
		{
			OnKeyDown(args);
		}
	}
}