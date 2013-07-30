using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ClearCanvas.Enterprise.Core.Web
{
	/// <summary>
	/// This filter does basic validation of parameters passed to a controller method.
	/// </summary>
	public class RequestValidationFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			if (actionContext.ModelState.IsValid)
				return;

			// Return the validation errors in the response body.
			var errors = new Dictionary<string, IEnumerable<string>>();
			foreach (var kvp in actionContext.ModelState)
			{
				var v = kvp.Value;
				if(v.Errors.Any())
					errors[kvp.Key] = v.Errors.Select(e => e.ErrorMessage);
			}

			var httpError = ErrorType.InvalidRequest.CreateHttpError(SR.ErrorInvalidRequestArgument, errors);
			actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpError);
		}
	}
}