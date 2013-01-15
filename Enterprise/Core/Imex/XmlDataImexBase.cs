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
using System.Xml;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Abstract base implementation of <see cref="IXmlDataImex"/>.
    /// </summary>
    public abstract class XmlDataImexBase : IXmlDataImex
    {
        /// <summary>
        /// Implements export functionality.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IExportItem> ExportCore();

        /// <summary>
        /// Implements import functionality.
        /// </summary>
        /// <param name="items"></param>
        protected abstract void ImportCore(IEnumerable<IImportItem> items);

        #region IXmlDataImex Members

        IEnumerable<IExportItem> IXmlDataImex.Export()
        {
            return ExportCore();
        }

        void IXmlDataImex.Import(IEnumerable<IImportItem> items)
        {
            ImportCore(items);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Writes the specified data to the specified xml writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="data"></param>
        protected static void Write(XmlWriter writer, object data)
        {
            JsmlSerializer.Serialize(writer, data, data.GetType().Name);
        }

        /// <summary>
        /// Reads an object of the specified class from the xml reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dataContractClass"></param>
        /// <returns></returns>
        protected static object Read(XmlReader reader, Type dataContractClass)
        {
            return JsmlSerializer.Deserialize(reader, dataContractClass);
        }

         #endregion
    }
}
