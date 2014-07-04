#region License

// Copyright (c) 2014, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ClearCanvas.Dicom.Utilities.Xml
{
	public class StudyXmlMemento
	{
		internal class StudyXmlNode
		{
			private class Utf8StringWriter : StringWriter
			{
				public override Encoding Encoding
				{
					get { return Encoding.UTF8; }
				}
			}

			public StudyXmlNode(string node)
			{
				XmlElementFragment = node;
			}

			public StudyXmlNode(XmlNode node)
			{
				using (TextWriter writer = new Utf8StringWriter())
				{
					using (XmlWriter xmlWriter = XmlWriter.Create(writer,new XmlWriterSettings(){ConformanceLevel = ConformanceLevel.Fragment}))
					{
						node.WriteTo(xmlWriter);
					}
					XmlElementFragment = writer.ToString();
				}
			}

			public StudyXmlNode()
			{
			}

			public void AddAttribute(string key, string value)
			{
				if (AttributeList == null)
					AttributeList = new List<KeyValuePair<string, string>>();
				AttributeList.Add(new KeyValuePair<string, string>(key, value));
			}

			public void AddChild(StudyXmlNode child)
			{
				if (ChildList == null)
					ChildList = new List<StudyXmlNode>();
				ChildList.Add(child);
			}

			public List<StudyXmlNode> ChildList { get; set; }

			public string XmlElementFragment { get; set; }

			public string ElementName { get; set; }

			public List<KeyValuePair<string, string>> AttributeList { get; set; }

			public void WriteTo(StreamWriter sw)
			{
				if (!string.IsNullOrEmpty(XmlElementFragment))
				{
					sw.Write(XmlElementFragment);
					return;
				}

				var sb = new StringBuilder();
				sb.AppendFormat("<{0}", ElementName);
				if (AttributeList != null)
				{
					foreach (var attrib in AttributeList)
						sb.AppendFormat(" {0}=\"{1}\"", attrib.Key, attrib.Value);
				}
				if (ChildList != null && ChildList.Count > 0)
				{
					sb.Append(">");
					sw.Write(sb.ToString());
					foreach (var child in ChildList)
						child.WriteTo(sw);

					sw.Write("</{0}>", ElementName);
				}
				else
				{
					sb.Append("/>");
					sw.Write(sb.ToString());
				}

			}
		}

		internal StudyXmlNode RootNode { get; set; }

		public XmlDocument Document { get; set; }

		public void Save(TextWriter writer)
		{
			Document.Save(writer);
		}

		public void LoadXml(string cachedXml)
		{
			if (Document == null)
				Document = new XmlDocument();

			Document.LoadXml(cachedXml);
		}
	}
}
