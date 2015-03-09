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
using System.Globalization;
using System.Linq;

namespace ClearCanvas.Dicom.Audit
{
	public partial class AuditMessage
	{

	}

    public partial class EventID
    {
        public static EventID ApplicationActivity = new EventID("110100", "DCM", "Application Activity", "Application audit event");
        public static EventID AuditLogUsed = new EventID("110101", "DCM", "Audit Log Used", "Audit Log Used audit event");
        public static EventID BeginTransferringDICOMInstances = new EventID("110102", "DCM", "Begin Transferring DICOM Instances", "Begin Storing DICOM Instances audit event");
        public static EventID DICOMInstancesAccessed = new EventID("110103", "DCM", "DICOM Instances Accessed", "DICOM Instances created, read, updated, or deleted audit event");
        public static EventID DICOMInstancesTransferred = new EventID("110104", "DCM", "DICOM Instances Transferred", "Storage of DICOM Instances complete audit event");
        public static EventID DICOMStudyDeleted = new EventID("110105", "DCM", "DICOM Study Deleted", "Deletion of Entire Study audit event");
        public static EventID SecurityAlert = new EventID("110113", "DCM", "Security Alert", "SecurityAlert audit event");
        public static EventID Export = new EventID("110106", "DCM", "Export", "Data exported out of the system audit event");
        public static EventID Import = new EventID("110107", "DCM", "Import", "Data imported into the system audit event");
        public static EventID Query = new EventID("110112", "DCM", "Query", "Query requested audit event");
        public static EventID UserAuthentication = new EventID("110114", "DCM", "User Authentication", "User Authentication audit event");
        public static EventID OrderRecord = new EventID("110109", "DCM", "Order Record", "Order Record audit event");
        public static EventID PatientRecord = new EventID("110110", "DCM", "Patient Record", "Patient Record audit event");
        public static EventID ProcedureRecord = new EventID("110111", "DCM", "Procedure Record", "Procedure Record audit event");
        public static EventID NetworkEntry = new EventID("110108", "DCM", "Network Entry", "Network Entry audit event");
		public static EventID HealthServicesProvisionEvent = new EventID("IHE0001", "IHE", "Health Services Provision Event", "Health services scheduled and performed within an instance or episode of care. This includes scheduling, initiation, updates or amendments, performing or completing the act, and cancellation.");
		public static EventID MedicationEvent = new EventID("IHE0002", "IHE", "Medication Event","Medication orders and administration within an instance or episode of care. This includes initial order, dispensing, delivery, and cancellation.");
		public static EventID PatientCareResourceAssignment = new EventID("IHE0003", "IHE", "Patient Care Resource Assignment", "Staffing or participant assignment actions relevant to the assignment of healthcare professionals, caregivers attending physician, residents, medical students, consultants, etc. to a patient It also includes change in assigned role or authorization, e.g., relative to healthcare status change, and de-assignment.");
		public static EventID PatientCareEpisode = new EventID("IHE0004", "IHE", "Patient Care Episode","Specific patient care episodes or problems that occur within an instance of care. This includes initial assignment, updates or amendments, resolution, completion, and cancellation.");
		public static EventID PatientCareProtocol = new EventID("IHE0005", "IHE", "Patient Care Protocol","Patient association with a care protocol. This includes initial assignment, scheduling, updates or amendments, completion, and cancellation");
		public static EventID CCHealthcareDataEvent = new EventID("CC001","ClearCanvas","Healthcare Data Event", "Event to signal when healthcare data has been updated");
		public static EventID CCHealthcareDataDeleted = new EventID("CC002", "ClearCanvas", "Healthcare Data Deleted", "Event to signal when healthcare data has been deleted");
		public static EventID CCHealthcareDataSearch = new EventID("CC003", "ClearCanvas", "Search Event", "Event to signal when healthcare data has been searched");

		public EventID(string _code, string _codeSystemNameField,
                       string _displayNameField, string description)
        {
            codeSystemName = _codeSystemNameField;
            code = _code;
            displayNameField = _displayNameField;
            _description = description;
        }

        private readonly string _description;

        public EventID()
        {
        }

        public string Description
        {
            get { return String.IsNullOrEmpty(_description) ? String.Empty : _description; }
        }
    }
    public partial class EventTypeCode
    {
        public static EventTypeCode ApplicationStart = new EventTypeCode("110120", "DCM", "Application Start", "Application Entity Started audit event type");
        public static EventTypeCode ApplicationStop = new EventTypeCode("110121", "DCM", "Application Stop", "Application Entity Stopped audit event type");
        public static EventTypeCode Login = new EventTypeCode("110122", "DCM", "Login", "User login attempted audit event type");
        public static EventTypeCode Logout = new EventTypeCode("110123", "DCM", "Logout", "User logout attempted audit event type");
        public static EventTypeCode Attach = new EventTypeCode("110124", "DCM", "Attach", "Node attaches to a network audit event type");
        public static EventTypeCode Detach = new EventTypeCode("110125", "DCM", "Detach", "Node detaches from a network audit event type");
        public static EventTypeCode NodeAuthentication = new EventTypeCode("110126", "DCM", "Node Authentication", "Node Authentication audit event type");
        public static EventTypeCode EmergencyOverride = new EventTypeCode("110127", "DCM", "Emergency Override", "Emergency Override audit event type");
        public static EventTypeCode NetworkConfiguration = new EventTypeCode("110128", "DCM", "Network Configuration", "Network configuration change audit event type");
        public static EventTypeCode SecurityConfiguration = new EventTypeCode("110129", "DCM", "Security Configuration", "Security configuration change audit event type");
        public static EventTypeCode HardwareConfiguration = new EventTypeCode("110130", "DCM", "Hardware Configuration", "Hardware configuration change audit event type");
        public static EventTypeCode SoftwareConfiguration = new EventTypeCode("110131", "DCM", "Software Configuration", "Software configuration change audit event type");
        public static EventTypeCode UseOfRestrictedFunction = new EventTypeCode("110132", "DCM", "Use of Restricted Function", "A use of a restricted function audit event type");
        public static EventTypeCode AuditRecordingStopped = new EventTypeCode("110133", "DCM", "Audit Recording Stopped", "Audit recording stopped audit event type");
        public static EventTypeCode AuditRecordingStarted = new EventTypeCode("110134", "DCM", "Audit Recording Started", "Audit recording started audit event type");
        public static EventTypeCode ObjectSecurityAttributesChanged = new EventTypeCode("110135", "DCM", "Object Security Attributes Changed", "The security attributes of an object changed audit event type");
        public static EventTypeCode SecurityRolesChanged = new EventTypeCode("110136", "DCM", "Security Roles Changed", "Security roles changed audit event type");
        public static EventTypeCode UserSecurityAttributesChanged = new EventTypeCode("110137", "DCM", "User security Attributes Changed", "The security attributes of a user changed audit event type");

        public EventTypeCode(string _code, string _codeSystemNameField,
                       string _displayNameField, string description)
        {
            codeSystemName = _codeSystemNameField;
            code = _code;
            displayNameField = _displayNameField;
            _description = description;
        }

        private readonly string _description;

        public EventTypeCode()
        {
        }

        public string Description
        {
            get { return String.IsNullOrEmpty(_description) ? String.Empty : _description; }
        }
    }

    public partial class RoleIDCode
    {
        public static RoleIDCode Application = new RoleIDCode("110150", "DCM", "Application", "Audit participant role ID of DICOM application");
        public static RoleIDCode ApplicationLauncher = new RoleIDCode("110151", "DCM", "Application Launcher", "Audit participant role ID of DICOM application launcher, i.e., the entity that started or stopped an application.");
        public static RoleIDCode Destination = new RoleIDCode("110152", "DCM", "Destination Role ID", "Audit participant role ID of the receiver of data");
        public static RoleIDCode Source = new RoleIDCode("110153", "DCM", "Source Role ID", "Audit participant role ID of the sender of data");
        public static RoleIDCode DestinationMedia = new RoleIDCode("110154", "DCM", "Destination Media", "Audit participant role ID of media receiving data during an export.");
        public static RoleIDCode SourceMedia = new RoleIDCode("110155", "DCM", "Source Media", "Audit participant role ID of media providing data during an import.");
        
        public RoleIDCode(string _code, string _codeSystemNameField,
                          string _displayNameField, string description)
        {
            codeSystemName = _codeSystemNameField;
            code = _code;
            displayNameField = _displayNameField;
            _description = description;
        }

        private readonly string _description;

        public RoleIDCode()
        {
        }

        public string Description
        {
            get { return String.IsNullOrEmpty(_description) ? String.Empty : _description; }
        }
    }

    public partial class ParticipantObjectIDTypeCode
    {
        public static ParticipantObjectIDTypeCode StudyInstanceUID = new ParticipantObjectIDTypeCode("110180", "DCM", "Study Instance UID", "Study Instance UID Participant Object ID type");
        public static ParticipantObjectIDTypeCode ClassUID = new ParticipantObjectIDTypeCode("110181", "DCM", "SOP Class UID", "SOP Class UID Participant Object ID type");
        public static ParticipantObjectIDTypeCode NodeID = new ParticipantObjectIDTypeCode("110182", "DCM", "Node ID", "ID of a node that is a participant object of an audit message");
		public static ParticipantObjectIDTypeCode VfsPath = new ParticipantObjectIDTypeCode("CCPO001","ClearCanvas","VFS Path", "Virtual Filesystem Path of an object that is a participant object of an audit message");

        public ParticipantObjectIDTypeCode(string _code, string _codeSystemNameField,
                          string _displayNameField, string description)
        {
            codeSystemName = _codeSystemNameField;
            code = _code;
            displayNameField = _displayNameField;
            _description = description;
        }

        private readonly string _description;

        public string Description
        {
            get { return String.IsNullOrEmpty(_description) ? String.Empty : _description; }
        }
    }

    public partial class MediaType
    {
        public static MediaType USBDiskEmulation = new MediaType("110030", "DCM", "USB Disk Emulation");
        public static MediaType Email = new MediaType("110031", "DCM", "Email");
        public static MediaType CD = new MediaType("110032", "DCM", "CD");
        public static MediaType DVD = new MediaType("110033", "DCM", "DVD");
        public static MediaType CompactFlash = new MediaType("110034", "DCM", "Compact Flash");
        public static MediaType MultiMediaCard = new MediaType("110035", "DCM", "Multi-media Card");
        public static MediaType SecureDigitalCard = new MediaType("110036", "DCM", "Secure Digital Card");
        public static MediaType URI = new MediaType("110037", "DCM", "URI");
        public static MediaType Film = new MediaType("110010", "DCM", "Film");
        public static MediaType PaperDocument = new MediaType("110038", "DCM", "Paper Document");


        public MediaType()
        {
        }

        public MediaType(string _code, string _codeSystemNameField, string _displayNameField)
        {
            codeSystemName = _codeSystemNameField;
            code = _code;
            displayNameField = _displayNameField;
        }
    }

    public partial class ActiveParticipantContents
	{
		public ActiveParticipantContents()
		{}

        public ActiveParticipantContents(AuditActiveParticipant participant)
		{
			if (participant.RoleIdCode != null)
			{
                RoleIDCode = new RoleIDCode[1];
				RoleIDCode[0] = participant.RoleIdCode;
			}
			if (participant.UserIsRequestor.HasValue)
			{
				UserIsRequestor = participant.UserIsRequestor.Value;
			}
			if (participant.UserId != null)
			{
				UserID = participant.UserId;
			}
			if (participant.AlternateUserId != null)
			{
				AlternativeUserID = participant.AlternateUserId;
			}
			if (participant.UserName != null)
				UserName = participant.UserName;

			if (participant.NetworkAccessPointId != null)
				NetworkAccessPointID = participant.NetworkAccessPointId;

			if (participant.NetworkAccessPointType.HasValue)
			{
				NetworkAccessPointTypeCode = (ActiveParticipantContentsNetworkAccessPointTypeCode) participant.NetworkAccessPointType.Value;
				NetworkAccessPointTypeCodeSpecified = true;
			}
			else
				NetworkAccessPointTypeCodeSpecified = false;
		}

        public ActiveParticipantContents(RoleIDCode roleId, string _userIDField, string alternateUserId, string _userNameField, string _networkAccessPointIDField, NetworkAccessPointTypeEnum? _networkAccessPointTypeCode, bool? userIsRequestor)
		{
			if (roleId != null)
			{
                RoleIDCode = new RoleIDCode[1];
				RoleIDCode[0] = roleId;
			}

			if (userIsRequestor.HasValue)
				UserIsRequestor = userIsRequestor.Value;

			if (_userIDField != null)
				UserID = _userIDField;

			if (alternateUserId != null)
				AlternativeUserID = alternateUserId;

			if (_userNameField != null)
				UserName = _userNameField;

			if (_networkAccessPointIDField != null)
				NetworkAccessPointID = _networkAccessPointIDField;

			if (_networkAccessPointTypeCode.HasValue)
			{
				NetworkAccessPointTypeCode = (ActiveParticipantContentsNetworkAccessPointTypeCode)_networkAccessPointTypeCode.Value;
				NetworkAccessPointTypeCodeSpecified = true;
			}
			else
				NetworkAccessPointTypeCodeSpecified = false;
		}
	}
	public partial class ParticipantObjectIDTypeCode
	{
		public ParticipantObjectIDTypeCode()
		{}
        public ParticipantObjectIDTypeCode(ParticipantObjectIDTypeCode value)
		{
			code = value.code;
			codeSystemName = value.codeSystemName;
			displayName = value.displayName;
			originalText = value.originalText;
		}
	}

	public partial class AuditSourceIdentificationContents
	{
		public AuditSourceIdentificationContents(DicomAuditSource auditSource)
		{
			if (!String.IsNullOrEmpty(auditSource.EnterpriseSiteId))
				AuditEnterpriseSiteID = auditSource.EnterpriseSiteId;
			AuditSourceID = auditSource.AuditSourceId;

			if (auditSource.AuditSourceType.HasValue)
			{
				// Note this was being encoded as the string representation of the enumerated value, and it shoudl be encoded as a numeric value
				code = ((int)auditSource.AuditSourceType.Value).ToString(CultureInfo.InvariantCulture);
			}
		}
        public AuditSourceIdentificationContents(string sourceId)
		{
			AuditSourceID = sourceId;
		}
	}

	public partial class AuditSourceIdentificationContents
	{
		public AuditSourceIdentificationContents()
		{}

        public AuditSourceIdentificationContents(AuditSourceTypeCodeEnum e)
		{
			code = ((byte)e).ToString();
		}
	}

	public partial class ParticipantObjectIDTypeCode
	{
		public ParticipantObjectIDTypeCode(ParticipantObjectIdTypeCodeEnum e)
		{
			code = ((byte)e).ToString();
		}
	}

	public partial class ParticipantObjectIdentificationContents
	{
		public ParticipantObjectIdentificationContents()
		{}

		public ParticipantObjectIdentificationContents(ParticipantObjectTypeCodeEnum? type, 
			ParticipantObjectTypeCodeRoleEnum? role,
			ParticipantObjectDataLifeCycleEnum? lifeCycle, 
			string participantObjectId, 
			ParticipantObjectIdTypeCodeEnum typeCode)
		{
			if (type.HasValue)
			{
				ParticipantObjectTypeCode = (ParticipantObjectIdentificationContentsParticipantObjectTypeCode)type.Value;
				ParticipantObjectTypeCodeSpecified = true;
			}
			else ParticipantObjectTypeCodeSpecified = false;

			if (role.HasValue)
			{
				ParticipantObjectTypeCodeRole = (ParticipantObjectIdentificationContentsParticipantObjectTypeCodeRole)role.Value;
				ParticipantObjectTypeCodeRoleSpecified = true;
			}
			else
				ParticipantObjectTypeCodeRoleSpecified = false;

			if (lifeCycle.HasValue)
			{
				ParticipantObjectDataLifeCycle = (ParticipantObjectIdentificationContentsParticipantObjectDataLifeCycle)lifeCycle.Value;
				ParticipantObjectDataLifeCycleSpecified = true;
			}
			else
				ParticipantObjectDataLifeCycleSpecified = false;

			ParticipantObjectID = participantObjectId;

			ParticipantObjectIDTypeCode = new ParticipantObjectIDTypeCode(typeCode);
		}

        public ParticipantObjectIdentificationContents(AuditParticipantObject item)
		{
			if (item.ParticipantObjectTypeCode.HasValue)
			{
                ParticipantObjectTypeCode = (ParticipantObjectIdentificationContentsParticipantObjectTypeCode)item.ParticipantObjectTypeCode.Value;
				ParticipantObjectTypeCodeSpecified = true;
			}
			else ParticipantObjectTypeCodeSpecified = false;

			if (item.ParticipantObjectTypeCodeRole.HasValue)
			{
                ParticipantObjectTypeCodeRole = (ParticipantObjectIdentificationContentsParticipantObjectTypeCodeRole)item.ParticipantObjectTypeCodeRole.Value;
				ParticipantObjectTypeCodeRoleSpecified = true;
			}
			else
				ParticipantObjectTypeCodeRoleSpecified = false;

			if (item.ParticipantObjectDataLifeCycle.HasValue)
			{
                ParticipantObjectDataLifeCycle = (ParticipantObjectIdentificationContentsParticipantObjectDataLifeCycle)item.ParticipantObjectDataLifeCycle.Value;
				ParticipantObjectDataLifeCycleSpecified = true;
			}
			else
				ParticipantObjectDataLifeCycleSpecified = false;

			if (item.ParticipantObjectIdTypeCode.HasValue)
			{
				ParticipantObjectIDTypeCode =
					new ParticipantObjectIDTypeCode(item.ParticipantObjectIdTypeCode.Value);
			}
			else if (item.ParticipantObjectIdTypeCodedValue != null)
			{
				ParticipantObjectIDTypeCode =
                    new ParticipantObjectIDTypeCode(item.ParticipantObjectIdTypeCodedValue);
			}

			if (item.ParticipantObjectDetail != null)
				ParticipantObjectDetail = new[] {item.ParticipantObjectDetail};

			if (!string.IsNullOrEmpty(item.ParticipantObjectDetailString))
			{
                ParticipantObjectDetail = new ParticipantObjectDetail[] { new ParticipantObjectDetail() {type = item.ParticipantObjectDetailString }};
			}

			if (!string.IsNullOrEmpty(item.ParticipantObjectId))
				ParticipantObjectID = item.ParticipantObjectId;

			if (!string.IsNullOrEmpty(item.ParticipantObjectName))
				Item = item.ParticipantObjectName;

			if (item.ParticipantObjectQuery != null)
				Item = item.ParticipantObjectQuery;

			if (!String.IsNullOrEmpty(item.AccessionNumber))
				Accession = new[] { new ParticipantObjectDescriptionTypeAccession(item.AccessionNumber) };
			if (!String.IsNullOrEmpty(item.MppsUid))
                MPPS = new[] { new ParticipantObjectDescriptionTypeMPPS(item.MppsUid) };

			if (item.SopClassDictionary != null && item.SopClassDictionary.Count > 0)
			{
				SOPClass = item.SopClassDictionary.Values.Select(sopClass => new ParticipantObjectDescriptionTypeSOPClass(sopClass.UID, sopClass.NumberOfInstances)).ToArray();
			}
		}
	}

	public partial class ParticipantObjectDescriptionTypeAccession
	{
		public ParticipantObjectDescriptionTypeAccession()
		{}

		public ParticipantObjectDescriptionTypeAccession(string accessionNumber)
		{
			Number = accessionNumber;
		}
	}

    public partial class ParticipantObjectDescriptionTypeMPPS
	{
		public ParticipantObjectDescriptionTypeMPPS()
		{}

		public ParticipantObjectDescriptionTypeMPPS(string mppsUid)
		{
			UID = mppsUid;
		}
	}

	public partial class ParticipantObjectDescriptionTypeSOPClass
	{
		public ParticipantObjectDescriptionTypeSOPClass()
		{}

		public ParticipantObjectDescriptionTypeSOPClass(string sopClassUid, int numberOfInstances)
		{
			UID = sopClassUid;
			NumberOfInstances = numberOfInstances.ToString();
		}
	}   
}