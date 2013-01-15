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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    [TypeConverter(typeof(DicomTagPathConverter))]
    public class DicomTagPath
    {
        private DicomTag _tag;
        private List<DicomTag> _parents;

        public DicomTag Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public List<DicomTag> Parents
        {
            get { return _parents; }
            set { _parents = value; }
        }

        public string HexString()
        {
            StringBuilder tagPath = new StringBuilder();
            tagPath.Append(StringUtilities.Combine(Parents, ",",
                                                             delegate(DicomTag tag) { return tag.HexString; }));
            if (tagPath.Length > 0)
                tagPath.Append(",");

            tagPath.Append(Tag.HexString);

            return tagPath.ToString();
        }

        public override string ToString()
        {
            return HexString();
        }
    }


    public class DicomTagPathConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string).IsAssignableFrom(sourceType))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                DicomTagPath tagPath = new DicomTagPath();
                List<DicomTag> parents;
                DicomTag tag;
                Parse((string)value, out parents, out tag);
                tagPath.Parents = parents;
                tagPath.Tag = tag;
                return tagPath;
            }
            return base.ConvertFrom(context, culture, value);
        }

        protected static void Parse(string tagPath, out List<DicomTag> parentTags, out DicomTag tag)
        {
            Platform.CheckForNullReference(tagPath, "tagPath");

            parentTags = null;
            tag = null;

            string[] tagPathComponents = tagPath.Split(',');
            if (tagPathComponents != null)
            {
                uint tagValue;
                if (tagPathComponents.Length > 1)
                {
                    parentTags = new List<DicomTag>();

                    for (int i = 0; i < tagPathComponents.Length - 1; i++)
                    {
                        tagValue = uint.Parse(tagPathComponents[i], NumberStyles.HexNumber);
                        DicomTag parent = DicomTagDictionary.GetDicomTag(tagValue);
                        if (parent == null)
                            throw new Exception(String.Format("Specified tag {0} is not in the dictionary", parent));
                        parentTags.Add(parent);
                    }
                }

                tagValue = uint.Parse(tagPathComponents[tagPathComponents.Length - 1], NumberStyles.HexNumber);
                tag = DicomTagDictionary.GetDicomTag(tagValue);
                if (tag == null)
                    throw new Exception(String.Format("Specified tag {0} is not in the dictionary", tag));

            }
        }
    }
}