using System.ServiceModel;
using System.Threading;
using Castle.Core.Interceptor;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Advice class responsible for capturing the current culture settings under which the client is running,
	/// and sending them to the server as a request header.
	/// </summary>
	class CultureClientSideAdvice : CultureAdviceBase, IInterceptor
	{
		#region Implementation of IInterceptor

		public void Intercept(IInvocation invocation)
		{
			var cultureDirective = new CultureDirective
									{
										Culture = Thread.CurrentThread.CurrentCulture.Name,
										UICulture = Thread.CurrentThread.CurrentUICulture.Name
									};

			WriteMessageHeaders(cultureDirective, OperationContext.Current);

			invocation.Proceed();
		}

		#endregion
	}
}
