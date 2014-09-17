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
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class BaseAdaptor<TServerEntity, TIEntity, TCriteria, TColumns>
        where TServerEntity : ServerEntity, new()
        where TCriteria : EntitySelectCriteria, new()
        where TColumns : EntityUpdateColumns
        where TIEntity : IEntityBroker<TServerEntity, TCriteria, TColumns>
    {
        #region Private Members

        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

        #endregion Private Members

        #region Protected Properties

        protected IPersistentStore PersistentStore
        {
            get { return _store; }
        }

        #endregion


        protected virtual void OnQuerying(IPersistenceContext context, TCriteria criteria)
        {
            
        }

        #region Public Methods

        public IList<TServerEntity> Get()
        {
			return Get(HttpContext.Current.GetSharedPersistentContext());
        }
		public IList<TServerEntity> Get(IPersistenceContext context)
		{
		    TIEntity find = context.GetBroker<TIEntity>();
			TCriteria criteria = new TCriteria();

            OnQuerying(context, criteria);
			
            IList<TServerEntity> list = find.Find(criteria);

			return list;		
		}


        public TServerEntity Get(ServerEntityKey key)
		{
			return Get(HttpContext.Current.GetSharedPersistentContext(), key);
		}

		public TServerEntity Get(IPersistenceContext context, ServerEntityKey key)
		{
            //TODO: Add data access filter ?
		    TIEntity select = context.GetBroker<TIEntity>();
			return select.Load(key);
		}

    	public IList<TServerEntity> Get(TCriteria criteria)
        {
            return Get(HttpContext.Current.GetSharedPersistentContext(), criteria);
        }

		public IList<TServerEntity> Get(IPersistenceContext context, TCriteria criteria)
		{
            OnQuerying(context, criteria);
			TIEntity select = context.GetBroker<TIEntity>();
				return select.Find(criteria);

		}
		public IList<TServerEntity> GetRange(TCriteria criteria, int startIndex, int maxRows)
		{
            return GetRange(HttpContext.Current.GetSharedPersistentContext(), criteria, startIndex, maxRows);
		}
		public IList<TServerEntity> GetRange(IPersistenceContext context, TCriteria criteria, int startIndex, int maxRows)
		{
		    OnQuerying(context, criteria);
			TIEntity select = context.GetBroker<TIEntity>();

            // SQL row index starts from 1
		    int fromRowIndex = startIndex + 1;
            return select.Find(criteria, fromRowIndex, maxRows);
		}

    	public int GetCount(TCriteria criteria)
		{
    	    OnQuerying(HttpContext.Current.GetSharedPersistentContext(), criteria);
            TIEntity select = HttpContext.Current.GetSharedPersistentContext().GetBroker<TIEntity>();
			return select.Count(criteria);
		}

		public TServerEntity GetFirst(TCriteria criteria)
		{
            return GetFirst(HttpContext.Current.GetSharedPersistentContext(), criteria);
		}

		public TServerEntity GetFirst(IPersistenceContext context, TCriteria criteria)
		{
            OnQuerying(context, criteria);
            TIEntity select = context.GetBroker<TIEntity>();
			return select.FindOne(criteria);
		}

        public TServerEntity Add(TColumns param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TServerEntity entity =  Add(context, param);
                    context.Commit();
                    return entity;
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception adding {0}", typeof (TServerEntity));
                throw;
            }
        }

        public TServerEntity Add(IUpdateContext context, TColumns param)
        {
        	TIEntity update = context.GetBroker<TIEntity>();

            TServerEntity newEntity = update.Insert(param);

            return newEntity;
        }


        public bool Update(ServerEntityKey key, TColumns param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    update.Update(key, param);

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof (TServerEntity));
                throw;
            }
        }

		public bool Update(TCriteria criteria, TColumns param)
		{
			try
			{
				using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					TIEntity update = context.GetBroker<TIEntity>();

					var result = update.Update(criteria, param);

					context.Commit();

					return result;
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof(TServerEntity));
				throw;
			}
		}

		public bool Delete(IUpdateContext context, ServerEntityKey key)
		{
			TIEntity update = context.GetBroker<TIEntity>();

			return update.Delete(key);
		}

    	public bool Delete(ServerEntityKey key)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    if (!update.Delete(key))
                        return false;

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof (TServerEntity));
                throw;
            }
        }

		public bool Delete(IUpdateContext context, TCriteria criteria)
		{
			TIEntity update = context.GetBroker<TIEntity>();

			if (update.Delete(criteria) < 0)
				return false;

			return true;
		}

    	public bool Delete(TCriteria criteria)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    if (update.Delete(criteria) < 0)
                        return false;

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof(TServerEntity));
                throw;
            }
        }

        #endregion
    }
}
