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
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using System.IdentityModel.Selectors;
using Castle.Core.Interceptor;
using ClearCanvas.Enterprise.Common.ServiceConfiguration.Server;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
	/// <summary>
	/// Mounts a set of related services on a common base URI using a common binding configuration.
	/// </summary>
	/// <remarks>
	/// Simplifies the process of hosting a number of related WCF services by limiting the amount
	/// of configuration required and applying the configuration across the entire set of services.
	/// </remarks>
	public class ServiceMount
	{
		private Uri _baseAddress;
		private IServiceHostConfiguration _configuration;
		private UserNamePasswordValidator _userValidator = new DefaultUserValidator();
		private ServiceCredentials _serviceCredentials = new DefaultServiceCredentials();
		private bool _sendExceptionDetailToClient;
		private bool _enablePerformanceLogging;
		private long _maxReceivedMessageSize = 1000000;
		private InstanceContextMode _instanceMode = InstanceContextMode.PerCall;
		private CertificateSearchDirective _certificateSearchDirective;



		private readonly List<ServiceHost> _serviceHosts = new List<ServiceHost>();

		/// <summary>
		/// Constructs a service mount that hosts services on the specified base URI
		/// using the specified service host configuration.
		/// </summary>
		/// <param name="baseAddress"></param>
		/// <param name="configuration"></param>
		public ServiceMount(Uri baseAddress, IServiceHostConfiguration configuration)
		{
			_baseAddress = baseAddress;
			_configuration = configuration;

			// establish default certificate search parameters consistent with behaviour prior to #8219
			_certificateSearchDirective = CertificateSearchDirective.CreateBasic(_baseAddress);
		}

		/// <summary>
		/// Constructs a service mount that hosts services on the specified base URI
		/// using the specified service host configuration.
		/// </summary>
		/// <param name="baseAddress"></param>
		/// <param name="serviceHostConfigurationClass"></param>
		public ServiceMount(Uri baseAddress, string serviceHostConfigurationClass)
			: this(baseAddress, (IServiceHostConfiguration)InstantiateClass(serviceHostConfigurationClass))
		{
		}

		#region Public API

		/// <summary>
		/// Gets or sets the base address on which services will be hosted.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public Uri BaseAddress
		{
			get { return _baseAddress; }
			set { _baseAddress = value; }
		}

		/// <summary>
		/// Gets or sets the configuration that will be applied to hosted services.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public IServiceHostConfiguration ServiceHostConfiguration
		{
			get { return _configuration; }
			set { _configuration = value; }

		}

		/// <summary>
		/// Gets or sets the parameters used to find the certificate to host the service
		/// </summary>
		public CertificateSearchDirective CertificateSearchDirective
		{
			get { return _certificateSearchDirective; }
			set { _certificateSearchDirective = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether exception detail should be returned to the client.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public bool SendExceptionDetailToClient
		{
			get { return _sendExceptionDetailToClient; }
			set { _sendExceptionDetailToClient = value; }
		}

		/// <summary>
		/// Gets or set the maximum size of received messages in bytes.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public long MaxReceivedMessageSize
		{
			get { return _maxReceivedMessageSize; }
			set { _maxReceivedMessageSize = value; }
		}

		/// <summary>
		/// The time, in seconds, in which a send operation must complete.
		/// </summary>
		/// <remarks>Value less than or equal to zero should be ignored.</remarks>
		public int SendTimeoutSeconds { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether performance logging is enabled.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public bool EnablePerformanceLogging
		{
			get { return _enablePerformanceLogging; }
			set { _enablePerformanceLogging = value; }
		}

		/// <summary>
		/// Gets or sets the service instance mode.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public InstanceContextMode InstanceMode
		{
			get { return _instanceMode; }
			set { _instanceMode = value; }
		}

		/// <summary>
		/// Gets or sets the credentials object that is used to establish authorization policies for authenticated services.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public ServiceCredentials Credentials
		{
			get { return _serviceCredentials; }
			set { _serviceCredentials = value; }
		}

		/// <summary>
		/// Gets or sets the user validator that is used to authenticate services that require authentication.
		/// </summary>
		/// <remarks>
		/// Must be set prior to calling <see cref="AddServices"/>.
		/// </remarks>
		public UserNamePasswordValidator UserValidator
		{
			get { return _userValidator; }
			set { _userValidator = value; }
		}

		/// <summary>
		/// Adds all services defined by the specified service layer extension point.
		/// </summary>
		/// <remarks>
		/// This method internally calls the <see cref="ApplyInterceptors"/> method to decorate
		/// the services with a set of AOP interceptors.
		/// </remarks>
		/// <param name="serviceLayer"></param>
		public void AddServices(IExtensionPoint serviceLayer)
		{
			IServiceFactory serviceFactory = new ServiceFactory(serviceLayer);
			ApplyInterceptors(serviceFactory.Interceptors);
			AddServices(serviceFactory);
		}

		/// <summary>
		/// Gets the list of <see cref="ServiceHost"/> objects created by calls to <see cref="AddServices"/>.
		/// </summary>
		public IList<ServiceHost> ServiceHosts
		{
			get { return _serviceHosts.AsReadOnly(); }
		}

		/// <summary>
		/// Open all mounted services.
		/// </summary>
		public void OpenServices()
		{
			foreach (var host in _serviceHosts)
			{
                try
                {
                    host.Open();                    
                }
                catch (Exception)
                {
                    Platform.Log(LogLevel.Error, "Error mounting service: {0}", host.Description.Endpoints[0].Address);
                    throw;
                }
			}
		}

		/// <summary>
		/// Close all mounted services.
		/// </summary>
		public void CloseServices()
		{
			foreach (var host in _serviceHosts)
			{
				host.Close();
			}
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Applies interceptors to the mounted services.
		/// </summary>
		/// <remarks>
		/// The default implementation of this method applies a default set of interceptors
		/// that are suitable for many common scenarios.  Override this method to customize the 
		/// interceptors by adding or removing items from the specified list.
		/// The order in which the interceptors are added is significant
		/// the first interceptor in the list will be the outermost, and
		/// the last interceptor in the list will be the innermost.
		/// </remarks>
		/// <param name="interceptors"></param>
		protected virtual void ApplyInterceptors(IList<IInterceptor> interceptors)
		{
			// exception logging occurs outside of the main persistence context
			interceptors.Add(new ExceptionLoggingAdvice());

			// add exception promotion advice at the beginning of the interception chain (outside of the service transaction)
			interceptors.Add(new ExceptionPromotionAdvice());

			// use client culture
			interceptors.Add(new CultureServerSideAdvice());

			// additional interceptors
			foreach (var interceptor in AdditionalServiceInterceptorProvider.GetInterceptors(ServiceInterceptSite.Server))
			{
				interceptors.Add(interceptor);
			}

			// add performance logging advice conditionally
			if (_enablePerformanceLogging)
			{
				interceptors.Add(new PerformanceLoggingAdvice());
			}

			// deadlock recovery occurs outside of persistence context,
			// since each retry should be done in a new persistence context
			interceptors.Add(new DeadlockRetryAdvice());

			// add outer audit advice outside of main persistence context advice
			interceptors.Add(new AuditAdvice.Outer());

			// add persistence context advice, that controls the persistence context for the main transaction
			interceptors.Add(new PersistenceContextAdvice());

			// add inner audit advice inside of main persistence context advice
			interceptors.Add(new AuditAdvice.Inner());

			// add response caching advice inside of persistence context, because
			// the context may be used when determining the cache directive, etc.
			interceptors.Add(new ResponseCachingServerSideAdvice());
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Adds all services provided by the specified service factory.
		/// </summary>
		/// <param name="serviceFactory"></param>
		private void AddServices(IServiceFactory serviceFactory)
		{
			var serviceClasses = serviceFactory.ListServiceClasses();
			foreach (var serviceClass in serviceClasses)
			{
				AddService(serviceClass, serviceFactory);
			}
		}

		private void AddService(Type serviceClass, IServiceFactory serviceFactory)
		{
			var contractAttribute = AttributeUtils.GetAttribute<ServiceImplementsContractAttribute>(serviceClass, false);
			if (contractAttribute == null)
				throw new ServiceMountException(string.Format("Unknown contract for service {0}", serviceClass.Name));


			// determine if service requires authentication
			var authenticationAttribute = AttributeUtils.GetAttribute<AuthenticationAttribute>(contractAttribute.ServiceContract);
			var authenticated = authenticationAttribute == null || authenticationAttribute.AuthenticationRequired;

			TransferMode mode = ServiceTransferModeAttribute.GetTransferMode(contractAttribute.ServiceContract);

			// create service URI
			var uri = new Uri(_baseAddress, contractAttribute.ServiceContract.FullName);

			Platform.Log(LogLevel.Info, "Mounting {0} on URI {1}",
				contractAttribute.ServiceContract.Name, uri);

			// create service host
			var host = new ServiceHost(serviceClass, uri);

			// if authenticated
			if (authenticated)
			{
				// replace the built-in ServiceCredentials object with our own (must be done prior to configuration)
				// this will install custom authorization policy
				host.Description.Behaviors.Remove<ServiceCredentials>();
				host.Description.Behaviors.Add(_serviceCredentials);

				// set authentication model to custom, and supply user validator
				host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
				host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = _userValidator;

				// set authorization mode to custom
				host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			}

			// build service according to binding
			_configuration.ConfigureServiceHost(host,
				new ServiceHostConfigurationArgs(
					contractAttribute.ServiceContract,
					uri, authenticated,
					_maxReceivedMessageSize,
					_certificateSearchDirective){SendTimeoutSeconds = SendTimeoutSeconds, TransferMode = mode});

			// add behaviour to inject AOP proxy service factory
			host.Description.Behaviors.Add(new ServiceFactoryInjectionServiceBehavior(contractAttribute.ServiceContract, serviceFactory));

			// adjust some service behaviours
			var serviceBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
			if (serviceBehavior != null)
			{
				// set instance mode to "per call"
				serviceBehavior.InstanceContextMode = _instanceMode;

				// determine whether to send exception detail back to clients
				serviceBehavior.IncludeExceptionDetailInFaults = _sendExceptionDetailToClient;
			}

			_serviceHosts.Add(host);
		}

		private static object InstantiateClass(string className)
		{
			var type = Type.GetType(className, true);
			return Activator.CreateInstance(type);
		}

		#endregion
	}
}
