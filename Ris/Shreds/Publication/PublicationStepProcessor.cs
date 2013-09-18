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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
	/// <summary>
	/// Defines the interface for a publication action, that is, an action to be taken
	/// upon publication of a radiololgy report.
	/// </summary>
	public interface IPublicationAction
	{
		bool Enabled { get; }
		int RetryCount { get; }
		void Execute(ReportPart reportPart, IPersistenceContext context);
	}

	[ExtensionPoint]
	public class PublicationActionExtensionPoint : ExtensionPoint<IPublicationAction>
	{
	}

	/// <summary>
	/// Processes <see cref="PublicationStep"/>s, queueing a work item for each <see cref="IPublicationAction"/>
	/// on each step.
	/// </summary>
	internal class PublicationStepProcessor : EntityQueueProcessor<PublicationStep>
	{
		private readonly ExtensionInfo[] _publicationActions;
		private readonly PublicationShredSettings _settings;

		public PublicationStepProcessor(PublicationShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_settings = settings;
			_publicationActions = new PublicationActionExtensionPoint().ListExtensions();
		}

		public override string Name
		{
			get { return this.GetType().Name; }
		}

		protected override IList<PublicationStep> GetNextEntityBatch(int batchSize)
		{
			// Get scheduled steps, where the "publishing cool-down" has elapsed
			// eg LastFailureTime is more than a specified number of seconds ago

			var page = new SearchResultPage(0, batchSize);
			return PersistenceScope.CurrentContext.GetBroker<IPublicationStepBroker>().FindUnprocessedSteps(_settings.FailedItemRetryDelay, page);
		}

		protected override void MarkItemClaimed(PublicationStep item)
		{
			// do nothing
		}

		protected override void ActOnItem(PublicationStep item)
		{
			Platform.Log(LogLevel.Info, "Processing publication step {0}", item.OID);
			// enqueue work item for each publication action
			foreach (var action in _publicationActions)
			{
				EnqueueWorkItem(item.ReportPart, action);
			}
		}

		protected override void OnItemFailed(PublicationStep item, Exception error)
		{
			// mark item as failed
			item.Fail();
		}

		protected override void OnItemSucceeded(PublicationStep item)
		{
			// all actions succeeded, so mark the publication item as being completed
			item.Complete(item.AssignedStaff);
		}

		private static void EnqueueWorkItem(ReportPart reportPart, ExtensionInfo publicationAction)
		{
			var actionType = publicationAction.ExtensionClass.FullName;
			var action = (IPublicationAction)new PublicationActionExtensionPoint().CreateExtension(new ClassNameExtensionFilter(actionType));
			if (!action.Enabled)
				return;

			var workQueueItem = new WorkQueueItem("Publication Action");
			workQueueItem.ExtendedProperties.Add("ReportPartRef", reportPart.GetRef().Serialize());
			workQueueItem.ExtendedProperties.Add("ActionType", actionType);

			var procedure = reportPart.Report.Procedures.First();
			workQueueItem.ExtendedProperties.Add("ProcedureType", procedure.Type.Name);
			workQueueItem.ExtendedProperties.Add("AccessionNumber", procedure.Order.AccessionNumber);
			workQueueItem.ExtendedProperties.Add("StudyInstanceUID", procedure.StudyInstanceUID);
			

			PersistenceScope.CurrentContext.Lock(workQueueItem, DirtyState.New);
		}
	}
}
