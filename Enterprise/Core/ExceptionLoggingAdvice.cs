#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.ServiceModel;
using Castle.Core.Interceptor;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Advice class responsbile for logging unhandled exceptions thrown from service operation methods.
    /// </summary>
    public class ExceptionLoggingAdvice : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
			catch(FaultException e)
			{
				// if it was promoted to a fault, it isn't really an error as far as the server is concerned,
				// but more of an expected type of failure that is being communicated back to the client
				// Log at info level, and don't include stack trace since it isn't necessary
				var operation = string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);
				var faultType = e.GetType().IsGenericType ? e.GetType().GetGenericArguments()[0] : null;
				Platform.Log(LogLevel.Info, "Fault ({0}): {1} ({2})", faultType != null ? faultType.Name : "unknown", e.Message, operation);

				// rethrow the exception so the fault gets to the client
				throw;
			}
            catch (Exception e)
            {
				Platform.Log(LogLevel.Error, e);

                // rethrow the exception so that the client gets an error
                throw;
            }
        }

        #endregion
    }
}
