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

namespace ClearCanvas.Enterprise.Support
{
    public class EntityFieldExchange : FieldExchange
    {
        private IInfoExchange _entityConversion;

        public EntityFieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter,
            IInfoExchange entityConversion)
            : base(classFieldGetter, classFieldSetter, infoFieldGetter, infoFieldSetter)
        {
            _entityConversion = entityConversion;
        }

        public override void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            Entity entity = (Entity)GetClassFieldValue(pobj);
            if (entity != null && pctx.IsProxyLoaded(entity))
            {
                SetInfoFieldValue(info, _entityConversion.GetInfoFromObject(entity, pctx));
            }
        }

        public override void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            EntityInfo entityInfo = (EntityInfo)GetInfoFieldValue(info);
            if (entityInfo != null && entityInfo.GetEntityRef() != null)
            {
                // don't copy any information from the referenced EntityInfo
                // only take its reference
                Entity entity = pctx.Load(entityInfo.GetEntityRef(), EntityLoadFlags.Proxy);    // proxy, no version check!
                SetClassFieldValue(pobj, entity);
            }
        }
    }

}
