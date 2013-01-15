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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a foreign key in a relational database model.
	/// </summary>
	[DataContract]
	public class ForeignKeyInfo : ConstraintInfo
	{
		private string _referencedTable;
		private List<string> _referencedColumns;

		public ForeignKeyInfo()
		{

		}

		internal ForeignKeyInfo(Table table, ForeignKey fk, Configuration config)
			: base("FK_", table.Name, fk.ColumnIterator, null)
		{
			//note: the fk object has a ReferencedTable property, but it doesn't always seem to be set
			//the reference class property is always set, so we use it instead to get the referenced table 
			Table referencedTable = config.GetClassMapping(fk.ReferencedEntityName).Table;
			_referencedTable = referencedTable.Name;
			_referencedColumns = CollectionUtils.Map<Column, string>(
                referencedTable.PrimaryKey.ColumnIterator,
				delegate(Column column) { return column.Name; });
		}

		/// <summary>
		/// Gets the name of the referenced (foreign) table.
		/// </summary>
		[DataMember]
		public string ReferencedTable
		{
			get { return _referencedTable; }
			private set { _referencedTable = value; }
		}

		/// <summary>
		/// Gets the names of the referenced (foreign) columns.
		/// </summary>
		[DataMember]
		public List<string> ReferencedColumns
		{
			get { return _referencedColumns; }
			private set { _referencedColumns = value; }
		}

		/// <summary>
		/// Returns true if this constraint matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Matches(ForeignKeyInfo that)
		{
			return this.ReferencedTable == that.ReferencedTable
				&& CollectionUtils.Equal<string>(this.ReferencedColumns, that.ReferencedColumns, false)
				&& base.Matches(that);
		}

		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
		public override string Identity
		{
			get
			{
				// note that the identity is based entirely on the column names, not the name of the constraint
				// the column names are sorted because we want the identity to be independent of column ordering
				return this.ReferencedTable
					+ StringUtilities.Combine(CollectionUtils.Sort(this.ReferencedColumns), "")
					+ base.Identity;
			}
		}
	}
}
