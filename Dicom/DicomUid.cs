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
using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Enumerated value for the various types of DICOM UIDs.
    /// </summary>
    public enum UidType
    {
        TransferSyntax,
        SOPClass,
        MetaSOPClass,
        SOPInstance,
        ApplicationContextName,
        CodingScheme,
        SynchronizationFrameOfReference,
        Unknown
    }

    /// <summary>
    /// Class used to represent a DICOM unique identifier (UID).
    /// </summary>
    public class DicomUid
    {
        #region Private Members
        private readonly string _uid;
        private readonly string _description;
        private readonly UidType _type;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uid">The UID string.</param>
        /// <param name="desc">A description of the UID.</param>
        /// <param name="type">The type of the UID.</param>
        public DicomUid(string uid, string desc, UidType type)
        {
            if (uid.Length > 64)
                throw new DicomException("Invalid UID length: " + uid.Length + "!");

            _uid = uid;
            _description = desc;
            _type = type;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The string representation of the UID.
        /// </summary>
        public string UID
        {
            get { return _uid; }
        }

        /// <summary>
        /// A description of the UID.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// The type of the UID.
        /// </summary>
        public UidType Type
        {
            get { return _type; }
        }
        #endregion

        /// <summary>
        /// Override that displays the type of the UID if known, or else the UID value itself.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Type == UidType.Unknown)
                return UID;
            return "==" + Description;
        }

        /// <summary>
        /// Override that compares if two DicomUid instances are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var uid = obj as DicomUid;
            if (uid != null)
                return uid.UID.Equals(UID);

            var value = obj as String;
            if (value != null)
                return value == UID;

            return false;
        }

        /// <summary>
        /// An override that determines a hash code for the instance.
        /// </summary>
        /// <returns>The hash code of the UID string.</returns>
        public override int GetHashCode()
        {
            return _uid.GetHashCode();
        }

        #region Static UID Generation Routines
        /* members for UID Generation */
        private static String _lastTimestamp;
        private static String _baseUid = null;
        private static Object _lock = new object();
        private static short _count = 0;

        /// <summary>
        /// This routine generates a DICOM Unique Identifier.  Note, this UID is NOT unique for UIDs generated in the same process across App Domains.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The UID generator uses the ClearCanvas UID base, the computers MAC address, a timestamp,
        /// a process ID, and a counter to ensure uniqueness for the UID.  The UID has the following
        /// components:
        /// </para>
        /// <list type="table">
        ///   <listheader>
        ///     <term>UID Component</term>
        /// <description>Description</description>
        ///   </listheader>
        /// <item>
        ///   <term> 1.3.6.1.4.1.25403 </term>
        ///   <description>
        ///   The ClearCanvas assigned UID root.  This root has been assigned to ClearCanvas by IANA.  This
        ///   component uses 17 characters.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>MAC Address (.NNNNNNNNNNNNNNN)</term>
        ///   <description>
        ///   The 6 bytes encoded in the network card's MAC address are masked into an unsigned long, and then 
        ///   added as decimal to the UID.  This component guarentees that the UID is
        ///   unique to the computer that it is running on.  This component uses 16 characters.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term> Process ID (.NNNNNNNNNN)</term>
        ///   <description>
        ///   The process ID of the process creating the UID.  This component along with the timestamp guarentee
        ///   that the UID is unique to the specific process that is generating it.  This component has a maximum
        ///   of 11 chracters.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term> Time Stamp (.YYYYMMDDhhmmss) </term>
        ///   <description>
        ///   The timestamp contains The year/month/day/hour/minute/second that the UID was created.  This
        ///   component uses 15 characters.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term> Counter (.NNNN)</term>
        ///   <description>
        ///   The counter is used in case more than one UID is generated within a second.  This component has a 
        ///   maximum of 5 characters.
        ///   </description>
        /// </item>
        /// </list>
        /// <para>
        /// The UID generator uses the above components to insure uniqueness.  The use of the MAC address ensures that 
        /// the UID is unique to the system that the generator is run on.  The use of a timestamp and process ID ensures 
        /// that the Uid is unique to a specific process, and the counter ensures that if more than one Uid is generated
        /// within a second by a process, that it is unique.
        /// </para>
        /// <para>
        /// Note that the definition of the components of the Uid may allow the Uid to reach the maximum length of 
        /// 64 characters, although it is unlikely this will ever happen.
        /// </para>
        /// </remarks>
        /// <returns></returns>
        private static DicomUid ObsoleteGenerateUid()
        {
            lock (_lock)
            {
                if (_baseUid == null)
                {
                    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                    if (nics.Length == 0)
                        throw new DicomException("No network cards in system, unable to generate UID");

                    byte[] addressBytes = nics[0].GetPhysicalAddress().GetAddressBytes();

                    if (addressBytes.Length > 6)
                        throw new DicomException("Unexpected length for MAC address, unable to generate a UID");

                    ulong address = 0;

                    foreach (byte b in addressBytes)
                    {
                        address <<= 8;
                        address |= b;
                    }

                    var sb = new StringBuilder();

                    // ClearCanvas root from IANA
                    sb.Append("1.3.6.1.4.1.25403");

                    // MAC address converted to decimal
                    sb.AppendFormat(".{0}", address);

                    // Process Id
                    sb.AppendFormat(".{0}", (uint)Process.GetCurrentProcess().Id);

                    _baseUid = sb.ToString();

                    _lastTimestamp = DateTime.Now.ToString("yyyyMMddhhmmss");
                }

                var uid = new StringBuilder();
                uid.Append(_baseUid);

                String time = DateTime.Now.ToString("yyyyMMddhhmmss");
                if (time.Equals(_lastTimestamp))
                {
                    if (_count == 9999)
                        throw new DicomException("Unexpected count reached max value in UID generator!");

                    _count++;
                }
                else
                {
                    _count = 1;
                    _lastTimestamp = time;
                }

                uid.AppendFormat(".{0}.{1}", time, _count);

                return new DicomUid(uid.ToString(),"Instance UID",UidType.SOPInstance);
            }
        }

        /// <summary>
        /// This routine generates a DICOM Unique Identifier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The UID generator uses a GUID to generate the UID, as descriped in DICOM CP-1156:
        ///
        /// ftp://medical.nema.org/medical/dicom/final/cp1156_ft.pdf
        /// 
        /// The UID is composed of the following components:
        /// </para>
        /// <list type="table">
        ///   <listheader>
        ///     <term>UID Component</term> <description>Description</description>
        ///   </listheader>
        /// <item>
        ///   <term> 2.25 </term>
        ///   <description>
        ///   The UID root for GUID UIDs as per CP-1156.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>GUID as Integer</term>
        ///   <description>
        ///   The Guid is converted to an integer and displayed in base 10.  Can be upto 39 characters long.
        ///   </description>
        /// </item>
        /// </list>
        /// <para>
        /// The UID generator uses the above components to insure uniqueness.  It simply converts a GUID acquired by a 
        /// call to Guid.NewGuid() into an integer and appends it to the UID for uniqueness.
        /// </para>
        /// </remarks>
        /// <returns></returns>
        public static DicomUid GenerateUid()
        {
            var guidBytes = string.Format("0{0:N}", Guid.NewGuid());
            var bigInteger = BigInteger.Parse(guidBytes, NumberStyles.HexNumber);

            return new DicomUid(string.Format(CultureInfo.InvariantCulture, "2.25.{0}", bigInteger), "Instance UID", UidType.SOPInstance);
        }

        #endregion
    } 

    public static class DicomUids
    {
        public static Dictionary<string, DicomUid> Entries = new Dictionary<string, DicomUid>();

        static DicomUids()
        {
            #region Load Internal UIDs
            Entries.Add(DicomUids.ImplicitVRLittleEndian.UID, DicomUids.ImplicitVRLittleEndian);
            Entries.Add(DicomUids.ExplicitVRLittleEndian.UID, DicomUids.ExplicitVRLittleEndian);
            Entries.Add(DicomUids.DeflatedExplicitVRLittleEndian.UID, DicomUids.DeflatedExplicitVRLittleEndian);
            Entries.Add(DicomUids.ExplicitVRBigEndian.UID, DicomUids.ExplicitVRBigEndian);
            Entries.Add(DicomUids.JPEGProcess1.UID, DicomUids.JPEGProcess1);
            Entries.Add(DicomUids.JPEGProcess2_4.UID, DicomUids.JPEGProcess2_4);
            Entries.Add(DicomUids.JPEGProcess3_5Retired.UID, DicomUids.JPEGProcess3_5Retired);
            Entries.Add(DicomUids.JPEGProcess6_8Retired.UID, DicomUids.JPEGProcess6_8Retired);
            Entries.Add(DicomUids.JPEGProcess7_9Retired.UID, DicomUids.JPEGProcess7_9Retired);
            Entries.Add(DicomUids.JPEGProcess10_12Retired.UID, DicomUids.JPEGProcess10_12Retired);
            Entries.Add(DicomUids.JPEGProcess11_13Retired.UID, DicomUids.JPEGProcess11_13Retired);
            Entries.Add(DicomUids.JPEGProcess14.UID, DicomUids.JPEGProcess14);
            Entries.Add(DicomUids.JPEGProcess15Retired.UID, DicomUids.JPEGProcess15Retired);
            Entries.Add(DicomUids.JPEGProcess16_18Retired.UID, DicomUids.JPEGProcess16_18Retired);
            Entries.Add(DicomUids.JPEGProcess17_19Retired.UID, DicomUids.JPEGProcess17_19Retired);
            Entries.Add(DicomUids.JPEGProcess20_22Retired.UID, DicomUids.JPEGProcess20_22Retired);
            Entries.Add(DicomUids.JPEGProcess21_23Retired.UID, DicomUids.JPEGProcess21_23Retired);
            Entries.Add(DicomUids.JPEGProcess24_26Retired.UID, DicomUids.JPEGProcess24_26Retired);
            Entries.Add(DicomUids.JPEGProcess25_27Retired.UID, DicomUids.JPEGProcess25_27Retired);
            Entries.Add(DicomUids.JPEGProcess28Retired.UID, DicomUids.JPEGProcess28Retired);
            Entries.Add(DicomUids.JPEGProcess29Retired.UID, DicomUids.JPEGProcess29Retired);
            Entries.Add(DicomUids.JPEGProcess14SV1.UID, DicomUids.JPEGProcess14SV1);
            Entries.Add(DicomUids.JPEGLSLossless.UID, DicomUids.JPEGLSLossless);
            Entries.Add(DicomUids.JPEGLSNearLossless.UID, DicomUids.JPEGLSNearLossless);
            Entries.Add(DicomUids.JPEG2000Lossless.UID, DicomUids.JPEG2000Lossless);
            Entries.Add(DicomUids.JPEG2000Lossy.UID, DicomUids.JPEG2000Lossy);
            Entries.Add(DicomUids.MPEG2.UID, DicomUids.MPEG2);
            Entries.Add(DicomUids.RLELossless.UID, DicomUids.RLELossless);
            Entries.Add(DicomUids.Verification.UID, DicomUids.Verification);
            Entries.Add(DicomUids.MediaStorageDirectoryStorage.UID, DicomUids.MediaStorageDirectoryStorage);
            Entries.Add(DicomUids.BasicStudyContentNotification.UID, DicomUids.BasicStudyContentNotification);
            Entries.Add(DicomUids.StorageCommitmentPushModel.UID, DicomUids.StorageCommitmentPushModel);
            Entries.Add(DicomUids.StorageCommitmentPullModel.UID, DicomUids.StorageCommitmentPullModel);
            Entries.Add(DicomUids.ProceduralEventLoggingSOPClass.UID, DicomUids.ProceduralEventLoggingSOPClass);
            Entries.Add(DicomUids.DetachedPatientManagement.UID, DicomUids.DetachedPatientManagement);
            Entries.Add(DicomUids.DetachedVisitManagement.UID, DicomUids.DetachedVisitManagement);
            Entries.Add(DicomUids.DetachedStudyManagement.UID, DicomUids.DetachedStudyManagement);
            Entries.Add(DicomUids.StudyComponentManagement.UID, DicomUids.StudyComponentManagement);
            Entries.Add(DicomUids.ModalityPerformedProcedureStep.UID, DicomUids.ModalityPerformedProcedureStep);
            Entries.Add(DicomUids.ModalityPerformedProcedureStepRetrieve.UID, DicomUids.ModalityPerformedProcedureStepRetrieve);
            Entries.Add(DicomUids.ModalityPerformedProcedureStepNotification.UID, DicomUids.ModalityPerformedProcedureStepNotification);
            Entries.Add(DicomUids.DetachedResultsManagement.UID, DicomUids.DetachedResultsManagement);
            Entries.Add(DicomUids.DetachedInterpretationManagement.UID, DicomUids.DetachedInterpretationManagement);
            Entries.Add(DicomUids.StorageServiceClass.UID, DicomUids.StorageServiceClass);
            Entries.Add(DicomUids.BasicFilmSession.UID, DicomUids.BasicFilmSession);
            Entries.Add(DicomUids.BasicFilmBoxSOP.UID, DicomUids.BasicFilmBoxSOP);
            Entries.Add(DicomUids.BasicGrayscaleImageBox.UID, DicomUids.BasicGrayscaleImageBox);
            Entries.Add(DicomUids.BasicColorImageBox.UID, DicomUids.BasicColorImageBox);
            Entries.Add(DicomUids.ReferencedImageBoxRetired.UID, DicomUids.ReferencedImageBoxRetired);
            Entries.Add(DicomUids.PrintJob.UID, DicomUids.PrintJob);
            Entries.Add(DicomUids.BasicAnnotationBox.UID, DicomUids.BasicAnnotationBox);
            Entries.Add(DicomUids.Printer.UID, DicomUids.Printer);
            Entries.Add(DicomUids.PrinterConfigurationRetrieval.UID, DicomUids.PrinterConfigurationRetrieval);
            Entries.Add(DicomUids.VOILUTBox.UID, DicomUids.VOILUTBox);
            Entries.Add(DicomUids.PresentationLUT.UID, DicomUids.PresentationLUT);
            Entries.Add(DicomUids.ImageOverlayBox.UID, DicomUids.ImageOverlayBox);
            Entries.Add(DicomUids.BasicPrintImageOverlayBox.UID, DicomUids.BasicPrintImageOverlayBox);
            Entries.Add(DicomUids.PrintQueueManagement.UID, DicomUids.PrintQueueManagement);
            Entries.Add(DicomUids.StoredPrintStorage.UID, DicomUids.StoredPrintStorage);
            Entries.Add(DicomUids.HardcopyGrayscaleImageStorage.UID, DicomUids.HardcopyGrayscaleImageStorage);
            Entries.Add(DicomUids.HardcopyColorImageStorage.UID, DicomUids.HardcopyColorImageStorage);
            Entries.Add(DicomUids.PullPrintRequest.UID, DicomUids.PullPrintRequest);
            Entries.Add(DicomUids.MediaCreationManagementSOPClass.UID, DicomUids.MediaCreationManagementSOPClass);
            Entries.Add(DicomUids.ComputedRadiographyImageStorage.UID, DicomUids.ComputedRadiographyImageStorage);
            Entries.Add(DicomUids.DigitalXRayImageStorageForPresentation.UID, DicomUids.DigitalXRayImageStorageForPresentation);
            Entries.Add(DicomUids.DigitalXRayImageStorageForProcessing.UID, DicomUids.DigitalXRayImageStorageForProcessing);
            Entries.Add(DicomUids.DigitalMammographyXRayImageStorageForPresentation.UID, DicomUids.DigitalMammographyXRayImageStorageForPresentation);
            Entries.Add(DicomUids.DigitalMammographyXRayImageStorageForProcessing.UID, DicomUids.DigitalMammographyXRayImageStorageForProcessing);
            Entries.Add(DicomUids.DigitalIntraoralXRayImageStorageForPresentation.UID, DicomUids.DigitalIntraoralXRayImageStorageForPresentation);
            Entries.Add(DicomUids.DigitalIntraoralXRayImageStorageForProcessing.UID, DicomUids.DigitalIntraoralXRayImageStorageForProcessing);
            Entries.Add(DicomUids.CTImageStorage.UID, DicomUids.CTImageStorage);
            Entries.Add(DicomUids.EnhancedCTImageStorage.UID, DicomUids.EnhancedCTImageStorage);
            Entries.Add(DicomUids.UltrasoundMultiframeImageStorageRetired.UID, DicomUids.UltrasoundMultiframeImageStorageRetired);
            Entries.Add(DicomUids.UltrasoundMultiframeImageStorage.UID, DicomUids.UltrasoundMultiframeImageStorage);
            Entries.Add(DicomUids.MRImageStorage.UID, DicomUids.MRImageStorage);
            Entries.Add(DicomUids.EnhancedMRImageStorage.UID, DicomUids.EnhancedMRImageStorage);
            Entries.Add(DicomUids.MRSpectroscopyStorage.UID, DicomUids.MRSpectroscopyStorage);
            Entries.Add(DicomUids.NuclearMedicineImageStorageRetired.UID, DicomUids.NuclearMedicineImageStorageRetired);
            Entries.Add(DicomUids.UltrasoundImageStorageRetired.UID, DicomUids.UltrasoundImageStorageRetired);
            Entries.Add(DicomUids.UltrasoundImageStorage.UID, DicomUids.UltrasoundImageStorage);
            Entries.Add(DicomUids.SecondaryCaptureImageStorage.UID, DicomUids.SecondaryCaptureImageStorage);
            Entries.Add(DicomUids.MultiframeSingleBitSecondaryCaptureImageStorage.UID, DicomUids.MultiframeSingleBitSecondaryCaptureImageStorage);
            Entries.Add(DicomUids.MultiframeGrayscaleByteSecondaryCaptureImageStorage.UID, DicomUids.MultiframeGrayscaleByteSecondaryCaptureImageStorage);
            Entries.Add(DicomUids.MultiframeGrayscaleWordSecondaryCaptureImageStorage.UID, DicomUids.MultiframeGrayscaleWordSecondaryCaptureImageStorage);
            Entries.Add(DicomUids.MultiframeTrueColorSecondaryCaptureImageStorage.UID, DicomUids.MultiframeTrueColorSecondaryCaptureImageStorage);
            Entries.Add(DicomUids.StandaloneOverlayStorage.UID, DicomUids.StandaloneOverlayStorage);
            Entries.Add(DicomUids.StandaloneCurveStorage.UID, DicomUids.StandaloneCurveStorage);
            Entries.Add(DicomUids.TwelveLeadECGWaveformStorage.UID, DicomUids.TwelveLeadECGWaveformStorage);
            Entries.Add(DicomUids.GeneralECGWaveformStorage.UID, DicomUids.GeneralECGWaveformStorage);
            Entries.Add(DicomUids.AmbulatoryECGWaveformStorage.UID, DicomUids.AmbulatoryECGWaveformStorage);
            Entries.Add(DicomUids.HemodynamicWaveformStorage.UID, DicomUids.HemodynamicWaveformStorage);
            Entries.Add(DicomUids.CardiacElectrophysiologyWaveformStorage.UID, DicomUids.CardiacElectrophysiologyWaveformStorage);
            Entries.Add(DicomUids.BasicVoiceAudioWaveformStorage.UID, DicomUids.BasicVoiceAudioWaveformStorage);
            Entries.Add(DicomUids.StandaloneModalityLUTStorage.UID, DicomUids.StandaloneModalityLUTStorage);
            Entries.Add(DicomUids.StandaloneVOILUTStorage.UID, DicomUids.StandaloneVOILUTStorage);
            Entries.Add(DicomUids.GrayscaleSoftcopyPresentationStateStorage.UID, DicomUids.GrayscaleSoftcopyPresentationStateStorage);
            Entries.Add(DicomUids.ColorSoftcopyPresentationStateStorage.UID, DicomUids.ColorSoftcopyPresentationStateStorage);
            Entries.Add(DicomUids.PseudoColorSoftcopyPresentationStateStorage.UID, DicomUids.PseudoColorSoftcopyPresentationStateStorage);
            Entries.Add(DicomUids.BlendingSoftcopyPresentationStateStorage.UID, DicomUids.BlendingSoftcopyPresentationStateStorage);
            Entries.Add(DicomUids.XRayAngiographicImageStorage.UID, DicomUids.XRayAngiographicImageStorage);
            Entries.Add(DicomUids.EnhancedXRayAngiographicImageStorage.UID, DicomUids.EnhancedXRayAngiographicImageStorage);
            Entries.Add(DicomUids.XRayRadiofluoroscopicImageStorage.UID, DicomUids.XRayRadiofluoroscopicImageStorage);
            Entries.Add(DicomUids.EnhancedXRayRadiofluoroscopicImageStorage.UID, DicomUids.EnhancedXRayRadiofluoroscopicImageStorage);
            Entries.Add(DicomUids.XRayAngiographicBiPlaneImageStorageRetired.UID, DicomUids.XRayAngiographicBiPlaneImageStorageRetired);
            Entries.Add(DicomUids.NuclearMedicineImageStorage.UID, DicomUids.NuclearMedicineImageStorage);
            Entries.Add(DicomUids.RawDataStorage.UID, DicomUids.RawDataStorage);
            Entries.Add(DicomUids.SpatialRegistrationStorage.UID, DicomUids.SpatialRegistrationStorage);
            Entries.Add(DicomUids.SpatialFiducialsStorage.UID, DicomUids.SpatialFiducialsStorage);
            Entries.Add(DicomUids.RealWorldValueMappingStorage.UID, DicomUids.RealWorldValueMappingStorage);
            Entries.Add(DicomUids.VLImageStorageRetired.UID, DicomUids.VLImageStorageRetired);
            Entries.Add(DicomUids.VLMultiframeImageStorageRetired.UID, DicomUids.VLMultiframeImageStorageRetired);
            Entries.Add(DicomUids.VLEndoscopicImageStorage.UID, DicomUids.VLEndoscopicImageStorage);
            Entries.Add(DicomUids.VLMicroscopicImageStorage.UID, DicomUids.VLMicroscopicImageStorage);
            Entries.Add(DicomUids.VLSlideCoordinatesMicroscopicImageStorage.UID, DicomUids.VLSlideCoordinatesMicroscopicImageStorage);
            Entries.Add(DicomUids.VLPhotographicImageStorage.UID, DicomUids.VLPhotographicImageStorage);
            Entries.Add(DicomUids.VideoEndoscopicImageStorage.UID, DicomUids.VideoEndoscopicImageStorage);
            Entries.Add(DicomUids.VideoMicroscopicImageStorage.UID, DicomUids.VideoMicroscopicImageStorage);
            Entries.Add(DicomUids.VideoPhotographicImageStorage.UID, DicomUids.VideoPhotographicImageStorage);
            Entries.Add(DicomUids.OphthalmicPhotography8BitImageStorage.UID, DicomUids.OphthalmicPhotography8BitImageStorage);
            Entries.Add(DicomUids.OphthalmicPhotography16BitImageStorage.UID, DicomUids.OphthalmicPhotography16BitImageStorage);
            Entries.Add(DicomUids.StereometricRelationshipStorage.UID, DicomUids.StereometricRelationshipStorage);
            Entries.Add(DicomUids.BasicTextSR.UID, DicomUids.BasicTextSR);
            Entries.Add(DicomUids.EnhancedSR.UID, DicomUids.EnhancedSR);
            Entries.Add(DicomUids.ComprehensiveSR.UID, DicomUids.ComprehensiveSR);
            Entries.Add(DicomUids.ProcedureLogStorage.UID, DicomUids.ProcedureLogStorage);
            Entries.Add(DicomUids.MammographyCADSR.UID, DicomUids.MammographyCADSR);
            Entries.Add(DicomUids.KeyObjectSelectionDocument.UID, DicomUids.KeyObjectSelectionDocument);
            Entries.Add(DicomUids.ChestCADSR.UID, DicomUids.ChestCADSR);
            Entries.Add(DicomUids.XRayRadiationDoseSR.UID, DicomUids.XRayRadiationDoseSR);
            Entries.Add(DicomUids.EncapsulatedPDFStorage.UID, DicomUids.EncapsulatedPDFStorage);
            Entries.Add(DicomUids.PositronEmissionTomographyImageStorage.UID, DicomUids.PositronEmissionTomographyImageStorage);
            Entries.Add(DicomUids.StandalonePETCurveStorage.UID, DicomUids.StandalonePETCurveStorage);
            Entries.Add(DicomUids.RTImageStorage.UID, DicomUids.RTImageStorage);
            Entries.Add(DicomUids.RTDoseStorage.UID, DicomUids.RTDoseStorage);
            Entries.Add(DicomUids.RTStructureSetStorage.UID, DicomUids.RTStructureSetStorage);
            Entries.Add(DicomUids.RTBeamsTreatmentRecordStorage.UID, DicomUids.RTBeamsTreatmentRecordStorage);
            Entries.Add(DicomUids.RTPlanStorage.UID, DicomUids.RTPlanStorage);
            Entries.Add(DicomUids.RTBrachyTreatmentRecordStorage.UID, DicomUids.RTBrachyTreatmentRecordStorage);
            Entries.Add(DicomUids.RTTreatmentSummaryRecordStorage.UID, DicomUids.RTTreatmentSummaryRecordStorage);
            Entries.Add(DicomUids.RTIonPlanStorage.UID, DicomUids.RTIonPlanStorage);
            Entries.Add(DicomUids.RTIonBeamsTreatmentRecordStorage.UID, DicomUids.RTIonBeamsTreatmentRecordStorage);
            Entries.Add(DicomUids.PatientRootQueryRetrieveInformationModelFIND.UID, DicomUids.PatientRootQueryRetrieveInformationModelFIND);
            Entries.Add(DicomUids.PatientRootQueryRetrieveInformationModelMOVE.UID, DicomUids.PatientRootQueryRetrieveInformationModelMOVE);
            Entries.Add(DicomUids.PatientRootQueryRetrieveInformationModelGET.UID, DicomUids.PatientRootQueryRetrieveInformationModelGET);
            Entries.Add(DicomUids.StudyRootQueryRetrieveInformationModelFIND.UID, DicomUids.StudyRootQueryRetrieveInformationModelFIND);
            Entries.Add(DicomUids.StudyRootQueryRetrieveInformationModelMOVE.UID, DicomUids.StudyRootQueryRetrieveInformationModelMOVE);
            Entries.Add(DicomUids.StudyRootQueryRetrieveInformationModelGET.UID, DicomUids.StudyRootQueryRetrieveInformationModelGET);
            Entries.Add(DicomUids.PatientStudyOnlyQueryRetrieveInformationModelFIND.UID, DicomUids.PatientStudyOnlyQueryRetrieveInformationModelFIND);
            Entries.Add(DicomUids.PatientStudyOnlyQueryRetrieveInformationModelMOVE.UID, DicomUids.PatientStudyOnlyQueryRetrieveInformationModelMOVE);
            Entries.Add(DicomUids.PatientStudyOnlyQueryRetrieveInformationModelGET.UID, DicomUids.PatientStudyOnlyQueryRetrieveInformationModelGET);
            Entries.Add(DicomUids.ModalityWorklistInformationModelFIND.UID, DicomUids.ModalityWorklistInformationModelFIND);
            Entries.Add(DicomUids.GeneralPurposeWorklistInformationModelFIND.UID, DicomUids.GeneralPurposeWorklistInformationModelFIND);
            Entries.Add(DicomUids.GeneralPurposeScheduledProcedureStepSOPClass.UID, DicomUids.GeneralPurposeScheduledProcedureStepSOPClass);
            Entries.Add(DicomUids.GeneralPurposePerformedProcedureStepSOPClass.UID, DicomUids.GeneralPurposePerformedProcedureStepSOPClass);
            Entries.Add(DicomUids.InstanceAvailabilityNotificationSOPClass.UID, DicomUids.InstanceAvailabilityNotificationSOPClass);
            Entries.Add(DicomUids.PatientInformationQuery.UID, DicomUids.PatientInformationQuery);
            Entries.Add(DicomUids.BreastImagingRelevantPatientInformationQuery.UID, DicomUids.BreastImagingRelevantPatientInformationQuery);
            Entries.Add(DicomUids.CardiacRelevantPatientInformationQuery.UID, DicomUids.CardiacRelevantPatientInformationQuery);
            Entries.Add(DicomUids.HangingProtocolStorage.UID, DicomUids.HangingProtocolStorage);
            Entries.Add(DicomUids.HangingProtocolInformationModelFIND.UID, DicomUids.HangingProtocolInformationModelFIND);
            Entries.Add(DicomUids.HangingProtocolInformationModelMOVE.UID, DicomUids.HangingProtocolInformationModelMOVE);
            Entries.Add(DicomUids.DetachedPatientManagementMetaSOPClass.UID, DicomUids.DetachedPatientManagementMetaSOPClass);
            Entries.Add(DicomUids.DetachedResultsManagementMetaSOPClass.UID, DicomUids.DetachedResultsManagementMetaSOPClass);
            Entries.Add(DicomUids.DetachedStudyManagementMetaSOPClass.UID, DicomUids.DetachedStudyManagementMetaSOPClass);
            Entries.Add(DicomUids.BasicGrayscalePrintManagement.UID, DicomUids.BasicGrayscalePrintManagement);
            Entries.Add(DicomUids.ReferencedGrayscalePrintManagementRetired.UID, DicomUids.ReferencedGrayscalePrintManagementRetired);
            Entries.Add(DicomUids.BasicColorPrintManagement.UID, DicomUids.BasicColorPrintManagement);
            Entries.Add(DicomUids.ReferencedColorPrintManagementRetired.UID, DicomUids.ReferencedColorPrintManagementRetired);
            Entries.Add(DicomUids.PullStoredPrintManagement.UID, DicomUids.PullStoredPrintManagement);
            Entries.Add(DicomUids.GeneralPurposeWorklistManagementMetaSOPClass.UID, DicomUids.GeneralPurposeWorklistManagementMetaSOPClass);
            Entries.Add(DicomUids.StorageCommitmentPushModelSOPInstance.UID, DicomUids.StorageCommitmentPushModelSOPInstance);
            Entries.Add(DicomUids.StorageCommitmentPullModelSOPInstance.UID, DicomUids.StorageCommitmentPullModelSOPInstance);
            Entries.Add(DicomUids.ProceduralEventLoggingSOPInstance.UID, DicomUids.ProceduralEventLoggingSOPInstance);
            Entries.Add(DicomUids.TalairachBrainAtlasFrameOfReference.UID, DicomUids.TalairachBrainAtlasFrameOfReference);
            Entries.Add(DicomUids.SPM2T1FrameOfReference.UID, DicomUids.SPM2T1FrameOfReference);
            Entries.Add(DicomUids.SPM2T2FrameOfReference.UID, DicomUids.SPM2T2FrameOfReference);
            Entries.Add(DicomUids.SPM2PDFrameOfReference.UID, DicomUids.SPM2PDFrameOfReference);
            Entries.Add(DicomUids.SPM2EPIFrameOfReference.UID, DicomUids.SPM2EPIFrameOfReference);
            Entries.Add(DicomUids.SPM2FILT1FrameOfReference.UID, DicomUids.SPM2FILT1FrameOfReference);
            Entries.Add(DicomUids.SPM2PETFrameOfReference.UID, DicomUids.SPM2PETFrameOfReference);
            Entries.Add(DicomUids.SPM2TRANSMFrameOfReference.UID, DicomUids.SPM2TRANSMFrameOfReference);
            Entries.Add(DicomUids.SPM2SPECTFrameOfReference.UID, DicomUids.SPM2SPECTFrameOfReference);
            Entries.Add(DicomUids.SPM2GRAYFrameOfReference.UID, DicomUids.SPM2GRAYFrameOfReference);
            Entries.Add(DicomUids.SPM2WHITEFrameOfReference.UID, DicomUids.SPM2WHITEFrameOfReference);
            Entries.Add(DicomUids.SPM2CSFFrameOfReference.UID, DicomUids.SPM2CSFFrameOfReference);
            Entries.Add(DicomUids.SPM2BRAINMASKFrameOfReference.UID, DicomUids.SPM2BRAINMASKFrameOfReference);
            Entries.Add(DicomUids.SPM2AVG305T1FrameOfReference.UID, DicomUids.SPM2AVG305T1FrameOfReference);
            Entries.Add(DicomUids.SPM2AVG152T1FrameOfReference.UID, DicomUids.SPM2AVG152T1FrameOfReference);
            Entries.Add(DicomUids.SPM2AVG152T2FrameOfReference.UID, DicomUids.SPM2AVG152T2FrameOfReference);
            Entries.Add(DicomUids.SPM2AVG152PDFrameOfReference.UID, DicomUids.SPM2AVG152PDFrameOfReference);
            Entries.Add(DicomUids.SPM2SINGLESUBJT1FrameOfReference.UID, DicomUids.SPM2SINGLESUBJT1FrameOfReference);
            Entries.Add(DicomUids.ICBM452T1FrameOfReference.UID, DicomUids.ICBM452T1FrameOfReference);
            Entries.Add(DicomUids.ICBMSingleSubjectMRIFrameOfReference.UID, DicomUids.ICBMSingleSubjectMRIFrameOfReference);
            Entries.Add(DicomUids.PrinterSOPInstance.UID, DicomUids.PrinterSOPInstance);
            Entries.Add(DicomUids.PrinterConfigurationRetrievalSOPInstance.UID, DicomUids.PrinterConfigurationRetrievalSOPInstance);
            Entries.Add(DicomUids.PrintQueueSOPInstance.UID, DicomUids.PrintQueueSOPInstance);
            Entries.Add(DicomUids.DICOMApplicationContextName.UID, DicomUids.DICOMApplicationContextName);
            Entries.Add(DicomUids.DICOMControlledTerminologyCodingScheme.UID, DicomUids.DICOMControlledTerminologyCodingScheme);
            Entries.Add(DicomUids.UniversalCoordinatedTime.UID, DicomUids.UniversalCoordinatedTime);

            #endregion
        }

        public static DicomUid Lookup(string uid)
        {
            DicomUid o;
            Entries.TryGetValue(uid, out o);
            return o ?? (new DicomUid(uid, "Unknown UID", UidType.Unknown));
        }

        #region Dicom UIDs
        /// <summary>TransferSyntax: Implicit VR Little Endian</summary>
        public static DicomUid ImplicitVRLittleEndian = new DicomUid("1.2.840.10008.1.2", "Implicit VR Little Endian", UidType.TransferSyntax);

        /// <summary>TransferSyntax: Explicit VR Little Endian</summary>
        public static DicomUid ExplicitVRLittleEndian = new DicomUid("1.2.840.10008.1.2.1", "Explicit VR Little Endian", UidType.TransferSyntax);

        /// <summary>TransferSyntax: Deflated Explicit VR Little Endian</summary>
        public static DicomUid DeflatedExplicitVRLittleEndian = new DicomUid("1.2.840.10008.1.2.1.99", "Deflated Explicit VR Little Endian", UidType.TransferSyntax);

        /// <summary>TransferSyntax: Explicit VR Big Endian</summary>
        public static DicomUid ExplicitVRBigEndian = new DicomUid("1.2.840.10008.1.2.2", "Explicit VR Big Endian", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Baseline (Process 1)</summary>
        public static DicomUid JPEGProcess1 = new DicomUid("1.2.840.10008.1.2.4.50", "JPEG Baseline (Process 1)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Extended (Process 2 &amp; 4)</summary>
        public static DicomUid JPEGProcess2_4 = new DicomUid("1.2.840.10008.1.2.4.51", "JPEG Extended (Process 2 & 4)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Extended (Process 3 &amp; 5) (Retired)</summary>
        public static DicomUid JPEGProcess3_5Retired = new DicomUid("1.2.840.10008.1.2.4.52", "JPEG Extended (Process 3 & 5) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Spectral Selection, Non-Hierarchical (Process 6 &amp; 8) (Retired)</summary>
        public static DicomUid JPEGProcess6_8Retired = new DicomUid("1.2.840.10008.1.2.4.53", "JPEG Spectral Selection, Non-Hierarchical (Process 6 & 8) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Spectral Selection, Non-Hierarchical (Process 7 &amp; 9) (Retired)</summary>
        public static DicomUid JPEGProcess7_9Retired = new DicomUid("1.2.840.10008.1.2.4.54", "JPEG Spectral Selection, Non-Hierarchical (Process 7 & 9) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Full Progression, Non-Hierarchical (Process 10 &amp; 12) (Retired)</summary>
        public static DicomUid JPEGProcess10_12Retired = new DicomUid("1.2.840.10008.1.2.4.55", "JPEG Full Progression, Non-Hierarchical (Process 10 & 12) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Full Progression, Non-Hierarchical (Process 11 &amp; 13) (Retired)</summary>
        public static DicomUid JPEGProcess11_13Retired = new DicomUid("1.2.840.10008.1.2.4.56", "JPEG Full Progression, Non-Hierarchical (Process 11 & 13) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Lossless, Non-Hierarchical (Process 14)</summary>
        public static DicomUid JPEGProcess14 = new DicomUid("1.2.840.10008.1.2.4.57", "JPEG Lossless, Non-Hierarchical (Process 14)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Lossless, Non-Hierarchical (Process 15) (Retired)</summary>
        public static DicomUid JPEGProcess15Retired = new DicomUid("1.2.840.10008.1.2.4.58", "JPEG Lossless, Non-Hierarchical (Process 15) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Extended, Hierarchical (Process 16 &amp; 18) (Retired)</summary>
        public static DicomUid JPEGProcess16_18Retired = new DicomUid("1.2.840.10008.1.2.4.59", "JPEG Extended, Hierarchical (Process 16 & 18) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Extended, Hierarchical (Process 17 &amp; 19) (Retired)</summary>
        public static DicomUid JPEGProcess17_19Retired = new DicomUid("1.2.840.10008.1.2.4.60", "JPEG Extended, Hierarchical (Process 17 & 19) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Spectral Selection, Hierarchical (Process 20 &amp; 22) (Retired)</summary>
        public static DicomUid JPEGProcess20_22Retired = new DicomUid("1.2.840.10008.1.2.4.61", "JPEG Spectral Selection, Hierarchical (Process 20 & 22) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Spectral Selection, Hierarchical (Process 21 &amp; 23) (Retired)</summary>
        public static DicomUid JPEGProcess21_23Retired = new DicomUid("1.2.840.10008.1.2.4.62", "JPEG Spectral Selection, Hierarchical (Process 21 & 23) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Full Progression, Hierarchical (Process 24 &amp; 26) (Retired)</summary>
        public static DicomUid JPEGProcess24_26Retired = new DicomUid("1.2.840.10008.1.2.4.63", "JPEG Full Progression, Hierarchical (Process 24 & 26) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Full Progression, Hierarchical (Process 25 &amp; 27) (Retired)</summary>
        public static DicomUid JPEGProcess25_27Retired = new DicomUid("1.2.840.10008.1.2.4.64", "JPEG Full Progression, Hierarchical (Process 25 & 27) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Lossless, Hierarchical (Process 28) (Retired)</summary>
        public static DicomUid JPEGProcess28Retired = new DicomUid("1.2.840.10008.1.2.4.65", "JPEG Lossless, Hierarchical (Process 28) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Lossless, Hierarchical (Process 29) (Retired)</summary>
        public static DicomUid JPEGProcess29Retired = new DicomUid("1.2.840.10008.1.2.4.66", "JPEG Lossless, Hierarchical (Process 29) (Retired)", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1])</summary>
        public static DicomUid JPEGProcess14SV1 = new DicomUid("1.2.840.10008.1.2.4.70", "JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1])", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG-LS Lossless Image Compression</summary>
        public static DicomUid JPEGLSLossless = new DicomUid("1.2.840.10008.1.2.4.80", "JPEG-LS Lossless Image Compression", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG-LS Lossy (Near-Lossless) Image Compression</summary>
        public static DicomUid JPEGLSNearLossless = new DicomUid("1.2.840.10008.1.2.4.81", "JPEG-LS Lossy (Near-Lossless) Image Compression", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG 2000 Lossless Image Compression</summary>
        public static DicomUid JPEG2000Lossless = new DicomUid("1.2.840.10008.1.2.4.90", "JPEG 2000 Lossless Image Compression", UidType.TransferSyntax);

        /// <summary>TransferSyntax: JPEG 2000 Lossy Image Compression</summary>
        public static DicomUid JPEG2000Lossy = new DicomUid("1.2.840.10008.1.2.4.91", "JPEG 2000 Lossy Image Compression", UidType.TransferSyntax);

        /// <summary>TransferSyntax: MPEG2 Main Profile @ Main Level</summary>
        public static DicomUid MPEG2 = new DicomUid("1.2.840.10008.1.2.4.100", "MPEG2 Main Profile @ Main Level", UidType.TransferSyntax);

        /// <summary>TransferSyntax: RLE Lossless</summary>
        public static DicomUid RLELossless = new DicomUid("1.2.840.10008.1.2.5", "RLE Lossless", UidType.TransferSyntax);

        /// <summary>SOPClass: Verification SOP Class</summary>
        public static DicomUid Verification = new DicomUid("1.2.840.10008.1.1", "Verification SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Media Storage Directory Storage</summary>
        public static DicomUid MediaStorageDirectoryStorage = new DicomUid("1.2.840.10008.1.3.10", "Media Storage Directory Storage", UidType.SOPClass);

        /// <summary>SOPClass: Basic Study Content Notification SOP Class</summary>
        public static DicomUid BasicStudyContentNotification = new DicomUid("1.2.840.10008.1.9", "Basic Study Content Notification SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Storage Commitment Push Model SOP Class</summary>
        public static DicomUid StorageCommitmentPushModel = new DicomUid("1.2.840.10008.1.20.1", "Storage Commitment Push Model SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Storage Commitment Pull Model SOP Class</summary>
        public static DicomUid StorageCommitmentPullModel = new DicomUid("1.2.840.10008.1.20.2", "Storage Commitment Pull Model SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Procedural Event Logging SOP Class</summary>
        public static DicomUid ProceduralEventLoggingSOPClass = new DicomUid("1.2.840.10008.1.40", "Procedural Event Logging SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Detached Patient Management SOP Class</summary>
        public static DicomUid DetachedPatientManagement = new DicomUid("1.2.840.10008.3.1.2.1.1", "Detached Patient Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Detached Visit Management SOP Class</summary>
        public static DicomUid DetachedVisitManagement = new DicomUid("1.2.840.10008.3.1.2.2.1", "Detached Visit Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Detached Study Management SOP Class</summary>
        public static DicomUid DetachedStudyManagement = new DicomUid("1.2.840.10008.3.1.2.3.1", "Detached Study Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Study Component Management SOP Class</summary>
        public static DicomUid StudyComponentManagement = new DicomUid("1.2.840.10008.3.1.2.3.2", "Study Component Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Modality Performed Procedure Step SOP Class</summary>
        public static DicomUid ModalityPerformedProcedureStep = new DicomUid("1.2.840.10008.3.1.2.3.3", "Modality Performed Procedure Step SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Modality Performed Procedure Step Retrieve SOP Class</summary>
        public static DicomUid ModalityPerformedProcedureStepRetrieve = new DicomUid("1.2.840.10008.3.1.2.3.4", "Modality Performed Procedure Step Retrieve SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Modality Performed Procedure Step Notification SOP Class</summary>
        public static DicomUid ModalityPerformedProcedureStepNotification = new DicomUid("1.2.840.10008.3.1.2.3.5", "Modality Performed Procedure Step Notification SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Detached Results Management SOP Class</summary>
        public static DicomUid DetachedResultsManagement = new DicomUid("1.2.840.10008.3.1.2.5.1", "Detached Results Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Detached Interpretation Management SOP Class</summary>
        public static DicomUid DetachedInterpretationManagement = new DicomUid("1.2.840.10008.3.1.2.6.1", "Detached Interpretation Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Storage Service Class</summary>
        public static DicomUid StorageServiceClass = new DicomUid("1.2.840.10008.4.2", "Storage Service Class", UidType.SOPClass);

        /// <summary>SOPClass: Basic Film Session SOP Class</summary>
        public static DicomUid BasicFilmSession = new DicomUid("1.2.840.10008.5.1.1.1", "Basic Film Session SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Basic Film Box SOP Class</summary>
        public static DicomUid BasicFilmBoxSOP = new DicomUid("1.2.840.10008.5.1.1.2", "Basic Film Box SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Basic Grayscale Image Box SOP Class</summary>
        public static DicomUid BasicGrayscaleImageBox = new DicomUid("1.2.840.10008.5.1.1.4", "Basic Grayscale Image Box SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Basic Color Image Box SOP Class</summary>
        public static DicomUid BasicColorImageBox = new DicomUid("1.2.840.10008.5.1.1.4.1", "Basic Color Image Box SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Referenced Image Box SOP Class (Retired)</summary>
        public static DicomUid ReferencedImageBoxRetired = new DicomUid("1.2.840.10008.5.1.1.4.2", "Referenced Image Box SOP Class (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: Print Job SOP Class</summary>
        public static DicomUid PrintJob = new DicomUid("1.2.840.10008.5.1.1.14", "Print Job SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Basic Annotation Box SOP Class</summary>
        public static DicomUid BasicAnnotationBox = new DicomUid("1.2.840.10008.5.1.1.15", "Basic Annotation Box SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Printer SOP Class</summary>
        public static DicomUid Printer = new DicomUid("1.2.840.10008.5.1.1.16", "Printer SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Printer Configuration Retrieval SOP Class</summary>
        public static DicomUid PrinterConfigurationRetrieval = new DicomUid("1.2.840.10008.5.1.1.16.376", "Printer Configuration Retrieval SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: VOI LUT Box SOP Class</summary>
        public static DicomUid VOILUTBox = new DicomUid("1.2.840.10008.5.1.1.22", "VOI LUT Box SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Presentation LUT SOP Class</summary>
        public static DicomUid PresentationLUT = new DicomUid("1.2.840.10008.5.1.1.23", "Presentation LUT SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Image Overlay Box SOP Class (Retired)</summary>
        public static DicomUid ImageOverlayBox = new DicomUid("1.2.840.10008.5.1.1.24", "Image Overlay Box SOP Class (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: Basic Print Image Overlay Box SOP Class</summary>
        public static DicomUid BasicPrintImageOverlayBox = new DicomUid("1.2.840.10008.5.1.1.24.1", "Basic Print Image Overlay Box SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Print Queue Management SOP Class</summary>
        public static DicomUid PrintQueueManagement = new DicomUid("1.2.840.10008.5.1.1.26", "Print Queue Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Stored Print Storage SOP Class</summary>
        public static DicomUid StoredPrintStorage = new DicomUid("1.2.840.10008.5.1.1.27", "Stored Print Storage SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Hardcopy Grayscale Image Storage SOP Class</summary>
        public static DicomUid HardcopyGrayscaleImageStorage = new DicomUid("1.2.840.10008.5.1.1.29", "Hardcopy Grayscale Image Storage SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Hardcopy Color Image Storage SOP Class</summary>
        public static DicomUid HardcopyColorImageStorage = new DicomUid("1.2.840.10008.5.1.1.30", "Hardcopy Color Image Storage SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Pull Print Request SOP Class</summary>
        public static DicomUid PullPrintRequest = new DicomUid("1.2.840.10008.5.1.1.31", "Pull Print Request SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Media Creation Management SOP Class</summary>
        public static DicomUid MediaCreationManagementSOPClass = new DicomUid("1.2.840.10008.5.1.1.33", "Media Creation Management SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Computed Radiography Image Storage</summary>
        public static DicomUid ComputedRadiographyImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.1", "Computed Radiography Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Digital X-Ray Image Storage - For Presentation</summary>
        public static DicomUid DigitalXRayImageStorageForPresentation = new DicomUid("1.2.840.10008.5.1.4.1.1.1.1", "Digital X-Ray Image Storage - For Presentation", UidType.SOPClass);

        /// <summary>SOPClass: Digital X-Ray Image Storage - For Processing</summary>
        public static DicomUid DigitalXRayImageStorageForProcessing = new DicomUid("1.2.840.10008.5.1.4.1.1.1.1.1", "Digital X-Ray Image Storage - For Processing", UidType.SOPClass);

        /// <summary>SOPClass: Digital Mammography X-Ray Image Storage - For Presentation</summary>
        public static DicomUid DigitalMammographyXRayImageStorageForPresentation = new DicomUid("1.2.840.10008.5.1.4.1.1.1.2", "Digital Mammography X-Ray Image Storage - For Presentation", UidType.SOPClass);

        /// <summary>SOPClass: Digital Mammography X-Ray Image Storage - For Processing</summary>
        public static DicomUid DigitalMammographyXRayImageStorageForProcessing = new DicomUid("1.2.840.10008.5.1.4.1.1.1.2.1", "Digital Mammography X-Ray Image Storage - For Processing", UidType.SOPClass);

        /// <summary>SOPClass: Digital Intra-oral X-Ray Image Storage - For Presentation</summary>
        public static DicomUid DigitalIntraoralXRayImageStorageForPresentation = new DicomUid("1.2.840.10008.5.1.4.1.1.1.3", "Digital Intra-oral X-Ray Image Storage - For Presentation", UidType.SOPClass);

        /// <summary>SOPClass: Digital Intra-oral X-Ray Image Storage - For Processing</summary>
        public static DicomUid DigitalIntraoralXRayImageStorageForProcessing = new DicomUid("1.2.840.10008.5.1.4.1.1.1.3.1", "Digital Intra-oral X-Ray Image Storage - For Processing", UidType.SOPClass);

        /// <summary>SOPClass: CT Image Storage</summary>
        public static DicomUid CTImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.2", "CT Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Enhanced CT Image Storage</summary>
        public static DicomUid EnhancedCTImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.2.1", "Enhanced CT Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Ultrasound Multi-frame Image Storage (Retired)</summary>
        public static DicomUid UltrasoundMultiframeImageStorageRetired = new DicomUid("1.2.840.10008.5.1.4.1.1.3", "Ultrasound Multi-frame Image Storage (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: Ultrasound Multi-frame Image Storage</summary>
        public static DicomUid UltrasoundMultiframeImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.3.1", "Ultrasound Multi-frame Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: MR Image Storage</summary>
        public static DicomUid MRImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.4", "MR Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Enhanced MR Image Storage</summary>
        public static DicomUid EnhancedMRImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.4.1", "Enhanced MR Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: MR Spectroscopy Storage</summary>
        public static DicomUid MRSpectroscopyStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.4.2", "MR Spectroscopy Storage", UidType.SOPClass);

        /// <summary>SOPClass: Nuclear Medicine Image Storage (Retired)</summary>
        public static DicomUid NuclearMedicineImageStorageRetired = new DicomUid("1.2.840.10008.5.1.4.1.1.5", "Nuclear Medicine Image Storage (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: Ultrasound Image Storage (Retired)</summary>
        public static DicomUid UltrasoundImageStorageRetired = new DicomUid("1.2.840.10008.5.1.4.1.1.6", "Ultrasound Image Storage (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: Ultrasound Image Storage</summary>
        public static DicomUid UltrasoundImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.6.1", "Ultrasound Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Secondary Capture Image Storage</summary>
        public static DicomUid SecondaryCaptureImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.7", "Secondary Capture Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Multi-frame Single Bit Secondary Capture Image Storage</summary>
        public static DicomUid MultiframeSingleBitSecondaryCaptureImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.7.1", "Multi-frame Single Bit Secondary Capture Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Multi-frame Grayscale Byte Secondary Capture Image Storage</summary>
        public static DicomUid MultiframeGrayscaleByteSecondaryCaptureImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.7.2", "Multi-frame Grayscale Byte Secondary Capture Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Multi-frame Grayscale Word Secondary Capture Image Storage</summary>
        public static DicomUid MultiframeGrayscaleWordSecondaryCaptureImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.7.3", "Multi-frame Grayscale Word Secondary Capture Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Multi-frame True Color Secondary Capture Image Storage</summary>
        public static DicomUid MultiframeTrueColorSecondaryCaptureImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.7.4", "Multi-frame True Color Secondary Capture Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Standalone Overlay Storage</summary>
        public static DicomUid StandaloneOverlayStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.8", "Standalone Overlay Storage", UidType.SOPClass);

        /// <summary>SOPClass: Standalone Curve Storage</summary>
        public static DicomUid StandaloneCurveStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.9", "Standalone Curve Storage", UidType.SOPClass);

        /// <summary>SOPClass: 12-lead ECG Waveform Storage</summary>
        public static DicomUid TwelveLeadECGWaveformStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.9.1.1", "12-lead ECG Waveform Storage", UidType.SOPClass);

        /// <summary>SOPClass: General ECG Waveform Storage</summary>
        public static DicomUid GeneralECGWaveformStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.9.1.2", "General ECG Waveform Storage", UidType.SOPClass);

        /// <summary>SOPClass: Ambulatory ECG Waveform Storage</summary>
        public static DicomUid AmbulatoryECGWaveformStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.9.1.3", "Ambulatory ECG Waveform Storage", UidType.SOPClass);

        /// <summary>SOPClass: Hemodynamic Waveform Storage</summary>
        public static DicomUid HemodynamicWaveformStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.9.2.1", "Hemodynamic Waveform Storage", UidType.SOPClass);

        /// <summary>SOPClass: Cardiac Electrophysiology Waveform Storage</summary>
        public static DicomUid CardiacElectrophysiologyWaveformStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.9.3.1", "Cardiac Electrophysiology Waveform Storage", UidType.SOPClass);

        /// <summary>SOPClass: Basic Voice Audio Waveform Storage</summary>
        public static DicomUid BasicVoiceAudioWaveformStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.9.4.1", "Basic Voice Audio Waveform Storage", UidType.SOPClass);

        /// <summary>SOPClass: Standalone Modality LUT Storage</summary>
        public static DicomUid StandaloneModalityLUTStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.10", "Standalone Modality LUT Storage", UidType.SOPClass);

        /// <summary>SOPClass: Standalone VOI LUT Storage</summary>
        public static DicomUid StandaloneVOILUTStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.11", "Standalone VOI LUT Storage", UidType.SOPClass);

        /// <summary>SOPClass: Grayscale Softcopy Presentation State Storage SOP Class</summary>
        public static DicomUid GrayscaleSoftcopyPresentationStateStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.11.1", "Grayscale Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Color Softcopy Presentation State Storage SOP Class</summary>
        public static DicomUid ColorSoftcopyPresentationStateStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.11.2", "Color Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Pseudo-Color Softcopy Presentation State Storage SOP Class</summary>
        public static DicomUid PseudoColorSoftcopyPresentationStateStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.11.3", "Pseudo-Color Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Blending Softcopy Presentation State Storage SOP Class</summary>
        public static DicomUid BlendingSoftcopyPresentationStateStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.11.4", "Blending Softcopy Presentation State Storage SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: X-Ray Angiographic Image Storage</summary>
        public static DicomUid XRayAngiographicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.12.1", "X-Ray Angiographic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Enhanced X-Ray Angiographic Image Storage</summary>
        public static DicomUid EnhancedXRayAngiographicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.12.1.1", "Enhanced X-Ray Angiographic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: X-Ray Radiofluoroscopic Image Storage</summary>
        public static DicomUid XRayRadiofluoroscopicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.12.2", "X-Ray Radiofluoroscopic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Enhanced X-Ray Radiofluoroscopic Image Storage</summary>
        public static DicomUid EnhancedXRayRadiofluoroscopicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.12.2.1", "Enhanced X-Ray Radiofluoroscopic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: X-Ray Angiographic Bi-Plane Image Storage (Retired)</summary>
        public static DicomUid XRayAngiographicBiPlaneImageStorageRetired = new DicomUid("1.2.840.10008.5.1.4.1.1.12.3", "X-Ray Angiographic Bi-Plane Image Storage (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: Nuclear Medicine Image Storage</summary>
        public static DicomUid NuclearMedicineImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.20", "Nuclear Medicine Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Raw Data Storage</summary>
        public static DicomUid RawDataStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.66", "Raw Data Storage", UidType.SOPClass);

        /// <summary>SOPClass: Spatial Registration Storage</summary>
        public static DicomUid SpatialRegistrationStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.66.1", "Spatial Registration Storage", UidType.SOPClass);

        /// <summary>SOPClass: Spatial Fiducials Storage</summary>
        public static DicomUid SpatialFiducialsStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.66.2", "Spatial Fiducials Storage", UidType.SOPClass);

        /// <summary>SOPClass: Real World Value Mapping Storage</summary>
        public static DicomUid RealWorldValueMappingStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.67", "Real World Value Mapping Storage", UidType.SOPClass);

        /// <summary>SOPClass: VL Image Storage (Retired)</summary>
        public static DicomUid VLImageStorageRetired = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1", "VL Image Storage (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: VL Multi-frame Image Storage (Retired)</summary>
        public static DicomUid VLMultiframeImageStorageRetired = new DicomUid("1.2.840.10008.5.1.4.1.1.77.2", "VL Multi-frame Image Storage (Retired)", UidType.SOPClass);

        /// <summary>SOPClass: VL Endoscopic Image Storage</summary>
        public static DicomUid VLEndoscopicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.1", "VL Endoscopic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: VL Microscopic Image Storage</summary>
        public static DicomUid VLMicroscopicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.2", "VL Microscopic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: VL Slide-Coordinates Microscopic Image Storage</summary>
        public static DicomUid VLSlideCoordinatesMicroscopicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.3", "VL Slide-Coordinates Microscopic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: VL Photographic Image Storage</summary>
        public static DicomUid VLPhotographicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.4", "VL Photographic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Video Endoscopic Image Storage</summary>
        public static DicomUid VideoEndoscopicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.1.1", "Video Endoscopic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Video Microscopic Image Storage</summary>
        public static DicomUid VideoMicroscopicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.2.1", "Video Microscopic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Video Photographic Image Storage</summary>
        public static DicomUid VideoPhotographicImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.4.1", "Video Photographic Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Ophthalmic Photography 8 Bit Image Storage</summary>
        public static DicomUid OphthalmicPhotography8BitImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.5.1", "Ophthalmic Photography 8 Bit Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Ophthalmic Photography 16 Bit Image Storage</summary>
        public static DicomUid OphthalmicPhotography16BitImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.5.2", "Ophthalmic Photography 16 Bit Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Stereometric Relationship Storage</summary>
        public static DicomUid StereometricRelationshipStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.77.1.5.3", "Stereometric Relationship Storage", UidType.SOPClass);

        /// <summary>SOPClass: Basic Text SR</summary>
        public static DicomUid BasicTextSR = new DicomUid("1.2.840.10008.5.1.4.1.1.88.11", "Basic Text SR", UidType.SOPClass);

        /// <summary>SOPClass: Enhanced SR</summary>
        public static DicomUid EnhancedSR = new DicomUid("1.2.840.10008.5.1.4.1.1.88.22", "Enhanced SR", UidType.SOPClass);

        /// <summary>SOPClass: Comprehensive SR</summary>
        public static DicomUid ComprehensiveSR = new DicomUid("1.2.840.10008.5.1.4.1.1.88.33", "Comprehensive SR", UidType.SOPClass);

        /// <summary>SOPClass: Procedure Log Storage</summary>
        public static DicomUid ProcedureLogStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.88.40", "Procedure Log Storage", UidType.SOPClass);

        /// <summary>SOPClass: Mammography CAD SR</summary>
        public static DicomUid MammographyCADSR = new DicomUid("1.2.840.10008.5.1.4.1.1.88.50", "Mammography CAD SR", UidType.SOPClass);

        /// <summary>SOPClass: Key Object Selection Document</summary>
        public static DicomUid KeyObjectSelectionDocument = new DicomUid("1.2.840.10008.5.1.4.1.1.88.59", "Key Object Selection Document", UidType.SOPClass);

        /// <summary>SOPClass: Chest CAD SR</summary>
        public static DicomUid ChestCADSR = new DicomUid("1.2.840.10008.5.1.4.1.1.88.65", "Chest CAD SR", UidType.SOPClass);

        /// <summary>SOPClass: X-Ray Radiation Dose SR</summary>
        public static DicomUid XRayRadiationDoseSR = new DicomUid("1.2.840.10008.5.1.4.1.1.88.67", "X-Ray Radiation Dose SR", UidType.SOPClass);

        /// <summary>SOPClass: Encapsulated PDF Storage</summary>
        public static DicomUid EncapsulatedPDFStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.104.1", "Encapsulated PDF Storage", UidType.SOPClass);

        /// <summary>SOPClass: Positron Emission Tomography Image Storage</summary>
        public static DicomUid PositronEmissionTomographyImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.128", "Positron Emission Tomography Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: Standalone PET Curve Storage</summary>
        public static DicomUid StandalonePETCurveStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.129", "Standalone PET Curve Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Image Storage</summary>
        public static DicomUid RTImageStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.1", "RT Image Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Dose Storage</summary>
        public static DicomUid RTDoseStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.2", "RT Dose Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Structure Set Storage</summary>
        public static DicomUid RTStructureSetStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.3", "RT Structure Set Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Beams Treatment Record Storage</summary>
        public static DicomUid RTBeamsTreatmentRecordStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.4", "RT Beams Treatment Record Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Plan Storage</summary>
        public static DicomUid RTPlanStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.5", "RT Plan Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Brachy Treatment Record Storage</summary>
        public static DicomUid RTBrachyTreatmentRecordStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.6", "RT Brachy Treatment Record Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Treatment Summary Record Storage</summary>
        public static DicomUid RTTreatmentSummaryRecordStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.7", "RT Treatment Summary Record Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Ion Plan Storage</summary>
        public static DicomUid RTIonPlanStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.8", "RT Ion Plan Storage", UidType.SOPClass);

        /// <summary>SOPClass: RT Ion Beams Treatment Record Storage</summary>
        public static DicomUid RTIonBeamsTreatmentRecordStorage = new DicomUid("1.2.840.10008.5.1.4.1.1.481.9", "RT Ion Beams Treatment Record Storage", UidType.SOPClass);

        /// <summary>SOPClass: Patient Root Query/Retrieve Information Model - FIND</summary>
        public static DicomUid PatientRootQueryRetrieveInformationModelFIND = new DicomUid("1.2.840.10008.5.1.4.1.2.1.1", "Patient Root Query/Retrieve Information Model - FIND", UidType.SOPClass);

        /// <summary>SOPClass: Patient Root Query/Retrieve Information Model - MOVE</summary>
        public static DicomUid PatientRootQueryRetrieveInformationModelMOVE = new DicomUid("1.2.840.10008.5.1.4.1.2.1.2", "Patient Root Query/Retrieve Information Model - MOVE", UidType.SOPClass);

        /// <summary>SOPClass: Patient Root Query/Retrieve Information Model - GET</summary>
        public static DicomUid PatientRootQueryRetrieveInformationModelGET = new DicomUid("1.2.840.10008.5.1.4.1.2.1.3", "Patient Root Query/Retrieve Information Model - GET", UidType.SOPClass);

        /// <summary>SOPClass: Study Root Query/Retrieve Information Model - FIND</summary>
        public static DicomUid StudyRootQueryRetrieveInformationModelFIND = new DicomUid("1.2.840.10008.5.1.4.1.2.2.1", "Study Root Query/Retrieve Information Model - FIND", UidType.SOPClass);

        /// <summary>SOPClass: Study Root Query/Retrieve Information Model - MOVE</summary>
        public static DicomUid StudyRootQueryRetrieveInformationModelMOVE = new DicomUid("1.2.840.10008.5.1.4.1.2.2.2", "Study Root Query/Retrieve Information Model - MOVE", UidType.SOPClass);

        /// <summary>SOPClass: Study Root Query/Retrieve Information Model - GET</summary>
        public static DicomUid StudyRootQueryRetrieveInformationModelGET = new DicomUid("1.2.840.10008.5.1.4.1.2.2.3", "Study Root Query/Retrieve Information Model - GET", UidType.SOPClass);

        /// <summary>SOPClass: Patient/Study Only Query/Retrieve Information Model - FIND</summary>
        public static DicomUid PatientStudyOnlyQueryRetrieveInformationModelFIND = new DicomUid("1.2.840.10008.5.1.4.1.2.3.1", "Patient/Study Only Query/Retrieve Information Model - FIND", UidType.SOPClass);

        /// <summary>SOPClass: Patient/Study Only Query/Retrieve Information Model - MOVE</summary>
        public static DicomUid PatientStudyOnlyQueryRetrieveInformationModelMOVE = new DicomUid("1.2.840.10008.5.1.4.1.2.3.2", "Patient/Study Only Query/Retrieve Information Model - MOVE", UidType.SOPClass);

        /// <summary>SOPClass: Patient/Study Only Query/Retrieve Information Model - GET</summary>
        public static DicomUid PatientStudyOnlyQueryRetrieveInformationModelGET = new DicomUid("1.2.840.10008.5.1.4.1.2.3.3", "Patient/Study Only Query/Retrieve Information Model - GET", UidType.SOPClass);

        /// <summary>SOPClass: Modality Worklist Information Model - FIND</summary>
        public static DicomUid ModalityWorklistInformationModelFIND = new DicomUid("1.2.840.10008.5.1.4.31", "Modality Worklist Information Model - FIND", UidType.SOPClass);

        /// <summary>SOPClass: General Purpose Worklist Information Model - FIND</summary>
        public static DicomUid GeneralPurposeWorklistInformationModelFIND = new DicomUid("1.2.840.10008.5.1.4.32.1", "General Purpose Worklist Information Model - FIND", UidType.SOPClass);

        /// <summary>SOPClass: General Purpose Scheduled Procedure Step SOP Class</summary>
        public static DicomUid GeneralPurposeScheduledProcedureStepSOPClass = new DicomUid("1.2.840.10008.5.1.4.32.2", "General Purpose Scheduled Procedure Step SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: General Purpose Performed Procedure Step SOP Class</summary>
        public static DicomUid GeneralPurposePerformedProcedureStepSOPClass = new DicomUid("1.2.840.10008.5.1.4.32.3", "General Purpose Performed Procedure Step SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: Instance Availability Notification SOP Class</summary>
        public static DicomUid InstanceAvailabilityNotificationSOPClass = new DicomUid("1.2.840.10008.5.1.4.33", "Instance Availability Notification SOP Class", UidType.SOPClass);

        /// <summary>SOPClass: General Relevant Patient Information Query General Relevant</summary>
        public static DicomUid PatientInformationQuery = new DicomUid("1.2.840.10008.5.1.4.37.1", "General Relevant Patient Information Query General Relevant", UidType.SOPClass);

        /// <summary>SOPClass: Breast Imaging Relevant Patient Information Query</summary>
        public static DicomUid BreastImagingRelevantPatientInformationQuery = new DicomUid("1.2.840.10008.5.1.4.37.2", "Breast Imaging Relevant Patient Information Query", UidType.SOPClass);

        /// <summary>SOPClass: Cardiac Relevant Patient Information Query</summary>
        public static DicomUid CardiacRelevantPatientInformationQuery = new DicomUid("1.2.840.10008.5.1.4.37.3", "Cardiac Relevant Patient Information Query", UidType.SOPClass);

        /// <summary>SOPClass: Hanging Protocol Storage</summary>
        public static DicomUid HangingProtocolStorage = new DicomUid("1.2.840.10008.5.1.4.38.1", "Hanging Protocol Storage", UidType.SOPClass);

        /// <summary>SOPClass: Hanging Protocol Information Model - FIND</summary>
        public static DicomUid HangingProtocolInformationModelFIND = new DicomUid("1.2.840.10008.5.1.4.38.2", "Hanging Protocol Information Model - FIND", UidType.SOPClass);

        /// <summary>SOPClass: Hanging Protocol Information Model - MOVE</summary>
        public static DicomUid HangingProtocolInformationModelMOVE = new DicomUid("1.2.840.10008.5.1.4.38.3", "Hanging Protocol Information Model - MOVE", UidType.SOPClass);

        /// <summary>MetaSOPClass: Detached Patient Management Meta SOP Class</summary>
        public static DicomUid DetachedPatientManagementMetaSOPClass = new DicomUid("1.2.840.10008.3.1.2.1.4", "Detached Patient Management Meta SOP Class", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: Detached Results Management Meta SOP Class</summary>
        public static DicomUid DetachedResultsManagementMetaSOPClass = new DicomUid("1.2.840.10008.3.1.2.5.4", "Detached Results Management Meta SOP Class", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: Detached Study Management Meta SOP Class</summary>
        public static DicomUid DetachedStudyManagementMetaSOPClass = new DicomUid("1.2.840.10008.3.1.2.5.5", "Detached Study Management Meta SOP Class", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: Basic Grayscale Print Management Meta SOP Class</summary>
        public static DicomUid BasicGrayscalePrintManagement = new DicomUid("1.2.840.10008.5.1.1.9", "Basic Grayscale Print Management Meta SOP Class", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: Referenced Grayscale Print Management Meta SOP Class (Retired)</summary>
        public static DicomUid ReferencedGrayscalePrintManagementRetired = new DicomUid("1.2.840.10008.5.1.1.9.1", "Referenced Grayscale Print Management Meta SOP Class (Retired)", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: Basic Color Print Management Meta SOP Class</summary>
        public static DicomUid BasicColorPrintManagement = new DicomUid("1.2.840.10008.5.1.1.18", "Basic Color Print Management Meta SOP Class", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: Referenced Color Print Management Meta SOP Class (Retired)</summary>
        public static DicomUid ReferencedColorPrintManagementRetired = new DicomUid("1.2.840.10008.5.1.1.18.1", "Referenced Color Print Management Meta SOP Class (Retired)", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: Pull Stored Print Management Meta SOP Class</summary>
        public static DicomUid PullStoredPrintManagement = new DicomUid("1.2.840.10008.5.1.1.32", "Pull Stored Print Management Meta SOP Class", UidType.MetaSOPClass);

        /// <summary>MetaSOPClass: General Purpose Worklist Management Meta SOP Class</summary>
        public static DicomUid GeneralPurposeWorklistManagementMetaSOPClass = new DicomUid("1.2.840.10008.5.1.4.32", "General Purpose Worklist Management Meta SOP Class", UidType.MetaSOPClass);

        /// <summary>SOPInstance: Storage Commitment Push Model SOP Instance</summary>
        public static DicomUid StorageCommitmentPushModelSOPInstance = new DicomUid("1.2.840.10008.1.20.1.1", "Storage Commitment Push Model SOP Instance", UidType.SOPInstance);

        /// <summary>SOPInstance: Storage Commitment Pull Model SOP Instance</summary>
        public static DicomUid StorageCommitmentPullModelSOPInstance = new DicomUid("1.2.840.10008.1.20.2.1", "Storage Commitment Pull Model SOP Instance", UidType.SOPInstance);

        /// <summary>SOPInstance: Procedural Event Logging SOP Instance</summary>
        public static DicomUid ProceduralEventLoggingSOPInstance = new DicomUid("1.2.840.10008.1.40.1", "Procedural Event Logging SOP Instance", UidType.SOPInstance);

        /// <summary>SOPInstance: Talairach Brain Atlas Frame of Reference</summary>
        public static DicomUid TalairachBrainAtlasFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.1", "Talairach Brain Atlas Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 T1 Frame of Reference</summary>
        public static DicomUid SPM2T1FrameOfReference = new DicomUid("1.2.840.10008.1.4.1.2", "SPM2 T1 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 T2 Frame of Reference</summary>
        public static DicomUid SPM2T2FrameOfReference = new DicomUid("1.2.840.10008.1.4.1.3", "SPM2 T2 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 PD Frame of Reference</summary>
        public static DicomUid SPM2PDFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.4", "SPM2 PD Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 EPI Frame of Reference</summary>
        public static DicomUid SPM2EPIFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.5", "SPM2 EPI Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 FIL T1 Frame of Reference</summary>
        public static DicomUid SPM2FILT1FrameOfReference = new DicomUid("1.2.840.10008.1.4.1.6", "SPM2 FIL T1 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 PET Frame of Reference</summary>
        public static DicomUid SPM2PETFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.7", "SPM2 PET Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 TRANSM Frame of Reference</summary>
        public static DicomUid SPM2TRANSMFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.8", "SPM2 TRANSM Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 SPECT Frame of Reference</summary>
        public static DicomUid SPM2SPECTFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.9", "SPM2 SPECT Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 GRAY Frame of Reference</summary>
        public static DicomUid SPM2GRAYFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.10", "SPM2 GRAY Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 WHITE Frame of Reference</summary>
        public static DicomUid SPM2WHITEFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.11", "SPM2 WHITE Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 CSF Frame of Reference</summary>
        public static DicomUid SPM2CSFFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.12", "SPM2 CSF Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 BRAINMASK Frame of Reference</summary>
        public static DicomUid SPM2BRAINMASKFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.13", "SPM2 BRAINMASK Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 AVG305T1 Frame of Reference</summary>
        public static DicomUid SPM2AVG305T1FrameOfReference = new DicomUid("1.2.840.10008.1.4.1.14", "SPM2 AVG305T1 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 AVG152T1 Frame of Reference</summary>
        public static DicomUid SPM2AVG152T1FrameOfReference = new DicomUid("1.2.840.10008.1.4.1.15", "SPM2 AVG152T1 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 AVG152T2 Frame of Reference</summary>
        public static DicomUid SPM2AVG152T2FrameOfReference = new DicomUid("1.2.840.10008.1.4.1.16", "SPM2 AVG152T2 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 AVG152PD Frame of Reference</summary>
        public static DicomUid SPM2AVG152PDFrameOfReference = new DicomUid("1.2.840.10008.1.4.1.17", "SPM2 AVG152PD Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: SPM2 SINGLESUBJT1 Frame of Reference</summary>
        public static DicomUid SPM2SINGLESUBJT1FrameOfReference = new DicomUid("1.2.840.10008.1.4.1.18", "SPM2 SINGLESUBJT1 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: ICBM 452 T1 Frame of Reference</summary>
        public static DicomUid ICBM452T1FrameOfReference = new DicomUid("1.2.840.10008.1.4.2.1", "ICBM 452 T1 Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: ICBM Single Subject MRI Frame of Reference</summary>
        public static DicomUid ICBMSingleSubjectMRIFrameOfReference = new DicomUid("1.2.840.10008.1.4.2.2", "ICBM Single Subject MRI Frame of Reference", UidType.SOPInstance);

        /// <summary>SOPInstance: Printer SOP Instance</summary>
        public static DicomUid PrinterSOPInstance = new DicomUid("1.2.840.10008.5.1.1.17", "Printer SOP Instance", UidType.SOPInstance);

        /// <summary>SOPInstance: Printer Configuration Retrieval SOP Instance</summary>
        public static DicomUid PrinterConfigurationRetrievalSOPInstance = new DicomUid("1.2.840.10008.5.1.1.17.376", "Printer Configuration Retrieval SOP Instance", UidType.SOPInstance);

        /// <summary>SOPInstance: Print Queue SOP Instance</summary>
        public static DicomUid PrintQueueSOPInstance = new DicomUid("1.2.840.10008.5.1.1.25", "Print Queue SOP Instance", UidType.SOPInstance);

        /// <summary>ApplicationContextName: DICOM Application Context Name</summary>
        public static DicomUid DICOMApplicationContextName = new DicomUid("1.2.840.10008.3.1.1.1", "DICOM Application Context Name", UidType.ApplicationContextName);

        /// <summary>CodingScheme: DICOM Controlled Terminology Coding Scheme</summary>
        public static DicomUid DICOMControlledTerminologyCodingScheme = new DicomUid("1.2.840.10008.2.16.4", "DICOM Controlled Terminology Coding Scheme", UidType.CodingScheme);

        /// <summary>SynchronizationFrameOfReference: Universal Coordinated Time</summary>
        public static DicomUid UniversalCoordinatedTime = new DicomUid("1.2.840.10008.15.1.1", "Universal Coordinated Time", UidType.SynchronizationFrameOfReference);
        #endregion
    }
    
}
