using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace ClearCanvas.Enterprise.Core.Web
{
	public class RawXmlMediaTypeFormatter : MediaTypeFormatter
	{
		public RawXmlMediaTypeFormatter()
		{
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
		}

		public override bool CanReadType(Type type)
		{
			return true;
		}

		public override bool CanWriteType(Type type)
		{
			return false; // implement this later if needed
		}

		public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
		{
			var task = content.ReadAsStringAsync();
			return task.ContinueWith(t => (object) t.Result);
		}
	}
}