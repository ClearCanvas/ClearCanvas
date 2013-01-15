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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	internal class RenderOptions
	{
		public RenderOptions()
		{
		}

		public RenderOptions(RelationalSchemaOptions options)
		{
			this.SuppressForeignKeys = options.SuppressForeignKeys;
			this.SuppressIndexes = options.SuppressIndexes;
			this.SuppressPrimaryKeys = options.SuppressPrimaryKeys;
			this.SuppressUniqueConstraints = options.SuppressUniqueConstraints;
		}

		public bool SuppressForeignKeys { get; set; }
		public bool SuppressUniqueConstraints { get; set; }
		public bool SuppressIndexes { get; set; }
		public bool SuppressPrimaryKeys { get; set; }
	}

	/// <summary>
	/// Describes a set of changes that transform a relational model.
	/// </summary>
	class RelationalModelTransform
	{
		private static readonly Dictionary<Type, int> _changeOrder = new Dictionary<Type, int>();

		/// <summary>
		/// Class constructor
		/// </summary>
		static RelationalModelTransform()
		{
			// define the order that changes should occur to avoid dependency issues
			_changeOrder.Add(typeof(DropIndexChange), 0);
			_changeOrder.Add(typeof(DropForeignKeyChange), 1);
			_changeOrder.Add(typeof(DropUniqueConstraintChange), 2);
			_changeOrder.Add(typeof(DropPrimaryKeyChange), 3);
			_changeOrder.Add(typeof(DropTableChange), 4);
			_changeOrder.Add(typeof(DropColumnChange), 5);
			_changeOrder.Add(typeof(ModifyColumnChange), 6);
			_changeOrder.Add(typeof(AddColumnChange), 7);
			_changeOrder.Add(typeof(AddTableChange), 8);
			_changeOrder.Add(typeof(AddPrimaryKeyChange), 9);
			_changeOrder.Add(typeof(AddUniqueConstraintChange), 10);
			_changeOrder.Add(typeof(AddForeignKeyChange), 11);
			_changeOrder.Add(typeof(AddIndexChange), 12);
			_changeOrder.Add(typeof(DropEnumValueChange), 13);
			_changeOrder.Add(typeof(AddEnumValueChange), 14);
		}

		private readonly List<RelationalModelChange> _changes;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="changes"></param>
		internal RelationalModelTransform(List<RelationalModelChange> changes)
		{
			_changes = changes;
		}

		/// <summary>
		/// Uses the specified renderer to render this transform.
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Statement[] Render(IRenderer renderer, RenderOptions options)
		{
			// filter changes according to options
			var filteredChanges = CollectionUtils.Select(_changes, change => FilterChange(change, options));

			// allow the renderer to modify the change set, and then sort the changes appropriately
			filteredChanges = OrderChanges(renderer.PreFilter(filteredChanges));

			var statements = new List<Statement>();
			foreach (var change in filteredChanges)
			{
				statements.AddRange(change.GetStatements(renderer));
			}
			return statements.ToArray();
		}

		/// <summary>
		/// Determines whether specified change should be included in rendering based on specified options.
		/// </summary>
		/// <param name="change"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		private static bool FilterChange(RelationalModelChange change, RenderOptions options)
		{
			if (options.SuppressForeignKeys)
			{
				if (change is AddForeignKeyChange || change is DropForeignKeyChange)
					return false;
			}
			if (options.SuppressIndexes)
			{
				if (change is AddIndexChange || change is DropIndexChange)
					return false;
			}
			if (options.SuppressUniqueConstraints)
			{
				if (change is AddUniqueConstraintChange || change is DropUniqueConstraintChange)
					return false;
			}
			if (options.SuppressPrimaryKeys)
			{
				if (change is AddPrimaryKeyChange || change is DropPrimaryKeyChange)
					return false;
			}
			return true;
		}

		private static List<RelationalModelChange> OrderChanges(IEnumerable<RelationalModelChange> changes)
		{
			// the algorithm here tries to do 2 things:
			// 1. Re-organize groups of changes so as to avoid any dependency problems.
			// 2. Preserve the order of changes as much as possible, not re-ordering anything
			// that doesn't need to be re-ordered to satisfy 1.  This *should* keep changes pertaining to the
			// same table clustered together where possible, and also keep AddEnumValueChanges in order

			// group changes by type
			IDictionary<Type, List<RelationalModelChange>> groupedByType = CollectionUtils.GroupBy(changes, c => c.GetType());

			// sort the types to avoid dependency issues
			var sortedTypes = CollectionUtils.Sort(groupedByType.Keys, (x, y) => _changeOrder[x].CompareTo(_changeOrder[y]));


			// flatten changes back into a single list
			return CollectionUtils.Concat<RelationalModelChange>(
					CollectionUtils.Map(sortedTypes, (Type t) => groupedByType[t]).ToArray());
		}
	}
}
