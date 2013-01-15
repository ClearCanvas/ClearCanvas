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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// This listener synchronizes values in a string property of an entity with a specified enumeration.  It is basically a trigger 
	/// to implement a loose coupling between the entity's property and the enumeration with the result that the enumeration then contains
	/// the distinct set of values for that property.  The enumeration can then be queried for the distinct set of values at a much lower
	/// cost than querying the entities themselves.
	/// </summary>
	/// <typeparam name="TChangedEntityClass"></typeparam>
	/// <typeparam name="TLooseEnumClass"></typeparam>
	public abstract class LooseEnumPropertyChangeSetListener<TChangedEntityClass, TLooseEnumClass> : IEntityChangeSetListener
		where TChangedEntityClass : Entity
		where TLooseEnumClass : EnumValue
	{
		/// <summary>
		/// Returns a string value from the specified entity which should be synchronized with this listener's enumeration class.
		/// </summary>
		/// <param name="changedEntity"></param>
		/// <returns></returns>
		public abstract string GetEnumCodeFromEntity(TChangedEntityClass changedEntity);

		#region Implementation of IEntityChangeSetListener

		public void PreCommit(EntityChangeSetPreCommitArgs args)
		{
			foreach (var entityChange in args.ChangeSet.Changes)
			{
				if (entityChange.ChangeType != EntityChangeType.Create)
					continue;

				if (entityChange.GetEntityClass() != typeof(TChangedEntityClass))
					continue;

				SyncEnumValue(entityChange.EntityRef, args.PersistenceContext);
			}
		}

		public void PostCommit(EntityChangeSetPostCommitArgs args)
		{
		}

		#endregion

		private void SyncEnumValue(EntityRef entityRef, IPersistenceContext persistenceContext)
		{
			var changedEntity = persistenceContext.Load<TChangedEntityClass>(entityRef);
			var enumBroker = persistenceContext.GetBroker<IEnumBroker>();

			var code = GetEnumCodeFromEntity(changedEntity);
			try
			{
				var foo = enumBroker.TryFind(typeof(TLooseEnumClass), code);
			}
			catch (EnumValueNotFoundException)
			{
				AddEnumValue(code, enumBroker, persistenceContext);
			}
		}

		private void AddEnumValue(string code, IEnumBroker enumBroker, IPersistenceContext persistenceContext)
		{
			try
			{
				enumBroker.AddValue(
					typeof(TLooseEnumClass),
					code,
					code,
					string.Empty,
					GetDisplayOrder(enumBroker),
					false);

				persistenceContext.SynchState();
			}
			catch (System.Exception)
			{
				Platform.Log(LogLevel.Error, string.Format("Cannot add code '{0}' to enumeration '{1}'", code, typeof(TLooseEnumClass).Name));
			}
		}

		private float GetDisplayOrder(IEnumBroker enumBroker)
		{
			var lastDisplayed = CollectionUtils.Max(
				enumBroker.Load(typeof(TLooseEnumClass), true), 
				null,
				(enumValue1, enumValue2) => enumValue1.DisplayOrder.CompareTo(enumValue2.DisplayOrder));

			return lastDisplayed != null ? lastDisplayed.DisplayOrder + 1 : 0;
		}
	}
}
