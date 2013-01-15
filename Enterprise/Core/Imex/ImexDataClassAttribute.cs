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

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines the class of data that an extension of <see cref="XmlDataImexExtensionPoint"/> is
    /// responsible for.
    /// </summary>
    /// <remarks>
    /// The data-class is a logical class name and need not refer to an actual entity class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class ImexDataClassAttribute : Attribute
    {
        private readonly string _dataClass;
        private int _itemsPerFile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataClass"></param>
        public ImexDataClassAttribute(string dataClass)
        {
            _dataClass = dataClass;
        }

        /// <summary>
        /// Gets the name of the data class.
        /// </summary>
        public string DataClass
        {
            get { return _dataClass; }
        }

        /// <summary>
        /// Gets or sets the default number of items to export per file.
        /// </summary>
        public int ItemsPerFile
        {
            get { return _itemsPerFile; }
            set { _itemsPerFile = value; }
        }
    }
}
