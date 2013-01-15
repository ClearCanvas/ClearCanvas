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
using ClearCanvas.Common.Utilities;

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
		private readonly SortedList<DicomTag, DicomTag> _excludedTags;

		public ExcludedTagsHelper(IInstanceXmlDicomAttributeCollection parent)
		{
			_parent = parent;
			_excludedTags = new SortedList<DicomTag, DicomTag>();
		}

		public IList<DicomTag> ExcludedTags
		{
			get
			{
				Cleanup();
				return _excludedTags.Keys;
			}
		}

		internal void Cleanup()
		{
			List<DicomTag> tagsToRemove = new List<DicomTag>();
			foreach (DicomTag tag in _excludedTags.Keys)
			{
				DicomAttribute attribute;
				if (_parent.TryGetAttribute(tag, out attribute) && !attribute.IsEmpty)
					tagsToRemove.Add(tag);
			}

			foreach (DicomTag tag in tagsToRemove)
				Remove(tag);
		}

		public void Remove(DicomTag tag)
		{
			DicomTag existingTag;
			if (_excludedTags.TryGetValue(tag, out existingTag))
				_excludedTags.Remove(existingTag);
		}

		public void Add(DicomTag tag)
		{
			DicomTag existingTag;
			if (!_excludedTags.TryGetValue(tag, out existingTag))
				_excludedTags.Add(tag, tag);
		}

		public void Add(IEnumerable<DicomTag> tagList)
		{
			foreach (DicomTag tag in tagList)
				Add(tag);
		}

		public bool IsTagExcluded(uint tag)
		{
			return CollectionUtils.Contains(ExcludedTags,
				delegate(DicomTag dicomTag) { return dicomTag.TagValue == tag; });
		}

		public bool HasExcludedTags(bool recursive)
		{
			if (ExcludedTags.Count > 0)
				return true;

			if (recursive)
			{
				foreach (DicomAttribute attribute in _parent)
				{
					if (attribute.Tag.VR == DicomVr.SQvr)
					{
						DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
						if (items != null)
						{
							foreach (DicomSequenceItem item in items)
							{
								if (item is InstanceXmlDicomSequenceItem)
								{
									if (((InstanceXmlDicomSequenceItem)item).HasExcludedTags(recursive))
										return true;
								}
							}
						}
					}
				}
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

			foreach (DicomTag tag in ExcludedTags)
			{
				if (!other.ExcludedTags.Contains(tag))
					return false;
			}

			return true;
		}

		#endregion

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is ExcludedTagsHelper)
				return Equals((ExcludedTagsHelper)obj);

			return false;
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
				_excludedTagsHelper.Add(((IInstanceXmlDicomAttributeCollection)source).ExcludedTags);
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
				if (!_excludedTagsHelper.Equals(((InstanceXmlDicomAttributeCollection)obj)._excludedTagsHelper))
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
				_excludedTagsHelper.Add(((IInstanceXmlDicomAttributeCollection)source).ExcludedTags);
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
				if (!_excludedTagsHelper.Equals(((InstanceXmlDicomSequenceItem)obj)._excludedTagsHelper))
					return false;
			}

			return base.Equals(obj);
		}
	}
}