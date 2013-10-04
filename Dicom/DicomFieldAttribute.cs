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

namespace ClearCanvas.Dicom
{
    public enum DicomFieldDefault
    {
        None,
        Null,
        Default,
        MinValue,
        MaxValue,
        DateTimeNow,
        StringEmpty,
        DBNull,
        EmptyArray
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DicomFieldAttribute : Attribute
    {
        private DicomTag _tag;
    	private DicomTag _parentTag;
        private DicomFieldDefault _default;
        private bool _defltOnZL;
        private bool _createEmpty;
		private bool _setNullValueIfEmpty;

        public DicomFieldAttribute(uint tag)
        {
            _tag = DicomTagDictionary.GetDicomTag(tag);
            if (_tag == null)
                _tag = new DicomTag(tag, "Unknown Tag", "UnknownTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);

            _default = DicomFieldDefault.None;
            _defltOnZL = false;
            _createEmpty = false;
        }

		[Obsolete("The nested dataset feature that this constructor would otherwise suggest isn't actually implemented.")]
		public DicomFieldAttribute(uint tag, uint parentTag)
			: this(tag)
		{
			_parentTag = DicomTagDictionary.GetDicomTag(parentTag);
			if (_parentTag == null)
				_parentTag = new DicomTag(parentTag, "Unknown Tag", "UnknownTag", DicomVr.UNvr, false, 1, uint.MaxValue, false);
		}

        public DicomTag Tag
        {
            get { return _tag; }
        }

		// TODO (CR Mar 2012): This is unused, which means it doesn't work.
		[Obsolete("The nested dataset feature that this property would otherwise suggest isn't actually implemented.")]
    	public DicomTag ParentTag
    	{
    		get { return _parentTag; }
    	}

        public DicomFieldDefault DefaultValue
        {
            get { return _default; }
            set { _default = value; }
        }

        public bool UseDefaultForZeroLength
        {
            get { return _defltOnZL; }
            set { _defltOnZL = value; }
        }

        public bool CreateEmptyElement
        {
            get { return _createEmpty; }
            set { _createEmpty = value; }
        }
		
    	public bool SetNullValueIfEmpty
    	{
			get { return _setNullValueIfEmpty; }
			set { _setNullValueIfEmpty = value; }
    	}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DicomClassAttribute : Attribute
    {
        private bool _defltOnZL;
        private bool _createEmpty;

        public DicomClassAttribute()
        {
        }

        public bool UseDefaultForZeroLength
        {
            get { return _defltOnZL; }
            set { _defltOnZL = value; }
        }

        public bool CreateEmptyElement
        {
            get { return _createEmpty; }
            set { _createEmpty = value; }
        }
    }
}
