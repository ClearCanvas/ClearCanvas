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
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    public class ChangeSetRecorder : IEntityChangeSetRecorder
    {
        public string OperationName { get; set; }

        public void WriteLogEntry(IEnumerable<EntityChange> changeSet, AuditLog auditLog)
        {
            AuditLogEntry entry = null;
            try
            {
                entry = CreateLogEntry(changeSet);
                auditLog.WriteEntry(entry.Category, entry.Details);
            }
            catch(Exception ex)
            {
                // Error saving the audit log repository. Write to log file instead.

                Platform.Log(LogLevel.Error, ex, "Error occurred when writing audit log");
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Audit log entry failed to save:");

                if (entry!=null)
                {
                    sb.AppendLine(String.Format("Operation: {0}", entry.Operation));
                    sb.AppendLine(String.Format("Details: {0}", entry.Details));
                }
                else
                {
                    foreach (EntityChange change in changeSet)
                    {
                        sb.AppendLine(String.Format("Changeset: {0} on entity: {1}", change.ChangeType, change.EntityRef));
                        if (change.PropertyChanges != null)
                        {
                            foreach (PropertyChange property in change.PropertyChanges)
                            {
                                sb.AppendLine(String.Format("{0} : Old Value: {1}\tNew Value: {2}",
                                                            property.PropertyName, property.OldValue, property.NewValue));

                            }
                        }
                    }
                }
                
                Platform.Log(LogLevel.Info, sb.ToString());
            }

        }

        public AuditLogEntry CreateLogEntry(IEnumerable<EntityChange> changeSet)
        {
            string details = string.Empty;
            string type = string.Empty;
            foreach (EntityChange change in changeSet)
            {
                if (change.ChangeType == EntityChangeType.Create)
                {
                    type = "Create";
                }
                else if (change.ChangeType == EntityChangeType.Delete)
                {
                    type = "Delete";
                }
                else if (change.ChangeType == EntityChangeType.Update)
                {
                    type = "Update";
                }
            }
            return new AuditLogEntry("ImageServer", type, details);			
        }
    }
}