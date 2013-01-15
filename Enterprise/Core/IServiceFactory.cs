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

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface to a service factory, which instantiates a service based on a specified
    /// contract.
    /// </summary>
    public interface IServiceFactory
    {
		/// <summary>
		/// Gets the list of interceptors that will be applied to service instances created by this factory.
		/// </summary>
		/// <remarks>
		/// Interceptors must be thread-safe, since the same interceptor instance is applied to every service object
		/// that is created.
		/// </remarks>
		IList<IInterceptor> Interceptors { get; }

        /// <summary>
        /// Obtains an instance of the service that implements the specified contract.
        /// </summary>
        /// <typeparam name="TServiceContract"></typeparam>
        /// <returns></returns>
        TServiceContract GetService<TServiceContract>();

        /// <summary>
        /// Obtains an instance of the service that implements the specified contract.
        /// </summary>
        /// <returns></returns>
        object GetService(Type serviceContract);

        /// <summary>
        /// Lists the service contracts supported by this factory.
        /// </summary>
        /// <returns></returns>
        ICollection<Type> ListServiceContracts();

        /// <summary>
        /// Lists the service classes that provide implementations of the contracts supported by this factory.
        /// </summary>
        /// <returns></returns>
        ICollection<Type> ListServiceClasses();

        /// <summary>
        /// Tests if this factory supports a service with the specified contract.
        /// </summary>
        /// <param name="serviceContract"></param>
        /// <returns></returns>
        bool HasService(Type serviceContract);
    }
}
