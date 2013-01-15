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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageServer.Common
{
    public static class ServerAuditHelper
    {

        public static void AddAuthorityGroupAccess(string studyInstanceUid, string accessionNumber, IList<string> assignedGroups)
        {
            Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
            Platform.CheckForNullReference(assignedGroups, "assignedGroups");

            var helper =
                new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                                                      EventIdentificationContentsEventOutcomeIndicator.Success,
                                                      EventIdentificationContentsEventActionCode.U,
                                                      EventTypeCode.ObjectSecurityAttributesChanged);

            // TODO: 8/19/2011, Develop a way to get the DisplayName for the user here for the audit log message
            helper.AddUser(new AuditPersonActiveParticipant(
                               Thread.CurrentPrincipal.Identity.Name,
                               null,
                               null));

            var participant = new AuditStudyParticipantObject(studyInstanceUid, accessionNumber);

            string updateDescription = StringUtilities.Combine(
                assignedGroups, ";",
                item => String.Format("Assigned Group Access=\"{0}\"", item)
                );

            participant.ParticipantObjectDetailString = updateDescription;
            helper.AddStudyParticipantObject(participant);


            ServerPlatform.LogAuditMessage(helper);
        }

        public static void RemoveAuthorityGroupAccess(string studyInstanceUid, string accessionNumber, IList<string> assignedGroups)
        {
            Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
            Platform.CheckForNullReference(assignedGroups, "assignedGroups");

            var helper =
                new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                                                      EventIdentificationContentsEventOutcomeIndicator.Success,
                                                      EventIdentificationContentsEventActionCode.U,
                                                      EventTypeCode.ObjectSecurityAttributesChanged);

            // TODO: 8/19/2011, Develop a way to get the DisplayName for the user here for the audit log message
            helper.AddUser(new AuditPersonActiveParticipant(
                               Thread.CurrentPrincipal.Identity.Name,
                               null,
                               null));


            var participant = new AuditStudyParticipantObject(studyInstanceUid, accessionNumber);

            string updateDescription = StringUtilities.Combine(
                assignedGroups, ";",
                item => String.Format("Removed Group Access=\"{0}\"", item)
                );

            participant.ParticipantObjectDetailString = updateDescription;
            helper.AddStudyParticipantObject(participant);

            ServerPlatform.LogAuditMessage(helper);
        }
    }
}
