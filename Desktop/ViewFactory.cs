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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Provides convenience methods for instantiating views.
	/// </summary>
	public static class ViewFactory
	{
		/// <summary>
		/// Creates a view for the specified extension point and GUI toolkit.
		/// </summary>
		/// <param name="extensionPoint">The view extension point.</param>
		/// <param name="toolkitId">The identifier of the desired GUI toolkit.</param>
		/// <returns>A new instance of a view extension for the specified extension point.</returns>
		/// <exception cref="NotSupportedException">Thrown if a view extension matching the specified GUI toolkit does not exist.</exception>
		public static IView CreateView(IExtensionPoint extensionPoint, string toolkitId)
		{
			// create an attribute representing the GUI toolkitID
			GuiToolkitAttribute toolkitAttr = new GuiToolkitAttribute(toolkitId);

			// create an extension that is tagged with the same toolkit
			return (IView) extensionPoint.CreateExtension(new AttributeExtensionFilter(toolkitAttr));
		}

		/// <summary>
		/// Creates a view for the specified extension point and current GUI toolkit.
		/// </summary>
		/// <param name="extensionPoint">The view extension point.</param>
		/// <returns>A new instance of a view extension for the specified extension point.</returns>
		/// <exception cref="NotSupportedException">Thrown if a view extension matching the specified GUI toolkit does not exist.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the main application view has not been created yet.</exception>
		public static IView CreateView(IExtensionPoint extensionPoint)
		{
			return CreateView(extensionPoint, Application.GuiToolkitID);
		}

		/// <summary>
		/// Gets whether or not a view is available for the view extension point associated with specified model type and current GUI toolkit.
		/// </summary>
		/// <remarks>
		/// The view is determined based on the view extension point that is associated with the specified
		/// model type using the <see cref="AssociateViewAttribute"/> attribute.
		/// </remarks>
		/// <param name="modelType">The type of the model object.</param>
		/// <returns>True if a matching, associated view extension is available; False otherwise.</returns>
		/// <exception cref="ArgumentException">Thrown if the type <paramref name="modelType"/> does not have an associated view extension point (doesn't define <see cref="AssociateViewAttribute"/>).</exception>
		/// <exception cref="InvalidOperationException">Thrown if the main application view has not been created yet.</exception>
		public static bool IsAssociatedViewAvailable(Type modelType)
		{
			IExtensionPoint viewExtPoint = GetViewExtensionPoint(modelType);

			// check if any extensions are available that match the GUI toolkit
			GuiToolkitAttribute toolkitAttr = new GuiToolkitAttribute(Application.GuiToolkitID);
			return viewExtPoint.ListExtensions(new AttributeExtensionFilter(toolkitAttr)).Length > 0;
		}

		/// <summary>
		/// Gets whether or not a view is available for the view extension point associated with specified model type and current GUI toolkit.
		/// </summary>
		/// <remarks>
		/// The view is determined based on the view extension point that is associated with the specified
		/// model type using the <see cref="AssociateViewAttribute"/> attribute.
		/// </remarks>
		/// <typeparam name="TModel">The type of the model object.</typeparam>
		/// <returns>True if a matching, associated view extension is available; False otherwise.</returns>
		/// <exception cref="ArgumentException">Thrown if the type <typeparamref name="TModel"/> does not have an associated view extension point (doesn't define <see cref="AssociateViewAttribute"/>).</exception>
		/// <exception cref="InvalidOperationException">Thrown if the main application view has not been created yet.</exception>
		public static bool IsAssociatedViewAvailable<TModel>()
		{
			return IsAssociatedViewAvailable(typeof (TModel));
		}

		private static IExtensionPoint GetViewExtensionPoint(Type modelType)
		{
			object[] attrs = modelType.GetCustomAttributes(typeof (AssociateViewAttribute), true);
			if (attrs.Length == 0)
				throw new ArgumentException(SR.ExceptionAssociateViewAttributeNotSpecified, "modelType");

			AssociateViewAttribute viewAttribute = (AssociateViewAttribute) attrs[0];
			return (IExtensionPoint) Activator.CreateInstance(viewAttribute.ViewExtensionPointType);
		}

		/// <summary>
		/// Creates a view for the view extension point associated with specified model type and current GUI toolkit.
		/// </summary>
		/// <remarks>
		/// The view is created based on the view extension point that is associated with the specified
		/// model type using the <see cref="AssociateViewAttribute"/> attribute.
		/// </remarks>
		/// <param name="modelType">The type of the model object.</param>
		/// <returns>A new instance of a view extension for the specified model type.</returns>
		/// <exception cref="ArgumentException">Thrown if the type <paramref name="modelType"/> does not have an associated view extension point (doesn't define <see cref="AssociateViewAttribute"/>).</exception>
		/// <exception cref="NotSupportedException">Thrown if a view extension for the specified model matching the specified GUI toolkit does not exist.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the main application view has not been created yet.</exception>
		public static IView CreateAssociatedView(Type modelType)
		{
			var viewExtPoint = GetViewExtensionPoint(modelType);
			return CreateView(viewExtPoint);
		}

		/// <summary>
		/// Creates a view for the view extension point associated with specified model type and current GUI toolkit.
		/// </summary>
		/// <remarks>
		/// The view is created based on the view extension point that is associated with the specified
		/// model type using the <see cref="AssociateViewAttribute"/> attribute.
		/// </remarks>
		/// <typeparam name="TModel">The type of the model object.</typeparam>
		/// <returns>A new instance of a view extension for the specified model type.</returns>
		/// <exception cref="ArgumentException">Thrown if the type <typeparamref name="TModel"/> does not have an associated view extension point (doesn't define <see cref="AssociateViewAttribute"/>).</exception>
		/// <exception cref="NotSupportedException">Thrown if a view extension for the specified model matching the specified GUI toolkit does not exist.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the main application view has not been created yet.</exception>
		public static IView CreateAssociatedView<TModel>()
		{
			return CreateAssociatedView(typeof (TModel));
		}

		/// <summary>
		/// Creates and associates an <see cref="IApplicationComponentView"/> view for the <see cref="IApplicationComponent"/> for the current GUI toolkit.
		/// </summary>
		/// <remarks>
		/// The view is created based on the view extension point that is associated with the specified
		/// application component class using the <see cref="AssociateViewAttribute"/> attribute.
		/// </remarks>
		/// <param name="component">The <see cref="IApplicationComponent"/> for which a view is to be created and associated.</param>
		/// <returns>A new instance of the view for the specified application component.</returns>
		/// <exception cref="ArgumentException">Thrown if the application component type of <paramref name="component"/> does not have an associated view extension point (doesn't define <see cref="AssociateViewAttribute"/>).</exception>
		/// <exception cref="NotSupportedException">Thrown if a view extension for the specified model matching the specified GUI toolkit does not exist.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the main application view has not been created yet.</exception>
		public static IApplicationComponentView CreateView(this IApplicationComponent component)
		{
			Platform.CheckForNullReference(component, "component");
			var applicationComponentView = (IApplicationComponentView) CreateAssociatedView(component.GetType());
			applicationComponentView.SetComponent(component);
			return applicationComponentView;
		}
	}
}