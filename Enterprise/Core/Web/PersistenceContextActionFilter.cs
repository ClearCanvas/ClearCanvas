using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Web
{
	public class PersistenceContextActionFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var actionDescriptor = actionContext.ActionDescriptor as ReflectedHttpActionDescriptor;
			if (actionDescriptor == null)
				return;	// don't think should ever happen in practice

			var method = actionDescriptor.MethodInfo;
			var attr = AttributeUtils.GetAttribute<ServiceOperationAttribute>(method);
			if (attr == null)
				return;		// no persistence attribute found

			var controller = actionContext.ControllerContext.Controller as IPersistenceEnabledController;
			if (controller == null)
				throw new InvalidOperationException("Controller must implement IPersistenceEnabledController for ReadOperation/UpdateOperation attributes to take effect.");

			// establish appropriate persistence scope
			var pscope = new PersistenceScope(attr is UpdateOperationAttribute ? PersistenceContextType.Update : PersistenceContextType.Read, PersistenceScopeOption.Required);
			controller.PersistenceScope = pscope;
		}

		public override void OnActionExecuted(HttpActionExecutedContext actionContext)
		{
			var controller = actionContext.ActionContext.ControllerContext.Controller as IPersistenceEnabledController;
			if (controller == null)
				return;

			var pscope = controller.PersistenceScope;
			if (pscope == null)
				return;

			var shouldCommit = actionContext.Exception == null;

			try
			{
				// as long as no exceptions have been thrown, mark the scope as Complete
				if (shouldCommit)
				{
					controller.PreCommit();
					pscope.Complete();
				}
			}
			finally
			{
				controller.PersistenceScope = null;
				pscope.Dispose();
			}

			// be sure we've successfully disposed of the persistence scope before calling PostCommit()
			if(shouldCommit)
				controller.PostCommit();
		}
	}
}