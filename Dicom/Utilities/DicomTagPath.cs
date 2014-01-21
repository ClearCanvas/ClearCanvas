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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities
{
	public class DicomTagPath : IEquatable<DicomTagPath>, IEquatable<DicomTag>, IEquatable<string>, IEquatable<uint>
	{
        public static readonly DicomTagPath Nil = new DicomTagPath();

		private static readonly string _exceptionFormatInvalidTagPath = "The specified Dicom Tag Path is invalid: {0}.";

		private static readonly char[] _pathSeparators = new char[] { '\\' , '/'};
		private static readonly char[] _tagSeparator = new char[] { ',' };

		private readonly DicomTag[] _tags;
		private string _path;

		public DicomTagPath(string path)
			: this(GetTags(path, null))
		{
		}

        public DicomTagPath(string path, DicomVr vr)
            : this(GetTags(path, vr))
        {
        }

		public DicomTagPath(uint tag)
			: this(new []{ tag })
		{
		}

        public DicomTagPath(uint tag, DicomVr vr)
            : this(new[] { tag }, vr)
        {
        }
        
        public DicomTagPath(DicomTag tag)
			: this(new []{ tag })
		{
		}

		public DicomTagPath(params uint[] tags)
            : this(GetTags(tags, null))
		{
		}

        public DicomTagPath(IEnumerable<uint> tags)
            : this(GetTags(tags.ToArray(), null))
        {
        }

        public DicomTagPath(IEnumerable<uint> tags, DicomVr vr)
			: this(GetTags(tags.ToArray(), vr))
        {
        }

        public DicomTagPath(params DicomTag[] tags)
            : this((IEnumerable<DicomTag>)tags)
        {
        }

	    public DicomTagPath(IEnumerable<DicomTag> tags)
		{
            Platform.CheckForNullReference(tags, "tags");
	    	_tags = tags.ToArray();
            ValidatePath(_tags);
		}

        private DicomTagPath()
        {
            _tags = new DicomTag[0];
        }

		public string Path
		{
			get
			{
				//Compute the path on demand and cache it because it's expensive.
				return _path ?? (_path = StringUtilities.Combine(_tags, "/", tag => String.Format("({0:x4},{1:x4})", tag.Group, tag.Element)));
			}
		}

		public IList<DicomTag> TagsInPath
		{
			get { return _tags.ToList(); }
		}

		public DicomVr ValueRepresentation
		{
			get { return _tags[_tags.Length - 1].VR; }	
		}

        public DicomTagPath UpOne()
        {
            if (_tags.Length == 1)
                throw new InvalidOperationException();

			return new DicomTagPath(_tags.Take(_tags.Length - 1));
        }

        public DicomTagPath DownOne()
        {
			if (_tags.Length == 1)
                throw new InvalidOperationException();

			return new DicomTagPath(_tags.Skip(_tags.Length - 1));
        }

        /// <summary>
        /// Gets all the <see cref="DicomAttribute"/>s along a particular path.
        /// </summary>
        public IEnumerable<DicomAttribute> SelectAttributesFrom(DicomAttributeCollection collection)
        {
            return SelectAttributesFrom(collection, false);
        }

        public IEnumerable<DicomAttribute> SelectAttributesFrom(DicomAttributeCollection collection, bool createSequences)
        {
            var current = TagsInPath[0];
            var attribute = collection[current];
            
            if (TagsInPath.Count == 1)
            {
                yield return attribute;
            }
            else
            {
                var children = DownOne();
                //By definition, the parent is a sequence
                var sequenceAttribute = collection[current];

                var items = sequenceAttribute.Values as DicomSequenceItem[];
                if (items == null || items.Length == 0)
                {
                    if (createSequences)
                        sequenceAttribute.Values = items = new []{new DicomSequenceItem()};
                    else
                        yield break;
                }

                //TODO: allow paths to have item selectors? e.g. ViewCodeSequence[0]/CodeMeaning?
                var selectedItems = items;
                foreach (var item in selectedItems)
                {
                    foreach (var itemAttribute in children.SelectAttributesFrom(item, createSequences))
                        yield return itemAttribute;
                }
            }
        }

	    public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is DicomTagPath)
				return Equals(obj as DicomTagPath);
			if (obj is DicomTag)
				return Equals(obj as DicomTag);
			if (obj is string)
				return Equals(obj as string);
			if (obj is uint)
				return Equals((uint)obj);

			return false;
		}

		#region IEquatable<DicomTagPath> Members

		public bool Equals(DicomTagPath other)
		{
			if (other == null)
				return false;

			return other.Path.Equals(Path);
		}

		#endregion	

		#region IEquatable<DicomTag> Members

		public bool Equals(DicomTag other)
		{
			if (other == null)
				return false;

			if (_tags.Length != 1)
				return false;

			return _tags[0].Equals(other);
		}

		#endregion

		#region IEquatable<string> Members

		public bool Equals(string other)
		{
			return Path.Equals(other);
		}

		#endregion

		#region IEquatable<uint> Members

		public bool Equals(uint other)
		{
			if (_tags.Length != 1)
				return false;

			return _tags[0].TagValue.Equals(other);
		}

		#endregion

		public override int GetHashCode()
		{
			int hash = 0x8323421;
			foreach(var tag in _tags)
				hash ^= tag.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			return Path;
		}

		public static DicomTagPath operator +(DicomTagPath left, DicomTagPath right)
		{
			var tags = new List<DicomTag>(left.TagsInPath);
			tags.AddRange(right.TagsInPath);
			return new DicomTagPath(tags);
		}

		public static DicomTagPath operator +(DicomTagPath left, DicomTag right)
		{
            var tags = new List<DicomTag>(left.TagsInPath) {right};
		    return new DicomTagPath(tags);
		}

		public static DicomTagPath operator +(DicomTagPath left, uint right)
		{
            var tags = new List<DicomTag>(left.TagsInPath) {DicomTagDictionary.GetDicomTag(right)};
		    return new DicomTagPath(tags);
		}
		
		public static implicit operator DicomTagPath(DicomTag tag)
		{
			return new DicomTagPath(tag);
		}

		public static implicit operator DicomTagPath(uint tag)
		{
			return new DicomTagPath(tag);
		}

		/// <summary>
		/// Implicit cast to a String object, for ease of use.
		/// </summary>
		public static implicit operator string(DicomTagPath path)
		{
			return path.ToString();
		}

		private static IEnumerable<DicomTag> GetTags(string path, DicomVr vr)
		{
			Platform.CheckForEmptyString(path, "path");

            var dicomTags = new List<DicomTag>();

			string[] groupElementValues = path.Split(_pathSeparators);

            for (int i = 0; i < groupElementValues.Length; ++i)
            {
                var groupElement = groupElementValues[i];
                string[] values = groupElement.Split(_tagSeparator);
                if (values.Length != 2)
                    throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));

                string group = values[0];
                if (!group.StartsWith("(") || group.Length != 5)
                    throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));

                string element = values[1];
                if (!element.EndsWith(")") || element.Length != 5)
                    throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));

                try
                {
                    ushort groupValue = Convert.ToUInt16(group.TrimStart('('), 16);
                    ushort elementValue = Convert.ToUInt16(element.TrimEnd(')'), 16);

                    var theVr = i < groupElementValues.Length - 1 ? DicomVr.SQvr : vr;
                    dicomTags.Add(NewTag(DicomTag.GetTagValue(groupValue, elementValue), theVr));
                }
                catch
                {
                    throw new ArgumentException(String.Format(_exceptionFormatInvalidTagPath, path));
                }
            }

			return dicomTags;
		}

		private static void ValidatePath(IList<DicomTag> dicomTags)
		{
			for (int i = 0; i < dicomTags.Count - 1; ++i)
			{
				if (dicomTags[i].VR != DicomVr.SQvr)
					throw new ArgumentException("All but the last item in the path must have VR = SQ.");
			}
		}

        private static IEnumerable<DicomTag> GetTags(IList<uint> tags, DicomVr vr)
        {
            var dicomTags = new List<DicomTag>();
            for (int i = 0; i < tags.Count; ++i)
            {
				var tag = tags[i];
				var theVr = i < tags.Count - 1 ? DicomVr.SQvr : vr;
                dicomTags.Add(NewTag(tag, theVr));
            }

            return dicomTags;
        }

	    internal static DicomTag NewTag(uint tag, DicomVr vr)
		{
            var theTag = DicomTagDictionary.GetDicomTag(tag);
            if (theTag == null)
            {
                theTag = new DicomTag(tag, "Unknown tag", "UnknownTag", vr ?? DicomVr.UNvr, false, 1, uint.MaxValue, false);
            }
            else if (vr != null && !theTag.VR.Equals(vr))
            {
                theTag = new DicomTag(tag, theTag.Name, theTag.VariableName, vr, theTag.MultiVR, theTag.VMLow,
                                      theTag.VMHigh, theTag.Retired);
            }

            return theTag;
        }
	}
}
