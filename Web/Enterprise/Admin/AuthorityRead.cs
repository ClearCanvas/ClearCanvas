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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Web.Enterprise.Admin
{   
     /// <summary>
    /// Wrapper for <see cref="IAuthorityGroupReadService"/> service.
    /// </summary>
    public sealed class AuthorityRead : IDisposable
    {
        private IAuthorityGroupReadService _service;

        public AuthorityRead()
        {
            _service =  Platform.GetService<IAuthorityGroupReadService>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_service != null && _service is IDisposable)
            {
                (_service as IDisposable).Dispose();
                _service = null;
            }
        }

        #endregion

        public IList<AuthorityGroupSummary> ListAllAuthorityGroups()
        {
            return _service.ListAuthorityGroups(new ListAuthorityGroupsRequest(){ }).AuthorityGroups;
        }

        public IList<AuthorityGroupDetail> ListAllAuthorityGroupDetails()
        {
            return _service.ListAuthorityGroups(new ListAuthorityGroupsRequest() { Details = true }).AuthorityGroupDetails;
        }

        public IList<AuthorityGroupDetail> ListDataAccessAuthorityGroupDetails()
        {
            var rq = new ListAuthorityGroupsRequest
                         {
                             DataGroup = true,
                             Details = true
                         };

            return _service.ListAuthorityGroups(rq).AuthorityGroupDetails;
        }

        public IList<AuthorityGroupSummary> ListDataAccessAuthorityGroups()
        {
            var rq = new ListAuthorityGroupsRequest
                         {
                             DataGroup = true
                         };

            return _service.ListAuthorityGroups(rq).AuthorityGroups;
        }
    }
}
