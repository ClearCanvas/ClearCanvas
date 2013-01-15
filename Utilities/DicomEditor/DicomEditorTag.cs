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
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomEditorTag
    {
        public DicomEditorTag(DicomAttribute attribute)
            : this(attribute, null, 0) 
        {
        }        

        public DicomEditorTag(DicomAttribute attribute, DicomEditorTag parentTag, int nestingLevel)
        {
            _attribute = attribute;

            _group = _attribute.Tag.Group;
            _element = _attribute.Tag.Element;
            _tagName = _attribute.Tag.Name;            

            _parentTag = parentTag;
            _nestingLevel = nestingLevel;
            _postitionOrdinal = 0;
        }

        public DicomEditorTag(string group, string element, string tagName, DicomEditorTag parentTag, int positionOrdinal, int displayLevel)
        {
            _attribute = null;

            _group = ushort.Parse(group, System.Globalization.NumberStyles.HexNumber);
            _element = ushort.Parse(element, System.Globalization.NumberStyles.HexNumber);            
            _tagName = tagName;

            _parentTag = parentTag;
            _postitionOrdinal = positionOrdinal;
            _nestingLevel = displayLevel;            
        }

        public uint TagId
        {
            get { return _attribute == null ? 0 : _attribute.Tag.TagValue; } 
        }

        public ushort Group
        {
            get { return _group; }
        }

        public ushort Element
        {
            get { return _element; }
        }

        public string TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        public string Vr
        {
            get { return _attribute == null? String.Empty : _attribute.Tag.VR.Name; }
        }

        public string Length
        {
            get { return _attribute == null? String.Empty : _attribute.StreamLength.ToString(); }            
        }

        public string Value
        {
            get 
            {
                if (_attribute == null)
                    return String.Empty;
                else if (this.Vr == "OB" && _attribute.StreamLength > 22)
                {
                    return "<OB Data>";
                }
                else
                {
                    return _attribute.ToString();                        
                }
            }
            set 
            { 
                if (this.IsEditable())
                    _attribute.SetStringValue(value);                
            }
        }

    	public bool IsRootLevelTag
    	{
    		get { return _parentTag == null; }
    	}

        #region Display Utilities

        public string DisplayKey
        {
            get
            {
                StringBuilder display = new StringBuilder();

                for (int i = 0; i < _nestingLevel; i++)
                {
                    display.Append("      ");
                }                
                display.AppendFormat("({0:x4}, {1:x4})", _group, _element);                

                return display.ToString();
            }
        }

        public bool IsEditable()
        {
            return !_unEditableVRList.Contains(this.Vr) && !this.DisplayKey.Contains(",0000)") && !this.DisplayKey.Contains("(0002,");
        }

        public static int TagCompare(DicomEditorTag one, DicomEditorTag two, SortType type)
        {
            return one.SortKey(type).CompareTo(two.SortKey(type));
        }

        private string SortKey(SortType type)
        {
            DicomEditorTag parentTag = _parentTag;
            string typeSpecificModifier;

            switch (type)
            {
                case SortType.GroupElement:
                    if (_parentTag == null)
                    {
                        return String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element) + _postitionOrdinal.ToString();
                    }
                    else
                    {
                        return _parentTag.SortKey(type) + String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element) + _postitionOrdinal.ToString();
                    }
                case SortType.TagName:
                    typeSpecificModifier = _tagName;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag._tagName;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Vr:
                    typeSpecificModifier = this.Vr;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag.Vr;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Length:
                    typeSpecificModifier = this.Length;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag.Length;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Value:
                    typeSpecificModifier = this.Value;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag.Value;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                default:
                    typeSpecificModifier = String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element) + _postitionOrdinal.ToString();
                    break;
            }

            return typeSpecificModifier + this.SortKey(SortType.GroupElement);
        }

        #endregion

        private ushort _group;
        private ushort _element;
        private string _tagName;      
        private int _nestingLevel;
        private int _postitionOrdinal;
        private DicomEditorTag _parentTag;

        private DicomAttribute _attribute;
        private ICollection<string> _unEditableVRList = new string[] { "SQ", @"??", "OB", "OW", "UN", String.Empty };
    }

    public enum SortType
    {
        GroupElement,
        TagName,
        Vr,
        Length,
        Value
    }
}
