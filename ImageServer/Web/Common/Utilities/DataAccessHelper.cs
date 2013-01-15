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

using System.Collections.Generic;
using System.Security.Principal;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class DataAccessHelper
    {
        private const string DataAccessSubCriteriaPrefix = "DataAccessSubCriteria";

        private static string GetDataAccessSubCriteriaCacheID(CustomPrincipal principal)
        {
            return DataAccessSubCriteriaPrefix + principal.SessionTokenId;
        }

        public static StudyDataAccessSelectCriteria GetDataAccessSubCriteriaForUser(IPersistenceContext context, IPrincipal user)
        {
            if (user.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies))
            {
                return null;
            }

            var principal = user as CustomPrincipal;
            if (principal == null)
                return null;

            string key = GetDataAccessSubCriteriaCacheID(principal);

            // check the cache first
            var subCriteria = Cache.Current[key] as StudyDataAccessSelectCriteria;
            if (subCriteria != null)
                return subCriteria;


            var oidList = new List<ServerEntityKey>();
            foreach (var oid in principal.Credentials.DataAccessAuthorityGroups)
                oidList.Add(new ServerEntityKey("OID", oid));
            var dataAccessGroupSelectCriteria = new DataAccessGroupSelectCriteria();
            dataAccessGroupSelectCriteria.AuthorityGroupOID.In(oidList);

            IList<DataAccessGroup> groups;
            var broker = context.GetBroker<IDataAccessGroupEntityBroker>();
            groups = broker.Find(dataAccessGroupSelectCriteria);


            var entityList = new List<ServerEntityKey>();
            foreach (DataAccessGroup group in groups)
            {
                entityList.Add(group.Key);
            }

            subCriteria = new StudyDataAccessSelectCriteria();
            subCriteria.DataAccessGroupKey.In(entityList);

            // put into cache for re-use
            Cache.Current[key] = subCriteria;

            return subCriteria;
        }

    }
}