using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.Web

{
	/// <summary>
	/// Converts exceptions to the appropriate HTTP Error representation.
	/// </summary>
	public class DefaultExceptionFilter : ExceptionFilterAttribute
	{
		#region Rule class

		public class Rule
		{
			private readonly Type _exceptionType;
			private readonly ErrorType _errorType;
			private readonly HttpStatusCode _statusCode;
			private readonly Func<Exception, ErrorType, HttpError> _errorFactory;

			public Rule(Type exceptionType, ErrorType errorType, HttpStatusCode statusCode)
				:this(exceptionType, errorType, statusCode, DefaultErrorFactory)
			{
			}

			public Rule(Type exceptionType, ErrorType errorType, HttpStatusCode statusCode, Func<Exception, ErrorType, HttpError> errorFactory)
			{
				_exceptionType = exceptionType;
				_errorType = errorType;
				_statusCode = statusCode;
				_errorFactory = errorFactory;
			}

			public Type ExceptionType
			{
				get { return _exceptionType; }
			}

			public HttpResponseMessage CreateResponse(HttpRequestMessage request, Exception e)
			{
				return request.CreateErrorResponse(_statusCode, _errorFactory(e, _errorType));
			}

			private static HttpError DefaultErrorFactory(Exception e, ErrorType errorType)
			{
				return errorType.CreateHttpError(e.Message);
			}
		}

		#endregion

		/// <summary>
		/// Define the default set of exception processing rules.
		/// </summary>
		private static readonly Rule[] _defaultRules = new[]
		{
			// Order of rules is significant. Rules are processed in order, so more specific exception types
			// should appear before their more general base types (just as in a typical 'catch' clause).

			//todo: add rule for concurrent modification

			// the requested resource was not found
			new Rule(typeof(ResourceNotFoundException), ErrorType.ObjectNotFound, HttpStatusCode.NotFound), 		
			
			// an entity *other than that referenced in the request path* was not found - hence BadRequest instead of NotFound
			new Rule(typeof(EntityNotFoundException), ErrorType.ObjectNotFound, HttpStatusCode.BadRequest),
 		
			new Rule(typeof(RequestValidationException), ErrorType.InvalidRequest, HttpStatusCode.BadRequest), 		
			new Rule(typeof(EntityValidationException), ErrorType.InvalidRequest, HttpStatusCode.BadRequest), 		
		
			new Rule(typeof(UserAccessDeniedException), ErrorType.LoginAccessDenied, HttpStatusCode.Forbidden),
 			new Rule(typeof(PasswordExpiredException), ErrorType.PasswordExpired, HttpStatusCode.BadRequest), 
		};

		/// <summary>
		/// Define the set of exception processing rules.
		/// </summary>
		private readonly List<Rule> _rules = new List<Rule>();

		public DefaultExceptionFilter()
		{
			_rules.AddRange(_defaultRules);
		}

		public List<Rule> Rules
		{
			get { return _rules; }
		}

		public override void OnException(HttpActionExecutedContext context)
		{
			var e = context.Exception;
			var exceptionType = e.GetType();

			Platform.Log(LogLevel.Error, e);

			// check rules in order, selecting the first one that matches
			var rule = _rules.FirstOrDefault(r => r.ExceptionType.IsAssignableFrom(exceptionType));
			if(rule != null)
			{
				context.Response = rule.CreateResponse(context.Request, e);
			}
		}
	}
}