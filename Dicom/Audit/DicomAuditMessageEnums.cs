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

namespace ClearCanvas.Dicom.Audit
{

	/// <summary>
	///  An identifier for the type of network access point that originated the audit event.
	/// </summary>
	public enum NetworkAccessPointTypeEnum
	{
		/// <summary>
		/// Machine Name (1)
		/// </summary>
		MachineName = 1,
		/// <summary>
		/// IP Address (2)
		/// </summary>
		IpAddress = 2,
		/// <summary>
		/// Phone Number (3)
		/// </summary>
		PhoneNumber = 3
	}

	/// <summary>
	/// Code specifying the type of source where event originated.
	/// </summary>
	public enum AuditSourceTypeCodeEnum
	{
		/// <summary>
		/// End-user interface (1)
		/// </summary>
		EndUserInterface = 1,
		/// <summary>
		/// Data acquisition device or instrument (2)
		/// </summary>
		DataAcquisitionDeviceOrInstrument = 2,
		/// <summary>
		/// Web server process tier in a multi-tier system (3)
		/// </summary>
		WebServerProcessTierInAMultiTierSystem = 3,
		/// <summary>
		/// Application server process tier in a multi-tier system (4)
		/// </summary>
		ApplicationServerProcessTierInMultiTierSystem = 4,
		/// <summary>
		/// Database server process tier in a multi-tier system (5)
		/// </summary>
		DatabaseServerProcessTierInMultiTierSystem = 5,
		/// <summary>
		/// Security server, e.g., a domain controller (6)
		/// </summary>
		SecurityServerEGDomainController = 6,
		/// <summary>
		/// ISO level 1-3 network component (7)
		/// </summary>
		ISOLevel13NetworkComponent = 7,
		/// <summary>
		/// ISO level 4-6 operating software (8)
		/// </summary>
		ISOLevel46OperatingSoftware = 8,
		/// <summary>
		/// External source, other or unknown type (9)
		/// </summary>
		ExternalSourceOtherOrUnknownType = 9
	}

	/// <summary>
	/// Code for the participant object type being audited.  This value is distinct 
	/// from the user's role or any user relationship to the participant object.
	/// </summary>
	public enum ParticipantObjectTypeCodeEnum
	{
		/// <summary>
		/// Person (1)
		/// </summary>
		Person = 1,
		/// <summary>
		/// System Object (2)
		/// </summary>
		SystemObject = 2,
		/// <summary>
		/// Organization (3)
		/// </summary>
		Organization = 3,
		/// <summary>
		/// Other (4)
		/// </summary>
		Other = 4
	}

	/// <summary>
	/// Code representing the functional application role of Participant Object being audited
	/// </summary>
	public enum ParticipantObjectTypeCodeRoleEnum
	{
		/// <summary>
		/// Patient (1) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/>
		/// </summary>
		Patient = 1,

		/// <summary>
		/// Location (2) for use with <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		Location = 2,

		/// <summary>
		/// Report (3) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		Report = 3,

		/// <summary>
		/// Resource (4) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		Resource = 4,

		/// <summary>
		/// Master file (5) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		MasterFile = 5,

		/// <summary>
		/// User (6) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		User = 6,

		/// <summary>
		/// List (7) file for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		List = 7,

		/// <summary>
		/// Doctor (8) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/>
		/// </summary>
		Doctor = 8,

		/// <summary>
		/// Subscriber (9) for use with <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		Subscriber = 9,

		/// <summary>
		/// Guarantor (10) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		Guarantor = 10,

		/// <summary>
		/// Security User Entity (11) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		SecurityUserEntity = 11,

		/// <summary>
		/// Security User Group (12) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		SecurityUserGroup = 12,

		/// <summary>
		/// Security Resource (13) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		SecurityResource = 13,

		/// <summary>
		/// Security Granularity Definition (14) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		SecurityGranularityDefinition = 14,

		/// <summary>
		/// Provider (15) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		Provider = 15,

		/// <summary>
		/// Data Destination (16) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		DataDestination = 16,

		/// <summary>
		/// Data Repository (17) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		DataRepository = 17,

		/// <summary>
		/// Schedule (18) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		Schedule = 18,

		/// <summary>
		/// Customer (19) for use with <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		Customer = 19,

		/// <summary>
		/// Job (20) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		Job = 20,

		/// <summary>
		/// Job Stream (21) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		JobStream = 21,
		/// <summary>
		/// Table (22) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		Table = 22,
		/// <summary>
		/// Routing Criteria (23) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		RoutingCriteria = 23,
		/// <summary>
		/// Query (24) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		Query = 24,
	}

	/// <summary>
	/// Identifier for the data life-cycle stage for the participant object.  This can be used to
	/// provide an audit trail for data, over time, as it passes through the system.
	/// </summary>
	public enum ParticipantObjectDataLifeCycleEnum
	{
		/// <summary>
		/// Origination / Creation (1)
		/// </summary>
		OriginationCreation = 1,
		/// <summary>
		/// Import / Copy from original (2)
		/// </summary>
		ImportCopyFromOriginal = 2,
		/// <summary>
		/// Amendment (3)
		/// </summary>
		Amendment = 3,
		/// <summary>
		/// Verification (4)
		/// </summary>
		Verification = 4,
		/// <summary>
		/// Translation(5)
		/// </summary>
		Translation = 5,
		/// <summary>
		/// Access / Use (6)
		/// </summary>
		AccessUse = 6,
		/// <summary>
		/// De-identification (7)
		/// </summary>
		DeIdentification = 7,
		/// <summary>
		/// Aggregation, summarization, derivation (8)
		/// </summary>
		AggregationSummarizationDerivation = 8,
		/// <summary>
		/// Report (9)
		/// </summary>
		Report = 9,
		/// <summary>
		/// Export / Copy to target (10)
		/// </summary>
		ExportCopyToTarget = 10,
		/// <summary>
		/// Disclosure (11)
		/// </summary>
		Disclosure = 11,
		/// <summary>
		/// Receipt of disclosure (12)
		/// </summary>
		ReceiptOfDisclosure = 12,
		/// <summary>
		/// Archiving (13)
		/// </summary>
		Archiving = 13,
		/// <summary>
		/// Logical deletion (14)
		/// </summary>
		LogicalDeletion = 14,
		/// <summary>
		/// Permanent erasure / Physical destruction (15)
		/// </summary>
		PermanentErasurePhysicalDestruction = 15,
	}

	/// <summary>
	///  Describes the identifier that is contained in Participant Object ID.
	/// </summary>
	public enum ParticipantObjectIdTypeCodeEnum
	{
		///<summary>
		/// Medical Record Number (1) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/>
		/// </summary>
		MedicalRecordNumber = 1,
		/// <summary>
		/// Patient Number (2) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/>
		/// </summary>
		PatientNumber = 2,
		/// <summary>
		/// Encounter Number (3) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/>
		/// </summary>
		EncounterNumber = 3,
		/// <summary>
		/// Enrollee Number (4) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/>
		/// </summary>
		EnrolleeNumber = 4,
		/// <summary>
		/// Social Security Number (5) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/>
		/// </summary>
		SocialSecurityNumber = 5,
		/// <summary>
		/// Account Number  (6) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		AccountNumber = 6,
		/// <summary>
		/// Guarantor Number  (7) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or <see cref="ParticipantObjectTypeCodeEnum.Organization"/>
		/// </summary>
		GuarantorNumber = 7,
		/// <summary>
		/// Report Name (8) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		ReportName = 8,
		/// <summary>
		/// Report Number (9) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		ReportNumber = 9,
		/// <summary>
		/// SearchCriteria (10) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		SearchCriteria = 10,
		/// <summary>
		/// User Identifier  (11) for use with <see cref="ParticipantObjectTypeCodeEnum.Person"/> or  <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		UserIdentifier = 11,
		/// <summary>
		/// URI (12) for use with <see cref="ParticipantObjectTypeCodeEnum.SystemObject"/>
		/// </summary>
		URI = 12,
	}
}