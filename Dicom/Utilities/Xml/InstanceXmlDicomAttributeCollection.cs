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

namespace ClearCanvas.Dicom.Utilities.Xml
{
	//TODO: this is not ideal, but is the most straightforward given the current study xml design.  Later,
	//we should refactor for a cleaner API.
	public interface IInstanceXmlDicomAttributeCollection : IDicomAttributeProvider, IEnumerable<DicomAttribute>
	{
		IList<DicomTag> ExcludedTags { get; }

		bool IsTagExcluded(uint tag);
		bool HasExcludedTags(bool recursive);
	}

	internal interface IPrivateInstanceXmlDicomAttributeCollection : IInstanceXmlDicomAttributeCollection
	{
		ExcludedTagsHelper ExcludedTagsHelper { get; }
	}

	internal class ExcludedTagsHelper : IEquatable<ExcludedTagsHelper>
	{
		private readonly IInstanceXmlDicomAttributeCollection _parent;
		private readonly SortedList<uint, DicomTag> _excludedTags;

		public ExcludedTagsHelper(IInstanceXmlDicomAttributeCollection parent)
		{
			_parent = parent;
            _excludedTags = new SortedList<uint, DicomTag>();
		}

		public IList<DicomTag> ExcludedTags
		{
			get
			{
				Cleanup();
				return _excludedTags.Values;
			}
		}

		internal void Cleanup()
		{
		    if (_excludedTags.Count == 0)
		        return;

            //Set the capacity right away so adding is faster.
			var tagsToRemove = new List<uint>(_excludedTags.Count);
			foreach (var tag in _excludedTags.Keys)
			{
				DicomAttribute attribute;
				if (_parent.TryGetAttribute(tag, out attribute))
					tagsToRemove.Add(tag);
			}

			foreach (var tag in tagsToRemove)
                _excludedTags.Remove(tag);
		}

		public void Remove(DicomTag tag)
		{
			_excludedTags.Remove(tag.TagValue);
		}

		public void Add(DicomTag tag)
		{
			_excludedTags[tag.TagValue] = tag;
		}

		public void Add(IEnumerable<DicomTag> tags)
		{
			foreach (DicomTag tag in tags)
				Add(tag);
		}

		public bool IsTagExcluded(uint tag)
		{
		    if (_excludedTags.Count == 0)
		        return false;

		    if (_excludedTags.Count == 1)
		        return ExcludedTags[0].TagValue == tag;

			return ExcludedTags.Any(dicomTag => dicomTag.TagValue == tag);
		}

		public bool HasExcludedTags(bool recursive)
		{
			if (ExcludedTags.Count > 0)
				return true;

		    if (!recursive) return false;

		    foreach (DicomAttribute attribute in _parent)
		    {
		        if (attribute.Tag.VR != DicomVr.SQvr) continue;

		        var items = attribute.Values as DicomSequenceItem[];
		        if (items != null && items.OfType<InstanceXmlDicomSequenceItem>().Any(item => item.HasExcludedTags(true)))
		            return true;
		    }

		    return false;
		}

		#region IEquatable<ExcludedTagsHelper> Members

		public bool Equals(ExcludedTagsHelper other)
		{
			if (other == null)
				return false;

			if (other.ExcludedTags.Count != ExcludedTags.Count)
				return false;

		    return ExcludedTags.All(tag => other.ExcludedTags.Contains(tag));
		}

		#endregion

		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object obj)
		{
		    var other = obj as ExcludedTagsHelper;
		    return other != null && Equals(other);
		}
	}

	public class InstanceXmlDicomAttributeCollection : DicomAttributeCollection, IPrivateInstanceXmlDicomAttributeCollection
	{
		private readonly ExcludedTagsHelper _excludedTagsHelper;

		internal InstanceXmlDicomAttributeCollection(DicomAttributeCollection source, bool copyBinary, bool copyPrivate, bool copyUnknown, uint stopTag)
			: base(source, copyBinary, copyPrivate, copyUnknown, stopTag)
		{
			_excludedTagsHelper = new ExcludedTagsHelper(this);
			if (source is IInstanceXmlDicomAttributeCollection)
				_excludedTagsHelper.Add(((IInstanceXmlDicomAttributeCollection) source).ExcludedTags);
		}

		internal InstanceXmlDicomAttributeCollection()
		{
			_excludedTagsHelper = new ExcludedTagsHelper(this);
		}

		public override DicomAttributeCollection Copy(bool copyBinary, bool copyPrivate, bool copyUnknown, uint stopTag)
		{
			return new InstanceXmlDicomAttributeCollection(this, copyBinary, copyPrivate, copyUnknown, stopTag);
		}

		#region IInstanceXmlDicomAttributeCollection Members

		public IList<DicomTag> ExcludedTags
		{
			get { return _excludedTagsHelper.ExcludedTags; }
		}

		public bool IsTagExcluded(uint tag)
		{
			return _excludedTagsHelper.IsTagExcluded(tag);
		}

		public bool HasExcludedTags(bool recursive)
		{
			return _excludedTagsHelper.HasExcludedTags(recursive);
		}

		#endregion

		#region IInternalInstanceXmlDicomAttributeCollection Members

		ExcludedTagsHelper IPrivateInstanceXmlDicomAttributeCollection.ExcludedTagsHelper
		{
			get { return _excludedTagsHelper; }
		}

		#endregion

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is InstanceXmlDicomAttributeCollection)
			{
				if (!_excludedTagsHelper.Equals(((InstanceXmlDicomAttributeCollection) obj)._excludedTagsHelper))
					return false;
			}

			return base.Equals(obj);
		}
	}

	public class InstanceXmlDicomSequenceItem : DicomSequenceItem, IPrivateInstanceXmlDicomAttributeCollection
	{
		private readonly ExcludedTagsHelper _excludedTagsHelper;

		internal InstanceXmlDicomSequenceItem(DicomSequenceItem source, bool copyBinary, bool copyPrivate, bool copyUnknown)
			: base(source, copyBinary, copyPrivate, copyUnknown)
		{
			_excludedTagsHelper = new ExcludedTagsHelper(this);
			if (source is IInstanceXmlDicomAttributeCollection)
				_excludedTagsHelper.Add(((IInstanceXmlDicomAttributeCollection) source).ExcludedTags);
		}

		internal InstanceXmlDicomSequenceItem()
		{
			_excludedTagsHelper = new ExcludedTagsHelper(this);
		}

		public override DicomAttributeCollection Copy(bool copyBinary, bool copyPrivate, bool copyUnknown)
		{
			return new InstanceXmlDicomSequenceItem(this, copyBinary, copyPrivate, copyUnknown);
		}

		#region IInstanceXmlDicomAttributeCollection Members

		public IList<DicomTag> ExcludedTags
		{
			get { return _excludedTagsHelper.ExcludedTags; }
		}

		public bool IsTagExcluded(uint tag)
		{
			return _excludedTagsHelper.IsTagExcluded(tag);
		}

		public bool HasExcludedTags(bool recursive)
		{
			return _excludedTagsHelper.HasExcludedTags(recursive);
		}

		#endregion

		#region IInternalInstanceXmlDicomAttributeCollection Members

		ExcludedTagsHelper IPrivateInstanceXmlDicomAttributeCollection.ExcludedTagsHelper
		{
			get { return _excludedTagsHelper; }
		}

		#endregion

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is InstanceXmlDicomSequenceItem)
			{
				if (!_excludedTagsHelper.Equals(((InstanceXmlDicomSequenceItem) obj)._excludedTagsHelper))
					return false;
			}

			return base.Equals(obj);
		}
	}
}