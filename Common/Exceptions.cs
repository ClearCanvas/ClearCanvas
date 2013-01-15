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
using System.Runtime.Serialization;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Used by the framework to relay errors regarding plugins.
	/// </summary>
	/// <seealso cref="PluginManager"/>
    [SerializableAttribute]
	public class PluginException : ApplicationException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(string message, Exception inner) : base(message, inner) { }
	}

	/// <summary>
	/// Used by the framework to relay errors regarding <see cref="IExtensionPoint"/>s.
	/// </summary>
	/// <seealso cref="PluginInfo"/>
    public class ExtensionPointException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionPointException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionPointException(string message, Exception inner) : base(message, inner) { }
    }

	/// <summary>
	/// Used by the framework to relay errors regarding extensions (created via <see cref="IExtensionPoint"/>s).
	/// </summary>
    public class ExtensionException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionException(string message, Exception inner) : base(message, inner) { }
    }
}
