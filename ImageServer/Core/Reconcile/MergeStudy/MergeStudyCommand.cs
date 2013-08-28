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
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Reconcile.MergeStudy
{
	/// <summary>
	/// Command for reconciling images by merging new images into an existing study.
	/// </summary>
	/// <remark>
	/// </remark>
	class MergeStudyCommand : ReconcileCommandBase
	{
		#region Private Members
		
		private int _failedCount;
		private int _processedCount;
		private StudyStorageLocation _destinationStudyStorage;
		private readonly bool _updateDestination;
		private readonly bool _complete;
		private readonly List<BaseImageLevelUpdateCommand> _commands;

	    #endregion

		#region Properties

		public StudyStorageLocation Location
		{
			get { return _destinationStudyStorage; }
		}

		#endregion

        #region Constructors
		/// <summary>
		/// Creates an instance of <see cref="MergeStudyCommand"/>
		/// </summary>
		public MergeStudyCommand(ReconcileStudyProcessorContext context, bool updateDestination, List<BaseImageLevelUpdateCommand> commands, bool complete)
			: base("Merge Study", true, context)
		{
			_updateDestination = updateDestination;
			_commands = commands;
			_complete = complete;
		}
		#endregion

		#region Overriden Protected Methods
		protected override void OnExecute(CommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");

			_destinationStudyStorage = Context.History.DestStudyStorageKey != null 
				? StudyStorageLocation.FindStorageLocations(StudyStorage.Load(Context.History.DestStudyStorageKey))[0] 
				: Context.WorkQueueItemStudyStorage;

            EnsureStudyCanBeUpdated(_destinationStudyStorage);

			if (_updateDestination)
				UpdateExistingStudy();
            
			LoadMergedStudyEntities();

            

            try
            {
                LoadUidMappings();

                if (Context.WorkQueueUidList.Count>0)
                {
                    ProcessUidList();
                    LogResult();
                }
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

        protected void LoadUidMappings()
        {
            // Load the mapping for the study
			if (_destinationStudyStorage != null)
            {
				string path = Path.Combine(_destinationStudyStorage.GetStudyPath(), "UidMap.xml");
                if (File.Exists(path))
                {
                    UidMapXml xml = new UidMapXml();
					xml.Load(_destinationStudyStorage);
                    UidMapper = new UidMapper(xml);

                    UidMapper.SeriesMapUpdated += UidMapper_SeriesMapUpdated;
                }
            }
        }

		#endregion

		#region Protected Methods
	
		protected override void OnUndo()
		{
		}

		#endregion

		#region Private Members
		
		private void LogResult()
		{
			StringBuilder log = new StringBuilder();
			log.AppendFormat("Destination location: {0}", _destinationStudyStorage.GetStudyPath());
			log.AppendLine();
			if (_failedCount > 0)
			{
				log.AppendFormat("{0} images failed to be reconciled.", _failedCount);
				log.AppendLine();
			}
            
			log.AppendFormat("{0} images have been reconciled.", _processedCount);
			log.AppendLine();
			Platform.Log(LogLevel.Info, log);
		}

		private void UpdateExistingStudy()
		{

			Platform.Log(LogLevel.Info, "Updating existing study...");
			using(ServerCommandProcessor updateProcessor = new ServerCommandProcessor("Update Study"))
			{
				UpdateStudyCommand studyUpdateCommand = new UpdateStudyCommand(Context.Partition, _destinationStudyStorage, _commands, ServerRuleApplyTimeEnum.SopProcessed, Context.WorkQueueItem);
				updateProcessor.AddCommand(studyUpdateCommand);
				if (!updateProcessor.Execute())
				{
					throw new ApplicationException(
						String.Format("Unable to update existing study: {0}", updateProcessor.FailureReason));
				}
			}            
		}

		private void ProcessUidList()
		{
		    string lastErrorMessage = "";

			Platform.Log(LogLevel.Info, "Populating new images into study folder.. {0} to go", Context.WorkQueueUidList.Count);

			StudyProcessorContext context = new StudyProcessorContext(_destinationStudyStorage);

			// Load the rules engine
			context.SopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, Context.WorkQueueItem.ServerPartitionKey);
			context.SopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
			context.SopProcessedRulesEngine.Load();

			// Add the update commands to
			context.UpdateCommands.AddRange(BuildUpdateCommandList());

            // Add command to update the Series & Sop Instances.
            context.UpdateCommands.Add(new SeriesSopUpdateCommand(Context.WorkQueueItemStudyStorage, _destinationStudyStorage, UidMapper));

            // Load the Study XML File
			StudyXml xml = LoadStudyXml(_destinationStudyStorage);
            PrintUpdateCommands(context.UpdateCommands);
            foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{
				// Load the file outside the try/catch block so it can be
				// referenced in the c
				string imagePath = GetReconcileUidPath(uid);
				DicomFile file = new DicomFile(imagePath);

				try
				{
					file.Load();

					string groupID = ServerHelper.GetUidGroup(file, Context.Partition, Context.WorkQueueItem.InsertTime);

				    SopInstanceProcessor sopProcessor = new SopInstanceProcessor(context) {EnforceNameRules = true };

					ProcessingResult result = sopProcessor.ProcessFile(groupID, file, xml, false, true, uid, GetReconcileUidPath(uid),
					                                                   SopInstanceProcessorSopType.NewSop);
					if (result.Status != ProcessingStatus.Success)
					{
						throw new ApplicationException(String.Format("Unable to reconcile image {0}", file.Filename));
					}

					_processedCount++;

					Platform.Log(ServerPlatform.InstanceLogLevel, "Reconciled SOP {0} [{1} of {2}]", uid.SopInstanceUid, _processedCount, Context.WorkQueueUidList.Count);
				}
				catch (Exception e)
				{
                    Platform.Log(LogLevel.Error, e, "Error occurred when processing uid {0}", uid.SopInstanceUid);
					
                    if (e is InstanceAlreadyExistsException
						|| e.InnerException != null && e.InnerException is InstanceAlreadyExistsException)
					{
                        // TODO (Rigel) - Check if we should include the WorkItemData to insert into the WorkQueue here.
						DuplicateSopProcessorHelper.CreateDuplicateSIQEntry(file, _destinationStudyStorage, GetReconcileUidPath(uid),
												   Context.WorkQueueItem, uid, null);
					}
                    else
                    {
                        lastErrorMessage = e.Message;
                        SopInstanceProcessor.FailUid(uid, true);
                    }
				    _failedCount++;
				}
			}

            if (_processedCount==0)
            {
                throw new ApplicationException(lastErrorMessage);
            }
		}

	    private void LoadMergedStudyEntities()
		{
			StudyStorage storage = StudyStorage.Load(_destinationStudyStorage.Key);
			_destinationStudyStorage = StudyStorageLocation.FindStorageLocations(storage)[0];
		}


		private List<BaseImageLevelUpdateCommand> BuildUpdateCommandList()
		{
			List<BaseImageLevelUpdateCommand> updateCommandList = new List<BaseImageLevelUpdateCommand>();
            
			ImageUpdateCommandBuilder builder = new ImageUpdateCommandBuilder();
			updateCommandList.AddRange(builder.BuildCommands<StudyMatchingMap>(_destinationStudyStorage));
            
			return updateCommandList;
		}

		#endregion

		#region Private Static Methods
		private static void PrintUpdateCommands(IEnumerable<BaseImageLevelUpdateCommand> updateCommandList)
		{
			StringBuilder log = new StringBuilder();
			log.AppendLine("Update on merged images:");
			foreach (BaseImageLevelUpdateCommand cmd in updateCommandList)
			{
				log.AppendLine(String.Format("\t{0}", cmd));
			}
			Platform.Log(LogLevel.Info, log);
		}

		#endregion
	}
}