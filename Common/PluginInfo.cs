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
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Describes a plugin, and provides properties for querying the extension points and extensions defined
	/// in the plugin.
	/// </summary>
	/// <remarks>
	/// Instances of this class are immutable and safe for concurrent access by multiple threads.
	/// </remarks>
	[Serializable]
	public sealed class PluginInfo : IBrowsable
	{
		/// <summary>
		/// Internal method used by the framework to discover extension points and extensions declared in a plugin.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="points"></param>
		/// <param name="extensions"></param>
		internal static void DiscoverExtensionPointsAndExtensions(Assembly asm, List<ExtensionPointInfo> points, List<ExtensionInfo> extensions)
		{
			foreach (var type in asm.GetTypes())
			{
				var epAttr = AttributeUtils.GetAttribute<ExtensionPointAttribute>(type, false);
				if (epAttr != null)
				{
					if (IsValidExtensionPointClass(type))
					{
						points.Add(new ExtensionPointInfo(type, GetExtensionInterface(type), epAttr.Name, epAttr.Description));
					}
					else
					{
						Platform.Log(LogLevel.Error, SR.ExceptionExtensionPointMustSubclassExtensionPoint, type.FullName);
					}
				}

				var attrs = AttributeUtils.GetAttributes<ExtensionOfAttribute>(type, false);
				foreach (var a in attrs)
				{
					// is the extension a concrete class?
					if (!IsConcreteClass(type))
					{
						Platform.Log(LogLevel.Error, SR.ExceptionExtensionMustBeConcreteClass, type.FullName);
						continue;
					}

					var extensionPointClass = a.ExtensionPointClass;
					var extensionInterface = GetExtensionInterface(extensionPointClass);

					// does the extension implement the required interface?
					if (!extensionInterface.IsAssignableFrom(type))
					{
						Platform.Log(LogLevel.Error, SR.ExceptionExtensionDoesNotImplementRequiredInterface,
							type.FullName,
							extensionInterface);

						continue;
					}
					extensions.Add(
						new ExtensionInfo(
							type,
							extensionPointClass,
							a.Name,
							a.Description,
							ExtensionSettings.Default.IsEnabled(type, a.Enabled),
							a.FeatureToken
						)
					);
				}
			}
		}

		private static Type GetExtensionInterface(Type extensionPointClass)
		{
			if (!IsValidExtensionPointClass(extensionPointClass))
				throw new ArgumentException("Specified type does not appear to be a valid extension point class.");

			return extensionPointClass.BaseType.GetGenericArguments()[0];
		}

		private static bool IsValidExtensionPointClass(Type extensionPointClass)
		{
			var baseType = extensionPointClass.BaseType;
			return baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(ExtensionPoint<>);
		}

		private static bool IsConcreteClass(Type type)
		{
			return !type.IsAbstract && type.IsClass;
		}


		private readonly string _name;
		private readonly string _description;
		private readonly string _icon;
		private readonly AssemblyRef _assembly;

		private readonly List<ExtensionPointInfo> _extensionPoints;
		private readonly List<ExtensionInfo> _extensions;

		/// <summary>
		/// Internal constructor.
		/// </summary>
		internal PluginInfo(AssemblyRef assembly, string name, string description, string icon)
			: this(assembly, name, description, icon, new List<ExtensionPointInfo>(), new List<ExtensionInfo>())
		{
			DiscoverExtensionPointsAndExtensions(assembly.Resolve(), _extensionPoints, _extensions);
		}

		/// <summary>
		/// Internal constructor.
		/// </summary>
		internal PluginInfo(AssemblyRef assembly, string name, string description, string icon, List<ExtensionPointInfo> extensionPoints, List<ExtensionInfo> extensions)
		{
			_name = name;
			_description = description;
			_assembly = assembly;
			_icon = icon;
			_extensionPoints = extensionPoints;
			_extensions = extensions;
		}

		/// <summary>
		/// Gets the set of extensions defined in this plugin, including disabled and unlicensed extensions.
		/// </summary>
		public IList<ExtensionInfo> Extensions
		{
			get { return _extensions.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the set of extension points defined in this plugin.
		/// </summary>
		public IList<ExtensionPointInfo> ExtensionPoints
		{
			get { return _extensionPoints.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the assembly that implements this plugin.
		/// </summary>
		public AssemblyRef Assembly
		{
			get { return _assembly; }
		}

		/// <summary>
		/// Gets the name of an icon resource to associate with the plugin.
		/// </summary>
		public string Icon
		{
			get { return _icon; }
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", _name ?? _assembly.Name, _assembly.Resolve().Location);
		}

		#region IBrowsable Members

		/// <summary>
		/// Formal name of this object, typically the type name or assembly name.  Cannot be null.
		/// </summary>
		public string FormalName
		{
			get { return Assembly.Resolve().FullName; }
		}

		/// <summary>
		/// Friendly name of the object, if one exists, otherwise null.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// A friendly description of this object, if one exists, otherwise null.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		#endregion
	}
}
