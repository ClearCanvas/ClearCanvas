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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Rules.GrantAccessAction
{
    
    /// <summary>
    /// <see cref="ServerDatabaseCommand"/> derived class for use with <see cref="ServerCommandProcessor"/> for inserting AutoRoute WorkQueue entries into the Persistent Store.
    /// </summary>
    public class InsertStudyDataAccessCommand : ServerDatabaseCommand
    {
        private readonly ServerActionContext _context;
        private readonly Guid _authorityGroupOid;
    	
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">A contentxt in which to apply the GrantAccess request.</param>
        /// <param name="authorityGroupOid">The OID of the Authority Group to grant access to the Study.</param>
        public InsertStudyDataAccessCommand(ServerActionContext context, Guid authorityGroupOid)
            : base("Update/Insert a StudyDataAccess Entry")
        {
            Platform.CheckForNullReference(context, "ServerActionContext");

            _context = context;
            _authorityGroupOid = authorityGroupOid;
        }

        /// <summary>
        /// Do the insertion of the AutoRoute.
        /// </summary>
        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            var criteria = new DataAccessGroupSelectCriteria();
            criteria.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID",_authorityGroupOid));

            var authorityGroup = updateContext.GetBroker<IDataAccessGroupEntityBroker>();

            DataAccessGroup group = authorityGroup.FindOne(criteria);
			if (group == null)
			{
				Platform.Log(LogLevel.Warn,
				             "AuthorityGroupOID '{0}' on partition {1} not in database for GrantAccess request!  Ignoring request.", _authorityGroupOid,
				             _context.ServerPartition.AeTitle);

                ServerPlatform.Alert(
                                AlertCategory.Application, AlertLevel.Warning,
                                SR.AlertComponentDataAccessRule, AlertTypeCodes.UnableToProcess, null, TimeSpan.FromMinutes(5),
                                SR.AlertDataAccessUnknownAuthorityGroup, _authorityGroupOid, _context.ServerPartition.AeTitle);
                return;
			}


            var entityBroker = updateContext.GetBroker<IStudyDataAccessEntityBroker>();

            var selectStudyDataAccess = new StudyDataAccessSelectCriteria();
            selectStudyDataAccess.DataAccessGroupKey.EqualTo(group.Key);
            selectStudyDataAccess.StudyStorageKey.EqualTo(_context.StudyLocationKey);

            if (entityBroker.Count(selectStudyDataAccess) == 0)
            {
                var insertColumns = new StudyDataAccessUpdateColumns
                                        {
                                            DataAccessGroupKey = group.Key,
                                            StudyStorageKey = _context.StudyLocationKey
                                        };

                entityBroker.Insert(insertColumns);
            }
        }
    }
}
