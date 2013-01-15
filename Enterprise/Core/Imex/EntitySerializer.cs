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
using System.Xml;
using ClearCanvas.Common.Utilities;
using System.Reflection;
using ClearCanvas.Enterprise.Core.Modelling;
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public class EntitySerializer<TEntity>
        where TEntity : Entity
    {
        #region Public Methods

        public void WriteEntity(TEntity entity, XmlWriter writer)
        {
            writer.WriteStartElement(entity.GetClass().Name);

            // if it's a subclass, rather than an instance of TEntity, then write the class name out as an attribute
            if (entity.GetClass().IsSubclassOf(typeof(TEntity)))
            {
                writer.WriteAttributeString("class", entity.GetClass().FullName);
            }

            WriteObjectProperties(entity, writer);
            writer.WriteEndElement();
        }

        #endregion

        #region Protected Overridables

        protected virtual void WriteProperty(object obj, PropertyInfo property, object value, XmlWriter writer)
        {
            if (IsEntity(property.PropertyType) || IsEntityCollection(property.PropertyType))
            {
                // do nothing ... no general way to write an entity or entity collection
                return;
            }

            WriteSingleValuedProperty(property, value, writer, delegate(object item) { WriteValue(item.GetType(), item, writer); });
        }

        #endregion

        #region Protected Helpers

        protected void WriteSingleValuedProperty(PropertyInfo property, object value, XmlWriter writer, Action<object> writeItemCallback)
        {
            writer.WriteStartElement(property.Name);
            writeItemCallback(value);
            writer.WriteEndElement();
        }

        protected void WriteMultiValuedProperty(PropertyInfo property, object value, XmlWriter writer, Action<object> writeItemCallback)
        {
            writer.WriteStartElement(property.Name);
            WriteCollection(value, writer, writeItemCallback);
            writer.WriteEndElement();
        }

        protected void WriteObjectProperties(object obj, XmlWriter writer)
        {
            ObjectWalker walker = new ObjectWalker(
                delegate(IObjectMemberContext ctx)
                {
                    WriteProperty(ctx.Object, (PropertyInfo)ctx.Member, ctx.MemberValue, writer);
                },
                delegate(MemberInfo member)
                {
                    return AttributeUtils.HasAttribute<PersistentPropertyAttribute>(member);
                });
            walker.IncludePublicFields = false;
            walker.Walk(obj);
        }

        #endregion

        #region Private Methods

        private void WriteCollection(object value, XmlWriter writer, Action<object> writeItemCallback)
        {
            foreach (object item in (IEnumerable)value)
            {
                writer.WriteStartElement("Item");
                writeItemCallback(item);
                writer.WriteEndElement();
            }
        }

        private void WriteValue(Type type, object value, XmlWriter writer)
        {
            if (value == null)
                return;

            if (IsEnum(type))
                WriteEnum(value, writer);
            else if (IsValueObject(type))
                WriteValueObject(value, writer);
            else if (IsCollection(type))    // collection of values
                WriteCollection(value, writer, delegate(object item) { WriteValue(item.GetType(), item, writer); });
            else if (IsPrimitive(type))
                WritePrimitive(value, writer);
            else
                throw new NotSupportedException();
        }

        private bool IsEntity(Type type)
        {
            return typeof(Entity).IsAssignableFrom(type);
        }

        private bool IsValueObject(Type type)
        {
            return typeof(ValueObject).IsAssignableFrom(type);
        }

        private bool IsEnum(Type type)
        {
            return type.IsEnum || typeof(EnumValue).IsAssignableFrom(type);
        }

        private bool IsCollection(Type type)
        {
            bool b = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
            return b;
        }

        private bool IsEntityCollection(Type type)
        {
            return IsCollection(type) && type.GetGenericArguments()[0].IsSubclassOf(typeof(Entity));
        }

        private bool IsPrimitive(Type type)
        {
            return true;
        }

        private void WriteEnum(object value, XmlWriter writer)
        {
            writer.WriteValue(value);
        }

        private void WritePrimitive(object value, XmlWriter writer)
        {
            writer.WriteValue(value);
        }

        private void WriteValueObject(object value, XmlWriter writer)
        {
            WriteObjectProperties(value, writer);
        }

	    #endregion
    }
}
