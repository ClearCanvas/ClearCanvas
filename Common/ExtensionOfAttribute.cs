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

namespace ClearCanvas.Common
{
	/// <summary>
	/// Attribute used to mark a class as being an extension of the specified extension point class.
	/// </summary>
	/// <remarks>
	/// Use this attribute to mark a class as being an extension of the specified extension point,
	/// specified by the <see cref="Type" /> of the extension point class.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ExtensionOfAttribute : Attribute
	{
		private readonly Type _extensionPointClass;

		/// <summary>
		/// Attribute constructor.
		/// </summary>
		/// <param name="extensionPointClass">The type of the extension point class which the target class extends.</param>
		public ExtensionOfAttribute(Type extensionPointClass)
		{
			Enabled = true;
			_extensionPointClass = extensionPointClass;
		}

		/// <summary>
		/// The class that defines the extension point which this extension extends.
		/// </summary>
		public Type ExtensionPointClass
		{
			get { return _extensionPointClass; }
		}

		/// <summary>
		/// A friendly name for the extension.
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// A friendly description for the extension.
		/// </summary>
		/// <remarks>
		/// This is optional and may be supplied as a named parameter.
		/// </remarks>
		public string Description { get; set; }

		/// <summary>
		/// The default enablement of the extension.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Feature identification token to be checked against application licensing.
		/// </summary>
		public string FeatureToken { get; set; }
	}
}