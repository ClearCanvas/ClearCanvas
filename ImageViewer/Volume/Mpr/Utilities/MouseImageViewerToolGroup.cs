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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using Action = ClearCanvas.Desktop.Actions.Action;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	public abstract class MouseImageViewerToolGroup<T> : MouseImageViewerTool where T : MouseImageViewerTool
	{
		private IToolSet _toolSet;
		private IActionSet _actionSet;
		private T _selectedTool;
		private event EventHandler _selectedToolChanged;

		protected MouseImageViewerToolGroup() {}

		#region Selected Slave Tool

		protected T SelectedTool
		{
			get { return _selectedTool; }
			private set
			{
				if (_selectedTool != value)
				{
					if (_selectedTool != null)
						this.OnToolUnselected(_selectedTool);

					_selectedTool = value;

					if (_selectedTool != null)
						this.OnToolSelected(_selectedTool);

					this.Active = (_selectedTool != null);
					this.OnSelectedToolChanged();
				}
			}
		}

		protected virtual void OnSelectedToolChanged()
		{
			EventsHelper.Fire(_selectedToolChanged, this, EventArgs.Empty);
		}

		protected virtual void OnToolSelected(T tool)
		{
			this.Behaviour = tool.Behaviour;
			this.DefaultMouseButtonShortcut = tool.DefaultMouseButtonShortcut;
			this.MouseButton = tool.MouseButton;
			this.MouseWheelShortcut = tool.MouseWheelShortcut;
			this.Select();
		}

		protected virtual void OnToolUnselected(T tool)
		{
			this.Behaviour = MouseButtonHandlerBehaviour.Default;
			this.DefaultMouseButtonShortcut = new MouseButtonShortcut(XMouseButtons.None);
			// We should technically deassign MouseButton here, but it causes a logged
			// error if the tool also happens to be active currently. There's no problem
			// if we don't deassign it however, as you can't actually do anything with
			// this tool until OnToolSelected gets called again, whereupon the correct
			// MouseButton will be assigned.
			this.MouseWheelShortcut = new MouseWheelShortcut();
		}

		public event EventHandler SelectedToolChanged
		{
			add { _selectedToolChanged += value; }
			remove { _selectedToolChanged -= value; }
		}

		#endregion

		#region Slave Tools and Actions

		protected abstract IEnumerable<T> CreateTools();

		public override IActionSet Actions
		{
			get
			{
				if (_actionSet == null)
				{
					IActionSet actionSet = new ActionSet();
					foreach (T tool in _toolSet.Tools)
						actionSet = actionSet.Union(new ActionSet(CustomActionAttributeProcessor.Process(tool)));
					_actionSet = actionSet;
				}
				return _actionSet.Union(base.Actions);
			}
		}

		public IActionSet SlaveActions
		{
			get { return _actionSet; }
		}

		protected IList<T> SlaveTools
		{
			get
			{
				if (_toolSet == null)
					return null;
				return new List<T>(ConvertTools(_toolSet.Tools)).AsReadOnly();
			}
		}

		private static IEnumerable<T> ConvertTools(IEnumerable<ITool> tools)
		{
			foreach (ITool tool in tools)
				yield return (T) tool;
		}

		private static void UpdateMouseButtonIconSet(IActionSet actions, XMouseButtons mouseButton)
		{
			foreach (IAction action in actions)
			{
				if (action is Action && action.IconSet is MouseButtonIconSet)
					((Action) action).IconSet = new MouseButtonIconSet(action.IconSet, mouseButton);
			}
		}

		#endregion

		#region Initialization and Disposal

		protected virtual void OnToolInitialized(T tool)
		{
			tool.ActivationChanged += OnToolActivationChanged;
			tool.DefaultMouseButtonShortcutChanged += OnToolDefaultMouseButtonShortcutChanged;
			tool.MouseButtonChanged += OnToolMouseButtonChanged;
			tool.MouseWheelShortcutChanged += OnToolMouseWheelShortcutChanged;
			tool.TooltipChanged += OnToolTooltipChanged;
		}

		protected virtual void OnToolDisposing(T tool)
		{
			tool.TooltipChanged -= OnToolTooltipChanged;
			tool.MouseWheelShortcutChanged -= OnToolMouseWheelShortcutChanged;
			tool.MouseButtonChanged -= OnToolMouseButtonChanged;
			tool.DefaultMouseButtonShortcutChanged -= OnToolDefaultMouseButtonShortcutChanged;
			tool.ActivationChanged -= OnToolActivationChanged;
		}

		public override void Initialize()
		{
			base.Initialize();

			InitializeSlaveTools();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeSlaveTools();
			}

			base.Dispose(disposing);
		}

		protected void ReinitializeTools()
		{
			this.DisposeSlaveTools();
			this.InitializeSlaveTools();
		}

		private void InitializeSlaveTools()
		{
			_toolSet = new ToolSet(this.CreateTools(), new ToolContext(this.ImageViewer));
			foreach (T tool in _toolSet.Tools)
				this.OnToolInitialized(tool);
		}

		private void DisposeSlaveTools()
		{
			this._actionSet = null;
			this.SelectedTool = null;

			if (_toolSet != null)
			{
				foreach (T tool in _toolSet.Tools)
					this.OnToolDisposing(tool);
				_toolSet.Dispose();
				_toolSet = null;
			}
		}

		#endregion

		#region Tool Activation

		private bool _suspendToolActivationChangedEvent = false;

		protected virtual void OnToolActivationChanged(object sender, EventArgs e)
		{
			T tool = (T) sender;
			if (tool.Active)
				this.SelectedTool = tool;
			ResyncToolActivation();
		}

		protected override void OnActivationChanged()
		{
			base.OnActivationChanged();
			if (!this.Active)
				this.SelectedTool = null;
			ResyncToolActivation();
		}

		private void ResyncToolActivation()
		{
			if (_suspendToolActivationChangedEvent)
				return;

			_suspendToolActivationChangedEvent = true;
			try
			{
				T selectedTool = this.SelectedTool;
				foreach (T tool in _toolSet.Tools)
					tool.Active = (tool == selectedTool);

				// call it one more time just to make sure
				if (selectedTool != null)
					selectedTool.Select();
			}
			finally
			{
				_suspendToolActivationChangedEvent = false;
			}
		}

		#endregion

		#region Tool Property Forwarding

		protected virtual void OnToolTooltipChanged(object sender, EventArgs e)
		{
			if (this.SelectedTool == sender)
				this.TooltipPrefix = this.SelectedTool.Tooltip;
		}

		protected virtual void OnToolMouseWheelShortcutChanged(object sender, EventArgs e)
		{
			if (this.SelectedTool == sender)
				this.MouseWheelShortcut = this.SelectedTool.MouseWheelShortcut;
		}

		protected virtual void OnToolMouseButtonChanged(object sender, EventArgs e)
		{
			T senderTool = (T) sender;
			UpdateMouseButtonIconSet(senderTool.Actions, senderTool.MouseButton);

			if (this.SelectedTool == sender)
				this.MouseButton = this.SelectedTool.MouseButton;
		}

		protected virtual void OnToolDefaultMouseButtonShortcutChanged(object sender, EventArgs e)
		{
			if (this.SelectedTool == sender)
				this.DefaultMouseButtonShortcut = this.SelectedTool.DefaultMouseButtonShortcut;
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.GetCursorToken(point);
			return base.GetCursorToken(point);
		}

		#endregion

		#region Mouse Input Forwarding

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.Start(mouseInformation);
			return base.Start(mouseInformation);
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.Track(mouseInformation);
			return base.Track(mouseInformation);
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (this.SelectedTool != null)
				return this.SelectedTool.Stop(mouseInformation);
			return base.Stop(mouseInformation);
		}

		public override void Cancel()
		{
			if (this.SelectedTool != null)
				this.SelectedTool.Cancel();
		}

		public override void StartWheel()
		{
			if (this.SelectedTool != null)
				this.SelectedTool.StartWheel();
		}

		public override void Wheel(int wheelDelta)
		{
			if (this.SelectedTool != null)
				this.SelectedTool.Wheel(wheelDelta);
		}

		public override void StopWheel()
		{
			if (this.SelectedTool != null)
				this.SelectedTool.StopWheel();
		}

		#endregion

		#region ToolContext Class

		private class ToolContext : IImageViewerToolContext
		{
			private readonly IImageViewer _viewer;

			public ToolContext(IImageViewer viewer)
			{
				_viewer = viewer;
			}

			public IImageViewer Viewer
			{
				get { return _viewer; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _viewer.DesktopWindow; }
			}

			/// <summary>
			/// Not supported.
			/// </summary>
			public event CancelEventHandler ViewerClosing
			{
				add { }
				remove { }
			}
		}

		#endregion

		#region CustomActionAttributeProcessor Class

		private static class CustomActionAttributeProcessor
		{
			/// <summary>
			/// Processes the set of action attributes declared on a given target object to generate the
			/// corresponding set of <see cref="IAction"/> objects.
			/// </summary>
			/// <param name="actionTarget">The target object on which the attributes are declared, typically a tool.</param>
			/// <returns>The resulting set of actions, where each action is bound to the target object.</returns>
			public static IAction[] Process(object actionTarget)
			{
				object[] attributes = actionTarget.GetType().GetCustomAttributes(typeof (ActionAttribute), true);

				// first pass - create an ActionBuilder for each initiator of the specified type
				List<CustomActionBuildingContext> actionBuilders = new List<CustomActionBuildingContext>();
				foreach (ActionAttribute a in attributes)
				{
					if (a is ActionInitiatorAttribute)
					{
						CustomActionBuildingContext actionBuilder = new CustomActionBuildingContext(GetQualifiedActionID(a, actionTarget), actionTarget);
						a.Apply(actionBuilder);
						actionBuilders.Add(actionBuilder);
					}
				}

				// second pass - apply decorators to all ActionBuilders with same actionID
				foreach (ActionAttribute a in attributes)
				{
					if (a is ActionDecoratorAttribute)
					{
						foreach (CustomActionBuildingContext actionBuilder in actionBuilders)
						{
							if (GetQualifiedActionID(a, actionTarget) == actionBuilder.ActionID)
							{
								a.Apply(actionBuilder);
							}
						}
					}
				}

				List<IAction> actions = new List<IAction>();
				foreach (CustomActionBuildingContext actionBuilder in actionBuilders)
				{
					actions.Add(actionBuilder.Action);
				}

				return actions.ToArray();
			}

			private static string GetQualifiedActionID(ActionAttribute a, object actionTarget)
			{
				return string.Format("{0}_{1:X8}", a.QualifiedActionID(actionTarget), actionTarget.GetHashCode());
			}

			/// <summary>
			/// Default implementation of <see cref="IActionBuildingContext"/>.
			/// </summary>
			private class CustomActionBuildingContext : IActionBuildingContext
			{
				private readonly string _actionID;
				private readonly object _actionTarget;
				private readonly ResourceResolver _resolver;
				private Action _action;

				public CustomActionBuildingContext(string actionID, object actionTarget)
				{
					_actionID = actionID;
					_actionTarget = actionTarget;

					_resolver = new ActionResourceResolver(_actionTarget);
				}

				public string ActionID
				{
					get { return _actionID; }
				}

				public Action Action
				{
					get { return _action; }
					set { _action = value; }
				}

				public IResourceResolver ResourceResolver
				{
					get { return _resolver; }
				}

				public object ActionTarget
				{
					get { return _actionTarget; }
				}
			}
		}

		#endregion
	}

	#region PersistentActionAttribute Class

	public sealed class PersistentAttribute : ActionDecoratorAttribute
	{
		private readonly bool _persistent;

		public PersistentAttribute(string actionId, bool persistent)
			: base(actionId)
		{
			_persistent = persistent;
		}

		public override void Apply(IActionBuildingContext builder)
		{
			builder.Action.Persistent = _persistent;
		}
	}

	#endregion
}