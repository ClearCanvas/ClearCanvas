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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
    /// <summary>
	/// Extends the <see cref="ImageViewerTool"/> class to provide functionality that is common to mouse tools.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A mouse tool is a tool that, when activated, is assigned to a specific mouse button 
    /// (see <see cref="MouseToolButtonAttribute"/>) and is given the opportunity to respond to 
    /// mouse events for that button.  Developers implementing mouse tools should subclass this class.
	/// </para>
	/// <para>
	/// Typically, a mouse tool needs to be decorated with certain attributes in order to work properly.  For
	/// example:
	/// <example>
	/// <code>
	/// [C#]
	///  	[MenuAction("activate", "imageviewer-contextmenu/My Tool", "Select", Flags = ClickActionFlags.CheckAction)]
	///		[MenuAction("activate", "global-menus/MenuTools/MenuStandard/My Tool", "Select", Flags = ClickActionFlags.CheckAction)]
	///		[ButtonAction("activate", "global-toolbars/ToolbarStandard/My Tool", "Select", Flags = ClickActionFlags.CheckAction)]
	///		[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandard/My Tool", "Select", KeyStroke = XKeys.R)]
	///		[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	///		[IconSet("activate", "Icons.MyToolSmall.png", "Icons.MyToolToolMedium.png", "Icons.MyToolToolLarge.png")]
	///		[MouseToolButton(XMouseButtons.Left, true)]
	/// </code>
	/// The "Select" parameter in each of the 'Action' attributes refers to the <see cref="MouseImageViewerTool.Select"/> method
	/// which activates (and checks) the tool.  All other tools with the same <see cref="MouseButton"/> are deactivated.
	/// </example>
	/// </para>
	/// <para>
	/// When a tool does not implement the typical mouse button handling behaviour, it should <b>not</b> be 
	/// decorated with any of the above attributes as it will result in unexpected behaviour with regards to the toolbars and menus.
	/// </para>
	/// <para>
	/// A mouse tool can also have an additional mouse button shortcut specified 
	/// (see <see cref="DefaultMouseToolButtonAttribute"/>) that does not require the mouse 
	/// tool to be activated in order to use it.  The value of <see cref="DefaultMouseButtonShortcut"/> need not be 
	/// modified (e.g. Ctrl, Shift),  however if another tool's <see cref="MouseButton"/> has the same value and is active,
	/// it will supersede any <see cref="DefaultMouseButtonShortcut"/> assignments.  Therefore, <see cref="DefaultMouseButtonShortcut"/>
	/// should typically only be assigned a non-modified value when no other <see cref="MouseImageViewerTool"/> has the same
	/// <see cref="MouseButton"/> value.
	/// </para>
	/// <para>
	/// One further piece of functionality that subclasses of <see cref="MouseImageViewerTool"/> can choose to implement
	/// is handling of the mouse wheel.  When decorated with a <see cref="MouseWheelHandlerAttribute"/>, the tool's
	/// <see cref="MouseWheelShortcut"/> will be set upon construction.  The subclass must override the mouse wheel-related
	/// methods in order for the tool to have any effect (see <see cref="StartWheel"/>, <see cref="StopWheel"/>, <see cref="WheelForward"/>,
	/// <see cref="WheelBack"/>).
	/// </para>
	/// </remarks>
	/// <seealso cref="MouseToolButtonAttribute"/>
	/// <seealso cref="DefaultMouseToolButtonAttribute"/>
    // TODO CR (May 10): there's something inconsistent here with the mouse button assignments
	// we allow the client/inheritor to set the mouse button, yet they are always synchronized to the tool settings (user config)
	// there should be a proper distinction between tools tied to settings (where the inheritor specifies the default mouse assignment)
	// and tools not tied to settings (where the client code specifies mouse assignment through other unspecified means)
	// this is where an IMouseImageViewerTool interface would come in handy - implement this interface for the latter scenario
	// the latter scenario may be less common, but that doesn't justify making it impossible - plus, it's more architecturally sound
	public abstract partial class MouseImageViewerTool :
		ImageViewerTool,
		IMouseButtonHandler,
		IMouseWheelHandler,
		ICursorTokenProvider
	{
		#region Private fields

		private int _lastX;
		private int _lastY;
		private int _deltaX;
		private int _deltaY;

    	private bool _actionsInitialized = false;

		private string _tooltipPrefix;
		private event EventHandler _tooltipChangedEvent;

		private XMouseButtons _mouseButton;
		private event EventHandler _mouseButtonChanged;
		private MouseButtonHandlerBehaviour _mousebuttonBehaviour;

		private MouseButtonShortcut _defaultMouseButtonShortcut;
		private event EventHandler _defaultMouseButtonShortcutChanged;

		private MouseWheelShortcut _mouseWheelShortcut;
		private event EventHandler _mouseWheelShortcutChanged;

        private bool _active;
        private event EventHandler _activationChangedEvent;

		private CursorToken _cursorToken;
		
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tooltipPrefix">The tooltip prefix, which usually describes the tool's function.</param>
		protected MouseImageViewerTool(string tooltipPrefix)
			: base()
		{
			_tooltipPrefix = tooltipPrefix;
			_mousebuttonBehaviour = MouseButtonHandlerBehaviour.Default;
			_mouseButton = XMouseButtons.None;
			_defaultMouseButtonShortcut = null;
			_mouseWheelShortcut = null;
			_active = false;
		}

		/// <summary>
		/// Gets or sets the tooltip prefix, which is usually a string describing the tool's function.
		/// </summary>
		protected MouseImageViewerTool()
			: this(SR.LabelUnknown)
		{
		}

		/// <summary>
		/// Registers the specified action as a tool activation action.
		/// </summary>
		/// <remarks>
		/// It is unecessary to call register an action if it calls the <see cref="Select"/> method directly.
		/// If the action invokes another method that calls <see cref="Select"/> in turn, use this method
		/// to register that action's ID during or before the tool initialization phase.
		/// </remarks>
		/// <param name="actionId">The unqualified action ID of the action that selects and activates this tool.</param>
    	protected void RegisterActivationActionId(string actionId)
    	{
    		Type type = this.GetType();
    		MouseToolSettingsProfile.Current.RegisterActivationActionId(string.Format("{0}:{1}", type.FullName, actionId), type);
    	}

		/// <summary>
		/// Gets or sets the tooltip prefix for this <see cref="MouseImageViewerTool"/>.
		/// </summary>
		protected virtual string TooltipPrefix
		{
			get { return _tooltipPrefix; }
			set 
			{
				if (_tooltipPrefix == value)
					return;

				_tooltipPrefix = value;
				this.OnTooltipChanged();
			}
		}

		/// <summary>
		/// Gets the cursor token associated with this mouse tool.
		/// </summary>
		protected CursorToken CursorToken
		{
			get { return _cursorToken; }
			set { _cursorToken = value; }
		}

		/// <summary>
		/// Gets the previous x coordinate of the mouse pointer.
		/// </summary>
		protected int LastX
		{
			get { return _lastX; }
		}

		/// <summary>
		/// Gets the previous y coordinate of the mouse pointer.
		/// </summary>
		protected int LastY
		{
			get { return _lastY; }
		}

		/// <summary>
		/// Gets the change in the x position of the mouse pointer since the previous 
		/// call to <see cref="Track"/>.
		/// </summary>
		protected int DeltaX
		{
			get { return _deltaX; }
		}

		/// <summary>
		/// Gets the change in the y position of the mouse pointer since the previous 
		/// call to <see cref="Track"/>.
		/// </summary>
		protected int DeltaY
		{
			get { return _deltaY; }
		}

		/// <summary>
		/// Gets the tooltip associated with this tool.
		/// </summary>
		/// <remarks>
		/// For mouse tools, this is a combination of <see cref="TooltipPrefix"/> 
		/// and <see cref="MouseButton"/> in the form "Prefix (button)".
		/// </remarks>
		public virtual string Tooltip
		{
			get
			{
				if (MouseButton == XMouseButtons.None)
					return TooltipPrefix;
				return string.Format("{0} ({1})", TooltipPrefix, XMouseButtonsConverter.Format(MouseButton));
			}
		}

        /// <summary>
        /// Gets whether or not this tool should be automatically selected/activated when the viewer opens initially.
        /// </summary>
        public bool InitiallyActive { get; internal set; }

		/// <summary>
		/// Gets or sets a value indicating whether this tool is currently active or not.  
		/// </summary>
		/// <remarks>
		/// Any number of mouse tools may be assigned to a given mouse button, but 
		/// only one such tool can be active at any given time.
		/// </remarks>
		public bool Active
		{
			get { return _active; }
			set
			{
				if (value == _active)
					return;

				_active = value;
				OnActivationChanged();
			}
		}

		/// <summary>
		/// Called when the <see cref="Active"/> property changes, thereby firing the <see cref="ActivationChanged"/> event.
		/// </summary>
    	protected virtual void OnActivationChanged()
    	{
    		EventsHelper.Fire(_activationChangedEvent, this, new EventArgs());
		}

    	/// <summary>
    	/// Called when the <see cref="Tooltip"/> property changes, thereby firing the <see cref="TooltipChanged"/> event.
    	/// </summary>
    	protected virtual void OnTooltipChanged()
    	{
    		EventsHelper.Fire(_tooltipChangedEvent, this, EventArgs.Empty);
    	}

    	/// <summary>
		/// Notifies observer(s) that the tooltip has changed.
		/// </summary>
		public virtual event EventHandler TooltipChanged
		{
			add { _tooltipChangedEvent += value; }
			remove { _tooltipChangedEvent -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="Active"/> property has changed.
		/// </summary>
		public event EventHandler ActivationChanged
		{
			add { _activationChangedEvent += value; }
			remove { _activationChangedEvent -= value; }
		}

    	/// <summary>
    	/// Gets the set of actions that act on this tool.
    	/// </summary>
    	/// <remarks>
    	/// <see cref="ITool.Actions"/> mentions that this property should not be considered dynamic.
    	/// This implementation assumes that the actions are <b>not</b> dynamic by lazily initializing
    	/// the actions and storing them.  If you wish to return actions dynamically, you must override
    	/// this property.
    	/// </remarks>
    	public override IActionSet Actions
    	{
    		get
    		{
    			try
    			{
    				return base.Actions;
    			}
    			finally
    			{
    				_actionsInitialized = true;
    			}
    		}
    	}

		/// <summary>
		/// Overrides <see cref="ToolBase.Initialize"/>.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			MouseToolAttributeProcessor.Process(this);
			MouseToolSettingsProfile.CurrentProfileChanged += HandleCurrentMouseToolSettingsProfileChanged;
		}

    	protected override void Dispose(bool disposing)
		{
			MouseToolSettingsProfile.CurrentProfileChanged -= HandleCurrentMouseToolSettingsProfileChanged;
			base.Dispose(disposing);
		}

		private void HandleCurrentMouseToolSettingsProfileChanged(object sender, EventArgs e)
		{
			this.OnMouseToolSettingsChanged(MouseToolSettingsProfile.Current[this.GetType()]);
		}

		/// <summary>
		/// Called when the mouse tool settings entry for this tool has changed.
		/// </summary>
		/// <remarks>
		/// The default implementation reloads all mouse button and shortcut assignments from the setting.
		/// Override this method to provide custom behaviour for attempting to change assignments on a live tool.
		/// </remarks>
		protected virtual void OnMouseToolSettingsChanged(MouseToolSettingsProfile.Setting setting)
		{
		    if (setting == null || setting.IsEmpty)
                return;

		    this.MouseButton = setting.MouseButton.GetValueOrDefault(this.MouseButton);

		    var oldDefaultMouseButtonShortcut = this.DefaultMouseButtonShortcut ?? new MouseButtonShortcut(XMouseButtons.None, ModifierFlags.None);
		    var defaultMouseButton = setting.DefaultMouseButton.GetValueOrDefault(oldDefaultMouseButtonShortcut.MouseButton);
		    if (defaultMouseButton == XMouseButtons.None && this.DefaultMouseButtonShortcut != null)
		    {
		        this.DefaultMouseButtonShortcut = null;
		    }
		    else if (defaultMouseButton != XMouseButtons.None)
		    {
		        var defaultMouseButtonModifiers = setting.DefaultMouseButtonModifiers.GetValueOrDefault(oldDefaultMouseButtonShortcut.Modifiers.ModifierFlags);
		        if (oldDefaultMouseButtonShortcut.MouseButton != defaultMouseButton || oldDefaultMouseButtonShortcut.Modifiers.ModifierFlags != defaultMouseButtonModifiers)
		            this.DefaultMouseButtonShortcut = new MouseButtonShortcut(defaultMouseButton, defaultMouseButtonModifiers);
		    }

		    //We DO change the initially active property of the tool because
		    //it essentially identifies the default mouse tool for each mouse button.
		    if (setting.InitiallyActive.HasValue)
		        InitiallyActive = setting.InitiallyActive.Value;
		}

        /// <summary>
        /// Requests that this mouse tool be the active tool for the mouse button to which it
        /// is assigned.
        /// </summary>
		public void Select()
		{
			this.Active = true;
		}

		/// <summary>
		/// Gets or sets the mouse button assigned to this tool.
		/// </summary>
		/// <remarks>
		/// It is expected that on creation of this tool, this property will be set to
		/// something other than 'None'.  Currently this is done in the overridden <see cref="Initialize" /> method.
		/// </remarks>
		public XMouseButtons MouseButton
		{
			get
			{
				return _mouseButton;
			}
			set
			{
				if (_mouseButton == value)
					return;

				_mouseButton = value;

				if (_actionsInitialized)
					UpdateMouseButtonIconSet(this.Actions, _mouseButton);

				this.OnMouseButtonChanged();

				//the mouse button assignment affects the tooltip.
				this.OnTooltipChanged();
			}
		}

		/// <summary>
		/// Gets or sets the default mouse button shortcut assigned to this tool.
		/// </summary>
		public MouseButtonShortcut DefaultMouseButtonShortcut
    	{
			get { return _defaultMouseButtonShortcut; }
			set
			{
				if (value != null && value.Equals(_defaultMouseButtonShortcut))
						return;

				_defaultMouseButtonShortcut = value;
				this.OnDefaultMouseButtonShortcutChanged();
			}
		}

		/// <summary>
		/// Gets or sets the modified mouse button shortcut assigned to this tool.
		/// </summary>
		/// <seealso cref="DefaultMouseButtonShortcut"/>
		[Obsolete("Now just gets/sets DefaultMouseButtonShortcut.")]
		public MouseButtonShortcut ModifiedMouseButtonShortcut
		{
			get { return DefaultMouseButtonShortcut; }
			set { DefaultMouseButtonShortcut = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="MouseWheelShortcut"/>.
		/// </summary>
		public MouseWheelShortcut MouseWheelShortcut
		{
			get { return _mouseWheelShortcut; }
			set
			{
				if (_mouseWheelShortcut != null && _mouseWheelShortcut.Equals(value))
					return;

				_mouseWheelShortcut = value;
				this.OnMouseWheelShortcutChanged();
			}
		}

		/// <summary>
		/// Called when the <see cref="MouseButton"/> property changes, thereby firing the <see cref="MouseButtonChanged"/> event.
		/// </summary>
		protected virtual void OnMouseButtonChanged()
		{
			EventsHelper.Fire(_mouseButtonChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Called when the <see cref="DefaultMouseButtonShortcut"/> property changes, thereby firing the <see cref="DefaultMouseButtonShortcutChanged"/> event.
		/// </summary>
		protected virtual void OnDefaultMouseButtonShortcutChanged()
		{
			EventsHelper.Fire(_defaultMouseButtonShortcutChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Called when the <see cref="MouseWheelShortcut"/> property changes, thereby firing the <see cref="MouseWheelShortcutChanged"/> event.
		/// </summary>
		protected virtual void OnMouseWheelShortcutChanged()
		{
			EventsHelper.Fire(_mouseWheelShortcutChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Fired when the <see cref="MouseButton"/> property has changed.
		/// </summary>
		public event EventHandler MouseButtonChanged
		{
			add { _mouseButtonChanged += value; }
			remove { _mouseButtonChanged -= value; }
		}

		/// <summary>
		/// Fired when the <see cref="ModifiedMouseButtonShortcut"/> property has changed.
		/// </summary>
		/// <seealso cref="DefaultMouseButtonShortcutChanged"/>
		[Obsolete("Now just observes DefaultMouseButtonShortcutChanged.")]
		public event EventHandler ModifiedMouseButtonShortcutChanged
		{
			add { DefaultMouseButtonShortcutChanged += value; }
			remove { DefaultMouseButtonShortcutChanged -= value; }
		}
		
		/// <summary>
		/// Fired when the <see cref="DefaultMouseButtonShortcut"/> property has changed.
		/// </summary>
		public event EventHandler DefaultMouseButtonShortcutChanged
		{
			add { _defaultMouseButtonShortcutChanged += value; }
			remove { _defaultMouseButtonShortcutChanged -= value; }
		}

    	/// <summary>
    	/// Fired when the <see cref="MouseWheelShortcut"/> property has changed.
    	/// </summary>
    	public event EventHandler MouseWheelShortcutChanged
    	{
    		add { _mouseWheelShortcutChanged += value; }
    		remove { _mouseWheelShortcutChanged -= value; }
    	}

    	#region IMouseButtonHandler

		/// <summary>
		/// Handles a "start mouse" message from the Framework.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>A value indicating whether the start message was handled.</returns>
		/// <remarks>
		/// <para>
		/// In most cases, <see cref="Start"/> corresponds to "mouse down".
		/// </para>
		/// <para>
		/// As a developer, you need to override this method in your 
		/// <see cref="MouseImageViewerTool"/> subclass to add your custom functionality, 
		/// but you should never have to call it; it should only ever have to be 
		/// called by the Framework.
		/// </para>
		/// </remarks>
		public virtual bool Start(IMouseInformation mouseInformation)
		{
			_lastX = mouseInformation.Location.X;
			_lastY = mouseInformation.Location.Y;

			return false;
		}

		/// <summary>
		/// Handles a "track mouse" message from the Framework.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>A value indicating whether the track message was handled.</returns>
		/// <remarks>
		/// <para>
		/// In most cases, <see cref="Track"/> corresponds to "mouse move".
		/// </para>
		/// <para>
		/// As a developer, you need to override this method in your 
		/// <see cref="MouseImageViewerTool"/> subclass to add your custom functionality, 
		/// but you should never have to call it; it should only ever have to be 
		/// called by the Framework.
		/// </para>
		/// </remarks>
		public virtual bool Track(IMouseInformation mouseInformation)
		{
			_deltaX = mouseInformation.Location.X - _lastX;
			_deltaY = mouseInformation.Location.Y - _lastY;

			_lastX = mouseInformation.Location.X;
			_lastY = mouseInformation.Location.Y;

			return false;
		}

		/// <summary>
		/// Handles a "stop mouse" message from the Framework.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>A value indicating whether the stop message was handled.</returns>
		/// <remarks>
		/// <para>
		/// In most cases, <see cref="Stop"/> corresponds to "mouse up".
		/// </para>
		/// <para>
		/// As a developer, you need to override this method in your 
		/// <see cref="MouseImageViewerTool"/> subclass to add your custom functionality, 
		/// but you should never have to call it; it should only ever have to be 
		/// called by the Framework.
		/// </para>
		/// </remarks>
		public virtual bool Stop(IMouseInformation mouseInformation)
		{
			_lastX = 0;
			_lastY = 0;

			return false;
		}

		/// <summary>
		/// Called by the framework when it needs to unexpectedly release capture on the tool, allowing it to do 
		/// any necessary cleanup.  This method should be overridden by any derived classes that need to do cleanup.
		/// </summary>
		public virtual void Cancel()
		{
		}

    	/// <summary>
    	/// Allows the <see cref="IMouseButtonHandler"/> to override certain default framework behaviour.
    	/// </summary>
    	public MouseButtonHandlerBehaviour Behaviour
		{
			get { return _mousebuttonBehaviour; }
			protected set { _mousebuttonBehaviour = value; }
		}

		#endregion

		#region Mouse Wheel

		#region IMouseWheelHandler Members

		/// <summary>
		/// Called by the framework when mouse wheel activity starts.
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		public virtual void StartWheel()
		{
		}

		/// <summary>
		/// Called by the framework each time the mouse wheel is moved.
		/// </summary>
		/// <remarks>
		/// Unless overridden, this method simply calls <see cref="WheelForward"/> and <see cref="WheelBack"/>.
		/// </remarks>
		public virtual void Wheel(int wheelDelta)
		{
			if (wheelDelta > 0)
				WheelForward();
			else if (wheelDelta < 0)
				WheelBack();
		}

		/// <summary>
		/// Called by the framework to indicate that mouse wheel activity has stopped 
		/// (a short period of time has elapsed without any activity).
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		public virtual void StopWheel()
		{
		}

		#endregion

		/// <summary>
		/// Called when the mouse wheel has moved forward.
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		protected virtual void WheelForward()
		{
		}

		/// <summary>
		/// Called when the mouse wheel has moved back.
		/// </summary>
		/// <remarks>
		/// This method does nothing unless overridden.
		/// </remarks>
		protected virtual void WheelBack()
		{
		}

		#endregion

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token associated with the tool.
		/// </summary>
		/// <param name="point">The point in destination (view) coordinates.</param>
		/// <returns>a <see cref="CursorToken"/> object that is used to construct the cursor in the view.</returns>
		public virtual CursorToken GetCursorToken(Point point)
		{
			return this.CursorToken;
		}

		#endregion
	}
}
