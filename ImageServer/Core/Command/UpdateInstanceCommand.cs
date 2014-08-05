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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Command
{

	public class UpdateInstanceCommand : ServerDatabaseCommand, IDisposable
	{
		const string UTF8 = "ISO_IR 192";

		#region Private Members

		private readonly StudyStorageLocation _studyLocation;
		private string _seriesInstanceUid;
		private PatientInfo _oldPatientInfo;
		private PatientInfo _newPatientInfo;
		private readonly ServerPartition _partition;
		private Study _study;
		private Patient _curPatient;
		private Patient _newPatient;
		private Series _curSeries;
		private readonly DicomFile _file;

		private readonly UpdateStudyStatistics _statistics;

		private bool _patientInfoIsNotChanged;

		#endregion

		#region Constructors

		public UpdateInstanceCommand(ServerPartition partition,
								  StudyStorageLocation studyLocation,
								  DicomFile file)
			: base("Update existing SOP Instance")
		{
			_partition = partition;
			_studyLocation = studyLocation;
			_file = file;
		}

		#endregion

		#region Properties

		public string NewStudyPath { get; set; }

		#endregion

		#region Protected Method

		protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
		{
			Initialize();

			UpdateDatabase();
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			_study = Study.Find(ServerExecutionContext.Current.ReadContext,_studyLocation.StudyInstanceUid, _partition);
			_curPatient = _study.LoadPatient(ServerExecutionContext.Current.ReadContext);
			_oldPatientInfo = new PatientInfo
				{
					PatientsName = _curPatient.PatientsName,
					PatientId = _curPatient.PatientId,
					IssuerOfPatientId = _curPatient.IssuerOfPatientId
				};

			_seriesInstanceUid = _file.DataSet[DicomTags.SeriesInstanceUid].ToString();
			var broker = ServerExecutionContext.Current.ReadContext.GetBroker<ISeriesEntityBroker>();
			var select = new SeriesSelectCriteria();
			select.SeriesInstanceUid.EqualTo(_seriesInstanceUid);
			select.ServerPartitionKey.EqualTo(_partition.Key);
			_curSeries = broker.FindOne(select);

			_newPatientInfo = new PatientInfo(_oldPatientInfo);
			_file.DataSet.LoadDicomFields(_newPatientInfo);

			_newPatient = FindPatient(_newPatientInfo, ServerExecutionContext.Current.ReadContext);
			_patientInfoIsNotChanged = _newPatientInfo.Equals(_oldPatientInfo);
		}

		private Patient FindPatient(PatientInfo patientInfo, IPersistenceContext context)
		{
			var patientFindBroker = context.GetBroker<IPatientEntityBroker>();
			var criteria = new PatientSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(_partition.Key);
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

			
		private static bool IsUTF8(string characterSet)
		{
			return string.Equals(characterSet, UTF8, StringComparison.InvariantCulture);
		}

		private void SetStudyEncoding(Study study, StudyUpdateColumns update)
		{
			// set the SpecificCharacterSet of the patient and study record. This will update the database
			// and force Patient/Study/Series level query response to be encoded in UTF8. Image level responses
			// will be encoded using the character set in the image (see QueryScpExtension) 
			//
			// Only update the db if necessary                
			if (IsUTF8(study.SpecificCharacterSet))
			{
				update.SpecificCharacterSet = UTF8;
			}
		}

		private void UpdatePatientEncoding(Patient patient, PatientUpdateColumns update)
		{
			// set the SpecificCharacterSet of the patient and study record. This will update the database
			// and force Patient/Study/Series level query response to be encoded in UTF8. Image level responses
			// will be encoded using the character set in the image (see QueryScpExtension) 
			//
			// Only update the db if necessary
			if (IsUTF8(patient.SpecificCharacterSet))
			{
				update.SpecificCharacterSet = UTF8;
			}
		}


		private void UpdateDatabase()
		{
			var patientUpdate = new PatientUpdateColumns();
			var seriesUpdate = new SeriesUpdateColumns();
			var studyUpdate = new StudyUpdateColumns();

			// Update Patient level info. Different cases can occur here: 
			//      A) Patient demographic info is not changed ==> update the current patient
			//      B) New patient demographics matches (another) existing patient in the datbase 
			//              ==> Transfer the study to that patient. This means the study count on both patients must be updated.
			//                  The current patient should also be deleted if there's no more study attached to it after the transfer.
			//      C) New patient demographics doesn't match any patient in the database
			//              ==> A new patient should be created for this study. The study count on the current patient should be updated
			//                  and the patient should also be deleted if this is the only study attached to it.
			if (_patientInfoIsNotChanged)
			{
				_newPatient = _curPatient;
			}
			else if (_newPatient == null)
			{
				// No matching patient in the database. We should create a new patient for this study
				_newPatient = CreateNewPatient(_newPatientInfo);
			}
			else
			{
				// There's already patient in the database with the new patient demographics
				// The study should be attached to that patient.
				TransferStudy(_study.Key, _oldPatientInfo, _newPatient);
			}

			// Copy the existing valus over into the study & series objects
			// Note, this sets up an update statement that will update the key columns for
			// Study Instance UID, Series Instance UID, however, note that the columns will not
			// actually change value.  Its alittle ugly, but it will make it so if we add new
			// columns in the future, it just "works".
			_file.DataSet.LoadDicomFields(patientUpdate);
			_file.DataSet.LoadDicomFields(studyUpdate);
			_file.DataSet.LoadDicomFields(seriesUpdate);

			// Get any extensions that exist and process them
			var ep = new ProcessorInsertExtensionPoint();
			var extensions = ep.CreateExtensions();
			foreach (IInsertExtension e in extensions)
				e.UpdateExtension(_partition.Key, patientUpdate,studyUpdate,seriesUpdate, _file);

			UpdatePatientEncoding(_newPatient, patientUpdate);
			SetStudyEncoding(_study, studyUpdate);

			// Update the Study table
			var patientUpdateBroker = UpdateContext.GetBroker<IPatientEntityBroker>();
			patientUpdateBroker.Update(_newPatient.Key, patientUpdate);

			// Update the Study table
			var studyUpdateBroker = UpdateContext.GetBroker<IStudyEntityBroker>();
			studyUpdateBroker.Update(_study.Key, studyUpdate);

			// Update the Series table
			var seriesUpdateBroker = UpdateContext.GetBroker<ISeriesEntityBroker>();
			seriesUpdateBroker.Update(_curSeries.Key, seriesUpdate);

			// If the Request Attributes Sequence is in the dataset, do an insert.
			// Small hole in this that if the value of this sequence has changed, both the old and
			// the new values will stay in the database, not much to do about it, except
			// reprocess the whole series, which doesn't seem worth it.
			if (_file.DataSet.Contains(DicomTags.RequestAttributesSequence))
			{
				var attribute = _file.DataSet[DicomTags.RequestAttributesSequence] as DicomAttributeSQ;
				if (attribute != null && !attribute.IsEmpty)
				{
					foreach (DicomSequenceItem sequenceItem in (DicomSequenceItem[])attribute.Values)
					{
						var requestParms = new RequestAttributesInsertParameters();
						sequenceItem.LoadDicomFields(requestParms);
						requestParms.SeriesKey = _curSeries.Key;

						var insertRequest = UpdateContext.GetBroker<IInsertRequestAttributes>();
						insertRequest.Execute(requestParms);
					}
				}
			}
		}

		private Patient CreateNewPatient(PatientInfo patientInfo)
		{
			Platform.Log(LogLevel.Info, "Creating new patient {0}", patientInfo.PatientId);

			var createPatientForStudy = UpdateContext.GetBroker<ICreatePatientForStudy>();
			var parms = new CreatePatientForStudyParameters
			{
				IssuerOfPatientId = patientInfo.IssuerOfPatientId,
				PatientId = patientInfo.PatientId,
				PatientsName = patientInfo.PatientsName,
				SpecificCharacterSet = _curPatient.SpecificCharacterSet, // this will be updated at the end if necessary
				StudyKey = _study.GetKey()
			};
			Patient newPatient = createPatientForStudy.FindOne(parms);
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

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
