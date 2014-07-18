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
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    public static class ServerAuditHelper
    {
        private static readonly object _syncLock = new object();
        private static DicomAuditSource _auditSource;
        private static AuditLog _log;
    

        /// <summary>
        /// A well known AuditSource for ImageServer audit logging.
        /// </summary>
        public static DicomAuditSource AuditSource
        {
            get
            {
                lock (_syncLock)
                {
	                return _auditSource ?? (_auditSource = new DicomAuditSource("ImageServer"));
                }
            }
        }

        public static void AddAuthorityGroupAccess(string studyInstanceUid, string accessionNumber, IList<string> assignedGroups)
        {
            Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
            Platform.CheckForNullReference(assignedGroups, "assignedGroups");

            var helper =
                new DicomInstancesAccessedAuditHelper(AuditSource,
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


            LogAuditMessage(helper);
        }

        public static void RemoveAuthorityGroupAccess(string studyInstanceUid, string accessionNumber, IList<string> assignedGroups)
        {
            Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
            Platform.CheckForNullReference(assignedGroups, "assignedGroups");

            var helper =
                new DicomInstancesAccessedAuditHelper(AuditSource,
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

            LogAuditMessage(helper);
        }

		/// <summary>
		/// Gets the session token ID of the current thread or null if not established.
		/// </summary>
		/// <returns></returns>
		private static string GetUserSessionId()
		{
			var p = Thread.CurrentPrincipal as IUserCredentialsProvider;
			return (p != null) ? p.SessionTokenId : null;
		}

		/// <summary>
		/// Gets the identity of the current thread or null if not established.
		/// </summary>
		/// <returns></returns>
		private static string GetUserName()
		{
			var p = Thread.CurrentPrincipal;

			if (p == null || p.Identity == null)
				return null;

			//TODO (CR September 2011) - this is not a robust solution.
			// Check if it's being called in a service.
			if (p.Identity is WindowsIdentity)
				return null;

			return p.Identity.Name;
		}


        /// <summary>
        /// Log an Audit message.
        /// </summary>
        /// <param name="helper"></param>
        public static void LogAuditMessage(DicomAuditHelper helper)
        {
			// Found doing this on the local thread had a performance impact with some DICOM operations,
			// make run as a task in the background to make it work faster.
	        
	        Task.Factory.StartNew(delegate
		        {
			        lock (_syncLock)
			        {
				        if (_log == null)
					        _log = new AuditLog(ProductInformation.Component, "DICOM");

				        string serializeText = null;
				        try
				        {
					        serializeText = helper.Serialize(false);
							_log.WriteEntry(helper.Operation, serializeText, GetUserName(), GetUserSessionId());
				        }
				        catch (Exception ex)
				        {
					        Platform.Log(LogLevel.Error, ex, "Error occurred when writing audit log");

					        var sb = new StringBuilder();
					        sb.AppendLine("Audit Log failed to save:");
					        sb.AppendLine(String.Format("Operation: {0}", helper.Operation));
					        sb.AppendLine(String.Format("Details: {0}", serializeText));
					        Platform.Log(LogLevel.Info, sb.ToString());
				        }
			        }
		        });
        }
    }
}
