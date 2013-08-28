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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core.Reconcile.CreateStudy
{
	/// <summary>
	/// Command for reconciling images by creating a new study or merge into an existing study.
	/// </summary>
	class CreateStudyCommand : ReconcileCommandBase
	{
		#region Private Members

		private readonly List<BaseImageLevelUpdateCommand> _commands;
		private StudyStorageLocation _destinationStudyStorage;
		private readonly bool _complete;

		#endregion

		#region Properties

		public StudyStorageLocation Location
		{
			get { return _destinationStudyStorage; }
		}

		private List<BaseImageLevelUpdateCommand> Commands
		{
			get { return _commands; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Create an instance of <see cref="CreateStudyCommand"/>
		/// </summary>
		public CreateStudyCommand(ReconcileStudyProcessorContext context, List<BaseImageLevelUpdateCommand> commands, bool complete)
			: base("Create Study", true, context)
		{
			_commands = commands;
			_complete = complete;
		}

		#endregion

		#region Overriden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			Platform.CheckForNullReference(Context, "Context");
			Platform.CheckForNullReference(Context.WorkQueueItem, "Context.WorkQueueItem");
			Platform.CheckForNullReference(Context.WorkQueueUidList, "Context.WorkQueueUidList");

			// Create or Load the Destination StorageLocation.  Note that this may cause 
			// issues with orphan StorageLocations if none of the images below are actually
			// processed.  This must be done becasue the SopInstanceProcessor requires the
			// destination location.
			CreateDestinationStudyStorage();

            try
            {
                if (Context.WorkQueueUidList.Count > 0)
                {
                    CreateOrLoadUidMappings();

                    PrintChangeList();

                    ProcessUidList();
                }
            }
            finally
            {
                UpdateHistory(_destinationStudyStorage);
            }

			if (_complete)
			{
				var engine = new StudyRulesEngine(_destinationStudyStorage, Context.Partition);
				engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed, theProcessor);
			}
		}

        protected void CreateOrLoadUidMappings()
        {
            // Load the mapping for the study
            if (_destinationStudyStorage != null)
            {
                var xml = new UidMapXml();
                xml.Load(_destinationStudyStorage);
                UidMapper = new UidMapper(xml);
            }
            else
            {
                UidMapper = new UidMapper();
            }

            UidMapper.SeriesMapUpdated += UidMapper_SeriesMapUpdated;
        }

	    protected override void OnUndo()
		{

		}

		#endregion

		#region Private Methods

		private void PrintChangeList()
		{
			StringBuilder log = new StringBuilder();
			log.AppendFormat("Applying following changes to images:\n");
			foreach (BaseImageLevelUpdateCommand cmd in Commands)
			{
				log.AppendFormat("{0}", cmd);
				log.AppendLine();
			}

			Platform.Log(LogLevel.Info, log);
		}


		private void ProcessUidList()
		{
			int counter = 0;
			Platform.Log(LogLevel.Info, "Populating images into study folder.. {0} to go", Context.WorkQueueUidList.Count);

			StudyProcessorContext context = new StudyProcessorContext(_destinationStudyStorage)
			                                	{
			                                		SopProcessedRulesEngine =
			                                			new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed,
			                                			                      Context.WorkQueueItem.ServerPartitionKey)
			                                	};

			// Load the rules engine
			context.SopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
			context.SopProcessedRulesEngine.Load();

			// Add the update commands to update the files.  Note that the new Study Instance Uid is already part of this update.
			context.UpdateCommands.AddRange(Commands);

			// Add command to update the Series & Sop Instances.
			context.UpdateCommands.Add(new SeriesSopUpdateCommand(Context.WorkQueueItemStudyStorage, _destinationStudyStorage, UidMapper));

			// Create/Load the Study XML File
			StudyXml xml = LoadStudyXml(_destinationStudyStorage);

		    string lastErrorMessage = "";

		    foreach (WorkQueueUid uid in Context.WorkQueueUidList)
			{
				string imagePath = GetReconcileUidPath(uid);
				var file = new DicomFile(imagePath);

				var sopProcessor = new SopInstanceProcessor(context) { EnforceNameRules = true };

				try
				{
					file.Load();

					string groupID = ServerHelper.GetUidGroup(file, _destinationStudyStorage.ServerPartition, Context.WorkQueueItem.InsertTime);

					ProcessingResult result = sopProcessor.ProcessFile(groupID, file, xml, false, true, uid, imagePath,
					                                                   SopInstanceProcessorSopType.NewSop);
					if (result.Status != ProcessingStatus.Success)
					{
						throw new ApplicationException(String.Format("Unable to reconcile image {0}", file.Filename));
					}

					counter++;
					Platform.Log(ServerPlatform.InstanceLogLevel, "Reconciled and Processed SOP {0} [{1} of {2}]",
								 uid.SopInstanceUid, counter, Context.WorkQueueUidList.Count);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Error occurred when processing uid {0}", uid.SopInstanceUid);

				    lastErrorMessage = e.Message;

				    SopInstanceProcessor.FailUid(uid, true);
				}
			}

            if (counter == 0)
            {
                throw new ApplicationException(lastErrorMessage);
            }
		}

	    private void CreateDestinationStudyStorage()
		{
			// This really should never happen;
			if (Context.History.DestStudyStorageKey != null)
			{
				_destinationStudyStorage =
					StudyStorageLocation.FindStorageLocations(StudyStorage.Load(Context.History.DestStudyStorageKey))[0];
				return;
			}

			string newStudyInstanceUid = string.Empty;

			// Get the new Study Instance Uid by looking through the update commands
			foreach (BaseImageLevelUpdateCommand command in Commands)
			{
				SetTagCommand setTag = command as SetTagCommand;
				if (setTag != null && setTag.Tag.TagValue.Equals(DicomTags.StudyInstanceUid))
				{
					newStudyInstanceUid = setTag.Value;
					break;
				}
			}

			if (string.IsNullOrEmpty(newStudyInstanceUid))
				throw new ApplicationException("Unexpectedly could not find new Study Instance Uid value for Create Study");


			using (ServerCommandProcessor processor = new ServerCommandProcessor("Reconciling image processor"))
			{
				// Assign new series and instance uid
				InitializeStorageCommand command = new InitializeStorageCommand(Context, newStudyInstanceUid, Context.WorkQueueItemStudyStorage.StudyFolder,
					TransferSyntax.GetTransferSyntax(Context.WorkQueueItemStudyStorage.TransferSyntaxUid));
				processor.AddCommand(command);

				if (!processor.Execute())
				{
					throw new ApplicationException(String.Format("Unable to create Study Storage for study: {0}", newStudyInstanceUid), processor.FailureException);
				}

				_destinationStudyStorage = command.Location;
			}
		}

		#endregion
	}

	/// <summary>
	/// Command to initialize the study storage record in the database
	/// </summary>
	class InitializeStorageCommand : ServerDatabaseCommand<ReconcileStudyProcessorContext>
	{
		private readonly string _studyInstanceUid;
		private readonly string _studyDate;
		private readonly TransferSyntax _transferSyntax;
		private StudyStorageLocation _location;

		public StudyStorageLocation Location
		{
			get { return _location; }
		}

		public InitializeStorageCommand(ReconcileStudyProcessorContext context, DicomMessageBase file)
			:base("InitializeStorageCommand", context)
		{
			Platform.CheckForNullReference(file, "file");
			_studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
			_studyDate = file.DataSet[DicomTags.StudyDate].ToString();
			_transferSyntax = file.TransferSyntax;
		}

		public InitializeStorageCommand(ReconcileStudyProcessorContext context, string studyInstanceUid,
			string studyDate, TransferSyntax transferSyntax)
			: base("InitializeStorageCommand", context)
		{
			Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
			Platform.CheckForNullReference(studyDate, "studyDate");
			Platform.CheckForNullReference(transferSyntax, "transferSyntax");

			_studyInstanceUid = studyInstanceUid;
			_studyDate = studyDate;
			_transferSyntax = transferSyntax;
		}

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			_location = FindOrCreateStudyStorageLocation();
		}

		private StudyStorageLocation FindOrCreateStudyStorageLocation()
		{
			Platform.CheckForNullReference(UpdateContext, "UpdateContext");
			Platform.CheckForNullReference(Context, "Context");
			Platform.CheckForNullReference(Context.Partition, "Context.Partition");

			bool created;
			StudyStorageLocation studyLocation = FilesystemMonitor.Instance.GetOrCreateWritableStudyStorageLocation(
				_studyInstanceUid,
				_studyDate,
				_transferSyntax,
				UpdateContext,
				Context.Partition,
				out created);

			return studyLocation;
		}
	}
}