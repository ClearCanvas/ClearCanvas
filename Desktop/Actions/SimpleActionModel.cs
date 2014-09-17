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

using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Represents a simple, in memory action model that is created in code, not via attributes.
	/// </summary>
	/// <remarks>
	/// The <see cref="SimpleActionModel"/> is particularly useful for action models that 
	/// are created in code and/or are not intended to be dynamic or extensible.
	/// </remarks>
	public class SimpleActionModel : ActionModelRoot
	{
		private readonly IResourceResolver _resolver;
		private readonly Dictionary<object, Action> _actions;
		private int _anonymousItemCount;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="resolver">The <see cref="IResourceResolver"/> used to resolve the path and icons for the actions.</param>
		public SimpleActionModel(IResourceResolver resolver)
		{
			_resolver = resolver;
			_actions = new Dictionary<object, Action>();
		}

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		public ClickAction AddAction(object key, [param : Localizable(true)] string displayName, string icon)
		{
			return AddActionHelper(key, displayName, icon, displayName, null, null);
		}

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
		public ClickAction AddAction(object key, [param : Localizable(true)] string displayName, string icon, ClickHandlerDelegate clickHandler)
		{
			return AddActionHelper(key, displayName, icon, displayName, clickHandler, null);
		}

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		public ClickAction AddAction(object key, [param : Localizable(true)] string displayName, string icon, [param : Localizable(true)] string tooltip)
		{
			return AddActionHelper(key, displayName, icon, tooltip, null, null);
		}

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
		public ClickAction AddAction(object key, [param : Localizable(true)] string displayName, string icon, [param : Localizable(true)] string tooltip, ClickHandlerDelegate clickHandler)
		{
			return AddActionHelper(key, displayName, icon, tooltip, clickHandler, null);
		}

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
		/// <param name="authorityToken">The authority token for the action.</param>
		public ClickAction AddAction(object key, [param : Localizable(true)] string displayName, string icon, [param : Localizable(true)] string tooltip, ClickHandlerDelegate clickHandler, string authorityToken)
		{
			return AddActionHelper(key, displayName, icon, tooltip, clickHandler, new PrincipalPermissionSpecification(authorityToken));
		}

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
		/// <param name="permissionSpec">The permission specification for the action.</param>
		public ClickAction AddAction(object key, [param : Localizable(true)] string displayName, string icon, [param : Localizable(true)] string tooltip, ClickHandlerDelegate clickHandler, ISpecification permissionSpec)
		{
			return AddActionHelper(key, displayName, icon, tooltip, clickHandler, permissionSpec);
		}

		/// <summary>
		/// Adds a <see cref="TextBoxAction"/> to this action model.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="tooltip"></param>
		/// <param name="permissionSpec"></param>
		/// <returns></returns>
		public TextBoxAction AddTextBoxAction(object key, [param : Localizable(true)] string tooltip, ISpecification permissionSpec)
		{
			Platform.CheckForNullReference(key, "key");

			var actionId = MakeAnonymousId();
			var action = new TextBoxAction(actionId, MakePath(actionId), _resolver) {Tooltip = tooltip};

			if (permissionSpec != null)
			{
				action.SetPermissibility(permissionSpec);
			}

			this.InsertAction(action);

			_actions[key] = action;

			return action;
		}

		/// <summary>
		/// Adds a separator at the current position.
		/// </summary>
		public void AddSeparator()
		{
			this.InsertSeparator(MakePath(MakeAnonymousId()));
		}

		/// <summary>
		/// Gets actions by key.
		/// </summary>
		public Action this[object key]
		{
			get { return _actions[key]; }
		}

		private ClickAction AddActionHelper(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler, ISpecification permissionSpec)
		{
			Platform.CheckForNullReference(key, "key");

			var action = new ClickAction(displayName, MakePath(displayName), ClickActionFlags.None, _resolver)
			             	{
			             		Tooltip = tooltip,
			             		Label = displayName
			             	};
			if (icon != null)
				action.IconSet = new IconSet(icon, icon, icon);

			if (clickHandler != null)
			{
				action.SetClickHandler(clickHandler);
			}
			if (permissionSpec != null)
			{
				action.SetPermissibility(permissionSpec);
			}

			this.InsertAction(action);

			_actions[key] = action;

			return action;
		}

		private ActionPath MakePath(string itemName)
		{
			return new ActionPath(string.Format("root/{0}", itemName), _resolver);
		}

		private string MakeAnonymousId()
		{
			return string.Format("item{0}", _anonymousItemCount++);
		}
	}
}