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
using System.Net.NetworkInformation;
using System.Text;
using JetBrains.Annotations;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Enumerated value for the various types of DICOM UIDs.
	/// </summary>
	public enum UidType
	{
		// ReSharper disable InconsistentNaming

		TransferSyntax,
		SOPClass,
		MetaSOPClass,
		SOPInstance,
		ApplicationContextName,
		CodingScheme,
		SynchronizationFrameOfReference,
		Unknown

		// ReSharper restore InconsistentNaming
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
		private static readonly Object _lock = new object();
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
		[UsedImplicitly]
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
					sb.AppendFormat(".{0}", (uint) Process.GetCurrentProcess().Id);

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

				return new DicomUid(uid.ToString(), "Instance UID", UidType.SOPInstance);
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
		///   The GUID is converted to an integer and displayed in base 10, which can be up to 39 characters long.
		///   </description>
		/// </item>
		/// </list>
		/// <para>
		/// The UID generator uses the above components to ensure uniqueness.  It simply converts a GUID acquired by a 
		/// call to <see cref="Guid.NewGuid"/> into an integer and appends it to the UID for uniqueness.
		/// </para>
		/// </remarks>
		/// <returns></returns>
		public static DicomUid GenerateUid()
		{
			return new DicomUid("2.25." + FormatGuidAsString(Guid.NewGuid()), "Instance UID", UidType.SOPInstance);
		}

#if UNIT_TESTS
		internal static string ConvertGuid(Guid guid)
		{
			return FormatGuidAsString(guid);
		}
#endif

		/// <summary>
		/// Converts the 128-bits of a GUID into a byte stream of 4x 32-bit words, respecting the system endianess so that
		/// the MSB of the GUID is the MSB of the first word, and LSB of the GUID is the LSB of the last word.
		/// </summary>
		private static byte[] GuidToSystemEndianBytes(Guid guid)
		{
			var bytes = guid.ToByteArray();

			// our conversion algorithm uses 4x 32-bit unsigned ints in most-to-least significant word order
			//   (4 system endian) (4 system endian) (4 system endian) (4 system endian)
			// but .NET GUIDs are broken up into parts and separately encoded with first 3 in system endian and last 2 in big endian
			//   (4 system endian)-(2 system endian)-(2 system endian)-(2 big endian)-(6 big endian)
			//
			// if system is little endian, we byte-swap here to build the 4 little endian words in the correct order
			//
			// if system is big endian, the bytes are already big endian and most-to-least significant word order
			// so no swapping is necessary (and all the calculations will be done in big endian anyway)
			if (BitConverter.IsLittleEndian)
			{
				var t = bytes[4];
				bytes[4] = bytes[6];
				bytes[6] = t;

				t = bytes[5];
				bytes[5] = bytes[7];
				bytes[7] = t;

				t = bytes[8];
				bytes[8] = bytes[11];
				bytes[11] = t;

				t = bytes[9];
				bytes[9] = bytes[10];
				bytes[10] = t;

				t = bytes[12];
				bytes[12] = bytes[15];
				bytes[15] = t;

				t = bytes[13];
				bytes[13] = bytes[14];
				bytes[14] = t;
			}

			return bytes;
		}

		/// <summary>
		/// Formats a GUID as a big decimal string of digits.
		/// </summary>
		private static unsafe string FormatGuidAsString(Guid guid)
		{
			// the conversion is based on the pen-and-paper algorithm for converting between bases:
			// 1. divide input by base, remainder is least significant output digit.
			// 2. divide quotient by base, remainder is next least significant output digit.
			// 3. repeat until quotient is zero.
			//
			// however, we are unable to write normal arirthmetic operations here because the operands are 128-bits, and we can do at best 64-bits!
			// the solution is to do "long division", i.e. division in smaller, manageable units, and carry over the remainder to lower places
			// we choose to break the number into 32-bits at a time, so that we can use 64-bit arirthmetic to accomodate each part plus the carry over
			// put another way, we are "rewriting" the 128-bit number as a 4-digit base 2^32 number
			//
			// thus, our long division algorithm now looks like this:
			// A. divide the most significant word by base, quotient is most significant word of overall quotient
			// B. divide the number (remainder * word base + next most significant word) by base, quotient is next most significant word of overall quotient
			// C. repeat until least significant word, where the remainder is simply the overall remainder
			//
			// the resulting algorithm is perhaps more convoluted than if we were to divide one byte at a time, but this does make it more efficient since
			// we are leveraging the CPU's native 32/64-bit binary base for arithmetic
			//
			// NOTE: The simpler algorithm for converting between bases is to start from the most significant digit, multiply by base, add next most
			// significant digit, multiply all that by base, and so on until you finish the sequence of digits. This does not work for us, because the
			// result of each stage is exponentially increasing, and you will quickly exceed the capabilities of native CPU arithmetic.

			// convert the 128-bits of the GUID to 4x 32-bit unsigned ints
			var bytes = GuidToSystemEndianBytes(guid);

			// allocate space for the string - we know it's at most 39 decimal digits
			const int maxDigits = 39;
			var chars = new char[maxDigits];

			fixed (char* pChars = chars)
			fixed (byte* pBytes = bytes)
			{
				// algorithm produces least significant digit first, so we set digits from the end of the string, and keep track of how many digits
				var countDigits = 0;
				var pC = &pChars[maxDigits - 1];

				// casts the bytes to a uint pointer, since we've rearranged the GUID as such
				var pB = (uint*) pBytes;
				do
				{
					// take the first word, divide by 10, and keep the quotient for the next round of calculations
					ulong r = pB[0];
					ulong q = r/10;
					pB[0] = (uint) q;

					// the remainder (i.e. r - q*10) is prepended to the second word as the most significant digit
					// and then divide by 10, and keep the quotient for the next round of calculations
					r = ((r - q*10) << 32) + pB[1];
					q = r/10;
					pB[1] = (uint) q;

					// the remainder is prepended to the third word as the most significant digit
					// and then divide by 10, and keep the quotient for the next round of calculations
					r = ((r - q*10) << 32) + pB[2];
					q = r/10;
					pB[2] = (uint) q;

					// the remainder is prepended to the fourth word as the most significant digit
					// and then divide by 10, and keep the quotient for the next round of calculations
					r = ((r - q*10) << 32) + pB[3];
					q = r/10;
					pB[3] = (uint) q;

					// the remainder is the next decimal digit in the result
					r = r - q*10;

					// the digits are yielded from least to most significant, so we fill the char array from the end
					*pC-- = (char) ('0' + r); // '0'+r being a way of converting a number between 0 and 9 to the equivalent character '0' to '9'

					// and keep track of how many digits that is
					++countDigits;

					// when the dividend for the next round of calculations is 0 (i.e. all words are 0), we are done
				} while (pB[0] != 0 || pB[1] != 0 || pB[2] != 0 || pB[3] != 0);

				// now return a string based on the pointer and offset based on number of digits we actually produced
				// note that the loop always produces at least one digit, even if that is '0'
				return new string(pChars, maxDigits - countDigits, countDigits);
			}
		}

		#endregion
	}

	public static class DicomUids
	{
		public static Dictionary<string, DicomUid> Entries = new Dictionary<string, DicomUid>();

		static DicomUids()
		{
			#region Load Internal UIDs

			Entries.Add(ImplicitVRLittleEndian.UID, ImplicitVRLittleEndian);
			Entries.Add(ExplicitVRLittleEndian.UID, ExplicitVRLittleEndian);
			Entries.Add(DeflatedExplicitVRLittleEndian.UID, DeflatedExplicitVRLittleEndian);
			Entries.Add(ExplicitVRBigEndian.UID, ExplicitVRBigEndian);
			Entries.Add(JPEGProcess1.UID, JPEGProcess1);
			Entries.Add(JPEGProcess2_4.UID, JPEGProcess2_4);
			Entries.Add(JPEGProcess3_5Retired.UID, JPEGProcess3_5Retired);
			Entries.Add(JPEGProcess6_8Retired.UID, JPEGProcess6_8Retired);
			Entries.Add(JPEGProcess7_9Retired.UID, JPEGProcess7_9Retired);
			Entries.Add(JPEGProcess10_12Retired.UID, JPEGProcess10_12Retired);
			Entries.Add(JPEGProcess11_13Retired.UID, JPEGProcess11_13Retired);
			Entries.Add(JPEGProcess14.UID, JPEGProcess14);
			Entries.Add(JPEGProcess15Retired.UID, JPEGProcess15Retired);
			Entries.Add(JPEGProcess16_18Retired.UID, JPEGProcess16_18Retired);
			Entries.Add(JPEGProcess17_19Retired.UID, JPEGProcess17_19Retired);
			Entries.Add(JPEGProcess20_22Retired.UID, JPEGProcess20_22Retired);
			Entries.Add(JPEGProcess21_23Retired.UID, JPEGProcess21_23Retired);
			Entries.Add(JPEGProcess24_26Retired.UID, JPEGProcess24_26Retired);
			Entries.Add(JPEGProcess25_27Retired.UID, JPEGProcess25_27Retired);
			Entries.Add(JPEGProcess28Retired.UID, JPEGProcess28Retired);
			Entries.Add(JPEGProcess29Retired.UID, JPEGProcess29Retired);
			Entries.Add(JPEGProcess14SV1.UID, JPEGProcess14SV1);
			Entries.Add(JPEGLSLossless.UID, JPEGLSLossless);
			Entries.Add(JPEGLSNearLossless.UID, JPEGLSNearLossless);
			Entries.Add(JPEG2000Lossless.UID, JPEG2000Lossless);
			Entries.Add(JPEG2000Lossy.UID, JPEG2000Lossy);
			Entries.Add(MPEG2.UID, MPEG2);
			Entries.Add(RLELossless.UID, RLELossless);
			Entries.Add(Verification.UID, Verification);
			Entries.Add(MediaStorageDirectoryStorage.UID, MediaStorageDirectoryStorage);
			Entries.Add(BasicStudyContentNotification.UID, BasicStudyContentNotification);
			Entries.Add(StorageCommitmentPushModel.UID, StorageCommitmentPushModel);
			Entries.Add(StorageCommitmentPullModel.UID, StorageCommitmentPullModel);
			Entries.Add(ProceduralEventLoggingSOPClass.UID, ProceduralEventLoggingSOPClass);
			Entries.Add(DetachedPatientManagement.UID, DetachedPatientManagement);
			Entries.Add(DetachedVisitManagement.UID, DetachedVisitManagement);
			Entries.Add(DetachedStudyManagement.UID, DetachedStudyManagement);
			Entries.Add(StudyComponentManagement.UID, StudyComponentManagement);
			Entries.Add(ModalityPerformedProcedureStep.UID, ModalityPerformedProcedureStep);
			Entries.Add(ModalityPerformedProcedureStepRetrieve.UID, ModalityPerformedProcedureStepRetrieve);
			Entries.Add(ModalityPerformedProcedureStepNotification.UID, ModalityPerformedProcedureStepNotification);
			Entries.Add(DetachedResultsManagement.UID, DetachedResultsManagement);
			Entries.Add(DetachedInterpretationManagement.UID, DetachedInterpretationManagement);
			Entries.Add(StorageServiceClass.UID, StorageServiceClass);
			Entries.Add(BasicFilmSession.UID, BasicFilmSession);
			Entries.Add(BasicFilmBoxSOP.UID, BasicFilmBoxSOP);
			Entries.Add(BasicGrayscaleImageBox.UID, BasicGrayscaleImageBox);
			Entries.Add(BasicColorImageBox.UID, BasicColorImageBox);
			Entries.Add(ReferencedImageBoxRetired.UID, ReferencedImageBoxRetired);
			Entries.Add(PrintJob.UID, PrintJob);
			Entries.Add(BasicAnnotationBox.UID, BasicAnnotationBox);
			Entries.Add(Printer.UID, Printer);
			Entries.Add(PrinterConfigurationRetrieval.UID, PrinterConfigurationRetrieval);
			Entries.Add(VOILUTBox.UID, VOILUTBox);
			Entries.Add(PresentationLUT.UID, PresentationLUT);
			Entries.Add(ImageOverlayBox.UID, ImageOverlayBox);
			Entries.Add(BasicPrintImageOverlayBox.UID, BasicPrintImageOverlayBox);
			Entries.Add(PrintQueueManagement.UID, PrintQueueManagement);
			Entries.Add(StoredPrintStorage.UID, StoredPrintStorage);
			Entries.Add(HardcopyGrayscaleImageStorage.UID, HardcopyGrayscaleImageStorage);
			Entries.Add(HardcopyColorImageStorage.UID, HardcopyColorImageStorage);
			Entries.Add(PullPrintRequest.UID, PullPrintRequest);
			Entries.Add(MediaCreationManagementSOPClass.UID, MediaCreationManagementSOPClass);
			Entries.Add(ComputedRadiographyImageStorage.UID, ComputedRadiographyImageStorage);
			Entries.Add(DigitalXRayImageStorageForPresentation.UID, DigitalXRayImageStorageForPresentation);
			Entries.Add(DigitalXRayImageStorageForProcessing.UID, DigitalXRayImageStorageForProcessing);
			Entries.Add(DigitalMammographyXRayImageStorageForPresentation.UID, DigitalMammographyXRayImageStorageForPresentation);
			Entries.Add(DigitalMammographyXRayImageStorageForProcessing.UID, DigitalMammographyXRayImageStorageForProcessing);
			Entries.Add(DigitalIntraoralXRayImageStorageForPresentation.UID, DigitalIntraoralXRayImageStorageForPresentation);
			Entries.Add(DigitalIntraoralXRayImageStorageForProcessing.UID, DigitalIntraoralXRayImageStorageForProcessing);
			Entries.Add(CTImageStorage.UID, CTImageStorage);
			Entries.Add(EnhancedCTImageStorage.UID, EnhancedCTImageStorage);
			Entries.Add(UltrasoundMultiframeImageStorageRetired.UID, UltrasoundMultiframeImageStorageRetired);
			Entries.Add(UltrasoundMultiframeImageStorage.UID, UltrasoundMultiframeImageStorage);
			Entries.Add(MRImageStorage.UID, MRImageStorage);
			Entries.Add(EnhancedMRImageStorage.UID, EnhancedMRImageStorage);
			Entries.Add(MRSpectroscopyStorage.UID, MRSpectroscopyStorage);
			Entries.Add(NuclearMedicineImageStorageRetired.UID, NuclearMedicineImageStorageRetired);
			Entries.Add(UltrasoundImageStorageRetired.UID, UltrasoundImageStorageRetired);
			Entries.Add(UltrasoundImageStorage.UID, UltrasoundImageStorage);
			Entries.Add(SecondaryCaptureImageStorage.UID, SecondaryCaptureImageStorage);
			Entries.Add(MultiframeSingleBitSecondaryCaptureImageStorage.UID, MultiframeSingleBitSecondaryCaptureImageStorage);
			Entries.Add(MultiframeGrayscaleByteSecondaryCaptureImageStorage.UID, MultiframeGrayscaleByteSecondaryCaptureImageStorage);
			Entries.Add(MultiframeGrayscaleWordSecondaryCaptureImageStorage.UID, MultiframeGrayscaleWordSecondaryCaptureImageStorage);
			Entries.Add(MultiframeTrueColorSecondaryCaptureImageStorage.UID, MultiframeTrueColorSecondaryCaptureImageStorage);
			Entries.Add(StandaloneOverlayStorage.UID, StandaloneOverlayStorage);
			Entries.Add(StandaloneCurveStorage.UID, StandaloneCurveStorage);
			Entries.Add(TwelveLeadECGWaveformStorage.UID, TwelveLeadECGWaveformStorage);
			Entries.Add(GeneralECGWaveformStorage.UID, GeneralECGWaveformStorage);
			Entries.Add(AmbulatoryECGWaveformStorage.UID, AmbulatoryECGWaveformStorage);
			Entries.Add(HemodynamicWaveformStorage.UID, HemodynamicWaveformStorage);
			Entries.Add(CardiacElectrophysiologyWaveformStorage.UID, CardiacElectrophysiologyWaveformStorage);
			Entries.Add(BasicVoiceAudioWaveformStorage.UID, BasicVoiceAudioWaveformStorage);
			Entries.Add(StandaloneModalityLUTStorage.UID, StandaloneModalityLUTStorage);
			Entries.Add(StandaloneVOILUTStorage.UID, StandaloneVOILUTStorage);
			Entries.Add(GrayscaleSoftcopyPresentationStateStorage.UID, GrayscaleSoftcopyPresentationStateStorage);
			Entries.Add(ColorSoftcopyPresentationStateStorage.UID, ColorSoftcopyPresentationStateStorage);
			Entries.Add(PseudoColorSoftcopyPresentationStateStorage.UID, PseudoColorSoftcopyPresentationStateStorage);
			Entries.Add(BlendingSoftcopyPresentationStateStorage.UID, BlendingSoftcopyPresentationStateStorage);
			Entries.Add(XRayAngiographicImageStorage.UID, XRayAngiographicImageStorage);
			Entries.Add(EnhancedXRayAngiographicImageStorage.UID, EnhancedXRayAngiographicImageStorage);
			Entries.Add(XRayRadiofluoroscopicImageStorage.UID, XRayRadiofluoroscopicImageStorage);
			Entries.Add(EnhancedXRayRadiofluoroscopicImageStorage.UID, EnhancedXRayRadiofluoroscopicImageStorage);
			Entries.Add(XRayAngiographicBiPlaneImageStorageRetired.UID, XRayAngiographicBiPlaneImageStorageRetired);
			Entries.Add(NuclearMedicineImageStorage.UID, NuclearMedicineImageStorage);
			Entries.Add(RawDataStorage.UID, RawDataStorage);
			Entries.Add(SpatialRegistrationStorage.UID, SpatialRegistrationStorage);
			Entries.Add(SpatialFiducialsStorage.UID, SpatialFiducialsStorage);
			Entries.Add(RealWorldValueMappingStorage.UID, RealWorldValueMappingStorage);
			Entries.Add(VLImageStorageRetired.UID, VLImageStorageRetired);
			Entries.Add(VLMultiframeImageStorageRetired.UID, VLMultiframeImageStorageRetired);
			Entries.Add(VLEndoscopicImageStorage.UID, VLEndoscopicImageStorage);
			Entries.Add(VLMicroscopicImageStorage.UID, VLMicroscopicImageStorage);
			Entries.Add(VLSlideCoordinatesMicroscopicImageStorage.UID, VLSlideCoordinatesMicroscopicImageStorage);
			Entries.Add(VLPhotographicImageStorage.UID, VLPhotographicImageStorage);
			Entries.Add(VideoEndoscopicImageStorage.UID, VideoEndoscopicImageStorage);
			Entries.Add(VideoMicroscopicImageStorage.UID, VideoMicroscopicImageStorage);
			Entries.Add(VideoPhotographicImageStorage.UID, VideoPhotographicImageStorage);
			Entries.Add(OphthalmicPhotography8BitImageStorage.UID, OphthalmicPhotography8BitImageStorage);
			Entries.Add(OphthalmicPhotography16BitImageStorage.UID, OphthalmicPhotography16BitImageStorage);
			Entries.Add(StereometricRelationshipStorage.UID, StereometricRelationshipStorage);
			Entries.Add(BasicTextSR.UID, BasicTextSR);
			Entries.Add(EnhancedSR.UID, EnhancedSR);
			Entries.Add(ComprehensiveSR.UID, ComprehensiveSR);
			Entries.Add(ProcedureLogStorage.UID, ProcedureLogStorage);
			Entries.Add(MammographyCADSR.UID, MammographyCADSR);
			Entries.Add(KeyObjectSelectionDocument.UID, KeyObjectSelectionDocument);
			Entries.Add(ChestCADSR.UID, ChestCADSR);
			Entries.Add(XRayRadiationDoseSR.UID, XRayRadiationDoseSR);
			Entries.Add(EncapsulatedPDFStorage.UID, EncapsulatedPDFStorage);
			Entries.Add(PositronEmissionTomographyImageStorage.UID, PositronEmissionTomographyImageStorage);
			Entries.Add(StandalonePETCurveStorage.UID, StandalonePETCurveStorage);
			Entries.Add(RTImageStorage.UID, RTImageStorage);
			Entries.Add(RTDoseStorage.UID, RTDoseStorage);
			Entries.Add(RTStructureSetStorage.UID, RTStructureSetStorage);
			Entries.Add(RTBeamsTreatmentRecordStorage.UID, RTBeamsTreatmentRecordStorage);
			Entries.Add(RTPlanStorage.UID, RTPlanStorage);
			Entries.Add(RTBrachyTreatmentRecordStorage.UID, RTBrachyTreatmentRecordStorage);
			Entries.Add(RTTreatmentSummaryRecordStorage.UID, RTTreatmentSummaryRecordStorage);
			Entries.Add(RTIonPlanStorage.UID, RTIonPlanStorage);
			Entries.Add(RTIonBeamsTreatmentRecordStorage.UID, RTIonBeamsTreatmentRecordStorage);
			Entries.Add(PatientRootQueryRetrieveInformationModelFIND.UID, PatientRootQueryRetrieveInformationModelFIND);
			Entries.Add(PatientRootQueryRetrieveInformationModelMOVE.UID, PatientRootQueryRetrieveInformationModelMOVE);
			Entries.Add(PatientRootQueryRetrieveInformationModelGET.UID, PatientRootQueryRetrieveInformationModelGET);
			Entries.Add(StudyRootQueryRetrieveInformationModelFIND.UID, StudyRootQueryRetrieveInformationModelFIND);
			Entries.Add(StudyRootQueryRetrieveInformationModelMOVE.UID, StudyRootQueryRetrieveInformationModelMOVE);
			Entries.Add(StudyRootQueryRetrieveInformationModelGET.UID, StudyRootQueryRetrieveInformationModelGET);
			Entries.Add(PatientStudyOnlyQueryRetrieveInformationModelFIND.UID, PatientStudyOnlyQueryRetrieveInformationModelFIND);
			Entries.Add(PatientStudyOnlyQueryRetrieveInformationModelMOVE.UID, PatientStudyOnlyQueryRetrieveInformationModelMOVE);
			Entries.Add(PatientStudyOnlyQueryRetrieveInformationModelGET.UID, PatientStudyOnlyQueryRetrieveInformationModelGET);
			Entries.Add(ModalityWorklistInformationModelFIND.UID, ModalityWorklistInformationModelFIND);
			Entries.Add(GeneralPurposeWorklistInformationModelFIND.UID, GeneralPurposeWorklistInformationModelFIND);
			Entries.Add(GeneralPurposeScheduledProcedureStepSOPClass.UID, GeneralPurposeScheduledProcedureStepSOPClass);
			Entries.Add(GeneralPurposePerformedProcedureStepSOPClass.UID, GeneralPurposePerformedProcedureStepSOPClass);
			Entries.Add(InstanceAvailabilityNotificationSOPClass.UID, InstanceAvailabilityNotificationSOPClass);
			Entries.Add(PatientInformationQuery.UID, PatientInformationQuery);
			Entries.Add(BreastImagingRelevantPatientInformationQuery.UID, BreastImagingRelevantPatientInformationQuery);
			Entries.Add(CardiacRelevantPatientInformationQuery.UID, CardiacRelevantPatientInformationQuery);
			Entries.Add(HangingProtocolStorage.UID, HangingProtocolStorage);
			Entries.Add(HangingProtocolInformationModelFIND.UID, HangingProtocolInformationModelFIND);
			Entries.Add(HangingProtocolInformationModelMOVE.UID, HangingProtocolInformationModelMOVE);
			Entries.Add(DetachedPatientManagementMetaSOPClass.UID, DetachedPatientManagementMetaSOPClass);
			Entries.Add(DetachedResultsManagementMetaSOPClass.UID, DetachedResultsManagementMetaSOPClass);
			Entries.Add(DetachedStudyManagementMetaSOPClass.UID, DetachedStudyManagementMetaSOPClass);
			Entries.Add(BasicGrayscalePrintManagement.UID, BasicGrayscalePrintManagement);
			Entries.Add(ReferencedGrayscalePrintManagementRetired.UID, ReferencedGrayscalePrintManagementRetired);
			Entries.Add(BasicColorPrintManagement.UID, BasicColorPrintManagement);
			Entries.Add(ReferencedColorPrintManagementRetired.UID, ReferencedColorPrintManagementRetired);
			Entries.Add(PullStoredPrintManagement.UID, PullStoredPrintManagement);
			Entries.Add(GeneralPurposeWorklistManagementMetaSOPClass.UID, GeneralPurposeWorklistManagementMetaSOPClass);
			Entries.Add(StorageCommitmentPushModelSOPInstance.UID, StorageCommitmentPushModelSOPInstance);
			Entries.Add(StorageCommitmentPullModelSOPInstance.UID, StorageCommitmentPullModelSOPInstance);
			Entries.Add(ProceduralEventLoggingSOPInstance.UID, ProceduralEventLoggingSOPInstance);
			Entries.Add(TalairachBrainAtlasFrameOfReference.UID, TalairachBrainAtlasFrameOfReference);
			Entries.Add(SPM2T1FrameOfReference.UID, SPM2T1FrameOfReference);
			Entries.Add(SPM2T2FrameOfReference.UID, SPM2T2FrameOfReference);
			Entries.Add(SPM2PDFrameOfReference.UID, SPM2PDFrameOfReference);
			Entries.Add(SPM2EPIFrameOfReference.UID, SPM2EPIFrameOfReference);
			Entries.Add(SPM2FILT1FrameOfReference.UID, SPM2FILT1FrameOfReference);
			Entries.Add(SPM2PETFrameOfReference.UID, SPM2PETFrameOfReference);
			Entries.Add(SPM2TRANSMFrameOfReference.UID, SPM2TRANSMFrameOfReference);
			Entries.Add(SPM2SPECTFrameOfReference.UID, SPM2SPECTFrameOfReference);
			Entries.Add(SPM2GRAYFrameOfReference.UID, SPM2GRAYFrameOfReference);
			Entries.Add(SPM2WHITEFrameOfReference.UID, SPM2WHITEFrameOfReference);
			Entries.Add(SPM2CSFFrameOfReference.UID, SPM2CSFFrameOfReference);
			Entries.Add(SPM2BRAINMASKFrameOfReference.UID, SPM2BRAINMASKFrameOfReference);
			Entries.Add(SPM2AVG305T1FrameOfReference.UID, SPM2AVG305T1FrameOfReference);
			Entries.Add(SPM2AVG152T1FrameOfReference.UID, SPM2AVG152T1FrameOfReference);
			Entries.Add(SPM2AVG152T2FrameOfReference.UID, SPM2AVG152T2FrameOfReference);
			Entries.Add(SPM2AVG152PDFrameOfReference.UID, SPM2AVG152PDFrameOfReference);
			Entries.Add(SPM2SINGLESUBJT1FrameOfReference.UID, SPM2SINGLESUBJT1FrameOfReference);
			Entries.Add(ICBM452T1FrameOfReference.UID, ICBM452T1FrameOfReference);
			Entries.Add(ICBMSingleSubjectMRIFrameOfReference.UID, ICBMSingleSubjectMRIFrameOfReference);
			Entries.Add(PrinterSOPInstance.UID, PrinterSOPInstance);
			Entries.Add(PrinterConfigurationRetrievalSOPInstance.UID, PrinterConfigurationRetrievalSOPInstance);
			Entries.Add(PrintQueueSOPInstance.UID, PrintQueueSOPInstance);
			Entries.Add(DICOMApplicationContextName.UID, DICOMApplicationContextName);
			Entries.Add(DICOMControlledTerminologyCodingScheme.UID, DICOMControlledTerminologyCodingScheme);
			Entries.Add(UniversalCoordinatedTime.UID, UniversalCoordinatedTime);

			#endregion
		}

		public static DicomUid Lookup(string uid)
		{
			DicomUid o;
			Entries.TryGetValue(uid, out o);
			return o ?? (new DicomUid(uid, "Unknown UID", UidType.Unknown));
		}

		#region Dicom UIDs

		// ReSharper disable InconsistentNaming

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

		// ReSharper restore InconsistentNaming

		#endregion
	}
}