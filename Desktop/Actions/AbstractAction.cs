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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// A view-less implementation of <see cref="IAction"/>.
	/// </summary>
	internal class AbstractAction : IAction
	{
		#region Static Helpers

		/// <summary>
		/// Creates an <see cref="AbstractAction"/> from a concrete <see cref="IAction"/>.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public static AbstractAction Create(IAction action)
		{
			if (action is IClickAction)
				return new AbstractClickAction((IClickAction) action);
			return new AbstractAction(action);
		}

		public static AbstractAction Create(string id, string path, bool isClickAction)
		{
			if (isClickAction)
				return new AbstractClickAction(id, path);
			return new AbstractAction(id, path);
		}

		public static AbstractAction Create(string id, string path, bool isClickAction, IResourceResolver resourceResolver)
		{
			if (isClickAction)
				return new AbstractClickAction(id, path, resourceResolver);
			return new AbstractAction(id, path, resourceResolver);
		}

		#endregion

		private event EventHandler _availableChanged;

		private static readonly IResourceResolver _globalResourceResolver = new ApplicationThemeResourceResolver(AppDomain.CurrentDomain.GetAssemblies());

		private readonly IResourceResolver _resourceResolver;
		private readonly IconSet _iconSet;
		private readonly string _actionId;
        private readonly IList<string> _formerActionIds;
        private readonly string _label;
		private readonly string _tooltip;
		private readonly bool _permissible;

		private ActionPath _path;
		private GroupHint _groupHint;
		private bool _available;

		private AbstractAction(string id, string path)
			: this(id, path, _globalResourceResolver) {}

		private AbstractAction(string id, string path, IResourceResolver resourceResolver)
		{
			Platform.CheckForEmptyString(id, "id");
			Platform.CheckForEmptyString(path, "path");

			_resourceResolver = resourceResolver;
			_actionId = id;
            _formerActionIds = new List<string>();
			_path = new ActionPath(path, resourceResolver);
			_groupHint = new GroupHint(string.Empty);
			_label = string.Empty;
			_tooltip = string.Empty;
			_iconSet = null;
			_available = true;
			_permissible = false;
		}

		private AbstractAction(IAction concreteAction)
		{
			Platform.CheckForNullReference(concreteAction, "concreteAction");
			Platform.CheckTrue(concreteAction.Persistent, "Action must be persistent.");

			_resourceResolver = concreteAction.ResourceResolver;
			_actionId = concreteAction.ActionID;
		    _formerActionIds = new List<string>(concreteAction.FormerActionIDs);
			_path = new ActionPath(concreteAction.Path.ToString(), concreteAction.ResourceResolver);
			_groupHint = new GroupHint(concreteAction.GroupHint.Hint);
			_label = concreteAction.Label;
			_tooltip = concreteAction.Tooltip;
			_iconSet = concreteAction.IconSet;
			_available = concreteAction.Available;
			_permissible = concreteAction.Permissible;
		}

		public string ActionId
		{
			get { return _actionId; }
		}

		string IAction.ActionID
		{
			get { return this.ActionId; }
		}

        IList<string> IAction.FormerActionIDs
        {
            get { return _formerActionIds; }
        }

		public ActionPath Path
		{
			get { return _path; }
			set { _path = value; }
		}

		public string GroupHint
		{
			get { return _groupHint.Hint; }
			set { _groupHint = new GroupHint(value); }
		}

		GroupHint IAction.GroupHint
		{
			get { return _groupHint; }
			set { _groupHint = value; }
		}

		public bool Available
		{
			get { return _available; }
			set
			{
				if (_available != value)
				{
					_available = value;
					EventsHelper.Fire(_availableChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler AvailableChanged
		{
			add { _availableChanged += value; }
			remove { _availableChanged -= value; }
		}

		/// <summary>
		/// <see cref="Label"/> is currently not persisted in the action model.
		/// </summary>
		public string Label
		{
			get { return _label; }
		}

		public event EventHandler LabelChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// <see cref="Tooltip"/> is currently not persisted in the action model.
		/// </summary>
		public string Tooltip
		{
			get { return _tooltip; }
		}

		public event EventHandler TooltipChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// <see cref="IconSet"/> is currently not persisted in the action model.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
		}

		public event EventHandler IconSetChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// This value is always determined at runtime based on the user's access permissions.
		/// </summary>
		public bool Permissible
		{
			get { return _permissible; }
		}

		/// <summary>
		/// This value is always based on the assembly defining the action.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
		}

		/// <summary>
		/// This value is always true, otherwise there's point in configuring it's position/properties in the persisted action model.
		/// </summary>
		bool IAction.Persistent
		{
			get { return true; }
		}

		/// <summary>
		/// This value is always dynamically tool-controlled and may not be overriden by the persisted action model.
		/// </summary>
		bool IAction.Enabled
		{
			get { return true; }
		}

		/// <summary>
		/// This event never fires becauses <see cref="Enabled"/> is not persistable.
		/// </summary>
		event EventHandler IAction.EnabledChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// This value is always dynamically tool-controlled and may not be overriden by the persisted action model.
		/// </summary>
		bool IAction.Visible
		{
			get { return true; }
		}

		/// <summary>
		/// This event never fires becauses <see cref="Visible"/> is not persistable.
		/// </summary>
		event EventHandler IAction.VisibleChanged
		{
			add { }
			remove { }
		}

		#region AbstractClickAction Class

		/// <summary>
		/// A view-less implementation of <see cref="IClickAction"/>.
		/// </summary>
		private class AbstractClickAction : AbstractAction, IClickAction
		{
			private XKeys _keyStroke;

			public AbstractClickAction(IClickAction concreteAction)
				: base(concreteAction)
			{
				_keyStroke = concreteAction.KeyStroke;
			}

			public AbstractClickAction(string id, string path)
				: base(id, path)
			{
				_keyStroke = XKeys.None;
			}

			public AbstractClickAction(string id, string path, IResourceResolver resourceResolver)
				: base(id, path, resourceResolver)
			{
				_keyStroke = XKeys.None;
			}

			public XKeys KeyStroke
			{
				get { return _keyStroke; }
				set { _keyStroke = value; }
			}

			/// <summary>
			/// This value is always dynamically tool-controlled and may not be overriden by the persisted action model.
			/// </summary>
			bool IClickAction.Checked
			{
				get { return false; }
			}

			/// <summary>
			/// This event never fires becauses <see cref="Checked"/> is not persistable.
			/// </summary>
			event EventHandler IClickAction.CheckedChanged
			{
				add { }
				remove { }
			}

			/// <summary>
			/// This value describes behaviour that is prescribed by the tool and may not be overriden by the persisted action model.
			/// </summary>
			bool IClickAction.IsCheckAction
			{
				get { return false; }
			}

			/// <summary>
			/// This value describes behaviour that is prescribed by the tool and may not be overriden by the persisted action model.
			/// </summary>
			bool IClickAction.CheckParents
			{
				get { return false; }
			}

			/// <summary>
			/// This method invokes behaviour that is performed by the tool and may not be overriden by the persisted action model.
			/// </summary>
			void IClickAction.Click() {}
		}

		#endregion
	}
}