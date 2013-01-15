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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	/// <summary>
	/// Compares two instances of <see cref="RelationalModelInfo"/> to determine the changes that are needed
	/// to transform one to the other.
	/// </summary>
	class RelationalModelComparator
	{
		delegate IEnumerable<RelationalModelChange> ItemProcessor<T>(T item);
		delegate IEnumerable<RelationalModelChange> CompareItemProcessor<T>(T initial, T desired);


		private readonly EnumOptions _enumOption;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="enumOption"></param>
		public RelationalModelComparator(EnumOptions enumOption)
		{
			_enumOption = enumOption;
		}

		/// <summary>
		/// Returns a <see cref="RelationalModelTransform"/> object that describe the changes
		/// required to transform the initial model into the desired model.
		/// </summary>
		/// <param name="initial"></param>
		/// <param name="desired"></param>
		/// <returns></returns>
		public RelationalModelTransform CompareModels(RelationalModelInfo initial, RelationalModelInfo desired)
		{
			var changes = new List<RelationalModelChange>();

			// compare tables
			changes.AddRange(
				CompareSets(initial.Tables, desired.Tables,
							AddTable,
							DropTable,
							CompareTables)
			);

			if (_enumOption != EnumOptions.None)
			{
				// compare enumeration values
				changes.AddRange(
					CompareSets(initial.Enumerations, desired.Enumerations,
								item => AddEnumeration(desired.GetTable(item.Table), item),
								item => DropEnumeration(initial.GetTable(item.Table), item),
								(x, y) => CompareEnumerations(desired.GetTable(y.Table), x, y))
				);
			}

			return new RelationalModelTransform(changes);
		}


		private static IEnumerable<RelationalModelChange> AddTable(TableInfo t)
		{
			var changes = new List<RelationalModelChange> { new AddTableChange(t) };
			changes.AddRange(
				CollectionUtils.Map<IndexInfo, RelationalModelChange>(t.Indexes, item => new AddIndexChange(t, item)));
			changes.AddRange(
				CollectionUtils.Map<ConstraintInfo, RelationalModelChange>(t.UniqueKeys, item => new AddUniqueConstraintChange(t, item)));
			changes.AddRange(
				CollectionUtils.Map<ForeignKeyInfo, RelationalModelChange>(t.ForeignKeys, item => new AddForeignKeyChange(t, item)));
			changes.AddRange(
				CollectionUtils.Map<ConstraintInfo, RelationalModelChange>(GetPrimaryKey(t), item => new AddPrimaryKeyChange(t, item)));
			return changes;
		}

		private static IEnumerable<RelationalModelChange> DropTable(TableInfo t)
		{
			// dropping the table will automatically drop all of its indexes and constraints
			return new List<RelationalModelChange> {new DropTableChange(t)};
		}

		private static IEnumerable<RelationalModelChange> CompareTables(TableInfo initial, TableInfo desired)
		{
			var table = desired;

			var changes = new List<RelationalModelChange>();
			changes.AddRange(
				CompareSets(initial.Columns, desired.Columns,
							item => AddColumn(table, item),
							item => DropColumn(table, item),
							(x, y) => CompareColumns(table, x, y)));

			changes.AddRange(
				CompareSets(initial.Indexes, desired.Indexes,
							item => AddIndex(table, item),
							item => DropIndex(table, item),
							(x, y) => CompareIndexes(table, x, y)));

			changes.AddRange(
				CompareSets(initial.ForeignKeys, desired.ForeignKeys,
							item => AddForeignKey(table, item),
							item => DropForeignKey(table, item),
							(x, y) => CompareForeignKeys(table, x, y)));

			changes.AddRange(
				CompareSets(initial.UniqueKeys, desired.UniqueKeys,
							item => AddUniqueConstraint(table, item),
							item => DropUniqueConstraint(table, item),
							(x, y) => CompareUniqueConstraints(table, x, y)));

			changes.AddRange(
				CompareSets(GetPrimaryKey(initial), GetPrimaryKey(desired),
							item => AddPrimaryKey(table, item),
							item => DropPrimaryKey(table, item),
							(x, y) => ComparePrimaryKeys(table, x, y)));

			return changes;
		}


		private static IEnumerable<RelationalModelChange> AddColumn(TableInfo table, ColumnInfo c)
		{
			return new RelationalModelChange[] { new AddColumnChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> DropColumn(TableInfo table, ColumnInfo c)
		{
			return new RelationalModelChange[] { new DropColumnChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> CompareColumns(TableInfo table, ColumnInfo initial, ColumnInfo desired)
		{
			var changes = new List<RelationalModelChange>();
			if (!initial.Matches(desired))
				changes.Add(new ModifyColumnChange(table, initial, desired));
			return changes;
		}

		private static IEnumerable<RelationalModelChange> AddIndex(TableInfo table, IndexInfo c)
		{
			return new RelationalModelChange[] { new AddIndexChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> DropIndex(TableInfo table, IndexInfo c)
		{
			return new RelationalModelChange[] { new DropIndexChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> CompareIndexes(TableInfo table, IndexInfo initial, IndexInfo desired)
		{
			var changes = new List<RelationalModelChange>();
			// TODO can indexes be altered or do they need to be dropped and recreated?
			return changes;
		}

		private static IEnumerable<RelationalModelChange> AddForeignKey(TableInfo table, ForeignKeyInfo c)
		{
			return new RelationalModelChange[] { new AddForeignKeyChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> DropForeignKey(TableInfo table, ForeignKeyInfo c)
		{
			return new RelationalModelChange[] { new DropForeignKeyChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> CompareForeignKeys(TableInfo table, ForeignKeyInfo initial, ForeignKeyInfo desired)
		{
			var changes = new List<RelationalModelChange>();
			// TODO can foreign keys be altered or do they need to be dropped and recreated?
			return changes;
		}

		private static IEnumerable<RelationalModelChange> AddUniqueConstraint(TableInfo table, ConstraintInfo c)
		{
			return new RelationalModelChange[] { new AddUniqueConstraintChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> DropUniqueConstraint(TableInfo table, ConstraintInfo c)
		{
			return new RelationalModelChange[] { new DropUniqueConstraintChange(table, c) };
		}

		private static IEnumerable<RelationalModelChange> CompareUniqueConstraints(TableInfo table, ConstraintInfo initial, ConstraintInfo desired)
		{
			var changes = new List<RelationalModelChange>();
			// TODO can constraints be altered or do they need to be dropped and recreated?
			return changes;
		}

		private static IEnumerable<RelationalModelChange> AddPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			return new RelationalModelChange[] { new AddPrimaryKeyChange(table, item) };
		}

		private static IEnumerable<RelationalModelChange> DropPrimaryKey(TableInfo table, ConstraintInfo item)
		{
			return new RelationalModelChange[] { new DropPrimaryKeyChange(table, item) };
		}

		private static IEnumerable<RelationalModelChange> ComparePrimaryKeys(TableInfo table, ConstraintInfo initial, ConstraintInfo desired)
		{
			var changes = new List<RelationalModelChange>();
			// TODO can constraints be altered or do they need to be dropped and recreated?
			return changes;
		}

		private IEnumerable<RelationalModelChange> AddEnumeration(TableInfo table, EnumerationInfo item)
		{
			// check enum options to determine if this item should be considered
			if (_enumOption == EnumOptions.All || (_enumOption == EnumOptions.Hard && item.IsHard))
			{
				return CollectionUtils.Map<EnumerationMemberInfo, RelationalModelChange>(
					item.Members,
					member => new AddEnumValueChange(table, member));
			}

			// nothing to do 
			return new RelationalModelChange[] { };
		}

		private static IEnumerable<RelationalModelChange> DropEnumeration(TableInfo table, EnumerationInfo item)
		{
			// nothing to do - the table will be dropped
			return new RelationalModelChange[] { };
		}

		private IEnumerable<RelationalModelChange> CompareEnumerations(TableInfo table, EnumerationInfo initial, EnumerationInfo desired)
		{
			// note: for soft enumerations, we don't do any updates, because they may have been customized already
			// hence only hard enums should ever be compared (need to ensure what is in the database matches the C# enum definition)
			if (_enumOption != EnumOptions.None && desired.IsHard)
			{
				return CompareSets(initial.Members, desired.Members,
									item => AddEnumerationValue(table, item),
									item => DropEnumerationValue(table, item),
									(x, y) => CompareEnumerationValues(table, x, y));
			}

			// nothing to do
			return new RelationalModelChange[] { };
		}

		private static IEnumerable<RelationalModelChange> AddEnumerationValue(TableInfo table, EnumerationMemberInfo item)
		{
			return new RelationalModelChange[] { new AddEnumValueChange(table, item) };
		}

		private static IEnumerable<RelationalModelChange> DropEnumerationValue(TableInfo table, EnumerationMemberInfo item)
		{
			return new RelationalModelChange[] { new DropEnumValueChange(table, item) };
		}

		private static IEnumerable<RelationalModelChange> CompareEnumerationValues(TableInfo table, EnumerationMemberInfo initial, EnumerationMemberInfo desired)
		{
			// nothing to do - once a value is populated, we do not update it, because it may have been customized
			return new RelationalModelChange[] { };
		}

		/// <summary>
		/// Compares the initial and desired sets of items, forwarding items to the appropriate
		/// callback and returning the aggregate set of results.
		/// </summary>
		/// <remarks>
		/// Items that appear in <paramref name="desired"/> but not in <paramref name="initial"/>
		/// are passed to <paramref name="addProcessor"/>.
		/// Items that appear in <paramref name="initial"/> but not in <paramref name="desired"/>
		/// are passed to <paramref name="dropProcessor"/>.
		/// Items that appear in both sets are passed to <paramref name="compareProcessor"/> for
		/// comparison.
		/// Results of all callbacks are aggregated and returned.
		/// </remarks>
		private static IEnumerable<RelationalModelChange> CompareSets<T>(IEnumerable<T> initial, IEnumerable<T> desired,
												   ItemProcessor<T> addProcessor,
												   ItemProcessor<T> dropProcessor,
												   CompareItemProcessor<T> compareProcessor)
			where T : ElementInfo
		{
			var changes = new List<RelationalModelChange>();

			// partition desired set into those items that are contained in the initial set (true) and
			// those that are not (false)
			var a = CollectionUtils.GroupBy(desired, x => CollectionUtils.Contains(initial, y => Equals(x, y)));

			// partition initial set into those items that are contained in the desired set (true) and
			// those that are not (false)
			var b = CollectionUtils.GroupBy(initial, x => CollectionUtils.Contains(desired, y => Equals(x, y)));

			// these items need to be added
			List<T> adds;
			if (a.TryGetValue(false, out adds))
			{
				foreach (var add in adds)
					changes.AddRange(addProcessor(add));
			}

			// these items need to be dropped
			List<T> drops;
			if (b.TryGetValue(false, out drops))
			{
				foreach (var drop in drops)
					changes.AddRange(dropProcessor(drop));
			}

			// these items exist in both sets, so they need to be compared one by one
			// these keys should either both exist, or both not exist
			var desiredCommon = a.ContainsKey(true) ? a[true] : new List<T>();
			var initialCommon = b.ContainsKey(true) ? b[true] : new List<T>();

			// sort these vectors by identity, so that they are aligned
			desiredCommon.Sort((x, y) => x.Identity.CompareTo(y.Identity));
			initialCommon.Sort((x, y) => x.Identity.CompareTo(y.Identity));

			// compare each of the common items
			for (var i = 0; i < initialCommon.Count; i++)
			{
				changes.AddRange(compareProcessor(initialCommon[i], desiredCommon[i]));
			}

			return changes;
		}

		private static ConstraintInfo[] GetPrimaryKey(TableInfo table)
		{
			return table.PrimaryKey == null ? new ConstraintInfo[0] : new[] { table.PrimaryKey };
		}
	}
}