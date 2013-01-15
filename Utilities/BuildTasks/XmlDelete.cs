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
	/// A task for deleting nodes from an XML document.
	/// </summary>
	/// <remarks>
	/// This task deletes the nodes selected by the <see cref="XmlTaskBase.XPath"/> expression.
	/// </remarks>
	public class XmlDelete : XmlTaskBase
	{
		/// <summary>
		/// Outputs whether or not the selected nodes were deleted.
		/// </summary>
		[Output]
		public bool Deleted { get; set; }

		protected override bool PerformTask(XmlNodeList xmlNodes)
		{
			foreach (XmlNode xmlNode in xmlNodes)
			{
				if (xmlNode is XmlAttribute)
				{
					var xmlAttribute = (XmlAttribute) xmlNode;
					if (xmlAttribute.OwnerElement != null)
						xmlAttribute.OwnerElement.Attributes.Remove(xmlAttribute);
				}
				else if (xmlNode is XmlEntity || xmlNode is XmlNotation)
				{
					if (xmlNode.OwnerDocument != null)
						xmlNode.OwnerDocument.RemoveChild(xmlNode);
				}
				else if (xmlNode.ParentNode != null)
				{
					xmlNode.ParentNode.RemoveChild(xmlNode);
				}
				else
				{
					throw new NotSupportedException("Attempt to remove unsupported XmlNode");
				}
			}

			FlagModified(Deleted = xmlNodes.Count > 0);
			return true;
		}
	}
}