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

#pragma warning disable 1591

using System.Xml;
using System.Xml.Schema;

namespace ClearCanvas.Common.Specifications
{
	/// <summary>
	/// Interface for Specification Operators.
	/// </summary>
	public interface IXmlSpecificationCompilerOperator
	{
		/// <summary>
		/// The XML Tag for the operator.
		/// </summary>
		string OperatorTag { get; }

		/// <summary>
		/// Compile the operator.
		/// </summary>
		/// <param name="xmlNode">The XML Node associated with the operator.</param>
		/// <param name="context">A context for the compiler.</param>
		/// <returns>A compiled <see cref="Specification"/>.</returns>
		Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context);

		/// <summary>
		/// Get an XmlSchema element that describes the schema for the operator element.
		/// </summary>
		/// <remarks>
		/// <para>
		/// It is assumed that a simple <see cref="XmlSchemaElement"/> is returned for the 
		/// operator.  The compiler combine the elements for each operator together into an
		/// <see cref="XmlSchema"/>.  If the specific element allows subelements, it should 
		/// be declared to allow any elements from the local namespace/Schema.
		/// </para>
		/// </remarks>
		/// <returns>The Schema element.</returns>
		XmlSchemaElement GetSchema();
	}
}
