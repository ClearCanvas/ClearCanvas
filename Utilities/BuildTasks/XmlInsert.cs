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
using System.Xml;
using Microsoft.Build.Framework;

namespace ClearCanvas.Utilities.BuildTasks
{
	/// <summary>
	/// Enumerated values indicating positioning of a new node relative to a selected node.
	/// </summary>
	public enum XmlNodePosition
	{
		/// <summary>
		/// Inserts a new node by appending it to the end of the child nodes of the selected node.
		/// </summary>
		Append,

		/// <summary>
		/// Inserts a new node by prepending it to the start of the child nodes of the selected node.
		/// </summary>
		Prepend,

		/// <summary>
		/// Inserts a new node by appending it as a sibling after the selected node.
		/// </summary>
		After,

		/// <summary>
		/// Inserts a new node by prepending it as a sibling before the selected node.
		/// </summary>
		Before
	}

	/// <summary>
	/// Enumerated values indicating type of new node to be created.
	/// </summary>
	public enum XmlNodeType
	{
		/// <summary>
		/// Creates a new attribute node.
		/// </summary>
		Attribute,

		/// <summary>
		/// Creates a new element node.
		/// </summary>
		Element,

		/// <summary>
		/// Creates a new CDATA section.
		/// </summary>
		CData,

		/// <summary>
		/// Creates a new comment node.
		/// </summary>
		Comment,

		/// <summary>
		/// Creates a new significant whitespace node.
		/// </summary>
		SignificantWhitespace,

		/// <summary>
		/// Creates a new text node.
		/// </summary>
		Text,

		/// <summary>
		/// Creates a new whitespace node.
		/// </summary>
		Whitespace
	}

	/// <summary>
	/// A task for inserting nodes into an XML document.
	/// </summary>
	/// <remarks>
	/// This task creates new nodes of type <see cref="Type"/> and inserts them relative to the nodes selected by the <see cref="XmlTaskBase.XPath"/> expression.
	/// </remarks>
	public class XmlInsert : XmlUpdateTaskBase
	{
		/// <summary>
		/// The name of the new attribute or element nodes to be created. Ignored if <see cref="Type"/> is not &quot;Attribute&quot; or &quot;Element&quot;.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The XML namespace URI for the new attribute or element nodes to be created. Ignored if <see cref="Type"/> is not &quot;Attribute&quot; or &quot;Element&quot;.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// The type of the new nodes to be created. Must be one of the values of <see cref="XmlNodeType"/>.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// The positioning of the new nodes relative to the selected nodes. Must be one of the values of <see cref="XmlNodePosition"/>.
		/// </summary>
		public string Position { get; set; }

		/// <summary>
		/// Outputs whether or not nodes were inserted into the selected nodes.
		/// </summary>
		[Output]
		public bool Inserted { get; set; }

		protected override bool AllowEmptyDocument
		{
			get { return true; }
		}

		/// <summary>
		/// Parses the value of <see cref="Position"/>.
		/// </summary>
		protected XmlNodePosition GetNodePosition()
		{
			if (string.IsNullOrEmpty(Position))
				return XmlNodePosition.Append;

			foreach (XmlNodePosition eValue in Enum.GetValues(typeof (XmlNodePosition)))
			{
				if (string.Equals(Position, eValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
					return eValue;
			}

			throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		/// Parses the value of <see cref="Type"/>.
		/// </summary>
		protected XmlNodeType GetNodeType()
		{
			if (string.IsNullOrEmpty(Type))
				return XmlNodeType.Element;

			foreach (XmlNodeType eValue in Enum.GetValues(typeof (XmlNodeType)))
			{
				if (string.Equals(Type, eValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
					return eValue;
			}

			throw new ArgumentOutOfRangeException();
		}

		protected override bool ValidateParameters(out string message)
		{
			try
			{
				GetNodePosition();
			}
			catch (ArgumentOutOfRangeException)
			{
				message = "Position must be one of \"Append\", \"Prepend\", \"After\" or \"Before\"";
				return false;
			}

			XmlNodeType nodeType;
			try
			{
				nodeType = GetNodeType();
			}
			catch (ArgumentOutOfRangeException)
			{
				message = "Type must be one of \"Attribute\", \"Element\", \"CDATA\", \"Comment\", \"Text\", \"Whitespace\" or \"SignificantWhitespace\"";
				return false;
			}

			if (string.IsNullOrEmpty(Name) && (nodeType == XmlNodeType.Attribute || nodeType == XmlNodeType.Element))
			{
				message = "Name is a required parameter for Attribute and Element nodes.";
				return false;
			}

			return base.ValidateParameters(out message);
		}

		protected override bool PerformTask(XmlNodeList xmlNodes)
		{
			var success = Cross(xmlNodes, GetOperations(), InsertNode);
			if (!success)
			{
				Log.LogError("Inputs must have only 1 item or be in 1:1 correspondance with selected nodes");
				return false;
			}

			FlagModified(Inserted = xmlNodes.Count > 0);
			return true;
		}

		/// <summary>
		/// Creates, updates and inserts a node.
		/// </summary>
		private void InsertNode(XmlNode anchor, IUpdateOperation updateOperation)
		{
			// create node according to type
			XmlNode newXmlNode;
			switch (GetNodeType())
			{
				case XmlNodeType.Attribute:
					newXmlNode = XmlDocument.CreateAttribute(Name, Namespace ?? string.Empty);
					break;
				case XmlNodeType.Element:
					newXmlNode = XmlDocument.CreateElement(Name, Namespace ?? string.Empty);
					break;
				case XmlNodeType.CData:
					newXmlNode = XmlDocument.CreateCDataSection(string.Empty);
					break;
				case XmlNodeType.Comment:
					newXmlNode = XmlDocument.CreateComment(string.Empty);
					break;
				case XmlNodeType.SignificantWhitespace:
					newXmlNode = XmlDocument.CreateSignificantWhitespace(string.Empty);
					break;
				case XmlNodeType.Text:
					newXmlNode = XmlDocument.CreateTextNode(string.Empty);
					break;
				case XmlNodeType.Whitespace:
					newXmlNode = XmlDocument.CreateWhitespace(string.Empty);
					break;
				default:
					// should not happen
					throw new ArgumentOutOfRangeException();
			}

			// update node with input
			updateOperation.Update(newXmlNode);

			// insert node relative to anchor
			if (newXmlNode is XmlAttribute)
			{
				var newXmlAttribute = (XmlAttribute) newXmlNode;
				switch (GetNodePosition())
				{
					case XmlNodePosition.Append:
					case XmlNodePosition.After:
						if (anchor is XmlAttribute)
						{
							var xmlAttribute = (XmlAttribute) anchor;
							if (xmlAttribute.OwnerElement != null)
								xmlAttribute.OwnerElement.Attributes.InsertAfter(newXmlAttribute, xmlAttribute);
						}
						else if (anchor.Attributes != null)
						{
							anchor.Attributes.Append(newXmlAttribute);
						}
						break;
					case XmlNodePosition.Prepend:
					case XmlNodePosition.Before:
						if (anchor is XmlAttribute)
						{
							var xmlAttribute = (XmlAttribute) anchor;
							if (xmlAttribute.OwnerElement != null)
								xmlAttribute.OwnerElement.Attributes.InsertBefore(newXmlAttribute, xmlAttribute);
						}
						else if (anchor.Attributes != null)
						{
							anchor.Attributes.Prepend(newXmlAttribute);
						}
						break;
					default:
						// should not happen
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch (GetNodePosition())
				{
					case XmlNodePosition.Append:
						anchor.AppendChild(newXmlNode);
						break;
					case XmlNodePosition.Prepend:
						anchor.PrependChild(newXmlNode);
						break;
					case XmlNodePosition.After:
						if (anchor.ParentNode != null)
							anchor.ParentNode.InsertAfter(newXmlNode, anchor);
						break;
					case XmlNodePosition.Before:
						if (anchor.ParentNode != null)
							anchor.ParentNode.InsertBefore(newXmlNode, anchor);
						break;
					default:
						// should not happen
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}