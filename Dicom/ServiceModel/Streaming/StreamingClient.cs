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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;
using System.Net.Cache;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
	public class RetrievePixelDataResult
	{
		private readonly FrameStreamingResultMetaData _metaData;
		private DicomCompressedPixelData _compressedPixelData;
		private byte[] _pixelData;

		internal RetrievePixelDataResult(byte[] uncompressedPixelData, FrameStreamingResultMetaData resultMetaData)
		{
			_pixelData = uncompressedPixelData;
			_metaData = resultMetaData;
		}

		internal RetrievePixelDataResult(DicomCompressedPixelData compressedPixelData, FrameStreamingResultMetaData resultMetaData)
		{
			_compressedPixelData = compressedPixelData;
			_metaData = resultMetaData;
		}

		public FrameStreamingResultMetaData MetaData
		{
			get { return _metaData; }	
		}

		public byte[] GetPixelData()
		{
			if (_compressedPixelData != null)
			{
				try
				{
					byte[] uncompressed = _compressedPixelData.GetFrame(0);

					_pixelData = uncompressed;
					_compressedPixelData = null;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Error occurred while decompressing the pixel data: {0}", ex.Message));
				}
			}

			return _pixelData;
		}
	}

    /// <summary>
    /// Represents a web client that can be used to retrieve study images or pixel data from a streaming server using WADO protocol.
    /// </summary>
    public class StreamingClient
    {
        private readonly Uri _baseUri;

        /// <summary>
        /// Creates an instance of <see cref="StreamingClient"/> to connect to a streaming server.
        /// </summary>
        /// <param name="baseUri">Base Uri to the location where the streaming server is located (eg. http://localhost:1000/wado). </param>
        public StreamingClient(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        #region Public Methods

		public RetrievePixelDataResult RetrievePixelData(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, int frame)
        {
			try
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				FrameStreamingResultMetaData result = new FrameStreamingResultMetaData();
				StringBuilder url = new StringBuilder();

				if (_baseUri.ToString().EndsWith("/"))
				{
					url.AppendFormat("{0}{1}", _baseUri, serverAE);
				}
				else
				{
					url.AppendFormat("{0}/{1}", _baseUri, serverAE);
				}

				url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}", studyInstanceUID, seriesInstanceUID, sopInstanceUid);
				url.AppendFormat("&frameNumber={0}", frame);
				url.AppendFormat("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas"));

				result.Speed.Start();

				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url.ToString());
				request.Accept = "application/dicom,application/clearcanvas,image/jpeg";
				request.Timeout = (int) TimeSpan.FromSeconds(StreamingSettings.Default.ClientTimeoutSeconds).TotalMilliseconds;
				request.KeepAlive = false;

				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				if (response.StatusCode != HttpStatusCode.OK)
				{
					throw new StreamingClientException(response.StatusCode, HttpUtility.HtmlDecode(response.StatusDescription));
				}

				Stream responseStream = response.GetResponseStream();
				BinaryReader reader = new BinaryReader(responseStream);
				byte[] buffer = reader.ReadBytes((int) response.ContentLength);
				reader.Close();
				responseStream.Close();
				response.Close();

				result.Speed.SetData(buffer.Length);
				result.Speed.End();

				result.ResponseMimeType = response.ContentType;
				result.Status = response.StatusCode;
				result.StatusDescription = response.StatusDescription;
				result.Uri = response.ResponseUri;
				result.ContentLength = buffer.Length;
				result.IsLast = (response.Headers["IsLast"] != null && bool.Parse(response.Headers["IsLast"]));

				clock.Stop();
				PerformanceReportBroker.PublishReport("Streaming", "RetrievePixelData", clock.Seconds);

				RetrievePixelDataResult pixelDataResult;
				if (response.Headers["Compressed"] != null && bool.Parse(response.Headers["Compressed"]))
					pixelDataResult = new RetrievePixelDataResult(CreateCompressedPixelData(response, buffer), result);
				else
					pixelDataResult = new RetrievePixelDataResult(buffer, result);

				return pixelDataResult;
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse)
				{
					HttpWebResponse response = (HttpWebResponse) ex.Response;
					throw new StreamingClientException(response.StatusCode, HttpUtility.HtmlDecode(response.StatusDescription));
				}
				throw new StreamingClientException(StreamingClientExceptionType.Network, ex);
			}
		}

		public Stream RetrieveImageHeader(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid)
		{
			StreamingResultMetaData metaInfo;
			return RetrieveImageHeader(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid, DicomTags.PixelData, out metaInfo);
		}

		public Stream RetrieveImageHeader(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, uint stopTag, out StreamingResultMetaData metaInfo)
		{
        	string imageUrl = BuildImageUrl(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid);
			imageUrl = imageUrl + String.Format("&stopTag={0:x8}", stopTag);
			imageUrl = imageUrl + String.Format("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas-header"));
        	return RetrieveImageData(imageUrl, out metaInfo);
		}
		
		public Stream RetrieveImage(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid)
        {
            StreamingResultMetaData result;
            return RetrieveImage(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid, out result);
        }

        public Stream RetrieveImage(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, out StreamingResultMetaData metaInfo)
        {
        	string imageUrl = BuildImageUrl(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid);
			imageUrl = imageUrl + String.Format("&contentType={0}", HttpUtility.HtmlEncode("application/dicom"));
        	return RetrieveImageData(imageUrl, out metaInfo);
        }

        public Stream RetrievePdf(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid)
        {
            StreamingResultMetaData result;
            return RetrievePdf(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid, out result);
        }

        public Stream RetrievePdf(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, out StreamingResultMetaData metaInfo)
        {
            string imageUrl = BuildImageUrl(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid);
            imageUrl = imageUrl + String.Format("&contentType={0}", HttpUtility.HtmlEncode("application/pdf"));
            return RetrieveImageData(imageUrl, out metaInfo);
        }

        #endregion Public Methods

		#region Private Methods

		private string BuildImageUrl(string serverAE, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid)
		{
			Platform.CheckForEmptyString(serverAE, "serverAE");
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");
			Platform.CheckForEmptyString(seriesInstanceUid, "seriesInstanceUid");
			Platform.CheckForEmptyString(sopInstanceUid, "sopInstanceUid");

			var url = new StringBuilder();
		    url.AppendFormat(_baseUri.ToString().EndsWith("/") ? "{0}{1}" : "{0}/{1}", _baseUri, serverAE);

		    url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}", studyInstanceUid, seriesInstanceUid, sopInstanceUid);
			return url.ToString();
		}

		private static MemoryStream RetrieveImageData(string url, out StreamingResultMetaData result)
		{
			try
			{
				result = new StreamingResultMetaData();

				result.Speed.Start();

				var request = (HttpWebRequest) WebRequest.Create(url.ToString());
				request.Accept = "application/dicom,application/clearcanvas,application/clearcanvas-header,image/jpeg,application/pdf";
				request.Timeout = (int) TimeSpan.FromSeconds(StreamingSettings.Default.ClientTimeoutSeconds).TotalMilliseconds;
				request.KeepAlive = false;

				HttpWebResponse response = (HttpWebResponse) request.GetResponse();
				if (response.StatusCode != HttpStatusCode.OK)
				{
					throw new StreamingClientException(response.StatusCode, HttpUtility.HtmlDecode(response.StatusDescription));
				}

				Stream responseStream = response.GetResponseStream();
				BinaryReader reader = new BinaryReader(responseStream);
				byte[] buffer = reader.ReadBytes((int) response.ContentLength);
				reader.Close();
				responseStream.Close();
				response.Close();

				result.Speed.SetData(buffer.Length);
				result.Speed.End();

				result.ResponseMimeType = response.ContentType;
				result.Status = response.StatusCode;
				result.StatusDescription = response.StatusDescription;
				result.Uri = response.ResponseUri;
				result.ContentLength = buffer.Length;

				return new MemoryStream(buffer);
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse)
				{
					HttpWebResponse response = (HttpWebResponse) ex.Response;
					throw new StreamingClientException(response.StatusCode, HttpUtility.HtmlDecode(response.StatusDescription));
				}
				throw new StreamingClientException(StreamingClientExceptionType.Network, ex);
			}
		}

		private static DicomCompressedPixelData CreateCompressedPixelData(HttpWebResponse response, byte[] pixelDataBuffer)
		{
			string transferSyntaxUid = response.Headers["TransferSyntaxUid"];
			TransferSyntax transferSyntax = TransferSyntax.GetTransferSyntax(transferSyntaxUid);
			ushort bitsAllocated = ushort.Parse(response.Headers["BitsAllocated"], CultureInfo.InvariantCulture);
			ushort bitsStored = ushort.Parse(response.Headers["BitsStored"], CultureInfo.InvariantCulture);
			ushort height = ushort.Parse(response.Headers["ImageHeight"], CultureInfo.InvariantCulture);
			ushort width = ushort.Parse(response.Headers["ImageWidth"], CultureInfo.InvariantCulture);
			ushort samples = ushort.Parse(response.Headers["SamplesPerPixel"], CultureInfo.InvariantCulture);

			DicomAttributeCollection collection = new DicomAttributeCollection();
			collection[DicomTags.BitsAllocated].SetUInt16(0, bitsAllocated);
			collection[DicomTags.BitsStored].SetUInt16(0, bitsStored);
			collection[DicomTags.HighBit].SetUInt16(0, ushort.Parse(response.Headers["HighBit"], CultureInfo.InvariantCulture));
			collection[DicomTags.Rows].SetUInt16(0, height);
			collection[DicomTags.Columns].SetUInt16(0, width);
			collection[DicomTags.PhotometricInterpretation].SetStringValue(response.Headers["PhotometricInterpretation"]);
			collection[DicomTags.PixelRepresentation].SetUInt16(0, ushort.Parse(response.Headers["PixelRepresentation"], CultureInfo.InvariantCulture));
			collection[DicomTags.SamplesPerPixel].SetUInt16(0, samples);
			collection[DicomTags.DerivationDescription].SetStringValue(response.Headers["DerivationDescription"]);
			collection[DicomTags.LossyImageCompression].SetStringValue(response.Headers["LossyImageCompression"]);
			collection[DicomTags.LossyImageCompressionMethod].SetStringValue(response.Headers["LossyImageCompressionMethod"]);
			collection[DicomTags.LossyImageCompressionRatio].SetFloat32(0, float.Parse(response.Headers["LossyImageCompressionRatio"], CultureInfo.InvariantCulture));
			collection[DicomTags.PixelData] = new DicomFragmentSequence(DicomTags.PixelData);

			ushort planar;
			if (ushort.TryParse(response.Headers["PlanarConfiguration"], NumberStyles.Integer, CultureInfo.InvariantCulture, out planar))
				collection[DicomTags.PlanarConfiguration].SetUInt16(0, planar);

			DicomCompressedPixelData cpd = new DicomCompressedPixelData(collection);
			cpd.TransferSyntax = transferSyntax;
			cpd.AddFrameFragment(pixelDataBuffer);

			return cpd;
		}

		#endregion
	}
}