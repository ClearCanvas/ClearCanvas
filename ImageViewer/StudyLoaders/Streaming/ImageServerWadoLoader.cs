using System;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Streaming;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class ImageServerFramePixelData : IFramePixelData
	{
		public ImageServerFramePixelData(RetrievePixelDataResult result)
		{
			InternalResult = result;
		}

		#region IFramePixelData Members

		public long BytesReceived
		{
			get { return InternalResult.MetaData.ContentLength; }
		}

		public byte[] GetPixelData(out string photometricInterpretation)
		{
			return InternalResult.GetPixelData(out photometricInterpretation);
		}

		#endregion

		private RetrievePixelDataResult InternalResult { get; set; }
	}

	internal class ImageServerWadoLoader : IDicomFileLoader
	{
	    private readonly string _aeTitle;
        private readonly Uri _wadoUri;

		public ImageServerWadoLoader(string hostName, string aeTitle, int wadoServicePort)
		{
		    _aeTitle = aeTitle;
            _wadoUri = new Uri(string.Format(StreamingSettings.Default.FormatWadoUriPrefix, hostName, wadoServicePort));
		}

		#region Implementation of IDicomFileLoader

		public bool CanLoadCompleteHeader
		{
			get { return true; }
		}

		public bool CanLoadPixelData
		{
			get { return false; }
		}

		public bool CanLoadFramePixelData
		{
			get { return true; }
		}

		public DicomFile LoadDicomFile(LoadDicomFileArgs args)
		{
			try
			{
				var client = new StreamingClient(_wadoUri);
				var file = new DicomFile();
				using (var stream = client.RetrieveImageHeader(_aeTitle, args.StudyInstanceUid, args.SeriesInstanceUid, args.SopInstanceUid))
				{
					file.Load(stream);
				}

				return file;
			}
			catch (Exception e)
			{
				throw TranslateStreamingException(e);
			}
		}

		public IFramePixelData LoadFramePixelData(LoadFramePixelDataArgs args)
		{
			try
			{
				var client = new StreamingClient(_wadoUri);
				var result = client.RetrievePixelData(_aeTitle, args.StudyInstanceUid, args.SeriesInstanceUid, args.SopInstanceUid, args.FrameNumber - 1);
				return new ImageServerFramePixelData(result);
			}
			catch (Exception e)
			{
				throw TranslateStreamingException(e);
			}
		}

		#endregion

		/// <summary>
		/// Translates possible exceptions thrown by <see cref="StreamingClient"/> and related classes into standardized, user-friendly error messages.
		/// </summary>
		private static Exception TranslateStreamingException(Exception exception)
		{
			if (exception is StreamingClientException)
			{
				switch (((StreamingClientException)exception).Type)
				{
					case StreamingClientExceptionType.Access:
						return new InvalidOperationException(SR.MessageStreamingAccessException, exception);
					case StreamingClientExceptionType.Network:
						return new IOException(SR.MessageStreamingNetworkException, exception);
					case StreamingClientExceptionType.Protocol:
					case StreamingClientExceptionType.Server:
					case StreamingClientExceptionType.UnexpectedResponse:
					case StreamingClientExceptionType.Generic:
					default:
						return new Exception(SR.MessageStreamingGenericException, exception);
				}
			}
			return new Exception(SR.MessageStreamingGenericException, exception);
		}
	}
}