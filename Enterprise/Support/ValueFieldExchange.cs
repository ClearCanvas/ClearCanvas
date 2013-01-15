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
    public class ValueFieldExchange : FieldExchange
    {
        private IInfoExchange _valueConversion;

        public ValueFieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter,
            IInfoExchange valueConversion)
            : base(classFieldGetter, classFieldSetter, infoFieldGetter, infoFieldSetter)
        {
            _valueConversion = valueConversion;
        }

        public override void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            SetInfoFieldValue(info, _valueConversion.GetInfoFromObject(GetClassFieldValue(pobj), pctx));
        }

        public override void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            SetClassFieldValue(pobj, _valueConversion.GetObjectFromInfo(GetInfoFieldValue(info), pctx));
        }
    }
}
