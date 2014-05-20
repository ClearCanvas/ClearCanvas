using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using Castle.Core.Interceptor;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Advice class responsible for parsing the culture directive message headers that the client has specified 
	/// in the request establishing the appropriate context for the current server thread.
	/// </summary>
	class CultureServerSideAdvice : CultureAdviceBase, IInterceptor
	{
		#region Implementation of IInterceptor

		public void Intercept(IInvocation invocation)
		{
			var initialCulture = Thread.CurrentThread.CurrentCulture;
			var initialUiCulture = Thread.CurrentThread.CurrentUICulture;

			CultureInfo clientCulture, clientUiCulture;
			var cultureDirective = ReadMessageHeaders(OperationContext.Current);

			// if a culture directive was specified, use it
			if (ParseCultureDirective(cultureDirective, out clientCulture, out clientUiCulture))
			{
				Platform.Log(LogLevel.Debug, "Client Culture Header [{0}]: Culture={1}, UICulture={2}",
					invocation.MethodInvocationTarget.Name, cultureDirective.Culture, cultureDirective.UICulture);
				try
				{
					Thread.CurrentThread.CurrentCulture = clientCulture;
					Thread.CurrentThread.CurrentUICulture = clientUiCulture;
					invocation.Proceed();
				}
				finally
				{
					// always return the thread to its original culture
					Thread.CurrentThread.CurrentCulture = initialCulture;
					Thread.CurrentThread.CurrentUICulture = initialUiCulture;
				}
			}
			else
			{
				invocation.Proceed();
			}
		}

		#endregion

		private static bool ParseCultureDirective(CultureDirective directive, out CultureInfo culture, out CultureInfo uiCulture)
		{
			if (directive == null)
			{
				culture = null;
				uiCulture = null;
				return false;
			}

			culture = SafeGetCulture(directive.Culture);
			uiCulture = SafeGetCulture(directive.UICulture);

			return true;
		}

		private static CultureInfo SafeGetCulture(string code)
		{
			if (string.IsNullOrEmpty(code))
				return CultureInfo.InvariantCulture;

			try
			{
				return CultureInfo.GetCultureInfo(code);
			}
			catch (Exception)
			{
				return CultureInfo.InvariantCulture;
			}
		}
	}
}
