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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Abstract base class for desktop objects such as windows, workspaces and shelves.
	/// </summary>
	public abstract class DesktopObject : IDesktopObject, IDisposable
	{
		private string _name;
		private string _title;
		private DesktopObjectState _state;
		private bool _active;

		private event EventHandler _opening;
		private event EventHandler _opened;
		private event EventHandler<ClosingEventArgs> _closing;
		private event EventHandler<ClosedEventArgs> _closed;

		private event EventHandler _titleChanged;
		private event EventHandler _activeChanged;
		private event EventHandler _internalActiveChanged;

		private bool _visible;
		private event EventHandler _visibleChanged;

		private IDesktopObjectView _view;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected DesktopObject(DesktopObjectCreationArgs args)
		{
			_name = args.Name;
			_title = args.Title;
			//_visible = true;    // all objects are visible by default

			Application.CurrentUICultureChanged += Application_CurrentUICultureChanged;
		}

		/// <summary>
		/// Finalizer.
		/// </summary>
		~DesktopObject()
		{
			Dispose(false);
		}

		#region Public properties

		/// <summary>
		/// Gets the runtime name of the object, or null if the object is not named.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the current state of the object.
		/// </summary>
		public DesktopObjectState State
		{
			get { return _state; }
		}

		/// <summary>
		/// Gets the title that is presented to the user on the screen.
		/// </summary>
		[Localizable(true)]
		public string Title
		{
			get { return _title; }
			protected set
			{
				if (value != _title)
				{
					_title = value;
					if (this.View != null)
					{
						this.View.SetTitle(_title);
					}
					OnTitleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this object is currently visible.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			private set
			{
				if (value != _visible)
				{
					_visible = value;
					OnVisibleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this object is currently active.
		/// </summary>
		public bool Active
		{
			get { return _active; }
			private set
			{
				if (value != _active)
				{
					_active = value;
					OnInternalActiveChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Activates the object.
		/// </summary>
		public virtual void Activate()
		{
			AssertState(new DesktopObjectState[] {DesktopObjectState.Open});
			DoActivate();
		}

		/// <summary>
		/// Checks if the object is in a closable state (would be able to close without user interaction).
		/// </summary>
		/// <returns>True if the object can be closed without user interaction.</returns>
		public bool QueryCloseReady()
		{
			AssertState(new DesktopObjectState[] {DesktopObjectState.Open, DesktopObjectState.Closing});

			return CanClose();
		}

		/// <summary>
		/// Tries to close the object, interacting with the user if necessary.
		/// </summary>
		/// <returns>True if the object is closed, otherwise false.</returns>
		public bool Close()
		{
			AssertState(new DesktopObjectState[] {DesktopObjectState.Open});

			return Close(UserInteraction.Allowed);
		}

		/// <summary>
		/// Tries to close the object, interacting with the user only if specified.
		/// </summary>
		/// <param name="interactive">A value specifying whether user interaction is allowed.</param>
		/// <returns>True if the object is closed, otherwise false.</returns>
		public bool Close(UserInteraction interactive)
		{
			AssertState(new DesktopObjectState[] {DesktopObjectState.Open});

			return Close(interactive, CloseReason.Program);
		}

		#endregion

		#region Public events

		/// <summary>
		/// Occurs when the object is about to open.
		/// </summary>
		public event EventHandler Opening
		{
			add { _opening += value; }
			remove { _opening -= value; }
		}

		/// <summary>
		/// Occurs when the object has opened.
		/// </summary>
		public event EventHandler Opened
		{
			add { _opened += value; }
			remove { _opened -= value; }
		}

		/// <summary>
		/// Occurs when the object is about to close.
		/// </summary>
		public event EventHandler<ClosingEventArgs> Closing
		{
			add { _closing += value; }
			remove { _closing -= value; }
		}

		/// <summary>
		/// Occurs when the object has closed.
		/// </summary>
		public event EventHandler<ClosedEventArgs> Closed
		{
			add { _closed += value; }
			remove { _closed -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="Visible"/> property changes.
		/// </summary>
		public event EventHandler VisibleChanged
		{
			add { _visibleChanged += value; }
			remove { _visibleChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="Active"/> property changes.
		/// </summary>
		public event EventHandler ActiveChanged
		{
			add { _activeChanged += value; }
			remove { _activeChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="Title"/> property changes.
		/// </summary>
		public event EventHandler TitleChanged
		{
			add { _titleChanged += value; }
			remove { _titleChanged -= value; }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		void IDisposable.Dispose()
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

		#region Protected Overridables

		/// <summary>
		/// Factory method to create a view for this object.
		/// </summary>
		protected abstract IDesktopObjectView CreateView();

		/// <summary>
		/// Initializes the object, prior to it becoming visible on the screen.
		/// </summary>
		/// <remarks>
		/// Override this method to perform custom initialization.
		/// </remarks>
		protected virtual void Initialize()
		{
			// nothing to initialize
		}

		/// <summary>
		/// Asks the object whether it is in a closable state without user intervention.
		/// </summary>
		/// <remarks>
		/// The default implementation just returns true. Override this method to customize the behaviour.
		/// The object must respond to this method without interacting with the user.  Therefore it should respond conservatively
		/// (e.g. respond with false if there may be unsaved data).
		/// </remarks>
		/// <returns>True if the object can be closed, otherwise false.</returns>
		protected internal virtual bool CanClose()
		{
			return true;
		}

		/// <summary>
		/// Gives the object an opportunity to prepare before being closed.
		/// </summary>
		/// <remarks>
		/// The object is free to interact with the user in this method, in order to make any preparations
		/// prior to being closed.  The object may return false if it still cannot close (e.g. there is
		/// unsaved data, and the user, when prompted, elects to cancel the close operation).
		/// </remarks>
		/// <param name="reason">The reason for closing the object.</param>
		/// <returns>True if the object is ready to close, or false it the object cannot be closed.</returns>
		protected virtual bool PrepareClose(CloseReason reason)
		{
			// first see if we can close without interacting
			// that way we avoid calling Activate() if not necessary
			if (CanClose())
				return true;

			// make active, so the user is not confused if it brings up a message box
			// (this would be done in an override of this method)
			DoActivate();

			// cannot close
			return false;
		}

		/// <summary>
		/// Called to dispose of this object.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Application.CurrentUICultureChanged -= Application_CurrentUICultureChanged;

				// view may have already been disposed in the Close method
				if (_view != null)
				{
					_view.Dispose();
					_view = null;
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="Opening"/> event.
		/// </summary>
		protected virtual void OnOpening(EventArgs args)
		{
			EventsHelper.Fire(_opening, this, args);
		}

		/// <summary>
		/// Raises the <see cref="Opened"/> event.
		/// </summary>
		protected virtual void OnOpened(EventArgs args)
		{
			EventsHelper.Fire(_opened, this, args);
		}

		/// <summary>
		/// Raises the <see cref="Closing"/> event.
		/// </summary>
		protected virtual void OnClosing(ClosingEventArgs args)
		{
			EventsHelper.Fire(_closing, this, args);
		}

		/// <summary>
		/// Raises the <see cref="Closed"/> event.
		/// </summary>
		protected virtual void OnClosed(EventArgs args)
		{
			EventsHelper.Fire(_closed, this, args);
		}

		/// <summary>
		/// Raises the <see cref="VisibleChanged"/> event.
		/// </summary>
		protected virtual void OnVisibleChanged(EventArgs args)
		{
			EventsHelper.Fire(_visibleChanged, this, args);
		}

		/// <summary>
		/// Raises the <see cref="ActiveChanged"/> event.
		/// </summary>
		protected virtual void OnActiveChanged(EventArgs args)
		{
			EventsHelper.Fire(_activeChanged, this, args);
		}

		/// <summary>
		/// Raises the <see cref="TitleChanged"/> event.
		/// </summary>
		protected virtual void OnTitleChanged(EventArgs args)
		{
			EventsHelper.Fire(_titleChanged, this, args);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Raises the <see cref="ActiveChanged"/> event.
		/// </summary>
		internal void RaiseActiveChanged()
		{
			OnActiveChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Gets the view for this object.
		/// </summary>
		protected IDesktopObjectView View
		{
			get { return _view; }
		}

		//TODO (Web Viewer): remove protected once we start hosting each app in it's own app domain.

		/// <summary>
		/// Opens this object.
		/// </summary>
		protected internal void Open()
		{
			// call initialize before opening
			// any exception thrown from initialize will therefore abort before opening, not after
			Initialize();

			_state = DesktopObjectState.Opening;
			OnOpening(EventArgs.Empty);

			_view = CreateView();
			_view.SetTitle(_title);
			_view.ActiveChanged += OnViewActiveChanged;
			_view.VisibleChanged += OnViewVisibleChanged;
			_view.CloseRequested += OnViewCloseRequested;

			_view.Open();

			_state = DesktopObjectState.Open;
			OnOpened(EventArgs.Empty);
		}

		private void OnViewActiveChanged(object sender, EventArgs args)
		{
			Active = _view.Active;
		}

		private void OnViewVisibleChanged(object sender, EventArgs args)
		{
			Visible = _view.Visible;
		}

		private void OnViewCloseRequested(object sender, EventArgs args)
		{
			// the request should always come from the active object, so interaction should be allowed
			Close(UserInteraction.Allowed, CloseReason.UserInterface);
		}

		/// <summary>
		/// Occurs when the <see cref="Active"/> property changes.
		/// </summary>
		internal event EventHandler InternalActiveChanged
		{
			add { _internalActiveChanged += value; }
			remove { _internalActiveChanged -= value; }
		}

		/// <summary>
		/// Closes this object.
		/// </summary>
		/// <returns>True if the object was closed, otherwise false.</returns>
		protected internal bool Close(UserInteraction interactive, CloseReason reason)
		{
			// easy case - bail if interaction is prohibited and we can't close without interacting
			if (interactive == UserInteraction.NotAllowed && !CanClose())
				return false;

			// either we can close without interacting, or interaction is allowed, so let's try and close

			// begin closing - the operation may yet be cancelled
			_state = DesktopObjectState.Closing;

			ClosingEventArgs args = new ClosingEventArgs(reason, interactive);
			OnClosing(args);

			if (args.Cancel || !PrepareClose(reason))
			{
				_state = DesktopObjectState.Open;
				return false;
			}

			_view.CloseRequested -= OnViewCloseRequested;
			_view.VisibleChanged -= OnViewVisibleChanged;
			_view.ActiveChanged -= OnViewActiveChanged;

			// notify inactive
			this.Active = false;

			try
			{
				// close the view
				_view.Dispose();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
			_view = null;

			// close was successful
			_state = DesktopObjectState.Closed;
			OnClosed(new ClosedEventArgs(reason));

			// dispose of this object after firing the Closed event
			// (reason being that handlers of the Closed event may expect this object to be intact)
			(this as IDisposable).Dispose();

			return true;
		}

		/// <summary>
		/// Asserts that the object is in one of the specified valid states.
		/// </summary>
		protected void AssertState(DesktopObjectState[] validStates)
		{
			if (!CollectionUtils.Contains<DesktopObjectState>(validStates,
			                                                  delegate(DesktopObjectState state) { return state == this.State; }))
			{
				string t = this.GetType().Name;
				string s = this.State.ToString();
				throw new InvalidOperationException(string.Format("Operation not valid on a {0} with State: {1}", t, s));
			}
		}

		/// <summary>
		/// Activates this object.
		/// </summary>
		private void DoActivate()
		{
			_view.Show(); // always ensure the object is visible prior to activating
			_view.Activate();
		}

		/// <summary>
		/// Raises the <see cref="InternalActiveChanged"/> event.
		/// </summary>
		private void OnInternalActiveChanged(EventArgs args)
		{
			EventsHelper.Fire(_internalActiveChanged, this, args);
		}

		/// <summary>
		/// Called when the current application UI culture has changed.
		/// </summary>
		protected virtual void OnCurrentUICultureChanged() {}

		private void Application_CurrentUICultureChanged(object sender, EventArgs e)
		{
			// notify subclasses that the application UI culture has changed
			OnCurrentUICultureChanged();
		}

		#endregion
	}
}