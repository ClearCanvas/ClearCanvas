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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single segment of a <see cref="Path"/>.
    /// </summary>
    public class PathSegment : IEquatable<PathSegment>
    {
        private readonly string _key;
        private readonly string _localized;

		///<summary>
		/// Creates a <see cref="PathSegment"/> from the specified string, treated as a literal.
		///</summary>
		///<param name="p"></param>
		public PathSegment(string p)
			:this(p, (IResourceResolver)null)
    	{
    	}


		/// <summary>
		/// Creates a <see cref="PathSegment"/> from the specified string, which may be either a resource key or a literal.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="resolver"></param>
    	public PathSegment(string p, IResourceResolver resolver)
			:this(p, resolver != null ? resolver.LocalizeString(p) : p)
    	{
    	}

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="key">The resource key or unlocalized path segment string.</param>
        /// <param name="localized">The localized path segment string.</param>
        internal PathSegment(string key, string localized)
        {
			// key must be non null and non empty
        	Platform.CheckForNullReference(key, "key");
        	Platform.CheckForEmptyString(key, "key");

            _key = key;
            _localized = localized;
        }

        /// <summary>
        /// Gets the resource key or unlocalized text.
        /// </summary>
        public string ResourceKey
        {
            get { return _key; }
        }

        /// <summary>
		/// Gets the localized text.
        /// </summary>
        public string LocalizedText
        {
            get { return _localized; }
        }


    	///<summary>
    	///</summary>
    	///<param name="pathSegment1"></param>
    	///<param name="pathSegment2"></param>
    	///<returns></returns>
    	public static bool operator !=(PathSegment pathSegment1, PathSegment pathSegment2)
    	{
    		return !Equals(pathSegment1, pathSegment2);
    	}

    	///<summary>
    	///</summary>
    	///<param name="pathSegment1"></param>
    	///<param name="pathSegment2"></param>
    	///<returns></returns>
    	public static bool operator ==(PathSegment pathSegment1, PathSegment pathSegment2)
    	{
    		return Equals(pathSegment1, pathSegment2);
    	}

    	/// <summary>
    	/// Gets whether or not <paramref name="pathSegment"/> is equal to this object.
    	/// </summary>
		public bool Equals(PathSegment pathSegment)
    	{
    		if (pathSegment == null) return false;
    		return Equals(_localized, pathSegment._localized);
    	}

		/// <summary>
		/// Gets whether or not <paramref name="obj"/> is equal to this object.
		/// </summary>
		public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj as PathSegment);
    	}

		/// <summary>
		/// Gets a hash code.
		/// </summary>
    	public override int GetHashCode()
    	{
    		return _localized != null ? _localized.GetHashCode() : 0;
    	}
    }
}
