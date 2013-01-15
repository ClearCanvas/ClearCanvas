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

namespace ClearCanvas.Enterprise.Hibernate
{
    internal class EmptyInterceptor : IInterceptor
    {
        #region IInterceptor Members

        /// <summary>
        /// Called when an entity is deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        public virtual void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
        }

        /// <summary>
        /// Called when a dirty entity is flushed, which implies an update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="currentState"></param>
        /// <param name="previousState"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public virtual bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return false;
        }

        /// <summary>
        /// Called when a new entity is saved for the first time
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public virtual bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return false;

        }

        public virtual int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return null;
        }

        public virtual object Instantiate(Type type, object id)
        {
            return null;
        }

        public virtual object IsUnsaved(object entity)
        {
            return null;
        }

        public virtual void PostFlush(System.Collections.ICollection entities)
        {
        }

        public virtual void PreFlush(System.Collections.ICollection entities)
        {
        }

        public virtual bool OnLoad(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            return false;

        }

        #endregion
    }
}
