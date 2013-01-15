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

using System.Collections.Specialized;
using System.IO;
using System.Net;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class ImageStreamingContext : StreamingContext
    {
        private DicomPixelData _pd;
        private string _serverAE;
        private int _frameNumber;
        private readonly string _nextSeriesUid;
        private readonly string _nextSopUid;

        #region PERFORMANCE TESTING STUFF
        private static DicomPixelData _testCompressedImage;
        private static DicomPixelData _testUncompressedImage;
        private readonly bool testCompressed;
        private readonly bool testUncompressed;
        #endregion

        public ImageStreamingContext(HttpListenerContext context)
        {
            Request = context.Request;
            Response = context.Response;
            NameValueCollection query = Request.QueryString;

            #region INIT STUFF FOR PERFORMANCE TESTING
            #if DEBUG 

            if (query["testcompressed"] != null)
            {
                testCompressed= true;
            }
            else if (query["testuncompressed"] != null)
            {
                testUncompressed = true;
            }
            if (_testCompressedImage == null)
            {
                using (Stream stream = typeof(ImageStreamingContext).Assembly.GetManifestResourceStream("ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Test.TestSamples.compressed.dcm"))
                {
                    DicomFile file = new DicomFile();
                    file.Load(stream);

                    _testCompressedImage = DicomPixelData.CreateFrom(file);
                }
                
            }

            if (_testUncompressedImage == null)
            {
                using (Stream stream = typeof(ImageStreamingContext).Assembly.GetManifestResourceStream("ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Test.TestSamples.uncompressed.dcm"))
                {
                    DicomFile file = new DicomFile();
                    file.Load(stream);

                    _testUncompressedImage = DicomPixelData.CreateFrom(file);
                }
                
            }
            #endif

            #endregion

            _frameNumber = 0;
            if (query["FrameNumber"] != null)
                int.TryParse(query["FrameNumber"], out _frameNumber);

            _nextSeriesUid = query["nextSeriesUid"];
            _nextSopUid = query["nextObjectUid"];

        }
       
        
        public string ImagePath
        {
            get
            {
                return StorageLocation.GetSopInstancePath(SeriesInstanceUid, ObjectUid);
            }
        }
        
        public DicomPixelData PixelData
        {
            get
            {
                #region PERFORMANCE TESTING CODE
                #if DEBUG 
                // If requested, the test images will be streamed directly from memory
                if (testCompressed)
                {
                    return _testCompressedImage;
                }
                else if (testUncompressed)
                {
                    return _testUncompressedImage;
                } 
                #endif
                #endregion

                if (_pd == null)
                {
                    PixelDataManager manager = PixelDataManager.GetInstance(StorageLocation);
                    _pd = manager.GetPixelData(SeriesInstanceUid, ObjectUid, _nextSeriesUid, _nextSopUid);
                }
                return _pd;
            }
        }

        public bool IsMultiFrame
        {
            get
            {
                return PixelData.NumberOfFrames > 1;
            }
        }

        public string ServerAE
        {
            get { return _serverAE; }
            set { _serverAE = value; }
        }

        public int FrameNumber
        {
            get { return _frameNumber; }
            set { _frameNumber = value; }
        }

    }
}
