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

using NHibernate;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IReadContext"/>.
    /// </summary>
    public class ReadContext : PersistenceContext, IReadContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory"></param>
        internal ReadContext(PersistentStore pstore)
            : base(pstore)
        {
        }

        #region Protected overrides

        protected override ISession CreateSession()
        {
            ISession session = this.PersistentStore.SessionFactory.OpenSession();

            // never write changes to the database from a read context
            session.FlushMode = FlushMode.Never;
            return session;
        }

        protected override void SynchStateCore()
        {
            // do nothing
        }

        internal override bool ReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    // commit the transaction - nothing will be flushed to the DB
                    CommitTransaction();
                }
                catch (Exception e)
                {
                    HandleHibernateException(e, SR.ExceptionCloseContext);
                }
            }

            base.Dispose(disposing);
        }

        protected override EntityLoadFlags DefaultEntityLoadFlags
        {
            get { return EntityLoadFlags.None; }
        }

        protected override void LockCore(DomainObject obj, DirtyState dirtyState)
        {
            if (dirtyState != DirtyState.Clean)
                throw new InvalidOperationException();

            this.Session.Lock(obj, LockMode.None);
        }

        #endregion
    }
}
