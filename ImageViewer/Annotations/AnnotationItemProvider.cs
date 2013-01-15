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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Base implementation of <see cref="IAnnotationItemProvider"/>.
	/// </summary>
	/// <seealso cref="IAnnotationItemProvider"/>
	public abstract class AnnotationItemProvider : IAnnotationItemProvider
	{
		private readonly string _identifier;
		private readonly string _displayName;

		/// <summary>
		/// A constructor that uses the <see cref="AnnotationItemProvider"/>'s unique identifier to determine
		/// the display name using an <see cref="IAnnotationResourceResolver"/>.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItemProvider"/>.</param>
		/// <param name="resolver">The object that will resolve the display name from 
		/// the <see cref="AnnotationItemProvider"/>'s unique identifier.</param>
		protected AnnotationItemProvider(string identifier, IAnnotationResourceResolver resolver)
			: this(identifier, resolver.ResolveDisplayName(identifier))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="identifier">The unique identifier of the <see cref="AnnotationItemProvider"/>.</param>
		/// <param name="displayName">The <see cref="AnnotationItemProvider"/>'s display name.</param>
		protected AnnotationItemProvider(string identifier, string displayName)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(displayName, "displayName");

			_identifier = identifier;
			_displayName = displayName;
		}

		#region IAnnotationItemProvider Members

		/// <summary>
		/// Gets a unique identifier.
		/// </summary>
		public string GetIdentifier()
		{
			return _identifier;
		}

		/// <summary>
		/// Gets a user friendly display name.
		/// </summary>
		public string GetDisplayName()
		{
			return _displayName;
		}

		/// <summary>
		/// Gets the logical group of <see cref="IAnnotationItem"/>s.
		/// </summary>
		public abstract IEnumerable<IAnnotationItem> GetAnnotationItems();

		#endregion
	}
}
