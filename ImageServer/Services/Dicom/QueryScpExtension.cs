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
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    ///<summary>
    /// Plugin for handling DICOM Query Requests implementing the <see cref="IDicomScp{TContext}"/> interface.
    ///</summary>
    [ExtensionOf(typeof (DicomScpExtensionPoint<DicomScpContext>))]
    public class QueryScpExtension : BaseScp
    {
        #region Private members

        private readonly List<SupportedSop> _list = new List<SupportedSop>();
		private bool _cancelReceived;
		private readonly Queue<DicomMessage> _responseQueue = new Queue<DicomMessage>(DicomSettings.Default.BufferedQueryResponses);
		private readonly object _syncLock = new object();
        private readonly List<IExtendedQueryKeys> _queryExtensions = new List<IExtendedQueryKeys>();
	    private readonly int _bufferedQueryResponses = DicomSettings.Default.BufferedQueryResponses;
	    private readonly int _maxQueryResponses = DicomSettings.Default.MaxQueryResponses;
	    private readonly bool _cfindRspAlwaysInUnicode = DicomSettings.Default.CFINDRspAlwaysInUnicode;
        #endregion

		#region Public Properties
    	public bool CancelReceived
    	{
    		get
    		{
				lock (_syncLock)
					return _cancelReceived;
    		}
    		set
    		{
				lock (_syncLock)
					_cancelReceived = value;
    		}
    	}
		#endregion

		#region Contructors

		/// <summary>
        /// Public default constructor.  Implements the Find and Move services for 
        /// Patient Root and Study Root queries.
        /// </summary>
        public QueryScpExtension()
		{
		    var sop = new SupportedSop
		                  {
		                      SopClass = SopClass.PatientRootQueryRetrieveInformationModelFind
		                  };
			sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);

            sop = new SupportedSop
                  	{
                  		SopClass = SopClass.StudyRootQueryRetrieveInformationModelFind
                  	};
			sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);

            var ep = new QueryKeysExtensionPoint();
            foreach (IExtendedQueryKeys q in ep.CreateExtensions())
                _queryExtensions.Add(q);
		}

        #endregion

        #region Private Methods

		/// <summary>
		/// Helper method for logging audit information.
		/// </summary>
		/// <param name="parms"></param>
		/// <param name="outcome"></param>
		/// <param name="msg">The query message to be audited</param>
		private static void AuditLog(AssociationParameters parms, EventIdentificationContentsEventOutcomeIndicator outcome, DicomMessage msg)
		{
		    try
		    {
                var helper = new QueryAuditHelper(ServerPlatform.AuditSource,
                                              outcome, parms, msg.AffectedSopClassUid, msg.DataSet);
                ServerAuditHelper.LogAuditMessage(helper);
		    }
		    catch (Exception e)
		    {
		        Platform.Log(LogLevel.Warn, "Unexpected exception logging DICOM Query audit message: {0}", e.Message);
		    }
		}

        /// <summary>
        /// Load the values for the tag <see cref="DicomTags.ModalitiesInStudy"/> into a response
        /// message for a specific <see cref="Study"/>.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="response">The message to add the value into.</param>
        /// <param name="key">The <see cref="ServerEntityKey"/> for the <see cref="Study"/>.</param>
        private static void LoadModalitiesInStudy(IPersistenceContext read, DicomMessageBase response, ServerEntityKey key)
        {
            var select = read.GetBroker<IQueryModalitiesInStudy>();

            var parms = new ModalitiesInStudyQueryParameters {StudyKey = key};

        	var list = select.Find(parms);

            var value = string.Empty;
            foreach (Series series in list)
            {
                value = value.Length == 0 
					? series.Modality 
					: String.Format("{0}\\{1}", value, series.Modality);
            }
            response.DataSet[DicomTags.ModalitiesInStudy].SetStringValue(value);
        }

        /// <summary>
        /// Load the values for the sequence <see cref="DicomTags.RequestAttributesSequence"/>
        /// into a response message for a specific series.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="response">The message to add the values into.</param>
        /// <param name="row">The <see cref="Series"/> entity to load the related <see cref="RequestAttributes"/> entity for.</param>
        private static void LoadRequestAttributes(IPersistenceContext read, DicomMessageBase response, Series row)
        {
			var select = read.GetBroker<IRequestAttributesEntityBroker>();

			var criteria = new RequestAttributesSelectCriteria();

            criteria.SeriesKey.EqualTo(row.Key);

            var list = select.Find(criteria);

            if (list.Count == 0)
            {
                response.DataSet[DicomTags.RequestAttributesSequence].SetNullValue();
                return;
            }

            foreach (RequestAttributes request in list)
            {
                var item = new DicomSequenceItem();
                item[DicomTags.ScheduledProcedureStepId].SetStringValue(request.ScheduledProcedureStepId);
                item[DicomTags.RequestedProcedureId].SetStringValue(request.RequestedProcedureId);

                response.DataSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);
            }
        }

        /// <summary>
        /// Find the <see cref="ServerEntityKey"/> reference for a given Study Instance UID and Server Partition.
        /// </summary>
		/// <param name="read">The connection to use to read the values.</param>
        /// <param name="studyInstanceUid">The list of Study Instance Uids for which to retrieve the table keys.</param>
        /// <returns>A list of <see cref="ServerEntityKey"/>s.</returns>
        private List<ServerEntityKey> LoadStudyKey(IPersistenceContext read, string[] studyInstanceUid)
        {
            var find = read.GetBroker<IStudyEntityBroker>();

            var criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(Partition.Key);
            if (studyInstanceUid.Length > 1)
                criteria.StudyInstanceUid.In(studyInstanceUid);
            else
                criteria.StudyInstanceUid.EqualTo(studyInstanceUid[0]);

            IList<Study> list = find.Find(criteria);

            var serverList = new List<ServerEntityKey>();

            foreach (Study row in list)
                serverList.Add(row.GetKey());

            return serverList;
        }

        /// <summary>
        /// Populate data from a <see cref="Patient"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
        /// <param name="response">The response message to populate with results.</param>
        /// <param name="tagList">The list of tags to populate.</param>
        /// <param name="row">The <see cref="Patient"/> table to populate from.</param>
        private void PopulatePatient(DicomMessageBase response, IEnumerable<DicomTag> tagList, Patient row)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            //dataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

            var characterSet = GetPreferredCharacterSet();
            if (!string.IsNullOrEmpty(characterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(characterSet);
                dataSet.SpecificCharacterSet = characterSet; 
            }
            else if (false == String.IsNullOrEmpty(row.SpecificCharacterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(row.SpecificCharacterSet);
                dataSet.SpecificCharacterSet = row.SpecificCharacterSet; // this will ensure the data is encoded using the specified character set
            }

        	IList<Study> relatedStudies = row.LoadRelatedStudies();
        	Study study = null;
			if (relatedStudies.Count > 0)
				study = CollectionUtils.FirstElement(relatedStudies);

            foreach (DicomTag tag in tagList)
            {
                try
                {
                    switch (tag.TagValue)
                    {
                        case DicomTags.PatientsName:
                            dataSet[DicomTags.PatientsName].SetStringValue(row.PatientsName);
                            break;
                        case DicomTags.PatientId:
                            dataSet[DicomTags.PatientId].SetStringValue(row.PatientId);
                            break;
                        case DicomTags.IssuerOfPatientId:
                            dataSet[DicomTags.IssuerOfPatientId].SetStringValue(row.IssuerOfPatientId);
                            break;
                        case DicomTags.NumberOfPatientRelatedStudies:
                            dataSet[DicomTags.NumberOfPatientRelatedStudies].AppendInt32(row.NumberOfPatientRelatedStudies);
                            break;
                        case DicomTags.NumberOfPatientRelatedSeries:
                            dataSet[DicomTags.NumberOfPatientRelatedSeries].AppendInt32(row.NumberOfPatientRelatedSeries);
                            break;
                        case DicomTags.NumberOfPatientRelatedInstances:
                            dataSet[DicomTags.NumberOfPatientRelatedInstances].AppendInt32(
                                row.NumberOfPatientRelatedInstances);
                            break;
                        case DicomTags.QueryRetrieveLevel:
                            dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("PATIENT");
                            break;
						case DicomTags.PatientsSex:
							if (study == null)
								dataSet[DicomTags.PatientsSex].SetNullValue();
							else
								dataSet[DicomTags.PatientsSex].SetStringValue(study.PatientsSex);
                    		break;
						case DicomTags.PatientsBirthDate:
							if (study == null)
								dataSet[DicomTags.PatientsBirthDate].SetNullValue();
							else
								dataSet[DicomTags.PatientsBirthDate].SetStringValue(study.PatientsBirthDate);
							break;
                    
						// Meta tags that should have not been in the RQ, but we've already set
						case DicomTags.RetrieveAeTitle:
						case DicomTags.InstanceAvailability:
						case DicomTags.SpecificCharacterSet:
                    		break;

                        default:
                            if (!tag.IsPrivate)
                                dataSet[tag].SetNullValue();

                            foreach (var q in _queryExtensions)
                                q.PopulatePatient(response, tag, row, study);

                            break;
                    }
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Unexpected error setting tag {0} in C-FIND-RSP",
                                 dataSet[tag].Tag.ToString());
                    if (!tag.IsPrivate)
                        dataSet[tag].SetNullValue();
                }
            }
        }

        /// <summary>
        /// Populate the data from a <see cref="Study"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
		/// <param name="read">The connection to use to read the values.</param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
	    /// <param name="row">The <see cref="Study"/> table to populate the response from.</param>
	    /// <param name="availability">Instance availability string.</param>
        private void PopulateStudy(IPersistenceContext read, DicomMessageBase response, IEnumerable<DicomTag> tagList, Study row, string availability)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);

			dataSet[DicomTags.InstanceAvailability].SetStringValue(availability);

            var characterSet = GetPreferredCharacterSet();
            if (!string.IsNullOrEmpty(characterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(characterSet);
                dataSet.SpecificCharacterSet = characterSet;
            }
            else if (false == String.IsNullOrEmpty(row.SpecificCharacterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(row.SpecificCharacterSet);
                dataSet.SpecificCharacterSet = row.SpecificCharacterSet; // this will ensure the data is encoded using the specified character set
            }
            foreach (DicomTag tag in tagList)
            {
                try
                {
                    switch (tag.TagValue)
                    {
                        case DicomTags.StudyInstanceUid:
                            dataSet[DicomTags.StudyInstanceUid].SetStringValue(row.StudyInstanceUid);
                            break;
                        case DicomTags.PatientsName:
                            dataSet[DicomTags.PatientsName].SetStringValue(row.PatientsName);
                            break;
                        case DicomTags.PatientId:
                            dataSet[DicomTags.PatientId].SetStringValue(row.PatientId);
                            break;
                        case DicomTags.PatientsBirthDate:
                            dataSet[DicomTags.PatientsBirthDate].SetStringValue(row.PatientsBirthDate);
                            break;
						case DicomTags.PatientsAge:
							dataSet[DicomTags.PatientsAge].SetStringValue(row.PatientsAge);
							break;
						case DicomTags.PatientsSex:
                            dataSet[DicomTags.PatientsSex].SetStringValue(row.PatientsSex);
                            break;
                        case DicomTags.StudyDate:
                            dataSet[DicomTags.StudyDate].SetStringValue(row.StudyDate);
                            break;
                        case DicomTags.StudyTime:
                            dataSet[DicomTags.StudyTime].SetStringValue(row.StudyTime);
                            break;
                        case DicomTags.AccessionNumber:
                            dataSet[DicomTags.AccessionNumber].SetStringValue(row.AccessionNumber);
                            break;
                        case DicomTags.StudyId:
                            dataSet[DicomTags.StudyId].SetStringValue(row.StudyId);
                            break;
                        case DicomTags.StudyDescription:
                            dataSet[DicomTags.StudyDescription].SetStringValue(row.StudyDescription);
                            break;
                        case DicomTags.ReferringPhysiciansName:
                            dataSet[DicomTags.ReferringPhysiciansName].SetStringValue(row.ReferringPhysiciansName);
                            break;
                        case DicomTags.NumberOfStudyRelatedSeries:
                            dataSet[DicomTags.NumberOfStudyRelatedSeries].AppendInt32(row.NumberOfStudyRelatedSeries);
                            break;
                        case DicomTags.NumberOfStudyRelatedInstances:
                            dataSet[DicomTags.NumberOfStudyRelatedInstances].AppendInt32(
                                row.NumberOfStudyRelatedInstances);
                            break;
                        case DicomTags.ModalitiesInStudy:
                            LoadModalitiesInStudy(read, response, row.Key);
                            break;
                        case DicomTags.QueryRetrieveLevel:
                            dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
                            break;
						// Meta tags that should have not been in the RQ, but we've already set
						case DicomTags.RetrieveAeTitle:
						case DicomTags.InstanceAvailability:
						case DicomTags.SpecificCharacterSet:
							break;
                        default:
                            if (!tag.IsPrivate)
                                dataSet[tag].SetNullValue();

                            foreach (var q in _queryExtensions)
                                q.PopulateStudy(response, tag, row);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Unexpected error setting tag {0} in C-FIND-RSP",
                                 dataSet[tag].Tag.ToString());
                    if (!tag.IsPrivate)
                        dataSet[tag].SetNullValue();
                }
            }
        }

        /// <summary>
        /// Populate the data from a <see cref="Series"/> entity into a DICOM C-FIND-RSP message.
        /// </summary>
		/// <param name="read">The connection to use to read the values.</param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="row">The <see cref="Series"/> table to populate the row from.</param>
        private void PopulateSeries(IPersistenceContext read, DicomMessageBase request, DicomMessageBase response, IEnumerable<DicomTag> tagList,
                                    Series row)
        {
            DicomAttributeCollection dataSet = response.DataSet;

        	Study theStudy = Study.Load(read, row.StudyKey);
        	StudyStorage storage = StudyStorage.Load(read, theStudy.ServerPartitionKey, theStudy.StudyInstanceUid);
            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue(storage.StudyStatusEnum == StudyStatusEnum.Nearline
                                                                       ? "NEARLINE"
                                                                       : "ONLINE");

            var characterSet = GetPreferredCharacterSet();
            if (!string.IsNullOrEmpty(characterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(characterSet);
                dataSet.SpecificCharacterSet = characterSet;
            }
            else if (false == String.IsNullOrEmpty(theStudy.SpecificCharacterSet))
			{
				dataSet[DicomTags.SpecificCharacterSet].SetStringValue(theStudy.SpecificCharacterSet);
				dataSet.SpecificCharacterSet = theStudy.SpecificCharacterSet; // this will ensure the data is encoded using the specified character set
			}

            foreach (DicomTag tag in tagList)
            {
                try
                {
                    switch (tag.TagValue)
                    {
                        case DicomTags.PatientId:
                            dataSet[DicomTags.PatientId].SetStringValue(request.DataSet[DicomTags.PatientId].ToString());
                            break;
                        case DicomTags.StudyInstanceUid:
                            dataSet[DicomTags.StudyInstanceUid].SetStringValue(
                                request.DataSet[DicomTags.StudyInstanceUid].ToString());
                            break;
                        case DicomTags.SeriesInstanceUid:
                            dataSet[DicomTags.SeriesInstanceUid].SetStringValue(row.SeriesInstanceUid);
                            break;
                        case DicomTags.Modality:
                            dataSet[DicomTags.Modality].SetStringValue(row.Modality);
                            break;
                        case DicomTags.SeriesNumber:
                            dataSet[DicomTags.SeriesNumber].SetStringValue(row.SeriesNumber);
                            break;
                        case DicomTags.SeriesDescription:
                            dataSet[DicomTags.SeriesDescription].SetStringValue(row.SeriesDescription);
                            break;
                        case DicomTags.PerformedProcedureStepStartDate:
                            dataSet[DicomTags.PerformedProcedureStepStartDate].SetStringValue(
                                row.PerformedProcedureStepStartDate);
                            break;
                        case DicomTags.PerformedProcedureStepStartTime:
                            dataSet[DicomTags.PerformedProcedureStepStartTime].SetStringValue(
                                row.PerformedProcedureStepStartTime);
                            break;
                        case DicomTags.NumberOfSeriesRelatedInstances:
                            dataSet[DicomTags.NumberOfSeriesRelatedInstances].AppendInt32(row.NumberOfSeriesRelatedInstances);
                            break;
                        case DicomTags.RequestAttributesSequence:
                            LoadRequestAttributes(read, response, row);
                            break;
                        case DicomTags.QueryRetrieveLevel:
                            dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("SERIES");
                            break;
						// Meta tags that should have not been in the RQ, but we've already set
						case DicomTags.RetrieveAeTitle:
						case DicomTags.InstanceAvailability:
						case DicomTags.SpecificCharacterSet:
							break;
                        default:
                            if (!tag.IsPrivate)
                                dataSet[tag].SetNullValue();

                            foreach (var q in _queryExtensions)
                                q.PopulateSeries(response, tag, row);

                            break;
                    }
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Unexpected error setting tag {0} in C-FIND-RSP",
                                 dataSet[tag].Tag.ToString());
                    if (!tag.IsPrivate)
                        dataSet[tag].SetNullValue();
                }
            }
        }

        /// <summary>
        /// Populate at the IMAGE level a response message.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="tagList"></param>
        /// <param name="theInstanceStream"></param>
        private void PopulateInstance(DicomMessageBase request, DicomMessageBase response, IEnumerable<DicomTag> tagList,
                                      InstanceXml theInstanceStream)
        {
            DicomAttributeCollection dataSet = response.DataSet;

            dataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
            dataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

            DicomAttributeCollection sourceDataSet = theInstanceStream.Collection;

            var characterSet = GetPreferredCharacterSet();
            if (!string.IsNullOrEmpty(characterSet))
            {
                dataSet[DicomTags.SpecificCharacterSet].SetStringValue(characterSet);
                dataSet.SpecificCharacterSet = characterSet;
            }
            else if (sourceDataSet.Contains(DicomTags.SpecificCharacterSet))
			{
				dataSet[DicomTags.SpecificCharacterSet].SetStringValue(sourceDataSet[DicomTags.SpecificCharacterSet].ToString());
				dataSet.SpecificCharacterSet = sourceDataSet[DicomTags.SpecificCharacterSet].ToString(); // this will ensure the data is encoded using the specified character set
			}

            foreach (DicomTag tag in tagList)
            {
                try
                {
                    switch (tag.TagValue)
                    {
                        case DicomTags.PatientId:
                            dataSet[DicomTags.PatientId].SetStringValue(request.DataSet[DicomTags.PatientId].ToString());
                            break;
                        case DicomTags.StudyInstanceUid:
                            dataSet[DicomTags.StudyInstanceUid].SetStringValue(
                                request.DataSet[DicomTags.StudyInstanceUid].ToString());
                            break;
                        case DicomTags.SeriesInstanceUid:
                            dataSet[DicomTags.SeriesInstanceUid].SetStringValue(
                                request.DataSet[DicomTags.SeriesInstanceUid].ToString());
                            break;
                        case DicomTags.QueryRetrieveLevel:
                            dataSet[DicomTags.QueryRetrieveLevel].SetStringValue("IMAGE");
                            break;
                        default:
                            if (sourceDataSet.Contains(tag))
                                dataSet[tag] = sourceDataSet[tag].Copy();
                            else if (!tag.IsPrivate)                           
                                dataSet[tag].SetNullValue();
                            break;
						// Meta tags that should have not been in the RQ, but we've already set
						case DicomTags.RetrieveAeTitle:
						case DicomTags.InstanceAvailability:
						case DicomTags.SpecificCharacterSet:
							break;
					}
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Unexpected error setting tag {0} in C-FIND-RSP",
                                 dataSet[tag].Tag.ToString());
                    if (!tag.IsPrivate)
                        dataSet[tag].SetNullValue();
                }
            }
        }

		private void SendBufferedResponses(DicomServer server, byte presentationId, DicomMessage requestMessage)
		{
			while (_responseQueue.Count > 0)
			{
				DicomMessage response = _responseQueue.Dequeue();
				server.SendCFindResponse(presentationId, requestMessage.MessageId, response,
						 DicomStatuses.Pending);

				if (CancelReceived)
					throw new DicomException("DICOM C-Cancel Received");

				if (!server.NetworkActive)
					throw new DicomException("Association is no longer valid.");
			}
		}

        /// <summary>
        /// Method for processing Patient level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="presentationId"></param>
        /// <param name="message">The Patient level query message.</param>
        /// <returns></returns>
        private void OnReceivePatientQuery(DicomServer server, byte presentationId, DicomMessage message)
        {
            var tagList = new List<DicomTag>();

            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var find = read.GetBroker<IPatientEntityBroker>();

                var criteria = new PatientSelectCriteria();
                criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

                DicomAttributeCollection data = message.DataSet;
            	var studySelect = new StudySelectCriteria();
            	bool studySubSelect = false;
                foreach (DicomAttribute attrib in message.DataSet)
                {
                    tagList.Add(attrib.Tag);
                    if (!attrib.IsNull)
                        switch (attrib.Tag.TagValue)
                        {
                            case DicomTags.PatientsName:
								QueryHelper.SetStringCondition(criteria.PatientsName, data[DicomTags.PatientsName].GetString(0, string.Empty));
                                break;
                            case DicomTags.PatientId:
								QueryHelper.SetStringCondition(criteria.PatientId, data[DicomTags.PatientId].GetString(0, string.Empty));
                                break;
                            case DicomTags.IssuerOfPatientId:
                                QueryHelper.SetStringCondition(criteria.IssuerOfPatientId,
												   data[DicomTags.IssuerOfPatientId].GetString(0, string.Empty));
                                break;
							case DicomTags.PatientsSex:
								// Specify a subselect on Patients Sex in Study
								QueryHelper.SetStringArrayCondition(studySelect.PatientsSex,
														(string[])data[DicomTags.PatientsSex].Values);
								if (!studySubSelect)
								{
									criteria.StudyRelatedEntityCondition.Exists(studySelect);
									studySubSelect = true;
								}
                        		break;

							case DicomTags.PatientsBirthDate:
								// Specify a subselect on Patients Birth Date in Study
								QueryHelper.SetStringArrayCondition(studySelect.PatientsBirthDate,
														(string[])data[DicomTags.PatientsBirthDate].Values);
								if (!studySubSelect)
								{
									criteria.StudyRelatedEntityCondition.Exists(studySelect);
									studySubSelect = true;
								}
								break;
                            default:
                                foreach (var q in _queryExtensions)
                                {
                                    bool extensionSubSelect;
                                    q.OnReceivePatientLevelQuery(message, attrib.Tag, criteria, studySelect, out extensionSubSelect);
                                    if (extensionSubSelect && !studySubSelect)
                                    {
                                        criteria.StudyRelatedEntityCondition.Exists(studySelect);
                                        studySubSelect = true;
                                    }
                                }
                                break;
                        }
                }

				int resultCount = 0;
                try
                {
                    find.Find(criteria, delegate(Patient row)
                                            {
												if (CancelReceived)
													throw new DicomException("DICOM C-Cancel Received");

                                            	resultCount++;
												if (_maxQueryResponses != -1
													&& _maxQueryResponses < resultCount)
												{
													SendBufferedResponses(server, presentationId, message);
													throw new DicomException("Maximum Configured Query Responses Exceeded: " + resultCount);
												}

                                            	var response = new DicomMessage();
												PopulatePatient(response, tagList, row);
                                            	_responseQueue.Enqueue(response);

												if (_responseQueue.Count >= _bufferedQueryResponses)
													SendBufferedResponses(server, presentationId, message);
                                            });

                	SendBufferedResponses(server, presentationId, message);

                }
                catch (Exception e)
                {
					if (CancelReceived)
					{
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
												 DicomStatuses.Cancel);
					}
					else if (_maxQueryResponses != -1 
						  && _maxQueryResponses < resultCount)
					{
						Platform.Log(LogLevel.Warn, "Maximum Configured Query Responses Exceeded: {0} on query from {1}", resultCount, server.AssociationParams.CallingAE);
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
												 DicomStatuses.Success);
						AuditLog(server.AssociationParams,
								 EventIdentificationContentsEventOutcomeIndicator.Success, message);
					}
					else
					{
						Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
						                         DicomStatuses.QueryRetrieveUnableToProcess);
						AuditLog(server.AssociationParams,
								 EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, message);
					}
                	return;
                }
            }

            var finalResponse = new DicomMessage();
            server.SendCFindResponse(presentationId, message.MessageId, finalResponse, DicomStatuses.Success);
			AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
        }

        /// <summary>
        /// Method for processing Study level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="presentationId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private void OnReceiveStudyLevelQuery(DicomServer server, byte presentationId, DicomMessage message)
        {
            var tagList = new List<DicomTag>();

            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var find = read.GetBroker<IStudyEntityBroker>();

                var criteria = new StudySelectCriteria();
                criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

                DicomAttributeCollection data = message.DataSet;
                foreach (DicomAttribute attrib in message.DataSet)
                {
                    tagList.Add(attrib.Tag);
                    if (!attrib.IsNull)
                        switch (attrib.Tag.TagValue)
                        {
                            case DicomTags.StudyInstanceUid:
                                QueryHelper.SetStringArrayCondition(criteria.StudyInstanceUid,
                                                        (string[]) data[DicomTags.StudyInstanceUid].Values);
                                break;
                            case DicomTags.PatientsName:
								QueryHelper.SetStringCondition(criteria.PatientsName, data[DicomTags.PatientsName].GetString(0, string.Empty));
                                break;
                            case DicomTags.PatientId:
								QueryHelper.SetStringCondition(criteria.PatientId, data[DicomTags.PatientId].GetString(0, string.Empty));
                                break;
                            case DicomTags.PatientsBirthDate:
                                QueryHelper.SetRangeCondition(criteria.PatientsBirthDate,
												  data[DicomTags.PatientsBirthDate].GetString(0, string.Empty));
                                break;
                            case DicomTags.PatientsSex:
								QueryHelper.SetStringCondition(criteria.PatientsSex, data[DicomTags.PatientsSex].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyDate:
								QueryHelper.SetRangeCondition(criteria.StudyDate, data[DicomTags.StudyDate].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyTime:
								QueryHelper.SetRangeCondition(criteria.StudyTime, data[DicomTags.StudyTime].GetString(0, string.Empty));
                                break;
                            case DicomTags.AccessionNumber:
                                QueryHelper.SetStringCondition(criteria.AccessionNumber,
												   data[DicomTags.AccessionNumber].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyId:
								QueryHelper.SetStringCondition(criteria.StudyId, data[DicomTags.StudyId].GetString(0, string.Empty));
                                break;
                            case DicomTags.StudyDescription:
                                QueryHelper.SetStringCondition(criteria.StudyDescription,
												   data[DicomTags.StudyDescription].GetString(0, string.Empty));
                                break;
                            case DicomTags.ReferringPhysiciansName:
                                QueryHelper.SetStringCondition(criteria.ReferringPhysiciansName,
												   data[DicomTags.ReferringPhysiciansName].GetString(0, string.Empty));
                                break;
                            case DicomTags.ModalitiesInStudy:
                                // Specify a subselect on Modality in series
                                var seriesSelect = new SeriesSelectCriteria();
                                QueryHelper.SetStringArrayCondition(seriesSelect.Modality,
                                                        (string[]) data[DicomTags.ModalitiesInStudy].Values);
                                criteria.SeriesRelatedEntityCondition.Exists(seriesSelect);
                                break;
                            default:
                                foreach (var q in _queryExtensions)
                                    q.OnReceiveStudyLevelQuery(message, attrib.Tag, criteria);
                                break;
                        }
                }

				int resultCount = 0;
				try
                {
                    // Open another read context, in case additional queries are required.
					using (IReadContext subRead = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
						// First find the Online studies
                    	var storageCriteria = new StudyStorageSelectCriteria();
                    	storageCriteria.StudyStatusEnum.NotEqualTo(StudyStatusEnum.Nearline);
						storageCriteria.QueueStudyStateEnum.NotIn(new[] {QueueStudyStateEnum.DeleteScheduled, QueueStudyStateEnum.WebDeleteScheduled, QueueStudyStateEnum.EditScheduled});
                    	criteria.StudyStorageRelatedEntityCondition.Exists(storageCriteria);

                        find.Find(criteria, delegate(Study row)
                                                {
													if (CancelReceived)
														throw new DicomException("DICOM C-Cancel Received");

													resultCount++;
													if (_maxQueryResponses != -1 && _maxQueryResponses < resultCount)
													{
														SendBufferedResponses(server, presentationId, message);
														throw new DicomException("Maximum Configured Query Responses Exceeded: " + resultCount);
													}

                                                    var response = new DicomMessage();
                                                    PopulateStudy(subRead, response, tagList, row, "ONLINE");
													_responseQueue.Enqueue(response);

													if (_responseQueue.Count >= _bufferedQueryResponses)
														SendBufferedResponses(server, presentationId, message);
											
                                                });

						// Now find the Nearline studies
						storageCriteria = new StudyStorageSelectCriteria();
						storageCriteria.StudyStatusEnum.EqualTo(StudyStatusEnum.Nearline);
						storageCriteria.QueueStudyStateEnum.NotIn(new[] { QueueStudyStateEnum.DeleteScheduled, QueueStudyStateEnum.WebDeleteScheduled, QueueStudyStateEnum.EditScheduled });
						criteria.StudyStorageRelatedEntityCondition.Exists(storageCriteria);

						find.Find(criteria, delegate(Study row)
												{
													if (CancelReceived)
														throw new DicomException("DICOM C-Cancel Received");

													resultCount++;
													if (_maxQueryResponses != -1 && _maxQueryResponses < resultCount)
													{
														SendBufferedResponses(server, presentationId, message);
														throw new DicomException("Maximum Configured Query Responses Exceeded: " + resultCount);
													}

													var response = new DicomMessage();
													PopulateStudy(subRead, response, tagList, row, "NEARLINE");
													_responseQueue.Enqueue(response);

													if (_responseQueue.Count >= _bufferedQueryResponses)
														SendBufferedResponses(server, presentationId, message);

												});

						SendBufferedResponses(server, presentationId, message);
					}
                }
                catch (Exception e)
                {
					if (CancelReceived)
					{
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
												 DicomStatuses.Cancel);
						AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
        
					}
					else if (_maxQueryResponses != -1
						  && _maxQueryResponses < resultCount)
					{
						Platform.Log(LogLevel.Warn, "Maximum Configured Query Responses Exceeded: {0} on query from {1}", resultCount, server.AssociationParams.CallingAE);
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
												 DicomStatuses.Success);
						AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
        
					}
					else
					{
						Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
						                         DicomStatuses.ProcessingFailure);
						AuditLog(server.AssociationParams,
								 EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, message);

					}
                	return;
                }
            }

            var finalResponse = new DicomMessage();
            server.SendCFindResponse(presentationId, message.MessageId, finalResponse, DicomStatuses.Success);

			AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
        
        	return;
        }

        /// <summary>
        /// Method for processing Series level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="presentationId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private void OnReceiveSeriesLevelQuery(DicomServer server, byte presentationId, DicomMessage message)
        {
            //Read context for the query.
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var tagList = new List<DicomTag>();

                var selectSeries = read.GetBroker<ISeriesEntityBroker>();

                var criteria = new SeriesSelectCriteria();
                criteria.ServerPartitionKey.EqualTo(Partition.GetKey());

                DicomAttributeCollection data = message.DataSet;
                foreach (DicomAttribute attrib in message.DataSet)
                {
                    tagList.Add(attrib.Tag);
                    if (!attrib.IsNull)
                        switch (attrib.Tag.TagValue)
                        {
                            case DicomTags.StudyInstanceUid:
                                List<ServerEntityKey> list =
                                    LoadStudyKey(read, (string[]) data[DicomTags.StudyInstanceUid].Values);
                                if (list.Count == 0)
                                {
                                    server.SendCFindResponse(presentationId, message.MessageId, new DicomMessage(), 
                                                             DicomStatuses.Success);
                                    AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
                                    return;
                                }
                                QueryHelper.SetKeyCondition(criteria.StudyKey, list.ToArray());
                                break;
                            case DicomTags.SeriesInstanceUid:
                                QueryHelper.SetStringArrayCondition(criteria.SeriesInstanceUid,
                                                        (string[]) data[DicomTags.SeriesInstanceUid].Values);
                                break;
                            case DicomTags.Modality:
                                QueryHelper.SetStringCondition(criteria.Modality, data[DicomTags.Modality].GetString(0, string.Empty));
                                break;
                            case DicomTags.SeriesNumber:
								QueryHelper.SetStringCondition(criteria.SeriesNumber, data[DicomTags.SeriesNumber].GetString(0, string.Empty));
                                break;
                            case DicomTags.SeriesDescription:
                                QueryHelper.SetStringCondition(criteria.SeriesDescription,
												   data[DicomTags.SeriesDescription].GetString(0, string.Empty));
                                break;
                            case DicomTags.PerformedProcedureStepStartDate:
                                QueryHelper.SetRangeCondition(criteria.PerformedProcedureStepStartDate,
												  data[DicomTags.PerformedProcedureStepStartDate].GetString(0, string.Empty));
                                break;
                            case DicomTags.PerformedProcedureStepStartTime:
                                QueryHelper.SetRangeCondition(criteria.PerformedProcedureStepStartTime,
												  data[DicomTags.PerformedProcedureStepStartTime].GetString(0, string.Empty));
                                break;
                            case DicomTags.RequestAttributesSequence: // todo
                                break;
                            default:
                                foreach (var q in _queryExtensions)
                                    q.OnReceiveSeriesLevelQuery(message, attrib.Tag, criteria);
                                break;
                        }
                }

				int resultCount = 0;
				try
                {
                    // Open a second read context, in case other queries are required.
					using (IReadContext subRead = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        selectSeries.Find(criteria, delegate(Series row)
                                                        {
															if (CancelReceived)
																throw new DicomException("DICOM C-Cancel Received");

															resultCount++;
															if (_maxQueryResponses != -1 && _maxQueryResponses < resultCount)
															{
																SendBufferedResponses(server, presentationId, message);
																throw new DicomException("Maximum Configured Query Responses Exceeded: " + resultCount);
															}

                                                        	var response = new DicomMessage();
                                                            PopulateSeries(subRead, message, response, tagList, row);
															_responseQueue.Enqueue(response);

															if (_responseQueue.Count >= _bufferedQueryResponses)
																SendBufferedResponses(server, presentationId, message);
														});
						SendBufferedResponses(server, presentationId, message);
					}
                }
                catch (Exception e)
                {
					if (CancelReceived)
					{
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
												 DicomStatuses.Cancel);
						AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
        
					}
					else if (_maxQueryResponses != -1 && _maxQueryResponses < resultCount)
					{
						Platform.Log(LogLevel.Warn, "Maximum Configured Query Responses Exceeded: {0} on query from {1}",resultCount,server.AssociationParams.CallingAE);

						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
												 DicomStatuses.Success);
						AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
        
					}
					else
					{
						Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationId, message.MessageId, errorResponse,
						                         DicomStatuses.ProcessingFailure);
						AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, message);
        
					}
                	return;
                }

                var finalResponse = new DicomMessage();
                server.SendCFindResponse(presentationId, message.MessageId, finalResponse, DicomStatuses.Success);

				AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
        
            	return;
            }
        }

        /// <summary>
        /// Compare at the IMAGE level if a query matches the data in an <see cref="InstanceXml"/> file.
        /// </summary>
        /// <param name="queryMessage"></param>
        /// <param name="matchTagList"></param>
        /// <param name="instanceStream"></param>
        /// <returns></returns>
        private static bool CompareInstanceMatch(DicomMessageBase queryMessage, IEnumerable<uint> matchTagList,
                                                 InstanceXml instanceStream)
        {
            foreach (uint tag in matchTagList)
            {
                if (!instanceStream.Collection.Contains(tag))
                    continue;

                DicomAttribute sourceAttrib = queryMessage.DataSet[tag];
                DicomAttribute matchAttrib = instanceStream.Collection[tag];

                if (sourceAttrib.Tag.VR.Equals(DicomVr.SQvr))
                    continue; // TODO

                if (sourceAttrib.IsNull)
                    continue;

                string sourceString = sourceAttrib.ToString();
                if (sourceString.Contains("*") || sourceString.Contains("?"))
                {
                    sourceString = sourceString.Replace("*", "[\x21-\x7E]");
                    sourceString = sourceString.Replace("?", ".");
                    if (!Regex.IsMatch(matchAttrib.ToString(), sourceString))
                        return false;
                }
                else if (!sourceAttrib.Equals(matchAttrib))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Method for processing Image level queries.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="presentationId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private void OnReceiveImageLevelQuery(DicomServer server, byte presentationId, DicomMessage message)
        {
            var tagList = new List<DicomTag>();
            var matchingTagList = new List<uint>();

            DicomAttributeCollection data = message.DataSet;
            string studyInstanceUid = data[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            string seriesInstanceUid = data[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);

            StudyStorageLocation location;
        	try
        	{
        		FilesystemMonitor.Instance.GetReadableStudyStorageLocation(Partition.Key, studyInstanceUid, StudyRestore.True,
        		                                                           StudyCache.True, out location);
        	}
        	catch (Exception e)
        	{
        	    Platform.Log(LogLevel.Error, "Unable to load storage location for study {0}: {1}", studyInstanceUid, e.Message);
                var failureResponse = new DicomMessage();
                failureResponse.DataSet[DicomTags.InstanceAvailability].SetStringValue("NEARLINE");
                failureResponse.DataSet[DicomTags.QueryRetrieveLevel].SetStringValue("IMAGE");
                failureResponse.DataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
                failureResponse.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				failureResponse.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				server.SendCFindResponse(presentationId, message.MessageId, failureResponse,
                                         DicomStatuses.QueryRetrieveUnableToProcess);

				AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, message);

            	return;
            }
            try
            {
                // Will always return a value, although it may be an empty StudyXml file
                StudyXml studyXml = LoadStudyXml(location);

                SeriesXml seriesXml = studyXml[seriesInstanceUid];
                if (seriesXml == null)
                {
                    var failureResponse = new DicomMessage();
                    failureResponse.DataSet[DicomTags.QueryRetrieveLevel].SetStringValue("IMAGE");
                    server.SendCFindResponse(presentationId, message.MessageId, failureResponse,
                                             DicomStatuses.QueryRetrieveUnableToProcess);
                    AuditLog(server.AssociationParams,
                             EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, message);
                    return;
                }

                foreach (DicomAttribute attrib in message.DataSet)
                {
                    if (attrib.Tag.TagValue.Equals(DicomTags.SpecificCharacterSet)
                        || attrib.Tag.TagValue.Equals(DicomTags.QueryRetrieveLevel))
                        continue;

                    tagList.Add(attrib.Tag);
                    if (!attrib.IsNull)
                        matchingTagList.Add(attrib.Tag.TagValue);
                }

                int resultCount = 0;

                foreach (InstanceXml theInstanceStream in seriesXml)
                {
                    if (CompareInstanceMatch(message, matchingTagList, theInstanceStream))
                    {
                        if (CancelReceived)
                        {
                            var failureResponse = new DicomMessage();
                            failureResponse.DataSet[DicomTags.QueryRetrieveLevel].SetStringValue("IMAGE");
                            server.SendCFindResponse(presentationId, message.MessageId, failureResponse,
                                                     DicomStatuses.Cancel);

                            AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success,
                                     message);
                            return;
                        }
                        resultCount++;
                        if (_maxQueryResponses != -1 && _maxQueryResponses < resultCount)
                        {
                            SendBufferedResponses(server, presentationId, message);
                            Platform.Log(LogLevel.Warn, "Maximum Configured Query Responses Exceeded: " + resultCount);
                            break;
                        }

                        var response = new DicomMessage();
                        PopulateInstance(message, response, tagList, theInstanceStream);
                        _responseQueue.Enqueue(response);

                        if (_responseQueue.Count >= _bufferedQueryResponses)
                            SendBufferedResponses(server, presentationId, message);
                    }
                }

                SendBufferedResponses(server, presentationId, message);

                var finalResponse = new DicomMessage();
                server.SendCFindResponse(presentationId, message.MessageId, finalResponse, DicomStatuses.Success);

                AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.Success, message);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexepected exception processing IMAGE level query for study {0}: {1}", studyInstanceUid, e.Message);
                var failureResponse = new DicomMessage();
                failureResponse.DataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");
                failureResponse.DataSet[DicomTags.QueryRetrieveLevel].SetStringValue("IMAGE");
                failureResponse.DataSet[DicomTags.RetrieveAeTitle].SetStringValue(Partition.AeTitle);
                failureResponse.DataSet[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
                failureResponse.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
                server.SendCFindResponse(presentationId, message.MessageId, failureResponse,
                                         DicomStatuses.QueryRetrieveUnableToProcess);

                AuditLog(server.AssociationParams, EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated, message);
            }
            return;
        }

        /// <summary>
        /// Get the prefered character set for the C-FIND-RSP.
        /// </summary>
        /// <returns></returns>
        private string GetPreferredCharacterSet()
        {
            // TODO: In the future, should be device-dependent
			if (_cfindRspAlwaysInUnicode)
                return "ISO_IR 192";
            return null;
        }

        #endregion

        #region IDicomScp Members

	    /// <summary>
	    /// Extension method called when a new DICOM Request message has been called that the 
	    /// extension will process.
	    /// </summary>
	    /// <param name="server"></param>
	    /// <param name="association"></param>
	    /// <param name="presentationId"></param>
	    /// <param name="message"></param>
	    /// <returns></returns>
	    public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association,
	                                          byte presentationId, DicomMessage message)
	    {
		    String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, string.Empty);

		    if (message.CommandField == DicomCommandField.CCancelRequest)
		    {
			    Platform.Log(LogLevel.Info, "Received C-FIND-CANCEL-RQ message.");
			    CancelReceived = true;
			    return true;
		    }

		    CancelReceived = false;

		    if (message.AffectedSopClassUid.Equals(SopClass.StudyRootQueryRetrieveInformationModelFindUid))
		    {
			    if (level.Equals("STUDY"))
			    {
				    // We use the ThreadPool to process the thread requests. This is so that we return back
				    // to the main message loop, and continue to look for cancel request messages coming
				    // in.  There's a small chance this may cause delays in responding to query requests if
				    // the .NET Thread pool fills up.
				    OnReceiveStudyLevelQuery(server, presentationId, message);
				    return true;
			    }
			    if (level.Equals("SERIES"))
			    {
				    OnReceiveSeriesLevelQuery(server, presentationId, message);
				    return true;
			    }
			    if (level.Equals("IMAGE"))
			    {
				    OnReceiveImageLevelQuery(server, presentationId, message);
				    return true;
			    }
			    Platform.Log(LogLevel.Error, "Unexpected Study Root Query/Retrieve level: {0}", level);

			    server.SendCFindResponse(presentationId, message.MessageId, new DicomMessage(),
			                             DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
			    return true;
		    }
		    if (message.AffectedSopClassUid.Equals(SopClass.PatientRootQueryRetrieveInformationModelFindUid))
		    {
			    if (level.Equals("PATIENT"))
			    {
				    OnReceivePatientQuery(server, presentationId, message);
				    return true;
			    }
			    if (level.Equals("STUDY"))
			    {
				    OnReceiveStudyLevelQuery(server, presentationId, message);
				    return true;
			    }
			    if (level.Equals("SERIES"))
			    {
				    OnReceiveSeriesLevelQuery(server, presentationId, message);
				    return true;
			    }
			    if (level.Equals("IMAGE"))
			    {
				    OnReceiveImageLevelQuery(server, presentationId, message);
				    return true;
			    }
			    Platform.Log(LogLevel.Error, "Unexpected Patient Root Query/Retrieve level: {0}", level);

			    server.SendCFindResponse(presentationId, message.MessageId, new DicomMessage(),
			                             DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
			    return true;
		    }

		    // Not supported message type, send a failure status.
		    server.SendCFindResponse(presentationId, message.MessageId, new DicomMessage(),
		                             DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
		    return true;
	    }

	    /// <summary>
        /// Extension method called to get a list of the SOP Classes and transfer syntaxes supported by the extension.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (!Context.AllowQuery)
                return new List<SupportedSop>();

            return _list;
        }

        #region Overridden BaseSCP methods

        protected override DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
        {
            if (!Device.AllowQuery)
            {
                return DicomPresContextResult.RejectUser;
            }

            return DicomPresContextResult.Accept;
        }

        #endregion Overridden BaseSCP methods

        #endregion
    }
}