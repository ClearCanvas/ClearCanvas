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
using System.Net;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Utilities.Statistics;

namespace ClearCanvas.Dicom.Samples
{
    /// <summary>
    /// DICOM Storage SCP Sample Application
    /// </summary>
    class StorageScp : IDicomServerHandler
    {
        #region Private Members
        private static bool _started;
        private static ServerAssociationParameters _staticAssocParameters;
        private AssociationStatisticsRecorder _statsRecorder;
        #endregion

        #region Constructors
        private StorageScp(DicomServer server, ServerAssociationParameters assoc)
        {
            _statsRecorder = new AssociationStatisticsRecorder(server); 
        }
        #endregion

        #region Public Properties
        public static bool Started
        {
            get { return _started; }
        }

        /// <summary>
        /// The path (directory) to store incoming images.
        /// </summary>
        public static string StorageLocation { get; set; }

        public static bool Bitbucket { get; set; }
        public static bool List { get; set; }

        public static bool JpegLossless { get; set; }
        public static bool JpegLossy { get; set; }
        public static bool Rle { get; set; }
        public static bool J2KLossy { get; set; }
        public static bool J2KLossless { get; set; }

        #endregion

        #region Private Methods
        private static void SetImageTransferSyntaxes(byte pcid, ServerAssociationParameters assoc)
        {
            if (JpegLossless)
                assoc.AddTransferSyntax(pcid,TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1);
            if (Rle)
                assoc.AddTransferSyntax(pcid, TransferSyntax.RleLossless);
            if (J2KLossy)
                assoc.AddTransferSyntax(pcid, TransferSyntax.Jpeg2000ImageCompression);
            if (J2KLossless)
                assoc.AddTransferSyntax(pcid, TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
            if (JpegLossy)
            {
                assoc.AddTransferSyntax(pcid, TransferSyntax.JpegBaselineProcess1);
                assoc.AddTransferSyntax(pcid, TransferSyntax.JpegExtendedProcess24);
            }
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);
        }

        private static void AddPresentationContexts(ServerAssociationParameters assoc)
        {
            byte pcid = assoc.AddPresentationContext(SopClass.VerificationSopClass);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.KeyObjectSelectionDocumentStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.ComprehensiveSrStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.BlendingSoftcopyPresentationStateStorageSopClass);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.ColonCadSrStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.DeformableSpatialRegistrationStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.EnhancedSrStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.BasicTextSrStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.EncapsulatedPdfStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.XRayRadiationDoseSrStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.ChestCadSrStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = assoc.AddPresentationContext(SopClass.EncapsulatedCdaStorage);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtBeamsDeliveryInstructionStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtBeamsTreatmentRecordStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtBrachyTreatmentRecordStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtDoseStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtImageStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtIonBeamsTreatmentRecordStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtIonPlanStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtPlanStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtStructureSetStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.RtTreatmentSummaryRecordStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			pcid = assoc.AddPresentationContext(SopClass.MammographyCadSrStorage);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			assoc.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);


            pcid = assoc.AddPresentationContext(SopClass.MrImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.CtImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.SecondaryCaptureImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundImageStorageRetired);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundMultiFrameImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.UltrasoundMultiFrameImageStorageRetired);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.NuclearMedicineImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.DigitalIntraOralXRayImageStorageForPresentation);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.DigitalIntraOralXRayImageStorageForProcessing);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.DigitalMammographyXRayImageStorageForPresentation);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.DigitalMammographyXRayImageStorageForProcessing);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.DigitalXRayImageStorageForPresentation);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.DigitalXRayImageStorageForProcessing);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.ComputedRadiographyImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.OphthalmicPhotography16BitImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.OphthalmicPhotography8BitImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.VideoEndoscopicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.VideoMicroscopicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.VideoPhotographicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.VlEndoscopicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.VlMicroscopicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.VlPhotographicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.VlSlideCoordinatesMicroscopicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.XRayAngiographicBiPlaneImageStorageRetired);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.XRayAngiographicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.XRayRadiofluoroscopicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.XRay3dAngiographicImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.XRay3dCraniofacialImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.OphthalmicTomographyImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.EnhancedCtImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.EnhancedMrColorImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.EnhancedMrImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.EnhancedPetImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.BreastTomosynthesisImageStorage);
            SetImageTransferSyntaxes(pcid, assoc);

            pcid = assoc.AddPresentationContext(SopClass.EnhancedUsVolumeStorage);
            SetImageTransferSyntaxes(pcid, assoc);

        }
        #endregion

        #region Static Public Methods
        public static void StartListening(string aeTitle, int port)
        {
            if (_started)
                return;

            _staticAssocParameters = new ServerAssociationParameters(aeTitle, new IPEndPoint(IPAddress.Any, port));

            AddPresentationContexts(_staticAssocParameters);

            DicomServer.StartListening(_staticAssocParameters,
                                       (server, assoc) => new StorageScp(server, assoc));

            _started = true;
        }

        public static void StopListening(int port)
        {
            if (_started)
            {
                DicomServer.StopListening(_staticAssocParameters);
                _started = false;
            }
        }
        #endregion


        #region IDicomServerHandler Members

       
        void IDicomServerHandler.OnReceiveAssociateRequest(DicomServer server, ServerAssociationParameters association)
        {
            server.SendAssociateAccept(association);
        }

        void IDicomServerHandler.OnReceiveRequestMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {

            if (message.CommandField == DicomCommandField.CEchoRequest)
            {
                server.SendCEchoResponse(presentationID, message.MessageId, DicomStatuses.Success);
                return;
            }

            String studyInstanceUid = null;
            String seriesInstanceUid = null;
            DicomUid sopInstanceUid;
            String patientName = null;

            bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetUid(0, out sopInstanceUid);
            if (ok) ok = message.DataSet[DicomTags.SeriesInstanceUid].TryGetString(0, out seriesInstanceUid);
            if (ok) ok = message.DataSet[DicomTags.StudyInstanceUid].TryGetString(0, out studyInstanceUid);
            if (ok) ok = message.DataSet[DicomTags.PatientsName].TryGetString(0, out patientName);
		
            if (!ok)
            {
                Platform.Log(LogLevel.Error, "Unable to retrieve UIDs from request message, sending failure status.");

                server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID,
                    DicomStatuses.ProcessingFailure);
                return;
            }
            TransferSyntax syntax = association.GetPresentationContext(presentationID).AcceptedTransferSyntax;

            if (List)
            {
                Platform.Log(LogLevel.Info, message.Dump());
            }

            if (Bitbucket)
            {
                Platform.Log(LogLevel.Info, "Received SOP Instance: {0} for patient {1} in syntax {2}", sopInstanceUid,
                             patientName, syntax.Name);

                server.SendCStoreResponse(presentationID, message.MessageId,
                    sopInstanceUid.UID,
                    DicomStatuses.Success);
                return;
            }

            if (!Directory.Exists(StorageLocation))
                Directory.CreateDirectory(StorageLocation);

            var path = new StringBuilder();
            path.AppendFormat("{0}{1}{2}{3}{4}", StorageLocation,  Path.DirectorySeparatorChar,
                studyInstanceUid, Path.DirectorySeparatorChar,seriesInstanceUid);

            Directory.CreateDirectory(path.ToString());

            path.AppendFormat("{0}{1}.dcm", Path.DirectorySeparatorChar, sopInstanceUid.UID);

            var dicomFile = new DicomFile(message, path.ToString())
                                {
                                    TransferSyntaxUid = syntax.UidString,
                                    MediaStorageSopInstanceUid = sopInstanceUid.UID,
                                    ImplementationClassUid = DicomImplementation.ClassUID.UID,
                                    ImplementationVersionName = DicomImplementation.Version,
                                    SourceApplicationEntityTitle = association.CallingAE,
                                    MediaStorageSopClassUid = message.SopClass.Uid
                                };


            dicomFile.Save(DicomWriteOptions.None);

            Platform.Log(LogLevel.Info, "Received SOP Instance: {0} for patient {1} in syntax {2}", sopInstanceUid,
                         patientName, syntax.Name);

            server.SendCStoreResponse(presentationID, message.MessageId,
                sopInstanceUid.UID, 
                DicomStatuses.Success);
        }

        void IDicomServerHandler.OnReceiveResponseMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
			Platform.Log(LogLevel.Error, "Unexpectedly received response mess on server.");

            server.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.UnrecognizedPDU);
        }



        void IDicomServerHandler.OnReceiveReleaseRequest(DicomServer server, ServerAssociationParameters association)
        {
			Platform.Log(LogLevel.Info, "Received association release request from  {0}.", association.CallingAE);
        }

	    public void OnReceiveDimseCommand(DicomServer server, ServerAssociationParameters association, byte presentationId,
	                                      DicomAttributeCollection command)
	    {
	    }

	    public IDicomFilestreamHandler OnStartFilestream(DicomServer server, ServerAssociationParameters association,
	                                                     byte presentationId, DicomMessage message)
	    {
			// Should not be called bcause OnReceiveDimseCommand isn't doing anything
		    throw new NotImplementedException();
	    }

	    void IDicomServerHandler.OnReceiveAbort(DicomServer server, ServerAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
        {
			Platform.Log(LogLevel.Error, "Unexpected association abort received.");
        }

        void IDicomServerHandler.OnNetworkError(DicomServer server, ServerAssociationParameters association, Exception e)
        {
            Platform.Log(LogLevel.Error, e, "Unexpected network error over association from {0}.", association.CallingAE);
        }

        void IDicomServerHandler.OnDimseTimeout(DicomServer server, ServerAssociationParameters association)
        {
            Platform.Log(LogLevel.Info, "Received DIMSE Timeout, continuing listening for messages");
        }
        

        protected void LogAssociationStatistics(ServerAssociationParameters association)
        {

        }
        #endregion
    }
}
