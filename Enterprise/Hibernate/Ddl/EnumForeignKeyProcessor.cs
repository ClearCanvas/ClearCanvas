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
using NHibernate.Cfg;
using NHibernate.Metadata;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using NHibernate.Mapping;
using ClearCanvas.Common.Utilities;
using System.Collections;
using NHibernate.Type;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Adds foreign-key constraints on hard-enum columns to the Hibernate relational model, since Hibernate does not know
    /// to add these constraints automatically.
    /// </summary>
    class EnumForeignKeyProcessor : IDdlPreProcessor
    {
        public void Process(Configuration config)
        {
            foreach (PersistentClass pc in config.ClassMappings)
            {
                CreateConstraints(config, pc.PropertyIterator);
            }
        }

		private void CreateConstraints(Configuration config, IEnumerable properties)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    // recur on component properties
                    Component comp = prop.Value as Component;
                    CreateConstraints(config, comp.PropertyIterator);
                }
                else if (prop.Value is Collection)
                {
                    // recur on collections-of-values (composite-element)
                    Collection coll = prop.Value as Collection;
                    if (coll.Element is Component)
                    {
                        Component comp = coll.Element as Component;
                        CreateConstraints(config, comp.PropertyIterator);
                    }
                }
                else
                {
                    // is this property mapped with an EnumHbm class???
                    if (prop.Type is EnumStringType)
                    {
                        Type enumClass = GetEnumValueClassForEnumType(prop.Type.ReturnedClass);

						// value may be null in which case there is no foreign key constraint
						if(enumClass != null)
						{
							// build a constraint for this column
							Table constrainedTable = prop.Value.Table;
							Column constrainedColumn = CollectionUtils.FirstElement<Column>(prop.ColumnIterator);
							constrainedTable.CreateForeignKey(null, new Column[] { constrainedColumn }, enumClass.FullName);
						}
                    }
                }
            }
        }

		/// <summary>
		/// Gets the associated EnumValue class, or null if there is no associated class.
		/// </summary>
		/// <param name="enumType"></param>
		/// <returns></returns>
        private static Type GetEnumValueClassForEnumType(Type enumType)
        {
            EnumValueClassAttribute attr = CollectionUtils.FirstElement<EnumValueClassAttribute>(
                enumType.GetCustomAttributes(typeof(EnumValueClassAttribute), false));

            return attr == null ? null : attr.EnumValueClass;
        }

        private Table GetTableForEnumClass(Type enumClass, PersistentStore store)
        {
            PersistentClass pclass = CollectionUtils.SelectFirst<PersistentClass>(store.Configuration.ClassMappings,
                delegate(PersistentClass c) { return c.MappedClass == enumClass; });

            if (pclass == null)
                throw new Exception(string.Format("{0} is not a persistent class", enumClass.FullName));

            return pclass.Table;
        }

        #region unused

        private void Write(string text, int depth)
        {
            string tabs = "";
            for (int i = 0; i < depth; i++) tabs += "\t";
            Console.WriteLine(tabs + text);
        }

        private void WriteProperties(IEnumerable properties, int depth)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    Write(prop.Name, depth);
                    Component comp = prop.Value as Component;
                    WriteProperties(comp.PropertyIterator, depth + 1);
                }
                else if (prop.Value is Collection)
                {
                    Write(prop.Name, depth);
                    Collection coll = prop.Value as Collection;
                    if (coll.Element is Component)
                    {
                        Component comp = coll.Element as Component;
                        WriteProperties(comp.PropertyIterator, depth + 1);
                    }
                }
                else
                {
                    if (prop.Type is EnumStringType)
                    {
                        Write(prop.Name, depth);
                        foreach (Column col in prop.ColumnIterator)
                        {
                            Write(prop.Value.Table.Name + "." + col.Name + ": " + prop.Type.ReturnedClass.FullName, depth + 1);
                        }
                    }
                    else
                    {
                        Write(prop.Name, depth);
                    }
                }
            }
        }
        #endregion
    }
}
