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
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Test
{
    class HeaderStreamingServiceClient : ClientBase<IHeaderStreamingService>, IHeaderStreamingService
    {
        public HeaderStreamingServiceClient()
        {

        }

        public HeaderStreamingServiceClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {

        }

        public Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters)
        {
            return Channel.GetStudyHeader(callingAETitle, parameters);
        }
    }

    class TestClient
    {
        private StudyXml _studyXml;
        private readonly string _id;

        public TestClient(string id)
        {
            _id = id;
        }
        public void GetStudy(string studyInstanceUid, Uri baseUri, string serverAE, bool singleImage)
        {
            Console.WriteLine("[{0}] : Loading {1}", _id, studyInstanceUid); 
            string uri = String.Format("http://{0}:{1}/HeaderStreaming/HeaderStreaming", "localhost", 50221);
            EndpointAddress endpoint = new EndpointAddress(uri);
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.TextEncoding = Encoding.UTF8;

            HeaderStreamingServiceClient headerClient = new HeaderStreamingServiceClient(binding, endpoint);

            using(headerClient)
            {
                HeaderStreamingParameters parms = new HeaderStreamingParameters();
                parms.StudyInstanceUID = studyInstanceUid;
                parms.ReferenceID = Guid.NewGuid().ToString();
                parms.ServerAETitle = "ImageServer";
                Stream stream = headerClient.GetStudyHeader("TEST", parms);
                
                var theMemento = new StudyXmlMemento();
                StudyXmlIo.ReadGzip(theMemento, stream);
                _studyXml = new StudyXml(studyInstanceUid);
                _studyXml.SetMemento(theMemento);
                stream.Close();
                headerClient.Close();
            }

            if (singleImage)
            {
                ulong size = 0;
                StreamingClient client = new StreamingClient(baseUri);
                foreach (SeriesXml series in _studyXml)
                {
                    foreach (InstanceXml instance in series)
                    {
                        do
                        {
                            Stream image =
                                client.RetrieveImage(serverAE, studyInstanceUid, series.SeriesInstanceUid,
                                                     instance.SopInstanceUid);
                            byte[] buffer = new byte[image.Length];
                            image.Read(buffer, 0, buffer.Length);
                            image.Close();
                            Console.Write(".");
                        } while (true);
                    }

                    Console.WriteLine("\n[{0}] : Finish Loading {1} [{2}]", _id, studyInstanceUid,
                                      ByteCountFormatter.Format(size));
                }

            }
            else
            {
                Random ran = new Random();
                do
                {
                    ulong size = 0;
                    StreamingClient client = new StreamingClient(baseUri);
                    foreach (SeriesXml series in _studyXml)
                    {
                        foreach (InstanceXml instance in series)
                        {
                            Stream image =
                                client.RetrieveImage(serverAE, studyInstanceUid, series.SeriesInstanceUid,
                                                     instance.SopInstanceUid);

                            byte[] buffer = new byte[image.Length];
                            image.Read(buffer, 0, buffer.Length);
                            image.Close();
                            size += (ulong)buffer.Length;
                            Console.Write(".");
                        }

                        Console.WriteLine("\n[{0}] : Finish Loading {1} [{2}]", _id, studyInstanceUid,
                                          ByteCountFormatter.Format(size));
                    }

                    Thread.Sleep(ran.Next(1000,5000));
                } while (true);
            }
            
            
        }

    }


    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class StressTest:IApplicationRoot
    {
        private List<string> _studies = new List<string>();
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            CommandLine cmd = new CommandLine(args);
            int numClients = int.Parse(cmd.Named["n"]);
            bool single = cmd.Switches["extreme"];
            StudyRootFindScu scu = new StudyRootFindScu();
            DicomAttributeCollection query = new DicomAttributeCollection();
            query[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
            query[DicomTags.StudyInstanceUid].SetStringValue("");
            scu.Find("Test", "ImageServer", "localhost", 5001, query);

            foreach(DicomAttributeCollection result in scu.Results)
            {
                
                _studies.Add(result[DicomTags.StudyInstanceUid].ToString());
            }

            Random r = new Random();
                                                       
            for (int i = 0; i < numClients; i++)
            {
                Thread thread = new Thread(delegate(object state)
                                               {
                                                   do
                                                   {
                                                       Thread.Sleep(r.Next(200,10000)); 
                                                       TestClient client = new TestClient("" + Thread.CurrentThread.ManagedThreadId);
                                                       client.GetStudy(_studies[r.Next(_studies.Count)],
                                                                       new Uri("http://localhost:1000/wado"),
                                                                       "ImageServer", single);                                                       
                                                   } while (true);
                                                   
                                                }
                                            );

                thread.Start(i);
            }
        }

        #endregion
    }
}
