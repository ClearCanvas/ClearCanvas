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
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Describes a constraint in a relational database model.
	/// </summary>
	[DataContract]
	public class ConstraintInfo : ElementInfo
	{
		private string _name;
		private List<string> _columns;


		public ConstraintInfo()
		{

		}

		/// <summary>
		/// Constructor for creating a constraint from a Hibnerate PrimaryKey object.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="constraint"></param>
		internal ConstraintInfo(Table table, PrimaryKey constraint)
			: this("PK_", table.Name, constraint.ColumnIterator, null)
		{
		}

		/// <summary>
		/// Constructor for creating a constraint from a Hibnerate UniqueKey object.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="constraint"></param>
		internal ConstraintInfo(Table table, UniqueKey constraint)
			: this("UQ_", table.Name, constraint.ColumnIterator, constraint.Name)
		{
		}

		/// <summary>
		/// Constructor for creating a unique constraint on a single column that is marked unique.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		internal ConstraintInfo(Table table, Column column)
			: this("UQ_", table.Name, new [] { column }, null)
		{
		}

		/// <summary>
		/// Constructor for creating a unique constraint on a single column that is marked unique.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		internal ConstraintInfo(TableInfo table, ColumnInfo column)
			: this("UQ_", table.Name, new [] { column })
		{
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected ConstraintInfo(string prefix, string table, IEnumerable<Column> columns, string constraintName)
		{
			_name = string.IsNullOrEmpty(constraintName) ? MakeName(prefix, table, columns) : MakeName(prefix, table, constraintName);
			_columns = CollectionUtils.Map(columns, (Column column) => column.Name);
		}

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected ConstraintInfo(string prefix, string table, IEnumerable<ColumnInfo> columns)
		{
			_name = MakeName(prefix, table, columns);
			_columns = CollectionUtils.Map(columns, (ColumnInfo column) => column.Name);
		}

		/// <summary>
		/// Gets the name of the constraint.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			protected set { _name = value; }
		}

		/// <summary>
		/// Gets the names of the columns on which the constraint is defined.
		/// </summary>
		[DataMember]
		public List<string> Columns
		{
			get { return _columns; }
			protected set { _columns = value; }
		}

		/// <summary>
		/// Returns true if this constraint matches that, property for property.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Matches(ConstraintInfo that)
		{
			return this.Name == that.Name &&
				CollectionUtils.Equal<string>(this.Columns, that.Columns, false);
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
				return StringUtilities.Combine(CollectionUtils.Sort(this.Columns), "");
			}
		}
	}
}
