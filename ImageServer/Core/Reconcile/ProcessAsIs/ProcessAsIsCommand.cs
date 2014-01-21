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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Reconcile.ProcessAsIs
{
	internal class ProcessAsIsCommand : ReconcileCommandBase
	{
		private StudyStorageLocation _destinationStudyStorage;
		private readonly bool _complete;

		public StudyStorageLocation Location
		{
			get { return _destinationStudyStorage; }
		}
   
		/// <summary>
		/// Creates an instance of <see cref="ProcessAsIsCommand"/>
		/// </summary>
		public ProcessAsIsCommand(ReconcileStudyProcessorContext context, bool complete)
			: base("Process As-is Command", true, context)
		{
			_complete = complete;
		}

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");
			
			DetermineTargetLocation();
		
			EnsureStudyCanBeUpdated(_destinationStudyStorage);

            try
            {
                if (Context.WorkQueueUidList.Count>0)
                    ProcessUidList();
            }
            finally
            {
                UpdateHistory(_destinationStudyStorage);
            }
			if (_complete)
			{
				StudyRulesEngine engine = new StudyRulesEngine(_destinationStudyStorage, Context.Partition);
				engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed, theProcessor);
			}
		}

		private void DetermineTargetLocation()
		{
			if (Context.History.DestStudyStorageKey!=null)
			{
				_destinationStudyStorage =
					StudyStorageLocation.FindStorageLocations(StudyStorage.Load(Context.History.DestStudyStorageKey))[0];

			}
			else
			{
				_destinationStudyStorage = Context.WorkQueueItemStudyStorage;
				Context.History.DestStudyStorageKey = _destinationStudyStorage.Key;
			}
		}

		protected override void OnUndo()
		{
			// undo is done  in SaveFile()
		}

		private void ProcessUidList()
		{
			int counter = 0;
			Platform.Log(LogLevel.Info, "Populating new images into study folder.. {0} to go", Context.WorkQueueUidList.Count);

			StudyProcessorContext context = new StudyProcessorContext(_destinationStudyStorage);

			// Load the rules engine
			context.SopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Context.WorkQueueItem.ServerPartitionKey);
			context.SopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
			context.SopProcessedRulesEngine.Load();

			// Load the Study XML File
			StudyXml xml = LoadStudyXml(_destinationStudyStorage);

		    string lastErrorMessage="";

		    foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{
				string imagePath = GetReconcileUidPath(uid);
				DicomFile file = new DicomFile(imagePath);
			
				try
				{
					file.Load();
					
					string groupID = ServerHelper.GetUidGroup(file, _destinationStudyStorage.ServerPartition, Context.WorkQueueItem.InsertTime);

				    SopInstanceProcessor sopProcessor = new SopInstanceProcessor(context);
                    ProcessingResult result = sopProcessor.ProcessFile(groupID, file, xml, false, true, uid, GetReconcileUidPath(uid), SopInstanceProcessorSopType.NewSop);
					if (result.Status != ProcessingStatus.Success)
					{
						throw new ApplicationException(String.Format("Unable to reconcile image {0}", file.Filename));
					}

					counter++;
			
					Platform.Log(ServerPlatform.InstanceLogLevel, "Reconciled SOP {0} [{1} of {2}]",
					             uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Error occurred when processing uid {0}", uid.SopInstanceUid);

                    if (e is InstanceAlreadyExistsException
                        || e.InnerException != null && e.InnerException is InstanceAlreadyExistsException)
                    {
                        // TODO (Rigel) - Check if we should include the WorkQueueData field here
                        DuplicateSopProcessorHelper.CreateDuplicateSIQEntry(file, _destinationStudyStorage, GetReconcileUidPath(uid),
                                                                           Context.WorkQueueItem, uid, null);
                    }
                    else
                    {
                        lastErrorMessage = e.Message;
                        SopInstanceProcessor.FailUid(uid, true);
                    }
				}
			}

            
            if (counter == 0)
            {
                throw new ApplicationException(lastErrorMessage);
            }
		}
	}
}