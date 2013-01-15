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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public class ExportCommandLine : CommandLine
    {
        private string _path;
        private string _dataClass;
        private bool _allClasses;
        private int _itemsPerFile;


        [CommandLineParameter(0, "path", Required = true)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [CommandLineParameter("class", "c", "Specifies the class of data to export. Required unless /all is specified.")]
        public string DataClass
        {
            get { return _dataClass; }
            set { _dataClass = value; }
        }

        [CommandLineParameter("all", "a", "Specifies that all data classes should be imported/exported.")]
        public bool AllClasses
        {
            get { return _allClasses; }
            set { _allClasses = value; }
        }

        [CommandLineParameter("i", "Specifies the number of items per file.  If 0 or ommitted, all items will be written to one file.")]
        public int ItemsPerFile
        {
            get { return _itemsPerFile; }
            set { _itemsPerFile = value; }
        }
    }
}
