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
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// This class controls the behaviour of objects in the <see cref="ITile"/>, namely the <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (<see cref="PresentationImage.SceneGraph"/>) and <see cref="ClearCanvas.Desktop.Tools.ITool"/>s 
	/// belonging to the current <see cref="IPresentationImage"/>, specifically those that implement <see cref="IMouseButtonHandler"/> and/or <see cref="IMouseWheelHandler"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// - The <see cref="TileController"/> receives messages from the view layer, interprets them, and allows the appropriate domain objects to handle
	/// the messages in a prescribed manner.  Here is a (highly simplified) description of how it works:
	/// <list>
	/// <item>
	/// <description>
	/// - When the mouse moves without any buttons down, all <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s 
	/// that implement <see cref="IMouseButtonHandler"/> have their <see cref="IMouseButtonHandler.Track"/> method called.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// - When a mouse button is clicked, an <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/> is searched for at the current mouse position
	/// that implements <see cref="IMouseButtonHandler"/>.  If one is found, it is given 'capture' until it releases capture or capture is canceled by the framework.
	/// If no <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/> is found, then all of the current <see cref="IPresentationImage"/>'s 
	/// <see cref="ClearCanvas.Desktop.Tools.ITool"/>s are searched for an <see cref="IMouseButtonHandler"/> and the same rules are applied.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// - When the right mouse button is clicked, the same thing occurs as for the left mouse button, but when it is released, a context menu is shown
	/// provided the mouse didn't move more than a couple of pixels.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// When the mouse wheel is used, a similar approach is taken as mentioned above for <see cref="IMouseButtonHandler"/>s, but for <see cref="IMouseWheelHandler"/>s.  
	/// However, only <see cref="ClearCanvas.Desktop.Tools.ITool"/>s are given the opportunity to handle the mouse wheel, not <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s.
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// Note that this object is instantiated from within the view layer and cannot be accessed from application or domain level code.  
	/// This is deliberate as it is intended for internal framework use only.
	/// </para>
	/// </remarks>
	/// <seealso cref="IMouseButtonHandler"/>
	/// <seealso cref="IMouseWheelHandler"/>
	/// <seealso cref="IMouseInformation"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="IPresentationImage"/>
	/// <seealso cref="PresentationImage"/>
	/// <seealso cref="PresentationImage.SceneGraph"/>
	/// <seealso cref="ClearCanvas.Desktop.Tools.ITool"/>
	/// <seealso cref="ClearCanvas.ImageViewer.BaseTools.ImageViewerTool"/>
	/// <seealso cref="ClearCanvas.ImageViewer.BaseTools.MouseImageViewerTool"/>
	public sealed class TileController : IMouseInformation, IDisposable
	{
		private delegate bool CallHandlerMethodDelegate(IMouseButtonHandler handler);

        #region Action Model Provider

        /// <summary>
        /// A proxy class for providing an action model directly to the tile control in order to show
        /// a context menu without the user having to click.
        /// </summary>
        private class ActionModelProvider : IContextMenuProvider, IMouseButtonHandler
        {
            private readonly ActionModelNode _actionModel;

            public ActionModelProvider(ActionModelNode actionModel)
            {
                _actionModel = actionModel;
            }

            #region IContextMenuProvider Members

            public ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
            {
                return _actionModel;
            }

            #endregion

            #region IMouseButtonHandler Members

            public bool Start(IMouseInformation mouseInformation)
            {
                return false;
            }

            public bool Track(IMouseInformation mouseInformation)
            {
                return false;
            }

            public bool Stop(IMouseInformation mouseInformation)
            {
                return false;
            }

            public void Cancel()
            {
            }

            public MouseButtonHandlerBehaviour Behaviour
            {
                get { return MouseButtonHandlerBehaviour.Default; }
            }

            #endregion
        } 
        #endregion

		#region MouseWheelManager class 

		private class MouseWheelManager
		{
			private readonly int WheelStopDelayMilliseconds = 500;

			//One per UI Thread.
			[ThreadStatic]
			private static MouseWheelManager _instance;

		    private TileController _captureTileController;
			private IMouseWheelHandler _captureMouseWheelHandler;

		    private DelayedEventPublisher _delayedStop;

			private MouseWheelManager()
			{
			}

			public static MouseWheelManager Instance
			{
				get { return _instance ?? (_instance = new MouseWheelManager()); }
			}

            public IMouseWheelHandler GetCaptureHandler(TileController tileController)
            {
                if (_captureTileController == tileController)
                    return _captureMouseWheelHandler;

                return null;
            }

			public void SetCaptureHandler(TileController tileController, IMouseWheelHandler captureMouseWheelHandler)
			{
                if (_captureTileController == tileController && _captureMouseWheelHandler == captureMouseWheelHandler)
					return;

				if (_captureMouseWheelHandler != null)
				{
                    if (_delayedStop != null)
                    {
                        _delayedStop.Dispose();
                        _delayedStop = null;
                    }

                    EventsHelper.Fire(tileController._wheelCaptureChangingEvent, tileController, new ItemEventArgs<IMouseWheelHandler>(null));
                    _captureMouseWheelHandler.StopWheel();
                    _captureTileController._tile.ImageViewer.EventBroker.OnMouseWheelCaptureChanged(new MouseWheelCaptureChangedEventArgs(_captureTileController._tile, false));
				}

			    _captureTileController = tileController;
				_captureMouseWheelHandler = captureMouseWheelHandler;
                
				if (_captureMouseWheelHandler == null)
				{
                    //This is only needed when the capture mouse wheel handler is not null.
				    _captureTileController = null;
                    if (_delayedStop != null)
                    {
                        _delayedStop.Dispose();
                        _delayedStop = null;
                    }
                    return;
				}

                _delayedStop = new DelayedEventPublisher((s, e) => SetCaptureHandler(tileController, null), WheelStopDelayMilliseconds);

                EventsHelper.Fire(tileController._wheelCaptureChangingEvent, tileController, new ItemEventArgs<IMouseWheelHandler>(_captureMouseWheelHandler));
                _captureMouseWheelHandler.StartWheel();
                _captureTileController._tile.ImageViewer.EventBroker.OnMouseWheelCaptureChanged(new MouseWheelCaptureChangedEventArgs(_captureTileController._tile, true));

                _delayedStop.Publish(this, EventArgs.Empty);
			}

		    public void OnMouseWheel()
		    {
                if (_delayedStop != null)
                    _delayedStop.Publish(this, EventArgs.Empty);
		    }
		}

		#endregion
		
		#region Private Fields

		private readonly Tile _tile;
		private bool _selectedOnThisClick;
		private bool _capturedOnThisClick;
		private Point _startMousePoint;
		private Point _currentMousePoint;
		private Rectangle _tileClientRectangle;

		private MemorableUndoableCommand _command;
		private IMouseButtonHandler _captureHandler;
		private int _startCount = 0;
		private CursorToken _cursorToken;

		private int _mouseMovedToleranceInPixel = 2;
		private int _contextMenuDelayInMilliseconds;
		private int _mouseHoldDownForContextMenuInMilliseconds;
		private long? _lastMouseDownProcessedTicks;
		private XMouseButtons _buttonForContextMenu = XMouseButtons.Right;
		private MouseButtonMessage.ButtonActions _buttonActionForContextMenu = MouseButtonMessage.ButtonActions.Up;
		private bool _contextMenuEnabled; 
		private IContextMenuProvider _contextMenuProvider;

		private event EventHandler _cursorTokenChanged;
		private event EventHandler<ItemEventArgs<Point>> _contextMenuRequested;
		private event EventHandler<ItemEventArgs<IMouseButtonHandler>> _captureChangingEvent;
        private event EventHandler<ItemEventArgs<IMouseWheelHandler>> _wheelCaptureChangingEvent;

		private XMouseButtons _activeButton;
		private uint _clickCount;

		private readonly IViewerShortcutManager _shortcutManager;
		private DelayedEventPublisher _delayedContextMenuRequestPublisher;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public TileController(Tile tile, IViewerShortcutManager shortcutManager)
		{
			Platform.CheckForNullReference(tile, "tile");
			Platform.CheckForNullReference(shortcutManager, "shortcutManager");

			_tile = tile;
            _tile.ContextMenuRequested += ProcessExplicitContextMenuRequest;

            _selectedOnThisClick = false;
			_capturedOnThisClick = false;
			_shortcutManager = shortcutManager;
			_contextMenuDelayInMilliseconds = InputManagementSettings.Default.ContextMenuDelay;
			_delayedContextMenuRequestPublisher = new DelayedEventPublisher(ProcessDelayedContextMenuRequest, _contextMenuDelayInMilliseconds);
		}

		public void Dispose()
		{
            if (_delayedContextMenuRequestPublisher == null)
                return;

		    ReleaseCapture(true);
		    this.CaptureMouseWheelHandler = null;

			_delayedContextMenuRequestPublisher.Dispose();
		    _tile.ContextMenuRequested -= ProcessExplicitContextMenuRequest;

		    _delayedContextMenuRequestPublisher = null;
		}

		#region Private Properties

		private IMouseButtonHandler CaptureHandler
		{
			get { return _captureHandler; }
			set
			{
				if (_captureHandler == value)
					return;

				// Developer's note: make sure your memento classes override and implement
				// object.Equals so that unnecessary commands don't get added to the command
				// history.

				// Also, note that the begin state will be captured after IMouseButtonHandler.Start
				// has been called, so if your originator object is sensitive to a change 
				// that occurs in the Start() method, store it as a variable and return it
				// from CreateMemento.  The end state will be captured after the last call
				// to IMouseButtonHandler.Stop (or Cancel), so always ensure the returned 
				// end state is accurate.

				if (_captureHandler != null && _captureHandler is IMemorable)
				{
					_command.EndState = ((IMemorable) _captureHandler).CreateMemento();
					if (_command.BeginState != null && !_command.BeginState.Equals(_command.EndState))
					{
						DrawableUndoableCommand drawableCommand = new DrawableUndoableCommand(_tile);
						drawableCommand.Enqueue(_command);
						_tile.ImageViewer.CommandHistory.AddCommand(drawableCommand);
					}
					
					_command = null;
				}

				_captureHandler = value;

				if (_captureHandler != null && _captureHandler is IMemorable)
				{
					IMemorable originator = (IMemorable) _captureHandler;
					_command = new MemorableUndoableCommand(originator);
					_command.BeginState = originator.CreateMemento();
				}
				
				//fire our own event first, so the view can release 'real' capture 
				// before other notifications go out through the event broker.
				EventsHelper.Fire(_captureChangingEvent, this, new ItemEventArgs<IMouseButtonHandler>(_captureHandler));

				_tile.ImageViewer.EventBroker.OnMouseCaptureChanged(new MouseCaptureChangedEventArgs(_tile, _captureHandler != null));
			}
		}

		private IMouseWheelHandler CaptureMouseWheelHandler
		{
            set { MouseWheelManager.Instance.SetCaptureHandler(this, value); }
		    get { return MouseWheelManager.Instance.GetCaptureHandler(this); }
		}

		#endregion

		#region Private Methods

		private static bool CanStartOnDoubleClick(IMouseButtonHandler handler)
		{
			return false == CancelStartOnDoubleClick(handler);
		}

		private static bool IgnoreDoubleClicks(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.IgnoreDoubleClicks) == MouseButtonHandlerBehaviour.IgnoreDoubleClicks;
		}

		private static bool CancelStartOnDoubleClick(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.CancelStartOnDoubleClick) == MouseButtonHandlerBehaviour.CancelStartOnDoubleClick;
		}

		private static bool SuppressContextMenu(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.SuppressContextMenu) == MouseButtonHandlerBehaviour.SuppressContextMenu;
		}

		private static bool ConstrainToTile(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.ConstrainToTile) == MouseButtonHandlerBehaviour.ConstrainToTile;
		}

		private static bool SuppressOnTileActivate(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.SuppressOnTileActivate) == MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		#region Mouse Message Processing

		private void SetCursorToken(IMouseButtonHandler handler, Point location)
		{
			if (handler is ICursorTokenProvider)
			{
				this.CursorToken = (handler as ICursorTokenProvider).GetCursorToken(location);
			}
			else
			{
				this.CursorToken = null;
			}
		}

		private void SetCursorToken(IMouseButtonHandler handler)
		{
			SetCursorToken(handler, Location);
		}

		private void SetCursorToken()
		{
			SetCursorToken(CaptureHandler, Location);
		}

		private void SetCapture(IMouseButtonHandler handler)
		{
			Trace.WriteLine(String.Format("Setting capture: {0}", handler.GetType().FullName));

			this.CaptureHandler = handler;

			if (SuppressContextMenu(this.CaptureHandler))
				_contextMenuEnabled = false;

			SetCursorToken();
			//tools can't have context menus
			if (handler is IGraphic)
				this.ContextMenuProvider = handler as IContextMenuProvider;
		}

		private void ReleaseCapture(bool cancel)
		{
			if (this.CaptureHandler != null && cancel)
				this.CaptureHandler.Cancel();

			_startCount = 0;

			this.CaptureHandler = null;
			this.CursorToken = null;
			this.ContextMenuProvider = null;
		}

		private bool ProcessKeyboardMessage(KeyboardButtonMessage keyboardMessage)
		{
			//keyboard up messages are just consumed.
			if (keyboardMessage.ButtonAction == KeyboardButtonMessage.ButtonActions.Up)
				return true;

			ReleaseCapture(true);
			this.CaptureMouseWheelHandler = null;

			IClickAction action = _shortcutManager.GetKeyboardAction(keyboardMessage.Shortcut);
            Trace.WriteLine(String.Format("Finding shortcut for: {0}", keyboardMessage.Shortcut));
			if (action != null)
			{
				action.Click();
				return true;
			}

			return false;
		}

		private bool ProcessMouseWheelMessage(MouseWheelMessage wheelMessage)
		{
			if (!_tile.Enabled)
				return true;

            if (wheelMessage.WheelDelta == 0)
            {
                if (CaptureMouseWheelHandler != null)
                {
                    //NOTE: hack for Webstation; we can't totally discard mouse wheel messages because
                    //they need to reset the timer that releases "wheel capture". So, if we see a wheel delta
                    //of zero, we just don't process the message through the handler, which could cause an unwanted draw.
                    MouseWheelManager.Instance.OnMouseWheel();
                    return true;
                }

                //Invalid message; do nothing.
                return false;
            }

            ReleaseCapture(true);

			IMouseWheelHandler handler = _shortcutManager.GetMouseWheelHandler(wheelMessage.Shortcut);
			if (handler != null)
			{
				this.CaptureMouseWheelHandler = handler;
				handler.Wheel(wheelMessage.WheelDelta);

				MouseWheelManager.Instance.OnMouseWheel();
				return true;
			}

			return false;
		}

		#region Mouse Button Down

		private bool ProcessMouseButtonDownMessage(MouseButtonMessage buttonMessage)
		{
			this.CaptureMouseWheelHandler = null;
			_lastMouseDownProcessedTicks = Environment.TickCount;

			//don't allow multiple buttons, it's just cleaner and easier to manage behaviour.
			if (_activeButton != 0)
			{
				_contextMenuEnabled = false;
				return true;
			}

			_activeButton = buttonMessage.Shortcut.MouseButton;
			_clickCount = buttonMessage.ClickCount;

			if (StartCaptureHandler(buttonMessage))
				return true;

			_tile.Select();
			_contextMenuEnabled = _clickCount == 1 && _buttonForContextMenu == buttonMessage.Shortcut.MouseButton;
			if (_contextMenuEnabled && _buttonActionForContextMenu == MouseButtonMessage.ButtonActions.Down)
			{
				_delayedContextMenuRequestPublisher.TimeoutMilliseconds = _mouseHoldDownForContextMenuInMilliseconds;
				_delayedContextMenuRequestPublisher.Publish(this, new ItemEventArgs<Point>(buttonMessage.Location));
			}

			_startMousePoint = buttonMessage.Location;

			if (_tile.PresentationImage == null || !_tile.Enabled)
				return true;

			//give unfocused graphics a chance to focus (in the case of going straight from context menu to a graphic).
			FindHandlingGraphic(TrackHandler);

			return StartNewHandler(buttonMessage);
		}

		private bool StartCaptureHandler(MouseButtonMessage buttonMessage)
		{
			if (this.CaptureHandler == null)
				return false;

			if (SuppressContextMenu(this.CaptureHandler))
				_contextMenuEnabled = false;

			if (CancelStartDueToDoubleClick())
			{
				Trace.WriteLine(String.Format("Double-click release {0}", this.CaptureHandler.GetType()));
				ReleaseCapture(true);

				StartNewHandler(buttonMessage);
			}
			else
			{
				Trace.WriteLine(String.Format("Start (Clicks: {0}, Count: {1})", _clickCount, _startCount));
				StartHandler(this.CaptureHandler);
				SetCursorToken();
			}

			//we only release capture on button up, so just consume.
			return true;
		}

		private bool StartNewHandler(MouseButtonMessage buttonMessage)
		{
			if (StartNewGraphicHandler())
				return true;
			else 
				return StartNewToolHandler(buttonMessage);
		}

		private bool StartNewGraphicHandler()
		{
			if (_tile.PresentationImage is PresentationImage)
			{
				CompositeGraphic sceneGraph = ((PresentationImage)_tile.PresentationImage).SceneGraph;
				foreach (IMouseButtonHandler handler in GetHandlerGraphics(sceneGraph))
				{
					if (CanStartNewHandler(handler) && StartHandler(handler))
					{
						SetCapture(handler);
						return true;
					}
				}
			}

			return false;
		}

		private bool StartNewToolHandler(MouseButtonMessage buttonMessage)
		{
			foreach (IMouseButtonHandler handler in _shortcutManager.GetMouseButtonHandlers(buttonMessage.Shortcut))
			{
				if (CanStartNewHandler(handler) && StartHandler(handler))
				{
					SetCapture(handler);
					return true;
				}
			}

			return false;
		}

		private bool CanStartNewHandler(IMouseButtonHandler handler)
		{
			if (_clickCount < 2)
				return true;
			else if (CanStartOnDoubleClick(handler))
				return true;
			else
				return false;
		}

		private bool CancelStartDueToDoubleClick()
		{
			return (_clickCount > 1 && _startCount == 1 && CancelStartOnDoubleClick(this.CaptureHandler));
		}

		#endregion

		private bool ProcessMouseButtonUpMessage(MouseButtonMessage buttonMessage)
		{
			var timeElapsedSinceMouseDown = _lastMouseDownProcessedTicks.HasValue
				? TimeSpan.FromMilliseconds(Environment.TickCount - _lastMouseDownProcessedTicks.Value).TotalMilliseconds
				: 0.0;
			_lastMouseDownProcessedTicks = null;

			if (_activeButton != buttonMessage.Shortcut.MouseButton)
				return true;

			_activeButton = 0;
			_clickCount = 0;

			var requestContextMenu = _buttonForContextMenu == buttonMessage.Shortcut.MouseButton &&
					_buttonActionForContextMenu == MouseButtonMessage.ButtonActions.Up &&
					!HasMoved(buttonMessage.Location) &&
					(_mouseHoldDownForContextMenuInMilliseconds == 0 || timeElapsedSinceMouseDown >= _mouseHoldDownForContextMenuInMilliseconds);

			if (this.CaptureHandler != null)
			{
				if (StopHandler(this.CaptureHandler))
				{
					if (_capturedOnThisClick && requestContextMenu)
					{
						_delayedContextMenuRequestPublisher.TimeoutMilliseconds = _contextMenuDelayInMilliseconds;
						_delayedContextMenuRequestPublisher.Publish(this, new ItemEventArgs<Point>(buttonMessage.Location));
					}

					return true;
				}

				if (requestContextMenu)
				{
					// Request the context menu right away
					_contextMenuEnabled = true;
					EventsHelper.Fire(_contextMenuRequested, this, new ItemEventArgs<Point>(buttonMessage.Location));
				}

                //Trace.WriteLine(String.Format("Release capture {0}", this.CaptureHandler.GetType()));

				ReleaseCapture(false);
				return true;
			}

			if (requestContextMenu)
			{
				// Request the context menu right away
				_contextMenuEnabled = true;
				EventsHelper.Fire(_contextMenuRequested, this, new ItemEventArgs<Point>(buttonMessage.Location));
			}

			return false;
		}

		private bool ProcessMouseButtonMessage(MouseButtonMessage buttonMessage)
		{
			this.Location = buttonMessage.Location;

			bool returnValue;
			if (buttonMessage.ButtonAction == MouseButtonMessage.ButtonActions.Down)
			{
				_selectedOnThisClick = !_tile.Selected;
				_capturedOnThisClick = this.CaptureHandler == null;
				returnValue = ProcessMouseButtonDownMessage(buttonMessage);
				_capturedOnThisClick = _capturedOnThisClick && (this.CaptureHandler != null);
				_selectedOnThisClick = false;
			}
			else
			{
				returnValue = ProcessMouseButtonUpMessage(buttonMessage);
			}

			return returnValue;
		}

		private void TrackCurrentPosition()
		{
			ProcessTrackMessage(new TrackMousePositionMessage(this.Location));
		}

		private bool ProcessTrackMessage(TrackMousePositionMessage trackMessage)
		{
			this.Location = trackMessage.Location;
						
			if (HasMoved(this.Location))
				_contextMenuEnabled = false;

			if (this.CaptureHandler != null)
			{
				if (ConstrainToTile(this.CaptureHandler) && !this.TileClientRectangle.Contains(this.Location))
				{
					SetCursorToken(null);
					return true;
				}

				if (this.CaptureHandler.Track(this))
				{
					SetCursorToken();
					return true;
				}
			}

			if (!_tile.Enabled)
				return true;

			IMouseButtonHandler handler = FindHandlingGraphic(TrackHandler);
			SetCursorToken(handler);
			return (handler != null);
		}

		private bool StartHandler(IMouseButtonHandler handler)
		{
			if (_selectedOnThisClick && SuppressOnTileActivate(handler))
				return false;

			if (_clickCount > 1 && IgnoreDoubleClicks(handler))
				return false;

			bool start = handler.Start(this);
			if (start)
				++_startCount;

			return start;
		}

		private bool TrackHandler(IMouseButtonHandler handler)
		{
			if (!_tile.Selected && SuppressOnTileActivate(handler))
				return false;

			if (ConstrainToTile(handler) && !this.TileClientRectangle.Contains(this.Location))
				return false;

			return handler.Track(this);
		}

		private bool StopHandler(IMouseButtonHandler handler)
		{
			bool handled = handler.Stop(this);
			if (!handled)
				_startCount = 0;

			return handled;
		}

		private IMouseButtonHandler FindHandlingGraphic(CallHandlerMethodDelegate handlerDelegate)
		{
			if (_tile.PresentationImage is PresentationImage)
			{
				CompositeGraphic sceneGraph = ((PresentationImage) _tile.PresentationImage).SceneGraph;
				foreach (IMouseButtonHandler handler in GetHandlerGraphics(sceneGraph))
				{
					if (handlerDelegate(handler))
						return handler;
				}
			}

			return null;
		}

		private IEnumerable<IGraphic> GetVisibleGraphics(CompositeGraphic compositeGraphic)
		{
		    return compositeGraphic.EnumerateChildGraphics(true).Where(g => g.Visible);
		}

	    private IEnumerable<IMouseButtonHandler> GetHandlerGraphics(CompositeGraphic compositeGraphic)
		{
            foreach (IGraphic graphic in GetVisibleGraphics(compositeGraphic))
			{
				if (graphic is IMouseButtonHandler)
				{
					yield return graphic as IMouseButtonHandler;
				}
				else if (graphic is CompositeGraphic)
				{
					foreach (IMouseButtonHandler handler in GetHandlerGraphics(graphic as CompositeGraphic))
						yield return handler;
				}
			}
		}

		/// <summary>
		/// Checks if <paramref name="point"/> has moved out of the tolerance zone from where the mouse button was pressed.
		/// </summary>
		public bool HasMoved(Point point)
		{
			return HasMoved(point, _startMousePoint, _mouseMovedToleranceInPixel);
		}

		private static bool HasMoved(Point testPoint, Point refPoint, int tolerance)
		{
			return Math.Abs(refPoint.X - testPoint.X) > tolerance || Math.Abs(refPoint.Y - testPoint.Y) > tolerance;
		}

		private void ProcessDelayedContextMenuRequest(object sender, EventArgs e)
		{
            var eventArgs = e as ItemEventArgs<Point>;
			if (eventArgs == null)
				return;

		    if (HasMoved(eventArgs.Item, _currentMousePoint, _mouseMovedToleranceInPixel))
                return;

			// Touch events are simulated as Left mouse button, which causes problem because the framework tools/graphics assumes right click will bring up RCCM.
			// Release previous capture and re-capture using a simulated right click.
			ReleaseCapture(true);
			_activeButton = XMouseButtons.Right;
			var simulatedRightMouseClick = new MouseButtonMessage(((ItemEventArgs<Point>)e).Item, XMouseButtons.Right, MouseButtonMessage.ButtonActions.Down, 1);
			StartNewHandler(simulatedRightMouseClick);

		    //When we show the context menu, reset the active button and start count,
            //because the user is going to have to start over again with a new click.
            _activeButton = 0;
		    _startCount = 0;

		    _contextMenuEnabled = true;
		    EventsHelper.Fire(_contextMenuRequested, this, eventArgs);

			// Release capture only after firing context menu requested, so the ContextMenuProvider stays the same
			if (CaptureHandler != null)
				ReleaseCapture(true);
		}

	    private void ProcessExplicitContextMenuRequest(object sender, TileContextMenuRequestEventArgs e)
	    {
            //Because it is likely to be called from within a mouse button handler's Start
            //method, which can cause problems, we'll post it instead.
            SynchronizationContext.Current.Post(ignore=>ProcessExplicitContextMenuRequest(e.Location, e.ActionModel), null);
	    }
        
	    private void ProcessExplicitContextMenuRequest(Point? location, ActionModelNode actionModel)
        {
            //Force a handler with capture to release.
            if (this.CaptureHandler != null)
                ReleaseCapture(true);

            //When we show the context menu, reset the active button and start count,
            //because the user is going to have to start over again with a new click.
            _activeButton = 0;
            _startCount = 0;

            if (!location.HasValue || !TileClientRectangle.Contains(location.Value))
                location = _currentMousePoint;

            if (actionModel == null)
            {
                CompositeGraphic sceneGraph = ((PresentationImage)_tile.PresentationImage).SceneGraph;
                //Get all the mouse button handlers that provide a context menu.
                foreach (var handlerGraphic in GetHandlerGraphics(sceneGraph).OfType<IContextMenuProvider>())
                {
                    var actionSet = handlerGraphic.GetContextMenuModel(this);
                    if (actionSet != null && actionSet.ChildNodes.Count > 0)
                    {
                        ContextMenuProvider = handlerGraphic;
                        break;
                    }
                }
            }
            else
            {
                ContextMenuProvider = new ActionModelProvider(actionModel);
            }

            //Request the context menu.
            _contextMenuEnabled = true;
            EventsHelper.Fire(_contextMenuRequested, this, new ItemEventArgs<Point>(location.Value));

            ContextMenuProvider = null;
        }
        
		#endregion
		#endregion

		#region Public Properties

		/// <summary>
		/// Used by the view layer to tell this object what the <see cref="Tile"/>'s client rectangle is.
		/// </summary>
		public Rectangle TileClientRectangle
		{
			get { return _tileClientRectangle; }
			set { _tileClientRectangle = value; }
		}

		/// <summary>
		/// Used by the view layer to decide whether or not to show the context menu.
		/// </summary>
		public bool ContextMenuEnabled
		{
			get { return _contextMenuEnabled; }
		}

		/// <summary>
		/// Used by the view layer to tell this object which button to use for context menu.
		/// </summary>
		public XMouseButtons ButtonForContextMenu
		{
			get { return _buttonForContextMenu; }
			set { _buttonForContextMenu = value; }
		}

		/// <summary>
		/// Used by the view layer to tell this object whether the context menu will be shown on mouse down or up.
		/// </summary>
		public MouseButtonMessage.ButtonActions ButtonActionForContextMenu
		{
			get { return _buttonActionForContextMenu; }
			set { _buttonActionForContextMenu = value; }
		}

		/// <summary>
		/// Used by the view layer to tell this object the tolerance level for mouse moved.
		/// </summary>
		public int MouseMovedToleranceInPixel
		{
			get { return _mouseMovedToleranceInPixel; }
			set { _mouseMovedToleranceInPixel = value; }
		}

		/// <summary>
		/// Used by the view layer to tell this object how long the mouse has to be hold down in order to show the context menu. Default is 0.
		/// </summary>
		public int MouseHoldDownForContextMenuInMilliseconds
		{
			get { return _mouseHoldDownForContextMenuInMilliseconds; }
			set { _mouseHoldDownForContextMenuInMilliseconds = value; }
		}

		/// <summary>
		/// Used by the view layer to tell this object the delay between a mouse up and the context menu showing.  Not applicable if <see cref="ButtonActionForContextMenu"/> is down.
		/// </summary>
		public int ContextMenuDelayInMilliseconds
		{
			get { return _contextMenuDelayInMilliseconds; }
			set { _contextMenuDelayInMilliseconds = value; }
		}

		/// <summary>
		/// Used by the view layer to retrieve the <see cref="ActionModelNode"/> in order to show a context menu.
		/// </summary>
		public IContextMenuProvider ContextMenuProvider
		{
			get
			{
				if (_contextMenuProvider == null)
					_contextMenuProvider = _tile.ImageViewer as IContextMenuProvider;

				return _contextMenuProvider;
			}
			set
			{
				_contextMenuProvider = value;
			}
		}

		/// <summary>
		/// Used by the view layer to determine the <see cref="CursorToken"/> to show.
		/// </summary>
		public CursorToken CursorToken
		{
			get
			{
				return _cursorToken;
			}
			set
			{
				if (_cursorToken == value)
					return;

				_cursorToken = value;
				EventsHelper.Fire(_cursorTokenChanged, this, new EventArgs());
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Called by the view layer so that the <see cref="TileController"/> can process the <paramref name="message"/>.
		/// </summary>
		public bool ProcessMessage(object message)
		{
			// if the user does anything other than mouse tracking, cancel any pending delayed context menu events
			if (!(message is TrackMousePositionMessage))
			{
				// we allow mouse tracking because of the small movement tolerance
				_delayedContextMenuRequestPublisher.Cancel();
			}

			if (message is LostFocusMessage)
			{
				this.CaptureMouseWheelHandler = null;
				return true;
			}
			else if (message is KeyboardButtonDownPreview)
			{
				//Right now, we can't determine what these keystrokes are going to do, so we just release mouse wheel capture.
				KeyboardButtonDownPreview preview = message as KeyboardButtonDownPreview;
				if (preview.Shortcut.KeyData != XKeys.None)
					this.CaptureMouseWheelHandler = null;
				return false;
			}
			else if (message is MouseButtonMessage)
			{
				return ProcessMouseButtonMessage(message as MouseButtonMessage);
			}
			else if (message is TrackMousePositionMessage)
			{
				return ProcessTrackMessage(message as TrackMousePositionMessage);
			}

            if (_tile.PresentationImage != null)
            {
                if (message is MouseWheelMessage)
                {
                    bool returnValue = ProcessMouseWheelMessage(message as MouseWheelMessage);
                    TrackCurrentPosition();
                    return returnValue;
                }
                else if (message is KeyboardButtonMessage)
                {
                    bool returnValue = ProcessKeyboardMessage(message as KeyboardButtonMessage);
                    TrackCurrentPosition();
                    return returnValue;
                }
                else if (message is ReleaseCaptureMessage)
                {
                    ReleaseCapture(true);
                    TrackCurrentPosition();
                    return true;
                }
                else if (message is MouseLeaveMessage)
                {
                    _tile.PresentationImage.FocussedGraphic = null;
                }
            }
            else
            {
                // We should respond to keyboard even when there's no presentation image.
                if (message is KeyboardButtonMessage)
                {
                    bool returnValue = ProcessKeyboardMessage(message as KeyboardButtonMessage);
                    return returnValue;
                }
            }
		    return false;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// For use by the view layer, so it can detect when the <see cref="CursorToken"/> has changed.
		/// </summary>
		public event EventHandler CursorTokenChanged
		{
			add { _cursorTokenChanged += value; }
			remove { _cursorTokenChanged -= value; }
		}

		/// <summary>
		/// Fired to request that the view layer show a context menu at the specified location.
		/// </summary>
		public event EventHandler<ItemEventArgs<Point>> ContextMenuRequested
		{
			add { _contextMenuRequested += value; }
			remove { _contextMenuRequested -= value; }
		}

		/// <summary>
		/// For use by the view layer, so it can detect when capture is changing.
		/// </summary>
		public event EventHandler<ItemEventArgs<IMouseButtonHandler>> CaptureChanging
		{
			add { _captureChangingEvent += value; }
			remove { _captureChangingEvent -= value; }
		}

        /// <summary>
        /// For use by the view layer, so it can detect when capture is changing.
        /// </summary>
        public event EventHandler<ItemEventArgs<IMouseWheelHandler>> WheelCaptureChanging
        {
            add { _wheelCaptureChangingEvent += value; }
            remove { _wheelCaptureChangingEvent -= value; }
        }

		#endregion

		#region IMouseInformation Members

		/// <summary>
		/// The <see cref="ITile"/> that is controlled by this <see cref="TileController"/>.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public ITile Tile
		{
			get { return _tile; }
		}

		/// <summary>
		/// The current mouse location, set by the view layer.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public Point Location
		{
			get { return _currentMousePoint; }
			private set { _currentMousePoint = value; }
		}

		/// <summary>
		/// Gets the currently depressed (<see cref="XMouseButtons"/>) mouse button, set internally by this class.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public XMouseButtons ActiveButton
		{
			get { return _activeButton; }
		}

		/// <summary>
		/// Gets the current click count.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public uint ClickCount
		{
			get { return _clickCount; }
		}

		#endregion
	}
}