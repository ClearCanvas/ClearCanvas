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
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Advice class responsbile for logging unhandled exceptions thrown from service operation methods.
    /// </summary>
    public class PerformanceLoggingAdvice : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            DateTime startTime = DateTime.Now;
            invocation.Proceed();
            DateTime endTime = DateTime.Now;


            string operationName = string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);

            Platform.Log(LogLevel.Info, "{0} {1} - {2} (Duration = {3}",
                operationName,
                startTime.ToString("yyyy-MM-dd hh:mm:ss.fff"),
                endTime.ToString("yyyy-MM-dd hh:mm:ss.fff"),
                (endTime - startTime).ToString());
        }

        #endregion
    }
}
