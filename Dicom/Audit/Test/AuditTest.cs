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

#if UNIT_TESTS

using System;
using System.Net;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Audit.Test
{

    [TestFixture]
    public class AuditTest : AbstractTest
    {
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ClearCanvas.Common.Platform.SetExtensionFactory(new ClearCanvas.Common.Utilities.NullExtensionFactory());
		}

		[Test]
		public void ApplicationActivityAuditTest()
		{
			ApplicationActivityAuditHelper helper =
				new ApplicationActivityAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success, ApplicationActivityType.ApplicationStarted,
					new AuditProcessActiveParticipant("testApp"));

			helper.AddUserParticipant(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

		[Test]
		public void AuditLogUsedAuditTest()
		{
			AuditLogUsedAuditHelper helper =
				new AuditLogUsedAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success, 
					"http://www.clearcanvas.ca");

			helper.AddActiveParticipant(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

		[Test]
		public void BeginTransferringDicomInstancesAuditTest()
		{
			AssociationParameters parms = new ClientAssociationParameters("CLIENT", "SERVER",
																		  new IPEndPoint(new IPAddress(new byte[] { 2, 2, 2, 2 }),
																						 2));
			parms.LocalEndPoint = new IPEndPoint(new IPAddress(new byte[] { 1, 1, 1, 1 }),
												 1);


			BeginTransferringDicomInstancesAuditHelper helper =
				new BeginTransferringDicomInstancesAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,
					parms, new AuditPatientParticipantObject("id1234", "Test Patient"));

			DicomAttributeCollection collection = new DicomAttributeCollection();
			SetupMR(collection);
			helper.AddStorageInstance(new StorageInstance(new DicomMessage(new DicomAttributeCollection(), collection)));
			
			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

		[Test]
		public void DataExportAuditTest()
		{
			DataExportAuditHelper helper =
				new DataExportAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,
					"MEDIA123");

			helper.AddExporter(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			DicomAttributeCollection collection = new DicomAttributeCollection();
			SetupMR(collection);
			helper.AddPatientParticipantObject(new AuditPatientParticipantObject(collection));
			helper.AddStorageInstance(new StorageInstance(new DicomMessage(new DicomAttributeCollection(), collection)));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

		[Test]
		public void DataImportAuditTest()
		{
			DataImportAuditHelper helper =
				new DataImportAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,
					"MEDIA123");

			helper.AddImporter(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			DicomAttributeCollection collection = new DicomAttributeCollection();
			SetupMR(collection);
			helper.AddPatientParticipantObject(new AuditPatientParticipantObject(collection));
			helper.AddStorageInstance(new StorageInstance(new DicomMessage(new DicomAttributeCollection(), collection)));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}


		[Test]
		public void DicomInstancesAccessedAuditTest()
		{
			DicomInstancesAccessedAuditHelper helper =
				new DicomInstancesAccessedAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,
					EventIdentificationContentsEventActionCode.R);

			helper.AddUser(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			DicomAttributeCollection collection = new DicomAttributeCollection();
			SetupMR(collection);
			helper.AddPatientParticipantObject(new AuditPatientParticipantObject(collection));
			helper.AddStorageInstance(new StorageInstance(new DicomMessage(new DicomAttributeCollection(), collection)));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}


		[Test]
		public void DicomInstancesTransferredAuditTest()
		{
			AssociationParameters parms = new ClientAssociationParameters("CLIENT", "SERVER",
			                                                              new IPEndPoint(new IPAddress(new byte[] {2, 2, 2, 2}),
			                                                                             2));
			parms.LocalEndPoint = new IPEndPoint(new IPAddress(new byte[] {1, 1, 1, 1}),
			                                     1);


			DicomInstancesTransferredAuditHelper helper =
				new DicomInstancesTransferredAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,
					EventIdentificationContentsEventActionCode.R,
					parms);

			DicomAttributeCollection collection = new DicomAttributeCollection();
			SetupMultiframeXA(collection,128,128,2);
			helper.AddPatientParticipantObject(new AuditPatientParticipantObject(collection));
			helper.AddStorageInstance(new StorageInstance(new DicomMessage(new DicomAttributeCollection(), collection)));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

		[Test]
		public void DicomStudyDeletedAuditTest()
		{
			DicomStudyDeletedAuditHelper helper =
				new DicomStudyDeletedAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success);

			helper.AddUserParticipant(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			DicomAttributeCollection collection = new DicomAttributeCollection();
			SetupMultiframeXA(collection, 128, 128, 2);
			helper.AddPatientParticipantObject(new AuditPatientParticipantObject(collection));
			helper.AddStorageInstance(new StorageInstance(new DicomMessage(new DicomAttributeCollection(), collection)));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

		[Test]
		public void NetworkEntryAuditTest()
		{
			NetworkEntryAuditHelper helper =
				new NetworkEntryAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,NetworkEntryType.Attach,
					new AuditProcessActiveParticipant("testAe"));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

    	[Test]
        public void QueryAuditTest()
        {
			DicomAttributeCollection query = new DicomAttributeCollection();
    		query[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
			query[DicomTags.StudyInstanceUid].SetNullValue();
			query[DicomTags.PatientId].SetNullValue();
			query[DicomTags.AccessionNumber].SetNullValue();
			query[DicomTags.PatientsName].SetStringValue("*W*");
			query[DicomTags.ReferringPhysiciansName].SetNullValue();
			query[DicomTags.StudyDate].SetNullValue();
			query[DicomTags.StudyTime].SetNullValue();
			query[DicomTags.StudyDescription].SetNullValue();
			query[DicomTags.PatientsBirthDate].SetNullValue();
			query[DicomTags.ModalitiesInStudy].SetNullValue();
			query[DicomTags.NumberOfStudyRelatedInstances].SetNullValue();
    		query[DicomTags.InstanceAvailability].SetNullValue();

        	AssociationParameters parms = new ClientAssociationParameters("CLIENT", "SERVER",
        	                                                         new IPEndPoint(new IPAddress(new byte[] {2, 2, 2, 2}),
        	                                                                       2));
			parms.LocalEndPoint = new IPEndPoint(new IPAddress(new byte[] {1, 1, 1, 1}),
        	                                                                       1);

    		QueryAuditHelper helper =
    			new QueryAuditHelper(new DicomAuditSource("testApplication"),
    			                     EventIdentificationContentsEventOutcomeIndicator.Success, parms,
    			                     SopClass.StudyRootQueryRetrieveInformationModelFindUid, query);

			helper.AddOtherParticipant(new AuditPersonActiveParticipant("testUser","test@test","Test Name"));
        	helper.AddPatientParticipantObject(new AuditPatientParticipantObject("id1234", "Test Patient"));

			AuditStudyParticipantObject study = new AuditStudyParticipantObject("1.2.3.4.5", "A1234", "1.2.3");
        	study.AddSopClass("1.2.3", 5);
			helper.AddStudyParticipantObject(study);

        	string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Exception exception;
			//Assert.Ignore("Skipping schema validation due to schema bug #9455");
			//if (!helper.Verify(out exception))
			//    throw exception;

    		helper =
    			new QueryAuditHelper(
    				new DicomAuditSource("testApplication2", "enterpriseId", AuditSourceTypeCodeEnum.EndUserInterface),
    				EventIdentificationContentsEventOutcomeIndicator.Success, parms,
    				SopClass.StudyRootQueryRetrieveInformationModelFindUid, query);
			helper.AddStudyParticipantObject(new AuditStudyParticipantObject("1.2.3.4.5"));

			output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			if (!helper.Verify(out exception))
				throw exception;           
        }

		[Test]
		public void SecurityAlertAuditTest()
		{
			SecurityAlertAuditHelper helper =
				new SecurityAlertAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,SecurityAlertEventTypeCodeEnum.NodeAuthentication);
			helper.AddReportingUser(new AuditProcessActiveParticipant("serverAe"));
			helper.AddActiveParticipant(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}

		[Test]
		public void UserAuthenticationAuditTest()
		{
			UserAuthenticationAuditHelper helper =
				new UserAuthenticationAuditHelper(
					new DicomAuditSource("testApp", "Site", AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem),
					EventIdentificationContentsEventOutcomeIndicator.Success,UserAuthenticationEventType.Login);
			helper.AddNode(new AuditProcessActiveParticipant("serverAe"));
			helper.AddUserParticipant(new AuditPersonActiveParticipant("testUser", "test@test", "Test Name"));

			string output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			Assert.Ignore("Skipping schema validation due to schema bug #9455");
			Exception exception;
			if (!helper.Verify(out exception))
				throw exception;
		}
	}
}

#endif