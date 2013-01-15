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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Helper class used by <see cref="UpdateContextInterceptor"/> to track changes to entities during usage
	/// of an update context.
	/// </summary>
	internal class ChangeTracker
	{

		// keeps a record of changes made to entities
		// a given entity instance will only appear once in this list
		// (a dictionary is not used because we cannot control the implemention of the entity's Equals/GetHashCode methods,
		// and we want to deal strictly with reference equality here)
		private readonly List<ChangeRecord> _changeRecords = new List<ChangeRecord>();

		/// <summary>
		/// Records that the specified change occured to the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="changeType"></param>
		/// <param name="propertyDiffs"></param>
		internal void RecordChange(Entity entity, EntityChangeType changeType, PropertyDiff[] propertyDiffs)
		{
			var thisChange = new ChangeRecord(entity, changeType, propertyDiffs);

			// check if this entity was already recorded in the list
			// note that reference equality is used because we do not know how the Equals method of the entity may be implemented
			// for example, it may define equality based on a property whose value has changed, which would break this logic
			var prevRecordIndex = _changeRecords.FindIndex(r => ReferenceEquals(r.Entity, entity));
			if (prevRecordIndex > -1)
			{
				// this entity was already marked as changed
				var previousChange = _changeRecords[prevRecordIndex];

				// compound the previous change record with this one
				_changeRecords[prevRecordIndex] = thisChange.Compound(previousChange);
			}
			else
			{
				// record this change in the change set
				_changeRecords.Add(thisChange);
			}
		}

		/// <summary>
		/// Clears all change records.
		/// </summary>
		internal void Clear()
		{
			_changeRecords.Clear();
		}

		/// <summary>
		/// Gets the set of <see cref="EntityChange"/> objects representing the cumulative changes made.
		/// </summary>
		internal EntityChange[] EntityChangeSet
		{
			get
			{
				return CollectionUtils.Map(_changeRecords, (ChangeRecord c) => c.AsEntityChange()).ToArray();
			}
		}

		/// <summary>
		/// Get the set of changed entities matching the specified filter.
		/// </summary>
		internal IList<Entity> GetChangedEntities(Predicate<EntityChangeType> filter)
		{
			var filtered = CollectionUtils.Select(_changeRecords, record => filter(record.ChangeType));
			return CollectionUtils.Map(filtered, (ChangeRecord cr) => cr.Entity);
		}
	}
}
