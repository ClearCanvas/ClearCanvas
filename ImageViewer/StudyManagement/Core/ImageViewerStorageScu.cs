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
using System.Globalization;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    /// <summary>
    /// Internal inherited <see cref="StorageScu"/> class.
    /// </summary>
    public class ImageViewerStorageScu : StorageScu
    {
        #region Private Members

        private DicomSendRequest _sendRequest;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets a value indicating whether or not the operation as a whole (as opposed to an individual sub-operation) has failed.
        /// </summary>
        /// <remarks>
        /// Typically, this refers to exceptions being thrown on the connection socket.
        /// </remarks>
        public bool Failed { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="localAETitle">The local AE Title.</param>
        /// <param name="request">The <see cref="WorkItemRequest"/> for the association.</param>
        public ImageViewerStorageScu(string localAETitle, IDicomServiceNode request)
            : base(localAETitle, request.AETitle, request.ScpParameters.HostName, request.ScpParameters.Port)
        {
        }

        

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
        public void LoadStudyFromStudyXml(StudyLocation location, StudyXml studyXml)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                LoadSeriesFromSeriesXml(studyXml, location, seriesXml, studyXml.PatientsName, studyXml.PatientId);
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="seriesInstanceUid"></param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
        public void LoadSeriesFromStudyXml(StudyLocation location, StudyXml studyXml, string seriesInstanceUid)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                if (seriesInstanceUid.Equals(seriesXml.SeriesInstanceUid))
                {
                    LoadSeriesFromSeriesXml(studyXml, location, seriesXml, studyXml.PatientsName, studyXml.PatientId);
                    break;
                }
            }
        }

        /// <summary>
        /// Blocking method to do the send.
        /// </summary>
        public void DoSend()
        {
            Failed = false;
            try
            {
                SendInternal();
                AuditSendOperation(true);
            }
            catch (Exception e)
            {
                // set the connection failure flag
                Failed = true;

                if (Status == ScuOperationStatus.ConnectFailed)
                {
                    OnSendError(String.Format("Unable to connect to remote server ({0}: {1}).",
                                              RemoteAE, FailureDescription ?? "no failure description provided"));
                }
                else if (StorageInstanceList.Count == 0)
                {
                    // if the storage instance count is zero, we know exactly why the storage operation failed
                    OnSendError(String.Format("Store operation failed (nothing to store)."));
                }
                else
                {
                    OnSendError(String.Format(
                        "An unexpected error occurred while processing the Store operation ({0}).", e.Message));
                }

                AuditSendOperation(false);
            }
        }

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        /// <param name="request">A read context to read from the database.</param>
        public void LoadPreferredSyntaxes(DicomSendRequest request)
        {
            _sendRequest = request;

            // TODO (CR Jun 2012): Just set it since it's not an event?

            // Add a delegate to do the actual selection of the contexts;
            PresentationContextSelectionDelegate += SelectPresentationContext;

            var dic = new Dictionary<SopClass, SupportedSop>();
     
            foreach (var storageInstance in StorageInstanceList)
            {
                // skip if failed in LoadStorageInstanceInfo, ie file not found
                if (storageInstance.SendStatus == DicomStatuses.ProcessingFailure)
                    continue;

                storageInstance.LoadInfo();
                SupportedSop sop;
                if (!dic.TryGetValue(storageInstance.SopClass,out sop))
                {
                    sop = new SupportedSop
                              {
                                  SopClass = storageInstance.SopClass
                              };

                    if (request.CompressionType == CompressionType.JpegLossless)
                        sop.AddSyntax(TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1);
                    else if (request.CompressionType == CompressionType.Rle)
                        sop.AddSyntax(TransferSyntax.RleLossless);
                    else if (request.CompressionType == CompressionType.J2KLossy)
                        sop.AddSyntax(TransferSyntax.Jpeg2000ImageCompression);
                    else if (request.CompressionType == CompressionType.J2KLossless)
                        sop.AddSyntax(TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
                    else if (request.CompressionType == CompressionType.JpegLossy)
                    {
                        sop.AddSyntax(TransferSyntax.JpegBaselineProcess1);
                        sop.AddSyntax(TransferSyntax.JpegExtendedProcess24);
                    }
                    dic.Add(storageInstance.SopClass, sop);
                }
            }

            SetPreferredSyntaxList(dic.Values);
        }
        #endregion

        #region Private Methods

        private byte SelectPresentationContext(ClientAssociationParameters association, DicomFile file, out DicomMessage message)
        {
            byte pcid = 0;

            message = new DicomMessage(file);

            // If Lossy compressed & we have a matching context, send
            // If we don't have a codec, just return
            if (message.TransferSyntax.Encapsulated && message.TransferSyntax.LossyCompressed)
            {
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass, message.TransferSyntax);                
                if (pcid != 0) return pcid;

                if (DicomCodecRegistry.GetCodec(message.TransferSyntax) == null)
                    return 0;
            }

            // If the image is lossless compressed & we don't have a codec, send if we
            // can as is.
            if (message.TransferSyntax.Encapsulated && message.TransferSyntax.LosslessCompressed)
            {
                if (DicomCodecRegistry.GetCodec(message.TransferSyntax) == null)
                {
                    pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass, message.TransferSyntax);
                    return pcid;
                }
            }

            // If lossless compressed & requesting lossless syntax, just send as is
            if (message.TransferSyntax.Encapsulated
                && message.TransferSyntax.LosslessCompressed
                && ((_sendRequest.CompressionType == CompressionType.Rle
                || _sendRequest.CompressionType == CompressionType.JpegLossless
                || _sendRequest.CompressionType == CompressionType.J2KLossless)))
            {
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass, message.TransferSyntax);
                if (pcid != 0) return pcid;
            }


            if (_sendRequest.CompressionType == CompressionType.Rle)
            {
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass, TransferSyntax.RleLossless);
                if (pcid != 0)
                {
                    return pcid;
                }
            }
            else if (_sendRequest.CompressionType == CompressionType.JpegLossless)
            {
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass, TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1);
                if (pcid != 0)
                {
                    return pcid;
                }
            }
            else if (_sendRequest.CompressionType == CompressionType.J2KLossless)
            {
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass, TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
                if (pcid != 0)
                {
                    return pcid;
                }
            }
            else if (_sendRequest.CompressionType == CompressionType.J2KLossy)
            {
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass, TransferSyntax.Jpeg2000ImageCompression);
                if (pcid != 0)
                {
                    var doc = new XmlDocument();

                    XmlElement element = doc.CreateElement("compress");
                    doc.AppendChild(element);
                    XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
                    syntaxAttribute.Value = TransferSyntax.Jpeg2000ImageCompressionUid;
                    element.Attributes.Append(syntaxAttribute);

                    decimal ratio = 100.0m  / _sendRequest.CompressionLevel;
                    XmlAttribute ratioAttribute = doc.CreateAttribute("ratio");
                    ratioAttribute.Value = ratio.ToString(CultureInfo.InvariantCulture);
                    element.Attributes.Append(ratioAttribute);

                    syntaxAttribute = doc.CreateAttribute("convertFromPalette");
                    syntaxAttribute.Value = true.ToString(CultureInfo.InvariantCulture);
                    element.Attributes.Append(syntaxAttribute);

                    IDicomCodecFactory[] codecs = DicomCodecRegistry.GetCodecFactories();
                    foreach (IDicomCodecFactory codec in codecs)
                        if (codec.CodecTransferSyntax.Equals(TransferSyntax.Jpeg2000ImageCompression))
                        {
                            try
                            {
                                if (message.TransferSyntax.Encapsulated)
                                {
                                    message.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
                                    message.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
                                }
                                message.ChangeTransferSyntax(TransferSyntax.Jpeg2000ImageCompression,
                                                             codec.GetDicomCodec(),
                                                             codec.GetCodecParameters(doc));
                                message.TransferSyntax = TransferSyntax.Jpeg2000ImageCompression;
                                return pcid;
                            }
                            catch (Exception e)
                            {
                                Platform.Log(LogLevel.Warn, e, "Unexpected exception changing transfer syntax to {0}.",
                                             TransferSyntax.Jpeg2000ImageCompression.Name);
                            }
                        }
                }
            }
            else if (_sendRequest.CompressionType == CompressionType.JpegLossy)
            {
                var iod = new ImagePixelMacroIod(message.DataSet);

                if (iod.BitsStored == 8)
                {
                    pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass,
                                                                            TransferSyntax.JpegBaselineProcess1);
                    if (pcid != 0)
                    {
                        var doc = new XmlDocument();

                        XmlElement element = doc.CreateElement("compress");
                        doc.AppendChild(element);
                        XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
                        syntaxAttribute.Value = TransferSyntax.JpegBaselineProcess1Uid;
                        element.Attributes.Append(syntaxAttribute);

                        syntaxAttribute = doc.CreateAttribute("quality");
                        syntaxAttribute.Value = _sendRequest.CompressionLevel.ToString(CultureInfo.InvariantCulture);
                        element.Attributes.Append(syntaxAttribute);

                        syntaxAttribute = doc.CreateAttribute("convertFromPalette");
                        syntaxAttribute.Value = true.ToString(CultureInfo.InvariantCulture);
                        element.Attributes.Append(syntaxAttribute);

                        IDicomCodecFactory[] codecs = DicomCodecRegistry.GetCodecFactories();
                        foreach (IDicomCodecFactory codec in codecs)
                            if (codec.CodecTransferSyntax.Equals(TransferSyntax.JpegBaselineProcess1))
                            {
                                try
                                {
                                    if (message.TransferSyntax.Encapsulated)
                                    {
                                        message.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
                                        message.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
                                    }

                                    message.ChangeTransferSyntax(TransferSyntax.JpegBaselineProcess1,
                                                                 codec.GetDicomCodec(),
                                                                 codec.GetCodecParameters(doc));
                                    message.TransferSyntax = TransferSyntax.JpegBaselineProcess1;
                                    return pcid;
                                }
                                catch (Exception e)
                                {
                                    Platform.Log(LogLevel.Warn, e,
                                                 "Unexpected exception changing transfer syntax to {0}.",
                                                 TransferSyntax.JpegBaselineProcess1.Name);
                                }
                            }
                    }
                }
                else if (iod.BitsStored == 12)
                {
                    pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass,
                                                                            TransferSyntax.JpegExtendedProcess24);
                    if (pcid != 0)
                    {
                        var doc = new XmlDocument();

                        XmlElement element = doc.CreateElement("compress");
                        doc.AppendChild(element);
                        XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
                        syntaxAttribute.Value = TransferSyntax.JpegExtendedProcess24Uid;
                        element.Attributes.Append(syntaxAttribute);

                        syntaxAttribute = doc.CreateAttribute("quality");
                        syntaxAttribute.Value = _sendRequest.CompressionLevel.ToString(CultureInfo.InvariantCulture);
                        element.Attributes.Append(syntaxAttribute);

                        syntaxAttribute = doc.CreateAttribute("convertFromPalette");
                        syntaxAttribute.Value = true.ToString(CultureInfo.InvariantCulture);
                        element.Attributes.Append(syntaxAttribute);

                        IDicomCodecFactory[] codecs = DicomCodecRegistry.GetCodecFactories();
                        foreach (IDicomCodecFactory codec in codecs)
                            if (codec.CodecTransferSyntax.Equals(TransferSyntax.JpegExtendedProcess24))
                            {
                                try
                                {
                                    if (message.TransferSyntax.Encapsulated)
                                    {
                                        message.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
                                        message.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
                                    }

                                    message.ChangeTransferSyntax(TransferSyntax.JpegExtendedProcess24,
                                                                 codec.GetDicomCodec(),
                                                                 codec.GetCodecParameters(doc));
                                    message.TransferSyntax = TransferSyntax.JpegExtendedProcess24;
                                    return pcid;
                                }
                                catch (Exception e)
                                {
                                    Platform.Log(LogLevel.Warn, e,
                                                 "Unexpected exception changing transfer syntax to {0}.",
                                                 TransferSyntax.JpegExtendedProcess24.Name);
                                }
                            }
                    }
                }
            }

            if (pcid == 0)
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass,
                                                                        TransferSyntax.ExplicitVrLittleEndian);
            if (pcid == 0)
                pcid = association.FindAbstractSyntaxWithTransferSyntax(message.SopClass,
                                                                        TransferSyntax.ImplicitVrLittleEndian);

            return pcid;
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="SeriesXml"/> file into the component for sending.
        /// </summary>
        /// <param name="seriesXml"></param>
        /// <param name="location"></param>
        /// <param name="patientsName"></param>
        /// <param name="patientId"></param>
        /// <param name="studyXml"></param>
        private void LoadSeriesFromSeriesXml(StudyXml studyXml, StudyLocation location, SeriesXml seriesXml,
                                             string patientsName, string patientId)
        {
            foreach (InstanceXml instanceXml in seriesXml)
            {
                var instance =
                    new StorageInstance(location.GetSopInstancePath(seriesXml.SeriesInstanceUid,
                                                                    instanceXml.SopInstanceUid));

                AddStorageInstance(instance);
                instance.SopClass = instanceXml.SopClass;
                instance.TransferSyntax = instanceXml.TransferSyntax;
                instance.SopInstanceUid = instanceXml.SopInstanceUid;
                instance.PatientId = patientId;
                instance.PatientsName = patientsName;
                instance.StudyInstanceUid = studyXml.StudyInstanceUid;
            }
        }

        private void AuditSendOperation(bool noExceptions)
        {
            if (noExceptions)
            {
                var sentInstances = new AuditedInstances();
                var failedInstances = new AuditedInstances();
                foreach (StorageInstance instance in StorageInstanceList)
                {
                    if (instance.SendStatus.Status == DicomState.Success)
                        sentInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
                    else
                        failedInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
                }
                AuditHelper.LogSentInstances(RemoteAE, RemoteHost, sentInstances, EventSource.CurrentProcess,
                                             EventResult.Success);
                AuditHelper.LogSentInstances(RemoteAE, RemoteHost, failedInstances, EventSource.CurrentProcess,
                                             EventResult.MinorFailure);
            }
            else
            {
                var sentInstances = new AuditedInstances();
                foreach (StorageInstance instance in StorageInstanceList)
                    sentInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
                AuditHelper.LogSentInstances(RemoteAE, RemoteHost, sentInstances, EventSource.CurrentProcess,
                                             EventResult.MajorFailure);
            }
        }

        private void SendInternal()
        {     
            base.Send();

            Join(new TimeSpan(0, 0, 0, 0, 1000));

            if (Status == ScuOperationStatus.Canceled)
            {
                OnSendError(String.Format("The Store operation has been cancelled ({0}).", RemoteAE));
            }
            else if (Status == ScuOperationStatus.ConnectFailed)
            {
                OnSendError(String.Format("Unable to connect to remote server ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.AssociationRejected)
            {
                OnSendError(String.Format("Association rejected ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.Failed)
            {
                OnSendError(String.Format("The Store operation failed ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.TimeoutExpired)
            {
                OnSendError(String.Format("The connection timeout has expired ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.UnexpectedMessage)
            {
                OnSendError("Unexpected message received; aborted association.");
            }
            else if (Status == ScuOperationStatus.NetworkError)
            {
                OnSendError("An unexpected network error has occurred.");
            }
        }

        private void OnSendError(string message)
        {
            FailureDescription = message;
            Platform.Log(LogLevel.Error, message);
        }

        #endregion        
    }
}


