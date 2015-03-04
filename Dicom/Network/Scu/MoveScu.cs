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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Iods;

namespace ClearCanvas.Dicom.Network.Scu
{

    /// <summary>
    /// Abstract class for Move SCU.  Please see <see cref="PatientRootMoveScu"/> and <see cref="StudyRootMoveScu"/>.
    /// </summary>
    public abstract class MoveScuBase : ScuBase 
    {
        #region Public Events/Delegates

        /// <summary>
		/// Event called when an image has completed being moved.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public event EventHandler<EventArgs> ImageMoveCompleted;

        /// <summary>
        /// Delegate for starting Move in ASynch mode with <see cref="BeginMove"/>.
        /// </summary>
        public delegate void MoveDelegate();

        #endregion

        #region Private Variables

        protected bool _cancelRequested = false;
        private ushort _moveMessageId = 0;

        private int _warningSubOperations = 0;
        private int _failureSubOperations = 0;
        private int _successSubOperations = 0;
        private int _totalSubOperations = 0;
        private int _remainingSubOperations = 0;
        private readonly DicomAttributeCollection _dicomAttributeCollection = new DicomAttributeCollection();
        string _destinationAe;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for Move SCU Component.
        /// </summary>
        /// <param name="localAe">The local AE title.</param>
        /// <param name="remoteAe">The remote AE title being connected to.</param>
        /// <param name="remoteHost">The hostname or IP address of the remote AE.</param>
        /// <param name="remotePort">The listen port of the remote AE.</param>
        /// <param name="destinationAe">The destination AE.</param>
        public MoveScuBase(string localAe, string remoteAe, string remoteHost, int remotePort, string destinationAe)
        {
            ClientAETitle = localAe;
            RemoteAE = remoteAe;
            RemoteHost = remoteHost;
            RemotePort = remotePort;
            _destinationAe = destinationAe;
        }

        #endregion

    	protected DicomAttributeCollection DicomAttributeCollection
    	{
			get { return _dicomAttributeCollection; }	
    	}

        #region Public Properties
        /// <summary>
        /// The number of tranferred SOP Instances which had a warning status.
        /// </summary>
        public int WarningSubOperations
        {
            get { return _warningSubOperations; }
        }

        /// <summary>
        /// The number of transferred SOP Instances that had a failure status.
        /// </summary>
        public int FailureSubOperations
        {
            get { return _failureSubOperations; }
        }

        /// <summary>
        /// The number of transferred SOP Instances that had a success status.
        /// </summary>
        public int SuccessSubOperations
        {
            get { return _successSubOperations; }
        }

        /// <summary>
        /// The total number of SOP Instances to transfer.
        /// </summary>
        public int TotalSubOperations
        {
            get { return _totalSubOperations; }
        }

        /// <summary>
        /// The number of remaining SOP Instances to transfer.
        /// </summary>
        public int RemainingSubOperations
        {
            get { return _remainingSubOperations; }
        }


        /// <summary>
        /// Gets or sets the destination ae.
        /// </summary>
        /// <value>The destination ae.</value>
        public string DestinationAe
        {
            get { return _destinationAe; }
        	set { _destinationAe = value; }
        }

		/// <summary>
		/// Gets or sets the MoveMessageId used for the C-MOVE-RQ
		/// </summary>
	    public ushort MoveMessageId
	    {
			get { return _moveMessageId; }
			set { _moveMessageId = value; }
	    }

    	public QueryRetrieveLevel CurrentLevel
    	{
    		get
    		{
				return IodBase.ParseEnum(_dicomAttributeCollection[DicomTags.QueryRetrieveLevel].GetString(0, String.Empty), QueryRetrieveLevel.None);
    		}
    	}


        /// <summary>
        /// Specifies the find sop class.
        /// </summary>
        /// <value>The find sop class.</value>
        /// <remarks>Abstract so subclass can specify.</remarks>
        public abstract SopClass MoveSopClass
        { get ; }
        #endregion

        #region Public Methods

        public override void Cancel()
        {
            if (LogInformation)
                Platform.Log(LogLevel.Info, "Canceling Scu connected from {0} to {1}:{2}:{3}...", ClientAETitle, RemoteAE,
                             RemoteHost, RemotePort);

            if (!_cancelRequested)
            {
                // Force a 10 second timeout.  This is mostly to cope with an ImageServer bug that
                // doesn't handle the C-CANCEL request properly.
                Client.AssociationParams.ReadTimeout = 10 * 1000;

                _cancelRequested = true;
                SendMoveCancelRequest(Client, AssociationParameters);
            }            
        }

        /// <summary>
        /// Move the SOP Instances
        /// </summary>
        public void Move()
        {
             _totalSubOperations = 0;
            _remainingSubOperations = 0;
            _failureSubOperations = 0;
            _successSubOperations = 0;
            _warningSubOperations = 0;

			if (LogInformation)
				Platform.Log(LogLevel.Info, "Preparing to connect to AE {0} on host {1} on port {2} for move request to {3}.",
				             RemoteAE, RemoteHost, RemotePort, _destinationAe);

            try
            {
                // the connect launches the actual send in a background thread.
                Connect();
            }
            catch (Exception e)
            {
				Platform.Log(LogLevel.Error,e,"Unexpected exception attempting to connect to {0}",RemoteAE);
                throw;
            }
        }

        /// <summary>
        /// Begins the move request in asynchronous mode.  
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="asyncState">State of the async.</param>
        /// <returns></returns>
        public IAsyncResult BeginMove(AsyncCallback callback, object asyncState)
        {
            MoveDelegate moveDelegate = this.Move;
            return moveDelegate.BeginInvoke(callback, asyncState);
        }

        /// <summary>
        /// Ends the move (asynchronous mode).  
        /// </summary>
        /// <param name="ar">The ar.</param>
        public void EndMove(IAsyncResult ar)
        {
            MoveDelegate moveDelegate = ((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate as MoveDelegate;

            if (moveDelegate != null)
            {
                moveDelegate.EndInvoke(ar);
            }
            else
            {
                throw new InvalidOperationException("cannot end invoke, asynchresult is null");
            }
        }

        /// <summary>
        /// Adds a study instance uid to the move request.
        /// </summary>
        /// <param name="studyInstanceUid">The study instance uid.</param>
        /// <exception cref="InvalidOperationException">If adding an instance of a different Query Level</exception>
		public virtual void AddStudyInstanceUid(string studyInstanceUid)
        {
			CheckForOtherLevel(QueryRetrieveLevel.Study);
            _dicomAttributeCollection[DicomTags.StudyInstanceUid].AppendString(studyInstanceUid);
        }

        /// <summary>
        /// Adds a patient id to the move request.
        /// </summary>
        /// <param name="patientId">The patient id.</param>
        /// <exception cref="InvalidOperationException">If adding an instance of a different Query Level</exception>
        public virtual void AddPatientId(string patientId)
        {
            CheckForOtherLevel(QueryRetrieveLevel.Patient);
            _dicomAttributeCollection[DicomTags.PatientId].AppendString(patientId);
        }

        /// <summary>
        /// Adds a series instance uid to the move request.
        /// </summary>
        /// <param name="seriesInstanceUid">The series instance uid.</param>
        /// <exception cref="InvalidOperationException">If adding an instance of a different Query Level</exception>
        public virtual void AddSeriesInstanceUid(string seriesInstanceUid)
        {
            if (_dicomAttributeCollection[DicomTags.StudyInstanceUid].Count != 1)
				throw new InvalidOperationException("Exactly one study uid must be specified.");

			CheckForOtherLevel(QueryRetrieveLevel.Series);
            _dicomAttributeCollection[DicomTags.SeriesInstanceUid].AppendString(seriesInstanceUid);
        }

        /// <summary>
        /// Adds a sop instance uid to the move request.
        /// </summary>
        /// <param name="sopInstanceUid">The sop instance uid.</param>
        /// <exception cref="InvalidOperationException">If adding an instance of a different Query Level</exception>
        public virtual void AddSopInstanceUid(string sopInstanceUid)
        {
			if (_dicomAttributeCollection[DicomTags.StudyInstanceUid].Count != 1)
				throw new InvalidOperationException("Exactly one study uid must be specified.");

			if (_dicomAttributeCollection[DicomTags.SeriesInstanceUid].Count != 1)
				throw new InvalidOperationException("Exactly one series uid must be specified.");

            CheckForOtherLevel(QueryRetrieveLevel.Image);
            _dicomAttributeCollection[DicomTags.SopInstanceUid].AppendString(sopInstanceUid);
        }
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Called when [image move completed].
        /// </summary>
        protected virtual void OnImageMoveCompleted()
        {
            EventHandler<EventArgs> tempHandler = ImageMoveCompleted;
            if (tempHandler != null)
                tempHandler(this, new EventArgs());
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Sends the move request (called after the association is accepted).
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="association">The association.</param>
        private void SendMoveRequest(DicomClient client, ClientAssociationParameters association)
        {
            byte pcid = association.FindAbstractSyntaxOrThrowException(MoveSopClass);

            var dicomMessage = new DicomMessage();
            foreach (DicomAttribute dicomAttribute in _dicomAttributeCollection)
            {
                // Need to do it this way in case the attribute is blank
                DicomAttribute dicomAttribute2 = dicomMessage.DataSet[dicomAttribute.Tag];
                if (dicomAttribute.Values != null)
                    dicomAttribute2.Values = dicomAttribute.Values;
            }

			if (_moveMessageId == 0)
				_moveMessageId = client.NextMessageID();

			client.SendCMoveRequest(pcid, _moveMessageId, _destinationAe, dicomMessage);
        }

        /// <summary>
        /// Sends the move request (called after the association is accepted).
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="association">The association.</param>
        private void SendMoveCancelRequest(DicomClient client, ClientAssociationParameters association)
        {
            byte pcid = association.FindAbstractSyntaxOrThrowException(MoveSopClass);
            client.SendCMoveCancelRequest(pcid, _moveMessageId);
        }

        /// <summary>
        /// Checks for other query retrieve levels already used, returns exception if trying to add a different level.
        /// Sets the QueryRetrieveLevel tag to the <paramref name="queryRetrieveLevel"/> if it's not invalid.
        /// </summary>
        /// <param name="queryRetrieveLevel">The query retrieve level.</param>
        /// <exception cref="InvalidOperationException">If adding an instance of a different Query Level</exception>
        private void CheckForOtherLevel(QueryRetrieveLevel queryRetrieveLevel)
        {
            QueryRetrieveLevel currentLevel = IodBase.ParseEnum(_dicomAttributeCollection[DicomTags.QueryRetrieveLevel].GetString(0, String.Empty), QueryRetrieveLevel.None);

			// Use the highest (most granular) query level (Image > Series > Study > Patient)
			if (queryRetrieveLevel > currentLevel)
			{
				IodBase.SetAttributeFromEnum(_dicomAttributeCollection[DicomTags.QueryRetrieveLevel], queryRetrieveLevel);
			}
			else if(queryRetrieveLevel != currentLevel)
			{
				string message = String.Format("The current query/retrieve level is '{0}' and cannot be set to {1}", currentLevel, queryRetrieveLevel);
				throw new InvalidOperationException(message);
			}
        }
        #endregion

        #region Protected Overridden Methods
        /// <summary>
        /// Scan the files to send, and create presentation contexts for each abstract syntax to send.
        /// </summary>
        protected override void SetPresentationContexts()
        {
            byte pcid = AssociationParameters.FindAbstractSyntax(MoveSopClass);
            if (pcid == 0)
            {
                pcid = AssociationParameters.AddPresentationContext(MoveSopClass);

                AssociationParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
                AssociationParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);
            }        
        }

        /// <summary>
        /// Called when received associate accept.  
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="association">The association.</param>
        public override void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association)
        {
            base.OnReceiveAssociateAccept(client, association);

            SendMoveRequest(client, association);
        }

        /// <summary>
        /// Called when received response message. 
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="association">The association.</param>
        /// <param name="presentationID">The presentation ID.</param>
        /// <param name="message">The message.</param>
		public override void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {
			// Discovered issue with an SCP that was not setting these values on the final Success return, so blocking an update of the values if all of them are 0.
			if (message.NumberOfFailedSubOperations != 0 
				|| message.NumberOfCompletedSubOperations != 0
				|| message.NumberOfRemainingSubOperations != 0
				|| message.NumberOfWarningSubOperations != 0)
        	{
        		_failureSubOperations = message.NumberOfFailedSubOperations;
        		_successSubOperations = message.NumberOfCompletedSubOperations;
        		_remainingSubOperations = message.NumberOfRemainingSubOperations;
        		_warningSubOperations = message.NumberOfWarningSubOperations;
        		_totalSubOperations = _failureSubOperations + _successSubOperations + _remainingSubOperations +
        		                      _warningSubOperations;
        	}

            if (!string.IsNullOrEmpty(message.ErrorComment))
                FailureDescription = message.ErrorComment;

        	if (message.Status.Status == DicomState.Pending)
        	{
        		OnImageMoveCompleted();
        	}
        	else
        	{
				DicomStatus status = DicomStatuses.LookupQueryRetrieve(message.Status.Code);
				if (message.Status.Status != DicomState.Success)
				{
					if (status.Status == DicomState.Cancel)
					{
                        if (LogInformation) Platform.Log(LogLevel.Info, "Cancel status received in Move Scu: {0}", status);
						Status = ScuOperationStatus.Canceled;
					}
                    else if (status.Status == DicomState.Failure)
					{
                        Platform.Log(LogLevel.Error, "Failure status received in Move Scu: {0}", status);
						Status = ScuOperationStatus.Failed;
						FailureDescription = status.ToString();
					}
                    else if (status.Status == DicomState.Warning)
					{
                        Platform.Log(LogLevel.Warn, "Warning status received in Move Scu: {0}", status);
					}
					else if (Status == ScuOperationStatus.Canceled)
					{
						if (LogInformation) Platform.Log(LogLevel.Info, "Client cancelled Move Scu operation.");
					}
				}
				else
				{
					if (LogInformation) Platform.Log(LogLevel.Info, "Success status received in Move Scu!");
				}

				client.SendReleaseRequest();
				StopRunningOperation();
			}
        }


		/// <summary>
		/// Called when a timeout occurs waiting for the next message, as specified by <see cref="AssociationParameters.ReadTimeout"/>.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="association">The association.</param>
		public override void OnDimseTimeout(DicomClient client, ClientAssociationParameters association)
		{
			Status = ScuOperationStatus.TimeoutExpired;
			FailureDescription =
				String.Format("Timeout Expired ({0} seconds) for remote AE {1} when processing C-MOVE-RQ, aborting connection", association.ReadTimeout/1000,
				              RemoteAE);
			Platform.Log(LogLevel.Error, FailureDescription);

			try
			{
				client.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Error aborting association");
			}

			Platform.Log(LogLevel.Warn, "Completed aborting connection (after DIMSE timeout) from {0} to {1}",
			             association.CallingAE, association.CalledAE);
			ProgressEvent.Set();
		}

    	#endregion
    }

    #region PatientRootFindScu Class
    /// <summary>
    /// Patient Root Move Scu
    /// <para>
    /// <example>
    /// MoveScuBase moveScu = new PatientRootMoveScu("myClientAeTitle", "myServerAe", "127.0.0.1", 5678, "destinationAE");
    /// moveScu.AddStudyInstanceUid("1.3.46.670589.5.2.10.2156913941.892665384.993397");
    /// moveScu.Move();
    /// </example>
    /// </para>
    /// </summary>
    public class PatientRootMoveScu : MoveScuBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PatientRootMoveScu"/> class.
        /// </summary>
        /// <param name="localAe">The local AE title.</param>
        /// <param name="remoteAe">The remote AE title being connected to.</param>
        /// <param name="remoteHost">The hostname or IP address of the remote AE.</param>
        /// <param name="remotePort">The listen port of the remote AE.</param>
        /// <param name="destinationAe">The destination AE.</param>
        public PatientRootMoveScu(string localAe, string remoteAe, string remoteHost, int remotePort, string destinationAe)
            : base(localAe, remoteAe, remoteHost, remotePort, destinationAe)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Specifies the move sop class (PatientRootQueryRetrieveInformationModelMove)
        /// </summary>
        /// <value>The find sop class.</value>
        /// <remarks>Abstract so subclass can specify.</remarks>
        public override SopClass MoveSopClass
        {
            get { return SopClass.PatientRootQueryRetrieveInformationModelMove; }
        }

		public override void AddStudyInstanceUid(string studyInstanceUid)
		{
			if (base.DicomAttributeCollection[DicomTags.PatientId].Count != 1)
				throw new InvalidOperationException("Exactly one patient ID must be specified.");

			base.AddStudyInstanceUid(studyInstanceUid);
		}

        #endregion
    }

    #endregion

    #region StudyRootMoveScu
    /// <summary>
    /// Patient Root Move Scu
    /// <para>
    /// <example>
    /// MoveScuBase moveScu = new StudyRootMoveScu("myClientAeTitle", "myServerAe", "127.0.0.1", 5678, "destinationAE");
    /// moveScu.AddStudyInstanceUid("1.3.46.670589.5.2.10.2156913941.892665384.993397");
    /// moveScu.Move();
    /// </example>
    /// </para>
    /// </summary>
    public class StudyRootMoveScu : MoveScuBase
    {

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StudyRootMoveScu"/> class.
        /// </summary>
        /// <param name="localAe">The local AE title.</param>
        /// <param name="remoteAe">The remote AE title being connected to.</param>
        /// <param name="remoteHost">The hostname or IP address of the remote AE.</param>
        /// <param name="remotePort">The listen port of the remote AE.</param>
        /// <param name="destinationAe">The destination AE.</param>
        public StudyRootMoveScu(string localAe, string remoteAe, string remoteHost, int remotePort, string destinationAe)
            : base(localAe, remoteAe, remoteHost, remotePort, destinationAe)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Specifies the move sop class (StudyRootQueryRetrieveInformationModelMove)
        /// </summary>
        /// <value>The find sop class.</value>
        /// <remarks>Abstract so subclass can specify.</remarks>
        public override SopClass MoveSopClass
        {
            get { return SopClass.StudyRootQueryRetrieveInformationModelMove; }
        }
        #endregion

		public override void AddPatientId(string patientId)
		{
			throw new InvalidOperationException("Cannot add patient ID to study root move scu.");
		}
    }

    #endregion

}
