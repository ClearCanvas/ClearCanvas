using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Enterprise.Common;
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

			string[] strings = url.Split(new[] { '/', '?', '&' });

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

			ServerPartition ae = ServerPartitionMonitor.Instance.GetPartition(aeTitle);
			if (ae == null)
			{
				//TODO: redirect to error page?
				Platform.Log(LogLevel.Debug, "{0}: Unexpected incorrect server information.", Name);
				throw new Exception("Unexpected incorrect server information.");
			}

			var uri = new Uri(string.Format("http://{0}:{1}/wado", "localhost", 1000));

			try
			{
				Platform.Log(LogLevel.Debug, "{0}: Retrieving data from {0}", Name, uri);

				var client = new StreamingClient(uri);
				var args = new StreamingClientArgs(aeTitle, studyInstanceUid, seriesInstanceUid, sopInstanceUid) {ContentType = ContentType};
				Stream input = client.RetrieveImageData(args);

				if (input == null)
				{
					Platform.Log(LogLevel.Debug, "{0}: server did not return any data", Name);
				}

				context.Response.ContentType = ContentType;

				var buffer = new byte[64 * 1024];
				using (var ms = new MemoryStream())
				{
					int read;
					while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
					{
						ms.Write(buffer, 0, read);
					}
					context.Response.BinaryWrite(ms.ToArray());
				}
			}
			catch (StreamingClientException ex)
			{
				Platform.Log(LogLevel.Error, ex, "{0}:  Error occurred while retrieving report", Name);

				//TODO: redirect to error page?
				throw;
			}
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}
	}
}
