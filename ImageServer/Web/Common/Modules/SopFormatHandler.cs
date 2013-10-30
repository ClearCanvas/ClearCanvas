using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common.ServiceModel;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Common.Modules
{
	public class PdfSopFormatHandler : SopFormatHandler, IHttpHandler
	{
		#region Overrides of ViewSopHandlerBase

		protected override string Name
		{
			get { return "View PDF"; }
		}

		protected override string ContentType
		{
			get { return "application/pdf"; }
		}

		#endregion
	}

	/*
	public class JpgSopFormatHandler : SopFormatHandler, IHttpHandler
	{
		#region Overrides of ViewSopHandlerBase

		protected override string Name
		{
			get { return "View JPG"; }
		}

		protected override string ContentType
		{
			get { return "image/jpeg"; }
		}

		#endregion
	}*/

	public abstract class SopFormatHandler
	{
		private const string StudyUrl = "study=";
		private const string SeriesUrl = "series=";
		private const string SopUrl = "sop=";
		private const string AeTitleUrl = "ae=";

		protected abstract string Name { get; }
		protected abstract string ContentType { get; }

		public void ProcessRequest(HttpContext context)
		{
			Platform.Log(LogLevel.Debug, "{0}: Received request from {1}", Name, context.Request.UserHostAddress);
			if (SessionManager.Current == null)
				throw new UserAccessDeniedException();
			if (!SessionManager.Current.User.IsInRole(ImageServer.Common.Authentication.AuthorityTokens.Study.View))
				throw new UserAccessDeniedException();

			string url = context.Request.RawUrl;

			string studyInstanceUid = string.Empty;
			string aeTitle = string.Empty;
			string seriesInstanceUid = string.Empty;
			string sopInstanceUid = string.Empty;

			string[] strings = url.Split(new[] {'/', '?', '&'});

			foreach (string s in strings)
			{
				if (s.StartsWith(StudyUrl))
				{
					studyInstanceUid = s.Substring(StudyUrl.Length);
				}
				if (s.StartsWith(SeriesUrl))
				{
					seriesInstanceUid = s.Substring(SeriesUrl.Length);
				}
				if (s.StartsWith(SopUrl))
				{
					sopInstanceUid = s.Substring(SopUrl.Length);
				}
				if (s.StartsWith(AeTitleUrl))
				{
					aeTitle = s.Substring(AeTitleUrl.Length);
				}
			}

			if (string.IsNullOrEmpty(studyInstanceUid) || string.IsNullOrEmpty(seriesInstanceUid) || string.IsNullOrEmpty(sopInstanceUid) || string.IsNullOrEmpty(aeTitle))
			{
				Platform.Log(LogLevel.Debug, "{0}: request does not contain all required parameters: {1}", Name, url);

				//TODO: redirect to error page?
				return;
			}

			try
			{
				context.Response.ContentType = ContentType;
				var request = new GetSopRequest
				              	{
									ServerAE = aeTitle,
									StudyInstanceUid = studyInstanceUid,
									SeriesInstanceUid = seriesInstanceUid,
									SopInstanceUid = sopInstanceUid
				              	};

				Platform.GetService(delegate(ISopDataService s)
				                    	{
											using (var pdfStream = s.GetSop(request))
											{
												using (var ms = new MemoryStream())
												{
													//NOTE: the stream coming off the network is not seekable, so we can't read it if it's not Part 10.
													//Since we know it's a document and not an image, we'll just copy it to a memory stream.
													pdfStream.CopyTo(ms);
													ms.Position = 0;

													var file = new DicomFile();
													file.Load(ms, DicomTagDictionary.GetDicomTag(DicomTags.MimeTypeOfEncapsulatedDocument), DicomReadOptions.Default | DicomReadOptions.DoNotStorePixelDataInDataSet);
													var document = new EncapsulatedPdfIod(file.DataSet);
													var documentBytes = document.EncapsulatedDocument.EncapsulatedDocument;
													context.Response.BinaryWrite(documentBytes);
												}
											}
				                    	});
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "{0}:  Error occurred while retrieving report", Name);

				//TODO: redirect to error page?
				throw;
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}