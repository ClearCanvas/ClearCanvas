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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	public class EntityChangeSetPreCommitArgs
	{
		public EntityChangeSetPreCommitArgs(string changeSetId, EntityChangeSet changeSet, IPersistenceContext context)
		{
			ChangeSetId = changeSetId;
			ChangeSet = changeSet;
			PersistenceContext = context;
		}

		/// <summary>
		/// Gets the identifier of this change set.
		/// </summary>
		public string ChangeSetId { get; private set; }

		/// <summary>
		/// Gets the change-set that is about to be committed.
		/// </summary>
		public EntityChangeSet ChangeSet { get; private set; }

		/// <summary>
		/// Gets the persistence context in which the change-set is about to be committed.
		/// Changes made still be made to entities in this context prior to commit.
		/// </summary>
		public IPersistenceContext PersistenceContext { get; private set; }
	}

	public class EntityChangeSetPostCommitArgs
	{
		public EntityChangeSetPostCommitArgs(string changeSetId, EntityChangeSet changeSet)
		{
			ChangeSetId = changeSetId;
			ChangeSet = changeSet;
		}

		/// <summary>
		/// Gets the identifier of this change set.
		/// </summary>
		public string ChangeSetId { get; private set; }

		/// <summary>
		/// Gets the change-set that was committed.
		/// </summary>
		public EntityChangeSet ChangeSet { get; private set; }
	}


	/// <summary>
	/// Defines an interface to an object that subscribes to entity-change set notifications.
	/// </summary>
	public interface IEntityChangeSetListener
	{
		/// <summary>
		/// Occurs immediately prior to committing a change-set.
		/// </summary>
		/// <param name="args"></param>
		void PreCommit(EntityChangeSetPreCommitArgs args);

		/// <summary>
		/// Occurs immediately after committing a change-set.
		/// </summary>
		/// <param name="args"></param>
		void PostCommit(EntityChangeSetPostCommitArgs args);
	}
}
