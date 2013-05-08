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

using System.Collections.Generic;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.Network.Scp
{
    /// <summary>
    /// Base class implementing a DICOM SCP.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class uses the ClearCanvas.Dicom assembly to implement a DICOM SCP.  
    /// It handles most of the basic interactions with the DICOM library.
    /// </para>
    /// <para>
    /// The class depends on an <see cref="ExtensionPoint"/> for handling action DICOM 
    /// services.  The class will load plugins that implement the <see cref="IDicomScp{TContext}"/> interface.
    /// It will query these plugins to determine what DICOM Servies they support, and then 
    /// construct a list of transfer syntaxes and DICOM services supported based on the plugins.
    /// </para>
    /// <para>
    /// When a request message arrives, the appropriate plugin will be called to process the
    /// incoming message.  Note that different plugins can support the same DICOM service, but 
    /// different transfer syntaxes.
    /// </para>
    /// </remarks>
    public class DicomScp<TContext>
    {
        /// <summary>
        /// Delegate called to verify if an association should be accepted or rejected.
        /// </summary>
        /// <remarks>
        /// If assigned in the constructor to <see cref="DicomScp{TContext}"/>, this delegate is called by <see cref="DicomScp{TContext}"/>
        /// to check if an association should be rejected or accepted.  
        /// </remarks>
        /// <param name="context">User parameters passed to the constructor to <see cref="DicomScp{TContext}"/></param>
        /// <param name="assocParms">Parameters for the association.</param>
        /// <param name="result">If the delegate returns false, the DICOM reject result is returned here.</param>
        /// <param name="reason">If the delegate returns false, the DICOM reject reason is returned here.</param>
        /// <returns>true if the association should be accepted, false if rejected.</returns>
        public delegate bool AssociationVerifyCallback(TContext context, ServerAssociationParameters assocParms, out DicomRejectResult result, out DicomRejectReason reason);

		/// <summary>
		/// Delegate called after an association is complete with a list of Storage images transferred.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="assocParams"></param>
		/// <param name="instances"></param>
    	public delegate void AssociationComplete(
    		TContext context, ServerAssociationParameters assocParams, List<StorageInstance> instances);
        #region Constructors
        /// <summary>
        /// Constructor for the DICOM SCP.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The constructor allows the user to pass an object to plugins that implement the 
        /// <see cref="IDicomScp{TContext}"/> interface. 
        /// </para>
        /// </remarks>
        /// <param name="context">An object to be passed to plugins implementing the <see cref="IDicomScp{TContext}"/> interface.</param>
        /// <param name="verifier">Delegate called when a new association arrives to verify if it should be accepted or rejected.  Can be set to null.</param>
        public DicomScp(TContext context, AssociationVerifyCallback verifier)
        {
            _context = context;
            _verifier = verifier;
        }

		/// <summary>
		/// Constructor for the DICOM SCP.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The constructor allows the user to pass an object to plugins that implement the 
		/// <see cref="IDicomScp{TContext}"/> interface. 
		/// </para>
		/// </remarks>
		/// <param name="context">An object to be passed to plugins implementing the <see cref="IDicomScp{TContext}"/> interface.</param>
		/// <param name="verifier">Delegate called when a new association arrives to verify if it should be accepted or rejected.  Can be set to null.</param>
		/// <param name="complete">Delegate called when the association is complete/released relaying storage instance information.  Typically used for audit log purposes.</param>
		public DicomScp(TContext context, AssociationVerifyCallback verifier, AssociationComplete complete)
		{
			_context = context;
			_verifier = verifier;
			_complete = complete;
		}
        #endregion

        #region Private Members

        private ServerAssociationParameters _assocParameters;
        private readonly TContext _context;
        private readonly AssociationVerifyCallback _verifier;
    	private readonly AssociationComplete _complete;
        #endregion

        #region Properties

        /// <summary>
        /// The local Application Entity Title of the DICOM SCP.
        /// </summary>
        public string AeTitle { get; set; }

        /// <summary>
        /// The listen port of the DICOM SCP. 
        /// </summary>
        public int ListenPort { get; set; }

        /// <summary>
        /// The listen port of the DICOM SCP. 
        /// </summary>
        public IPAddress ListenAddress { get; set; }


        /// <summary>
        /// The Association parameters used to negotiate the association.
        /// </summary>
        public ServerAssociationParameters AssociationParameters
        {
            get { return _assocParameters; }
        }

		/// <summary>
		/// The context associated with component.
		/// </summary>
    	public TContext Context
    	{
			get { return _context; }
    	}
        #endregion

        #region Private Methods
        /// <summary>
        /// Create the list of presentation contexts for the DICOM SCP.
        /// </summary>
        /// <remarks>
        /// The method loads the DICOM Scp plugins, and then queries them
        /// to construct a list of presentation contexts that are supported.
        /// </remarks>
		private void CreatePresentationContexts()
        {
        	var ep = new DicomScpExtensionPoint<TContext>();
        	object[] scps = ep.CreateExtensions();
        	foreach (object obj in scps)
        	{
        		var scp = obj as IDicomScp<TContext>;
        		scp.SetContext(_context);

        		IList<SupportedSop> sops = scp.GetSupportedSopClasses();
        		foreach (SupportedSop sop in sops)
        		{
        			byte pcid = _assocParameters.FindAbstractSyntax(sop.SopClass);
        			if (pcid == 0)
        				pcid = _assocParameters.AddPresentationContext(sop.SopClass);

        			// Now add all the transfer syntaxes, if necessary
        			foreach (TransferSyntax syntax in sop.SyntaxList)
        			{
        				// Check if the syntax is registered already
        				if (0 == _assocParameters.FindAbstractSyntaxWithTransferSyntax(sop.SopClass, syntax))
        				{
        					_assocParameters.AddTransferSyntax(pcid, syntax);
        				}
        			}
        		}
        	}

			// Sort the presentation contexts, and put them in the order that we prefer them.
			// Favor Explicit over Implicit transfer syntaxes, lossless compression over lossy
			// compression, and lossless compressed over uncompressed.
        	foreach (DicomPresContext serverContext in _assocParameters.GetPresentationContexts())
        	{
				serverContext.SortTransfers(delegate(TransferSyntax s1, TransferSyntax s2)
				                            	{
													if (s1.Equals(s2))
														return 0;
													if (s1.ExplicitVr && !s2.ExplicitVr)
														return -1;
													if (!s1.ExplicitVr && s2.ExplicitVr)
														return 1;
													if (s1.Encapsulated && s2.Encapsulated)
													{
														if (s1.LosslessCompressed == s2.LosslessCompressed)
															return 0;
														if (s1.LosslessCompressed && s2.LossyCompressed)
															return -1;
														return 1;
													}
													if (s1.Encapsulated)
													{
														if (s1.LossyCompressed)
															return 1;
														return -1;
													}

													if (s2.Encapsulated)
													{
														if (s2.LossyCompressed)
															return -1;
														return 1;

													}
				                            		return 0;
				                            	}
					);
        	}
        }

    	#endregion

        #region Public Methods
        /// <summary>
        /// Delegate for use with <see cref="DicomServer"/> to create a handler
        /// that implements the <see cref="IDicomServerHandler"/> interface for a new incoming association.
        /// </summary>
        /// <param name="assoc">The association parameters for the negotiated association.</param>
        /// <param name="server">The server.</param>
        /// <returns>A new <see cref="DicomScpHandler{TContext}"/> instance.</returns>
        public IDicomServerHandler StartAssociation(DicomServer server, ServerAssociationParameters assoc)
        {
        	return new DicomScpHandler<TContext>(server, assoc, _context, _verifier, _complete);
        }


        /// <summary>
        /// Start listening for associations.
        /// </summary>
        /// <returns>true on success, false on failure.</returns>
        public bool Start()
        {
            if (ListenAddress != null)
                return Start(ListenAddress);

            Platform.Log(LogLevel.Fatal, "Attempted to listen on AE {0} with no Listening IP Address set.", AeTitle);
             
            return false;
        }


        /// <summary>
        /// Start listening for associations.
        /// </summary>
        /// <returns>true on success, false on failure.</returns>
        public bool Start(IPAddress addr)
        {
            try
            {
                ListenAddress = addr;

                _assocParameters = new ServerAssociationParameters(AeTitle, new IPEndPoint(addr, ListenPort));

                // Load our presentation contexts from all the extensions
                CreatePresentationContexts();

                if (_assocParameters.GetPresentationContextIDs().Count == 0)
                {
                    Platform.Log(LogLevel.Fatal, "No configured presentation contexts for AE: {0}", AeTitle);
                    return false;
                }

                return DicomServer.StartListening(_assocParameters, StartAssociation);
            }
            catch (DicomException ex)
            {
                Platform.Log(LogLevel.Fatal, ex, "Unexpected exception when starting listener on port {0)", ListenPort);
                return false;
            }
        }

        /// <summary>
        /// Stop the association listener.
        /// </summary>
        public void Stop()
        {
            try
            {
                DicomServer.StopListening(_assocParameters);
            }
            catch (DicomException e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when stopping listening on port {0}", ListenPort);
            }
        }
        #endregion
    }
}
