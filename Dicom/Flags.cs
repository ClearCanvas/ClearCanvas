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

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Static helper class for checking if flags have been set.
    /// </summary>
    public static class Flags
    {
        public static bool IsSet(DicomDumpOptions options, DicomDumpOptions flag)
        {
            return (options & flag) == flag;
        }
        public static bool IsSet(DicomReadOptions options, DicomReadOptions flag)
        {
            return (options & flag) == flag;
        }
        public static bool IsSet(DicomWriteOptions options, DicomWriteOptions flag)
        {
            return (options & flag) == flag;
        }
    }

    /// <summary>
    /// An enumerated value to specify options when generating a dump of a DICOM object.
    /// </summary>
    [Flags]
    public enum DicomDumpOptions
    {
        None = 0,
        ShortenLongValues = 1,
        Restrict80CharactersPerLine= 2,
        KeepGroupLengthElements = 4,
        Default = ShortenLongValues | Restrict80CharactersPerLine
    }

    /// <summary>
    /// An enumerated value to specify options when reading DICOM files. 
    /// </summary>
    [Flags]
    public enum DicomReadOptions
    {
        None = 0,
        KeepGroupLengths = 1,
        UseDictionaryForExplicitUN = 2,
        AllowSeekingForContext = 4,
        ReadNonPart10Files = 8,
        DoNotStorePixelDataInDataSet = 16,
        StorePixelDataReferences = 32,
        Default = UseDictionaryForExplicitUN | AllowSeekingForContext | ReadNonPart10Files
    }

    /// <summary>
    /// An enumerated value to specify options when writing DICOM files.
    /// </summary>
    [Flags]
    public enum DicomWriteOptions
    {
        None = 0,
        CalculateGroupLengths = 1,
        ExplicitLengthSequence = 2,
        ExplicitLengthSequenceItem = 4,
        WriteFragmentOffsetTable = 8,
        Default = WriteFragmentOffsetTable
    }
}
