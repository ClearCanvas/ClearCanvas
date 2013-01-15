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
using System.Reflection;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Attribute class used to define <see cref="DropDownAction"/>s.
	/// </summary>
	public class DropDownActionAttribute : ActionInitiatorAttribute
	{
		private readonly string _path;
		private readonly string _menuModelPropertyName;
		private bool _initiallyAvailable = true;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The action ID.</param>
		/// <param name="path">A path indicating which toolbar the dropdown button should appear on.</param>
		/// <param name="menuModelPropertyName">The name of the property in the target class (i.e. the
		/// class to which this attribute is applied) that returns the menu model as an <see cref="ActionModelNode"/>.</param>
		public DropDownActionAttribute(string actionID, string path, string menuModelPropertyName)
			: base(actionID)
		{
			Platform.CheckForEmptyString(actionID, "actionID");
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForEmptyString(menuModelPropertyName, "menuModelPropertyName");

			_path = path;
			_menuModelPropertyName = menuModelPropertyName;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the action should be available by default when not overriden by the action model.
		/// </summary>
		public bool InitiallyAvailable
		{
			get { return _initiallyAvailable; }
			set { _initiallyAvailable = value; }
		}

		/// <summary>
		/// Constructs/initializes a <see cref="DropDownAction"/> via the given <see cref="IActionBuildingContext"/>.
		/// </summary>
		/// <remarks>For internal framework use only.</remarks>
		public override void Apply(IActionBuildingContext builder)
		{
			ActionPath path = new ActionPath(_path, builder.ResourceResolver);
			builder.Action = new DropDownAction(builder.ActionID, path, builder.ResourceResolver);
			builder.Action.Available = this.InitiallyAvailable;
			builder.Action.Persistent = true;
			builder.Action.Label = path.LastSegment.LocalizedText;

			((DropDownAction)builder.Action).SetMenuModelDelegate(
				CreateGetMenuModelDelegate(builder.ActionTarget, _menuModelPropertyName));
		}

		/// <summary>
		/// Validates the property exists and has a public get method before returning them as out parameters.
		/// </summary>
		/// <exception cref="ActionBuilderException">Thrown if the property doesn't exist or does not have a public get method.</exception>
		protected internal static void GetPropertyAndGetter(object target, string propertyName, Type type, out PropertyInfo info, out MethodInfo getter)
		{
			info = target.GetType().GetProperty(propertyName, type);
			if (info == null)
			{
				throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderPropertyDoesNotExist, propertyName, target.GetType().FullName));
			}

			getter = info.GetGetMethod();
			if (getter == null)
			{
				throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderPropertyDoesNotHavePublicGetMethod, propertyName, target.GetType().FullName));
			}
		}

        internal static Func<ActionModelNode> CreateGetMenuModelDelegate(object target, string propertyName)
		{
			PropertyInfo info;
			MethodInfo getter;
			GetPropertyAndGetter(target, propertyName, typeof(ActionModelNode), out info, out getter);

            return (Func<ActionModelNode>)Delegate.CreateDelegate(typeof(Func<ActionModelNode>), target, getter);
		}
	}
}
