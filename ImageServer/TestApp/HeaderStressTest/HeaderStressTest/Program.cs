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
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Threading;
using System.Xml;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using HeaderStressTest.services;

namespace HeaderStressTest
{
    class HeaderTestClient
    {
        List<StudyInfo> studies = new List<StudyInfo>();

        private AutoResetEvent waitHandle = new AutoResetEvent(false);
        private string _remoteAE;
        private string _remoteHost;
        private string _localAE;
        private int _remotePort;
        private int _delay;
        private string _fixedStudyInstanceUid;

        public string RemoteAE
        {
            get { return _remoteAE; }
            set { _remoteAE = value; }
        }

        public string LocalAE
        {
            get { return _localAE; }
            set { _localAE = value; }
        }

        public string RemoteHost
        {
            get { return _remoteHost; }
            set { _remoteHost = value; }
        }

        public int RemotePort
        {
            get { return _remotePort; }
            set { _remotePort = value; }
        }

        public string FixedStudyInstanceUid
        {
            get { return _fixedStudyInstanceUid; }
            set { _fixedStudyInstanceUid = value; }
        }

        public int Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        public void Run()
        {
            HeaderStreamingServiceClient client = null;

            studies = null;

            StudyInfo study = null;

            while (true)
            {
                Random r = new Random();

                if (String.IsNullOrEmpty(FixedStudyInstanceUid))
                {
                    bool refresh = false;
                    if (studies == null) 
                        refresh = r.Next() % 10 == 0;
                    else
                    {
                        refresh = r.NextDouble() < (1.0f/studies.Count/1000f);
                    }

                    if (refresh)
                    {
                        studies = new List<StudyInfo>();

                        CFindSCU cfind = new CFindSCU();
                        cfind.AETitle = LocalAE;
                        cfind.OnResultReceive += new CFindSCU.ResultReceivedHandler(cfind_OnResultReceive);
                        cfind.OnQueryCompleted += new CFindSCU.QueryCompletedHandler(cfind_OnQueryCompleted);
                        cfind.Query(RemoteAE, RemoteHost, RemotePort);
                        waitHandle.WaitOne();
                        
                    }
                   
                    
                }
                else
                {
                    studies = new List<StudyInfo>();
                    study = new StudyInfo();
                    study.StudyUid = FixedStudyInstanceUid;
                    studies.Add(study);
                }


                if (studies!=null && studies.Count > 0)
                {
                    
                    try
                    {
                        if (client==null)
                        {
                            client = new HeaderStreamingServiceClient();
                            client.ClientCredentials.ClientCertificate.SetCertificate(
                                    StoreLocation.LocalMachine, StoreName.My, 
                                    X509FindType.FindBySubjectName,
                                    Dns.GetHostName());

                            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

                        }

                        study = studies[r.Next(studies.Count - 1)];

                        
                        HeaderStreamingParameters param = new HeaderStreamingParameters();
                        param.ServerAETitle = RemoteAE;
                        param.StudyInstanceUID = study.StudyUid;
                        param.ReferenceID = Guid.NewGuid().ToString();
                        TimeSpanStatistics ts = new TimeSpanStatistics();
                        ts.Start();
                        Console.WriteLine("************ RETRIEVING... {0} **************", LocalAE);
                            
                        Stream input = client.GetStudyHeader(LocalAE, param);

                        if (input!=null)
                        {
                            string outputdir = Path.Combine("./output", LocalAE);
                            if (!Directory.Exists(outputdir))
                                Directory.CreateDirectory(outputdir);

                            string temp = Path.Combine(outputdir, study.StudyUid + ".xml");
                            Console.WriteLine("Reading");
                            using (FileStream output = new FileStream(temp, FileMode.OpenOrCreate))
                            {
                                GZipStream gzStream = new GZipStream(input, CompressionMode.Decompress);

                                byte[] buffer = new byte[32*1024*1024];
                                int size = gzStream.Read(buffer, 0, buffer.Length);
                                int count = 0;
                                while(size>0)
                                {
                                    output.Write(buffer, 0, size);
                                    count += size;
                                    Console.Write("\r{0} KB", count/1024);
                                    size = gzStream.Read(buffer, 0, buffer.Length); 
                                }
                                
                                output.Close();
                            }

                            using (FileStream output = new FileStream(temp, FileMode.Open))
                            {
                                var theMemento = new StudyXmlMemento();
                                Console.WriteLine("Reading into xml");
                                StudyXmlIo.Read(theMemento, output);
                                Console.WriteLine("Done");
                            }
                                

                        }
                        else
                        {
                            Console.WriteLine("{2} - {1,-16} {0,-64}... NOT FOUND", study.StudyUid, LocalAE, System.Diagnostics.Stopwatch.GetTimestamp()); 
                        
                        }

                        ts.End();
                        input.Close();

                        //File.Delete(temp);
                        Console.WriteLine("{3} - {2,-16} {0,-64}... OK {1}", study.StudyUid, ts.FormattedValue, LocalAE, System.Diagnostics.Stopwatch.GetTimestamp());

                    }
                    catch(TimeoutException)
                    {
                        // try again
                        Console.WriteLine("{2} - {1,-16} {0,-64}... TIMEOUT", study.StudyUid, LocalAE, System.Diagnostics.Stopwatch.GetTimestamp());
                    }
                    catch (Exception fault)
                    {
                        Console.WriteLine("{3} - {2,-16} {0,-64}... FAILED {1}", study.StudyUid, fault.Message, LocalAE, System.Diagnostics.Stopwatch.GetTimestamp());
                        if (client!=null)
                        {
                            client.Abort();
                            client.Close();
                            client = null;
                        }
                    }
                    
                    Thread.Sleep(r.Next(Delay));
                }
                else
                {
                    Thread.Sleep(r.Next(1000,3000));
                }
                
                
            }
          
        }

        void cfind_OnQueryCompleted()
        {
            waitHandle.Set();
        }

        void cfind_OnResultReceive(DicomAttributeCollection ds)
        {
            StudyInfo study = new StudyInfo();
            study.StudyUid = ds[DicomTags.StudyInstanceUid].GetString(0, "");
            studies.Add(study);
        }
    }

    

    class Program
    {
        static public Dictionary<string, string> ParseArguments(string[] args)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            for (int i=0; i<args.Length; i+=2)
            {
                string key = args[i];
                string value = args[i + 1];

                arguments[key] = value;
            }
            return arguments;
        }

        static void Main(string[] args)
        {
            if (args.Length==0)
            {
                Console.WriteLine(
                    "Usage:   -lae <LocalAETtile> -rae <ServerAE> -h <ServerHost> -p <ServerPort> [-n <NumberOfClients>] [-d DelayBetweenCalls] [-suid <StudyUIDToRetrieve>] ");
                return;
            }
            Dictionary<string, string> parms = ParseArguments(args);
            Random r = new Random();
            int clientCount = parms.ContainsKey("-n")? Int32.Parse(parms["-n"]) : r.Next(20) + 1;
            
            Console.WriteLine("{0} clients", clientCount);

            

            for(int i=0; i<clientCount; i++)
            {
                HeaderTestClient testClient = new HeaderTestClient();
                testClient.LocalAE = parms["-lae"] + "-" + i;
                testClient.RemoteAE = parms["-rae"];
                testClient.RemoteHost = parms["-h"];
                testClient.RemotePort = Int32.Parse(parms["-p"]);

                if (parms.ContainsKey("-d"))
                {
                    testClient.Delay = Int32.Parse(parms["-d"]);
                }
                else
                {
                    testClient.Delay = 500;
                }

                if (parms.ContainsKey("-suid"))
                {
                    testClient.FixedStudyInstanceUid = parms["-suid"];
                }


                Thread t = new Thread(delegate()
                                          {
                                              testClient.Run();
                                          });

                t.Start();
                Thread.Sleep(r.Next(200));
            }

        }

        
    }
}
