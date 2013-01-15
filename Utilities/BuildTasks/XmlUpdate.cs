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
	/// A task for updating nodes in an XML document.
	/// </summary>
	/// <remarks>
	/// This task updates the nodes selected by the <see cref="XmlTaskBase.XPath"/> expression.
	/// </remarks>
	public class XmlUpdate : XmlUpdateTaskBase
	{
		/// <summary>
		/// Outputs whether or not the selected nodes were updated.
		/// </summary>
		[Output]
		public bool Updated { get; set; }

		protected override bool PerformTask(XmlNodeList xmlNodes)
		{
			var success = Cross(xmlNodes, GetOperations(), (node, op) => op.Update(node));
			if (!success)
			{
				Log.LogError("Inputs must have only 1 item or be in 1:1 correspondance with selected nodes");
				return false;
			}

			FlagModified(Updated = xmlNodes.Count > 0);
			return true;
		}
	}

	/// <summary>
	/// Base class for <see cref="XmlTaskBase"/>s that update nodes in XML documents.
	/// </summary>
	public abstract class XmlUpdateTaskBase : XmlTaskBase
	{
		/// <summary>
		/// The inner XML contents with which to update the nodes. May not be used in conjunction with <see cref="Value"/>.
		/// </summary>
		public string[] InnerXml { get; set; }

		/// <summary>
		/// The value contents with which to update the nodes. May not be used in conjunction with <see cref="InnerXml"/>.
		/// </summary>
		public string[] Value { get; set; }

		/// <summary>
		/// The attribute names with which to update the nodes. Must be used in conjunction with <see cref="Value"/>.
		/// </summary>
		public string[] Attribute { get; set; }

		/// <summary>
		/// The XML namespace URI for the attribute names with which to update the nodes. Must be used in conjunction with <see cref="Attribute"/> and <see cref="Value"/>.
		/// </summary>
		public string[] AttributeNamespace { get; set; }

		/// <summary>
		/// A value indicating whether or not empty elements should be condensed into the empty tag format.
		/// </summary>
		public bool UseEmptyTag { get; set; }

		/// <summary>
		/// Parses the values of <see cref="InnerXml"/>, <see cref="Value"/> and <see cref="Attribute"/> into <see cref="IUpdateOperation"/>s.
		/// </summary>
		protected IUpdateOperation[] GetOperations()
		{
			if (!IsNullOrEmpty(InnerXml))
			{
				var ops = new IUpdateOperation[InnerXml.Length];
				for (int n = 0; n < InnerXml.Length; n++)
					ops[n] = new SetInnerXmlOperation(InnerXml[n], UseEmptyTag);
				return ops;
			}
			else if (!IsNullOrEmpty(Value))
			{
				var ops = new IUpdateOperation[Value.Length];
				if (!IsNullOrEmpty(Attribute))
				{
					if (Attribute.Length > 1 && Attribute.Length != Value.Length)
						throw new ArgumentException("Attribute must have only 1 item or be in 1:1 correspondance with Value");

					var namespaceUri = !IsNullOrEmpty(AttributeNamespace) ? AttributeNamespace : new[] {""};
					if (namespaceUri.Length > 1 && namespaceUri.Length != Attribute.Length)
						throw new ArgumentException("AttributeNamespace must have only 1 item or be in 1:1 correspondance with Attribute");

					for (int n = 0; n < Value.Length; n++)
						ops[n] = new SetAttributeOperation(Value[n], Attribute.Length == 1 ? Attribute[0] : Attribute[n], namespaceUri.Length == 1 ? namespaceUri[0] : namespaceUri[n]);
				}
				else
				{
					for (int n = 0; n < Value.Length; n++)
						ops[n] = new SetValueOperation(Value[n], UseEmptyTag);
				}
				return ops;
			}
			else
			{
				return new[] {new SetValueOperation(string.Empty, UseEmptyTag)};
			}
		}

		protected override bool ValidateParameters(out string message)
		{
			if (!IsNullOrEmpty(InnerXml) && !IsNullOrEmpty(Value))
			{
				message = "Only one of InnerXml and Value can be set";
				return false;
			}

			try
			{
				GetOperations();
			}
			catch (ArgumentException ex)
			{
				message = ex.Message;
				return false;
			}

			return base.ValidateParameters(out message);
		}

		/// <summary>
		/// Determines whether or not a <see cref="string"/> <paramref name="array"/> is null, empty, or contains a single null or empty string.
		/// </summary>
		private static bool IsNullOrEmpty(string[] array)
		{
			return array == null || array.Length == 0 || (array.Length == 1 && string.IsNullOrEmpty(array[0]));
		}

		/// <summary>
		/// Callback function for <see cref="Cross{T}"/>.
		/// </summary>
		protected delegate void CrossCallback<T>(XmlNode xmlNode, T item);

		/// <summary>
		/// Performs a <paramref name="callback"/> after matching each node in <paramref name="xmlNodeList"/> to an appropriate element in <paramref name="items"/>.
		/// </summary>
		/// <returns>Returns False if there is no 1:1, 1:N or N:1 correspondance between nodes and items.</returns>
		protected static bool Cross<T>(XmlNodeList xmlNodeList, T[] items, CrossCallback<T> callback)
		{
			var nodeCount = xmlNodeList.Count;
			var inputCount = items.Length;

			if (nodeCount > 1 && inputCount > 1 && nodeCount != inputCount)
				return false;

			if (nodeCount == inputCount)
			{
				for (int n = 0; n < items.Length; n++)
					callback.Invoke(xmlNodeList[n], items[n]);
			}
			else if (inputCount == 1)
			{
				for (int n = 0; n < xmlNodeList.Count; n++)
					callback.Invoke(xmlNodeList[n], items[0]);
			}
			else if (nodeCount == 1)
			{
				for (int n = 0; n < items.Length; n++)
					callback.Invoke(xmlNodeList[0], items[n]);
			}
			return true;
		}

		/// <summary>
		/// Represents an operation that updates an <see cref="XmlNode"/>.
		/// </summary>
		protected interface IUpdateOperation
		{
			/// <summary>
			/// Updates the <paramref name="xmlNode"/>.
			/// </summary>
			void Update(XmlNode xmlNode);
		}

		/// <summary>
		/// An <see cref="IUpdateOperation"/> that updates the <see cref="XmlNode.InnerXml"/> of an <see cref="XmlNode"/>.
		/// </summary>
		private class SetInnerXmlOperation : IUpdateOperation
		{
			private readonly string _innerXml;
			private readonly bool _useEmptyTag;

			public SetInnerXmlOperation(string innerXml, bool useEmptyTag)
			{
				_innerXml = innerXml ?? string.Empty;
				_useEmptyTag = useEmptyTag;
			}

			public void Update(XmlNode xmlNode)
			{
				xmlNode.InnerXml = _innerXml;
				if (xmlNode is XmlElement)
					((XmlElement) xmlNode).IsEmpty = string.IsNullOrEmpty(_innerXml) && _useEmptyTag;
			}
		}

		/// <summary>
		/// An <see cref="IUpdateOperation"/> that updates the <see cref="XmlNode.InnerText"/> of an <see cref="XmlElement"/> or the <see cref="XmlNode.Value"/> of any other <see cref="XmlNode"/>.
		/// </summary>
		private class SetValueOperation : IUpdateOperation
		{
			private readonly string _value;
			private readonly bool _useEmptyTag;

			public SetValueOperation(string value, bool useEmptyTag)
			{
				_value = value ?? string.Empty;
				_useEmptyTag = useEmptyTag;
			}

			public void Update(XmlNode xmlNode)
			{
				if (xmlNode is XmlElement)
				{
					// the Value property is invalid on an XmlElement - use InnerText instead
					xmlNode.InnerText = _value;
					((XmlElement) xmlNode).IsEmpty = string.IsNullOrEmpty(_value) && _useEmptyTag;
				}
				else
				{
					xmlNode.Value = _value;
				}
			}
		}

		/// <summary>
		/// An <see cref="IUpdateOperation"/> that updates the <see cref="XmlNode.Attributes"/> of an <see cref="XmlNode"/>.
		/// </summary>
		private class SetAttributeOperation : IUpdateOperation
		{
			private readonly string _value;
			private readonly string _attribute;
			private readonly string _namespaceUri;

			public SetAttributeOperation(string value, string attribute, string namespaceUri)
			{
				_value = value ?? string.Empty;
				_attribute = attribute;
				_namespaceUri = namespaceUri ?? string.Empty;
			}

			public void Update(XmlNode xmlNode)
			{
				if (xmlNode.Attributes != null)
				{
					var attribute = xmlNode.Attributes[_attribute];
					if (attribute == null && xmlNode.OwnerDocument != null)
					{
						attribute = xmlNode.OwnerDocument.CreateAttribute(_attribute, _namespaceUri);
						xmlNode.Attributes.Append(attribute);
					}

					if (attribute != null)
						attribute.Value = _value;
				}
			}
		}
	}
}