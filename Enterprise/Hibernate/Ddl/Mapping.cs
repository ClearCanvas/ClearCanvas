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
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// For internal use.  Copied from NHibernate source.
    /// </summary>
    internal class Mapping : IMapping
    {
        private readonly Configuration configuration;

        public Mapping(Configuration configuration)
        {
            this.configuration = configuration;
        }

        private PersistentClass GetPersistentClass(string className)
        {
            PersistentClass pc = configuration.GetClassMapping(className);
            if (pc == null)
            {
                throw new MappingException("persistent class not known: " + className);
            }
            return pc;
        }

        public IType GetIdentifierType(string className)
        {
            return GetPersistentClass(className).Identifier.Type;
        }

        public string GetIdentifierPropertyName(string className)
        {
            PersistentClass pc = GetPersistentClass(className);
            if (!pc.HasIdentifierProperty)
            {
                return null;
            }
            return pc.IdentifierProperty.Name;
        }

        public IType GetReferencedPropertyType(string className, string propertyName)
        {
            PersistentClass pc = GetPersistentClass(className);
            Property prop = pc.GetProperty(propertyName);

            if (prop == null)
            {
                throw new MappingException("property not known: " + pc.MappedClass.FullName + '.' + propertyName);
            }
            return prop.Type;
        }

        public bool HasNonIdentifierPropertyNamedId(string className)
        {
            return "id".Equals(GetIdentifierPropertyName(className));
        }
    }
}
