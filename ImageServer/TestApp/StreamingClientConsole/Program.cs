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
using System.Text;
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using System.IO;
using System.IO.Compression;
using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;
using System.Threading;

namespace StreamingClientConsole
{
    class Program
    {
        private static TestStreamingClientCommandLine _cmdLine;
        private static AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private static List<StudyInfo> _studies = new List<StudyInfo>();

        static void Main(string[] args)
        {            
            //get server details from args
             _cmdLine = new TestStreamingClientCommandLine();
            try
            {
                _cmdLine.Parse(args);       

                //perform query for studies; maybe optional search parameters in xml?

                _studies = new List<StudyInfo>();

                Console.WriteLine("Querying for study list from AE: {0} (Host: {1} / Port: {2})", _cmdLine.AeTitle, _cmdLine.Host,_cmdLine.DicomServicePort);
               
                CFindSCU cfind = new CFindSCU();
                cfind.AETitle = "TESTSTREAM";
                cfind.OnResultReceive += new CFindSCU.ResultReceivedHandler(cfind_OnResultReceive);
                cfind.OnQueryCompleted += new CFindSCU.QueryCompletedHandler(cfind_OnQueryCompleted);
                cfind.Query(_cmdLine.AeTitle, _cmdLine.Host, _cmdLine.DicomServicePort);
                _waitHandle.WaitOne();

                if (_studies != null && _studies.Count > 0)
                {
                    foreach (StudyInfo s in _studies)
                    {
                        //iterate through in thread:
                        //  fetching header
                        //  fetching each sop
                        //  log after each sop
                        
                        Console.WriteLine("Retrieving header for Study: {0}", s.StudyUid);
                        XmlDocument doc = RetrieveHeaderXml(s.StudyUid);

                        Uri uri = new Uri(String.Format("http://{0}:{1}/WADO", _cmdLine.Host, _cmdLine.WadoServicePort));
                        StreamingClient client = new StreamingClient(uri);
                        byte[] pixelData;

                        Console.WriteLine("Streaming images for Study: {0}", s.StudyUid);
                        StudyXml studyXml = new StudyXml(s.StudyUid);
                        studyXml.SetMemento(new StudyXmlMemento {Document = doc});

                        foreach (SeriesXml seriesXml in studyXml)
                        {
                            foreach (InstanceXml instanceXml in seriesXml)
                            {
                                pixelData = client.RetrievePixelData(
                                    _cmdLine.AeTitle,
                                    s.StudyUid,
                                    seriesXml.SeriesInstanceUid,
                                    instanceXml.SopInstanceUid,
                                    0).GetPixelData();                                
                            }
                        }
                    }
                }
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                _cmdLine.PrintUsage(Console.Out);
            }
        }

        private static void cfind_OnQueryCompleted()
        {
            _waitHandle.Set();
        }

        private static void cfind_OnResultReceive(DicomAttributeCollection ds)
        {
            StudyInfo study = new StudyInfo();
            study.StudyUid = ds[DicomTags.StudyInstanceUid].GetString(0, "");
            _studies.Add(study);
        }

        private static XmlDocument RetrieveHeaderXml(string studyInstanceUid)
        {
            HeaderStreamingParameters headerParams = new HeaderStreamingParameters();
            headerParams.StudyInstanceUID = studyInstanceUid;
            headerParams.ServerAETitle = _cmdLine.AeTitle;
            headerParams.ReferenceID = Guid.NewGuid().ToString();

            string uri = String.Format("http://{0}:{1}/HeaderStreaming/HeaderStreaming", _cmdLine.Host, _cmdLine.HeaderServicePort);
            EndpointAddress endpoint = new EndpointAddress(uri);

            HeaderStreamingServiceClient client =
                new HeaderStreamingServiceClient(
                "BasicHttpBinding_IHeaderStreamingService",
                endpoint);

            try
            {
                client.Open();
                Stream stream = client.GetStudyHeader(_cmdLine.AeTitle, headerParams);
                client.Close();
                return DecompressHeaderStreamToXml(stream);
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        private static XmlDocument DecompressHeaderStreamToXml(Stream stream)
        {
            GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress);

            XmlDocument doc = new XmlDocument();
            doc.Load(gzStream);
            return doc;
        }
    }

    class HeaderStreamingServiceClient : ClientBase<IHeaderStreamingService>, IHeaderStreamingService
    {
        public HeaderStreamingServiceClient()
        {

        }

        public HeaderStreamingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {

        }

        public Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters)
        {
            return base.Channel.GetStudyHeader(callingAETitle, parameters);
        }
    }

    class TestStreamingClientCommandLine : CommandLine
    {
        private string _aeTitle;
        private string _host;
        private int _dicomServicePort;
        private string _headerServicePort;
        private string _wadoServicePort;
        private int _delay;                

        [CommandLineParameter("ae", "Server AETITLE")]
        public string AeTitle
        {
            get { return _aeTitle; }
            set { _aeTitle = value; }
        }

        [CommandLineParameter("dicomport", "dp", "Server dicom listening port")]
        public int DicomServicePort
        {
            get { return _dicomServicePort; }
            set { _dicomServicePort = value; }
        }

        [CommandLineParameter("host", "h", "Server hostname")]
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        [CommandLineParameter("headerport", "hp", "Server header service port")]
        public string HeaderServicePort
        {
            get { return _headerServicePort; }
            set { _headerServicePort = value; }
        }

        [CommandLineParameter("wadoport", "wp", "Server wado service port")]
        public string WadoServicePort
        {
            get { return _wadoServicePort; }
            set { _wadoServicePort = value; }
        }

        [CommandLineParameter("delay", "d", "Delay in seconds in between each study")]
        public int Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }
    }
}
