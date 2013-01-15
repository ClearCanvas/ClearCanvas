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

using System.Collections.Generic;
using System.Net;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class ScuTests : AbstractTest
	{
		[TestFixtureSetUp]
		public void Init()
		{
			_serverType = TestTypes.Receive;
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		TestTypes _serverType;

		public IDicomServerHandler ServerHandlerCreator(DicomServer server, ServerAssociationParameters assoc)
		{
			return new ServerHandler(this, _serverType);
		}

		private StorageScu SetupScu()
		{
			StorageScu scu = new StorageScu("TestAe", "AssocTestServer", "localhost", 2112);

			IList<DicomAttributeCollection> list = SetupMRSeries(4, 2, DicomUid.GenerateUid().UID);

			foreach (DicomAttributeCollection collection in list)
			{
				DicomFile file = new DicomFile("test", new DicomAttributeCollection(), collection);
				file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
				file.MediaStorageSopClassUid = SopClass.MrImageStorage.Uid;
				file.MediaStorageSopInstanceUid = collection[DicomTags.SopInstanceUid].ToString();

				scu.AddStorageInstance(new StorageInstance(file));
			}

			return scu;
		}

		[Test]
		public void ScuAbortTest()
		{
			int port = 2112;

			/* Setup the Server */
			ServerAssociationParameters serverParameters = new ServerAssociationParameters("AssocTestServer", new IPEndPoint(IPAddress.Any, port));
			byte pcid = serverParameters.AddPresentationContext(SopClass.MrImageStorage);
			serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
			serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrBigEndian);
			serverParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

			_serverType = TestTypes.Receive;
			DicomServer.StartListening(serverParameters, ServerHandlerCreator);

			StorageScu scu = SetupScu();

			IList<DicomAttributeCollection> list = SetupMRSeries(4, 2, DicomUid.GenerateUid().UID);

			foreach (DicomAttributeCollection collection in list)
			{
				DicomFile file = new DicomFile("test",new DicomAttributeCollection(),collection );
				file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
				file.MediaStorageSopClassUid = SopClass.MrImageStorage.Uid;
				file.MediaStorageSopInstanceUid = collection[DicomTags.SopInstanceUid].ToString();

				scu.AddStorageInstance(new StorageInstance(file));
			}

			scu.ImageStoreCompleted += delegate(object o, StorageInstance instance)
			                           	{
											// Test abort
			                           		scu.Abort();
			                           	};

			scu.Send();
			scu.Join();

			Assert.AreEqual(scu.Status, ScuOperationStatus.NetworkError);

			// StopListening
			DicomServer.StopListening(serverParameters);
		}
	}
}

#endif
