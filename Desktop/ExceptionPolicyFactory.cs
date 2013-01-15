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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    ///<summary>
    /// Provides <see cref="IExceptionPolicy"/> objects via static <see cref="GetPolicy"/> method.
    ///</summary>
    internal class ExceptionPolicyFactory
    {
        private sealed class DefaultExceptionPolicy : IExceptionPolicy
        {
            public void Handle(Exception e, IExceptionHandlingContext context)
            {
                context.Log(LogLevel.Error, e);
                context.ShowMessageBox(e.Message, true);
            }
        }

    	private static readonly IDictionary<Type, IExceptionPolicy> _policies = CreatePolicies();
    	private static readonly IExceptionPolicy _defaultPolicy = new DefaultExceptionPolicy();

		private static IDictionary<Type, IExceptionPolicy> CreatePolicies()
        {
            var policies = new Dictionary<Type, IExceptionPolicy>();

        	try
        	{
				foreach (IExceptionPolicy policy in new ExceptionPolicyExtensionPoint().CreateExtensions())
        		{
        			foreach (ExceptionPolicyForAttribute attr in policy.GetType().GetCustomAttributes(typeof (ExceptionPolicyForAttribute), true))
        			{
        				if (!policies.ContainsKey(attr.ExceptionType))
        					policies[attr.ExceptionType] = policy;
        			}
        		}
        	}
			catch (NotSupportedException)
			{}
        	catch (Exception e)
        	{
        		Platform.Log(LogLevel.Debug, e);
        	}

			return policies;
        }

        ///<summary>
        /// Returns an <see cref="IExceptionPolicy"/> for a requested <see cref="Exception"/> type.
        ///</summary>
        ///<param name="exceptionType">An <see cref="Exception"/> derived type.</param>
        ///<returns>An <see cref="IExceptionPolicy"/> for the requested type if found or a <see cref="DefaultExceptionPolicy"/>.</returns>
        public static IExceptionPolicy GetPolicy(Type exceptionType)
        {
            IExceptionPolicy policy;
            if(!_policies.TryGetValue(exceptionType, out policy))
            	policy = _defaultPolicy;

            return policy;
        }
    }
}
