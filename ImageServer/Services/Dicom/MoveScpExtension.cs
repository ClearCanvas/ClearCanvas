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
using System.Globalization;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    ///<summary>
    /// Plugin for handling DICOM Retrieve Requests implementing the <see cref="IDicomScp{TContext}"/> interface.
    ///</summary>
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class MoveScpExtension : BaseScp
    {
        #region Private members
        private readonly List<SupportedSop> _list = new List<SupportedSop>();
        private ImageServerStorageScu _theScu;
        #endregion

        #region Contructors
        /// <summary>
        /// Public default constructor.  Implements the Find and Move services for 
        /// Patient Root and Study Root queries.
        /// </summary>
        public MoveScpExtension()
        {
            var sop = new SupportedSop
                          {
                              SopClass = SopClass.PatientRootQueryRetrieveInformationModelMove
                          };
        	sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);

            sop = new SupportedSop
                  	{
                  		SopClass = SopClass.StudyRootQueryRetrieveInformationModelMove
                  	};
        	sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Create a list of SOP Instances to move based on a Patient level C-MOVE-RQ.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="msg"></param>
        /// <param name="errorComment"> </param>
        /// <returns></returns>
        private bool GetSopListForPatient(IPersistenceContext read, DicomMessageBase msg, out string errorComment)
        {
            errorComment = string.Empty;
            string patientId = msg.DataSet[DicomTags.PatientId].GetString(0, "");

            var select = read.GetBroker<IStudyEntityBroker>();

            var criteria = new StudySelectCriteria();
            criteria.PatientId.EqualTo(patientId);
			criteria.ServerPartitionKey.EqualTo(Partition.Key);

            IList<Study> studyList = select.Find(criteria);

        	bool bOfflineFound = false;
            foreach (Study study in studyList)
            {
                StudyStorageLocation location;

				try
				{
					FilesystemMonitor.Instance.GetReadableStudyStorageLocation(Partition.Key, study.StudyInstanceUid,
					                                                           StudyRestore.True, StudyCache.True, out location);
				}
                catch (StudyIsNearlineException e)
                {
                    errorComment = string.Format(e.RestoreRequested ? "Study is nearline, inserted restore request: {0}" : "Study is nearline: {0}", study.StudyInstanceUid);

                    bOfflineFound = true;
                    continue;				        
                }
				catch (Exception e)
				{
				    errorComment = string.Format("Exception occurred when determining study location: {0}", e.Message);
					bOfflineFound = true;
					continue;
				}

            	StudyXml theStream = LoadStudyXml(location);

                _theScu.LoadStudyFromStudyXml(location.GetStudyPath(), theStream);
            }

            return !bOfflineFound;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on a Study level C-MOVE-RQ.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="errorComment"></param>
        /// <returns></returns>
        private bool GetSopListForStudy(DicomMessageBase msg, out string errorComment)
        {
            errorComment = string.Empty;
        	bool bOfflineFound = false;

            var studyList = (string[]) msg.DataSet[DicomTags.StudyInstanceUid].Values;

            // Now get the storage location
            foreach (string studyInstanceUid in studyList)
            {
                StudyStorageLocation location;

            	try
            	{
            		FilesystemMonitor.Instance.GetReadableStudyStorageLocation(Partition.Key, studyInstanceUid,
            		                                                           StudyRestore.True, StudyCache.True,
            		                                                           out location);
            	}
            	catch (StudyIsNearlineException e)
            	{
                    errorComment = string.Format(e.RestoreRequested ? "Study is nearline, inserted restore request: {0}" : "Study is nearline: {0}", studyInstanceUid);

            	    bOfflineFound = true;
					continue;
            	}
				catch (Exception e)
				{
                    errorComment = string.Format("Exception occurred when determining study location: {0}", e.Message);
					bOfflineFound = true;
					continue;
				}

				StudyXml theStream = LoadStudyXml(location);

                _theScu.LoadStudyFromStudyXml(location.GetStudyPath(), theStream);
            }

			if (bOfflineFound) return false;

            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on a Series level C-MOVE-RQ
        /// </summary>
        /// <param name="persistenceContext"></param>
        /// <param name="msg"></param>
        /// <param name="errorComment"> </param>
        /// <returns></returns>
        private bool GetSopListForSeries(IPersistenceContext persistenceContext, DicomMessageBase msg, out string errorComment)
        {
            errorComment = string.Empty;
            string studyInstanceUid = msg.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            var seriesList = (string[])msg.DataSet[DicomTags.SeriesInstanceUid].Values;

            // Now get the storage location
            StudyStorageLocation location;

			try
			{
				FilesystemMonitor.Instance.GetReadableStudyStorageLocation(Partition.Key, studyInstanceUid, StudyRestore.True,
				                                                           StudyCache.True, out location);
			}
			catch(StudyIsNearlineException e)
			{
                errorComment = string.Format(e.RestoreRequested ? "Study is nearline, inserted restore request: {0}" : "Study is nearline: {0}", studyInstanceUid);
				return false;
			}
			catch (Exception e)
			{
                errorComment = string.Format("Exception occurred when determining study location: {0}", e.Message);
                return false;
			}

			var select = persistenceContext.GetBroker<IStudyEntityBroker>();

			var criteria = new StudySelectCriteria();
			criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
			criteria.ServerPartitionKey.EqualTo(Partition.Key);

			Study study = select.FindOne(criteria);
        	StudyXml studyStream = LoadStudyXml(location);

            foreach (string seriesInstanceUid in seriesList)
            {
				_theScu.LoadSeriesFromSeriesXml(studyStream, Path.Combine(location.GetStudyPath(), seriesInstanceUid), studyStream[seriesInstanceUid], study.PatientsName, study.PatientId);
            }

            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on an Image level C-MOVE-RQ.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="errorComment"> </param>
        /// <returns></returns>
        private bool GetSopListForSop(DicomMessageBase msg, out string errorComment)
        {
            errorComment = string.Empty;
            string studyInstanceUid = msg.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            string seriesInstanceUid = msg.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
            var sopInstanceUidArray = (string[])msg.DataSet[DicomTags.SopInstanceUid].Values;

            // Now get the storage location
            StudyStorageLocation location;

			try
			{
				FilesystemMonitor.Instance.GetReadableStudyStorageLocation(Partition.Key, studyInstanceUid, StudyRestore.True,
																		   StudyCache.True, out location);
			}
			catch (StudyIsNearlineException e)
            {
                errorComment = string.Format(e.RestoreRequested ? "Study is nearline, inserted restore request: {0}" : "Study is nearline: {0}", studyInstanceUid);
                return false;
			}
			catch (Exception e)
			{
                errorComment = string.Format("Exception occurred when determining study location: {0}", e.Message);
                return false;
			}

        	// There can be multiple SOP Instance UIDs in the move request
            foreach (string sopInstanceUid in sopInstanceUidArray)
            {
                string path = Path.Combine(location.GetStudyPath(), seriesInstanceUid);
                path = Path.Combine(path, sopInstanceUid + ServerPlatform.DicomFileExtension);
                _theScu.AddStorageInstance(new StorageInstance(path));
            }

            return true;
        }

        /// <summary>
        /// Load <see cref="Device"/> information for a Move destination.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="partition"></param>
        /// <param name="remoteAe"></param>
        /// <returns></returns>
        private static Device LoadRemoteHost(IPersistenceContext read, ServerPartition partition, string remoteAe)
        {
            var select = read.GetBroker<IDeviceEntityBroker>();

            // Setup the select parameters.
            var selectParms = new DeviceSelectCriteria();
            selectParms.AeTitle.EqualTo(remoteAe);
            selectParms.ServerPartitionKey.EqualTo(partition.GetKey());
            var devices = select.Find(selectParms);
            foreach (var d in devices)
            {
                if (string.Compare(d.AeTitle, remoteAe, false, CultureInfo.InvariantCulture) == 0)
                {
                    return d;
                }
            }
            return null;
        }
        #endregion

        #region IDicomScp Members
        /// <summary>
        /// Main routine for processing C-MOVE-RQ messages.  Called by the <see cref="DicomScp{DicomScpParameters}"/> component.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationId, DicomMessage message)
        {
            bool finalResponseSent = false;
            string errorComment;

            try
            {
                // Check for a Cancel message, and cancel the SCU.
                if (message.CommandField == DicomCommandField.CCancelRequest)
                {
                    if (_theScu != null)
                    {
                        _theScu.Cancel();
                    }
                    return true;
                }

                // Get the level of the Move.
                String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, "");

                // Trim the remote AE, see extra spaces at the end before which has caused problems
                string remoteAe = message.MoveDestination.Trim();

                // Open a DB Connection
                using (IReadContext read = _store.OpenReadContext())
                {
                    // Load remote device information fromt he database.
                    Device device = LoadRemoteHost(read, Partition, remoteAe);
                    if (device == null)
                    {
                        errorComment = string.Format(
                                     "Unknown move destination \"{0}\", failing C-MOVE-RQ from {1} to {2}",
                                     remoteAe, association.CallingAE, association.CalledAE);
                        Platform.Log(LogLevel.Error, errorComment);
                        server.SendCMoveResponse(presentationId, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.QueryRetrieveMoveDestinationUnknown, errorComment);
                        finalResponseSent = true;
                        return true;
                    }

                    // If the remote node is a DHCP node, use its IP address from the connection information, else
                    // use what is configured.  Always use the configured port.
                    if (device.Dhcp)
                        device.IpAddress = association.RemoteEndPoint.Address.ToString();

                    // Now setup the StorageSCU component
                    _theScu = new ImageServerStorageScu(Partition, device,
                                             association.CallingAE, message.MessageId);


                    // Now create the list of SOPs to send
                    bool bOnline;
                   
                    if (level.Equals("PATIENT"))
                    {
                        bOnline = GetSopListForPatient(read, message, out errorComment);
                    }
                    else if (level.Equals("STUDY"))
                    {
                        bOnline = GetSopListForStudy(message, out errorComment);
                    }
                    else if (level.Equals("SERIES"))
                    {
                        bOnline = GetSopListForSeries(read, message, out errorComment);
                    }
                    else if (level.Equals("IMAGE"))
                    {
                        bOnline = GetSopListForSop(message, out errorComment);
                    }
                    else
                    {
                        errorComment = string.Format("Unexpected Study Root Move Query/Retrieve level: {0}", level);
                        Platform.Log(LogLevel.Error, errorComment);

                        server.SendCMoveResponse(presentationId, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass,
                                                 errorComment);
                        finalResponseSent = true;
                        return true;
                    }

                    // Could not find an online/readable location for the requested objects to move.
					// Note that if the C-MOVE-RQ included a list of study instance uids, and some 
					// were online and some offline, we don't fail now (ie, the check on the Count)
					if (!bOnline && _theScu.StorageInstanceList.Count == 0)
                    {
                        Platform.Log(LogLevel.Error, "Unable to find online storage location for C-MOVE-RQ: {0}", errorComment);

                        server.SendCMoveResponse(presentationId, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.QueryRetrieveUnableToPerformSuboperations,
                                                 string.IsNullOrEmpty(errorComment) ? string.Empty : errorComment);
                        finalResponseSent = true;
                    	_theScu.Dispose();
                        _theScu = null;
                        return true;
                    }

                    // No files were eligible for transfer, just send success and return
                    if (_theScu.StorageInstanceList.Count == 0)
                    {
                        server.SendCMoveResponse(presentationId, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.Success,
                                                 0, 0, 0, 0);
                        finalResponseSent = true;
						_theScu.Dispose();
						_theScu = null;
                        return true;
                    }

                    // set the preferred syntax lists
                    _theScu.LoadPreferredSyntaxes(read);

                	_theScu.ImageStoreCompleted += delegate(Object sender, StorageInstance instance)
                	                               	{
                	                               		var scu = (StorageScu) sender;
                	                               		var msg = new DicomMessage();
                	                               		DicomStatus status;

                                                        if (instance.SendStatus.Status == DicomState.Failure)
                                                        {
                                                            errorComment =
                                                                string.IsNullOrEmpty(instance.ExtendedFailureDescription)
                                                                    ? instance.SendStatus.ToString()
                                                                    : instance.ExtendedFailureDescription;
                                                        }

                	                               		if (scu.RemainingSubOperations == 0)
                	                               		{
                	                               			foreach (StorageInstance sop in _theScu.StorageInstanceList)
                	                               			{
                	                               				if ((sop.SendStatus.Status != DicomState.Success)
                	                               				    && (sop.SendStatus.Status != DicomState.Warning))
                	                               					msg.DataSet[DicomTags.FailedSopInstanceUidList].AppendString(sop.SopInstanceUid);
                	                               			}
                	                               			if (scu.Status == ScuOperationStatus.Canceled)
                	                               				status = DicomStatuses.Cancel;
                	                               			else if (scu.Status == ScuOperationStatus.ConnectFailed)
                	                               				status = DicomStatuses.QueryRetrieveMoveDestinationUnknown;
                	                               			else if (scu.FailureSubOperations > 0)
                	                               				status = DicomStatuses.QueryRetrieveSubOpsOneOrMoreFailures;
															else if (!bOnline)
																status = DicomStatuses.QueryRetrieveUnableToPerformSuboperations;
                	                               			else
                	                               				status = DicomStatuses.Success;
                	                               		}
                	                               		else
                	                               		{
                	                               			status = DicomStatuses.Pending;

                	                               			if ((scu.RemainingSubOperations%5) != 0)
                	                               				return;
                	                               			// Only send a RSP every 5 to reduce network load
                	                               		}
                	                               	    server.SendCMoveResponse(presentationId, message.MessageId,
                	                               	                             msg, status,
                	                               	                             (ushort) scu.SuccessSubOperations,
                	                               	                             (ushort) scu.RemainingSubOperations,
                	                               	                             (ushort) scu.FailureSubOperations,
                	                               	                             (ushort) scu.WarningSubOperations,
                                                                                 status == DicomStatuses.QueryRetrieveSubOpsOneOrMoreFailures
                	                               	                                 ? errorComment
                	                               	                                 : string.Empty);
                	                               	    
                	                               	    
                	                               		if (scu.RemainingSubOperations == 0)
                	                               			finalResponseSent = true;
                	                               	};

                    _theScu.AssociationAccepted +=
                        (sender, parms) => AssociationAuditLogger.BeginInstancesTransferAuditLogger(
                            _theScu.StorageInstanceList,
                            parms);
					
                    _theScu.BeginSend(
                        delegate(IAsyncResult ar)
                        	{
								if (_theScu != null)
								{
                                    if (!finalResponseSent)
                                    {
                                        var msg = new DicomMessage();
                                        server.SendCMoveResponse(presentationId, message.MessageId,
                                                                 msg, DicomStatuses.QueryRetrieveSubOpsOneOrMoreFailures,
                                                                 (ushort) _theScu.SuccessSubOperations,
                                                                 0,
                                                                 (ushort) (_theScu.FailureSubOperations + _theScu.RemainingSubOperations),
                                                                 (ushort) _theScu.WarningSubOperations, errorComment);
                                        finalResponseSent = true;
                                    }

									_theScu.EndSend(ar);
									_theScu.Dispose();
									_theScu = null;
								}
                        	},
                        _theScu);

                    return true;
                } // end using()
            } 
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error,e,"Unexpected exception when processing C-MOVE-RQ");
                if (finalResponseSent == false)
                {
                    try
                    {
                        server.SendCMoveResponse(presentationId, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.QueryRetrieveUnableToProcess, e.Message);
                        finalResponseSent = true;
                    }
                    catch (Exception x)
                    {
                        Platform.Log(LogLevel.Error, x,
                                     "Unable to send final C-MOVE-RSP message on association from {0} to {1}",
                                     association.CallingAE, association.CalledAE);
                        server.Abort();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Return a list of SOP Classes and Transfer Syntaxes supported by this extension.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (!Context.AllowRetrieve)
                return new List<SupportedSop>();

            return _list;
        }

        #endregion

        #region Overridden BaseSCP methods

        protected override DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
        {            
            
            if (!Device.AllowRetrieve)
            {
                return DicomPresContextResult.RejectUser; 
            }

            return DicomPresContextResult.Accept;
            
        }

        #endregion Overridden BaseSCP methods
    }
}