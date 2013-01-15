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
using System.Collections;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Support
{
    public class CollectionFieldExchange<TInfoElement> : FieldExchange
    {
        private IInfoExchange _elementConversion;

        public CollectionFieldExchange(
            GetFieldValueDelegate classFieldGetter,
            SetFieldValueDelegate classFieldSetter,
            GetFieldValueDelegate infoFieldGetter,
            SetFieldValueDelegate infoFieldSetter,
            IInfoExchange elementConversion)
            : base(classFieldGetter, classFieldSetter, infoFieldGetter, infoFieldSetter)
        {
            _elementConversion = elementConversion;
        }

        public override void SetInfoFieldFromObject(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            IEnumerable pobjCollection = (IEnumerable)GetClassFieldValue(pobj);
            if (pobjCollection != null && pctx.IsCollectionLoaded(pobjCollection))
            {
                List<TInfoElement> infoCollection = new List<TInfoElement>();
                foreach (object element in pobjCollection)
                {
                    infoCollection.Add((TInfoElement)_elementConversion.GetInfoFromObject(element, pctx));
                }
                SetInfoFieldValue(info, infoCollection);
            }
        }

        public override void SetObjectFieldFromInfo(DomainObject pobj, DomainObjectInfo info, IPersistenceContext pctx)
        {
            IList infoCollection = (IList)GetInfoFieldValue(info);
            if (infoCollection != null)
            {
                IEnumerable pobjCollection = (IEnumerable)GetClassFieldValue(pobj);
                foreach (object element in infoCollection)
                {
                    if (pobjCollection is IList)
                    {
                        (pobjCollection as IList).Add(_elementConversion.GetObjectFromInfo(element, pctx));
                    }
                    else if (pobjCollection is ISet)
                    {
                        (pobjCollection as ISet).Add(_elementConversion.GetObjectFromInfo(element, pctx));
                    }
                }
            }
        }
    }
}
