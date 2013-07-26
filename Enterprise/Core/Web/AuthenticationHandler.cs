using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Enterprise.Core.Web
{
	public class AuthenticationHandler : DelegatingHandler
	{
		private const string CustomScheme = "CCToken";


		public AuthenticationHandler()
		{
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			try
			{
				var authHeader = request.Headers.Authorization;
				TryAuthenticate(authHeader);

				return base.SendAsync(request, cancellationToken).ContinueWith(
					task =>
					{
						var response = task.Result;
						if (response.StatusCode == HttpStatusCode.Unauthorized)
						{
							SetAuthenticateHeader(response);
						}

						return response;
					});
			}
			catch
			{
				// we shouldn't really end up here, unless something has gone quite wrong
				return Task<HttpResponseMessage>.Factory.StartNew(() =>
				{
					var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
					SetAuthenticateHeader(response);

					return response;
				});
			}
		}

		private static void TryAuthenticate(AuthenticationHeaderValue authHeader)
		{
			if (authHeader == null)
				return;

			if (authHeader.Scheme != CustomScheme)
				return;

			if (string.IsNullOrEmpty(authHeader.Parameter))
				return;

			var parameterParts = authHeader.Parameter.Split(':');
			var userId = parameterParts[0];
			var sessionToken = new SessionToken(parameterParts[1]);

			try
			{
				ValidateSessionResponse response = null;
				Platform.GetService<IAuthenticationService>(
					service => { response = service.ValidateSession(new ValidateSessionRequest(userId, sessionToken) {GetAuthorizations = true}); });

				var identity = new GenericIdentity(userId);
				var principal = DefaultPrincipal.CreatePrincipal(
					identity,
					response.SessionToken,
					response.AuthorityTokens);

				SetPrincipal(principal);
			}
			catch (FaultException<InvalidUserSessionException>)
			{
				// if authentication fails, we simply do not set a principle
				// the request is treated as anonymous
			}
		}

		private static void SetAuthenticateHeader(HttpResponseMessage response)
		{
			response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(CustomScheme));
		}

		private static void SetPrincipal(IPrincipal principal)
		{
			Thread.CurrentPrincipal = principal;
			if (HttpContext.Current != null)
			{
				HttpContext.Current.User = principal;
			}
		}
	}
}