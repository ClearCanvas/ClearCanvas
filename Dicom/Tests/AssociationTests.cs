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
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ClearCanvas.Dicom.Network;
using NUnit.Framework;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Tests
{
    public enum TestTypes
    {
        AssociationReject,
        AssociationAbort,
        SendMR,
		Receive
    }

    public class ClientHandler : IDicomClientHandler
    {
        private readonly AbstractTest _test;
        private readonly TestTypes _type;
        public ManualResetEvent _threadStop = new ManualResetEvent(false);

        public bool OnClientConnectedCalled = false;
        public bool OnClientClosedCalled = false;

        public ClientHandler(AbstractTest test, TestTypes type)
        {
            _test = test;
            _type = type;
        }

        #region IDicomClientHandler Members

        public void OnDimseTimeout(DicomClient client, ClientAssociationParameters association)
        {
        }

        public void OnClientClosed(DicomClient client, ClientAssociationParameters association)
        {
            OnClientClosedCalled = true;
        }

        public void OnNetworkError(DicomClient client, ClientAssociationParameters association, Exception e)
        {
            Assert.Fail("Incorrectly received OnNetworkError callback");
        }

        public void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association)
        {
            if (_type == TestTypes.AssociationReject)
            {
                Assert.Fail("Unexpected negotiated association on reject test.");
            }
            else if (_type == TestTypes.SendMR)
            {
                DicomMessage msg = new DicomMessage();

                _test.SetupMR(msg.DataSet);
                byte id = association.FindAbstractSyntaxWithTransferSyntax(msg.SopClass, TransferSyntax.ExplicitVrLittleEndian);

                client.SendCStoreRequest(id, client.NextMessageID(), DicomPriority.Medium, msg);
            }
            else
            {
                Assert.Fail("Unexpected test type");
            }
        }

        public void OnReceiveRequestMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {
            Assert.Fail("Incorrectly received OnReceiveRequestMessage callback");
        }

        public void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {
            client.SendReleaseRequest();
            Assert.AreEqual(message.Status.Code, DicomStatuses.Success.Code, "Incorrect DICOM status returned");
        }

        public void OnReceiveReleaseResponse(DicomClient client, ClientAssociationParameters association)
        {
            // Signal the main thread we're exiting
            _threadStop.Set();
        }

        public void OnReceiveAssociateReject(DicomClient client, ClientAssociationParameters association, DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
        {
            if (_type == TestTypes.AssociationReject)
            {
                Assert.IsTrue(source == DicomRejectSource.ServiceProviderACSE);
                Assert.IsTrue(result == DicomRejectResult.Permanent);
                Assert.IsTrue(reason == DicomRejectReason.NoReasonGiven);
                _threadStop.Set();
            }
            else
                Assert.Fail("Incorrectly received OnReceiveAssociateReject callback");
        }

        public void OnReceiveAbort(DicomClient client, ClientAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
        {
            Assert.Fail("Incorrectly received OnReceiveAbort callback");
        }

        #endregion
    }

    public class ServerHandler : IDicomServerHandler
    {
        readonly AbstractTest _test;
        readonly TestTypes _type;

        public ServerHandler(AbstractTest test, TestTypes type)
        {
            _test = test;
            _type = type;
            MessagesReceived = new List<DicomMessage>();
        }

        public List<DicomMessage> MessagesReceived { get; set; }

        #region IDicomServerHandler Members

        public void OnClientConnected(DicomServer server, ServerAssociationParameters association)
        {

        }

        public void OnClientClosed(DicomServer server, ServerAssociationParameters association)
        {
        }

        public void OnNetworkError(DicomServer server, ServerAssociationParameters association, Exception e)
        {
            Assert.Fail("Unexpected network error: " + e.Message);
        }

        public void OnDimseTimeout(DicomServer client, ServerAssociationParameters association)
        {
        }

        public void OnReceiveAssociateRequest(DicomServer server, ServerAssociationParameters association)
        {
            server.SendAssociateAccept(association);
        }

        public void OnReceiveRequestMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
        	if (_type == TestTypes.SendMR)
        	{
        		var testSet = new DicomAttributeCollection();

        		_test.SetupMR(testSet);

        		bool same = testSet.Equals(message.DataSet);


        		string studyId = message.DataSet[DicomTags.StudyId].GetString(0, "");
        		Assert.AreEqual(studyId, "1933");


        		DicomUid sopInstanceUid;
        		bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetUid(0, out sopInstanceUid);
        		if (!ok)
        		{
        			server.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
        			return;
        		}

        		server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID, DicomStatuses.Success);
        	}
        	else if (_type == TestTypes.Receive)
        	{
				DicomUid sopInstanceUid;
				bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetUid(0, out sopInstanceUid);
				if (!ok)
				{
					server.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.InvalidPDUParameter);
					return;
				}

        	    MessagesReceived.Add(message);

				server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID, DicomStatuses.Success);
			}
        	else
        	{
				Platform.Log(LogLevel.Error,"Unexpected test type mode");
				server.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.InvalidPDUParameter);
        	}
    }

        public void OnReceiveResponseMessage(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            server.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
            Assert.Fail("Unexpected OnReceiveResponseMessage");
        }

        public void OnReceiveReleaseRequest(DicomServer server, ServerAssociationParameters association)
        {

        }

	    public void OnReceiveDimseCommand(DicomServer server, ServerAssociationParameters association, byte presentationId,
	                                      DicomAttributeCollection command)
	    {
	    }

	    public IDicomFilestreamHandler OnStartFilestream(DicomServer server, ServerAssociationParameters association,
	                                                     byte presentationId, DicomMessage message)
	    {
		    throw new NotImplementedException();
	    }

	    public void OnReceiveAbort(DicomServer server, ServerAssociationParameters association, DicomAbortSource source, DicomAbortReason reason)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }

    [TestFixture]
    public class AssociationTests : AbstractTest
    {
        TestTypes _serverType;

        public IDicomServerHandler ServerHandlerCreator(DicomServer server, ServerAssociationParameters assoc)
        {
            return new ServerHandler(this,_serverType);
        }  

        [Test]
        public void RejectTests()
        {
            const int port = 2112;

            /* Setup the Server */
            var serverParameters = new ServerAssociationParameters("AssocTestServer", new IPEndPoint(IPAddress.Any, port));
            byte pcid = serverParameters.AddPresentationContext(SopClass.MrImageStorage);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrBigEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            _serverType = TestTypes.AssociationReject;
            DicomServer.StartListening(serverParameters, ServerHandlerCreator);

            /* Setup the client */
            var clientParameters = new ClientAssociationParameters("AssocTestClient", "AssocTestServer",
                                                                                           new IPEndPoint(IPAddress.Loopback, port));
            pcid = clientParameters.AddPresentationContext(SopClass.CtImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            /* Open the association */
            var handler = new ClientHandler(this, TestTypes.AssociationReject);
            DicomClient client = DicomClient.Connect(clientParameters, handler);


            handler._threadStop.WaitOne();
            client.Dispose();

            _serverType = TestTypes.AssociationReject;

            /* Setup the client */
            clientParameters = new ClientAssociationParameters("AssocTestClient", "AssocTestServer",
                                                               new IPEndPoint(IPAddress.Loopback, port));
            pcid = clientParameters.AddPresentationContext(SopClass.MrImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);


            /* Open the association */
            var clientHandler = new ClientHandler(this, TestTypes.AssociationReject);
            client = DicomClient.Connect(clientParameters, clientHandler);

            handler._threadStop.WaitOne();
            client.Dispose();


            DicomServer.StopListening(serverParameters);

        }

       

        [Test]
        public void ServerTest()
        {
            const int port = 2112;

            /* Setup the Server */
            var serverParameters = new ServerAssociationParameters("AssocTestServer",new IPEndPoint(IPAddress.Any,port));
            byte pcid = serverParameters.AddPresentationContext(SopClass.MrImageStorage);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrBigEndian);
            serverParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            _serverType = TestTypes.SendMR;
            DicomServer.StartListening(serverParameters, ServerHandlerCreator);

            /* Setup the client */
            var clientParameters = new ClientAssociationParameters("AssocTestClient","AssocTestServer",
                                                                                           new IPEndPoint(IPAddress.Loopback,port));
            pcid = clientParameters.AddPresentationContext(SopClass.MrImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            pcid = clientParameters.AddPresentationContext(SopClass.CtImageStorage);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ExplicitVrLittleEndian);
            clientParameters.AddTransferSyntax(pcid, TransferSyntax.ImplicitVrLittleEndian);

            /* Open the association */
            var handler = new ClientHandler(this,TestTypes.SendMR);
            DicomClient client = DicomClient.Connect(clientParameters, handler);

            handler._threadStop.WaitOne();

            client.Dispose();

            DicomServer.StopListening(serverParameters);
        }
    }
}

#endif
