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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{    
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class MoveScpExtension : ScpExtension
	{
		#region SendOperationInfo class

		private class SendOperationInfo
		{
			public SendOperationInfo(WorkItemData reference, ushort messageId, byte presentationId, ClearCanvas.Dicom.Network.DicomServer server)
			{
				PresentationId = presentationId;
				Server = server;
				WorkItemData = reference;
				MessageId = messageId;
			    SubOperations = 0;
			    Complete = false;
			    FailedSopInstanceUids = new List<string>();
			}

            public WorkItemData WorkItemData;

			public readonly ushort MessageId;
			public readonly byte PresentationId;
			public readonly ClearCanvas.Dicom.Network.DicomServer Server;
		    public int SubOperations;
		    public readonly List<string> FailedSopInstanceUids;
		    public bool Complete;
		}

		#endregion

		#region Private Fields

		private readonly object _syncLock = new object();
		private readonly List<SendOperationInfo> _sendOperations;
	    private readonly IWorkItemActivityMonitor _activityMonitor;

		#endregion

        #region Constructors

        public MoveScpExtension()
			: base(GetSupportedSops())
		{
			_sendOperations = new List<SendOperationInfo>();
            // TODO (CR Jun 2012 - Med): This item is disposable and should ideally be cleaned up.
            _activityMonitor = WorkItemActivityMonitor.Create(false);
		    _activityMonitor.WorkItemsChanged += UpdateProgress;
		}

        #endregion

        #region Private Methods

        private static IEnumerable<SupportedSop> GetSupportedSops()
		{
		    var sop = new SupportedSop
		                  {
		                      SopClass = SopClass.StudyRootQueryRetrieveInformationModelMove
		                  };
		    sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
			sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
			yield return sop;
		}

		private SendOperationInfo GetSendOperationInfo(WorkItemData reference)
		{
			lock (_syncLock)
			{
				return CollectionUtils.SelectFirst(_sendOperations,
				                                   info => info.WorkItemData.Identifier == reference.Identifier);
			}
		}

		private List<SendOperationInfo> GetSendOperationInfo(int dicomMessageId)
		{
			lock (_syncLock)
			{
				return CollectionUtils.Select(_sendOperations,
				                                   info => info.MessageId == dicomMessageId);
			}
		}

        private DicomSendProgress GetProgressByMessageId(int dicomMessageId)
        {
            var list = GetSendOperationInfo(dicomMessageId);
            if (list.Count == 1)
            {
                SendOperationInfo sendOperation = CollectionUtils.FirstElement(list);
                var progress = sendOperation.WorkItemData.Progress as DicomSendProgress ?? new DicomSendProgress
                                                                                               {
                                                                                                   TotalImagesToSend = sendOperation.SubOperations
                                                                                               };
                if (progress.TotalImagesToSend == 0)
                    progress.TotalImagesToSend = sendOperation.SubOperations;

                return progress;
            }

            // TODO (CR Jun 2012): this actually happen?
            // (SW) - It won't actually happen with our client, but it could happen by other DICOM clients that have a list of UIDs to move.
            var aggregateProgress = new DicomSendProgress();
            foreach (var sendOperation in list)
            {
                var currentProgress = sendOperation.WorkItemData.Progress as DicomSendProgress;
                if (currentProgress == null)
                {
                    aggregateProgress.TotalImagesToSend += sendOperation.SubOperations;
                }
                else
                {
                    aggregateProgress.TotalImagesToSend += currentProgress.TotalImagesToSend;
                    aggregateProgress.FailureSubOperations += currentProgress.FailureSubOperations;
                    aggregateProgress.SuccessSubOperations += currentProgress.SuccessSubOperations;
                    aggregateProgress.WarningSubOperations += currentProgress.WarningSubOperations;
                }
            }
            return aggregateProgress;
        }

        private bool SendOperationsComplete(int dicomMessageId)
        {
            var list = GetSendOperationInfo(dicomMessageId);
            foreach (var sendOperation in list)
            {
                if (!sendOperation.Complete)
                    return false;
            }
            return true;
        }

	    private void RemoveSendOperationInfo(SendOperationInfo info)
		{
			lock(_syncLock)
			{
				_sendOperations.Remove(info);
			}
		}

        private void UpdateProgress(object sender, WorkItemsChangedEventArgs workItemsChangedEventArgs)
        {
        	foreach (var item in workItemsChangedEventArgs.ChangedItems)
        	{
        		UpdateProgress(item);
        	}	
        }

        private void UpdateProgress(WorkItemData workItem)
        {
            try
            {
				SendOperationInfo sendOperationInfo = GetSendOperationInfo(workItem);
                if (sendOperationInfo == null)
                {
                    return;
                }

				sendOperationInfo.WorkItemData = workItem;

                var progress = GetProgressByMessageId(sendOperationInfo.MessageId);

                var msg = new DicomMessage();
                DicomStatus status;

				if (workItem.Status == WorkItemStatusEnum.Failed)
                {
                    sendOperationInfo.Complete = true;
                }
				else if (progress.RemainingSubOperations == 0 && workItem.Status != WorkItemStatusEnum.Pending)
                {
                    sendOperationInfo.Complete = true;
                }

                if (SendOperationsComplete(sendOperationInfo.MessageId))
                {
                    status = DicomStatuses.Success;

                    foreach (SendOperationInfo info in GetSendOperationInfo(sendOperationInfo.MessageId))
                    {
                        foreach (string sopInstanceUid in info.FailedSopInstanceUids)
                            msg.DataSet[DicomTags.FailedSopInstanceUidList].AppendString(sopInstanceUid);
						if (workItem.Status == WorkItemStatusEnum.Canceled)
                            status = DicomStatuses.Cancel;
						else if (workItem.Status == WorkItemStatusEnum.Failed)
                            status = DicomStatuses.QueryRetrieveUnableToProcess;
                        else if (progress.FailureSubOperations > 0 && status == DicomStatuses.Success)
                            status = DicomStatuses.QueryRetrieveSubOpsOneOrMoreFailures;
                    }
                }
                else
                {
                    status = DicomStatuses.Pending;
                    if ((progress.RemainingSubOperations%5) != 0)
                        return;

                    // Only send a RSP every 5 to reduce network load
                }

                if (sendOperationInfo.Server.NetworkActive)
                {
                    sendOperationInfo.Server.SendCMoveResponse(sendOperationInfo.PresentationId,
                                                               sendOperationInfo.MessageId,
                                                               msg, status,
                                                               (ushort) progress.SuccessSubOperations,
                                                               (ushort) progress.RemainingSubOperations,
                                                               (ushort) progress.FailureSubOperations,
                                                               (ushort) progress.WarningSubOperations);
                }

                if (status != DicomStatuses.Pending || !sendOperationInfo.Server.NetworkActive)
                {
                    foreach (SendOperationInfo info in GetSendOperationInfo(sendOperationInfo.MessageId))
                    {
                        RemoveSendOperationInfo(info);
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected error processing C-MOVE Responses");
            }
        }

	    private void OnReceiveMoveStudiesRequest(ClearCanvas.Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, IDicomServiceNode remoteAEInfo)
		{
			IEnumerable<string> studyUids = (string[])message.DataSet[DicomTags.StudyInstanceUid].Values;

            foreach (string studyUid in studyUids)
            {
                lock (_syncLock)
                {
                    int subOperations = 0;
                    using (var context = new DataAccessContext())
                    {
                        var s =
                            context.GetStudyStoreQuery().StudyQuery(new StudyRootStudyIdentifier
                                                                        {StudyInstanceUid = studyUid});
                        var identifier = CollectionUtils.FirstElement(s);
                        if (identifier.NumberOfStudyRelatedInstances.HasValue)
                            subOperations = identifier.NumberOfStudyRelatedInstances.Value;

                        var client = new DicomSendBridge();
                        client.SendStudy(remoteAEInfo, identifier, WorkItemPriorityEnum.High);
                        _sendOperations.Add(new SendOperationInfo(client.WorkItem, message.MessageId,
                                                                  presentationID,
                                                                  server)
                                                {
                                                    SubOperations = subOperations
                                                });
                    }
                }
            }
		}

        private void OnReceiveMoveSeriesRequest(ClearCanvas.Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, IDicomServiceNode remoteAEInfo)
		{
			string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
			var seriesUids = (string[])message.DataSet[DicomTags.SeriesInstanceUid].Values;

            lock (_syncLock)
            {

                int subOperations = 0;
                using (var context = new DataAccessContext())
                {
                    var results = context.GetStudyStoreQuery().SeriesQuery(new SeriesIdentifier
                                                                               {
                                                                                   StudyInstanceUid =
                                                                                       studyInstanceUid,
                                                                               });
                    foreach (SeriesIdentifier series in results)
                    {
                        foreach (string seriesUid in seriesUids)
                            if (series.SeriesInstanceUid.Equals(seriesUid) &&
                                series.NumberOfSeriesRelatedInstances.HasValue)
                            {
                                subOperations += series.NumberOfSeriesRelatedInstances.Value;
                                break;
                            }
                    }

                    var s =
                        context.GetStudyStoreQuery().StudyQuery(new StudyRootStudyIdentifier
                                                                    {StudyInstanceUid = studyInstanceUid});
                    var identifier = CollectionUtils.FirstElement(s);
                    var client = new DicomSendBridge();

                    client.SendSeries(remoteAEInfo, identifier, seriesUids, WorkItemPriorityEnum.High);
                    _sendOperations.Add(new SendOperationInfo(client.WorkItem, message.MessageId, presentationID,
                                                              server)
                                            {
                                                SubOperations = subOperations
                                            });
                }
            }
		}

        private void OnReceiveMoveImageRequest(ClearCanvas.Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, IDicomServiceNode remoteAEInfo)
		{
			string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
			string seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
			var sopInstanceUids = (string[])message.DataSet[DicomTags.SopInstanceUid].Values;

            lock (_syncLock)
            {

                using (var context = new DataAccessContext())
                {
                    var s = context.GetStudyStoreQuery().StudyQuery(new StudyRootStudyIdentifier
                                                                    {StudyInstanceUid = studyInstanceUid});
                    var identifier = CollectionUtils.FirstElement(s);

                    var client = new DicomSendBridge();
                    client.SendSops(remoteAEInfo, identifier, seriesInstanceUid, sopInstanceUids, WorkItemPriorityEnum.High);
                    _sendOperations.Add(new SendOperationInfo(client.WorkItem, message.MessageId, presentationID, server)
                                            {
                                                SubOperations = sopInstanceUids.Length
                                            });
                }
            }
		}

		private void OnReceiveCancelRequest(DicomMessage message)
		{
			lock (_syncLock)
			{
                foreach (SendOperationInfo info in _sendOperations)
                {
                    if (info.MessageId == message.MessageIdBeingRespondedTo)
                    {
                        RemoveSendOperationInfo(info);
                        WorkItemService.WorkItemService.Instance.Update(new WorkItemUpdateRequest
                                                                            {
                                                                                Cancel = true,
                                                                                Identifier = info.WorkItemData.Identifier
                                                                            });
                    }                    
                }
			}
		}

		#endregion

		#region Overrides

		public override bool OnReceiveRequest(ClearCanvas.Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			//// Check for a Cancel message, and cancel the SCU.
			if (message.CommandField == DicomCommandField.CCancelRequest)
			{
				OnReceiveCancelRequest(message);
				return true;
			}

		    // TODO (CR Jun 2012): Log when there's more than 1.

		    var remoteAE = ServerDirectory.GetRemoteServersByAETitle(message.MoveDestination).FirstOrDefault();
            if (remoteAE == null)
			{
				server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
					DicomStatuses.QueryRetrieveMoveDestinationUnknown);
				return true;
			}

			String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, string.Empty);

			try
			{
				if (level.Equals("STUDY"))
				{
                    OnReceiveMoveStudiesRequest(server, presentationID, message, remoteAE);
				}
				else if (level.Equals("SERIES"))
				{
                    OnReceiveMoveSeriesRequest(server, presentationID, message, remoteAE);
				}
				else if (level.Equals("IMAGE"))
				{
                    OnReceiveMoveImageRequest(server, presentationID, message, remoteAE);
				}
				else
				{
					Platform.Log(LogLevel.Error, "Unexpected Study Root Move Query/Retrieve level: {0}", level);

					server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
					                         DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
					return true;
				}
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing C-MOVE-RQ");
				try
				{
					server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
											 DicomStatuses.QueryRetrieveUnableToProcess, e.Message);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex,
								 "Unable to send final C-MOVE-RSP message on association from {0} to {1}",
								 association.CallingAE, association.CalledAE);
				}
			}

			return true;
		}

        public override void Cleanup()
        {
            _activityMonitor.WorkItemsChanged -= UpdateProgress;
            _activityMonitor.Dispose();            
        }

		#endregion
	}
}
