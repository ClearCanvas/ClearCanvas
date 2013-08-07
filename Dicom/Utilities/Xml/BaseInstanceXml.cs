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

using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Utilities.Xml
{
	/// <summary>
	/// Class for representing a base instance of a series as XML.
	/// </summary>
	public class BaseInstanceXml : InstanceXml
	{
		/// <summary>
		/// Creates an empty instance of <see cref="BaseInstanceXml"/>.
		/// </summary>
		public BaseInstanceXml()
			: base(new InstanceXmlDicomAttributeCollection(), null, TransferSyntax.ExplicitVrLittleEndian)
		{
		}

		/// <summary>
		/// Creates an instance of <see cref="BaseInstanceXml"/> from a specified Xml node.
		/// </summary>
		/// <param name="node"></param>
		public BaseInstanceXml(XmlNode node)
			: base(node, null)
		{
		}

		/// <summary>
		/// Creates an instance of <see cref="BaseInstanceXml"/> based on the specified <see cref="DicomAttributeCollection"/>.
		/// </summary>
		/// <param name="collect1"></param>
		/// <param name="collect2"></param>
		public BaseInstanceXml(DicomAttributeCollection collect1, DicomAttributeCollection collect2)
			: this()
		{
			Platform.CheckForNullReference(collect1, "collect1");
			Platform.CheckForNullReference(collect2, "collect2");

			foreach (DicomAttribute attrib1 in collect1)
			{
				DicomAttribute attrib2;
				if ((attrib1 is DicomAttributeOB)
					|| (attrib1 is DicomAttributeOW)
					|| (attrib1 is DicomAttributeOF)
					|| (attrib1 is DicomAttributeOD)
					|| (attrib1 is DicomFragmentSequence))
				{
					if (collect2.TryGetAttribute(attrib1.Tag, out attrib2))
						((IPrivateInstanceXmlDicomAttributeCollection)Collection).ExcludedTagsHelper.Add(attrib1.Tag);
					continue;
				}

				if (collect2.TryGetAttribute(attrib1.Tag, out attrib2))
				{
					if (!attrib1.IsEmpty && attrib1.Equals(attrib2)) //don't store empty tags in the base collection.
					{
						Collection[attrib1.Tag] = attrib1.Copy();
					}
				}
			}

			if (collect1 is IInstanceXmlDicomAttributeCollection && collect2 is IInstanceXmlDicomAttributeCollection)
			{
				IInstanceXmlDicomAttributeCollection collection2 = (IInstanceXmlDicomAttributeCollection) collect2;
				foreach (DicomTag tag in ((IInstanceXmlDicomAttributeCollection)collect1).ExcludedTags)
				{
					if (collection2.ExcludedTags.Contains(tag))
						PrivateCollection.ExcludedTagsHelper.Add(tag);
				}
			}
		}

		public new InstanceXmlDicomAttributeCollection Collection
		{
			get { return (InstanceXmlDicomAttributeCollection)base.Collection; }
		}

		internal IPrivateInstanceXmlDicomAttributeCollection PrivateCollection
		{
			get { return (IPrivateInstanceXmlDicomAttributeCollection)base.Collection; }
		}
	}
}