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

using System.Data.SqlClient;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    /// <summary>
    /// Provides implementation of <see cref="IReadContext"/> for use with ADO.NET and Sql Server.
    /// </summary>
    public class ReadContext : PersistenceContext,IReadContext
    {
        internal ReadContext(SqlConnection connection, ITransactionNotifier transactionNotifier)
            : base(connection, transactionNotifier)
        { }

        #region IDisposable Members

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // end the transaction
            }

            // important to call base class to close the session, etc.
            base.Dispose(disposing);
        }

        #endregion
    }
}