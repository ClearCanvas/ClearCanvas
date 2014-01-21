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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Events;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Core
{
	/// <summary>
	/// Class for editing a study. 
	/// </summary>
	public class StudyEditor : IDisposable
	{
		#region Private Fields
		private readonly WorkQueue _workQueue;
		#endregion

		#region Properties

		public Patient Patient { get; private set; }

		public Study Study { get; private set; }

		public ServerPartition ServerPartition { get; private set; }

		public StudyStorageLocation StorageLocation { get; private set; }

		public string FailureReason { get; set; }

		/// <summary>
        /// Gets the new <see cref="StudyStorageLocation"/> for the study after it is updated.
        /// </summary>
        public StudyStorageLocation NewStorageLocation { get; private set; }

	    #endregion

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="thePartition"></param>
		/// <param name="location"></param>
		/// <param name="thePatient"></param>
		/// <param name="theStudy"></param>
		public StudyEditor(ServerPartition thePartition, StudyStorageLocation location, Patient thePatient, Study theStudy, WorkQueue workQueue)
		{
			FailureReason = string.Empty;
			Platform.CheckForNullReference(thePartition, "thePartition");
			Platform.CheckForNullReference(location, "location");
			Platform.CheckForNullReference(thePatient, "thePatient");
			Platform.CheckForNullReference(theStudy, "theStudy");

			ServerPartition = thePartition;
			StorageLocation = location;
			
            Patient = thePatient;
            Study = theStudy;
			_workQueue = workQueue;

            // Scrub for invalid characters that may cause a failure when the Xml is generated for the history
		    Patient.PatientId = XmlUtils.XmlCharacterScrub(Patient.PatientId);
            Patient.PatientsName = XmlUtils.XmlCharacterScrub(Patient.PatientsName);
            
            Study.StudyDescription = XmlUtils.XmlCharacterScrub(Study.StudyDescription);
            Study.ReferringPhysiciansName = XmlUtils.XmlCharacterScrub(Study.ReferringPhysiciansName);
            Study.PatientId = XmlUtils.XmlCharacterScrub(Study.PatientId);
            Study.PatientsName = XmlUtils.XmlCharacterScrub(Study.PatientsName);            
		}
		#endregion

		/// <summary>
		/// Perform the edit.
		/// </summary>
		/// <param name="actionXml">A serialized XML representation of <see cref="SetTagCommand"/> objects</param>
		/// <returns></returns>
		public bool Edit(XmlElement actionXml)
		{
			Platform.Log(LogLevel.Info,
						 "Starting Edit of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
						 Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
						 Study.AccessionNumber, ServerPartition.Description);

            var parser = new EditStudyWorkQueueDataParser();
		    EditStudyWorkQueueData data = parser.Parse(actionXml);

			using (var processor = new ServerCommandProcessor("Web Edit Study"))
			{
				// Convert UpdateItem in the request into BaseImageLevelUpdateCommand
				List<BaseImageLevelUpdateCommand> updateCommands = null;
				if (data != null)
				{
					updateCommands = CollectionUtils.Map<Edit.UpdateItem, BaseImageLevelUpdateCommand>(
						data.EditRequest.UpdateEntries,
						delegate(Edit.UpdateItem item)
							{
								// Note: For edit, we assume each UpdateItem is equivalent to SetTagCommand
								return new SetTagCommand(item.DicomTag.TagValue, item.OriginalValue, item.Value);
							}
						);
				}

				var updateStudyCommand =
					new UpdateStudyCommand(ServerPartition, StorageLocation, updateCommands, 
						ServerRuleApplyTimeEnum.SopEdited, _workQueue);
				processor.AddCommand(updateStudyCommand);

				// Note, this command will only insert the ArchiveQueue command if a delete doesn't exist
				processor.AddCommand(new InsertArchiveQueueCommand(ServerPartition.Key, StorageLocation.Key));


				if (!processor.Execute())
				{
					Platform.Log(LogLevel.Error, processor.FailureException, "Unexpected failure editing study: {0}",
					             processor.FailureReason);
					FailureReason = processor.FailureReason;

					return false;
				}

				var context = new StudyEditedEventArgs
				{
					CommandProcessor = processor,
					EditType = data == null ? EditType.WebEdit : data.EditRequest.EditType,
					OriginalStudyStorageLocation = StorageLocation,
					EditCommands = updateCommands,
					OriginalStudy = Study,
					OrginalPatient = Patient,
					UserId = data == null ? string.Empty : data.EditRequest.UserId,
					Reason = data == null ? string.Empty : data.EditRequest.Reason
				};

				// reload the StudyStorageLocation
				NewStorageLocation = StudyStorageLocation.FindStorageLocations(StorageLocation.StudyStorage)[0];
				context.NewStudyStorageLocation = NewStorageLocation;

				EventManager.FireEvent(this, context);

				if (updateStudyCommand.Statistics != null)
					StatisticsLogger.Log(LogLevel.Info, updateStudyCommand.Statistics);

				return true;
			}
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
