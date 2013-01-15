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
using System.Runtime.Serialization;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Enterprise.Support
{
    public class DomainObjectInfoExchange<TDomainObject, TDomainObjectInfo> : IInfoExchange
        where TDomainObject : DomainObject, new()
        where TDomainObjectInfo : DomainObjectInfo, new()
    {
        private List<IFieldExchange> _fieldExchangers = new List<IFieldExchange>();

        public DomainObjectInfoExchange()
        {
            // validation
            if(!typeof(TDomainObject).Equals(DomainObjectExchangeBuilder.GetAssociatedDomainClass(typeof(TDomainObjectInfo))))
                throw new Exception("Cannot convert between these types");

            // build the conversion
            _fieldExchangers.AddRange(
                DomainObjectExchangeBuilder.CreateFieldExchangers(typeof(TDomainObject), typeof(TDomainObjectInfo)));
        }

        protected IList<IFieldExchange> FieldExchangers
        {
            get { return _fieldExchangers; }
        }

        public TDomainObjectInfo GetInfoFromObject(TDomainObject obj, IPersistenceContext pctx)
        {
            if (obj == null) return null;
            TDomainObjectInfo info = new TDomainObjectInfo();
            foreach (IFieldExchange fe in _fieldExchangers)
            {
                fe.SetInfoFieldFromObject(obj, info, pctx);
            }
            return info;
        }

        public TDomainObject GetObjectFromInfo(TDomainObjectInfo info, IPersistenceContext pctx)
        {
            if (info == null) return null;
            TDomainObject obj = new TDomainObject();
            foreach (IFieldExchange fe in _fieldExchangers)
            {
                fe.SetObjectFieldFromInfo(obj, info, pctx);
            }
            return obj;
        }

        #region IConversion Members

        object IInfoExchange.GetInfoFromObject(object pobj, IPersistenceContext pctx)
        {
            return GetInfoFromObject((TDomainObject)pobj, pctx);
        }

        object IInfoExchange.GetObjectFromInfo(object info, IPersistenceContext pctx)
        {
            return GetObjectFromInfo((TDomainObjectInfo)info, pctx);
        }

        #endregion
    }

}
