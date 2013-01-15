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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Used by <see cref="ChangeTracker"/> to record changes to entities.
	/// </summary>
	internal class ChangeRecord
	{
		private readonly Entity _entity;
		private readonly EntityChangeType _changeType;
		private readonly PropertyDiff[] _propertyDiffs;

		public ChangeRecord(Entity entity, EntityChangeType changeType, PropertyDiff[] propertyDiffs)
		{
			_entity = entity;
			_changeType = changeType;
			_propertyDiffs = propertyDiffs;
		}

		public Entity Entity
		{
			get { return _entity; }
		}

		public EntityChangeType ChangeType
		{
			get { return _changeType; }
		}

		public PropertyDiff[] PropertyDiffs
		{
			get { return _propertyDiffs; }
		}

		public EntityChange AsEntityChange()
		{
			var propertyChanges = CollectionUtils.Map(GetRelevantPropertyDiffs(), (PropertyDiff diff) => diff.AsPropertyChange());
			return new EntityChange(_entity.GetRef(), _changeType, propertyChanges.ToArray());
		}

		/// <summary>
		/// Returns a new change record that represents the total of this change
		/// and the previous change.
		/// </summary>
		/// <param name="previousChange"></param>
		/// <returns></returns>
		public ChangeRecord Compound(ChangeRecord previousChange)
		{
			// assume the propertyDiffs array in both objects is aligned
			var resultDiffs = new PropertyDiff[_propertyDiffs.Length];
			for (var i = 0; i < _propertyDiffs.Length; i++)
			{
				resultDiffs[i] = _propertyDiffs[i].Compound(previousChange.PropertyDiffs[i]);
			}

			// return a new change record that represents the accumulation of both changes
			// the resultant ChangeType depends on whether this change Supercedes previousChange, or vice versa
			return new ChangeRecord(_entity, Supercedes(previousChange) ? _changeType : previousChange._changeType, resultDiffs);
		}

		/// <summary>
		/// Checks whether this change supercedes the specified other change.  This change supercedes other iff
		/// the <see cref="ChangeType"/> of this change is greater than the <see cref="ChangeType"/> of the other.
		/// </summary>
		/// <remarks>
		/// The <see cref="EntityChangeType.Create"/> value supercedes <see cref="EntityChangeType.Update"/>, and 
		/// <see cref="EntityChangeType.Delete"/> supercedes both.  In other words, a Create followed by an update
		/// is fundamentally a Create, and a Create or Update followed by a Delete is fundamentally a Delete.
		/// </remarks>
		/// <param name="other"></param>
		/// <returns></returns>
		private bool Supercedes(ChangeRecord other)
		{
			return _changeType > other.ChangeType;
		}

		/// <summary>
		/// Gets only the property diffs that are relevant to this change type.
		/// </summary>
		/// <returns></returns>
		private List<PropertyDiff> GetRelevantPropertyDiffs()
		{
			// due to NH quirkyness, the Version property is not updated until the actual flush occurs,
			// so the old value and new value are identical
			// for this reason, we just exclude it here so as not to cause downstream confusion
			// and arguably "Version" is not a *real* property in any case
			var propDiffsExcludingVersion = CollectionUtils.Select(_propertyDiffs, p => p.PropertyName != "Version");

			switch (_changeType)
			{
				// for creates, include all properties
				case EntityChangeType.Create:
					return propDiffsExcludingVersion;

				// for updates, include only the properties that have actually changed
				case EntityChangeType.Update:
					return CollectionUtils.Select(propDiffsExcludingVersion, diff => diff.IsChanged);

				// for deletes, include all properties but
				// exclude collection properties (NH seems to complain when attmepting to lazy load collection after parent entity has been deleted)
				case EntityChangeType.Delete:
					return CollectionUtils.Select(propDiffsExcludingVersion, diff => !diff.IsCollectionProperty);
				default:
					return new List<PropertyDiff>();

			}
		}
	}
}
