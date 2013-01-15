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

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Detailed view of a <see cref="Study"/> in the context of the WorkQueue configuration UI.
    /// </summary>
    /// <remarks>
    /// A <see cref="StudyDetails"/> contains detailed information of a <see cref="Study"/> and related information 
    /// to be displayed within the WorkQueue configuration UI.
    /// <para>
    /// A <see cref="StudyDetails"/> can be created using a <see cref="StudyDetailsAssembler"/> object.
    /// </para>
    /// </remarks>
    /// <seealso cref="WorkQueueDetails"/>
    public class StudyDetails
    {
        #region Public Properties

        public string StudyInstanceUID { get; set; }

        public string Status { get; set; }

        public string PatientName { get; set; }

        public string AccessionNumber { get; set; }

        public string PatientID { get; set; }

        public string StudyDescription { get; set; }

        public string StudyDate { get; set; }

        public string StudyTime { get; set; }

        public string Modalities { get; set; }

        public bool? WriteLock { get; set; }

		public short ReadLock { get; set; }

        #endregion Public Properties
    }
}