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
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// This service provider allows the application server to make use of core services internally
	/// by providing these services in-process.
	/// </summary>
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class InProcessCoreServiceProvider : IServiceProvider
	{
		private readonly IServiceFactory _serviceFactory;

		public InProcessCoreServiceProvider()
		{
			_serviceFactory = new ServiceFactory(new CoreServiceExtensionPoint());

			// exception logging occurs outside of the main persistence context
			// JR: is there any point in logging exceptions from the in-process provider?  Or is this just redundant?
			//_serviceFactory.Interceptors.Add(new ExceptionLoggingAdvice());

			// add outer audit advice outside of main persistence context advice
			_serviceFactory.Interceptors.Add(new AuditAdvice.Outer());

			// add persistence context advice, that controls the persistence context for the main transaction
			_serviceFactory.Interceptors.Add(new PersistenceContextAdvice());

			// add inner audit advice inside of main persistence context advice
			_serviceFactory.Interceptors.Add(new AuditAdvice.Inner());
		}

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (_serviceFactory.HasService(serviceType))
			{
				return _serviceFactory.GetService(serviceType);
			}
			return null;    // as per MSDN
		}

		#endregion
	}
}
