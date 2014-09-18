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

using System.Threading.Tasks;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Utilities.Statistics
{
    /// <summary>
    /// Wrapper class to generate transmission statistics for a DICOM association.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class AssociationStatisticsRecorder
    {
        
        #region private members
        // The tranmission statistics.
        private TransmissionStatistics _assocStats = null;
    	private bool _logInformation;
        #endregion

        #region Public Properties
        /// <summary>
        ///  Gets the statistics of the transmissions.
        /// </summary>
        public TransmissionStatistics Statistics
        {
            get { return _assocStats;  }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Creates an instance of <seealso cref="AssociationStatisticsRecorder"/>
        /// </summary>
        /// <param name="network"></param>
        public AssociationStatisticsRecorder(NetworkBase network)
        {
        	_logInformation = network.LogInformation;

            // hookup network events
            network.AssociationEstablished+=OnAssociationEstablished;
            network.MessageReceived += OnDicomMessageReceived;
            network.MessageSent += OnDicomMessageSent;
            network.AssociationReleased+=OnAssociationReleased;

            string description;
            if (network is DicomClient)
                description = string.Format("DICOM association from {0} [{1}:{2}] to {3} [{4}:{5}]",
                                            network.AssociationParams.CallingAE,
                                            network.AssociationParams.LocalEndPoint.Address,
                                            network.AssociationParams.LocalEndPoint.Port,
                                            network.AssociationParams.CalledAE,
                                            network.AssociationParams.RemoteEndPoint.Address,
                                            network.AssociationParams.RemoteEndPoint.Port);
            else
                description = string.Format("DICOM association from {0} [{1}:{2}] to {3} [{4}:{5}]",
                                            network.AssociationParams.CallingAE,
                                            network.AssociationParams.RemoteEndPoint.Address,
                                            network.AssociationParams.RemoteEndPoint.Port,
                                            network.AssociationParams.CalledAE,
                                            network.AssociationParams.LocalEndPoint.Address,
                                            network.AssociationParams.LocalEndPoint.Port);

            _assocStats = new TransmissionStatistics(description);
        }


        #endregion

        #region protected methods
        /// <summary>
        /// Event handlers called when association has been established.
        /// </summary>
        /// <param name="assoc">The association</param>
        protected void OnAssociationEstablished(AssociationParameters assoc)
        {
            if (_assocStats == null)
                _assocStats = new TransmissionStatistics(string.Format("DICOM association from {0} [{1}:{2}] to {3}", 
                                    assoc.CallingAE, 
                                    assoc.RemoteEndPoint.Address, 
                                    assoc.RemoteEndPoint.Port,
                                    assoc.CalledAE));
                      
            // start recording
            _assocStats.Begin(); 
        }

        /// <summary>
        /// Event handler called when an association has been released.
        /// </summary>
        /// <param name="assoc">The association</param>
        protected void OnAssociationReleased(AssociationParameters assoc)
        {
            if (_assocStats == null)
                return;

            // update the association statistics
            _assocStats.IncomingBytes = assoc.TotalBytesRead;
            _assocStats.OutgoingBytes = assoc.TotalBytesSent; 
            
            // signal stop recording.. the statistic object will fill out whatever 
            // it needs at this point based on what we have set
            _assocStats.End();

	        if (_logInformation)
		        Task.Factory.StartNew(() => StatisticsLogger.Log(LogLevel.Info, _assocStats));
        }

        /// <summary>
        /// Event handler called when an association has been aborted.
        /// </summary>
        /// <param name="assoc">The aborted association</param>
        /// <param name="reason">The abort reason</param>
        protected void OnAssociationAborted(AssociationParameters assoc, DicomAbortReason reason)
        {
            if (_assocStats == null)
                return;

            // update the association statistics
            _assocStats.IncomingBytes = assoc.TotalBytesRead;
            _assocStats.OutgoingBytes = assoc.TotalBytesSent; 
            
            // signal stop recording.. the statistic object will fill out whatever 
            // it needs at this point based on what we have set
            _assocStats.End();           
        }

        /// <summary>
        /// Event handler called while a DICOM message has been received.
        /// </summary>
        /// <param name="assoc">The association</param>
        /// <param name="dcmMsg">The received DICOM message</param>
        private void OnDicomMessageReceived(
                                AssociationParameters assoc, 
                                DicomMessage dcmMsg)
        {
            if (_assocStats == null)
                return;

            // update the association stats
            _assocStats.IncomingBytes = assoc.TotalBytesRead;
            _assocStats.OutgoingBytes = assoc.TotalBytesSent; 
            
            _assocStats.IncomingMessages++;            
        }

        /// <summary>
        /// Event handler called while a DICOM message has been sent.
        /// </summary>
        /// <param name="assoc">The association</param>
        /// <param name="dcmMsg">The request DICOM message sent</param>
        private void OnDicomMessageSent(
                                AssociationParameters assoc,
                                DicomMessage dcmMsg)
        {
            if (_assocStats == null)
                return;

            // update the association stats
            _assocStats.IncomingBytes = assoc.TotalBytesRead;
            _assocStats.OutgoingBytes = assoc.TotalBytesSent; 
            
            _assocStats.OutgoingMessages++;
        }

        #endregion
    }
}
