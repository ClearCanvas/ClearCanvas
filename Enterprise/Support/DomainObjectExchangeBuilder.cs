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
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Collections;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Support
{
    class DomainObjectExchangeBuilder
    {
        public static IList<IFieldExchange> CreateFieldExchangers(Type domainClass, Type infoClass)
        {
            if (infoClass.GetCustomAttributes(typeof(DataContractAttribute), false).Length == 0)
                throw new Exception("Missing DataContract attribute");  //TODO fix this

            List<IFieldExchange> exchanges = new List<IFieldExchange>();
            foreach (PropertyInfo infoClassProperty in infoClass.GetProperties())
            {
                if (infoClassProperty.GetCustomAttributes(typeof(DataMemberAttribute), true).Length > 0)
                {
                    PropertyInfo domainClassProperty = domainClass.GetProperty(infoClassProperty.Name);
                    exchanges.Add(CreateFieldExchange(domainClassProperty, infoClassProperty));
                }
            }
            return exchanges;
        }

        private static IFieldExchange CreateFieldExchange(PropertyInfo domainClassProperty, PropertyInfo infoClassProperty)
        {
            GetFieldValueDelegate domainFieldGetter = CreateGetFieldValueDelegate(domainClassProperty);
            GetFieldValueDelegate infoFieldGetter = CreateGetFieldValueDelegate(infoClassProperty);
            SetFieldValueDelegate domainFieldSetter = CreateSetFieldValueDelegate(domainClassProperty);
            SetFieldValueDelegate infoFieldSetter = CreateSetFieldValueDelegate(infoClassProperty);

            Type fieldType = infoClassProperty.PropertyType;
            if (fieldType.IsSubclassOf(typeof(ValueObjectInfo)))
            {
                IInfoExchange conversion = GetDomainObjectConversion(domainClassProperty.PropertyType, infoClassProperty.PropertyType);
                return new ValueFieldExchange(domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, conversion);
            }
            else if (fieldType.IsSubclassOf(typeof(EntityInfo)))
            {
                IInfoExchange conversion = GetDomainObjectConversion(domainClassProperty.PropertyType, infoClassProperty.PropertyType);
                return new EntityFieldExchange(domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, conversion);
            }
            else if (CollectionUtils.Contains<Type>(fieldType.GetInterfaces(), delegate(Type t) { return t.Equals(typeof(IList)); }))
            {
                Type infoElementType = GetCollectionElementType(fieldType);
                Type domainElementType = GetAssociatedDomainClass(infoElementType);
                IInfoExchange conversion = GetDomainObjectConversion(domainElementType, infoElementType);

                Type typedCollectionFieldExchange = typeof(CollectionFieldExchange<>).MakeGenericType(new Type[] { infoElementType });
                return (IFieldExchange)Activator.CreateInstance(typedCollectionFieldExchange, domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, conversion);
            }
            else
            {
                // assume it is a primitive field
                return new ValueFieldExchange(domainFieldGetter, domainFieldSetter, infoFieldGetter, infoFieldSetter, new PrimitiveTypeInfoExchange());
            }
        }

        private static GetFieldValueDelegate CreateGetFieldValueDelegate(PropertyInfo property)
        {
            return (GetFieldValueDelegate)Delegate.CreateDelegate(typeof(GetFieldValueDelegate), property.GetGetMethod());
        }

        private static SetFieldValueDelegate CreateSetFieldValueDelegate(PropertyInfo property)
        {
            return property.GetSetMethod() == null ? null :
                (SetFieldValueDelegate)Delegate.CreateDelegate(typeof(SetFieldValueDelegate), property.GetSetMethod());
        }

        private static IInfoExchange GetDomainObjectConversion(Type domainClass, Type infoClass)
        {
            Type typedConversion = typeof(DomainObjectInfoExchange<,>).MakeGenericType(new Type[] { domainClass, infoClass });
            return (IInfoExchange)Activator.CreateInstance(typedConversion);
        }

        private static Type GetCollectionElementType(Type collectionType)
        {
            if (!collectionType.IsGenericType)
                throw new Exception("can't determine element type");    // TODO fix this

            Type[] genericArgs = collectionType.GetGenericArguments();
            if(genericArgs.Length == 0)
                throw new Exception("can't determine element type");    // TODO fix this

            return genericArgs[0];
        }

        public static Type GetAssociatedDomainClass(Type infoClass)
        {
            AssociateDomainClassAttribute assocDomainClassAttr = CollectionUtils.FirstElement<AssociateDomainClassAttribute>(
                infoClass.GetCustomAttributes(typeof(AssociateDomainClassAttribute), false));
            if (assocDomainClassAttr == null)
                throw new Exception("Missing assoc domain class attr");     // TODO fix this
            return assocDomainClassAttr.DomainClass;
        }
    }
}
