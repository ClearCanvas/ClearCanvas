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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;
using NHibernate.Mapping;
using Iesi.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using NHibernate.Type;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
    /// <summary>
    /// Adds DB indexes on foreign key columns to the Hibernate relational model, based on a set of rules described below.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Rules:
    /// 1. For entities, create indexes on all references to other entities and enums.
    /// 2. For collections of values, create indexes only on the reference to the owner.
    /// 3. For many-to-many collection tables, create 2 indexes, each containing both of the columns, but varying which column is
    /// listed first.  This should improve join performance, going in either direction, by allowing the DB to bypass the join table
    /// altogether, using only the index.  This is explained to some degree here http://msdn2.microsoft.com/en-us/library/ms191195.aspx.
    /// </para>
    /// <para>
    /// This class make decisions about which indexes to create based on foreign keys.  Therefore, 
    /// ensure the that <see cref="EnumForeignKeyProcessor"/> and any other processors
    /// that create foreign keys are run prior to this processor.
    /// </para>
    /// </remarks>
    class ForeignKeyIndexProcessor : IndexCreatorBase
    {
        #region Overrides

        public override void Process(Configuration config)
        {
            // Rules:
            // 1. For entities, create indexes on all references to other entities and enums
            foreach (PersistentClass pc in config.ClassMappings)
            {
                CreateIndexes(config, pc.PropertyIterator);
            }

            // 2. For collections of values, create indexes only on the reference to the owner
            // 3. For many-to-many collection tables, create indexes on both columns together, going in both directions??

            foreach (Collection collection in config.CollectionMappings)
            {
                CreateIndexes(config, collection);
            }
        }

        #endregion

		private void CreateIndexes(Configuration config, Collection collection)
        {
            if(collection.Element is ManyToOne)
            {
                // many-to-many collection

                // collect all columns that participate in foreign keys
                HybridSet columns = new HybridSet();
                foreach (ForeignKey fk in collection.CollectionTable.ForeignKeyIterator)
                {
                    CollectionUtils.ForEach(fk.ColumnIterator, 
                        delegate(Column col)
                            {
                                columns.Add(col);
                            });
                }

                // there should always be exactly 2 "foreign key' columns in a many-many join table, AFAIK
                if (columns.Count != 2)
                {
                    throw new Exception("SNAFU");
                }

                List<Column> indexColumns = new List<Column>(new TypeSafeEnumerableWrapper<Column>(columns));

                // create two indexes, each containing both columns, going in both directions
                CreateIndex(collection.CollectionTable, indexColumns);

                indexColumns.Reverse();
                CreateIndex(collection.CollectionTable, indexColumns);
            }
            else
            {
                // this is a value collection, or a one-to-many collection

                // find the foreign-key that refers back to the owner table (assume there is only one of these - is this always true??)
                ForeignKey foreignKey = CollectionUtils.SelectFirst<ForeignKey>(collection.CollectionTable.ForeignKeyIterator,
                    delegate (ForeignKey fk) { return Equals(fk.ReferencedTable, collection.Table); });

                // create an index on all columns in this foreign key
                if(foreignKey != null)
                {
                    CreateIndex(collection.CollectionTable, new TypeSafeEnumerableWrapper<Column>(foreignKey.ColumnIterator));
                }
            }
        }

		private void CreateIndexes(Configuration config, IEnumerable properties)
        {
            foreach (Property prop in properties)
            {
                if (prop.Value is Component)
                {
                    // recur on component properties
                    Component comp = (Component) prop.Value;
                    CreateIndexes(config, comp.PropertyIterator);
                }
                else
                {
                    // is this property mapped with an EnumHbm class, or is it a many-to-one??
                    if (prop.Type is EnumStringType || prop.Type is ManyToOneType)
                    {
                        // index this column
                        Table indexedTable = prop.Value.Table;
                        Column indexedColumn = CollectionUtils.FirstElement<Column>(prop.ColumnIterator);
                        CreateIndex(indexedTable, new Column[] { indexedColumn });
                    }
                }
            }
        }
    }
}
