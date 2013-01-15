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

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Stores DICOM implementation specific information.
    /// </summary>
    public static class DicomImplementation
    {
        #region Private Static Members
        private static DicomUid _classUid = new DicomUid("1.3.6.1.4.1.25403.1.1.1", "Implementation Class UID", UidType.Unknown);
        private static string _version = "Dicom 0.1";
        private static IDicomCharacterSetParser _characterParser = new SpecificCharacterSetParser();
        private static bool _unitTest = false;
        #endregion

        #region Public Static Properties
        /// <summary>
        /// Unit tests are currently being run.
        /// </summary>
        public static bool UnitTest
        {
            get { return _unitTest; }
            set { _unitTest = value; }
        }
        /// <summary>
        /// The DICOM Implementation Class UID.
        /// </summary>
        public static DicomUid ClassUID
        {
            get { return _classUid; }
            set { _classUid = value; }        
        }

        /// <summary>
        /// The DICOM Implementation Version.
        /// </summary>
        public static string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// The Specific Character Set Parser used by the implementation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property sets the parser to be used to translate between raw bytes encoded in a
        /// DICOM stream for text attributes and the unicode characters stored by the assembly 
        /// for text attributes.  A default implementation is included, which can be overridden.
        /// </para>
        /// <para>
        /// See the <see cref="IDicomCharacterSetParser"/> interface for the methods required 
        /// to be implemented for a character set parser.
        /// </para>
        /// </remarks>
        public static IDicomCharacterSetParser CharacterParser
        {
            get { return _characterParser; }
            set { _characterParser = value; }
        }
        #endregion
    }

}
