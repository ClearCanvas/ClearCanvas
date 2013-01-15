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
using System.Net;
using System.Web;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Streaming;

namespace WADOClient
{
    /// <summary>
    /// Types of data to be retrieved from the streaming server.
    /// </summary>
    public enum ContentTypes
    {
        /// <summary>
        /// Auto-detected by the server according to the protocol and the data
        /// </summary>
        NotSpecified,

        /// <summary>
        /// Indicates the client is expecting data to be in Dicom format.
        /// </summary>
        Dicom,

        /// <summary>
        /// Indicates the client is expecting data to be raw pixel data.
        /// </summary>
        RawPixel
    }

    class Program
    {
        static private string serverHost;
        static private int serverPort;
        private static ContentTypes type;
        private static string serverAE;
        private static string studyFolder;

        static bool repeat;

        static void Main(string[] args)
        {
            CommandLine cmdline = new CommandLine(args);
            IDictionary<string, string> parameters = cmdline.Named;

            serverAE = parameters["serverAE"];
            serverHost = parameters["host"];
            serverPort = int.Parse(parameters["port"]);
            studyFolder = parameters["folder"];
            
            if (parameters.ContainsKey("type"))
            {
                if (parameters["type"] == "dicom") type = ContentTypes.Dicom;
                else if (parameters["type"] == "pixel") type = ContentTypes.RawPixel;
                else
                    throw new Exception("Invalid 'type' value");
            }

            repeat = cmdline.Switches.ContainsKey("repeat");

            Console.WriteLine("Retrieve image in {0}", studyFolder);

            DirectoryInfo dirInfo = new DirectoryInfo(studyFolder);

            DirectoryInfo[] partitions = dirInfo.GetDirectories();

            try
            {
                do
                {
                    Random r = new Random();
                    DirectoryInfo partition = partitions[r.Next(partitions.Length)];

                    DirectoryInfo[] studydates = partition.GetDirectories();
                    if (studydates.Length>0)
                    {
                        // pick one
                        DirectoryInfo studyate = studydates[r.Next(studydates.Length)];

                        DirectoryInfo[] studies = studyate.GetDirectories();

                        if (studies.Length > 0)
                        {
                            // pick one
                            DirectoryInfo study = studies[r.Next(studies.Length)];

                            string path = study.FullName;
                            RetrieveImages(partition.Name, path);
                        }
                    }
                    

                    //if (repeat)
                    //    Thread.Sleep(r.Next(10000));

                } while (repeat);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        
        }

        
        private static void RetrieveImages(string serverAE, string studyPath)
        {
            Console.WriteLine("server={0}", serverAE);
            string baseUri = String.Format("http://{0}:{1}", serverHost, serverPort);
            StreamingClient client = new StreamingClient(new Uri(baseUri));
            int totalFrameCount = 0;
            
            DirectoryInfo directoryInfo = new DirectoryInfo(studyPath);
            string studyUid = directoryInfo.Name;

            RateStatistics frameRate = new RateStatistics("Speed", "frame");
            RateStatistics speed = new RateStatistics("Speed", RateType.BYTES);
            AverageRateStatistics averageSpeed = new AverageRateStatistics(RateType.BYTES);
            ByteCountStatistics totalSize = new ByteCountStatistics("Size");
            
            frameRate.Start();
            speed.Start();

            Console.WriteLine("\n------------------------------------------------------------------------------------------------------------------------");

            string[] seriesDirs = Directory.GetDirectories(studyPath);
            foreach(string seriesPath in seriesDirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(seriesPath);
                string seriesUid = dirInfo.Name;
                string[] objectUidPath = Directory.GetFiles(seriesPath, "*.dcm");
                
                foreach (string uidPath in objectUidPath)
                {
                    FileInfo fileInfo = new FileInfo(uidPath);
                    string uid = fileInfo.Name.Replace(".dcm", "");
                    Console.Write("{0,-64}... ", uid);
                                    
                    try
                    {
                        Stream imageStream;
                        StreamingResultMetaData imageMetaData;
                        FrameStreamingResultMetaData frameMetaData;
                                   
                        switch(type)
                        {
                            case ContentTypes.Dicom:
                                imageStream = client.RetrieveImage(serverAE, studyUid, seriesUid, uid, out imageMetaData);
                                totalFrameCount++;
                                averageSpeed.AddSample(imageMetaData.Speed);
                                totalSize.Value += (ulong)imageMetaData.ContentLength;

                                Console.WriteLine("1 dicom sop [{0,10}] in {1,12}\t[mime={2}]", ByteCountFormatter.Format((ulong)imageStream.Length), TimeSpanFormatter.Format(imageMetaData.Speed.ElapsedTime), imageMetaData.ResponseMimeType);
                                
                                break;

                            case ContentTypes.RawPixel:
                                TimeSpanStatistics elapsedTime = new TimeSpanStatistics();
                                elapsedTime.Start();
                                ulong instanceSize = 0;
                                int frameCount = 0;
                                do
                                {
									RetrievePixelDataResult result = client.RetrievePixelData(serverAE, studyUid, seriesUid, uid, frameCount);
                                	frameMetaData = result.MetaData;
									totalFrameCount++;
                                    frameCount++;
                                    averageSpeed.AddSample(frameMetaData.Speed);
                                    totalSize.Value += (ulong)frameMetaData.ContentLength;
                                    instanceSize += (ulong)frameMetaData.ContentLength;

                                } while (!frameMetaData.IsLast);

                                elapsedTime.End();
                                Console.WriteLine("{0,3} frame(s) [{1,10}] in {2,12}\t[mime={3}]", frameCount, ByteCountFormatter.Format(instanceSize), elapsedTime.FormattedValue, frameMetaData.ResponseMimeType);
                                break;

                            default:

                                imageStream = client.RetrieveImage(serverAE, studyUid, seriesUid, uid, out imageMetaData);
                                totalFrameCount++;
                                averageSpeed.AddSample(imageMetaData.Speed);
                                totalSize.Value += (ulong)imageMetaData.ContentLength;

                                Console.WriteLine("1 object [{0,10}] in {1,12}\t[mime={2}]", ByteCountFormatter.Format((ulong)imageStream.Length), TimeSpanFormatter.Format(imageMetaData.Speed.ElapsedTime), imageMetaData.ResponseMimeType);

                                break;
                        }

                    }
                    catch(Exception ex)
                    {
                        if (ex is WebException)
                        {
                            HttpWebResponse rsp = ( (ex as WebException).Response as HttpWebResponse);
                        
                            string msg = String.Format("Error: {0} : {1}", rsp.StatusCode,HttpUtility.HtmlDecode(rsp.StatusDescription)
                                );
                            Console.WriteLine(msg);
                        }
                        else
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            frameRate.SetData(totalFrameCount);
            frameRate.End();
            speed.SetData(totalSize.Value);
            speed.End();


            Console.WriteLine("\nTotal {0,3} image(s)/frame(s) [{1,10}] in {2,12}   ==>  [ Speed: {3,12} or {4,12}]",
                    totalFrameCount, totalSize.FormattedValue,
                    TimeSpanFormatter.Format(frameRate.ElapsedTime),
                    frameRate.FormattedValue,
                    speed.FormattedValue
                    );

                                        
        }
    }
}
