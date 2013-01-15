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
using System.Text;
using System.Xml;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines an interface an item to be written to the specified <see cref="XmlWriter"/>.
    /// </summary>
    public interface IExportItem
    {
        void Write(XmlWriter writer);
    }

    /// <summary>
    /// Defines an interface to an item to be read from a <see cref="XmlReader"/>.
    /// </summary>
    public interface IImportItem
    {
        XmlReader Read();
    }

    /// <summary>
    /// Defines an interface to a class that is responsible for exporting/importing data in XML format.
    /// </summary>
    public interface IXmlDataImex
    {
        /// <summary>
        /// Export all data as a set of <see cref="IExportItem"/>s.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IExportItem> Export();

        /// <summary>
        /// Import the specified set of <see cref="IImportItem"/>s.
        /// </summary>
        /// <param name="items"></param>
        void Import(IEnumerable<IImportItem> items);
    }
}
