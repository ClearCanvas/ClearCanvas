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
using System.Net;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Network
{
    /// <summary>
    /// Enumerated value representing the various DICOM Role selections that can be negotiated. 
    /// </summary>
    public enum DicomRoleSelection {
		Disabled,
		SCU,
		SCP,
		Both,
		None
	}

    /// <summary>
    /// Enumerated value that represents the various DICOM presentation context status values.
    /// </summary>
	public enum DicomPresContextResult : byte {
		Proposed = 255,
		Accept = 0,
		RejectUser = 1,
		RejectNoReason = 2,
		RejectAbstractSyntaxNotSupported = 3,
		RejectTransferSyntaxesNotSupported = 4
	}

    /// <summary>
    /// Internal representation of a presentation context.
    /// </summary>
	public class DicomPresContext {
		#region Private Members
		private readonly byte _pcid;
		private DicomPresContextResult _result;
		private DicomRoleSelection _roles;
		private readonly SopClass _abstract;
		private List<TransferSyntax> _transfers;
		#endregion

		#region Public Constructor
		public DicomPresContext(byte pcid, SopClass abstractSyntax) {
			_pcid = pcid;
			_result = DicomPresContextResult.Proposed;
			_roles = DicomRoleSelection.Disabled;
			_abstract = abstractSyntax;
            if (abstractSyntax.Uid.Length == 0)
                throw new DicomException("Invalid abstract syntax for presentation context, UID is zero length.");
			_transfers = new List<TransferSyntax>();
		}

		internal DicomPresContext(byte pcid, SopClass abstractSyntax, TransferSyntax transferSyntax, DicomPresContextResult result) {
			_pcid = pcid;
			_result = result;
			_roles = DicomRoleSelection.Disabled;
			_abstract = abstractSyntax;
            if (abstractSyntax.Uid.Length == 0)
                throw new DicomException("Invalid abstract syntax for presentation context, UID is zero length.");
            _transfers = new List<TransferSyntax>();
			_transfers.Add(transferSyntax);
		}
		#endregion

		#region Public Properties
		public byte ID {
			get { return _pcid; }
		}

		public DicomPresContextResult Result {
			get { return _result; }
		}

		public bool IsRoleSelect {
			get { return _roles != DicomRoleSelection.Disabled; }
		}
		public bool IsSupportScuRole {
			get { return _roles == DicomRoleSelection.SCU || _roles == DicomRoleSelection.Both; }
		}
		public bool IsSupportScpRole {
			get { return _roles == DicomRoleSelection.SCP || _roles == DicomRoleSelection.Both; }
		}

		public SopClass AbstractSyntax {
			get { return _abstract; }
		}

		public TransferSyntax AcceptedTransferSyntax {
			get {
                if (Result != DicomPresContextResult.Accept)
                    return null;
				if (_transfers.Count > 0)
					return _transfers[0];
				return null;
			}
		}
		#endregion

		#region Public Members
		public void SetResult(DicomPresContextResult result) {
			_result = result;
		}

		public void SetRoleSelect(DicomRoleSelection roles) {
			_roles = roles;
		}
		public DicomRoleSelection GetRoleSelect() {
			return _roles;
		}

		public void AddTransfer(TransferSyntax ts) {
			if (!_transfers.Contains(ts))
				_transfers.Add(ts);
		}

		public void RemoveTransfer(TransferSyntax ts) {
			if (_transfers.Contains(ts))
				_transfers.Remove(ts);
		}

		public void ClearTransfers() {
			_transfers.Clear();
		}

		public IList<TransferSyntax> GetTransfers() {
			return _transfers.AsReadOnly();
		}

		public void SortTransfers(Comparison<TransferSyntax> comparison)
		{
			_transfers.Sort(comparison);
		}

		public bool HasTransfer(TransferSyntax ts)
		{
			if (_result == DicomPresContextResult.Accept || _result == DicomPresContextResult.Proposed)
                return _transfers.Contains(ts);
			return false;
		}

    	public string GetResultDescription() {
			switch (_result) {
			case DicomPresContextResult.Accept:
				return "Accept";
			case DicomPresContextResult.Proposed:
				return "Proposed";
			case DicomPresContextResult.RejectAbstractSyntaxNotSupported:
				return "Reject - Abstract Syntax Not Supported";
			case DicomPresContextResult.RejectNoReason:
				return "Reject - No Reason";
			case DicomPresContextResult.RejectTransferSyntaxesNotSupported:
				return "Reject - Transfer Syntaxes Not Supported";
			case DicomPresContextResult.RejectUser:
				return "Reject - User";
			default:
				return "Unknown";
			}
		}
		#endregion
	}

    /// <summary>
    /// Class used to represent parameters used to negotiate an association.
    /// </summary>
    public abstract class AssociationParameters : EventArgs
    {
        
        #region Private Members

        private DateTime _timeStamp = DateTime.Now;
        // Setting the value so a PDU (including the PDU header) fit into 
        // a multiple of the TCP/IP Maximum Segment Size of 1460 will help 
        // increase performance.  The PDU header is 6 bytes, and should 
        // be subtracted from the multiple of 1460 to get the PDU size.
        // For instance (1460 * 80) - 6 = 116,794 bytes
        private SortedList<byte, DicomPresContext> _presContexts;

        // Sizes that result in PDUs that are multiples of the MTU work better.
        // Setting these values to an even multiple of the TCP/IP maximum
        // segement size of 1460 can help increase performance.  81 * 1460 is the default

        // Performance stuff

        #endregion

		#region Constructors
		protected AssociationParameters(String callingAE, String calledAE, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint) {
		    DisableNagle = NetworkSettings.Default.DisableNagle;
		    ConnectTimeout = NetworkSettings.Default.ConnectTimeout;
		    WriteTimeout = NetworkSettings.Default.WriteTimeout;
		    ReadTimeout = NetworkSettings.Default.ReadTimeout;
		    ReceiveBufferSize = NetworkSettings.Default.ReceiveBufferSize;
		    SendBufferSize = NetworkSettings.Default.SendBufferSize;
		    RemoteMaximumPduLength = NetworkSettings.Default.RemoteMaxPduLength;
		    LocalMaximumPduLength = NetworkSettings.Default.LocalMaxPduLength;
		    RemoteMaxOperationsPerformed = 1;
		    RemoteMaxOperationsInvoked = 1;
		    LocalMaxOperationsInvoked = 1;
		    LocalMaxOperationsPerformed = 1;
		    TotalBytesSent = 0;
		    ApplicationContextName = DicomUids.DICOMApplicationContextName;
			ImplementationClass = DicomImplementation.ClassUID;
			ImplementationVersion = DicomImplementation.Version;
			_presContexts = new SortedList<byte, DicomPresContext>();

            CalledAE = calledAE;
            CallingAE = callingAE;

            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;

            TotalBytesRead = 0;
            TotalDimseReceived = 0;
		}

        protected AssociationParameters(AssociationParameters parameters)
        {
            DisableNagle = NetworkSettings.Default.DisableNagle;
            RemoteMaxOperationsPerformed = 1;
            RemoteMaxOperationsInvoked = 1;
            TotalDimseReceived = 0;
            TotalBytesSent = 0;
            TotalBytesRead = 0;
            ApplicationContextName = parameters.ApplicationContextName;
            CalledAE = parameters.CalledAE;
            CallingAE = parameters.CallingAE;
            ImplementationClass = parameters.ImplementationClass;
            ImplementationVersion = parameters.ImplementationVersion;
            LocalEndPoint = parameters.LocalEndPoint;
            LocalMaximumPduLength = parameters.LocalMaximumPduLength;
            RemoteMaximumPduLength = parameters.RemoteMaximumPduLength;
            ReadTimeout = parameters.ReadTimeout;
            ReceiveBufferSize = parameters.ReceiveBufferSize;
            RemoteEndPoint = parameters.RemoteEndPoint;
            SendBufferSize = parameters.SendBufferSize;
            WriteTimeout = parameters.WriteTimeout;
            ConnectTimeout = parameters.ConnectTimeout;

            foreach (byte id in parameters._presContexts.Keys)
            {
                AddPresentationContext(id,parameters._presContexts[id].AbstractSyntax);

                foreach (TransferSyntax ts in parameters._presContexts[id].GetTransfers())
                {
                    AddTransferSyntax(id,ts);
                }

                SetRoleSelect(id, parameters._presContexts[id].GetRoleSelect());

            }
        }
		#endregion

		#region Public Properties

        public ulong TotalBytesRead { get; set; }

        public ulong TotalBytesSent { get; set; }

        public int TotalDimseReceived { get; set; }

        /// <summary>
        /// The Maximum operations invoked negotiated for the association.
        /// </summary>
        public ushort RemoteMaxOperationsInvoked { get; set; }

        /// <summary>
        /// The Maximum operations performed negotiated for the association.
        /// </summary>
        public ushort RemoteMaxOperationsPerformed { get; set; }

        /// <summary>
        /// The Maximum operations invoked negotiated for the association.
        /// </summary>
        public ushort LocalMaxOperationsInvoked { get; set; }

        /// <summary>
        /// The Maximum operations performed negotiated for the association.
        /// </summary>
        public ushort LocalMaxOperationsPerformed { get; set; }

        /// <summary>
        /// The Maximum PDU Length negotiated for the association
        /// </summary>
        public uint LocalMaximumPduLength { get; set; }

        /// <summary>
        /// The Remote Maximum PDU Length negotiated for the association
        /// </summary>
        public uint RemoteMaximumPduLength { get; set; }

        /// <summary>
        /// The network Send Buffer size utilized by this application.
        /// </summary>
        public int SendBufferSize { get; set; }

        /// <summary>
        /// The network Receive Buffer size utilized by this application.
        /// </summary>
        public int ReceiveBufferSize { get; set; }

        /// <summary>
        /// The timeout for any network Read operations in milliseconds.
        /// </summary>
        public int ReadTimeout { get; set; }

        /// <summary>
        /// The timeout for any network write operations in milliseconds.
        /// </summary>
        public int WriteTimeout { get; set; }

        /// <summary>
        /// The timeout when connecting to a remote server in milliseconds.
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// Flag to set if the Nagle algorithm is disabled for connections
        /// </summary>
        public bool DisableNagle { get; set; }

        /// <summary>
        /// Called AE (association acceptor AE) for the association
        /// </summary>
        public string CalledAE { get; set; }

        /// <summary>
        /// Calling AE (association requestor AE) for the association
        /// </summary>
        public string CallingAE { get; set; }

        /// <summary>
        /// Gets or sets the Application Context Name.
        /// </summary>
        /// <seealso cref="DicomUid"/>
        public DicomUid ApplicationContextName { get; set; }

        /// <summary>
        /// Gets or sets the Implementation Class UID.
        /// </summary>
        public DicomUid ImplementationClass { get; set; }

        /// <summary>
        /// Gets or sets the Implementation Version Name.
        /// </summary>
        public string ImplementationVersion { get; set; }

        /// <summary>
        /// The remote end point for the association.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; internal set; }

        /// <summary>
        /// The local end point of the association.
        /// </summary>
        /// 
        public IPEndPoint LocalEndPoint { get; internal set; }

        /// <summary>
        /// Remote hostname or IP addresses.
        /// </summary>
        public string RemoteHostname { get; internal set; }

        /// <summary>
        /// Remote port.
        /// </summary>
        public int RemotePort { get; internal set; }

        #endregion

        #region Events

        #endregion

        #region Internal Properties
        internal SortedList<byte, DicomPresContext> PresentationContexts
        {
            get { return _presContexts; }
            set { _presContexts = value; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
        }

        #endregion

        #region Public Methods
        /// <summary>
		/// Adds a Presentation Context to the DICOM Associate.
		/// </summary>
		public void AddPresentationContext(byte pcid, SopClass abstractSyntax) {
			_presContexts.Add(pcid, new DicomPresContext(pcid, abstractSyntax));
		}

		/// <summary>
		/// Adds a Presentation Context to the DICOM Associate.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Note, this method will create a new presentation context for the
		/// <see cref="SopClass"/> even if one already exists for the 
		/// <see cref="SopClass"/>. 
		/// </para>
		/// </remarks>
		public byte AddPresentationContext(SopClass abstractSyntax) {
			byte pcid = 1;
			foreach (byte id in _presContexts.Keys) {
				//if (_presContexts[id].AbstractSyntax == abstractSyntax)
				//	return id;
				if (id >= pcid)
					pcid = (byte)(id + 2);
			}
			AddPresentationContext(pcid, abstractSyntax);
			return pcid;
		}

		/// <summary>
		/// Determines if the specified Presentation Context ID exists.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>True if exists.</returns>
		public bool HasPresentationContextID(byte pcid) {
			return _presContexts.ContainsKey(pcid);
		}

		/// <summary>
		/// Gets a list of the Presentation Context IDs in the DICOM Associate.
		/// </summary>
		public IList<byte> GetPresentationContextIDs() {
			return _presContexts.Keys;
		}

        /// <summary>
        /// Gets a list of the <see cref="TransferSyntax"/>es specified for a Presentation
        /// Context.
        /// </summary>
        /// <param name="pcid">Presentation Context ID</param>
        /// <returns>A list of <see cref="TransferSyntax"/>es.</returns>
        public IList<TransferSyntax> GetPresentationContextTransferSyntaxes(byte pcid)
        {
            return GetPresentationContext(pcid).GetTransfers();
        }

		/// <summary>
		/// Sets the result of the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="result">Result</param>
		public void SetPresentationContextResult(byte pcid, DicomPresContextResult result) {
			GetPresentationContext(pcid).SetResult(result);
		}

		/// <summary>
		/// Gets the result of the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Result</returns>
		public DicomPresContextResult GetPresentationContextResult(byte pcid) {
			return GetPresentationContext(pcid).Result;
		}

		/// <summary>
		/// Adds a Transfer Syntax to the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="ts">Transfer Syntax</param>
		public void AddTransferSyntax(byte pcid, TransferSyntax ts) {
			GetPresentationContext(pcid).AddTransfer(ts);
		}

		/// <summary>
		/// Gets the number of Transfer Syntaxes in the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Number of Transfer Syntaxes</returns>
		public int GetTransferSyntaxCount(byte pcid) {
			return GetPresentationContext(pcid).GetTransfers().Count;
		}

		/// <summary>
		/// Gets the Transfer Syntax at the specified index.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="index">Index of Transfer Syntax</param>
		/// <returns>Transfer Syntax</returns>
		public TransferSyntax GetTransferSyntax(byte pcid, int index) {
			return GetPresentationContext(pcid).GetTransfers()[index];
		}

		/// <summary>
		/// Removes a Transfer Syntax from the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="ts">Transfer Syntax</param>
		public void RemoveTransferSyntax(byte pcid, TransferSyntax ts) {
			GetPresentationContext(pcid).RemoveTransfer(ts);
		}

		/// <summary>
		/// Gets the Abstract Syntax for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Abstract Syntax</returns>
		public DicomUid GetAbstractSyntax(byte pcid) {
            SopClass sop = GetPresentationContext(pcid).AbstractSyntax;
			return sop.DicomUid;
		}

		/// <summary>
		/// Gets the accepted Transfer Syntax for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <returns>Transfer Syntax</returns>
		public TransferSyntax GetAcceptedTransferSyntax(byte pcid) {
			return GetPresentationContext(pcid).AcceptedTransferSyntax;
		}

		/// <summary>
		/// Finds the Presentation Context with the specified Abstract Syntax, or 0 if it can't find it.
		/// </summary>
		/// <param name="abstractSyntax">Abstract Syntax</param>
		/// <returns>Presentation Context ID, or 0 if it can't find it.</returns>
		public byte FindAbstractSyntax(SopClass abstractSyntax) {
			foreach (DicomPresContext ctx in _presContexts.Values) {
				if (ctx.AbstractSyntax.Uid == abstractSyntax.Uid)
					return ctx.ID;
			}
			return 0;
		}

        /// <summary>
        /// Finds the Presentation Context with the specified Abstract Syntax.  If it can't find it, throws an <see cref="DicomException"/>.
        /// It is useful to throw an exception for for a Scu, so we don't have to keep checking for a valid pcid.
        /// </summary>
        /// <param name="abstractSyntax">Abstract Syntax</param>
        /// <returns>Presentation Context ID</returns>
        /// <exception cref="DicomException"/>
        public byte FindAbstractSyntaxOrThrowException(SopClass abstractSyntax)
        {
            foreach (DicomPresContext ctx in _presContexts.Values)
            {
                if (ctx.AbstractSyntax.Uid == abstractSyntax.Uid)
                    return ctx.ID;
            }
            throw new DicomException("Cannot find abstract syntax in presentation context: " + abstractSyntax.ToString());
        }

		/// <summary>
		/// Finds the Presentation Context with the specified Abstract Syntax and Transfer Syntax.
		/// </summary>
		/// <param name="abstractSyntax">Abstract Syntax</param>
		/// <param name="transferSyntax">Transfer Syntax</param>
		/// <returns>Presentation Context ID</returns>
		public byte FindAbstractSyntaxWithTransferSyntax(SopClass abstractSyntax, TransferSyntax transferSyntax) {
			foreach (DicomPresContext ctx in _presContexts.Values) {
				if (ctx.AbstractSyntax.Uid == abstractSyntax.Uid && ctx.HasTransfer(transferSyntax))
					return ctx.ID;
			}
			return 0;
		}

		/// <summary>
		/// Determines if Role Selection is enabled for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		public bool IsRoleSelect(byte pcid) {
			return GetPresentationContext(pcid).IsRoleSelect;
		}

		/// <summary>
		/// Determines whether the User Role is supported for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		public bool IsSupportScuRole(byte pcid) {
			return GetPresentationContext(pcid).IsSupportScuRole;
		}

		/// <summary>
		/// Determines whether the Provider Role is supported for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		public bool IsSupportScpRole(byte pcid) {
			return GetPresentationContext(pcid).IsSupportScpRole;
		}

		/// <summary>
		/// Enables or disables Role Selection. It also sets the User Role and Provider Role, if enabled, for the specified Presentation Context.
		/// </summary>
		/// <param name="pcid">Presentation Context ID</param>
		/// <param name="roles">Supported Roles</param>
		public void SetRoleSelect(byte pcid, DicomRoleSelection roles) {
			GetPresentationContext(pcid).SetRoleSelect(roles);
		}
		#endregion

		#region Internal Methods

        internal void SetAcceptedTransferSyntax(byte pcid, int index)
        {
            TransferSyntax ts = GetPresentationContext(pcid).GetTransfers()[index];
            GetPresentationContext(pcid).ClearTransfers();
            GetPresentationContext(pcid).AddTransfer(ts);
        }

        internal void SetAcceptedTransferSyntax(byte pcid, TransferSyntax ts)
        {
            GetPresentationContext(pcid).ClearTransfers();
            GetPresentationContext(pcid).AddTransfer(ts);
        }

		internal void AddPresentationContext(byte pcid, DicomUid abstractSyntax, TransferSyntax transferSyntax, DicomPresContextResult result) {
			_presContexts.Add(pcid, new DicomPresContext(pcid, SopClass.GetSopClass(abstractSyntax.UID), transferSyntax, result));
		}

		public DicomPresContext GetPresentationContext(byte pcid) {
			DicomPresContext ctx;
			if (!_presContexts.TryGetValue(pcid, out ctx))
				throw new DicomNetworkException("Invalid Presentaion Context ID");
			return ctx;
		}

		internal IList<DicomPresContext> GetPresentationContexts() {
			return _presContexts.Values;
		}
		#endregion


        public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Application Context:     {0}", ApplicationContextName);
            sb.AppendLine();
			sb.AppendFormat("Implementation Class:    {0}", ImplementationClass);
            sb.AppendLine(); 
            sb.AppendFormat("Implementation Version:  {0}", ImplementationVersion);
            sb.AppendLine(); 
            sb.AppendFormat("Local Maximum PDU Size:  {0}", LocalMaximumPduLength);
            sb.AppendLine();
            sb.AppendFormat("Remote Maximum PDU Size: {0}", RemoteMaximumPduLength);
            sb.AppendLine();
            sb.AppendFormat("Called AE Title:         {0}", CalledAE);
            sb.AppendLine(); 
            sb.AppendFormat("Calling AE Title:        {0}", CallingAE);
            sb.AppendLine(); 
            sb.AppendFormat("Presentation Contexts:   {0}", _presContexts.Count);
            sb.AppendLine(); 
            foreach (DicomPresContext pctx in _presContexts.Values)
            {
				if (pctx.Result == DicomPresContextResult.Accept)
				{
					TransferSyntax ts = pctx.GetTransfers()[0];
					sb.AppendFormat("	Presentation Context {0} [{1}] Abstract: {2}	Transfer: {3}", pctx.ID, pctx.GetResultDescription(),
									pctx.AbstractSyntax.Name, ts.Name);
					sb.AppendLine();
				}
				else
				{
					sb.AppendFormat("	Presentation Context {0} [{1}] Abstract: {2}", pctx.ID, pctx.GetResultDescription(),
					                pctx.AbstractSyntax.Name);
					sb.AppendLine();
					foreach (TransferSyntax ts in pctx.GetTransfers())
					{
						sb.AppendFormat("		Transfer: {0}", (ts.DicomUid.Type == UidType.Unknown)
						                                   	?
						                                   		ts.DicomUid.UID
						                                   	: ts.DicomUid.Description);
						sb.AppendLine();
					}
				}
            }
			return sb.ToString();
		}
    }

    /// <summary>
    /// Association parameters structure used for client connections.
    /// </summary>
    public class ClientAssociationParameters : AssociationParameters
    {
        public ClientAssociationParameters(String callingAe, String calledAe, string hostname, int port)
               : base(callingAe, calledAe, null, null)
        {
            IPAddress addr;

            RemotePort = port;
            RemoteHostname = hostname;

            if (IPAddress.TryParse(hostname, out addr))
            {
                RemoteEndPoint = new IPEndPoint(addr, port);
            }
        }

        public ClientAssociationParameters(String callingAE, String calledAE, IPEndPoint remoteEndPoint)
            : base(callingAE,calledAE,null,remoteEndPoint)
        {
            RemotePort = remoteEndPoint.Port;
            RemoteHostname = remoteEndPoint.Address.ToString();
		}

        private ClientAssociationParameters(ClientAssociationParameters parameters)
            : base(parameters)
        {
        }

        internal ClientAssociationParameters Copy(ClientAssociationParameters sourceParameters)
        {
            return new ClientAssociationParameters(sourceParameters);
        }
    }

    /// <summary>
    /// Association parameters structure used for server connections.
    /// </summary>
    public class ServerAssociationParameters : AssociationParameters
    {
        internal ServerAssociationParameters()
            : base(null, null, null, null)
        {
        }

        public ServerAssociationParameters(String calledAe, IPEndPoint localEndPoint )
			: base(null, calledAe, localEndPoint, null)
        {
        }

    }
}
