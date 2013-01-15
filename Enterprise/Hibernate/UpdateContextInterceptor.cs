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

using NHibernate;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common.Specifications;
using System.Collections;
using ClearCanvas.Common.Utilities;
using NHibernate.Type;
using NHibernate.Collection;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// Implementation of NHibernate IInterceptor, used to record entity change-set for a transaction.
	/// </summary>
	internal class UpdateContextInterceptor : EmptyInterceptor
	{
		private readonly ChangeTracker _fullChangeTracker = new ChangeTracker();
		private readonly List<ChangeTracker> _changeTrackers = new List<ChangeTracker>();
		private readonly Queue<DomainObject> _pendingValidations = new Queue<DomainObject>();
		private readonly UpdateContext _owner;


		internal UpdateContextInterceptor(UpdateContext owner)
		{
			_owner = owner;
			_changeTrackers.Add(_fullChangeTracker);
		}

		/// <summary>
		/// Gets the set of all changes made in this update context.
		/// </summary>
		internal EntityChange[] FullChangeSet
		{
			get { return _fullChangeTracker.EntityChangeSet; }
		}

		internal void AddChangeTracker(ChangeTracker tracker)
		{
			_changeTrackers.Add(tracker);
		}

		internal void RemoveChangeTracker(ChangeTracker tracker)
		{
			_changeTrackers.Remove(tracker);
		}


		#region IInterceptor Members

		/// <summary>
		/// Called when an entity is deleted
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="state"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			// build a list of property diffs
			// the current state is "null" since the entity was deleted
			var propertyDiffs = GetPropertyDiffs(propertyNames, types, null, state);
			RecordChange(entity, EntityChangeType.Delete, propertyDiffs);
		}

		/// <summary>
		/// Called when a dirty entity is flushed, which implies an update to the DB.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="currentState"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			// This method may be called more 
			// than once for a given entity during the lifetime of the update context, and the difference between the 
			// currentState and previousState parameters will reflect only the changes
			// to the entity that have occured since the last time this method was called.

			// build a list of property diffs
			var propertyDiffs = GetPropertyDiffs(propertyNames, types, currentState, previousState);


			// Build a list of dirty properties for validation
			var dirtyProperties = CollectionUtils.Select(propertyDiffs, diff => diff.IsChanged);

			// run low-level validation prior to flush, passing the list of dirty properties 
			// in order to optimize which rules are tested
			// rather than testing every validation rule we can selectively test only those rules
			// that may be affected by the modified state
			var dirtyPropNames = CollectionUtils.Map(dirtyProperties, (PropertyDiff pc) => pc.PropertyName);
			_owner.Validator.ValidateLowLevel((DomainObject)entity, rule => ShouldCheckRule(rule, dirtyPropNames));

			RecordChange(entity, EntityChangeType.Update, propertyDiffs);
			return false;
		}

		/// <summary>
		/// Called when a new entity is added to the persistence context via <see cref="PersistenceContext.Lock(ClearCanvas.Enterprise.Core.Entity)"/> with
		/// <see cref="DirtyState.New"/>.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="state"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			// ignore the addition of log entries (any subclass of LogEntry)
			if (typeof(LogEntry).IsAssignableFrom(NHibernateUtil.GetClass(entity)))
				return false;

			// we could validate the entity here, but it seems rather counter-productive, given
			// that the entity is not actually being written to the DB yet and further changes
			// may be made to it by the application before it is written.  Therefore, we choose
			// not to validate the entity here, but instead put it in a queue to be validated at flush time
			_pendingValidations.Enqueue((DomainObject)entity);

			// build a list of property diffs
			// the previous state is "null" since the entity was just created
			var propertyDiffs = GetPropertyDiffs(propertyNames, types, state, null);
			RecordChange(entity, EntityChangeType.Create, propertyDiffs);
			return false;

		}

		/// <summary>
		/// Called prior to every flush.
		/// </summary>
		/// <param name="entities"></param>
		public override void PreFlush(ICollection entities)
		{
			// validate any transient entities that have not yet been validated
			// note that there is a possibility that NHibernate may do its own validation of new entities
			// prior to arriving here, in which case it will have already thrown an exception
			// this is unfortunate, because the exceptions that we generate are more informative
			// and user-friendly, but there is no obvious solution to this as of NH1.0
			// TODO: NH1.2 added new methods to the Interceptor API - see if any of these will get around this problem
			while (_pendingValidations.Count > 0)
			{
				var domainObject = _pendingValidations.Dequeue();
				_owner.Validator.ValidateLowLevel(domainObject, rule => true);
			}

			base.PreFlush(entities);
		}

		public override void PostFlush(ICollection entities)
		{
			base.PostFlush(entities);
		}

		#endregion

		private void RecordChange(object domainObject, EntityChangeType changeType, PropertyDiff[] propertyDiffs)
		{
			if(IsChangeSetPublishable((DomainObject)domainObject))
			{
				// update all change trackers
				var entity = (Entity)domainObject;
				foreach (var changeTracker in _changeTrackers)
				{
					changeTracker.RecordChange(entity, changeType, propertyDiffs);
				}
			}
		}

		private static bool IsChangeSetPublishable(DomainObject domainObject)
		{
			// ignore changes to enum values for now
			// TODO: should probably record changes to enum values
			if (domainObject is EnumValue)
				return false;

			// check for an attribute - if no attribute, then default is publishable
			var a = AttributeUtils.GetAttribute<PublishInChangeSetsAttribute>(domainObject.GetClass(), true);
			return a == null || a.IsPublishable;
		}

		private static bool ShouldCheckRule(ISpecification rule, ICollection<string> dirtyProperties)
		{
			// if the rule is not bound to specific properties, then it should be checked
			if (!(rule is IPropertyBoundRule))
				return true;

			var pbRule = rule as IPropertyBoundRule;

			// if the rule is bound to a property of an embedded value rather than the entity itself, then return true
			// (the rule won't actually be tested unless the property containing the embedded value is dirty)
			if (CollectionUtils.Contains(pbRule.Properties, p => typeof(ValueObject).IsAssignableFrom(p.DeclaringType)))
				return true;

			// otherwise, we assume the rule is bound to a property of the entity

			// if no properties are dirty, we don't need to check it
			if (dirtyProperties.Count == 0)
				return false;

			// if the rule is bound to any properties that are dirty, return true
			return CollectionUtils.Contains((rule as IPropertyBoundRule).Properties,
											prop => dirtyProperties.Contains(prop.Name));
		}

		private static PropertyDiff[] GetPropertyDiffs(string[] propertyNames, IType[] types, object[] currentState, object[] previousState)
		{
			var diffs = new PropertyDiff[propertyNames.Length];
			for (var i = 0; i < diffs.Length; i++)
			{
				var oldValue = previousState == null ? null : previousState[i];
				var newValue = currentState == null ? null : currentState[i];

				// need to handle collections specially
				if (types[i].IsCollectionType && ReferenceEquals(oldValue, newValue))
				{
					// the collection instance itself has not changed, but perhaps the content has?
					// if the oldValue is a persistent collection, then we can get a snapshot from
					// when the collection was initially loaded, and see if it is dirty
					if (oldValue is IPersistentCollection && NHibernateUtil.IsInitialized(oldValue))
					{
						oldValue = GetCollectionSnapshot(oldValue as IPersistentCollection);
					}
				}

				diffs[i] = new PropertyDiff(propertyNames[i], types[i], oldValue, newValue);
			}
			return diffs;
		}

		private static object GetCollectionSnapshot(IPersistentCollection collection)
		{
			// the collection is not dirty, then the snapshot is the collection
			if (!collection.IsDirty)
			{
				return collection;
			}

			// it is dirty, so we need to get the snapshot
			var snapshot = collection.StoredSnapshot;

			// sometimes it seems there is no snapshot - in this case we just return null
			// to indicate that the snapshot is empty
			if (snapshot == null)
				return null;

			// unfortunately, the snapshot is not always stored in the same data structure as the collection itself
			if (collection is ISet)
			{
				// "set"
				// we return an untyped set - this is a bit lazy, we could create a typed set with some extra effort, but do we need it?
				return new HybridSet((ICollection)snapshot);
			}
			if (collection is IList && snapshot is IDictionary)
			{
				// "idbag"
				// we return an untyped list - this is a bit lazy, we could create a typed list with some extra effort, but do we need it?
				return new ArrayList(((IDictionary)snapshot).Values);
			}
			if (collection is IList && snapshot is IList)
			{
				// "list" or "bag"
				// in this case, the NH snapshot is the same type as the original collection
				return snapshot;
			}
			if (collection is IDictionary)
			{
				// in this case, the NH snapshot is the same type as the original collection
				return snapshot;
			}
			// TODO: implement this for other types of collection
			throw new NotImplementedException("Snapshot is not implemented for this collection type.");
		}
	}
}
