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
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Network
{
	/// <summary>
	/// Class used by DICOM Clients for all network functionality.
	/// </summary>
	public sealed class DicomClient : NetworkBase, IDisposable
	{
		#region Private Members

		private IPEndPoint _remoteEndPoint;
		private int _timeout;
		private Socket _socket;
		private Stream _network;
		private ManualResetEvent _closedEvent;
		private bool _closedOnError;
		private readonly IDicomClientHandler _handler;
		private bool _disposed;

		#endregion

		#region Public Constructors

		private DicomClient(AssociationParameters assoc, IDicomClientHandler handler)
		{
			_remoteEndPoint = assoc.RemoteEndPoint;
			_socket = null;
			_network = null;
			_closedEvent = null;
			_timeout = 10;
			_handler = handler;
			_assoc = assoc;
		}

		#endregion

		#region Public Properties

		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}

		public Socket InternalSocket
		{
			get { return _socket; }
		}

		/// <summary>
		/// Flag telling if the connection was closed on an error.
		/// </summary>
		public bool ClosedOnError
		{
			get { return _closedOnError; }
		}

		#endregion

		#region Private Methods

		private void SetSocketOptions(ClientAssociationParameters parameters)
		{
			_socket.ReceiveBufferSize = parameters.ReceiveBufferSize;
			_socket.SendBufferSize = parameters.SendBufferSize;
			_socket.ReceiveTimeout = parameters.ReadTimeout;
			_socket.SendTimeout = parameters.WriteTimeout;
			_socket.LingerState = new LingerOption(false, 0);
			// Nagle option
			_socket.NoDelay = parameters.DisableNagle;
		}

		private void Connect(IPEndPoint ep)
		{
			_socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			SetSocketOptions(_assoc as ClientAssociationParameters);

			IAsyncResult result = _socket.BeginConnect(ep, null, null);

			bool success = result.AsyncWaitHandle.WaitOne(_assoc.ConnectTimeout, true);

			if (!success)
			{
				// NOTE, MUST CLOSE THE SOCKET
				_socket.Close();
				throw new DicomNetworkException(String.Format("Timeout while attempting to connect to remote server {0}", ep));
			}

			if (!_socket.Connected)
			{
				// NOTE, MUST CLOSE THE SOCKET
				_socket.Close();
				throw new DicomNetworkException(String.Format("Connection failed to remote server {0}", ep));
			}

			_network = new NetworkStream(_socket);

			InitializeNetwork(_network, "DicomClient: " + ep, false);

			_closedEvent = new ManualResetEvent(false);

			_remoteEndPoint = ep;

			_assoc.RemoteEndPoint = ep;
			_assoc.LocalEndPoint = _socket.LocalEndPoint as IPEndPoint;

			OnClientConnected();
		}

		private void Connect()
		{
			_closedOnError = false;

			if (_assoc.RemoteEndPoint != null)
			{
				Connect(_assoc.RemoteEndPoint);
			}
			else
			{
				IPHostEntry entry;

				try
				{
					entry = Dns.GetHostEntry(_assoc.RemoteHostname);
				}
				catch (SocketException x)
				{
					if (x.SocketErrorCode == SocketError.NoData || x.SocketErrorCode == SocketError.HostNotFound)
						throw new DicomException(SR.UnknownHost, x);
					throw;
				}

				IPAddress[] list = entry.AddressList;
				foreach (IPAddress dnsAddr in list)
				{
					if (dnsAddr.AddressFamily == AddressFamily.InterNetwork)
					{
						try
						{
							Connect(new IPEndPoint(dnsAddr, _assoc.RemotePort));
							return;
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Unable to connect to remote host, attempting other addresses: {0}",
							             dnsAddr.ToString());
						}
					}
				}
				foreach (IPAddress dnsAddr in list)
				{
					if (dnsAddr.AddressFamily == AddressFamily.InterNetworkV6)
					{
						try
						{
							Connect(new IPEndPoint(dnsAddr, _assoc.RemotePort));
							return;
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Unable to connection to remote host, attempting other addresses: {0}",
							             dnsAddr.ToString());
						}
					}
				}
				String message = String.Format("Unable to connect to: {0}:{1}, no valid addresses to connect to", _assoc.RemoteHostname, _assoc.RemotePort);

				Platform.Log(LogLevel.Error, message);
				throw new DicomException(message);
			}
		}

		private void ConnectTLS()
		{
			_closedOnError = false;

			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			SetSocketOptions(_assoc as ClientAssociationParameters);

			_socket.Connect(_remoteEndPoint);

			_network = new SslStream(new NetworkStream(_socket));

			InitializeNetwork(_network, "TLS Client handler to: " + _remoteEndPoint);

			_closedEvent = new ManualResetEvent(false);

			OnClientConnected();
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Connection to a remote DICOM application.
		/// </summary>
		/// <param name="assoc"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static DicomClient Connect(AssociationParameters assoc, IDicomClientHandler handler)
		{
			DicomClient client = new DicomClient(assoc, handler);
			client.Connect();
			return client;
		}

		/// <summary>
		/// Connection to a remote DICOM application via TLS.
		/// </summary>
		/// <param name="assoc"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static DicomClient ConnectTLS(AssociationParameters assoc, IDicomClientHandler handler)
		{
			DicomClient client = new DicomClient(assoc, handler);
			client.ConnectTLS();
			return client;
		}

		/// <summary>
		/// Wait for the background thread for the client to close.
		/// </summary>
		public void Join()
		{
			_closedEvent.WaitOne();
		}

		/// <summary>
		/// Wait a specified timeout for the background thread for the client to close.
		/// </summary>
		/// <returns>
		/// True if the background thread has exited.
		/// </returns>
		/// <param name="timeout"></param>
		public bool Join(TimeSpan timeout)
		{
			return _closedEvent.WaitOne(timeout, true);
		}

		#endregion

		#region NetworkBase Overrides

		/// <summary>
		/// Close the DICOM connection.
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
				}
				State = DicomAssociationState.Sta1_Idle;
			}
		}

		private void OnClientConnected()
		{
			if (LogInformation) Platform.Log(LogLevel.Debug, "{0} SCU -> Network Connected: {2} {1}", _assoc.CallingAE, InternalSocket.RemoteEndPoint.ToString(), _assoc.CalledAE);

			SendAssociateRequest(_assoc);
		}

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
				List<Socket> readSockets = new List<Socket>();
				readSockets.Add(_socket);
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

		protected override void OnNetworkError(Exception e, bool closeConnection)
		{
			try
			{
				_handler.OnNetworkError(this, _assoc as ClientAssociationParameters, e);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Unexpected exception when calling IDicomClientHandler.OnNetworkError");
			}

			_closedOnError = true;
			if (closeConnection)
				CloseNetwork(System.Threading.Timeout.Infinite);
		}

		protected override void OnDimseTimeout()
		{
			try
			{
				_handler.OnDimseTimeout(this, _assoc as ClientAssociationParameters);
			}
			catch (Exception e)
			{
				OnUserException(e, "Unexpected exception on OnDimseTimeout");
			}
		}

		protected override void OnReceiveAssociateAccept(AssociationParameters association)
		{
			try
			{
				_handler.OnReceiveAssociateAccept(this, association as ClientAssociationParameters);
			}
			catch (Exception e)
			{
				OnUserException(e, "Unexpected exception on OnReceiveAssociateAccept");
			}
		}

		protected override void OnReceiveAssociateReject(DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
		{
			_handler.OnReceiveAssociateReject(this, _assoc as ClientAssociationParameters, result, source, reason);

			_closedOnError = true;
			CloseNetwork(System.Threading.Timeout.Infinite);
		}

		protected override void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
		{
			try
			{
				_handler.OnReceiveAbort(this, _assoc as ClientAssociationParameters, source, reason);
			}
			catch (Exception e)
			{
				OnUserException(e, "Unexpected exception on OnReceiveAbort");
			}
			_closedOnError = true;
			CloseNetwork(System.Threading.Timeout.Infinite);
		}

		protected override void OnReceiveReleaseResponse()
		{
			try
			{
				_handler.OnReceiveReleaseResponse(this, _assoc as ClientAssociationParameters);
			}
			catch (Exception e)
			{
				OnUserException(e, "Unexpected exception on OnReceiveReleaseResponse");
			}
			_closedOnError = false;
			CloseNetwork(System.Threading.Timeout.Infinite);
		}

		protected override void OnReceiveDimseRequest(byte pcid, DicomMessage msg)
		{
			try
			{
				_handler.OnReceiveRequestMessage(this, _assoc as ClientAssociationParameters, pcid, msg);
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
				_handler.OnReceiveResponseMessage(this, _assoc as ClientAssociationParameters, pcid, msg);
			}
			catch (Exception e)
			{
				OnUserException(e, "Unexpected exception on OnReceiveResponseMessage");
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
			try
			{
				// 2500 millisecond timeout
				Abort(2500);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown during dispose");
			}
		}

		#endregion
	}
}