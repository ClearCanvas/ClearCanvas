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
using System.Diagnostics;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Diagnostics;
using ClearCanvas.ImageServer.Core.Events;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;
using SaveDicomFileCommand = ClearCanvas.ImageServer.Core.Command.SaveDicomFileCommand;
using Settings=ClearCanvas.ImageServer.Core.Diagnostics.Settings;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// <see cref="ServerDatabaseCommand"/> for updating a study.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public class UpdateStudyCommand : ServerDatabaseCommand, IDisposable
	{
		private const string UTF8 = "ISO_IR 192";

		#region Private Members

		private readonly List<InstanceInfo> _updatedSopList = new List<InstanceInfo>();
		private readonly StudyStorageLocation _oldStudyLocation;
		private string _oldStudyPath;
		private string _oldStudyInstanceUid;
		private string _newStudyInstanceUid;
		private string _oldStudyFolder;
		private bool _initialized;
		private PatientInfo _oldPatientInfo;
		private PatientInfo _newPatientInfo;
		private readonly IList<BaseImageLevelUpdateCommand> _commands;
		private string _backupDir;
		private readonly ServerPartition _partition;
		private Study _study;
		private Patient _curPatient;
		private Patient _newPatient;
		private StudyStorage _storage;
		private readonly WorkQueue _workQueue;

		private readonly UpdateStudyStatistics _statistics;
		private int _totalSopCount;
		private bool _restored;
		private bool _deleteOriginalFolder;

		private bool _patientInfoChanged;
		private readonly ServerRulesEngine _rulesEngine;

		private bool atLeastOneFileUpdatedToUTF8 = false;

		#endregion

		#region Constructors

		public UpdateStudyCommand(ServerPartition partition,
		                          StudyStorageLocation studyLocation,
		                          IList<BaseImageLevelUpdateCommand> imageLevelCommands,
		                          ServerRuleApplyTimeEnum applyTime,
		                          WorkQueue workQueue)
			: base("Update existing study")
		{
			_partition = partition;
			_oldStudyLocation = studyLocation;
			_commands = imageLevelCommands;
			_workQueue = workQueue;
			_statistics = new UpdateStudyStatistics(_oldStudyLocation.StudyInstanceUid);
			// Load the engine for editing rules.
			_rulesEngine = new ServerRulesEngine(applyTime, _partition.Key);
			if (applyTime.Equals(ServerRuleApplyTimeEnum.SopProcessed))
				_rulesEngine.AddIncludeType(ServerRuleTypeEnum.AutoRoute);
			_rulesEngine.Load();
		}

		#endregion

		#region Properties

		public new UpdateStudyStatistics Statistics
		{
			get { return _statistics; }
		}

		public string NewStudyPath { get; set; }

		#endregion

		#region Protected Method

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			Statistics.ProcessTime.Start();

			Initialize();

			PrintUpdateCommands();

			if (RequiresRollback)
				BackupFilesystem();

			UpdateFilesystem();

			UpdateDatabase();

			Statistics.ProcessTime.End();
		}

		protected override void OnUndo()
		{
			RestoreFilesystem();

			// db rollback is done by the processor
			CleanupBackupFiles();

			_restored = true;
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			_backupDir = ProcessorContext.BackupDirectory;

			_oldStudyPath = _oldStudyLocation.GetStudyPath();
			_oldStudyInstanceUid = _oldStudyLocation.StudyInstanceUid;
			_oldStudyFolder = _oldStudyLocation.StudyFolder;
			_newStudyInstanceUid = _oldStudyInstanceUid;

			_study = _oldStudyLocation.LoadStudy(ServerExecutionContext.Current.ReadContext);
			_totalSopCount = _study.NumberOfStudyRelatedInstances;
			_curPatient = _study.LoadPatient(ServerExecutionContext.Current.ReadContext);
			_oldPatientInfo = new PatientInfo
				{
					PatientsName = _curPatient.PatientsName,
					PatientId = _curPatient.PatientId,
					IssuerOfPatientId = _curPatient.IssuerOfPatientId
				};

			_newPatientInfo = new PatientInfo(_oldPatientInfo);
			Debug.Assert(_newPatientInfo.Equals(_oldPatientInfo));

			foreach (BaseImageLevelUpdateCommand command in _commands)
			{
				ImageLevelUpdateEntry imageLevelUpdate = command.UpdateEntry;
				if (imageLevelUpdate == null)
					continue;

				if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.StudyInstanceUid)
				{
					_newStudyInstanceUid = imageLevelUpdate.GetStringValue();
				}
				else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.PatientId)
				{
					_newPatientInfo.PatientId = imageLevelUpdate.GetStringValue();
				}
				else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.IssuerOfPatientId)
				{
					_newPatientInfo.IssuerOfPatientId = imageLevelUpdate.GetStringValue();
				}
				else if (imageLevelUpdate.TagPath.Tag.TagValue == DicomTags.PatientsName)
				{
					_newPatientInfo.PatientsName = imageLevelUpdate.GetStringValue();
				}
			}

			Platform.CheckForNullReference(_newStudyInstanceUid, "_newStudyInstanceUid");

			NewStudyPath = Path.Combine(_oldStudyLocation.FilesystemPath, _partition.PartitionFolder);
			NewStudyPath = Path.Combine(NewStudyPath, _oldStudyFolder);
			NewStudyPath = Path.Combine(NewStudyPath, _newStudyInstanceUid);

			_newPatient = FindPatient(_newPatientInfo, ServerExecutionContext.Current.ReadContext);
			_patientInfoChanged = !_newPatientInfo.AreSame(_oldPatientInfo, false);

			Statistics.InstanceCount = _study.NumberOfStudyRelatedInstances;
			Statistics.StudySize = (ulong) _oldStudyLocation.LoadStudyXml().GetStudySize();

			// The study path will be changed. We will need to delete the original folder at the end.
			// May be too simple to test if two paths are the same. But let's assume it is good enough for 99% of the time.
			_deleteOriginalFolder = NewStudyPath != _oldStudyPath;
			_initialized = true;
		}

		private void CleanupBackupFiles()
		{
			DirectoryUtility.DeleteIfExists(_backupDir);
		}

		private static Patient FindPatient(PatientInfo patientInfo, IPersistenceContext context)
		{
			var patientFindBroker = context.GetBroker<IPatientEntityBroker>();
			var criteria = new PatientSelectCriteria();

			if (!String.IsNullOrEmpty(patientInfo.PatientId))
				criteria.PatientId.EqualTo(patientInfo.PatientId);
			else
				criteria.PatientId.IsNull();

			if (!String.IsNullOrEmpty(patientInfo.PatientsName))
				criteria.PatientsName.EqualTo(patientInfo.PatientsName);
			else
				criteria.PatientsName.IsNull();

			return patientFindBroker.FindOne(criteria);
		}

		private void PrintUpdateCommands()
		{
			var log = new StringBuilder();
			log.AppendLine(String.Format("Study to be updated:"));
			log.AppendLine(String.Format("\tServer Partition: {0}", _partition.AeTitle));
			log.AppendLine(String.Format("\tStorage GUID: {0}", _oldStudyLocation.Key.Key));
			log.AppendLine(String.Format("\tPatient ID: {0}", _study.PatientId));
			log.AppendLine(String.Format("\tPatient Name: {0}", _study.PatientsName));
			log.AppendLine(String.Format("\tAccession #: {0}", _study.AccessionNumber));
			log.AppendLine(String.Format("\tStudy ID : {0}", _study.StudyId));
			log.AppendLine(String.Format("\tStudy Date : {0}", _study.StudyDate));
			log.AppendLine(String.Format("\tPatient's Birth Date : {0}", _study.PatientsBirthDate));
			log.AppendLine(String.Format("\tStudy Instance Uid: {0}", _study.StudyInstanceUid));
			log.AppendLine(String.Format("\tInstance Count: {0}", _study.NumberOfStudyRelatedInstances));
			log.AppendLine(String.Format("\tCurrent location: {0}", _oldStudyPath));
			log.AppendLine();
			log.AppendLine("Changes to be applied:");
			foreach (BaseImageLevelUpdateCommand cmd in _commands)
			{
				log.AppendLine(String.Format("\t{0}", cmd));
			}

			log.AppendLine(String.Format("\tNew location: {0}", NewStudyPath));
			Platform.Log(LogLevel.Info, log);
		}

		private void RestoreFilesystem()
		{
			if (!RequiresRollback || !_initialized || _backupDir == null)
				return;

			if (NewStudyPath == _oldStudyPath)
			{
				// Study folder was not changed. Files were overwritten.

				// restore header
				Platform.Log(LogLevel.Info, "Restoring old study header...");

				FileUtils.Copy(Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml"), _oldStudyLocation.GetStudyXmlPath(), true);
				FileUtils.Copy(Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml.gz"),
				               _oldStudyLocation.GetCompressedStudyXmlPath(), true);

				// restore updated SOPs
				Platform.Log(LogLevel.Info, "Restoring old study folder... {0} sop need to be restored", _updatedSopList.Count);
				int restoredCount = 0;
				foreach (InstanceInfo sop in _updatedSopList)
				{
					string backupSopPath = Path.Combine(_backupDir, sop.SopInstanceUid + ServerPlatform.DicomFileExtension);

					FileUtils.Copy(backupSopPath, _oldStudyLocation.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid), true);

					restoredCount++;
					Platform.Log(ServerPlatform.InstanceLogLevel, "Restored SOP {0} [{1} of {2}]", sop.SopInstanceUid, restoredCount,
					             _updatedSopList.Count);

					SimulateErrors();
				}

				if (restoredCount > 0)
					Platform.Log(LogLevel.Info, "{0} SOP(s) have been restored.", restoredCount);
			}
			else
			{
				// Different study folder was used. Original folder must be kept around 
				// because we are rolling back.
				_deleteOriginalFolder = false;
			}
		}

		private static void SimulateErrors()
		{
			RandomError.Generate(Settings.SimulateEditError, "Update study errors");
		}

		private void UpdateEntity(ServerEntity entity)
		{
			EntityDicomMap entityMap = EntityDicomMapManager.Get(entity.GetType());

			foreach (BaseImageLevelUpdateCommand command in _commands)
			{
				ImageLevelUpdateEntry entry = command.UpdateEntry;
				if (!entityMap.ContainsKey(entry.TagPath.Tag))
					continue;

				string value = entry.GetStringValue();
				DicomTag tag = entry.TagPath.Tag;
				if (tag.TagValue == DicomTags.PatientsSex)
				{
					// Valid Patient's Sex value : "M", "F" or "O"
					if (!String.IsNullOrEmpty(value) && !value.ToUpper().Equals("M") && !value.ToUpper().Equals("F"))
						value = "O";
				}
				int maxLength = tag.VR.Equals(DicomVr.PNvr) ? 64 : (int) tag.VR.MaximumLength;
				if (value != null && value.Length > maxLength)
				{
					Platform.Log(LogLevel.Warn, "Truncating value to VR Length: {0}: {1}", tag.VR.Name, value);
					if (!entityMap.Populate(entity, entry.TagPath.Tag, value.Substring(0, maxLength)))
						throw new ApplicationException(String.Format("Unable to update {0}. See log file for details.", entity.Name));
				}
				else
				{
					if (!entityMap.Populate(entity, entry.TagPath.Tag, value))
						throw new ApplicationException(String.Format("Unable to update {0}. See log file for details.", entity.Name));
				}
			}
		}

		private static bool IsUTF8(string characterSet)
		{
			return string.Equals(characterSet, UTF8, StringComparison.InvariantCulture);
		}

		private void LoadEntities()
		{
			_storage = StudyStorage.Load(_oldStudyLocation.Key);
			_study = _storage.LoadStudy(UpdateContext);
		}

		private void SetStudyEncoding(Study study)
		{
			// set the SpecificCharacterSet of the patient and study record. This will update the database
			// and force Patient/Study/Series level query response to be encoded in UTF8. Image level responses
			// will be encoded using the character set in the image (see QueryScpExtension) 
			//
			if (atLeastOneFileUpdatedToUTF8)
			{
				// Only update the db if necessary                
				if (!IsUTF8(study.SpecificCharacterSet))
				{
					Platform.Log(LogLevel.Info, "Updating encoding for study information in the database to UTF8 [ UID={0} ]",
					             study.StudyInstanceUid);
					study.SpecificCharacterSet = UTF8; // db update happens later
				}
			}
		}
		

		private void UpdateDatabase()
		{
			// Reload the StudyStorage and Study tables.
			LoadEntities();

			UpdateEntity(_study);
			UpdateEntity(_curPatient);
			UpdateEntity(_storage);

			SetStudyEncoding(_study);

			var order = FindOrderForStudy();
			_study.OrderKey = order == null ? null : order.Key;

			// Update the Study table
			var studyUpdateBroker = UpdateContext.GetBroker<IStudyEntityBroker>();
			studyUpdateBroker.Update(_study);

			// Update the StudyStorage table
			var storageUpdateBroker = UpdateContext.GetBroker<IStudyStorageEntityBroker>();
			storageUpdateBroker.Update(_storage);

			// Update Patient level info. Different cases can occur here: 
			//      A) Patient demographic info is not changed ==> update the current patient
			//      B) New patient demographics matches (another) existing patient in the datbase 
			//              ==> Transfer the study to that patient. This means the study count on both patients must be updated.
			//                  The current patient should also be deleted if there's no more study attached to it after the transfer.
			//      C) New patient demographics doesn't match any patient in the database
			//              ==> A new patient should be created for this study. The study count on the current patient should be updated
			//                  and the patient should also be deleted if this is the only study attached to it.
			if (!_patientInfoChanged)
			{
				UpdatePatientDemographics(_curPatient.GetKey(), _newPatientInfo);
			}
			else if (_newPatient == null)
			{
				// No matching patient in the database. We should create a new patient for this study
				_newPatient = CreateNewPatient(_newPatientInfo);
				UpdatePatientDemographics(_newPatient.GetKey(), _newPatientInfo);
			}
			else
			{
				// There's already patient in the database with the new patient demographics
				// The study should be attached to that patient.
				TransferStudy(_study.Key, _oldPatientInfo, _newPatient);
				UpdatePatientDemographics(_newPatient.GetKey(), _newPatientInfo);
			}
		}

		private void UpdatePatientDemographics(ServerEntityKey patientEntityKey, PatientInfo patientInfo)
		{
			Platform.Log(LogLevel.Info, "Update patient record...");
			var patientUpdateBroker = UpdateContext.GetBroker<IPatientEntityBroker>();
			var columns = new PatientUpdateColumns();
			columns.IssuerOfPatientId = patientInfo.IssuerOfPatientId;
			columns.PatientId = patientInfo.PatientId;
			columns.PatientsName = patientInfo.PatientsName;
			if (atLeastOneFileUpdatedToUTF8) 
				columns.SpecificCharacterSet = UTF8;
			
			patientUpdateBroker.Update(patientEntityKey, columns);

		}

		private Order FindOrderForStudy()
		{
			var select = new OrderSelectCriteria();
			select.ServerPartitionKey.EqualTo(_study.ServerPartitionKey);
			select.AccessionNumber.EqualTo(_study.AccessionNumber);
			select.PatientId.EqualTo(_study.PatientId);

			var broker = UpdateContext.GetBroker<IOrderEntityBroker>();
			return broker.FindOne(select);
		}

		private Patient CreateNewPatient(PatientInfo patientInfo)
		{
			Platform.Log(LogLevel.Info, "Creating new patient {0}", patientInfo.PatientId);

			var createStudyBroker = UpdateContext.GetBroker<ICreatePatientForStudy>();
			var parms = new CreatePatientForStudyParameters
				{
					IssuerOfPatientId = patientInfo.IssuerOfPatientId,
					PatientId = patientInfo.PatientId,
					PatientsName = patientInfo.PatientsName,
					SpecificCharacterSet = _curPatient.SpecificCharacterSet, // this will be updated at the end if necessary
					StudyKey = _study.GetKey()
				};
			Patient newPatient = createStudyBroker.FindOne(parms);
			if (newPatient == null)
				throw new ApplicationException("Unable to create patient for the study");

			return newPatient;
		}


		private void TransferStudy(ServerEntityKey studyKey, PatientInfo oldPatient, Patient newPatient)
		{
			Platform.Log(LogLevel.Info, "Transferring study from {0} [ID={1}] to {2} [ID={3}]",
			             oldPatient.PatientsName, oldPatient.PatientId, newPatient.PatientsName, newPatient.PatientId);

			var attachStudyToPatientBroker = UpdateContext.GetBroker<IAttachStudyToPatient>();
			var parms = new AttachStudyToPatientParamaters
				{
					StudyKey = studyKey,
					NewPatientKey = newPatient.GetKey()
				};
			attachStudyToPatientBroker.Execute(parms);
		}

		private void UpdateFilesystem()
		{
			Platform.Log(LogLevel.Info, "Updating filesystem...");
			StudyXml studyXml = _oldStudyLocation.LoadStudyXml();
			StudyXmlOutputSettings outputSettings = ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings;

			var newStudyXml = new StudyXml();
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					string path = Path.Combine(_oldStudyPath, seriesXml.SeriesInstanceUid);
					path = Path.Combine(path, instanceXml.SopInstanceUid);
					path += ServerPlatform.DicomFileExtension;

					if (!File.Exists(path))
					{
						Platform.Log(LogLevel.Info, "SOP {0} is referenced in study xml but does not exist. It will be removed");
						continue; // file was removed but xml was not updated?
					}

					try
					{
						var file = new DicomFile(path);
						file.Load();

						var instance = new InstanceInfo
							{
								SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty),
								SopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty)
							};

						UpdateDicomFile(file);

						// Add into the temporary study xml
						long fileSize = 0;
						var finfo = new FileInfo(file.Filename);
						if (finfo.Exists)
							fileSize = finfo.Length;
						newStudyXml.AddFile(file, fileSize, outputSettings);


						_updatedSopList.Add(instance);
						Platform.Log(ServerPlatform.InstanceLogLevel, "SOP {0} has been updated [{1} of {2}].", instance.SopInstanceUid,
						             _updatedSopList.Count, _totalSopCount);

						EventManager.FireEvent(this,
						                       new UpdateSopEventArgs
							                       {
								                       File = file,
								                       ServerPartitionEntry = _partition,
								                       WorkQueueUidEntry = null,
								                       WorkQueueEntry = _workQueue,
								                       FileLength = (ulong) fileSize
							                       });

					}
					catch (Exception)
					{
						FileUtils.Delete(Path.Combine(_backupDir, instanceXml.SopInstanceUid) + ".bak"); //dont' need to restore this file
						throw;
					}
				}
			}

			// Log any study-level warnings
			if (_updatedSopList.Count != _totalSopCount)
			{
				Platform.Log(LogLevel.Warn, "Inconsistent data: expected {0} instances to be updated / Found {1}.", _totalSopCount,
				             _updatedSopList.Count);
			}

			// update the header
			Platform.Log(LogLevel.Info, "Generating new study header...");
			string newStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml");
			string gzipStudyXmlPath = Path.Combine(NewStudyPath, _newStudyInstanceUid + ".xml.gz");
			using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(newStudyXmlPath, FileMode.Create),
			                  gzipStream = FileStreamOpener.OpenForSoleUpdate(gzipStudyXmlPath, FileMode.Create))
			{
				StudyXmlIo.WriteXmlAndGzip(newStudyXml.GetMemento(outputSettings), xmlStream, gzipStream);
				xmlStream.Close();
				gzipStream.Close();
			}
		}

		private void UpdateDicomFile(DicomFile file)
		{
			var originalCS = file.DataSet.SpecificCharacterSet;

			var sq = new OriginalAttributesSequence
				{
					ModifiedAttributesSequence = new DicomSequenceItem(),
					ModifyingSystem = ProductInformation.Component,
					ReasonForTheAttributeModification = "CORRECT",
					AttributeModificationDatetime = Platform.Time,
					SourceOfPreviousValues = file.SourceApplicationEntityTitle
				};

			foreach (BaseImageLevelUpdateCommand command in _commands)
			{
				command.File = file;
				command.Apply(file, sq);
			}

			var sqAttrib = file.DataSet[DicomTags.OriginalAttributesSequence] as DicomAttributeSQ;
			if (sqAttrib != null)
				sqAttrib.AddSequenceItem(sq.DicomSequenceItem);

			var newCS = file.DataSet.SpecificCharacterSet;

			if (!string.Equals(originalCS, newCS))
			{
				if (Platform.IsLogLevelEnabled(ServerPlatform.InstanceLogLevel))
				{
					Platform.Log(ServerPlatform.InstanceLogLevel, "Specific Character Set for SOP {0} has been updated [{1}->{2}]",
					             file.MediaStorageSopInstanceUid, originalCS, newCS);
				}

				if (!Common.Settings.Default.AllowedConvertToUnicodeOnEdit)
				{
					Platform.Log(LogLevel.Warn, "File was converted to unicode but AllowedConvertToUnicodeOnEdit is false");
				}
				atLeastOneFileUpdatedToUTF8 = atLeastOneFileUpdatedToUTF8 || IsUTF8(newCS);
			}

			SaveFile(file);
		}

		private void SaveFile(DicomFile file)
		{
			String seriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
			String sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

			String destPath = _oldStudyLocation.FilesystemPath;

			using (var filesystemUpdateProcessor = new ServerCommandProcessor("Update Study"))
			{
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, _partition.PartitionFolder);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, _oldStudyFolder);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, _newStudyInstanceUid);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, seriesInstanceUid);
				filesystemUpdateProcessor.AddCommand(new CreateDirectoryCommand(destPath));

				destPath = Path.Combine(destPath, sopInstanceUid);
				destPath += ServerPlatform.DicomFileExtension;

				// Overwrite the prior file
				var saveCommand = new SaveDicomFileCommand(destPath, file, false);
				filesystemUpdateProcessor.AddCommand(saveCommand);

				if (_rulesEngine != null)
				{
					var context = new ServerActionContext(file, _oldStudyLocation.FilesystemKey, _partition, _oldStudyLocation.Key,
					                                      filesystemUpdateProcessor);
					_rulesEngine.Execute(context);
				}

				if (!filesystemUpdateProcessor.Execute())
				{
					throw new ApplicationException(String.Format("Unable to update image {0} : {1}", file.Filename,
					                                             filesystemUpdateProcessor.FailureReason));
				}
			}
		}

		private void BackupFilesystem()
		{
			Platform.Log(LogLevel.Info, "Backing up current study folder to {0}", _backupDir);
			StudyXml studyXml = _oldStudyLocation.LoadStudyXml();
			FileUtils.Copy(_oldStudyLocation.GetStudyXmlPath(), Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml"), true);
			FileUtils.Copy(_oldStudyLocation.GetCompressedStudyXmlPath(),
			               Path.Combine(_backupDir, _study.StudyInstanceUid + ".xml.gz"), true);

			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					string existingFile = _oldStudyLocation.GetSopInstancePath(seriesXml.SeriesInstanceUid, instanceXml.SopInstanceUid);

					var backupPath = new FileInfo(Path.Combine(_backupDir, instanceXml.SopInstanceUid + ServerPlatform.DicomFileExtension));
					FileUtils.Copy(existingFile, backupPath.FullName, true);
				}
			}

			Platform.Log(LogLevel.Info, "A copy of {0} has been saved in {1}.", _oldStudyInstanceUid, _backupDir);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (RollBackRequested)
			{
				if (_restored)
					CleanupBackupFiles();
			}
			else
			{
				if (NewStudyPath != _oldStudyPath && _deleteOriginalFolder)
				{
					Platform.Log(LogLevel.Info, "Removing old study folder...");
					DirectoryUtility.DeleteIfExists(_oldStudyPath, true);
				}

				CleanupBackupFiles();
			}
		}

		#endregion
	}
}