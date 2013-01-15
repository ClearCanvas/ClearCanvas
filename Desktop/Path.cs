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

using System.Reflection;
using ClearCanvas.Common.Utilities;
using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a path.
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable.
    /// </remarks>
    public class Path : IEquatable<Path>
    {
		/// <summary>
		/// Gets the empty <see cref="Path"/> object.
		/// </summary>
		public static readonly Path Empty = new Path(new PathSegment[]{});


		private const string SEPARATOR = "/";
    	private const string ESCAPED_SEPARATOR = "'/";
		private const string TEMP = "__$:$__";

        private readonly List<PathSegment> _segments;

		#region Constructors

		/// <summary>
        /// Creates a new <see cref="Path"/> from the specified path string, resolving
        /// resource keys in the path string using the specified <see cref="ResourceResolver"/>.
        /// </summary>
        /// <remarks>
        /// <para>
		/// The path string is a string of the form A[/B[/C...]] where the forward slash delineates
		/// segments.  To include a slash character as part of a segment, escape it by preceding it
		/// with a single quote, e.g. '/.
		/// </para>
        /// <para>
		/// Each segment may be either a literal or a resource key.  Localization
		/// will be attempted on each path segment by treating the segment as a resource key,
		/// and path segments that fail as resource keys will be treated as literals.
		/// </para>
        /// </remarks>
        /// <param name="pathString">The path string to parse.</param>
        /// <param name="resolver">The <see cref="IResourceResolver"/> to use for localization.</param>
        public Path(string pathString, IResourceResolver resolver)
			:this(ParsePathString(pathString, resolver))
        {
        }

		/// <summary>
		/// Creates a new <see cref="Path"/> from the specified path string, with no resource resolver.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The path string is a string of the form A[/B[/C...]] where the forward slash delineates
		/// segments.  To include a slash character as part of a segment, escape it by preceding it
		/// with a single quote, e.g. '/.
		/// </para>
		/// <para>
		/// The path string must only contain literals, because there is no resource resolver to perform
		/// localization.
		/// </para>
		/// </remarks>
		/// <param name="pathString"></param>
		public Path(string pathString)
			:this(ParsePathString(pathString, null))
		{
		}

		/// <summary>
		/// Creates a new single segment <see cref="Path"/> from the specified segment.
		/// </summary>
		/// <param name="segment"></param>
		public Path(PathSegment segment)
			:this(segment == null ? null : new[]{ segment })
		{
		}

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="segments"></param>
        private Path(IEnumerable<PathSegment> segments)
        {
        	Platform.CheckForNullReference(segments, "segments");
            _segments = new List<PathSegment>(segments);
		}

		#endregion

		#region Public API

		/// <summary>
        /// Gets the individual segments contained in this path.
        /// </summary>
        public IList<PathSegment> Segments
        {
            get { return _segments.AsReadOnly(); }
        }

        /// <summary>
        /// The final segment in this path, or null if this path is empty.
        /// </summary>
        public PathSegment LastSegment
        {
            get { return CollectionUtils.LastElement(_segments); }
        }

		/// <summary>
		/// Gets a new <see cref="Path"/> object representing the specified sub-path.
		/// </summary>
		public Path SubPath(int start, int count)
		{
			return new Path(_segments.GetRange(start, count));
		}

		/// <summary>
		/// Gets the full path string, localized.
		/// </summary>
		public string LocalizedPath
		{
			get
			{
				return StringUtilities.Combine(_segments, SEPARATOR, s => Escape(s.LocalizedText), false);
			}
		}

		/// <summary>
		/// Returns a new <see cref="Path"/> object obtained by appending <paramref name="other"/> path
		/// to this path.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Path Append(Path other)
		{
			var combined = new List<PathSegment>(_segments);
			combined.AddRange(other.Segments);
			
			return new Path(combined);
		}

		///<summary>
		/// Returns a new <see cref="Path"/> object obtained by appending <paramref name="segment"/> to this path.
		///</summary>
		///<param name="segment"></param>
		///<returns></returns>
		public Path Append(PathSegment segment)
		{
			return new Path(new List<PathSegment>(_segments) {segment});
		}

        /// <summary>
        /// Converts this path back to the original string on which it was based.
        /// </summary>
        public override string ToString()
        {
			return StringUtilities.Combine(_segments, SEPARATOR, s => Escape(s.ResourceKey), false);
		}

		/// <summary>
		/// Returns a new <see cref="Path"/> object representing the longest common path
		/// between this object and <paramref name="other"/>.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Path GetCommonPath(Path other)
		{
			var commonPath = new List<PathSegment>();
			for(var i = 0; i < Math.Min(_segments.Count, other.Segments.Count); i++)
			{
				if(_segments[i] == other.Segments[i])
					commonPath.Add(_segments[i]);
				else
					break;	// must break as soon as paths differ
			}

			return new Path(commonPath);
		}

        /// <summary>
        /// Returns true if this path starts with <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool StartsWith(Path other)
        {
            // if other path is longer, then this path can't possibly "start with" it
            if (other.Segments.Count > _segments.Count)
                return false;

            // check that segments are equal up to length of other path
            for (var i = 0; i < other.Segments.Count; i++)
            {
                if (!Equals(_segments[i], other.Segments[i]))
                    return false;
            }
            return true;
        }

		/// <summary>
		/// Gets whether or not this instance is equal to <paramref name="other"/>.
		/// </summary>
    	public bool Equals(Path other)
    	{
    		if (ReferenceEquals(null, other)) return false;
    		if (ReferenceEquals(this, other)) return true;
    		return CollectionUtils.Equal<PathSegment>(other._segments, _segments, true);
    	}

		/// <summary>
		/// Gets whether or not this instance is equal to <paramref name="obj"/>.
		/// </summary>
		public override bool Equals(object obj)
    	{
    		return Equals(obj as Path);
    	}

    	/// <summary>
    	/// Gets a hash code.
    	/// </summary>
		public override int GetHashCode()
    	{
    		return _segments.GetHashCode();
    	}

		/// <summary>
		/// Determines whether <paramref name="left"/> is equivalent to <paramref name="right"/>.
		/// </summary>
    	public static bool operator ==(Path left, Path right)
    	{
    		return Equals(left, right);
    	}

		/// <summary>
		/// Determines whether <paramref name="left"/> is not equivalent to <paramref name="right"/>.
		/// </summary>
		public static bool operator !=(Path left, Path right)
    	{
    		return !Equals(left, right);
		}

		#endregion

		#region Helpers

		private static PathSegment[] ParsePathString(string pathString, IResourceResolver resolver)
		{
        	Platform.CheckForNullReference(pathString, "pathString");

			// degenerate case
			if(pathString == string.Empty)
				return new PathSegment[0];

			// replace any escaped separators with some weird temporary string
			pathString = pathString.Replace(ESCAPED_SEPARATOR, TEMP);

			// split string by separator, removing any empty segments
        	var parts = pathString.Split(new[] {SEPARATOR}, StringSplitOptions.RemoveEmptyEntries);

			// replace temp string with unescaped separator, and create segments
			return CreateSegments(
				CollectionUtils.Map(parts, (string p) => p.Replace(TEMP, SEPARATOR)),
				resolver);
		}

    	private static PathSegment[] CreateSegments(IList<string> parts, IResourceResolver resolver)
    	{
    		return CollectionUtils.Map(parts, (string p) => new PathSegment(p, resolver)).ToArray();
    	}

		private static string Escape(string s)
		{
			return s.Replace(SEPARATOR, ESCAPED_SEPARATOR);
		}

		#endregion
	}
}
