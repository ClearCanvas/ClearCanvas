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
using System.Xml.Schema;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
	/// Defines an interface to an operator for use with <see cref="XmlActionCompiler{TActionContext}"/>.
    /// </summary>
    public interface IXmlActionCompilerOperator<TActionContext>
    {
        /// <summary>
        /// The name of the action implemented.  This is typically the name of the <see cref="XmlElement"/> describing the action.
        /// </summary>
        string OperatorTag { get; }

        /// <summary>
        /// Method used to compile the action.  
        /// </summary>
        /// <param name="xmlNode">Input <see cref="XmlElement"/> describing the action to perform.</param>
        /// <returns>A class implementing the <see cref="IActionItem{TActionContext}"/> interface which can perform the action.</returns>
        IActionItem<TActionContext> Compile(XmlElement xmlNode);

        /// <summary>
        /// Get an <see cref="XmlSchemaElement"/> describing the ActionItem for validation purposes.
        /// </summary>
        /// <returns></returns>
        XmlSchemaElement GetSchema();
    }
}