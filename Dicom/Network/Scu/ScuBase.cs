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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Network.Scu
{
	// TODO CR (23 Jan 2014): Make this class abstract, if in fact the class could never be used without overriding OnReceiveResponse

	/// <summary>
	/// This is the base class for SCU classes.
	/// </summary>
	/// <remarks>The three main methods that shouold be overwritten are <see cref="SetPresentationContexts"/>,
	/// <see cref="OnReceiveAssociateAccept"/>, and <see cref="OnReceiveAssociateAccept"/>.
	/// <para>Note, built into this class, so all Scu classes can use the <see cref="Timeout"/> property and
	/// the <see cref="Cancel"/> method.
	/// </para>
	/// </remarks>
	public class ScuBase : IDicomClientHandler, IDisposable
	{
		#region Private Variables...

		private string _clientAETitle;
		protected ClientAssociationParameters _associationParameters = null;
		protected DicomClient _dicomClient = null;
		private readonly AutoResetEvent _progressEvent = new AutoResetEvent(false);
		private string _remoteAE;
		private string _remoteHost;
		private int _remotePort;
		private ScuOperationStatus _status;
		private int _readTimeout = NetworkSettings.Default.ReadTimeout;
		private int _writeTimeout = NetworkSettings.Default.WriteTimeout;
		private string _failureDescription = "";
		private DicomState _resultStatus;
		private bool _logInformation = true;

		#endregion

		#region Protected Properties...

		/// <summary>
		/// Gets the progress event.
		/// </summary>
		/// <value>The progress event.</value>
		protected AutoResetEvent ProgressEvent
		{
			get { return _progressEvent; }
		}

		#region AssociationParameters

		/// <summary>
		/// Gets or sets the association parameters.
		/// </summary>
		/// <value>The association parameters.</value>
		protected ClientAssociationParameters AssociationParameters
		{
			get { return _associationParameters; }
			set { _associationParameters = value; }
		}

		#endregion

		#region Client

		/// <summary>
		/// Gets or sets the Dicom Client.
		/// </summary>
		/// <value>The client.</value>
		protected DicomClient Client
		{
			get { return _dicomClient; }
			set { _dicomClient = value; }
		}

		#endregion

		#region ClientAETitle

		/// <summary>
		/// Gets or sets the client AE title.
		/// </summary>
		/// <value>The client AE title.</value>
		protected string ClientAETitle
		{
			get { return _clientAETitle; }
			set { _clientAETitle = value; }
		}

		#endregion

		#region RemoteAE

		/// <summary>
		/// Gets or sets the remote AE.
		/// </summary>
		/// <value>The remote AE.</value>
		protected string RemoteAE
		{
			get { return _remoteAE; }
			set { _remoteAE = value; }
		}

		#endregion

		#region RemoteHost

		/// <summary>
		/// Gets or sets the remote host.
		/// </summary>
		/// <value>The remote host.</value>
		protected string RemoteHost
		{
			get { return _remoteHost; }
			set { _remoteHost = value; }
		}

		#endregion

		#region RemotePort

		/// <summary>
		/// Gets or sets the remote port.
		/// </summary>
		/// <value>The remote port.</value>
		protected int RemotePort
		{
			get { return _remotePort; }
			set { _remotePort = value; }
		}

		#endregion

		#region FailureDescription

		public string FailureDescription
		{
			get { return _failureDescription; }
			set { _failureDescription = value; }
		}

		#endregion

		#region Status

		/// <summary>
		/// Gets or sets the operation status.
		/// </summary>
		/// <value>The status.</value>
		public ScuOperationStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		#endregion

		#region Timeout

		/// <summary>
		/// Gets or sets the read timeout.
		/// </summary>
		/// <remarks>Set by default to the value configured as <see cref="NetworkSettings.Default.ReadTimeout"/></remarks>
		/// <value>The Read timeout in milliseconds.  (Default: 30000)</value>
		public int ReadTimeout
		{
			get { return _readTimeout; }
			set { _readTimeout = value; }
		}

		/// <summary>
		/// Gets or sets the write timeout.
		/// </summary>
		/// <remarks>Set by default to the value configured as <see cref="NetworkSettings.Default.WriteTimeout"/></remarks>
		/// <value>The Write timeout in milliseconds.  (Default: 30000)</value>
		public int WriteTimeout
		{
			get { return _writeTimeout; }
			set { _writeTimeout = value; }
		}

		#endregion

		#region LogInformation

		/// <summary>
		/// Sets if informational logging should be done by the Scu.  
		/// </summary>
		/// <value>Boolean, <c>true</c> if informational logging should be done, <c>
		/// false</c> otherwise.  (Default: true)</value>
		public bool LogInformation
		{
			get { return _logInformation; }
			set
			{
				_logInformation = value;
				if (_dicomClient != null)
					_dicomClient.LogInformation = LogInformation;
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets Canceled - ie, rerturns true if the <see cref="Status"/> Property equals Canceled.
		/// </summary>
		/// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
		public bool Canceled
		{
			get { return Status == ScuOperationStatus.Canceled; }
		}

		/// <summary>
		/// Gets the result status.
		/// </summary>
		/// <value>The result status.</value>
		public DicomState ResultStatus
		{
			get { return _resultStatus; }
			protected set { _resultStatus = value; }
		}

		#endregion

		#region Public Events/Delegates...

		/// <summary>
		/// Occurs when the association has been accepted.
		/// </summary>
		public event EventHandler<AssociationParameters> AssociationAccepted;

		/// <summary>
		/// Occurs when the associated has been rejected by the remote AE
		/// </summary>
		public event EventHandler<AssociationParameters> AssociationRejected;

		/// <summary>
		/// Occurs when the associated has been released
		/// </summary>
		public event EventHandler<AssociationParameters> AssociationReleased;

		/// <summary>
		/// Occurs when the associated has been aborted
		/// </summary>
		public event EventHandler<AssociationParameters> AssociationAborted;

		/// <summary>
		/// Occurs when a network error occured
		/// </summary>
		public event EventHandler<AssociationParameters> NetworkError;

		#endregion

		#region Public Methods...

		/// <summary>
		/// Cancels the operation.
		/// </summary>
		public virtual void Cancel()
		{
			if (LogInformation)
				Platform.Log(LogLevel.Info, "Canceling Scu connected from {0} to {1}:{2}:{3}...", ClientAETitle, RemoteAE,
				             RemoteHost, RemotePort);
			Status = ScuOperationStatus.Canceled;
			ProgressEvent.Set();
		}

		/// <summary>
		/// Wait for the background thread for the client to close. 
		/// </summary>
		public void Join()
		{
			if (Client != null)
			{
				Client.Join();
				Client = null;
			}
		}

		/// <summary>
		/// Wait for the background thread for the client to close. 
		/// </summary>
		/// <returns>
		/// <value>true</value> if the thread as exited, <value>false</value> if timeout.
		/// </returns>
		public bool Join(TimeSpan timeout)
		{
			if (Client != null)
			{
				bool returnVal = Client.Join(timeout);
				Client = null;
				return returnVal;
			}
			return true;
		}

		/// <summary>
		/// Convert a collection of DICOM attribute collections into the specificed list of Iods.
		/// </summary>
		/// <typeparam name="T">An <see cref="IodBase"/> derived class.</typeparam>
		/// <param name="collectionList">The list of DICOM attribute collections to convert</param>
		/// <returns>The list of IODs.</returns>
		public static IList<T> GetResultsAsIod<T>(IList<DicomAttributeCollection> collectionList) where T : IodBase, new()
		{
			IList<T> iodList = new List<T>();

			foreach (DicomAttributeCollection collection in collectionList)
			{
				T iod = new T();
				iod.DicomAttributeProvider = collection;
				iodList.Add(iod);
			}
			return iodList;
		}

		public void Abort()
		{
			if (_dicomClient != null)
			{
				// Force a 2.5 second timeout
				_dicomClient.Abort(2500);
				_dicomClient.Dispose();
				_dicomClient = null;
				Status = ScuOperationStatus.NetworkError;
				ProgressEvent.Set();
			}
		}

		#endregion

		#region Protected Methods...

		/// <summary>
		/// Connects to specified server with the specified information.
		/// </summary>
		/// <remarks>Note this calls <see cref="SetPresentationContexts"/> to get the list of presentation
		/// contexts for the association request, so this method should be overwritten in the subclass.</remarks>
		protected void Connect()
		{
			try
			{
				if (_dicomClient != null)
				{
					// TODO: Dispose of properly...
					CloseDicomClient();
				}

				if (String.IsNullOrEmpty(ClientAETitle) || String.IsNullOrEmpty(RemoteAE) ||
				    String.IsNullOrEmpty(RemoteHost) || RemotePort == 0)
					throw new InvalidOperationException("Client and/or Remote info not specified.");

				AssociationParameters =
					new ClientAssociationParameters(ClientAETitle, RemoteAE, RemoteHost, RemotePort);

				AssociationParameters.ReadTimeout = ReadTimeout;
				AssociationParameters.WriteTimeout = WriteTimeout;

				SetPresentationContexts();

				Status = ScuOperationStatus.Running;
				_dicomClient = DicomClient.Connect(AssociationParameters, this);
				_dicomClient.LogInformation = LogInformation;
				ProgressEvent.WaitOne();
			}
			catch (Exception ex)
			{
				Status = ScuOperationStatus.ConnectFailed;
				FailureDescription = string.Format("Error connecting to {0}: {1}", RemoteAE, ex.Message);
				Platform.Log(LogLevel.Error, ex, "Exception attempting connection to RemoteHost {0} ({1}:{2})", RemoteAE, RemoteHost, RemotePort);
				throw;
			}
		}

		protected void Connect(string clientAETitle, string remoteAE, string remoteHost, int remotePort)
		{
			if (LogInformation) Platform.Log(LogLevel.Info, "Preparing to connect to AE {0} on host {1} on port {2}", remoteAE, remoteHost, remotePort);
			try
			{
				ClientAETitle = clientAETitle;
				RemoteAE = remoteAE;
				RemoteHost = remoteHost;
				RemotePort = remotePort;
				Connect();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception trying to connect to Remote AE {0} on host {1} on port {2}", remoteAE, remoteHost, remotePort);
				throw;
			}
		}

		/// <summary>
		/// Checks for canceled.  Throws a <see cref="OperationCanceledException"/> if the operation is canceled.
		/// </summary>
		/// <exception cref="OperationCanceledException"/>
		protected void CheckForCanceled()
		{
			if (Status == ScuOperationStatus.Canceled)
				throw new OperationCanceledException();
		}

		/// <summary>
		/// Checks for timeout expired.  Throws a <see cref="TimeoutException"/> if timeout has expired.
		/// </summary>
		/// <exception cref="TimeoutException"/>
		protected void CheckForTimeoutExpired()
		{
			if (Status == ScuOperationStatus.TimeoutExpired)
				throw new TimeoutException();
		}

		/// <summary>
		/// Stops the running operation, by setting the Status to NotRunning, stopping the timer, and setting
		/// the Progress Wait Event so execution can continue.
		/// </summary>
		protected void StopRunningOperation()
		{
			//If it's anything else, then the client code may want to inspect the value.
			if (Status == ScuOperationStatus.Running)
				Status = ScuOperationStatus.NotRunning;

			ProgressEvent.Set();
		}

		protected void StopRunningOperation(ScuOperationStatus status)
		{
			Status = status;
			ProgressEvent.Set();
		}

		/// <summary>
		/// Adds the sop class to presentation context for Explicit and Implicit Vr Little Endian
		/// </summary>
		/// <param name="sopClass">The sop class.</param>
		/// <exception cref="DicomException"/>
		/// <exception cref="ArgumentNullException"/>
		protected void AddSopClassToPresentationContext(SopClass sopClass)
		{
			if (sopClass == null)
				throw new ArgumentNullException("sopClass");

			byte pcid = AssociationParameters.FindAbstractSyntax(sopClass);
			if (pcid == 0)
			{
				pcid = AssociationParameters.AddPresentationContext(sopClass);

				AssociationParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
				AssociationParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);
			}
			else
			{
				throw new DicomException("Cannot find SopClass in association parameters: " + sopClass);
			}
		}

		/// <summary>
		/// Releases the connection.
		/// </summary>
		/// <param name="client">The client.</param>
		protected void ReleaseConnection(DicomClient client)
		{
			if (client != null)
				client.SendReleaseRequest();
			StopRunningOperation();
		}

		#endregion

		#region Protected Abstracts/Virtual Methods...

		/// <summary>
		/// Sets the presentation contexts that the association will attempt to connect on.
		/// Note, this must be implemented in the subclass.
		/// </summary>
		protected virtual void SetPresentationContexts()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Private Methods...

		/// <summary>
		/// This (forcefully?) Closes the dicom client.
		/// </summary>
		private void CloseDicomClient()
		{
			try
			{
				_dicomClient.Dispose();
				_dicomClient = null;
			}
			catch (Exception) {}
		}

		#endregion

		#region IDicomClientHandler Members

		/// <summary>
		/// Called when received associate accept.  In this event we should then send the specific request 
		/// we wish to do (ie, CStore, CEcho, etc.).
		/// </summary>
		/// <remarks>Note, this should be overridden in subclasses.  It is also recommended for the overridden
		/// method to first call this to log the associate accept information:
		/// <code>
		/// public override void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association)
		/// {
		///     base.OnReceiveAssociateAccept(client, association);
		///     // SEND Request like CStore or CEcho...
		/// }
		/// </code>
		/// </remarks>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		public virtual void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association)
		{
			if (LogInformation)
				Platform.Log(LogLevel.Info, "Association Accepted from {0} to remote AE {1}:{2}", association.CallingAE,
				             association.CalledAE, association.RemoteEndPoint.ToString());

			EventHandler<AssociationParameters> tempHandler = AssociationAccepted;
			if (tempHandler != null)
				tempHandler(this, association);
		}

		/// <summary>
		/// Called when received response message.  This should be overridden by the subclass.
		/// </summary>
		/// <remarks>This is where we receive the message back in a CEcho or CFind, and for a CStore, we
		/// would send additional files if necessary.  Note, the subclass should *not* call this method.  Perhaps
		/// this is comfusing with <see cref="OnReceiveAssociateAccept"/> recommended to call the base class.?
		/// <para>Note, the overridden method in the subclass should call <see cref="StopRunningOperation"/> 
		/// when operation is completed.</para>
		/// </remarks>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		/// <param name="presentationID">The presentation ID.</param>
		/// <param name="message">The message.</param>
		public virtual void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Called when [receive associate reject].
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		/// <param name="result">The result.</param>
		/// <param name="source">The source.</param>
		/// <param name="reason">The reason.</param>
		public void OnReceiveAssociateReject(DicomClient client, ClientAssociationParameters association, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
		{
			FailureDescription =
				String.Format("Association Rejection when {0} connected to remote AE {1}:{2}", association.CallingAE,
				              association.CalledAE, association.RemoteEndPoint);
			Platform.Log(LogLevel.Warn, FailureDescription);
			try
			{
				StopRunningOperation(ScuOperationStatus.AssociationRejected);
			}
			finally
			{
				if (AssociationRejected!=null)
					EventsHelper.Fire(AssociationRejected, this, association);
			}
		}

		/// <summary>
		/// Called when [receive request message].
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		/// <param name="presentationID">The presentation ID.</param>
		/// <param name="message">The message.</param>
		public virtual void OnReceiveRequestMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
		{
			Platform.Log(LogLevel.Error, "Unexpected OnReceiveRequestMessage callback on client.");
			try
			{
				client.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Error aborting association");
			}
			StopRunningOperation(ScuOperationStatus.UnexpectedMessage);
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Called when [receive release response].
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		public void OnReceiveReleaseResponse(DicomClient client, ClientAssociationParameters association)
		{
			if (LogInformation) Platform.Log(LogLevel.Info, "Association released from {0} to {1}", association.CallingAE, association.CalledAE);
			try
			{
				StopRunningOperation();
			}
			finally
			{
				if (AssociationReleased != null)
					EventsHelper.Fire(this.AssociationReleased, this, association);
			}
		}


		/// <summary>
		/// Called when [receive abort].
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		/// <param name="source">The source.</param>
		/// <param name="reason">The reason.</param>
		public void OnReceiveAbort(DicomClient client, ClientAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
		{
			FailureDescription = String.Format("Unexpected association abort received from {0} to {1}", association.CallingAE, association.CalledAE);
			Platform.Log(LogLevel.Warn, FailureDescription);
			try
			{
				StopRunningOperation(ScuOperationStatus.UnexpectedMessage);
			}
			finally
			{
				if (AssociationAborted!=null)
				{
					EventsHelper.Fire(AssociationAborted, this, association);
				}
			}
		}

		/// <summary>
		/// Called when [network error].
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		/// <param name="e">The e.  Note, e can be null in some instances.</param>
		public void OnNetworkError(DicomClient client, ClientAssociationParameters association, Exception e)
		{
			//TODO: right now this method gets called in timeout and abort situations.  Should add
			// the appropriate methods to the IDicomClientHandler to address this.

			//We don't want to blow away other failure descriptions (e.g. the OnDimseTimeout one).
			if (Status == ScuOperationStatus.Running)
				FailureDescription = String.Format("Unexpected network error: {0}", e == null ? "Unknown" : e.Message);

			if (client.State == DicomAssociationState.Sta13_AwaitingTransportConnectionClose)
			{
				//When this state is set and an error occurs, an appropriate message has already been logged in the client.
			}
			else
			{
				Platform.Log(LogLevel.Warn, FailureDescription);
			}

			//We don't want to blow away the OnDimseTimeout 'TimeoutExpired' status.
			ScuOperationStatus stopStatus = Status;
			if (stopStatus == ScuOperationStatus.Running)
				stopStatus = ScuOperationStatus.NetworkError;

			ResultStatus = DicomState.Failure;

			try
			{
				StopRunningOperation(stopStatus);
			}
			finally
			{
				if (NetworkError!=null)
				{
					EventsHelper.Fire(NetworkError, this, association);
				}
			}
		}

		/// <summary>
		/// Called when [dimse timeout].
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		public virtual void OnDimseTimeout(DicomClient client, ClientAssociationParameters association)
		{
			Status = ScuOperationStatus.TimeoutExpired;
			ResultStatus = DicomState.Failure;

			FailureDescription = String.Format("Timeout Expired ({0} seconds) for remote AE {1}, aborting connection", association.ReadTimeout/1000,
			                                   RemoteAE);
			if (LogInformation) Platform.Log(LogLevel.Info, FailureDescription);

			try
			{
				client.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Error aborting association");
			}

			Platform.Log(LogLevel.Warn, "Completed aborting connection (after DIMSE timeout) from {0} to {1}", association.CallingAE, association.CalledAE);
			ProgressEvent.Set();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ScuBase"/> is reclaimed by garbage collection.
		/// </summary>
		~ScuBase()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown during finalizer");
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown during dispose");
			}
		}

		private bool _disposed = false;

		protected bool IsDisposed
		{
			get { return _disposed; }
		}

		/// <summary>
		/// Called to perform tasks associated with freeing, releasing, or resetting managed and unmanaged resources.
		/// </summary>
		/// <param name="disposing">Indicates whether or not the method is being called by <see cref="Dispose"/> or the object finalizer.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				// Dispose of other Managed objects, ie
				CloseDicomClient();
			}

			// FREE UNMANAGED RESOURCES
			_disposed = true;
		}

		#endregion

	}

	/// <summary>
	/// 
	/// </summary>
	public enum ScuOperationStatus
	{
		/// <summary>
		/// 
		/// </summary>
		NotRunning = 0,

		/// <summary>
		/// 
		/// </summary>
		Running = 1,

		/// <summary>
		/// Dimse timeout expired.
		/// </summary>
		TimeoutExpired = 2,

		/// <summary>
		/// The scu operation was cancelled.
		/// </summary>
		Canceled = 3,

		///<summary>
		/// A failure occured
		/// </summary>
		Failed = 4,

		///<summary>
		/// A connection failure occured
		/// </summary>
		ConnectFailed = 5,

		/// <summary>
		/// The association was rejected.
		/// </summary>
		AssociationRejected = 6,

		/// <summary>
		/// An unexpected network error occurred.
		/// </summary>
		NetworkError = 7,

		/// <summary>
		/// An unexpected message was received and the association was aborted.
		/// </summary>
		UnexpectedMessage = 8
	}
}