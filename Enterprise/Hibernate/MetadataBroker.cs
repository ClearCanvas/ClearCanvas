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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class MetadataBroker : Broker, IMetadataBroker
    {
        #region IMetadataBroker Members

        public IList<Type> ListEntityClasses()
        {
            return ListPersistentClasses(delegate(Type c) { return typeof(Entity).IsAssignableFrom(c); });
        }

        public IList<Type> ListEnumValueClasses()
        {
            return ListPersistentClasses(delegate(Type c) { return typeof(EnumValue).IsAssignableFrom(c); });
        }

        #endregion

        private IList<Type> ListPersistentClasses(Predicate<Type> filter)
        {
            ICollection<PersistentClass> persistentClasses = CollectionUtils.Select<PersistentClass>(
                this.Context.PersistentStore.Configuration.ClassMappings,
                delegate(PersistentClass c) { return filter(c.MappedClass); });

            return CollectionUtils.Map<PersistentClass, Type, List<Type>>(persistentClasses,
                delegate(PersistentClass pc) { return pc.MappedClass; });
        }
    }
}
