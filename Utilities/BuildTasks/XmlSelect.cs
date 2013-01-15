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
using Microsoft.Build.Framework;

namespace ClearCanvas.Utilities.BuildTasks
{
	/// <summary>
	/// A task for selecting nodes from an XML document.
	/// </summary>
	/// <remarks>
	/// This task returns the properties of the nodes selected by the <see cref="XmlTaskBase.XPath"/> expression.
	/// </remarks>
	public class XmlSelect : XmlTaskBase
	{
		/// <summary>
		/// Outputs the names of the selected nodes.
		/// </summary>
		/// <remarks>
		/// The number of elements in this output will correspond with the number of nodes selected by <see cref="XmlTaskBase.XPath"/>.
		/// </remarks>
		[Output]
		public string[] Name { get; set; }

		/// <summary>
		/// Outputs the inner XML contents of the selected nodes.
		/// </summary>
		/// <remarks>
		/// The number of elements in this output will correspond with the number of nodes selected by <see cref="XmlTaskBase.XPath"/>.
		/// </remarks>
		[Output]
		public string[] InnerXml { get; set; }

		/// <summary>
		/// Outputs the outer XML contents of the selected nodes.
		/// </summary>
		/// <remarks>
		/// The number of elements in this output will correspond with the number of nodes selected by <see cref="XmlTaskBase.XPath"/>.
		/// </remarks>
		[Output]
		public string[] OuterXml { get; set; }

		/// <summary>
		/// Outputs the value contents of the selected nodes.
		/// </summary>
		/// <remarks>
		/// The number of elements in this output will correspond with the number of nodes selected by <see cref="XmlTaskBase.XPath"/>.
		/// </remarks>
		[Output]
		public string[] Value { get; set; }

		/// <summary>
		/// Outputs whether or not the selected nodes exist.
		/// </summary>
		[Output]
		public bool Exists { get; set; }

		protected override bool PerformTask(XmlNodeList xmlNodes)
		{
			var count = xmlNodes.Count;

			var name = new string[count];
			var innerXml = new string[count];
			var outerXml = new string[count];
			var value = new string[count];
			for (int n = 0; n < count; n++)
			{
				name[n] = xmlNodes[n].Name;
				innerXml[n] = xmlNodes[n].InnerXml;
				outerXml[n] = xmlNodes[n].OuterXml;
				value[n] = xmlNodes[n] is XmlElement ? xmlNodes[n].InnerText : xmlNodes[n].Value;
			}

			Name = name;
			InnerXml = innerXml;
			OuterXml = outerXml;
			Value = value;
			Exists = xmlNodes.Count > 0;
			return true;
		}
	}
}