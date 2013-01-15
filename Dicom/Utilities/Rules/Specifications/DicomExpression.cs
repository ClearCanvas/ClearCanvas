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
using System.Globalization;
using ClearCanvas.Common;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Dicom.Utilities.Rules.Specifications
{
    public class NoSuchDicomTagException : Exception
    {
        private readonly string _tagName;

        public NoSuchDicomTagException(string tagName)
        {
            _tagName = tagName;
        }

        public string TagName
        {
            get { return _tagName; }
        }
    }
    /// <summary>
    /// Expression factory for evaluating expressions that reference attributes within a <see cref="DicomMessageBase"/>.
    /// </summary>
    [ExtensionOf(typeof (ExpressionFactoryExtensionPoint))]
    [LanguageSupport("dicom")]
    public class DicomExpressionFactory : IExpressionFactory
    {
        #region IExpressionFactory Members

        public Expression CreateExpression(string text)
        {
            if (text.StartsWith("$"))
            {
                DicomTag tag = DicomTagDictionary.GetDicomTag(text.Substring(1));
                if (tag == null)
                    throw new XmlSpecificationCompilerException("Invalid DICOM tag: " + text);
            }
            return new DicomExpression(text);
        }

        #endregion
    }

    /// <summary>
    /// An expression handler for <see cref="DicomAttributeCollection"/> or 
    /// <see cref="DicomMessageBase"/> classes.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// This expression filter will evaluate input text with a leading $ as the name of a DICOM Tag.
    /// It will lookup the name of the tag, and retrieve the value of the tag and return it.  if there is no
    /// leading $, the value will be passed through.
    /// </para>
    /// </remarks>
    public class DicomExpression : Expression
    {
        public DicomExpression(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Evaluate 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override object Evaluate(object arg)
        {
            if (string.IsNullOrEmpty(Text))
                return null;

            if (Text.StartsWith("$"))
            {
                var msg = arg as DicomMessageBase;
                var collection = arg as DicomAttributeCollection;
                if (collection == null && msg == null)
                    return null;

                DicomTag tag = DicomTagDictionary.GetDicomTag(Text.Substring(1));
                if (tag == null)
                {
                    throw new NoSuchDicomTagException(Text.Substring(1));
                }
                    

                if (msg != null)
                {
                    if (msg.DataSet.Contains(tag))
                        collection = msg.DataSet;
                    else if (msg.MetaInfo.Contains(tag))
                        collection = msg.MetaInfo;
                }
				if (collection == null)
					return null;

            	DicomAttribute attrib;
				if (collection.TryGetAttribute(tag,out attrib))
				{
					if (attrib.IsEmpty || attrib.IsNull)
						return null;

					if (attrib.Tag.VR.Equals(DicomVr.SLvr))
						return attrib.GetInt32(0, 0);
				    if (attrib.Tag.VR.Equals(DicomVr.SSvr))
				        return attrib.GetInt16(0, 0);
				    if (attrib.Tag.VR.Equals(DicomVr.ATvr) || attrib.Tag.VR.Equals(DicomVr.ULvr))
				        return attrib.GetUInt32(0, 0);
				    if (attrib.Tag.VR.Equals(DicomVr.DSvr) || attrib.Tag.VR.Equals(DicomVr.FDvr))
				        return attrib.GetFloat64(0, 0f);
				    if (attrib.Tag.VR.Equals(DicomVr.FLvr))
				        return attrib.GetFloat32(0, 0f);
				    if (attrib.Tag.VR.Equals(DicomVr.ISvr))
				        return attrib.GetInt64(0, 0);
				    if (attrib.Tag.VR.Equals(DicomVr.USvr))
				        return attrib.GetUInt16(0, 0);
				    if (attrib.Tag.VR.Equals(DicomVr.SLvr))
				        return attrib.GetInt32(0, 0);
				    if (attrib.Tag.VR.Equals(DicomVr.OBvr)
				        || attrib.Tag.VR.Equals(DicomVr.OWvr)
				        || attrib.Tag.VR.Equals(DicomVr.OFvr))
				        return attrib.StreamLength.ToString(CultureInfo.InvariantCulture);
				    return attrib.ToString().Trim();
				}

            	return null;
            }

            return Text;
        }
    }
}