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
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Wraps <seealso cref="AuthorityGroupDetail"/> for display purposes. This class also contains additional properties related to data access.
    /// </summary>
    public class AuthorityGroupStudyAccessInfo
    {
        #region Public Properties

        public string AuthorityOID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this authority group can access to all server partitions
        /// </summary>
        public bool CanAccessToAllPartitions { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this authority group can access to all studies within a given partition
        /// </summary>
        public bool CanAccessToAllStudies { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this authority group is a Data Access group
        /// </summary>
        public bool IsDataAccessAuthorityGroup { get; private set; }

        public StudyDataAccess StudyDataAccess { get; set; }

        #endregion


        #region Constructor

        public AuthorityGroupStudyAccessInfo(AuthorityGroupDetail detail)
        {
            AuthorityOID = detail.AuthorityGroupRef.ToString(false, false);
            Name = detail.Name;
            Description = detail.Description;
            IsDataAccessAuthorityGroup = detail.DataGroup;

            CanAccessToAllPartitions = HasToken(detail.AuthorityTokens,
                                                ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllPartitions);


            CanAccessToAllStudies= HasToken(detail.AuthorityTokens,
                                            ClearCanvas.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies);
        }

        #endregion


        #region  Private Methods

        private static bool HasToken(List<AuthorityTokenSummary> tokens, string token)
        {
            return tokens.Exists(t => t.Name.Equals(token, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}