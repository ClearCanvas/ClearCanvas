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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Applied to a service operation to indicate that the response is cacheable.
	/// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class ResponseCachingAttribute : Attribute
    {
        private readonly string _directiveMethod;

		/// <summary>
		/// Constructor which accepts the name of a method on the service that supplies the <see cref="ResponseCachingDirective"/>
		/// for this operation.
		/// </summary>
		/// <param name="directiveMethod">See <cref="DirectiveMethod"/>.</param>
        public ResponseCachingAttribute(string directiveMethod)
        {
            _directiveMethod = directiveMethod;
        }

		/// <summary>
		/// Gets the name of a method on the service class that returns an instance of <see cref="ResponseCachingDirective"/>.
		/// </summary>
		/// <remarks>
		/// The named method must accept the same parameters as the service operation to which it is applied
		/// (or it may accept more general Types of those parameters e.g. objects), and must return
		/// an instance of <see cref="ResponseCachingDirective"/>.  For example, a typical signature
		/// would be this: 
		/// <code>
		///		ResponseCachingDirective MyMethod(object request)
		/// </code>
		/// When the service operation is invoked, this method will be invoked with the request object that was passed
		/// to the service.  Hence the caching directive may vary as a function of the request.
		/// </remarks>
        public string DirectiveMethod
        {
            get { return _directiveMethod; }
        }
    }
}
