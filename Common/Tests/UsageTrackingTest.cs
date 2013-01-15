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

#if	UNIT_TESTS_USAGE_TRACKING


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common.UsageTracking;
using NUnit.Framework;

namespace ClearCanvas.Common.Tests
{
    [TestFixture]
    public class UsageTrackingTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            // load the default setting values.
            UsageTrackingSettings.Default.Reload();
            var dummy = UsageTrackingSettings.Default.Enabled;
            dummy = UsageTrackingSettings.Default.DisplayMessages;
        }

        #region Helper Methods

        private static void ChangeSettings(string key, object value)
        {
            if (UsageTrackingSettings.Default.PropertyValues[key] == null)
                UsageTrackingSettings.Default.PropertyValues.Add(
                    new System.Configuration.SettingsPropertyValue(new System.Configuration.SettingsProperty(key)));
            UsageTrackingSettings.Default.PropertyValues[key].PropertyValue = value;
        }

        private static void Wait(MockService service, int ms, bool serviceShouldReceiveMsg)
        {
            if (!service._sync.WaitOne(ms))
            {
                if (serviceShouldReceiveMsg)
                    Assert.Fail("Service does not receive the message as expected");
            }
            else
            {
                if (!serviceShouldReceiveMsg)
                    Assert.Fail("Service shall not receive the message");
            }
        }

        #endregion


        /// <summary>
        /// This makes sure nothing is sent if usage tracking is disabled
        /// </summary>
        [Test]
        public void TestDisabled()
        {
            ChangeSettings("Enabled", false);

            var service = MockServer.Start(new Uri("http://localhost:8080"));

            int count = service.ReceivedMessageCount;
            UsageTracking.UsageTracking.Register(UsageTracking.UsageTracking.GetUsageMessage());
            Wait(service, 5000, false);
            Assert.AreEqual(count, service.ReceivedMessageCount);
        }

        /// <summary>
        /// This makes sure message is sent when usage tracking is enabled.
        /// </summary>
        [Test]
        public void TestEnabled()
        {
            ChangeSettings("Enabled",true);
            var service = MockServer.Start(new Uri("http://localhost:8080"));

            int count = service.ReceivedMessageCount;
            UsageTracking.UsageTracking.Register(UsageTracking.UsageTracking.GetUsageMessage());
            Wait(service, 5000, true);
            Assert.AreEqual(count + 1, service.ReceivedMessageCount);
        }

        /// <summary>
        /// This makes sure errors on the server does not affect the client
        /// </summary>
        [Test]
        public void TestErrorOnServer()
        {
            ChangeSettings("Enabled",true);
            var service = MockServer.Start(new Uri("http://localhost:8080"));
            
            service.ThrowError = true;

            int count = service.ReceivedMessageCount;
            UsageTracking.UsageTracking.Register(UsageTracking.UsageTracking.GetUsageMessage());
            Wait(service, 5000, true);
            Assert.AreEqual(count + 1, service.ReceivedMessageCount);
        }

        /// <summary>
        /// This makes sure NO exception is throw when connection cannot be made (bad address)
        /// </summary>
        [Test]
        public void TestNoConnection()
        {
            ChangeSettings("Enabled",true);
            var service = MockServer.Start(new Uri("http://localhost:9000"));
            service.ThrowError = true;
            
            int count = service.ReceivedMessageCount;
            UsageTracking.UsageTracking.Register(UsageTracking.UsageTracking.GetUsageMessage());
            Wait(service, 5000, false);
            Assert.AreEqual(count, service.ReceivedMessageCount);
        }

        /// <summary>
        /// This makes sure NO exception is throw when connection is dropped during transmission
        /// </summary>
        [Test]
        public void TestErrorDuringTransmission()
        {
            ChangeSettings("Enabled",true);
            var service = MockServer.Start(new Uri("http://localhost:8080"));
            
            service.ThrowCommunicationError = true;

            int count = service.ReceivedMessageCount;
            UsageTracking.UsageTracking.Register(UsageTracking.UsageTracking.GetUsageMessage());
            Wait(service, 5000, true);
            Assert.AreEqual(count+1, service.ReceivedMessageCount);
        }


        /// <summary>
        /// This makes sure NO exception is thrown if message is too large
        /// </summary>
        [Test]
        public void TestMessageSize()
        {
            ChangeSettings("Enabled",true);
            var service = MockServer.Start(new Uri("http://localhost:8080"));
            
            int count = service.ReceivedMessageCount;
            var msg = UsageTracking.UsageTracking.GetUsageMessage();
            List<UsageApplicationData> data = new List<UsageApplicationData>
                                                  {
                                                      new UsageApplicationData
                                                          {
                                                              Key = Guid.NewGuid().ToString(),
                                                              Value = Guid.NewGuid().ToString()
                                                          }
                                                  };
            msg.AppData = data;
            UsageTracking.UsageTracking.Register(msg);
            Wait(service, 5000, true);
            Assert.AreEqual(count+1, service.ReceivedMessageCount);


            service.Reset();
            count = service.ReceivedMessageCount;
            for (int i = 0; i < 100000; i++)
            {
                data.Add(new UsageApplicationData
                             {
                    Key = Guid.NewGuid().ToString(),
                    Value = Guid.NewGuid().ToString()
                });
            }
            msg.AppData = data;
            UsageTracking.UsageTracking.Register(msg);
            Wait(service, 5000, false);
            Assert.AreEqual(count, service.ReceivedMessageCount);

        }

        /// <summary>
        /// This makes sure the response from the server is passed to the caller 
        /// </summary>
        [Test]
        public void TestResponse()
        {
            ChangeSettings("Enabled",true);
            ChangeSettings("DisplayMessages", true);
            var service = MockServer.Start(new Uri("http://localhost:8080"));
            
            Exception exception = null;

            ManualResetEvent msgReceiveEvent = new ManualResetEvent(false);
            
            service.Response = new RegisterResponse
                                   {
                                       Message = new DisplayMessage {Title = @"Title", @Message = @"The Message"}
                                   };

            UsageTracking.UsageTracking.MessageEvent += (s, ev) =>
                                                            {
                                                                try
                                                                {
                                                                    if (ev.Item!=null)
                                                                    {
                                                                        Assert.AreEqual(@"Title", ev.Item.Title);
                                                                        Assert.AreEqual(@"The Message", ev.Item.Message);
                                                                    }
                                                                    
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    exception = ex;
                                                                }
                                                                finally
                                                                {
                                                                    msgReceiveEvent.Set();
                                                                }
                                                            };
            
            
            var msg = UsageTracking.UsageTracking.GetUsageMessage();
            UsageTracking.UsageTracking.Register(msg);

            Wait(service, 5000, true);

            if (exception!=null)
                Assert.Fail("The response message doesn't match what's expected: {0}", exception.Message);

        }

        /// <summary>
        /// This makes sure the client thread is not blocked by the server
        /// </summary>
        [Test]
        public void TestBlockOnServer()
        {
            
            ChangeSettings("Enabled",true);
            var service = MockServer.Start(new Uri("http://localhost:8080"));
            
            // Instruct the service not to return until 20 seconds later
            // The client thread should not be blocked during this time.
            const int serverDelay = 20000;

            service.DelayResponse = serverDelay;

            int count = service.ReceivedMessageCount;
            UsageTracking.UsageTracking.Register(UsageTracking.UsageTracking.GetUsageMessage());

            int n = 0;
            const int checkPeriod = 10;

            while(true)
            {
                if (service._sync.WaitOne(checkPeriod))
                    break;
                n++;
            }

            const int checkWindow = serverDelay / 2;
            Assert.Greater(n, checkWindow / checkPeriod);
            Assert.AreEqual(count + 1, service.ReceivedMessageCount);
            
        }
    }


    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    class MockService : IUsageTracking
    {
        public bool ThrowError { get; set; }
        public bool ThrowCommunicationError { get; set; }
        public int ReceivedMessageCount { get; set; }
        public RegisterResponse Response { get; set; }
        public int DelayResponse { get; set; }

        public ManualResetEvent _sync = new ManualResetEvent(false);

        public RegisterResponse Register(RegisterRequest request)
        {
            try
            {
                ReceivedMessageCount++;
                if (ThrowError)
                    throw new Exception("Exception on the server");

                if (ThrowCommunicationError)
                    throw new CommunicationException("CommunicationException");

                if (DelayResponse>0)
                {
                    Thread.Sleep(DelayResponse);
                }
                return Response;
            }
            finally
            {
                _sync.Set();
            }
        }

        internal void Reset()
        {
            _sync.Reset();
        }
    }

    static class MockServer
    {
        private static ServiceHost _host;
        private static MockService _service;
        static public MockService Start(Uri endpoint)
        {
            if (_host != null)
                Stop();

            _service = new MockService();
            _host = new ServiceHost(_service, new[] { endpoint });
            _host.AddServiceEndpoint(typeof(IUsageTracking), new WSHttpBinding(), "UsageTracking");
            _host.Open();

            return _service;
        }

        static public void Stop()
        {
            if (_host!=null)
            {
                _host.Close();
                _host = null;
            }
        }
    }
}

#endif