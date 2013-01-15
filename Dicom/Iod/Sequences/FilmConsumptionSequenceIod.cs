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
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Sequences
{
    /// <summary>
    /// Film Consumption Sequence.  
    /// </summary>
    /// <remarks>As per Part 3, Table C4.17, pg 260</remarks>
    public class FilmConsumptionSequenceIod : SequenceIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FilmConsumptionSequenceIod"/> class.
        /// </summary>
        public FilmConsumptionSequenceIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilmConsumptionSequenceIod"/> class.
        /// </summary>
        /// <param name="dicomSequenceItem">The dicom sequence item.</param>
        public FilmConsumptionSequenceIod(DicomSequenceItem dicomSequenceItem)
            : base(dicomSequenceItem)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Number of films actually printed.
        /// </summary>
        /// <value>The number of films.</value>
        public int NumberOfFilms
        {
            get { return base.DicomAttributeProvider[DicomTags.NumberOfFilms].GetInt32(0, 0); }
            set { base.DicomAttributeProvider[DicomTags.NumberOfFilms].SetInt32(0, value); }
        }

        /// <summary>
        /// Type(s) of medium on which images were printed.
        /// </summary>
        /// <value>The type of the medium.</value>
        public MediumType MediumType
        {
            get { return IodBase.ParseEnum<MediumType>(base.DicomAttributeProvider[DicomTags.MediumType].GetString(0, String.Empty), MediumType.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.MediumType], value); }
        }

        /// <summary>
        /// Size(s) of film on which images were printed.
        /// </summary>
        /// <value>The film size id.</value>
        public FilmSize FilmSizeId
        {
            get { return FilmSize.FromDicomString(base.DicomAttributeProvider[DicomTags.FilmSizeId].GetString(0, String.Empty)); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.FilmSizeId], value.DicomString); }
        }
        
        
        #endregion
    }

    
}
