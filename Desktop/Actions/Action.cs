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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Abstract base class providing a default implementation of <see cref="IAction"/>.  
	/// </summary>
	/// <remarks>
	/// Action classes should inherit from this class rather than implement <see cref="IAction"/> directly.
	/// </remarks>
	public abstract class Action : IAction
	{
		private readonly string _actionID;

		private ActionPath _path;
		private readonly IResourceResolver _resourceResolver;

		private GroupHint _groupHint;

		private IconSet _iconSet;
		private event EventHandler _iconSetChanged;

		private bool _enabled;
		private event EventHandler _enabledChanged;

		private bool _visible;
		private event EventHandler _visibleChanged;

		private string _tooltip;
		private event EventHandler _tooltipChanged;

		private string _label;
		private event EventHandler _labelChanged;

		private bool _available;
		private event EventHandler _availableChanged;

		private bool _persistent;

		private ISpecification _permissionSpec;
		private ISpecification _featureSpec;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="path">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve icons associated with this action.</param>
		protected Action(string actionID, ActionPath path, IResourceResolver resourceResolver)
		{
			_actionID = actionID;
			_path = path;
			_resourceResolver = resourceResolver;

			// smart defaults
			_enabled = true;
			_visible = true;
			_available = true;

			_persistent = false;

			FormerActionIDs = new List<string>();
		}

		/// <summary>
		/// Sets the <see cref="ISpecification"/> that is tested to establish whether the 
		/// current user has sufficient privileges to access the action.
		/// </summary>
		/// <remarks>
		/// This overload is useful when an actions permissibility is a boolean function of a
		/// multiple authority tokens.  Use the <see cref="PrincipalPermissionSpecification"/>, in 
		/// combination with the <see cref="AndSpecification"/> and <see cref="OrSpecification"/> classes
		/// to build up a complex specification for permissibility.
		/// </remarks>
		public void SetPermissibility(ISpecification permissionSpecification)
		{
			_permissionSpec = permissionSpecification;
		}

		/// <summary>
		/// Sets a single authority token that is tested to establish whether the 
		/// current user has sufficient privileges to access the action.
		/// </summary>
		/// <remarks>
		/// This overload is useful in the common case where an actions permissibility
		/// is tied to a single authority token.  To handle a situation where the permissibility
		/// is a function of multiple authority tokens, use the <see cref="SetPermissibility(ISpecification)"/>
		/// overload.
		/// </remarks>
		/// <param name="authorityToken"></param>
		public void SetPermissibility(string authorityToken)
		{
			SetPermissibility(new PrincipalPermissionSpecification(authorityToken));
		}

		/// <summary>
		/// Provides internal access to the permission specification.
		/// </summary>
		internal ISpecification PermissionSpecification
		{
			get { return _permissionSpec; }
			set { _permissionSpec = value; }
		}

		/// <summary>
		/// Sets the <see cref="ISpecification"/> that is tested to establish whether the
		/// current installation is licensed to access the action.
		/// </summary>
		/// <param name="featureSpecification">An <see cref="ISpecification"/> used to determine whether or not the application license allows access to the action.</param>
		public void SetFeatureAuthorization(ISpecification featureSpecification)
		{
			_featureSpec = featureSpecification;
		}

		/// <summary>
		/// Sets a feature token that is tested to establish whether the
		/// current installation is licensed to access the action.
		/// </summary>
		/// <param name="featureToken">A feature identification token used to determine whether or not the application license allows access to the action.</param>
		public void SetFeatureAuthorization(string featureToken)
		{
			SetFeatureAuthorization(new FeatureAuthorizationSpecification(featureToken));
		}

		/// <summary>
		/// Gets or sets the <see cref="ISpecification"/> that is tested to establish whether the
		/// current installation is licensed to access the action.
		/// </summary>
		internal ISpecification FeatureSpecification
		{
			get { return _featureSpec; }
			set { _featureSpec = value; }
		}

		#region IAction members

		/// <summary>
		/// Gets the fully-qualified logical identifier for this action.
		/// </summary>
		public string ActionID
		{
			get { return _actionID; }
		}

		/// <summary>
		/// Gets any former <see cref="IAction.ActionID"/>s, in case an <see cref="IAction"/>
		/// or <see cref="Tool{TContextInterface}"/> has moved.
		/// </summary>
		/// <remarks>Use the <see cref="ActionFormerlyAttribute"/> in action declarations to indicate to the framework
		/// that an action used to have a different <see cref="IAction.ActionID"/>. This way, the action will not lose it's place
		/// in the action model just because the code moved. Obviously, only do this if, indeed, you want the action
		/// to maintain it's same place in the action model.
		/// </remarks>
		public IList<string> FormerActionIDs { get; private set; }

		/// <summary>
		/// Gets the resource resolver associated with this action, that will be used to resolve
		/// action path and icon resources when required.
		/// </summary>
		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
		}

		/// <summary>
		/// Gets or sets the menu or toolbar path for this action.
		/// </summary>
		public virtual ActionPath Path
		{
			get { return _path; }
			set { _path = value; }
		}

		/// <summary>
		/// Gets or sets the group hint for this action.
		/// </summary>
		/// <remarks>
		/// The GroupHint for an action must not be null.  If an action has no groupHint,
		/// the GroupHint should be "" (default).
		/// </remarks>
		public virtual GroupHint GroupHint
		{
			get
			{
				if (_groupHint == null)
					_groupHint = new GroupHint("");

				return _groupHint;
			}
			set
			{
				_groupHint = value;

				if (_groupHint == null)
					_groupHint = new GroupHint("");
			}
		}

		/// <summary>
		/// Gets the icon that the action presents in the UI.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
			set
			{
				if (_iconSet == value)
					return;

				_iconSet = value;
				EventsHelper.Fire(_iconSetChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the label that the action presents in the UI.
		/// </summary>
		[Localizable(true)]
		public string Label
		{
			get { return _label; }
			set
			{
				if (value != _label)
				{
					_label = value;
					EventsHelper.Fire(_labelChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the tooltip that the action presents in the UI.
		/// </summary>
		[Localizable(true)]
		public string Tooltip
		{
			get { return _tooltip; }
			set
			{
				if (value != _tooltip)
				{
					_tooltip = value;
					EventsHelper.Fire(_tooltipChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the enablement state that the action presents in the UI.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (value != _enabled)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the visibility state that the action presents in the UI.
		/// </summary>
		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (value != _visible)
				{
					_visible = value;
					EventsHelper.Fire(_visibleChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets whether or not the action is available as controlled by the user.
		/// </summary>
		/// <remarks>
		/// The value of <see cref="Available"/> should override <see cref="Visible"/>
		/// as it represents the user's desire to see the action at all, rather than tool logic.
		/// </remarks>
		public virtual bool Available
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

		/// <summary>
		/// Gets a value indicating whether or not the action is 'persistent'.
		/// </summary>
		/// <remarks>
		/// Actions created via the Action attributes are considered persistent and are
		/// committed to the <see cref="ActionModelSettings"/>,
		/// otherwise they are considered generated and they are not committed.
		/// </remarks>
		public virtual bool Persistent
		{
			get { return _persistent; }
			set { _persistent = value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IAction.Enabled"/> property of this action changes.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IAction.Visible"/> property of this action changes.
		/// </summary>
		public event EventHandler VisibleChanged
		{
			add { _visibleChanged += value; }
			remove { _visibleChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IAction.Available"/> property of this action changes.
		/// </summary>
		public event EventHandler AvailableChanged
		{
			add { _availableChanged += value; }
			remove { _availableChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IAction.Label"/> property of this action changes.
		/// </summary>
		public event EventHandler LabelChanged
		{
			add { _labelChanged += value; }
			remove { _labelChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IAction.Tooltip"/> property of this action changes.
		/// </summary>
		public event EventHandler TooltipChanged
		{
			add { _tooltipChanged += value; }
			remove { _tooltipChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IAction.IconSet"/> property of this action changes.
		/// </summary>
		public event EventHandler IconSetChanged
		{
			add { _iconSetChanged += value; }
			remove { _iconSetChanged -= value; }
		}

		/// <summary>
		/// Gets a value indicating whether this action is permissible.
		/// </summary>
		/// <remarks>
		/// In addition to the <see cref="IAction.Visible"/> and <see cref="IAction.Enabled"/> properties, the view
		/// will use this property to control whether the action can be invoked.  Typically
		/// this property is implemented to indicate whether the current user has permission
		/// to execute the action.
		/// </remarks>
		public bool Permissible
		{
			get
			{
				// feature spec takes precendence using AND logic
				if (_featureSpec != null && _featureSpec.Test(this).Fail) return false;

				// no permission spec, so assume this action is not protected at all
				if (_permissionSpec == null)
					return true;

				// test this action against the permission spec
				return _permissionSpec.Test(this).Success;
			}
		}

		#endregion
	}
}