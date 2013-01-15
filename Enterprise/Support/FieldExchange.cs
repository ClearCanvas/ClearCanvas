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
using ClearCanvas.Enterprise;
using System.Collections;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Support
{
    public abstract class FieldExchange : IFieldExchange
    {
        private GetFieldValueDelegate _classFieldGetter;
        private SetFieldValueDelegate _classFieldSetter;
        private GetFieldValueDelegate _infoFieldGetter;
        private SetFieldValueDelegate _infoFieldSetter;

        public FieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter)
        {
            _classFieldGetter = classFieldGetter;
            _classFieldSetter = classFieldSetter;
            _infoFieldGetter = infoFieldGetter;
            _infoFieldSetter = infoFieldSetter;
        }

        public abstract void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx);
        public abstract void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx);

        protected object GetInfoFieldValue(DomainObjectInfo info)
        {
            return _infoFieldGetter(info);
        }

        protected object GetClassFieldValue(DomainObject pobj)
        {
            return _classFieldGetter(pobj);
        }

        protected void SetInfoFieldValue(DomainObjectInfo info, object value)
        {
            _infoFieldSetter(info, value);
        }

        protected void SetClassFieldValue(DomainObject pobj, object value)
        {
            _classFieldSetter(pobj, value);
        }
    }
}
