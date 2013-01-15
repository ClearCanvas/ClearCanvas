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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implementation of <see cref="IEnumBroker"/>.
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class EnumBroker : Broker, IEnumBroker
    {
        #region IEnumBroker Members

        public IList<EnumValue> Load(Type enumValueClass, bool includeDeactivated)
        {
            return LoadTable<EnumValue>(enumValueClass, includeDeactivated);
        }

        public IList<TEnumValue> Load<TEnumValue>(bool includeDeactivated) where TEnumValue : EnumValue
        {
            return LoadTable<TEnumValue>(typeof(TEnumValue), includeDeactivated);
        }

        public EnumValue Find(Type enumValueClass, string code)
        {
            Platform.CheckForEmptyString(code, "code");
            return this.Context.LoadEnumValue(enumValueClass, code, true);
        }

        public TEnumValue Find<TEnumValue>(string code)
            where TEnumValue : EnumValue
        {
            return (TEnumValue)Find(typeof(TEnumValue), code);
        }

        public EnumValue TryFind(Type enumValueClass, string code)
        {
            Platform.CheckForEmptyString(code, "code");

            EnumValue foundEnumValue = CollectionUtils.SelectFirst(
                Load(enumValueClass, false), 
                delegate(EnumValue enumValue) { return enumValue.Code == code; });

            if(foundEnumValue == null)
                throw new EnumValueNotFoundException(enumValueClass, code, null);

            return foundEnumValue;
        }

        public TEnumValue TryFind<TEnumValue>(string code)
            where TEnumValue : EnumValue
        {
            return (TEnumValue)TryFind(typeof(TEnumValue), code);
        }

        public EnumValue AddValue(Type enumValueClass, string code, string value, string description, float displayOrder, bool deactivated)
        {
            EnumValue ev = (EnumValue)Activator.CreateInstance(enumValueClass, true);
            UpdateValue(ev, code, value, description, displayOrder, deactivated);

            this.Context.Session.Save(ev);

            return ev;
        }

        public EnumValue UpdateValue(Type enumValueClass, string code, string value, string description, float displayOrder, bool deactivated)
        {
            EnumValue ev = this.Context.LoadEnumValue(enumValueClass, code, false); 
            UpdateValue(ev, code, value, description, displayOrder, deactivated);

            return ev;
        }

        public void RemoveValue(Type enumValueClass, string code)
        {
           EnumValue ev = this.Context.LoadEnumValue(enumValueClass, code, true);
           this.Context.Session.Delete(ev);
        }

        #endregion

        private IList<T> LoadTable<T>(Type enumValueClass, bool includeDeactivated)
        {
            HqlQuery q = new HqlQuery(string.Format("from {0}", enumValueClass.FullName));

            // bug : NHibernate does not properly convert property to column name, therefore we use the column name for now
            if (!includeDeactivated)
                q.Conditions.Add(new HqlCondition("Deactivated_ = ?", false));

            // bug : NHibernate does not properly convert property to column name, therefore we use the column name for now
            q.Sorts.Add(new HqlSort("DisplayOrder_", true, 0));

            // caching these queries will be very efficient, because the tables rarely change
            q.Cacheable = true;
            return ExecuteHql<T>(q);
        }

        private void UpdateValue(EnumValue ev, string code, string value, string description, float displayOrder, bool deactivated)
        {
            SetEnumValueProperty(ev, "Code", code);
            SetEnumValueProperty(ev, "Value", value);
            SetEnumValueProperty(ev, "Description", description);
            SetEnumValueProperty(ev, "DisplayOrder", displayOrder);
            SetEnumValueProperty(ev, "Deactivated", deactivated);
        }

        private static void SetEnumValueProperty(EnumValue ev, string property, object value)
        {
            MethodInfo setter = typeof(EnumValue).GetProperty(property).GetSetMethod(true);
            setter.Invoke(ev, new object[] { value });
        }
    }
}
