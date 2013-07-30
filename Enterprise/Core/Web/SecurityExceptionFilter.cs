using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http.Filters;

namespace ClearCanvas.Enterprise.Core.Web
{
	public class SecurityExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext context)
		{
			if (context.Exception is SecurityException)
			{
				context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Forbidden, context.Exception.Message);
			}
		}
	}
}