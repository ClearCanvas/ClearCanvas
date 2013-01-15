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

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Represents an exception thrown when an attempt is made to set the dicom text value
    /// which character(s) which is not covered by the specific character set of the Dicom attribute collection 
    /// which the attribute is part of.
    /// </summary>
    public class DicomCharacterSetException : Exception
    {
        public DicomCharacterSetException(uint tag, string characterSets, string offendedValue, string message)
            : base(message)
        {
            DicomTag = tag;
            SpecificCharacterSets = characterSets;
            OffendedValue = offendedValue;
        }

        public uint DicomTag { get; private set; }
        public string SpecificCharacterSets { get; private set; }
        public string OffendedValue { get; private set; }
    }
}