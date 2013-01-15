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
using System.Threading;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// An implementation of <see cref="ISpecification"/> that tests if the current thread principal is in a given role.
    /// </summary>
    public class PrincipalPermissionSpecification : ISpecification
    {
        private readonly string _role;

        /// <summary>
        /// Constructs an instance of this class for the specified role.
        /// </summary>
        public PrincipalPermissionSpecification(string role)
        {
            _role = role;
        }

        #region ISpecification Members

        /// <summary>
        /// Tests the <see cref="Thread.CurrentPrincipal"/> for the permission represented by this object.
        /// </summary>
		/// <remarks>
		/// If the application is running in non-authenticated (stand-alone) mode, the test will always
		/// succeed.  If the application is running in authenticated (enterprise) mode, the test succeeds only
		/// if the thread current principal is in the role assigned to this instance.
		/// </remarks>
		/// <param name="obj">This parameter is ignored.</param>
        public TestResult Test(object obj)
        {
			// if the thread is running in a non-authenticated mode, then we have no choice but to allow.
			// this seems a little counter-intuitive, but basically we're counting on the fact that if
			// the desktop is running in an enterprise environment, then the thread *will* be authenticated,
			// and that this is enforced by some mechanism outside the scope of this class.  The only
			// scenario in which the thread would ever be unauthenticated is the stand-alone scenario.
			if(Thread.CurrentPrincipal == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false)
				return new TestResult(true);

			// if running in authenticated mode, test the role
            return new TestResult(Thread.CurrentPrincipal.IsInRole(_role));
        }

        #endregion
    }
}
