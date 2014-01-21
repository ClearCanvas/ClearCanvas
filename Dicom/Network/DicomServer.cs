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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Network
{
    public delegate IDicomServerHandler StartAssociation(DicomServer server, ServerAssociationParameters assoc);
    /// <summary>
    /// Class used by DICOM server applications for network related activites.
    /// </summary>
    public sealed class DicomServer : NetworkBase, IDisposable
    {

        #region Static Public Methods
        /// <summary>
        /// Start listening for incoming associations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that StartListening can be called multiple times with different association parameters.
        /// </para>
        /// </remarks>
        /// <param name="parameters">The parameters to use when listening for associations.</param>
        /// <param name="acceptor">A delegate to be called to return a class instance that implements
        /// the <see cref="IDicomServerHandler"/> interface to handle an incoming association.</param>
        /// <returns><i>true</i> on success, <i>false</i> on failure</returns>
        public static bool StartListening(ServerAssociationParameters parameters, StartAssociation acceptor)
        {
            return Listener.Listen(parameters, acceptor);
        }

        /// <summary>
        /// Stop listening for incoming associations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that <see cref="StartListening"/> can be called multiple times with different association
        /// parameters.
        /// </para>
        /// </remarks>
        /// <param name="parameters">The parameters to stop listening on.</param>
        public static void StopListening(ServerAssociationParameters parameters)
        {
            Listener.StopListening(parameters);
        }
        #endregion

        #region Private Members
        private readonly string _host;
		private Socket _socket;
		private Stream _network;
		private ManualResetEvent _closedEvent;
		private bool _closedOnError;
        IDicomServerHandler _handler;
        private Dictionary<string, ListenerInfo> _appList;
        private bool _disposed;
		private IDicomFilestreamHandler _filestreamHandler;

		#endregion

        #region Public Properties
        /// <summary>
        /// Property that tells if an association was closed because of an error.
        /// </summary>
        public bool ClosedOnError
        {
            get { return _closedOnError; }
        }
        #endregion

        #region Constructors

        internal DicomServer(Socket socket, Dictionary<string,ListenerInfo> appList)
        {
            var remote = (IPEndPoint)socket.RemoteEndPoint;

            _host = remote.Address.ToString();

            _socket = socket;
            _network = new NetworkStream(_socket);
            _closedEvent = null;
            _handler = null;
            _appList = appList;

            // Start background thread for incoming associations
            InitializeNetwork(_network, "DicomServer: " + _host, false);
        }
		#endregion

        #region Private Methods
        private void SetSocketOptions(ServerAssociationParameters parameters)
        {
            _socket.ReceiveBufferSize = parameters.ReceiveBufferSize;
            _socket.SendBufferSize = parameters.SendBufferSize;
            _socket.ReceiveTimeout = parameters.ReadTimeout;
            _socket.SendTimeout = parameters.WriteTimeout;
            _socket.LingerState = new LingerOption(false, 0);
			// Nagle option
        	_socket.NoDelay = parameters.DisableNagle;
        }

        private static bool NegotiateAssociation(AssociationParameters cp, ServerAssociationParameters sp)
        {
            foreach (DicomPresContext clientContext in cp.GetPresentationContexts())
            {
                TransferSyntax selectedSyntax = null;
				foreach (DicomPresContext serverContext in sp.GetPresentationContexts())
				{
					if (clientContext.AbstractSyntax.Uid.Equals(serverContext.AbstractSyntax.Uid))
					{
						foreach (TransferSyntax ts in serverContext.GetTransfers())
						{
							if (clientContext.HasTransfer(ts))
							{
								selectedSyntax = ts;
								break;
							}
						}						
					}

					if (selectedSyntax != null)
						break;
				}

                if (selectedSyntax != null)
                {
                    clientContext.ClearTransfers();
                    clientContext.AddTransfer(selectedSyntax);
                    clientContext.SetResult(DicomPresContextResult.Accept);
                }
                else
                {
                    // No contexts accepted, set if abstract or transfer syntax reject
                    clientContext.SetResult(0 == sp.FindAbstractSyntax(clientContext.AbstractSyntax)
                                                ? DicomPresContextResult.RejectAbstractSyntaxNotSupported
                                                : DicomPresContextResult.RejectTransferSyntaxesNotSupported);
                }
            }
            bool anyValidContexts = false;

            foreach (DicomPresContext clientContext in cp.GetPresentationContexts())
            {
                if (clientContext.Result == DicomPresContextResult.Accept)
                {
                    anyValidContexts = true;
                    break;
                }
            }
            if (anyValidContexts == false)
            {
                return false;
            }      

            return true;
        }

        #endregion

        #region Public Methods

        #endregion

        #region NetworkBase Overrides
        /// <summary>
        /// Close the association.
        /// </summary>
		/// <param name="millisecondsTimeout">The timeout in milliseconds to wait for the closure
		/// of the network thread.</param>
		protected override void CloseNetwork(int millisecondsTimeout)
        {
			ShutdownNetworkThread(millisecondsTimeout);
			lock (this)
            {
                if (_network != null)
                {
                    _network.Close();
                    _network.Dispose();
                    _network = null;
                }
                if (_socket != null)
                {
                    if (_socket.Connected)
                        _socket.Close();
                    _socket = null;
                }
                if (_closedEvent != null)
                {
                    _closedEvent.Set();
                    _closedEvent = null;
                }
				State = DicomAssociationState.Sta1_Idle;
            }			
        }

        /// <summary>
        /// Used internally to determine if the connection has network data available.
        /// </summary>
        /// <returns></returns>
        protected override bool NetworkHasData()
        {
            if (_socket == null)
                return false;

            // Tells the state of the connection as of the last activity on the socket
            if (!_socket.Connected)
            {
				OnNetworkError(null, true);
                return false;
            }

            // This is the recommended way to determine if a socket is still active, make a
            // zero byte send call, and see if an exception is thrown.  See the Socket.Connected
            // MSDN documentation  Only do the check when we know there's no data available
            try
            {
				var readSockets = new List<Socket>
				                      {
				                          _socket
				                      };
                Socket.Select(readSockets, null, null, 100000);
				if (readSockets.Count == 1)
				{
					if (_socket.Available > 0)
						return true;
					OnNetworkError(null, true);
					return false;
				}

				_socket.Send(new byte[1], 0, 0);
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (!e.NativeErrorCode.Equals(10035))
					OnNetworkError(e, true);
            }

            return false;
        }

        /// <summary>
        /// Method called on a network error.
        /// </summary>
        /// <param name="e">The exception that caused the network error</param>
        /// <param name="closeConnection">Flag telling if the connection should be closed</param>
        protected override void OnNetworkError(Exception e, bool closeConnection)
        {
	        try
	        {
				if (_filestreamHandler != null)
				{
					_filestreamHandler.CancelStream();
					_filestreamHandler = null;
				}
	        }
	        catch (Exception x)
	        {
				Platform.Log(LogLevel.Error, x, "Unexpected exception when calling IDicomFilestreamHandler.CancelStream");
			}

            try
            {
                if (_handler != null && State != DicomAssociationState.Sta13_AwaitingTransportConnectionClose)
                    _handler.OnNetworkError(this, _assoc as ServerAssociationParameters, e);
            }
            catch (Exception x) 
            {
				Platform.Log(LogLevel.Error, x, "Unexpected exception when calling IDicomServerHandler.OnNetworkError");
            }

            _closedOnError = true;
            if (closeConnection)
				CloseNetwork(Timeout.Infinite);
        }

        /// <summary>
        /// Method called when receiving an association request.
        /// </summary>
        /// <param name="association"></param>
        protected override void OnReceiveAssociateRequest(ServerAssociationParameters association)
        {
        	ListenerInfo info;
            if (!_appList.TryGetValue(association.CalledAE, out info))
            {
				Platform.Log(LogLevel.Error, "Rejecting association from {0}: Invalid Called AE Title ({1}).", association.CallingAE, association.CalledAE);
                SendAssociateReject(DicomRejectResult.Permanent, DicomRejectSource.ServiceProviderACSE, DicomRejectReason.CalledAENotRecognized);
                return;
            }

            // Populate the AssociationParameters properly
            association.ReadTimeout = info.Parameters.ReadTimeout;
            association.ReceiveBufferSize = info.Parameters.ReceiveBufferSize;
            association.WriteTimeout = info.Parameters.WriteTimeout;
            association.SendBufferSize = info.Parameters.SendBufferSize;

            association.RemoteEndPoint = _socket.RemoteEndPoint as IPEndPoint;
            association.LocalEndPoint = _socket.LocalEndPoint as IPEndPoint;


            // Setup Socketoptions based on the user's settings
            SetSocketOptions(association);

            // Select the presentation contexts
            bool anyValidContexts = NegotiateAssociation(association, info.Parameters);
            if (!anyValidContexts)
            {
				Platform.Log(LogLevel.Error, "Rejecting association from {0}: No valid presentation contexts.", association.CallingAE);
                SendAssociateReject(DicomRejectResult.Permanent, DicomRejectSource.ServiceProviderACSE, DicomRejectReason.NoReasonGiven);
                return;
            }
            
            _appList = null;

            try
            {
                _handler = info.StartDelegate(this, association);
                _handler.OnReceiveAssociateRequest(this, association);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveAssociateRequest or StartDelegate");
            }
        }

        protected override void OnDimseTimeout()
        {
            try
            {
                _handler.OnDimseTimeout(this, _assoc as ServerAssociationParameters);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnDimseTimeout");
            }
        }

        protected override void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            try
            {
                _handler.OnReceiveAbort(this, _assoc as ServerAssociationParameters, source, reason);
            }
            catch (Exception e) 
            {
                OnUserException(e, "Unexpected exception OnReceiveAbort");
            }

            _closedOnError = true;
			CloseNetwork(Timeout.Infinite);
        }

        protected override void OnReceiveReleaseRequest()
        {
            try
            {
                _handler.OnReceiveReleaseRequest(this, _assoc as ServerAssociationParameters);
                
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveReleaseRequest");
                return;
            }
            SendReleaseResponse();
        }

        protected override void OnReceiveDimseRequest(byte pcid, DicomMessage msg)
        {
            try
            {
                _handler.OnReceiveRequestMessage(this, _assoc as ServerAssociationParameters, pcid, msg);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveRequestMessage");
            }
        }

        protected override void OnReceiveDimseResponse(byte pcid, DicomMessage msg)
        {
            try
            {
                _handler.OnReceiveResponseMessage(this, _assoc as ServerAssociationParameters, pcid, msg);
            }
            catch (Exception e)
            {
                OnUserException(e, "Unexpected exception on OnReceiveResponseMessage");
            }
        }

		protected override void OnReceiveDimseCommand(byte pcid, DicomAttributeCollection command)
		{
			try
			{
				_handler.OnReceiveDimseCommand(this, _assoc as ServerAssociationParameters, pcid, command);
			}
			catch (Exception e)
			{
				OnUserException(e, "Unexpected exception on OnReceiveDimseCommand");
			}
		}

		protected override bool OnReceiveFileStream(byte pcid, DicomAttributeCollection command, DicomAttributeCollection dataset, byte[] data, int offset, int count, bool isFirst, bool isLast)
		{
			try
			{
				if (isFirst)
					_filestreamHandler = _handler.OnStartFilestream(this, _assoc as ServerAssociationParameters, pcid, new DicomMessage(command, dataset));

				if (!_filestreamHandler.SaveStreamData(new DicomMessage(command, dataset), data, offset, count))
					return false;

				if (isLast)
				{
					_filestreamHandler.CompleteStream(this, _assoc as ServerAssociationParameters, pcid,
					                                  new DicomMessage(command, dataset));
					_filestreamHandler = null;
				}

				return true;
			}
			catch (Exception e)
			{
				OnUserException(e, "Unexpected exception on OnReceiveFileStream");
				return false;
			}
		}

        #endregion

        #region IDisposable Members
        ///
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// object is reclaimed by garbage collection.
        ///
        ~DicomServer()
        {
            Dispose(false);
        }

        ///
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ///
        /// Disposes the specified disposing.
        ///
        /// if set to true [disposing].
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // Dispose of other Managed objects, 
				// 2500 millisecond timeout
                Abort(2500);
            }

			try
			{
				if (_filestreamHandler != null)
				{
					_filestreamHandler.CancelStream();
					_filestreamHandler = null;
				}
			}
			catch (Exception x)
			{
				Platform.Log(LogLevel.Error, x, "Unexpected exception when calling IDicomFilestreamHandler.CancelStream");
			}

            // FREE UNMANAGED RESOURCES
            _disposed = true;
        }
        #endregion
    }
}
