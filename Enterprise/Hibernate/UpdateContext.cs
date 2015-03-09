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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using NHibernate;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common.Audit;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// NHibernate implemenation of <see cref="IUpdateContext"/>.
	/// </summary>
	public class UpdateContext : PersistenceContext, IUpdateContext
	{
		// read this setting value once and cache it for process lifetime for performance reasons; changes will require a restart.
		private static readonly bool _enableChangeSetRecording = (new EntityChangeSetRecorderSettings().EnableRecording);


		private UpdateContextInterceptor _interceptor;
		private IEntityChangeSetRecorder _changeSetRecorder;
		private IDomainObjectValidator _validator;
		private readonly ChangeTracker _validationChangeTracker; 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="persistentStore"></param>
		/// <param name="mode"></param>
		internal UpdateContext(PersistentStore persistentStore, UpdateContextSyncMode mode)
			: base(persistentStore)
		{
			if (mode == UpdateContextSyncMode.Hold)
				throw new NotSupportedException("UpdateContextSyncMode.Hold is not supported");

			// create a default change-set logger
			_changeSetRecorder = new DefaultEntityChangeSetRecorder();
			_validator = new DomainObjectValidator();
			_validationChangeTracker = new ChangeTracker();
		}

		/// <summary>
		/// Disable domain object validation on this context.
		/// </summary>
		/// <remarks>
		/// This feature should be used with care.
		/// </remarks>
		public void DisableValidation()
		{
			_validator = DomainObjectValidator.NullValidator;
		}

		#region IUpdateContext members

		/// <summary>
		/// Gets or sets the change-set logger for auditing.
		/// </summary>
		/// <remarks>
		/// Setting this property to null will disable change-set auditing for this update context.
		/// </remarks>
		public IEntityChangeSetRecorder ChangeSetRecorder
		{
			get { return _changeSetRecorder; }
			set { _changeSetRecorder = value; }
		}
		
		/// <summary>
		/// Gets the set of entities that are affected by this update context, along with the type of change for each entity.
		/// </summary>
		/// <remarks>Not supported by this implementation.</remarks>
		public IDictionary<object, EntityChangeType> GetAffectedEntities()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Attempts to flush and commit all changes made within this update context to the persistent store.
		/// </summary>
		/// <remarks>
		/// If this operation succeeds, the state of the persistent store will be syncrhonized with the state
		/// of all domain objects that are attached to this context, and the context can continue to be used
		/// for read operations only. If the operation fails, an exception will be thrown.
		/// </remarks>
		public void Commit()
		{
			try
			{
				// flush session prior to commit, this ensures that all entities are validated and changes
				// recorded by the interceptor
				FlushAndValidate();

				EventsHelper.Fire(PreCommit, this, EventArgs.Empty);

				// publish pre-commit to listeners
				var changeSetId = Guid.NewGuid().ToString("N");
				var changeSetPublisher = new EntityChangeSetPublisher();
				changeSetPublisher.PreCommit(new EntityChangeSetPreCommitArgs(changeSetId, new EntityChangeSet(_interceptor.FullChangeSet), this));

				// flush session again, in case pre-commit listeners made modifications to entities in this context
				FlushAndValidate();

				// do audit
				AuditTransaction();

				// do final commit
				CommitTransaction();

				// publish post-commit to listeners
				changeSetPublisher.PostCommit(new EntityChangeSetPostCommitArgs(changeSetId, new EntityChangeSet(_interceptor.FullChangeSet)));
			}
			catch (Exception e)
			{
				HandleHibernateException(e, SR.ExceptionCommitFailure);
			}
		}

		public event EventHandler PreCommit;

		public event EventHandler PostCommit;

		#endregion

		#region Protected overrides

		protected override ISession CreateSession()
		{
			_interceptor = new UpdateContextInterceptor(this);
			_interceptor.AddChangeTracker(_validationChangeTracker);
			return PersistentStore.SessionFactory.OpenSession(_interceptor);
		}

		protected override void LockCore(DomainObject obj, DirtyState dirtyState)
		{
			switch (dirtyState)
			{
				case DirtyState.Dirty:
					Session.Update(obj);
					break;
				case DirtyState.New:
					CheckRequiredFields(obj);
					Session.Save(obj);
					break;
				case DirtyState.Clean:
					Session.Lock(obj, LockMode.None);
					break;
			}
		}

		internal IDomainObjectValidator Validator
		{
			get { return _validator; }
		}

		internal override bool ReadOnly
		{
			get { return false; }
		}

		protected override void SynchStateCore()
		{
			FlushAndValidate();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				try
				{
					// assume the transaction failed and rollback
					RollbackTransaction();
				}
				catch (Exception e)
				{
					HandleHibernateException(e, SR.ExceptionCloseContext);
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Specifies that the version should be checked.  This makes sense, as a default, in an update context
		/// to ensure versioning isn't violated.
		/// </summary>
		protected override EntityLoadFlags DefaultEntityLoadFlags
		{
			get { return EntityLoadFlags.CheckVersion; }
		}

		#endregion


		#region Helpers

		private void CheckRequiredFields(DomainObject entity)
		{
			// This is really a HACK
			// we need to test the required field rules before NHibernate gets a chance to complain about them
			// in order to provide more descriptive error message (the NHibernate error messages aren't as nice as ours)
			_validator.ValidateRequiredFieldsPresent(entity);
		}

		private void FlushAndValidate()
		{
			// order of operations here is extremely important!

			// flush first
			// this will apply low-level validation from within the interceptor callbacks prior to writing to db
			// and it will ensure that the _validationChangeTracker is up to date
			Session.Flush();

			// _validationChangeTracker is used to determine which entities need high-level validation
			// apply high-level validation to modified entities (excluding those that have been deleted)
			var modifiedEntities =
				_validationChangeTracker.GetChangedEntities(changeType => changeType != EntityChangeType.Delete);
			foreach (var entity in modifiedEntities)
			{
				_validator.ValidateHighLevel(entity);
			}

			// clear the validation change tracker, so that it will
			// in future contain only entries for things that have changed after this point
			_validationChangeTracker.Clear();
		}

		/// <summary>
		/// Writes an audit log entry for the current change-set, assuming the
		/// <see cref="ChangeSetRecorder"/> property is set and recording is enabled.
		/// </summary>
		private void AuditTransaction()
		{
			if (_enableChangeSetRecording && _changeSetRecorder != null)
			{
				// write to the "ChangeSet" audit log
				var auditLog = new AuditLog(ProductInformation.Component, "ChangeSet");
				_changeSetRecorder.WriteLogEntry(_interceptor.FullChangeSet, auditLog);
			}
		}

		#endregion
	}
}
