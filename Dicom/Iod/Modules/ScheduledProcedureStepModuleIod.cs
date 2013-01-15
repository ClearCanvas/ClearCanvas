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

using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Scheduled Procedure Step Modole
    /// </summary>
    /// <remarks>As per Dicom Doc 3, Table C.4-10 (pg 246)</remarks>
    public class ScheduledProcedureStepModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
		/// Constructor.
		/// </summary>
        public ScheduledProcedureStepModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
		public ScheduledProcedureStepModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the scheduled procedure step sequence list.
        /// </summary>
        /// <value>The scheduled procedure step sequence list.</value>
        public SequenceIodList<ScheduledProcedureStepSequenceIod> ScheduledProcedureStepSequenceList
        {
            get 
            {
                return new SequenceIodList<ScheduledProcedureStepSequenceIod>(base.DicomAttributeProvider[DicomTags.ScheduledProcedureStepSequence] as DicomAttributeSQ); 
            }
        }

       #endregion

    }


}
