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
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// An ImageServer specific customization of the <see cref="StorageScu"/> component.
    /// </summary>
    public class ImageServerStorageScu : StorageScu
    {
        #region Private Members
        private readonly Device _remoteDevice;
        private readonly ServerPartition _partition;
        private readonly Dictionary<string, List<TransferSyntax>> _sopClassUncompressedProposedTransferSyntaxes =new Dictionary<string, List<TransferSyntax>>();

    	#endregion

        #region Constructors...
        /// <summary>
        /// Constructor for Storage SCU Component.
        /// </summary>
        public ImageServerStorageScu(ServerPartition partition, Device remoteDevice)
            :base(partition.AeTitle,remoteDevice.AeTitle,remoteDevice.IpAddress,remoteDevice.Port)
        {
            _remoteDevice = remoteDevice;
            _partition = partition;
        }

        /// <summary>
        /// Constructor for Storage SCU Component.
        /// </summary>
        /// <param name="partition">The <see cref="ServerPartition"/> instagating the association.</param>
        /// <param name="remoteDevice">The <see cref="Device"/> being connected to.</param>
        /// <param name="moveOriginatorAe">The Application Entity Title of the application that orginated this C-STORE association.</param>
        /// <param name="moveOrginatorMessageId">The Message ID of the C-MOVE-RQ message that orginated this C-STORE association.</param>
        public ImageServerStorageScu(ServerPartition partition, Device remoteDevice, string moveOriginatorAe, ushort moveOrginatorMessageId)
            : base(partition.AeTitle, remoteDevice.AeTitle, remoteDevice.IpAddress, remoteDevice.Port, moveOriginatorAe, moveOrginatorMessageId)
        {
        	_remoteDevice = remoteDevice;
            _partition = partition;
        }
        #endregion

        #region Protected Methods

        protected override IEnumerable<TransferSyntax> GetProposedTransferSyntaxesForUncompressedObjects(string sopClassUid)
        {
            List<TransferSyntax> syntaxes;

            if (!_sopClassUncompressedProposedTransferSyntaxes.TryGetValue(sopClassUid, out syntaxes))
            {
                syntaxes = new List<TransferSyntax>();

                // Check the ServerSopClass table to see if only implicit LE should be used when handling the sop class
                var configuration = new PartitionSopClassConfiguration();
                var partitionSopClass = configuration.GetPartitionSopClass(_partition, sopClassUid);
                if (partitionSopClass != null)
                {
                    if (partitionSopClass.ImplicitOnly)
                    {
                        Platform.Log(LogLevel.Info, "System is being configured to send {0} using Implicit LE.", SopClass.GetSopClass(partitionSopClass.SopClassUid));
                    }
                    else
                        syntaxes.Add(TransferSyntax.ExplicitVrLittleEndian);

                    syntaxes.Add(TransferSyntax.ImplicitVrLittleEndian);
                }

                _sopClassUncompressedProposedTransferSyntaxes.Add(sopClassUid, syntaxes);
            }

            return syntaxes;

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        /// <param name="read">A read context to read from the database.</param>
        public void LoadPreferredSyntaxes(IPersistenceContext read)
        {
            var select = read.GetBroker<IDevicePreferredTransferSyntaxEntityBroker>();

            // Setup the select parameters.
            var criteria = new DevicePreferredTransferSyntaxSelectCriteria();
            criteria.DeviceKey.EqualTo(_remoteDevice.GetKey());

            IList<DevicePreferredTransferSyntax> list = select.Find(criteria);

            // Translate the list returned into the database into a list that is supported by the Storage SCU Component
            var sopList = new List<SupportedSop>();
            foreach (DevicePreferredTransferSyntax preferred in list)
            {
                var sop = new SupportedSop
                              {
                                  SopClass = SopClass.GetSopClass(preferred.GetServerSopClass().SopClassUid)
                              };
            	sop.AddSyntax(TransferSyntax.GetTransferSyntax(preferred.GetServerTransferSyntax().Uid));

                sopList.Add(sop);
            }

            SetPreferredSyntaxList(sopList);
        }

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        public void LoadPreferredSyntaxes()
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                LoadPreferredSyntaxes(read);
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="SeriesXml"/> file into the component for sending.
        /// </summary>
        /// <param name="seriesXml"></param>
        /// <param name="seriesPath"></param>
        /// <param name="patientsName"></param>
        /// <param name="patientId"></param>
        /// <param name="studyXml"></param>
        public void LoadSeriesFromSeriesXml(StudyXml studyXml, string seriesPath, SeriesXml seriesXml, string patientsName, string patientId)
        {
            foreach (InstanceXml instanceXml in seriesXml)
            {
                string instancePath = Path.Combine(seriesPath, instanceXml.SopInstanceUid + ServerPlatform.DicomFileExtension);
                var instance = new StorageInstance(instancePath);
                
                AddStorageInstance(instance);
                instance.SopClass = instanceXml.SopClass;
                instance.TransferSyntax = instanceXml.TransferSyntax;
                instance.SopInstanceUid = instanceXml.SopInstanceUid;
            	instance.PatientId = patientId;
            	instance.PatientsName = patientsName;
            	instance.StudyInstanceUid = studyXml.StudyInstanceUid;
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
        /// </summary>
        /// <param name="studyPath"></param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
        public void LoadStudyFromStudyXml(string studyPath, StudyXml studyXml)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                string seriesPath = Path.Combine(studyPath, seriesXml.SeriesInstanceUid);

				LoadSeriesFromSeriesXml(studyXml, seriesPath, seriesXml, studyXml.PatientsName, studyXml.PatientId);
            }
        }

        #endregion

    }
}
