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
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Network
{
	/// <summary>
	/// Internal enumerated value used to represent the DICOM Upper Layer State Machine (Part PS 3.8, Section 9.2.1
	/// </summary>
	internal enum DicomAssociationState
	{
		// ReSharper disable InconsistentNaming
		Sta1_Idle,
		Sta2_TransportConnectionOpen,
		Sta3_AwaitingLocalAAssociationResponsePrimative,
		Sta4_AwaitingTransportConnectionOpeningToComplete,
		Sta5_AwaitingAAssociationACOrReject,
		Sta6_AssociationEstablished,
		Sta7_AwaitingAReleaseRP,
		Sta8_AwaitingAReleaseRPLocalUser,
		Sta9_ReleaseCollisionRequestorSide,
		Sta10_ReleaseCollisionAcceptorSide,
		Sta11_ReleaseCollisionRequestorSide,
		Sta12_ReleaseCollisionAcceptorSide,
		Sta13_AwaitingTransportConnectionClose
		// ReSharper restore InconsistentNaming
	}

	/// <summary>
	/// Query/Retrieve levels defined by DICOM
	/// </summary>
	public enum DicomQueryRetrieveLevel
	{
		Patient,
		Study,
		Series,
		Instance,
		Worklist
	}

	/// <summary>
	/// An enumerated value representing the priority values encoded in the tag <see cref="DicomTags.Priority"/>.
	/// </summary>
	public enum DicomPriority : ushort
	{
		Low = 0x0002,
		Medium = 0x0000,
		High = 0x0001
	}

	/// <summary>
	/// An enumerated value represneting the values for the tag <see cref="DicomTags.CommandField"/>.
	/// </summary>
	public enum DicomCommandField : ushort
	{
		CStoreRequest = 0x0001,
		CStoreResponse = 0x8001,
		CGetRequest = 0x0010,
		CGetResponse = 0x8010,
		CFindRequest = 0x0020,
		CFindResponse = 0x8020,
		CMoveRequest = 0x0021,
		CMoveResponse = 0x8021,
		CEchoRequest = 0x0030,
		CEchoResponse = 0x8030,
		NEventReportRequest = 0x0100,
		NEventReportResponse = 0x8100,
		NGetRequest = 0x0110,
		NGetResponse = 0x8110,
		NSetRequest = 0x0120,
		NSetResponse = 0x8120,
		NActionRequest = 0x0130,
		NActionResponse = 0x8130,
		NCreateRequest = 0x0140,
		NCreateResponse = 0x8140,
		NDeleteRequest = 0x0150,
		NDeleteResponse = 0x8150,
		CCancelRequest = 0x0FFF
	}

	internal class DcmDimseInfo
	{
		public DicomAttributeCollection Command;
		public DicomAttributeCollection Dataset;
		public ChunkStream CommandData;
		public ChunkStream DatasetData;
		public DicomStreamReader CommandReader;
		public DicomStreamReader DatasetReader;
		public bool IsNewDimse;
		public DicomReadStatus ParseDatasetStatus;
		public DicomTag DatasetStopTag;
		public bool StreamMessage;

		public DcmDimseInfo()
		{
			IsNewDimse = true;
			ParseDatasetStatus = DicomReadStatus.NeedMoreData;
			DatasetStopTag = null;
			StreamMessage = false;
		}
	}

	/// <summary>
	/// Class used for DICOM network communications.
	/// </summary>
	/// <remarks>
	/// The classes <see cref="DicomClient"/>"/> and <see cref="DicomServer"/> inherit from this class, to implement network functionality.
	/// </remarks>
	public abstract class NetworkBase
	{
		#region Protected Members

		private const int Timeout = 60; // Default timeout when none set
		private ushort _messageId;
		private Stream _network;
		protected AssociationParameters _assoc;
		private DcmDimseInfo _dimse;
		private Thread _processThread;
		private Thread _readThread;
		private Thread _writeThread;
		private bool _stop;
		internal DicomAssociationState State = DicomAssociationState.Sta1_Idle;
		private DateTime _dimseTimeout;
		private readonly BlockingQueue<RawPDU> _pduQueue = new BlockingQueue<RawPDU>();
		private readonly BlockingQueue<NetworkProcessor> _processingQueue = new BlockingQueue<NetworkProcessor>();
		private readonly object _syncLock = new object();
		private readonly object _writeSyncLock = new object();
		private bool _multiThreaded;

		#endregion

		#region Public members

		public AssociationParameters AssociationParams
		{
			get { return _assoc; }
		}

		/// <summary>
		/// Flag telling if informational level logging should be done.
		/// </summary>
		public bool LogInformation { get; set; }

		/// <summary>
		/// Flag telling if the network 
		/// </summary>
		public bool NetworkActive
		{
			get { return _network != null; }
		}

		public bool StreamMessage
		{
			set
			{
				if (_dimse != null)
					_dimse.StreamMessage = value;
			}
		}

		/// <summary>
		/// When reading the dataset of a DIMSE message, set tag at which to stop parsing the data.
		/// </summary>
		public DicomTag DimseDatasetStopTag
		{
			set
			{
				if (_dimse != null)
					_dimse.DatasetStopTag = value;
			}
		}

		/// <summary>
		/// The number of outstanding operations.  Used when asynchronous operations are negotiated.
		/// </summary>
		public ushort OutstandingOperations { get; set; }

		#endregion

		#region Public Constructors

		protected NetworkBase()
		{
			LogInformation = true;
			_messageId = 1;
		}

		#endregion

		#region Protected Methods

		protected void InitializeNetwork(Stream network, String name)
		{
			InitializeNetwork(network, name, false);
		}

		protected void InitializeNetwork(Stream network, String name, bool multiThreaded)
		{
			_network = network;
			_stop = false;
			_readThread = new Thread(RunRead);
			_readThread.Name = String.Format("{0} Read [{1}]", name, _readThread.ManagedThreadId);
			_readThread.Start();

			_multiThreaded = multiThreaded;

			if (multiThreaded)
			{
				_writeThread = new Thread(RunWrite);
				_writeThread.Name = String.Format("{0} Write [{1}]", name, _writeThread.ManagedThreadId);
				_writeThread.Start();

				_processThread = new Thread(RunProcess);
				_processThread.Name = String.Format("{0} Process [{1}]", name, _processThread.ManagedThreadId);
				_processThread.Start();
			}
		}

		/// <summary>
		/// Method for shutting down the network thread.  Should only be called from the CloseNetwork() routine.
		/// </summary>
		protected void ShutdownNetworkThread(int millisecondsTimeout)
		{
			_stop = true;
			if (_readThread != null)
			{
				if (!Thread.CurrentThread.Equals(_readThread))
				{
					_readThread.Join(millisecondsTimeout);
					_readThread = null;
				}
			}

			if (_writeThread != null)
			{
				_pduQueue.ContinueBlocking = false;

				if (!Thread.CurrentThread.Equals(_writeThread))
				{
					_writeThread.Join(millisecondsTimeout);
					_writeThread = null;
				}
			}

			if (_processThread != null)
			{
				_processingQueue.ContinueBlocking = false;

				if (!Thread.CurrentThread.Equals(_processThread))
				{
					_processThread.Join(millisecondsTimeout);
					_processThread = null;
				}
			}
		}

		/// <summary>
		/// Method called after network activity to reset the time when a DIMSE timeout will occur.
		/// </summary>
		protected void ResetDimseTimeout()
		{
			lock (_syncLock)
			{
				//TODO (Time Review): use Environment.TickCount.  It probably won't have much of
				//a performance impact here, but it is pretty inefficient when called in a tight loop.
				_dimseTimeout = _assoc != null
					? DateTime.Now.AddMilliseconds(_assoc.ReadTimeout)
					: DateTime.Now.AddSeconds(Timeout);
			}
		}

		/// <summary>
		/// Return a synchronized version of time for the Dimse Timeout
		/// </summary>
		/// <returns></returns>
		protected DateTime GetDimseTimeout()
		{
			lock (_syncLock)
				return _dimseTimeout;
		}

		/// <summary>
		/// Method for closing the network connection.
		/// </summary>
		/// <param name="millisecondsTimeout">The timeout in milliseconds to wait for the closure
		/// of the network.</param>
		protected abstract void CloseNetwork(int millisecondsTimeout);

		/// <summary>
		/// Internal routine for enqueueing a PDU for transfer.
		/// </summary>
		/// <param name="pdu"></param>
		internal void EnqueuePdu(RawPDU pdu)
		{
			if (_multiThreaded)
				_pduQueue.Enqueue(pdu);
			else
				SendRawPDU(pdu);
		}

		protected abstract bool NetworkHasData();

		protected virtual void OnUserException(Exception e, String description)
		{
			Platform.Log(LogLevel.Error, e, "Unexpected User exception, description: " + description);
			switch (State)
			{
				case DicomAssociationState.Sta2_TransportConnectionOpen:
					OnNetworkError(e, true);
					break;
				case DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative:
					OnNetworkError(e, true);
					break;
				case DicomAssociationState.Sta4_AwaitingTransportConnectionOpeningToComplete:
					OnNetworkError(e, true);
					break;
				case DicomAssociationState.Sta5_AwaitingAAssociationACOrReject:
					OnNetworkError(e, true);
					break;
				case DicomAssociationState.Sta6_AssociationEstablished:
					Platform.Log(LogLevel.Error, "Aborting association from {0} to {1}", _assoc.CallingAE, _assoc.CalledAE);
					SendAssociateAbort(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified);
					OnNetworkError(e, false);
					break;
				case DicomAssociationState.Sta7_AwaitingAReleaseRP:
					OnNetworkError(e, true);
					break;
				case DicomAssociationState.Sta8_AwaitingAReleaseRPLocalUser:
					OnNetworkError(e, true);
					break;
				default:
					OnNetworkError(e, true);
					break;
			}
		}

		/// <summary>
		/// Callback called on a network error.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="closeConnection"></param>
		protected virtual void OnNetworkError(Exception e, bool closeConnection) {}

		/// <summary>
		/// Callback called on a timeout.
		/// </summary>
		protected virtual void OnDimseTimeout() {}

		protected virtual void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected virtual void OnReceiveAssociateRequest(ServerAssociationParameters association)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected virtual void OnReceiveAssociateAccept(AssociationParameters association) {}

		protected virtual void OnReceiveAssociateReject(DicomRejectResult result, DicomRejectSource source,
		                                                DicomRejectReason reason)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected virtual void OnReceiveReleaseRequest()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected virtual void OnReceiveReleaseResponse()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected virtual void OnReceiveDimseRequest(byte pcid, DicomMessage msg) {}

		protected virtual void OnReceiveDimseResponse(byte pcid, DicomMessage msg) {}

		protected virtual void OnReceiveDimseCommand(byte pcid, DicomAttributeCollection command) {}

		protected virtual bool OnReceiveFileStream(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset, byte[] data, int offset, int count, bool encounteredStopTag, bool isFirst, bool isLast)
		{
			return false;
		}

		private bool OnReceiveDimse(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset)
		{
			var msg = new DicomMessage(command, dataset);
			DicomCommandField commandField = msg.CommandField;

			if ((commandField == DicomCommandField.CStoreRequest)
			    || (commandField == DicomCommandField.CCancelRequest)
			    || (commandField == DicomCommandField.CEchoRequest)
			    || (commandField == DicomCommandField.CFindRequest)
			    || (commandField == DicomCommandField.CGetRequest)
			    || (commandField == DicomCommandField.CMoveRequest)
			    || (commandField == DicomCommandField.NActionRequest)
			    || (commandField == DicomCommandField.NCreateRequest)
			    || (commandField == DicomCommandField.NDeleteRequest)
			    || (commandField == DicomCommandField.NEventReportRequest)
			    || (commandField == DicomCommandField.NGetRequest)
			    || (commandField == DicomCommandField.NSetRequest))
			{
				msg.TransferSyntax = _assoc.GetAcceptedTransferSyntax(pcid);
				OnReceiveDimseRequest(pcid, msg);

				if (MessageReceived != null)
					MessageReceived(_assoc, msg);

				return true;
			}

			if ((commandField == DicomCommandField.CStoreResponse)
			    || (commandField == DicomCommandField.CEchoResponse)
			    || (commandField == DicomCommandField.CFindResponse)
			    || (commandField == DicomCommandField.CGetResponse)
			    || (commandField == DicomCommandField.CMoveResponse)
			    || (commandField == DicomCommandField.NActionResponse)
			    || (commandField == DicomCommandField.NCreateResponse)
			    || (commandField == DicomCommandField.NDeleteResponse)
			    || (commandField == DicomCommandField.NEventReportResponse)
			    || (commandField == DicomCommandField.NGetResponse)
			    || (commandField == DicomCommandField.NSetResponse))
			{
				OnReceiveDimseResponse(pcid, msg);

				if (MessageReceived != null)
					MessageReceived(_assoc, msg);
				return true;
			}
			return false;
		}

		protected virtual void OnDimseSent(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset)
		{
			var msg = new DicomMessage(command, dataset);
			DicomCommandField commandField = msg.CommandField;

			if ((commandField == DicomCommandField.CStoreRequest)
			    || (commandField == DicomCommandField.CCancelRequest)
			    || (commandField == DicomCommandField.CEchoRequest)
			    || (commandField == DicomCommandField.CFindRequest)
			    || (commandField == DicomCommandField.CGetRequest)
			    || (commandField == DicomCommandField.CMoveRequest)
			    || (commandField == DicomCommandField.NActionRequest)
			    || (commandField == DicomCommandField.NCreateRequest)
			    || (commandField == DicomCommandField.NDeleteRequest)
			    || (commandField == DicomCommandField.NEventReportRequest)
			    || (commandField == DicomCommandField.NGetRequest)
			    || (commandField == DicomCommandField.NSetRequest))
			{
				if (MessageSent != null)
					MessageSent(_assoc, msg);
			}

			if ((commandField == DicomCommandField.CStoreResponse)
			    || (commandField == DicomCommandField.CEchoResponse)
			    || (commandField == DicomCommandField.CFindResponse)
			    || (commandField == DicomCommandField.CGetResponse)
			    || (commandField == DicomCommandField.CMoveResponse)
			    || (commandField == DicomCommandField.NActionResponse)
			    || (commandField == DicomCommandField.NCreateResponse)
			    || (commandField == DicomCommandField.NDeleteResponse)
			    || (commandField == DicomCommandField.NEventReportResponse)
			    || (commandField == DicomCommandField.NGetResponse)
			    || (commandField == DicomCommandField.NSetResponse))
			{
				if (MessageSent != null)
					MessageSent(_assoc, msg);
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Defines an event handler  when an association has been established.
		/// </summary>
		/// <param name="assoc"></param>
		public delegate void AssociationEstablishedEventHandler(AssociationParameters assoc);

		/// <summary>
		/// Defines an event handler  when an association has been rejected.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="reason"></param>
		public delegate void AssociationRejectedEventHandler(DicomRejectSource source, DicomRejectReason reason);

		/// <summary>
		/// Defines an event handler  when an association is being released.
		/// </summary>
		/// <param name="assoc"></param>
		public delegate void AssociationReleasingEventHandler(AssociationParameters assoc);

		/// <summary>
		/// Defines an event handler  when an association has been released.
		/// </summary>
		/// <param name="assoc"></param>
		public delegate void AssociationReleasedEventHandler(AssociationParameters assoc);

		/// <summary>
		/// Defines an event handler  when an association has been aborted.
		/// </summary>
		/// <param name="assoc"></param>
		/// <param name="reason"></param>
		public delegate void AssociationAbortedEventHandler(AssociationParameters assoc, DicomAbortReason reason);

		/// <summary>
		/// Defines an event handler  when a Dimse message is being sent.
		/// </summary>
		/// <param name="assoc"></param>
		/// <param name="presentationContextID"></param>
		/// <param name="command"></param>
		/// <param name="dataset"></param>
		public delegate void DimseMessageSendingEventHandler(
			AssociationParameters assoc, byte presentationContextID, DicomAttributeCollection command,
			DicomAttributeCollection dataset);

		/// <summary>
		/// Defines an event handler  when a Dimse message is being received.
		/// </summary>
		/// <param name="assoc"></param>
		/// <param name="presentationContextID"></param>
		public delegate void DimseMessageReceivingEventHandler(AssociationParameters assoc, byte presentationContextID);

		/// <summary>
		/// Defines an event handler  when a Dicom message has been sent.
		/// </summary>
		/// <param name="assoc"></param>
		/// <param name="msg"/>
		public delegate void MessageSentEventHandler(AssociationParameters assoc, DicomMessage msg);

		/// <summary>
		/// Defines an event handler  when a Dicom message has been received.
		/// </summary>
		/// <param name="assoc"></param>
		/// <param name="msg"/>
		public delegate void MessageReceivedEventHandler(AssociationParameters assoc, DicomMessage msg);

		/// <summary>
		/// Defines an event handler  when the network stream has been closed.
		/// </summary>
		/// <param name="data"></param>
		public delegate void NetworkClosedEventHandler(object data);

		/// <summary>
		/// Delegate for processing events that occur on the network.
		/// </summary>
		public delegate void NetworkProcessor();

		/// <summary>
		/// Defines an event handler  when a network error occurs
		/// </summary>
		/// <param name="data"/>
		public delegate void NetworkErrorEventHandler(object data);

		/// <summary>
		/// Occurs when an association has been established between the called AE and calling AE.
		/// </summary>
		public event AssociationEstablishedEventHandler AssociationEstablished;

		/// <summary>
		/// Occurs when an association has been rejected.
		/// </summary>
		public event AssociationRejectedEventHandler AssociationRejected;

		/// <summary>
		/// Occurs when an association is being released.
		/// </summary>
		public event AssociationReleasingEventHandler AssociationReleasing;

		/// <summary>
		/// Occurs when an association has been released.
		/// </summary>
		public event AssociationReleasedEventHandler AssociationReleased;

		/// <summary>
		/// Occurs when an association is being aborted.
		/// </summary>
		public event AssociationAbortedEventHandler AssociationAborted;

		/// <summary>
		/// Occurs when a dimse message is being sent.
		/// </summary>
		public event DimseMessageSendingEventHandler DimseMessageSending;

		/// <summary>
		/// Occurs when a dimse message is being received.
		/// </summary>
		public event DimseMessageReceivingEventHandler DimseMessageReceiving;

		/// <summary>
		/// Occurs when a dicom message has been sent.
		/// </summary>
		public event MessageSentEventHandler MessageSent;

		/// <summary>
		/// Occurs when a dicom message has been received.
		/// </summary>
		public event MessageReceivedEventHandler MessageReceived;

		/// <summary>
		/// Occurs when a network stream has been closed.
		/// </summary>
		public event NetworkClosedEventHandler NetworkClosed;

		/// <summary>
		/// Occurs when a network error occured.
		/// </summary>
		public event NetworkErrorEventHandler NetworkError;

		#endregion

		#region Public Methods

		/// <summary>
		/// Force a shutdown of the DICOM connection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will force the network connection for the <see cref="DicomClient"/> or 
		/// <see cref="DicomServer"/>to be closed and the background thread for processing the 
		/// association to shutdown.  The routine will block for the number of milliseconds specified
		/// by <param name="millisecondTimeout"/>.
		/// </para>
		/// <para>
		/// Note, for a graceful shutdown the <see cref="SendAssociateAbort"/> or 
		/// <see cref="SendReleaseRequest"/> methods should be called.  These routines
		/// will gracefully shutdown DICOM connections.  The <see cref="DicomClient.Join()"/>
		/// method can then be called to wait for the background thread to clean up.
		/// </para>
		/// </remarks>
		public void Abort(int millisecondTimeout)
		{
			try
			{
				if (State != DicomAssociationState.Sta1_Idle
				    && State != DicomAssociationState.Sta13_AwaitingTransportConnectionClose
				    && _network != null)
					SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
			}
			finally
			{
				CloseNetwork(millisecondTimeout);
			}
		}

		/// <summary>
		/// Force a shutdown of the DICOM connection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will force the network connection for the <see cref="DicomClient"/> or 
		/// <see cref="DicomServer"/>to be closed and the background thread for processing the 
		/// association to shutdown.  The routine will block until the shutdown has completed.
		/// </para>
		/// <para>
		/// Note, for a graceful shutdown the <see cref="SendAssociateAbort"/> or 
		/// <see cref="SendReleaseRequest"/> methods should be called.  These routines
		/// will gracefully shutdown DICOM connections.  The <see cref="DicomClient.Join()"/>
		/// method can then be called to wait for the background thread to clean up.
		/// </para>
		/// </remarks>
		public void Abort()
		{
			try
			{
				if (State != DicomAssociationState.Sta1_Idle
				    && State != DicomAssociationState.Sta13_AwaitingTransportConnectionClose)
					SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
			}
			finally
			{
				CloseNetwork(System.Threading.Timeout.Infinite);
			}
		}

		/// <summary>
		/// Returns the next message Id to be used over the association.
		/// </summary>
		/// <returns></returns>
		public ushort NextMessageID()
		{
			return _messageId++;
		}

		/// <summary>
		/// Method used to send an association request.
		/// </summary>
		/// <param name="associate">The parameters used in the association request.</param>
		public void SendAssociateRequest(AssociationParameters associate)
		{
			_assoc = associate;
			var pdu = new AAssociateRQ(_assoc);

			State = DicomAssociationState.Sta5_AwaitingAAssociationACOrReject;

			EnqueuePdu(pdu.Write());
		}

		/// <summary>
		/// Method to send an association abort PDU.
		/// </summary>
		/// <param name="source">The source of the abort.</param>
		/// <param name="reason">The reason for the abort.</param>
		public void SendAssociateAbort(DicomAbortSource source, DicomAbortReason reason)
		{
			if (State != DicomAssociationState.Sta13_AwaitingTransportConnectionClose)
			{
				var pdu = new AAbort(source, reason);

				EnqueuePdu(pdu.Write());
				State = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;

				if (AssociationAborted != null)
					AssociationAborted(_assoc, reason);
			}
			else
			{
				Platform.Log(LogLevel.Error, "Unexpected state for association abort, closing connection from {0} to {1}",
				             _assoc.CallingAE, _assoc.CalledAE);

				OnNetworkError(null, true);

				if (NetworkClosed != null)
					NetworkClosed("Unexpected state for association abort");
			}
		}

		/// <summary>
		/// Method to send an association accept.
		/// </summary>
		/// <param name="associate">The parameters to use for the association accept.</param>
		public void SendAssociateAccept(AssociationParameters associate)
		{
			if (State != DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative)
			{
				Platform.Log(LogLevel.Error, "Error attempting to send association accept at invalid time in association.");
				SendAssociateAbort(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified);
				throw new DicomNetworkException(
					"Attempting to send association accept at invalid time in association, aborting");
			}
			var pdu = new AAssociateAC(_assoc);

			EnqueuePdu(pdu.Write());

			State = DicomAssociationState.Sta6_AssociationEstablished;

			if (AssociationEstablished != null)
				AssociationEstablished(_assoc);
		}

		/// <summary>
		/// Method to send an association rejection.
		/// </summary>
		/// <param name="result">The </param>
		/// <param name="source"></param>
		/// <param name="reason"></param>
		public void SendAssociateReject(DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
		{
			if (State != DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative)
			{
				Platform.Log(LogLevel.Error, "Error attempting to send association reject at invalid time in association.");
				SendAssociateAbort(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified);
				throw new DicomNetworkException(
					"Attempting to send association reject at invalid time in association, aborting");
			}
			var pdu = new AAssociateRJ(result, source, reason);

			EnqueuePdu(pdu.Write());

			State = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;

			if (AssociationRejected != null)
				AssociationRejected(source, reason);
		}

		/// <summary>
		/// Method to send an association release request.  this method can only be used by clients.
		/// </summary>
		public void SendReleaseRequest()
		{
			if (State != DicomAssociationState.Sta6_AssociationEstablished)
			{
				Platform.Log(LogLevel.Error, "Unexpected attempt to send Release Request when in invalid state.");
				return;
			}

			var pdu = new AReleaseRQ();

			EnqueuePdu(pdu.Write());

			State = DicomAssociationState.Sta7_AwaitingAReleaseRP;

			// still waiting for remote AE to send release response
			if (AssociationReleasing != null)
				AssociationReleasing(_assoc);
		}

		/// <summary>
		/// Method to send an association release response.
		/// </summary>
		protected void SendReleaseResponse()
		{
			if (State != DicomAssociationState.Sta8_AwaitingAReleaseRPLocalUser) {}

			var pdu = new AReleaseRP();

			EnqueuePdu(pdu.Write());
			State = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;

			if (AssociationReleased != null)
				AssociationReleased(_assoc);
		}

		/// <summary>
		/// Method to send a DICOM C-ECHO-RQ message.
		/// </summary>
		/// <param name="presentationID">The presentation context to send the request on.</param>
		/// <param name="messageID">The messageID to use.</param>
		public void SendCEchoRequest(byte presentationID, ushort messageID)
		{
			Platform.Log(LogLevel.Info, "Sending C Echo request, pres ID: {0}, messageID = {1}", presentationID, messageID);
			var msg = new DicomMessage
			          {
				          MessageId = messageID,
				          CommandField = DicomCommandField.CEchoRequest,
				          AffectedSopClassUid = _assoc.GetAbstractSyntax(presentationID).UID,
				          DataSetType = 0x0101
			          };

			SendDimse(presentationID, msg.CommandSet, null);
		}

		/// <summary>
		/// Method to send a DICOM C-ECHO-RSP message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="status"></param>
		public void SendCEchoResponse(byte presentationID, ushort messageID, DicomStatus status)
		{
			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
			var msg = new DicomMessage
			          {
				          MessageIdBeingRespondedTo = messageID,
				          CommandField = DicomCommandField.CEchoResponse,
				          AffectedSopClassUid = affectedClass.UID,
				          DataSetType = 0x0101,
				          Status = status
			          };

			SendDimse(presentationID, msg.CommandSet, null);
		}

		/// <summary>
		/// Method to send a DICOM C-STORE-RQ message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="priority"></param>
		/// <param name="message"></param>
		public void SendCStoreRequest(byte presentationID, ushort messageID,
		                              DicomPriority priority, DicomMessage message)
		{
			SendCStoreRequest(presentationID, messageID, priority, null, 0, message);
		}

		/// <summary>
		/// Method to send a DICOM C-STORE-RQ message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="priority"></param>
		/// <param name="message"></param>
		/// <param name="overrideParameters"></param>
		public void SendCStoreRequest(byte presentationID, ushort messageID,
		                              DicomPriority priority, DicomMessage message,
		                              DicomCodecParameters overrideParameters)
		{
			SendCStoreRequest(presentationID, messageID, priority, null, 0, message, overrideParameters);
		}

		/// <summary>
		/// Method to send a DICOM C-STORE-RQ message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="priority"></param>
		/// <param name="moveAE"></param>
		/// <param name="moveMessageID"></param>
		/// <param name="message"></param>
		public void SendCStoreRequest(byte presentationID, ushort messageID,
		                              DicomPriority priority, string moveAE, ushort moveMessageID, DicomMessage message)
		{
			SendCStoreRequest(presentationID, messageID, priority, moveAE, moveMessageID, message, null);
		}

		/// <summary>
		/// Method to send a DICOM C-STORE-RQ message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="priority"></param>
		/// <param name="moveAE"></param>
		/// <param name="moveMessageID"></param>
		/// <param name="message"></param>
		/// <param name="overrideParameters"></param>
		public void SendCStoreRequest(byte presentationID, ushort messageID,
		                              DicomPriority priority, string moveAE, ushort moveMessageID, DicomMessage message,
		                              DicomCodecParameters overrideParameters)
		{
			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);

			if (!affectedClass.UID.Equals(message.SopClass.Uid))
				throw new DicomException(
					String.Format(
						"SOP Class Uid in the message {0} does not match SOP Class UID for presentation context {1}",
						message.SopClass.Uid, affectedClass.UID));

			DicomAttributeCollection command = message.MetaInfo;

			message.MessageId = messageID;
			message.CommandField = DicomCommandField.CStoreRequest;
			message.AffectedSopClassUid = message.SopClass.Uid;
			message.DataSetType = 0x0202;
			message.Priority = priority;

			String sopInstanceUid;
			bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetString(0, out sopInstanceUid);
			if (!ok)
				throw new DicomException("SOP Instance UID unexpectedly not set in CStore Message being sent.");

			message.AffectedSopInstanceUid = sopInstanceUid;

			if (!string.IsNullOrEmpty(moveAE))
			{
				message.MoveOriginatorApplicationEntityTitle = moveAE;
				message.MoveOriginatorMessageId = moveMessageID;
			}

			// Handle compress/decompress if necessary
			TransferSyntax contextSyntax = _assoc.GetAcceptedTransferSyntax(presentationID);
			if ((contextSyntax != message.TransferSyntax)
			    && (contextSyntax.Encapsulated || message.TransferSyntax.Encapsulated))
			{
				if (overrideParameters != null)
					message.ChangeTransferSyntax(contextSyntax, null, overrideParameters);
				else
					message.ChangeTransferSyntax(contextSyntax);
			}

			SendDimse(presentationID, command, message.DataSet);
		}

		/// <summary>
		/// Method to send a DICOM C-STORE-RQ message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="priority"></param>
		/// <param name="moveAE"></param>
		/// <param name="moveMessageID"></param>
		/// <param name="dataSetStream"></param>
		public void SendCStoreRequest(byte presentationID, ushort messageID,
		                              DicomPriority priority, string moveAE, ushort moveMessageID, string sopInstanceUid, string sopClassUid, Stream dataSetStream)
		{
			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);

			DicomMessage message = new DicomMessage();
			DicomAttributeCollection command = message.MetaInfo;

			message.MessageId = messageID;
			message.CommandField = DicomCommandField.CStoreRequest;
			message.AffectedSopClassUid = sopClassUid;
			message.DataSetType = 0x0202;
			message.Priority = priority;
			message.AffectedSopInstanceUid = sopInstanceUid;

			if (!string.IsNullOrEmpty(moveAE))
			{
				message.MoveOriginatorApplicationEntityTitle = moveAE;
				message.MoveOriginatorMessageId = moveMessageID;
			}

			SendDimseDataSetStream(presentationID, command, dataSetStream);
		}

		/// <summary>
		/// Method to send a DICOM C-STORE-RSP message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="affectedInstance"></param>
		/// <param name="status"></param>
		public void SendCStoreResponse(byte presentationID, ushort messageID, string affectedInstance, DicomStatus status)
		{
			var msg = new DicomMessage
			          {
				          MessageIdBeingRespondedTo = messageID,
				          CommandField = DicomCommandField.CStoreResponse,
				          AffectedSopClassUid = _assoc.GetAbstractSyntax(presentationID).UID,
				          AffectedSopInstanceUid = affectedInstance,
				          DataSetType = 0x0101,
				          Status = status
			          };

			SendDimse(presentationID, msg.CommandSet, null);
		}

		/// <summary>
		/// Method to send a DICOM C-STORE-RSP message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="affectedInstance"></param>
		/// <param name="status"></param>
		/// <param name="errorComment">An extended textual error comment on failure. The comment will be truncated to 64 characters.</param>
		public void SendCStoreResponse(byte presentationID, ushort messageID, string affectedInstance, DicomStatus status, string errorComment)
		{
			var msg = new DicomMessage
			          {
				          MessageIdBeingRespondedTo = messageID,
				          CommandField = DicomCommandField.CStoreResponse,
				          AffectedSopClassUid = _assoc.GetAbstractSyntax(presentationID).UID,
				          AffectedSopInstanceUid = affectedInstance,
				          DataSetType = 0x0101,
				          Status = status,
			          };

			if (!string.IsNullOrEmpty(errorComment))
			{
				msg.ErrorComment = errorComment.Substring(0, (int) Math.Min(DicomVr.LOvr.MaximumLength, errorComment.Length));
			}

			SendDimse(presentationID, msg.CommandSet, null);
		}

		/// <summary>
		/// Method to send a DICOM C-FIND-RQ message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="message"></param>
		public void SendCFindRequest(byte presentationID, ushort messageID, DicomMessage message)
		{
			if (message.DataSet.IsEmpty())
				throw new DicomException("Unexpected empty DataSet when sending C-FIND-RQ.");

			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);

			message.AffectedSopClassUid = affectedClass.UID;
			message.MessageId = messageID;
			message.CommandField = DicomCommandField.CFindRequest;
			if (!message.CommandSet.Contains(DicomTags.Priority))
				message.Priority = DicomPriority.Medium;
			message.DataSetType = 0x0202;

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Method to send a DICOM C-CANCEL-FIND-RQ message.
		/// </summary>
		/// <param name="messageId">The message ID of the original C-FIND-RQ that is being canceled</param>
		/// <param name="presentationId"></param>
		public void SendCFindCancelRequest(byte presentationId, ushort messageId)
		{
			var message = new DicomMessage
			              {
				              CommandField = DicomCommandField.CCancelRequest,
				              DataSetType = 0x0101,
				              MessageIdBeingRespondedTo = messageId
			              };

			SendDimse(presentationId, message.CommandSet, null);
		}

		/// <summary>
		/// Method to send a DICOM C-FIND-RSP message.
		/// </summary>
		/// <param name="presentationId"></param>
		/// <param name="messageId"></param>
		/// <param name="status"></param>
		/// <param name="message"></param>
		public void SendCFindResponse(byte presentationId, ushort messageId, DicomMessage message, DicomStatus status)
		{
			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationId);
			message.CommandField = DicomCommandField.CFindResponse;
			message.Status = status;
			message.MessageIdBeingRespondedTo = messageId;
			message.AffectedSopClassUid = affectedClass.UID;
			message.DataSetType = message.DataSet.IsEmpty() ? (ushort) 0x0101 : (ushort) 0x0202;

			SendDimse(presentationId, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Method to send a DICOM C-MOVE-RQ message.
		/// </summary>
		/// <param name="presentationId"></param>
		/// <param name="messageId"></param>
		/// <param name="destinationAE"></param>
		/// <param name="message"></param>
		public void SendCMoveRequest(byte presentationId, ushort messageId, string destinationAE, DicomMessage message)
		{
			if (message.DataSet.IsEmpty())
				throw new DicomException("Unexpected empty DataSet when sending C-MOVE-RQ.");

			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationId);
			message.CommandField = DicomCommandField.CMoveRequest;
			message.MessageId = messageId;
			message.AffectedSopClassUid = affectedClass.UID;
			if (!message.CommandSet.Contains(DicomTags.Priority))
				message.Priority = DicomPriority.Medium;
			message.DataSetType = 0x0202;
			message.MoveDestination = destinationAE;
			SendDimse(presentationId, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Method to send a DICOM C-CANCEL-MOVE-RQ message.
		/// </summary>
		/// <param name="messageID">The message ID of the original C-MOVE-RQ that is being canceled</param>
		/// <param name="presentationID"></param>
		public void SendCMoveCancelRequest(byte presentationID, ushort messageID)
		{
			var message = new DicomMessage
			              {
				              CommandField = DicomCommandField.CCancelRequest,
				              DataSetType = 0x0101,
				              MessageIdBeingRespondedTo = messageID
			              };
			SendDimse(presentationID, message.CommandSet, null);
		}

		/// <summary>
		/// Method to send a DICOM C-MOVE-RSP message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="message"></param>
		/// <param name="status"></param>
		public void SendCMoveResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status)
		{
			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
			message.CommandField = DicomCommandField.CMoveResponse;
			message.Status = status;
			message.MessageIdBeingRespondedTo = messageID;
			message.AffectedSopClassUid = affectedClass.UID;
			message.DataSetType = message.DataSet.IsEmpty() ? (ushort) 0x0101 : (ushort) 0x0202;

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Method to send a DICOM C-MOVE-RSP message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="message"></param>
		/// <param name="status"></param>
		/// <param name="errorComment">An extended textual error comment. The comment will be truncated to 64 characters. </param>
		public void SendCMoveResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status, string errorComment)
		{
			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);
			message.CommandField = DicomCommandField.CMoveResponse;
			message.Status = status;
			message.MessageIdBeingRespondedTo = messageID;
			message.AffectedSopClassUid = affectedClass.UID;
			message.DataSetType = message.DataSet.IsEmpty() ? (ushort) 0x0101 : (ushort) 0x0202;
			if (!string.IsNullOrEmpty(errorComment))
				message.ErrorComment = errorComment.Substring(0, (int) Math.Min(DicomVr.LOvr.MaximumLength, errorComment.Length));

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Method to send a DICOM C-MOVE-RSP message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="message"></param>
		/// <param name="status"></param>
		/// <param name="numberOfCompletedSubOperations"></param>
		/// <param name="numberOfRemainingSubOperations"></param>
		/// <param name="numberOfFailedSubOperations"></param>
		/// <param name="numberOfWarningSubOperations"></param>
		public void SendCMoveResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status,
		                              ushort numberOfCompletedSubOperations, ushort numberOfRemainingSubOperations,
		                              ushort numberOfFailedSubOperations, ushort numberOfWarningSubOperations)
		{
			SendCMoveResponse(presentationID, messageID, message, status, numberOfCompletedSubOperations,
			                  numberOfRemainingSubOperations, numberOfFailedSubOperations, numberOfWarningSubOperations,
			                  string.Empty);
		}

		/// <summary>
		/// Method to send a DICOM C-MOVE-RSP message.
		/// </summary>
		/// <param name="presentationID"></param>
		/// <param name="messageID"></param>
		/// <param name="message"></param>
		/// <param name="status"></param>
		/// <param name="numberOfCompletedSubOperations"></param>
		/// <param name="numberOfRemainingSubOperations"></param>
		/// <param name="numberOfFailedSubOperations"></param>
		/// <param name="numberOfWarningSubOperations"></param>
		/// <param name="errorComment">An extended textual error comment. The comment will be truncated to 64 characters. </param>
		public void SendCMoveResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status,
		                              ushort numberOfCompletedSubOperations, ushort numberOfRemainingSubOperations,
		                              ushort numberOfFailedSubOperations, ushort numberOfWarningSubOperations,
		                              string errorComment)
		{
			message.CommandField = DicomCommandField.CMoveResponse;
			message.Status = status;
			message.MessageIdBeingRespondedTo = messageID;
			message.AffectedSopClassUid = _assoc.GetAbstractSyntax(presentationID).UID;
			message.NumberOfCompletedSubOperations = numberOfCompletedSubOperations;
			message.NumberOfRemainingSubOperations = numberOfRemainingSubOperations;
			message.NumberOfFailedSubOperations = numberOfFailedSubOperations;
			message.NumberOfWarningSubOperations = numberOfWarningSubOperations;
			message.DataSetType = message.DataSet.IsEmpty() ? (ushort) 0x0101 : (ushort) 0x0202;
			if (!string.IsNullOrEmpty(errorComment))
				message.ErrorComment = errorComment.Substring(0, (int) Math.Min(DicomVr.LOvr.MaximumLength, errorComment.Length));

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Sends an N-Get request.
		/// </summary>
		/// <param name="requestedSopInstanceUid">The requested sop instance uid.</param>
		/// <param name="presentationID">The presentation ID.</param>
		/// <param name="messageID">The message ID.</param>
		/// <param name="message">The message.</param>
		public void SendNGetRequest(DicomUid requestedSopInstanceUid, byte presentationID, ushort messageID, DicomMessage message)
		{
			if (message.DataSet.IsEmpty())
				throw new DicomException("Unexpected empty DataSet when sending N-GET-RQ.");

			DicomUid affectedClass = _assoc.GetAbstractSyntax(presentationID);

			message.AffectedSopClassUid = affectedClass.UID;
			message.MessageId = messageID;
			message.CommandField = DicomCommandField.NGetRequest;
			message.DataSetType = 0x0102;

			message.CommandSet[DicomTags.RequestedSopClassUid].SetStringValue(affectedClass.UID);
			message.CommandSet[DicomTags.RequestedSopInstanceUid].SetStringValue(requestedSopInstanceUid.UID);

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Sends an N-Create request, affected class is the one associated with the <paramref name="presentationID"/>.
		/// </summary>
		/// <param name="affectedSopInstanceUid">The affected sop instance uid.</param>
		/// <param name="presentationID">The presentation ID.</param>
		/// <param name="messageID">The message ID.</param>
		/// <param name="message">The message.</param>
		public void SendNCreateRequest(DicomUid affectedSopInstanceUid, byte presentationID, ushort messageID, DicomMessage message)
		{
			SendNCreateRequest(affectedSopInstanceUid, presentationID, messageID, message, null);
		}

		/// <summary>
		/// Sends an N-Create Request.
		/// </summary>
		/// <param name="affectedSopInstanceUid">The affected sop instance uid.</param>
		/// <param name="presentationID">The presentation ID.</param>
		/// <param name="messageID">The message ID.</param>
		/// <param name="message">The message.</param>
		/// <param name="affectedClass">The affected class.</param>
		public void SendNCreateRequest(DicomUid affectedSopInstanceUid, byte presentationID, ushort messageID, DicomMessage message, DicomUid affectedClass)
		{
			if (message.DataSet.IsEmpty())
				throw new DicomException("Unexpected empty DataSet when sending N-CREATE-RQ.");

			if (affectedClass == null)
				affectedClass = _assoc.GetAbstractSyntax(presentationID);

			message.CommandSet[DicomTags.AffectedSopClassUid].SetStringValue(affectedClass.UID);
			message.CommandSet[DicomTags.MessageId].SetUInt16(0, messageID);
			message.CommandSet[DicomTags.CommandField].SetUInt16(0, (ushort) DicomCommandField.NCreateRequest);
			message.CommandSet[DicomTags.DataSetType].SetUInt16(0, 0x0102);

			if (affectedSopInstanceUid != null)
				message.CommandSet[DicomTags.AffectedSopInstanceUid].SetStringValue(affectedSopInstanceUid.UID);

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Sends an N-Create Response.
		/// </summary>
		/// <param name="presentationID">The presentation context ID</param>
		/// <param name="messageID">The MessageID being responsed to.</param>
		/// <param name="message">The response message to send.</param>
		/// <param name="status">The status to send.</param>
		public void SendNCreateResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status)
		{
			SendNCreateNSetNDeleteHelper(DicomCommandField.NCreateResponse, presentationID, messageID, message, status);
		}

		/// <summary>
		/// Sends an N-Set request.
		/// </summary>
		/// <param name="presentationID">The presentation context ID to send the request over.</param>
		/// <param name="messageID">The message ID.</param>
		/// <param name="message">The message.</param>
		public void SendNSetRequest(byte presentationID, ushort messageID, DicomMessage message)
		{
			if (message.DataSet.IsEmpty())
				throw new DicomException("Unexpected empty DataSet when sending N-SET-RQ.");

			message.CommandSet[DicomTags.MessageId].SetUInt16(0, messageID);
			message.CommandSet[DicomTags.CommandField].SetUInt16(0, (ushort) DicomCommandField.NSetRequest);
			message.CommandSet[DicomTags.DataSetType].SetUInt16(0, 0x0102);

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Sends an N-Set Response.
		/// </summary>
		/// <param name="presentationID">The presentation context ID to send th response over.</param>
		/// <param name="messageID">The message ID.</param>
		/// <param name="message">The response message to send.</param>
		/// <param name="status">The status to set in the response message.</param>
		public void SendNSetResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status)
		{
			SendNCreateNSetNDeleteHelper(DicomCommandField.NSetResponse, presentationID, messageID, message, status);
		}

		/// <summary>
		/// Sends an N-Action request.
		/// </summary>
		/// <param name="presentationID">The presentation ID to send the request message on.</param>
		/// <param name="messageID">The message ID.</param>
		/// <param name="message">The message.</param>
		public void SendNActionRequest(byte presentationID, ushort messageID, DicomMessage message)
		{
			message.CommandSet[DicomTags.MessageId].SetUInt16(0, messageID);
			message.CommandSet[DicomTags.CommandField].SetUInt16(0, (ushort) DicomCommandField.NActionRequest);

			if (message.DataSet == null || message.DataSet.IsEmpty())
				message.CommandSet[DicomTags.DataSetType].SetUInt16(0, 0x101);
			else
				message.CommandSet[DicomTags.DataSetType].SetUInt16(0, 0x102);

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Sends an N-Action Response.
		/// </summary>
		/// <param name="presentationID">The presentation context ID to send the response message on.</param>
		/// <param name="messageID">The message ID of the message responding to.</param>
		/// <param name="message">The response message to send.</param>
		/// <param name="status">The status to set in the response message.</param>
		public void SendNActionResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status)
		{
			SendNCreateNSetNDeleteHelper(DicomCommandField.NActionResponse, presentationID, messageID, message, status);
		}

		/// <summary>
		/// Sends an N-Delete Request.
		/// </summary>
		/// <param name="presentationID">The presentation ID.</param>
		/// <param name="messageID">The message ID.</param>
		/// <param name="message">The message.</param>
		public void SendNDeleteRequest(byte presentationID, ushort messageID, DicomMessage message)
		{
			message.CommandSet[DicomTags.MessageId].SetUInt16(0, messageID);
			message.CommandSet[DicomTags.CommandField].SetUInt16(0, (ushort) DicomCommandField.NDeleteRequest);

			if (message.DataSet == null || message.DataSet.IsEmpty())
				message.CommandSet[DicomTags.DataSetType].SetUInt16(0, 0x101);
			else
				message.CommandSet[DicomTags.DataSetType].SetUInt16(0, 0x102);

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Sends an N-Delete Response.
		/// </summary>
		/// <param name="presentationID">The presentation context ID to send the response message on.</param>
		/// <param name="messageID">The message ID of the request message being responded to.</param>
		/// <param name="message">The response message to send.</param>
		/// <param name="status">The status to send in the response message.</param>
		public void SendNDeleteResponse(byte presentationID, ushort messageID, DicomMessage message, DicomStatus status)
		{
			SendNCreateNSetNDeleteHelper(DicomCommandField.NDeleteResponse, presentationID, messageID, message, status);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Main processing routine for processing a network connection.
		/// </summary>
		private void RunRead()
		{
			try
			{
				ResetDimseTimeout();

				while (!_stop)
				{
					if (NetworkHasData())
					{
						ResetDimseTimeout();

						bool success = ProcessNextPDU();
						if (!success)
						{
							// Start the Abort process, not much else we can do
							Platform.Log(LogLevel.Error,
							             "Unexpected error processing PDU.  Aborting Association from {0} to {1}",
							             _assoc.CallingAE, _assoc.CalledAE);
							SendAssociateAbort(DicomAbortSource.ServiceProvider, DicomAbortReason.InvalidPDUParameter);
						}
					}
					else if (DateTime.Now > GetDimseTimeout())
					{
						string errorMessage;
						switch (State)
						{
							case DicomAssociationState.Sta6_AssociationEstablished:
								OnDimseTimeout();
								ResetDimseTimeout();
								break;
							case DicomAssociationState.Sta2_TransportConnectionOpen:
								errorMessage = "ARTIM timeout when waiting for AAssociate Request PDU, closing connection.";
								Platform.Log(LogLevel.Error, errorMessage);
								State = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
								OnNetworkError(new DicomNetworkException(errorMessage), true);
								if (NetworkClosed != null)
									NetworkClosed(errorMessage);
								break;
							case DicomAssociationState.Sta5_AwaitingAAssociationACOrReject:
								errorMessage = "ARTIM timeout when waiting for AAssociate AC or RJ PDU, closing connection.";
								Platform.Log(LogLevel.Error, errorMessage);
								State = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;
								OnNetworkError(new DicomNetworkException(errorMessage), true);
								if (NetworkClosed != null)
									NetworkClosed(errorMessage);
								break;
							case DicomAssociationState.Sta13_AwaitingTransportConnectionClose:
								errorMessage = string.Format(
									"Timeout when waiting for transport connection to close from {0} to {1}.  Dropping Connection.",
									_assoc.CallingAE, _assoc.CalledAE);
								Platform.Log(LogLevel.Error, errorMessage);
								OnNetworkError(new DicomNetworkException(errorMessage), true);
								if (NetworkClosed != null)
									NetworkClosed(errorMessage);
								break;
							default:
								Platform.Log(LogLevel.Error, "DIMSE timeout in unexpected state: {0}", State.ToString());
								OnDimseTimeout();
								ResetDimseTimeout();
								break;
						}
					}
					//else
					//{
					//    Thread.Sleep(0);
					//}
				}
				_network.Close();
				_network.Dispose();
				_network = null;
			}
			catch (Exception e)
			{
				OnNetworkError(e, true);

				if (NetworkError != null)
					NetworkError(e);
			}
		}

		/// <summary>
		/// Main processing routine for processing a network connection.
		/// </summary>
		private void RunWrite()
		{
			try
			{
				while (!_stop)
				{
					RawPDU rawPdu;
					if (_pduQueue.Dequeue(out rawPdu))
					{
						SendRawPDU(rawPdu);
					}
				}
			}
			catch (Exception e)
			{
				OnNetworkError(e, true);

				if (NetworkError != null)
					NetworkError(e);

				CloseNetwork(0);
			}
		}

		/// <summary>
		/// Main processing routine for processing a network connection.
		/// </summary>
		private void RunProcess()
		{
			try
			{
				while (!_stop)
				{
					NetworkProcessor processDelegate;
					if (_processingQueue.Dequeue(out processDelegate))
					{
						processDelegate();
					}
				}
			}
			catch (Exception e)
			{
				OnNetworkError(e, true);

				if (NetworkError != null)
					NetworkError(e);

				CloseNetwork(0);
			}
		}

		private bool ProcessRawPDU(RawPDU raw)
		{
			try
			{
				Platform.Log(LogLevel.Debug, "Received PDU: {0}", raw.ToString());
				switch (raw.Type)
				{
					case 0x01:
					{
						_assoc = new ServerAssociationParameters();
						var pdu = new AAssociateRQ(_assoc);
						pdu.Read(raw);
						State = DicomAssociationState.Sta3_AwaitingLocalAAssociationResponsePrimative;
						OnReceiveAssociateRequest(_assoc as ServerAssociationParameters);

						if (State != DicomAssociationState.Sta13_AwaitingTransportConnectionClose &&
						    State != DicomAssociationState.Sta6_AssociationEstablished)
						{
							Platform.Log(LogLevel.Error, "Association incorrectly not accepted or rejected, aborting.");
							return false;
						}

						//if derived class call SendAssociateAccept, it has fired this event
						//if (AssociationEstablished != null)
						//    AssociationEstablished(_assoc);

						return true;
					}
					case 0x02:
					{
						var pdu = new AAssociateAC(_assoc);
						pdu.Read(raw);
						State = DicomAssociationState.Sta6_AssociationEstablished;

						OnReceiveAssociateAccept(_assoc);

						if (AssociationEstablished != null)
							AssociationEstablished(_assoc);

						return true;
					}
					case 0x03:
					{
						var pdu = new AAssociateRJ();
						pdu.Read(raw);
						State = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;

						if (AssociationRejected != null)
							AssociationRejected(pdu.Source, pdu.Reason);

						OnReceiveAssociateReject(pdu.Result, pdu.Source, pdu.Reason);

						return true;
					}
					case 0x04:
					{
						var pdu = new PDataTFRead();
						pdu.Read(raw);
						return ProcessPDataTF(pdu);
					}
					case 0x05:
					{
						var pdu = new AReleaseRQ();
						pdu.Read(raw);
						State = DicomAssociationState.Sta8_AwaitingAReleaseRPLocalUser;

						OnReceiveReleaseRequest();

						return true;
					}
					case 0x06:
					{
						var pdu = new AReleaseRP();
						pdu.Read(raw);
						State = DicomAssociationState.Sta13_AwaitingTransportConnectionClose;

						if (AssociationReleased != null)
							AssociationReleased(_assoc);

						OnReceiveReleaseResponse();

						return true;
					}
					case 0x07:
					{
						var pdu = new AAbort();
						pdu.Read(raw);
						State = DicomAssociationState.Sta1_Idle;

						if (AssociationAborted != null)
							AssociationAborted(_assoc, pdu.Reason);

						OnReceiveAbort(pdu.Source, pdu.Reason);

						return true;
					}
					case 0xFF:
					{
						Platform.Log(LogLevel.Error, "Unexpected PDU type: 0xFF.  Potential parsing error.");
						return false;
					}
					default:
						throw new DicomNetworkException("Unknown PDU type");
				}
			}
			catch (Exception e)
			{
				OnNetworkError(e, true);

				if (NetworkError != null)
					NetworkError(e);

				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing PDU.");
				return false;
			}
		}

		private bool ProcessNextPDU()
		{
			var raw = new RawPDU(_network);

			raw.ReadPDU();

			if (_multiThreaded)
			{
				_processingQueue.Enqueue(delegate
				                         {
					                         if (raw.Type == 0x04)
					                         {
						                         if (_dimse == null)
						                         {
							                         _dimse = new DcmDimseInfo();
							                         _assoc.TotalDimseReceived++;
						                         }
					                         }

					                         if (!ProcessRawPDU(raw))
					                         {
						                         Platform.Log(LogLevel.Error,
						                                      "Unexpected error processing PDU.  Aborting Association from {0} to {1}",
						                                      _assoc.CallingAE, _assoc.CalledAE);
						                         SendAssociateAbort(DicomAbortSource.ServiceProvider,
						                                            DicomAbortReason.InvalidPDUParameter);
					                         }
				                         });
				return true;
			}

			if (raw.Type == 0x04)
			{
				if (_dimse == null)
				{
					_dimse = new DcmDimseInfo();
					_assoc.TotalDimseReceived++;
				}
			}

			return ProcessRawPDU(raw);
		}

		private bool ProcessPDataTF(PDataTFRead pdu)
		{
			try
			{
				byte pcid = 0;
				foreach (PDV pdv in pdu.PDVs)
				{
					pcid = pdv.PCID;
					if (pdv.IsCommand)
					{
						if (_dimse.CommandData == null)
							_dimse.CommandData = new ChunkStream();

						_dimse.CommandData.AddChunk(pdv.Value);

						if (_dimse.Command == null)
						{
							_dimse.Command = new DicomAttributeCollection(0x00000000, 0x0000FFFF);
						}

						if (_dimse.CommandReader == null)
						{
							_dimse.CommandReader = new DicomStreamReader(_dimse.CommandData)
							                       {
								                       TransferSyntax = TransferSyntax.ImplicitVrLittleEndian,
								                       Dataset = _dimse.Command
							                       };
						}

						DicomReadStatus stat =
							_dimse.CommandReader.Read(null, DicomReadOptions.UseDictionaryForExplicitUN);
						if (stat == DicomReadStatus.UnknownError)
						{
							Platform.Log(LogLevel.Error, "Unexpected parsing error when reading command group elements.");
							return false;
						}
						_assoc.TotalBytesRead += (UInt64) pdv.PDVLength - 6;
						if (DimseMessageReceiving != null)
							DimseMessageReceiving(_assoc, pcid);

						if (pdv.IsLastFragment)
						{
							if (stat == DicomReadStatus.NeedMoreData)
							{
								Platform.Log(LogLevel.Error,
								             "Unexpected end of StreamReader.  More data needed ({0} bytes, last tag read {1}) after reading last PDV fragment.",
								             _dimse.CommandReader.BytesNeeded, _dimse.CommandReader.LastTagRead.ToString());
								return false;
							}
							_dimse.CommandData = null;
							_dimse.CommandReader = null;

							bool isLast = true;
							if (_dimse.Command.Contains(DicomTags.DataSetType))
							{
								if (_dimse.Command[DicomTags.DataSetType].GetUInt16(0, 0x0) != 0x0101)
									isLast = false;
							}

							OnReceiveDimseCommand(pcid, _dimse.Command);

							if (isLast)
							{
								bool ret = OnReceiveDimse(pcid, _dimse.Command, _dimse.Dataset);
								if (!ret)
									Platform.Log(LogLevel.Error, "Error with OnReceiveDimse");

								LogSendReceive(true, _dimse.Command, _dimse.Dataset);

								//_assoc.TotalBytesRead += (UInt64)total;

								_dimse = null;
								return ret;
							}
						}
					}
					else
					{
						if (_dimse.DatasetData == null)
							_dimse.DatasetData = new ChunkStream();

						if (_dimse.Dataset == null)
							_dimse.Dataset = new DicomAttributeCollection(0x00040000, 0xFFFFFFFF);

						if (_dimse.DatasetReader == null)
						{
							_dimse.DatasetReader = new DicomStreamReader(_dimse.DatasetData)
							                       {
								                       TransferSyntax = _assoc.GetAcceptedTransferSyntax(pdv.PCID),
								                       Dataset = _dimse.Dataset
							                       };
						}

						if (!_dimse.DatasetReader.EncounteredStopTag)
						{
							_dimse.DatasetData.AddChunk(pdv.Value);

							_dimse.ParseDatasetStatus = _dimse.DatasetReader.Read(_dimse.DatasetStopTag, DicomReadOptions.UseDictionaryForExplicitUN);
							if (_dimse.ParseDatasetStatus == DicomReadStatus.UnknownError)
							{
								Platform.Log(LogLevel.Error, "Unexpected parsing error when reading DataSet.");
								return false;
							}
						}

						_assoc.TotalBytesRead += (UInt64) pdv.PDVLength - 6;
						if (DimseMessageReceiving != null)
							DimseMessageReceiving(_assoc, pcid);

						bool ret = true;
						if (_dimse.StreamMessage)
						{
							if (_dimse.IsNewDimse)
							{
								byte[] fileGroup2 = CreateFileHeader(pcid, _dimse.Command);
								ret = OnReceiveFileStream(pcid, _dimse.Command, _dimse.Dataset, fileGroup2, 0, fileGroup2.Length, _dimse.DatasetReader.EncounteredStopTag, _dimse.IsNewDimse, false);
								_dimse.IsNewDimse = false;
							}

							ret = ret && OnReceiveFileStream(pcid, _dimse.Command, _dimse.Dataset, pdv.Value.Array, pdv.Value.Index, pdv.Value.Count, _dimse.DatasetReader.EncounteredStopTag, false, pdv.IsLastFragment);
							if (!ret)
								Platform.Log(LogLevel.Error, "Error with OnReceiveFileStream");
						}

						if (pdv.IsLastFragment)
						{
							if (_dimse.ParseDatasetStatus == DicomReadStatus.NeedMoreData)
							{
								Platform.Log(LogLevel.Error,
								             "Unexpected end of StreamReader.  More data needed ({0} bytes, last tag read {1}) after reading last PDV fragment.",
								             _dimse.DatasetReader.BytesNeeded, _dimse.DatasetReader.LastTagRead.ToString());
								return false;
							}
							_dimse.CommandData = null;
							_dimse.CommandReader = null;

							LogSendReceive(true, _dimse.Command, _dimse.Dataset);

							if (!_dimse.StreamMessage)
							{
								ret = OnReceiveDimse(pcid, _dimse.Command, _dimse.Dataset);
								if (!ret)
									Platform.Log(LogLevel.Error, "Error with OnReceiveDimse");
							}
							else
							{
								if (MessageReceived != null)
									MessageReceived(_assoc, new DicomMessage(_dimse.Command, _dimse.Dataset));
							}

							_dimse = null;
							return ret;
						}

						if (!ret) return false;
					}
				}

				return true;
			}
			catch (Exception e)
			{
				//do something here!
				Platform.Log(LogLevel.Error, e, "Unexpected exception processing P-DATA PDU");
				return false;
			}
		}

		private byte[] CreateFileHeader(byte pcid, DicomAttributeCollection command)
		{
			var msg = new DicomMessage(command, new DicomAttributeCollection());
			var file = new DicomFile
			           {
				           MediaStorageSopClassUid = msg.AffectedSopClassUid,
				           MediaStorageSopInstanceUid = msg.AffectedSopInstanceUid,
				           ImplementationClassUid = DicomImplementation.ClassUID.UID,
				           ImplementationVersionName = DicomImplementation.Version,
				           SourceApplicationEntityTitle = _assoc.CallingAE,
				           TransferSyntax = _assoc.GetAcceptedTransferSyntax(pcid)
			           };

			var ms = new MemoryStream();
			file.Save(ms, DicomWriteOptions.Default);
			var byteArray = new byte[ms.Length];
			var sourceArray = ms.GetBuffer();
			Array.Copy(sourceArray, 0, byteArray, 0, ms.Length);
			return byteArray;
		}

		private void SendRawPDU(RawPDU pdu)
		{
			//ResetDimseTimeout();

			// If the try/catch is reintroduced here, it must
			// throw an exception, if the exception is just eaten, 
			// you can get into a case where there's repetetive errors
			// trying to send PDUs, until a whole message is sent.

			//try
			//{
			if (_network != null)
				pdu.WritePDU(_network);
			//}
			//catch (Exception e)
			//{
			//    OnNetworkError(e);
			//    throw new DicomException("Unexpected exception when writing PDU",e);
			//}

			ResetDimseTimeout();
		}

		/// <summary>
		/// Method for sending a DIMSE mesage.
		/// </summary>
		/// <param name="pcid"></param>
		/// <param name="command"></param>
		/// <param name="dataset"></param>
		private void SendDimse(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset)
		{
			try
			{
				TransferSyntax ts = _assoc.GetAcceptedTransferSyntax(pcid);

				PDataTFStream pdustream;
				if (_assoc.RemoteMaximumPduLength == 0 || _assoc.RemoteMaximumPduLength > _assoc.LocalMaximumPduLength)
					pdustream = new PDataTFStream(this, pcid, _assoc.LocalMaximumPduLength, NetworkSettings.Default.CombineCommandDataPdu);
				else
					pdustream = new PDataTFStream(this, pcid, _assoc.RemoteMaximumPduLength, NetworkSettings.Default.CombineCommandDataPdu);
				pdustream.OnTick += delegate
				                    {
					                    if (DimseMessageSending != null)
						                    DimseMessageSending(_assoc, pcid, command, dataset);
				                    };

				// Introduced lock as risk mitigation for ticket #10147.  Note that a more thorough locking
				// mechanism should be developed to work across PDU types, and also should take into account
				// if we do end up using _multiThreaded = true
				lock (_writeSyncLock)
				{
					LogSendReceive(false, command, dataset);

					var dsw = new DicomStreamWriter(pdustream);
					dsw.Write(TransferSyntax.ImplicitVrLittleEndian,
					          command, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

					if ((dataset != null) && !dataset.IsEmpty())
					{
						pdustream.IsCommand = false;
						dsw.Write(ts, dataset, DicomWriteOptions.Default);
					}

					// flush last pdu
					pdustream.Flush(true);
				}

				_assoc.TotalBytesSent += (ulong) pdustream.BytesWritten;

				OnDimseSent(pcid, command, dataset);
			}
			catch (Exception e)
			{
				OnNetworkError(e, true);

				// TODO
				// Should we throw another exception here?  Should the user know there's an error?  They'll get
				// the error reported to them through the OnNetworkError routine, and throwing an exception here
				// might cause us to call OnNetworkError a second time, because the exception may be caught at a higher
				// level
				// Note, when fixing defect #8184, realized that throwing an exception here would cause
				// failures in the ImageServer, because there are places where we wouldn't catch the 
				// exception.  Should be careful if this is ever introduced back in.
				//throw new DicomException("Unexpected exception when sending a DIMSE message",e);
			}
		}

		/// <summary>
		/// Method for sending a DIMSE mesage.
		/// </summary>
		/// <param name="pcid"></param>
		/// <param name="command"></param>
		/// <param name="dataset"></param>
		private void SendDimseDataSetStream(byte pcid, DicomAttributeCollection command, Stream dataset)
		{
			try
			{
				TransferSyntax ts = _assoc.GetAcceptedTransferSyntax(pcid);

				PDataTFStream pdustream;
				if (_assoc.RemoteMaximumPduLength == 0 || _assoc.RemoteMaximumPduLength > _assoc.LocalMaximumPduLength)
					pdustream = new PDataTFStream(this, pcid, _assoc.LocalMaximumPduLength, NetworkSettings.Default.CombineCommandDataPdu);
				else
					pdustream = new PDataTFStream(this, pcid, _assoc.RemoteMaximumPduLength, NetworkSettings.Default.CombineCommandDataPdu);
				pdustream.OnTick += delegate
				                    {
					                    if (DimseMessageSending != null)
						                    DimseMessageSending(_assoc, pcid, command, null);
				                    };

				// Introduced lock as risk mitigation for ticket #10147.  Note that a more thorough locking
				// mechanism should be developed to work across PDU types, and also should take into account
				// if we do end up using _multiThreaded = true
				lock (_writeSyncLock)
				{
					LogSendReceive(false, command, null);

					var dsw = new DicomStreamWriter(pdustream);
					dsw.Write(TransferSyntax.ImplicitVrLittleEndian,
					          command, DicomWriteOptions.Default | DicomWriteOptions.CalculateGroupLengths);

					if (dataset != null)
					{
						pdustream.IsCommand = false;

						pdustream.Write(dataset);
					}

					// flush last pdu
					pdustream.Flush(true);
				}

				_assoc.TotalBytesSent += (ulong) pdustream.BytesWritten;

				OnDimseSent(pcid, command, null);
			}
			catch (Exception e)
			{
				OnNetworkError(e, true);

				// TODO
				// Should we throw another exception here?  Should the user know there's an error?  They'll get
				// the error reported to them through the OnNetworkError routine, and throwing an exception here
				// might cause us to call OnNetworkError a second time, because the exception may be caught at a higher
				// level
				// Note, when fixing defect #8184, realized that throwing an exception here would cause
				// failures in the ImageServer, because there are places where we wouldn't catch the 
				// exception.  Should be careful if this is ever introduced back in.
				//throw new DicomException("Unexpected exception when sending a DIMSE message",e);
			}
		}

		/// <summary>
		/// Helper for sending N-Create, N-Set, and N-Delete Response messages.
		/// </summary>
		/// <param name="commandField">The type of message.</param>
		/// <param name="presentationId">The presentation context ID to send the message on.</param>
		/// <param name="messageId">The message ID to use for the message.</param>
		/// <param name="message">The actual message to send.</param>
		/// <param name="status">The response message.</param>
		private void SendNCreateNSetNDeleteHelper(DicomCommandField commandField, byte presentationId, ushort messageId, DicomMessage message, DicomStatus status)
		{
			message.CommandField = commandField;
			message.MessageIdBeingRespondedTo = messageId;
			message.AffectedSopClassUid = message.AffectedSopClassUid;

			if (message.DataSet == null || message.DataSet.IsEmpty())
				message.DataSetType = 0x0101;
			else
				message.DataSetType = 0x0102;
			message.Status = status;
			SendDimse(presentationId, message.CommandSet, message.DataSet);
		}

		/// <summary>
		/// Sends an N-Event-Report Response.
		/// </summary>
		/// <param name="presentationID">The presentation context ID</param>
		/// <param name="requestMessage">The message being responsed to.</param>
		/// <param name="message">The response message to send.</param>
		/// <param name="status">The status to send.</param>
		public void SendNEventReportResponse(byte presentationID, DicomMessage requestMessage, DicomMessage message, DicomStatus status)
		{
			message.CommandSet[DicomTags.CommandField].SetUInt16(0, (ushort) DicomCommandField.NEventReportResponse);
			message.CommandSet[DicomTags.MessageIdBeingRespondedTo].SetUInt16(0, requestMessage.MessageId);
			message.CommandSet[DicomTags.EventTypeId].SetUInt16(0, requestMessage.EventTypeId);
			message.CommandSet[DicomTags.AffectedSopClassUid].SetStringValue(requestMessage.AffectedSopClassUid);
			message.CommandSet[DicomTags.AffectedSopInstanceUid].SetStringValue(requestMessage.AffectedSopInstanceUid);
			message.CommandSet[DicomTags.Status].SetUInt16(0, status.Code);
			message.CommandSet[DicomTags.DataSetType].SetUInt16(0, 0x0101);

			SendDimse(presentationID, message.CommandSet, message.DataSet);
		}

		private static void LogSendReceive(bool receive, DicomAttributeCollection metaInfo, DicomAttributeCollection dataSet)
		{
			if (Platform.IsLogLevelEnabled(LogLevel.Debug))
			{
				string receiveOrSend = receive ? "Receive" : "Send";
				Platform.Log(LogLevel.Debug,
				             receiveOrSend + " MetaInfo:\r\n" + (metaInfo != null ? metaInfo.DumpString : String.Empty));
				Platform.Log(LogLevel.Debug, receiveOrSend + " DataSet:\r\n" + (dataSet != null ? dataSet.DumpString : String.Empty));
			}
		}

		#endregion
	}
}