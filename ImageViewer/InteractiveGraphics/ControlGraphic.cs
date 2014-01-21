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

using System.Drawing;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines a graphic that decorates an <see cref="IGraphic"/> with user interaction
	/// components controlling an underlying graphic in the scene graph.
	/// </summary>
	public interface IControlGraphic : IDecoratorGraphic, ICursorTokenProvider, IMouseButtonHandler, IExportedActionsProvider
	{
		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		/// <remarks>
		/// The controlled graphic is the first non-decorator graphic in the
		/// control graphics chain (the first graphic that doesn't implement
		/// <see cref="IDecoratorGraphic"/> when recursively following the
		/// <see cref="IDecoratorGraphic.DecoratedGraphic"/> property.)
		/// </remarks>
		IGraphic Subject { get; }

		/// <summary>
		/// Gets or sets the color of the control parts of the graphic.
		/// </summary>
		/// <remarks>
		/// This property does not affect the color of the underlying subject or chained control graphics.
		/// </remarks>
		Color Color { get; set; }

		/// <summary>
		/// Gets or sets whether or not the control parts of the graphic are visible.
		/// </summary>
		/// <remarks>
		/// This property does not affect the visibility of the underlying subject or chained control graphics.
		/// </remarks>
		bool Show { get; set; }

		/// <summary>
		/// Gets or sets whether or not the control parts of the graphic are enabled.
		/// </summary>
		/// <remarks>
		/// When the control graphic is disabled, the control parts of the graphic do not show and do not respond to user interaction.
		/// This property does not affect the enablement of the underlying subject or chained control graphics.
		/// </remarks>
		bool Enabled { get; set; }
	}

	/// <summary>
	/// Base class for implementations of <see cref="IControlGraphic"/>.
	/// </summary>
	/// <remarks>
	/// A <see cref="ControlGraphic"/> can be used on it's own where one is required,
	/// but you essentially want one that does nothing.
	/// </remarks>
	[Cloneable]
	public class ControlGraphic : DecoratorCompositeGraphic, IControlGraphic
	{
		private Color _color = Color.Yellow;
		private bool _show = true;
		private bool _enabled = true;

		[CloneIgnore]
		private IMouseButtonHandler _capturedHandler = null;

		[CloneIgnore]
		private PointF _lastTrackedPosition = PointF.Empty;

		[CloneIgnore]
		private bool _isTracking = false;

		/// <summary>
		/// Constructs a new control graphic to control the given subject graphic.
		/// </summary>
		/// <param name="subject">The graphic to control.</param>
		public ControlGraphic(IGraphic subject) : base(subject) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected ControlGraphic(ControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets the subject graphic that this graphic controls.
		/// </summary>
		/// <remarks>
		/// The controlled graphic is the first non-decorator graphic in the
		/// control graphics chain (the first graphic that doesn't implement
		/// <see cref="IDecoratorGraphic"/> when recursively following the
		/// <see cref="IDecoratorGraphic.DecoratedGraphic"/> property.)
		/// </remarks>
		public IGraphic Subject
		{
			get
			{
				var controlGraphic = DecoratedGraphic as IControlGraphic;
				return controlGraphic != null ? (controlGraphic).Subject : DecoratedGraphic;
			}
		}

		/// <summary>
		/// Gets a string that describes the type of control operation that this graphic provides.
		/// </summary>
		public virtual string CommandName
		{
			get { return null; }
		}

		/// <summary>
		/// Gets or sets the color of the control parts of the graphic.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					OnColorChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value to show or hide this control graphic without affecting the
		/// visibility of the underlying subject or other control graphics.
		/// </summary>
		public bool Show
		{
			get { return _enabled && _show; }
			set
			{
				if (_show != value)
				{
					_show = value;
					OnShowChanged();
				}
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					OnEnabledChanged();
					OnShowChanged();
				}
			}
		}

		/// <summary>
		/// Gets the last tracked cursor position in source or destination coordinates.
		/// </summary>
		protected PointF LastTrackedPosition
		{
			get { return CoordinateSystem == CoordinateSystem.Source ? SpatialTransform.ConvertToSource(_lastTrackedPosition) : _lastTrackedPosition; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the control graphic is currently tracking mouse input.
		/// </summary>
		protected bool IsTracking
		{
			get { return _isTracking; }
		}

		/// <summary>
		/// Called when the <see cref="Color"/> property changes.
		/// </summary>
		protected virtual void OnColorChanged() {}

		/// <summary>
		/// Called when the <see cref="Show"/> property changes.
		/// </summary>
		protected virtual void OnShowChanged() {}

		/// <summary>
		/// Called when the <see cref="Enabled"/> property changes.
		/// </summary>
		protected virtual void OnEnabledChanged() {}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework requesting the cursor token for a particular screen coordinate via <see cref="GetCursorToken"/>.
		/// </summary>
		/// <param name="point">The screen coordinate for which the cursor is requested.</param>
		/// <returns></returns>
		protected virtual CursorToken GetCursorToken(Point point)
		{
			return null;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to a mouse button click via <see cref="Start"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the <see cref="ControlGraphic"/> did something as a result of the call and hence would like to receive capture; False otherwise.</returns>
		protected virtual bool Start(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework tracking mouse input via <see cref="Track"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the message was handled; False otherwise.</returns>
		protected virtual bool Track(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response a mouse button release via <see cref="Stop"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the framework should <b>not</b> release capture; False otherwise.</returns>
		protected virtual bool Stop(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to an attempt to cancel the current operation via <see cref="Cancel"/>.
		/// </summary>
		protected virtual void Cancel() {}

		public override PointF GetClosestPoint(PointF point)
		{
			// if the control is not enabled, closest point should ignore the control graphics (which is everything except the decorated graphic)
			return Enabled ? base.GetClosestPoint(point) : DecoratedGraphic.GetClosestPoint(point);
		}

		public override bool HitTest(Point point)
		{
			// if the control is not enabled, hit test should ignore the control graphics (which is everything except the decorated graphic)
			return Enabled ? base.HitTest(point) : DecoratedGraphic.HitTest(point);
		}

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		/// <remarks>
		/// The <see cref="ControlGraphic"/> implementation returns the the cursor token
		/// provided by the current input handler, <see cref="GetCursorToken"/>, or any
		/// child graphics implementing <see cref="ICursorTokenProvider"/>,
		/// in decreasing order of priority.
		/// </remarks>
		CursorToken ICursorTokenProvider.GetCursorToken(Point point)
		{
			// TODO (CR Oct 2011): This pattern could be reused outside the context
			// of a "Control" graphic. Perhaps we could move it out to a "GraphicsController"
			// class that enumerates through a list of graphics looking for a handler.
			CursorToken cursor = null;

			if (_capturedHandler != null)
			{
				var provider = _capturedHandler as ICursorTokenProvider;
				if (provider != null)
					cursor = (provider).GetCursorToken(point);
			}

			if (Enabled && cursor == null)
				cursor = GetCursorToken(point);

			if (cursor == null)
			{
				foreach (IGraphic graphic in EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					ICursorTokenProvider provider = graphic as ICursorTokenProvider;
					if (provider != null)
					{
						cursor = provider.GetCursorToken(point);
						if (cursor != null)
							break;
					}
				}
			}

			return cursor;
		}

		#endregion

		#region IMouseButtonHandler Members

		/// <summary>
		/// Called by the framework each time a mouse button is pressed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// As a general rule, if the <see cref="IMouseButtonHandler"/> object did anything as a result of this call, it must 
		/// return true.  If false is returned, <see cref="IMouseButtonHandler.Start"/> is called on other <see cref="IMouseButtonHandler"/>s
		/// until one returns true.
		/// </para>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation finds a handler by trying <see cref="Start"/>,
		/// and any child graphics implementing <see cref="IMouseButtonHandler"/>, in decreasing order of priority.
		/// </para>
		/// </remarks>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the <see cref="ControlGraphic"/> did something as a result of the call and hence would like to receive capture; False otherwise.</returns>
		bool IMouseButtonHandler.Start(IMouseInformation mouseInformation)
		{
			bool result = false;

			if (_capturedHandler != null)
			{
				result = _capturedHandler.Start(mouseInformation);
				if (result) return true;
			}

			if (Enabled)
			{
				CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					if (HitTest(mouseInformation.Location))
					{
						_lastTrackedPosition = mouseInformation.Location;
						_isTracking = true;
					}
					result = Start(mouseInformation);
					_isTracking = _isTracking && result;
				}
				finally
				{
					ResetCoordinateSystem();
				}
			}

			_capturedHandler = null;
			if (!result)
			{
				foreach (IGraphic graphic in EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					IMouseButtonHandler handler = graphic as IMouseButtonHandler;
					if (handler != null)
					{
						result = handler.Start(mouseInformation);
						if (result)
						{
							_capturedHandler = handler;
							break;
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Called by the framework when the mouse has moved.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A button does not necessarily have to be down for this message to be called.  The framework can
		/// call it any time the mouse moves.
		/// </para>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation calls <see cref="IMouseButtonHandler.Track"/> on
		/// the current handler, <see cref="Track"/>, or any child graphics implementing <see cref="IMouseButtonHandler"/>,
		/// in decreasing order of priority.
		/// </para>
		/// </remarks>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the message was handled; False otherwise.</returns>
		bool IMouseButtonHandler.Track(IMouseInformation mouseInformation)
		{
			bool result = false;

			if (_capturedHandler != null)
				return _capturedHandler.Track(mouseInformation);

			if (Enabled)
			{
				try
				{
					result = Track(mouseInformation);
				}
				finally
				{
					if (_isTracking)
						_lastTrackedPosition = mouseInformation.Location;
				}
			}

			if (!result)
			{
				foreach (IGraphic graphic in EnumerateChildGraphics(true))
				{
					if (!graphic.Visible)
						continue;

					IMouseButtonHandler handler = graphic as IMouseButtonHandler;
					if (handler != null)
					{
						result = handler.Track(mouseInformation);
						if (result)
							break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Called by the framework when the mouse button is released.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation calls <see cref="IMouseButtonHandler.Stop"/> on
		/// the current handler, <see cref="Stop"/>, or any child graphics implementing <see cref="IMouseButtonHandler"/>,
		/// in decreasing order of priority.
		/// </para>
		/// </remarks>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the framework should <b>not</b> release capture; False otherwise.</returns>
		bool IMouseButtonHandler.Stop(IMouseInformation mouseInformation)
		{
			bool result;

			if (_capturedHandler != null)
			{
				result = _capturedHandler.Stop(mouseInformation);
				if (!result)
				{
					_capturedHandler = null;
					return false;
				}
			}

			try
			{
				result = Stop(mouseInformation);
			}
			finally
			{
				_isTracking = false;
				_lastTrackedPosition = PointF.Empty;
			}

			return result;
		}

		/// <summary>
		/// Called by the framework to let <see cref="IMouseButtonHandler"/> perform any necessary cleanup 
		/// when capture is going to be forcibly released.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="ControlGraphic"/> implementation calls <see cref="IMouseButtonHandler.Cancel"/> on
		/// the current handler or <see cref="Cancel"/> in decreasing order of priority.
		/// </para>
		/// </remarks>
		void IMouseButtonHandler.Cancel()
		{
			if (_capturedHandler != null)
			{
				_capturedHandler.Cancel();
				_capturedHandler = null;
			}

			try
			{
				Cancel();
			}
			finally
			{
				_isTracking = false;
				_lastTrackedPosition = PointF.Empty;
			}
		}

		/// <summary>
		/// Gets the desired behaviour of this mouse input handler.
		/// </summary>
		/// <remarks>
		/// The default implementation returns the behaviour of the lowest control
		/// graphic in the control chain, or <see cref="MouseButtonHandlerBehaviour.None"/>
		/// if this graphic is the lowest in the control chain.
		/// </remarks>
		public virtual MouseButtonHandlerBehaviour Behaviour
		{
			get
			{
				var controlGraphic = DecoratedGraphic as IControlGraphic;
				return controlGraphic != null ? (controlGraphic).Behaviour : MouseButtonHandlerBehaviour.None;
			}
		}

		#endregion

		#region IExportedActionsProvider Members

		/// <summary>
		/// Gets a set of exported <see cref="IAction"/>s.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This mechanism is useful when a particular component defines generally useful <see cref="IAction"/>s
		/// without requiring specific knowledge of the action model sites that the client code uses.
		/// </para>
		/// <para>
		/// Overriding implementations should generally call the base implementation and invoke a <see cref="IActionSet.Union"/>
		/// with any new actions the derived class wishes to provide in order to maintain full functionality of any
		/// control graphics further down in the chain.
		/// </para>
		/// </remarks>
		/// <param name="site">The action model site at which the actions should reside.</param>
		/// <param name="mouseInformation">The mouse input when the action model was requested, such as in response to a context menu request.</param>
		/// <returns>A set of exported <see cref="IAction"/>s.</returns>
		public virtual IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			return EnumerateChildGraphics(true).OfType<IExportedActionsProvider>()
				.Select(p => p.GetExportedActions(site, mouseInformation))
				.Where(a => a != null)
				.Aggregate((IActionSet) new ActionSet(), (a, b) => a.Union(b));
		}

		IActionSet IExportedActionsProvider.GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			return Enabled ? GetExportedActions(site, mouseInformation) : new ActionSet();
		}

		#endregion

		#region IMemorable Helpers

		/// <summary>
		/// Helper method to add the operation of an <see cref="ControlGraphic"/> implementing <see cref="IMemorable"/> to the associated command history.
		/// </summary>
		/// <typeparam name="T">The derived type of <see cref="ControlGraphic"/>. Must implement <see cref="IMemorable"/>.</typeparam>
		/// <param name="originator">The <see cref="ControlGraphic"/> that performed the operation.</param>
		/// <param name="beginState">The memento created by <see cref="IMemorable.CreateMemento"/> before the operation.</param>
		/// <param name="endState">The memento created by <see cref="IMemorable.CreateMemento"/> after the operation.</param>
		protected static void AddToCommandHistory<T>(T originator, object beginState, object endState) where T : ControlGraphic, IMemorable
		{
			if (originator.ImageViewer == null)
				return;
			if (beginState == endState) // ensure that both states aren't simultaneously null and that they're not the same states
				return;
			if (beginState != null && beginState.Equals(endState)) // ensure that beginState isn't equivalent to endState
				return;

			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(originator);
			memorableCommand.BeginState = beginState;
			memorableCommand.EndState = endState;

			DrawableUndoableCommand command = new DrawableUndoableCommand(originator);
			command.Name = originator.CommandName;
			command.Enqueue(memorableCommand);

			originator.ImageViewer.CommandHistory.AddCommand(command);
		}

		#endregion
	}
}