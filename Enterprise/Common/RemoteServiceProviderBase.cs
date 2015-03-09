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
using System.ServiceModel.Security;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Security.Cryptography.X509Certificates;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common.Configuration;

namespace ClearCanvas.Enterprise.Common
{
	#region RemoteServiceProviderArgs class

	/// <summary>
	/// Encapsulates options that configure a <see cref="RemoteServiceProviderBase{T}"/>.
	/// </summary>
	public class RemoteServiceProviderArgs
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="configurationClassName"></param>
		/// <param name="maxReceivedMessageSize"></param>
		/// <param name="certificateValidationMode"></param>
		/// <param name="revocationMode"></param>
		[Obsolete("Use another constructor overload")]
		public RemoteServiceProviderArgs(
			string baseUrl,
			string configurationClassName,
			int maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode,
			X509RevocationMode revocationMode)
			: this(baseUrl, null, configurationClassName, maxReceivedMessageSize, certificateValidationMode,
				revocationMode, null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="failoverBaseUrl"></param>
		/// <param name="configurationClassName"></param>
		/// <param name="maxReceivedMessageSize"></param>
		/// <param name="certificateValidationMode"></param>
		/// <param name="revocationMode"></param>
		public RemoteServiceProviderArgs(
			string baseUrl,
			string failoverBaseUrl,
			string configurationClassName,
			int maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode,
			X509RevocationMode revocationMode)
			: this(baseUrl, failoverBaseUrl, configurationClassName, maxReceivedMessageSize, certificateValidationMode,
				revocationMode, null)
		{
		}



		/// <summary>
		/// Constructor
		/// </summary>
		public RemoteServiceProviderArgs(
			string baseUrl,
			string failoverBaseUrl,
			string configurationClassName,
			long maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode,
			X509RevocationMode revocationMode,
			string credentialsProviderClassName)
		{
			BaseUrl = baseUrl;
			FailoverBaseUrl = failoverBaseUrl;
			Configuration = InstantiateClass<IServiceChannelConfiguration>(configurationClassName);
			MaxReceivedMessageSize = maxReceivedMessageSize;
			CertificateValidationMode = certificateValidationMode;
			RevocationMode = revocationMode;
			UserCredentialsProvider = string.IsNullOrEmpty(credentialsProviderClassName) ?
				new DefaultUserCredentialsProvider() : 
				InstantiateClass<IUserCredentialsProvider>(credentialsProviderClassName);
		}

		/// <summary>
		/// Base URL shared by all services in the service layer.
		/// </summary>
		public string BaseUrl { get; set; }

		/// <summary>
		/// Failover base URL shared by all services in the service layer.
		/// </summary>
		public string FailoverBaseUrl { get; set; }

		/// <summary>
		/// Minimum time that must elapse before attempting to contact a previously unreachable endpoint.
		/// </summary>
		public TimeSpan FailedEndpointBlackoutTime { get; set; }

		/// <summary>
		/// Configuration that is responsible for configuring the service binding/endpoint.
		/// </summary>
		public IServiceChannelConfiguration Configuration { get; set; }

		/// <summary>
		/// Maximum size in bytes of message received by the service client.
		/// </summary>
		public long MaxReceivedMessageSize { get; set; }

		/// <summary>
		/// The time, in seconds, in which a send operation must complete.
		/// </summary>
		/// <remarks>Value less than or equal to zero should be ignored.</remarks>
		public int SendTimeoutSeconds { get; set; }

		/// <summary>
		/// Certificate validation mode.
		/// </summary>
		public X509CertificateValidationMode CertificateValidationMode { get; set; }

		/// <summary>
		/// Certificate revocation mode.
		/// </summary>
		public X509RevocationMode RevocationMode { get; set; }

		/// <summary>
		/// Gets or sets an <see cref="IUserCredentialsProvider"/>.
		/// </summary>
		public IUserCredentialsProvider UserCredentialsProvider { get; set; }

		private static T InstantiateClass<T>(string className)
		{
			try
			{
				var type = Type.GetType(className, true);
				return (T)Activator.CreateInstance(type);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Cannot instantiate class {0}", className);
				throw;
			}
		}
	}

	#endregion


	/// <summary>
	/// Abstract base class for remote service provider extensions.
	/// </summary>
	public abstract class RemoteServiceProviderBase : IServiceProvider
	{
		#region DisposableInterceptor

		/// <summary>
		/// Interceptor that ensure <see cref="IDisposable"/> is honoured.
		/// </summary>
		internal class DisposableInterceptor : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				// if not invoking IDisposable.Dispose(), we can just Proceed and return
				if (!InvocationMethodIsDispose(invocation))
				{
					// proceed normally
					invocation.Proceed();
					return;
				}

				var channel = GetInvocationTarget(invocation) as IClientChannel;
				try
				{
					// to propertly clean up the channel, we should call Close() first,
					// and then Dispose(), which may be redundant, but might as well be strict
					// do not proceed along the interceptor chain
					if(channel != null)
					{
						switch (channel.State)
						{
							case CommunicationState.Opened:
								channel.Close();
								break;
							case CommunicationState.Faulted:
								channel.Abort();
								break;
						}
						channel.Dispose();
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e,
						"Exception generated when attempting to close channel with URI {0}",
						channel.RemoteAddress.Uri);

					channel.Abort();
				}
			}

			private static bool InvocationMethodIsDispose(IInvocation invocation)
			{
				return invocation.Method.DeclaringType == typeof(IDisposable)
					&& invocation.Method.Name == "Dispose";
			}

			private static object GetInvocationTarget(IInvocation invocation)
			{
				var target = invocation.InvocationTarget;

				// this is odd - it is not clear whether InvocationTarget is the proxy or the target
				// DP seems to do different things under different circumstances, probably due to bugs
				// we do the safe thing and check if we need to access the inner object
				if ((target is IProxyTargetAccessor))
				{
					return ((IProxyTargetAccessor)target).DynProxyGetTarget();
				}

				// just return the target
				return target;
			}
		}

		#endregion

		private readonly ProxyGenerator _proxyGenerator;
		private readonly IChannelProvider _channelProvider;
		private readonly IUserCredentialsProvider _userCredentialsProvider;

		private readonly ResponseCachingClientSideAdvice _responseCachingAdvice;
		private readonly FailoverClientAdvice _failoverAdvice;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		protected RemoteServiceProviderBase(RemoteServiceProviderArgs args)
			: this(new StaticChannelProvider(args), args.UserCredentialsProvider, !string.IsNullOrEmpty(args.FailoverBaseUrl))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected RemoteServiceProviderBase(IChannelProvider channelProvider, IUserCredentialsProvider userCredentialsProvider, bool supportFailover)
		{
			_channelProvider = channelProvider;
			_userCredentialsProvider = userCredentialsProvider;
			_proxyGenerator = new ProxyGenerator();

			_responseCachingAdvice = new ResponseCachingClientSideAdvice();
			_failoverAdvice = supportFailover ? new FailoverClientAdvice(this) : null;
		}

		#region IServiceProvider

		public object GetService(Type serviceContract)
		{
			// check if the service is provided by this provider
			if (CanProvideService(serviceContract))
				return null;

			// create the channel
			var authenticationRequired = AuthenticationAttribute.IsAuthenticationRequired(serviceContract);
			var credentials = authenticationRequired
			                  	? new ChannelCredentials {UserName = this.UserName, Password = this.Password}
			                  	: null;
			var channel = _channelProvider.GetPrimary(serviceContract, credentials);

			// create an AOP proxy around the channel, and return that
			return CreateChannelProxy(serviceContract, channel);
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Gets a value indicating whether this service provider provides the specified service.
		/// </summary>
		protected abstract bool CanProvideService(Type serviceType);

		/// <summary>
		/// Applies AOP interceptors to the proxy.
		/// </summary>
		/// <remarks>
		/// Override this method to customize which interceptors are applied to the
		/// proxy by adding/removing or inserting into the specified list.
		/// The order of interceptors is significant.  The first entry
		/// in the list is the outermost, and the last entry in the list is the 
		/// innermost.
		/// </remarks>
		/// <param name="serviceType"></param>
		/// <param name="interceptors"></param>
		protected virtual void ApplyInterceptors(Type serviceType, IList<IInterceptor> interceptors)
		{
			// this must be added as the outer-most interceptor
			// it is basically a hack to prevent the interception chain from acting on a call to Dispose(),
			// because Dispose() is not a service operation
			interceptors.Add(new DisposableInterceptor());

			// additional interceptors are added outside of all others
			foreach (var interceptor in AdditionalServiceInterceptorProvider.GetInterceptors(ServiceInterceptSite.Client))
			{
				interceptors.Add(interceptor);
			}

			if (ClearCanvas.Common.Caching.Cache.IsSupported())
			{
				// add response-caching client-side advice
				interceptors.Add(_responseCachingAdvice);
			}

			interceptors.Add(new CultureClientSideAdvice());

			// if failover was defined, add fail-over advice at the end of the list, closest the target call
			if(_failoverAdvice != null)
			{
				interceptors.Add(_failoverAdvice);
			}
		}

		/// <summary>
		/// Gets the user name to pass as a credential to the service.
		/// </summary>
		protected virtual string UserName
		{
			get { return _userCredentialsProvider == null ? "" : _userCredentialsProvider.UserName; }
		}

		/// <summary>
		/// Gets the password to pass as a credential to the service.
		/// </summary>
		protected virtual string Password
		{
			get { return _userCredentialsProvider == null ? "" : _userCredentialsProvider.SessionTokenId; }
		}

		/// <summary>
		/// Attempts to get a failover channel for the specified failed channel.
		/// Note that a raw channel is returned, not a decorated proxy.
		/// </summary>
		/// <param name="failedChannel"></param>
		/// <returns></returns>
		protected internal IClientChannel GetFailoverChannel(IClientChannel failedChannel)
		{
			return _channelProvider.GetFailover(failedChannel);
		}

		#endregion

		#region Helpers

		private object CreateChannelProxy(Type serviceContract, IClientChannel channel)
		{
			// ensure we only access the proxy generator in a thread-safe manner
			lock(_proxyGenerator)
			{
				//These interceptors used to be initialized once and shared amongst all
				//the clients.  #7179 changed that, so if we find another solution for it
				//later, we could make them shared again.
				var interceptors = new List<IInterceptor>();
				ApplyInterceptors(serviceContract, interceptors);

				var options = new ProxyGenerationOptions();

				// create and return proxy
				// note: _proxyGenerator does internal caching based on service contract
				// so subsequent calls based on the same contract will be fast
				// note: important to proxy IDisposable too, otherwise channels can't get disposed!!!
				var aopChain = new AopInterceptorChain(interceptors);
				return _proxyGenerator.CreateInterfaceProxyWithTarget(
					serviceContract,
					new[] { serviceContract, typeof(IDisposable) },
					channel,
					options,
					aopChain);
			}
		}

		#endregion
	}

	/// <summary>
	/// Abstract base class for remote service provider extensions.
	/// </summary>
	/// <typeparam name="TServiceLayerAttribute">Attribute that identifiers the service layer to which a service belongs.</typeparam>
	public abstract class RemoteServiceProviderBase<TServiceLayerAttribute> : RemoteServiceProviderBase
		where TServiceLayerAttribute : Attribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		protected RemoteServiceProviderBase(RemoteServiceProviderArgs args)
			: base(args)
		{
		}

		#region Protected API

		/// <summary>
		/// Gets a value indicating whether this service provider provides the specified service.
		/// </summary>
		/// <remarks>
		/// The default implementation is based on the service contract being marked with the <see cref="TServiceLayerAttribute"/>
		/// attribute.  Override this method to customize which services are provided by this provider.
		/// </remarks>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		protected override bool CanProvideService(Type serviceType)
		{
			return !AttributeUtils.HasAttribute<TServiceLayerAttribute>(serviceType);
		}

		#endregion
	}
}
