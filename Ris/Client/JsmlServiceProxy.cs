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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// AsyncInvocationCompletedEventArgs event args.
	/// </summary>
	public class AsyncInvocationCompletedEventArgs : EventArgs
	{
		public AsyncInvocationCompletedEventArgs(string invocationId, string response)
		{
			this.InvocationId = invocationId;
			this.Response = response;
		}

		/// <summary>
		/// Gets the string ID assigned to the invocation.
		/// </summary>
		public string InvocationId { get; private set; }

		/// <summary>
		/// Gets the JSML-encoded response.
		/// </summary>
		public string Response { get; private set; }
	}

	/// <summary>
	/// AsyncInvocationCompletedEventArgs event args.
	/// </summary>
	public class AsyncInvocationErrorEventArgs : EventArgs
	{
		public AsyncInvocationErrorEventArgs(string invocationId, Exception e)
		{
			this.InvocationId = invocationId;
			this.Error = e;
		}

		/// <summary>
		/// Gets the string ID assigned to the invocation.
		/// </summary>
		public string InvocationId { get; private set; }

		/// <summary>
		/// Gets the exception object for the error that occured.
		/// </summary>
		public Exception Error { get; private set; }
	}

	/// <summary>
    /// Service proxy for use by javascript code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a COM-visible class that allows javascript code running in a browser to effectively
    /// make use of <see cref="Platform.GetService"/> to obtain an abritrary service and invoke 
    /// operations on it.  For ease of use, a wrapper (outer proxy) may be created in javascript
    /// around this object (innerproxy) that allows service methods to be invoked directly.
    /// An example is shown below.  In this example, the call to window.external.GetServiceProxy
    /// returns an instance of this class.
    /// <code>
    /// getService: function(serviceContractName)
    ///	{
    ///	    var innerProxy = window.external.GetServiceProxy(serviceContractName);
    ///	    var operations = JSML.parse(innerProxy.GetOperationNames());
    ///	    
    ///	    var proxy = { _innerProxy: innerProxy };
    ///	    operations.each(
    ///	        function(operation)
    ///	        {
    ///	            proxy[operation] = 
    ///	                function(request)
    ///	                {
    ///	                    return JSML.parse( this._innerProxy.InvokeOperation(operation, JSML.create(request, "requestData")) );
    ///	                };
    ///	        });
    ///	    return proxy;
    ///	}
    /// </code>
    /// </para>
    /// <para>
    /// The proxy can operate in one of two modes: client-side shim or server-side shim.
    /// With client-side shim, the request/response objects are converted to/from JSML on the client-side,
    /// and with server-side shim, this conversion is performed by the server.  Client-side shim is probably more
    /// efficient when a binary encoding such as net.tcp is being used between client and server, whereas
    /// the server-side shim is probably more efficient when a more verbose XML-based protocol is being used 
    /// between client and server.
    /// </para>
    /// </remarks>
    [ComVisible(true)]
    public class JsmlServiceProxy
    {


		/// <summary>
		/// Dynamic dispatch style shim
		/// </summary>
		interface IShim
		{
			string GetOperationNames(string serviceContractName);
			string InvokeOperation(string serviceContractName, string operationName, string requestJsml);
		}

		/// <summary>
		/// Invokes service operations normally, and performs JSML translation on the client.
		/// </summary>
		class ClientSideShim : IShim
		{
			public string GetOperationNames(string serviceContractName)
			{
				var names = ShimUtil.GetOperationNames(serviceContractName);
				return JsmlSerializer.Serialize(names, "operationNames");
			}

			public string InvokeOperation(string serviceContractName, string operationName, string requestJsml)
			{
				return ShimUtil.InvokeOperation(serviceContractName, operationName, requestJsml);
			}
		}

		/// <summary>
		/// Invokes serivce operations via the JSML shim service, so that JSML translation is performed on server.
		/// </summary>
		class ServerSideShim : IShim
		{
			public string GetOperationNames(string serviceContractName)
			{
				string[] names = null;
				Platform.GetService<IJsmlShimService>(
					service => names = service.GetOperationNames(new GetOperationNamesRequest(serviceContractName)).OperationNames);
				return JsmlSerializer.Serialize(names, "operationNames");
			}

			public string InvokeOperation(string serviceContractName, string operationName, string requestJsml)
			{
				string responseJsml = null;
				Platform.GetService<IJsmlShimService>(
					service =>
					{
						var request = new InvokeOperationRequest(serviceContractName, operationName, new JsmlBlob(requestJsml));
						responseJsml = service.InvokeOperation(request).ResponseJsml.Value;
					});
				return responseJsml;
			}
		}

        private readonly string _serviceContractName;
    	private readonly IShim _shim;
    	private EventHandler<AsyncInvocationCompletedEventArgs> _asyncInvocationCompleted;
		private EventHandler<AsyncInvocationErrorEventArgs> _asyncInvocationError;

        /// <summary>
        /// Constructs a proxy instance.
        /// </summary>
        /// <param name="serviceContractInterfaceName">An assembly-qualified service contract name.</param>
        /// <param name="useServerSideShim">True to use server-side shim, false for client-side.</param>
        public JsmlServiceProxy(string serviceContractInterfaceName, bool useServerSideShim)
        {
            _serviceContractName = serviceContractInterfaceName;
        	_shim = useServerSideShim ? (IShim) new ServerSideShim() : new ClientSideShim();
        }

		/// <summary>
		/// Occurs to notify that an asynchronous invocation, initiated with <see cref="InvokeOperationAsync"/>, has completed.
		/// </summary>
		public event EventHandler<AsyncInvocationCompletedEventArgs> AsyncInvocationCompleted
		{
			add { _asyncInvocationCompleted += value; }
			remove { _asyncInvocationCompleted -= value; }
		}

		/// <summary>
		/// Occurs to notify that an asynchronous invocation, initiated with <see cref="InvokeOperationAsync"/>, has resulted in an error.
		/// </summary>
		public event EventHandler<AsyncInvocationErrorEventArgs> AsyncInvocationError
		{
			add { _asyncInvocationError += value; }
			remove { _asyncInvocationError -= value; }
		}

        /// <summary>
        /// Returns the names of the operations provided by the service.
        /// </summary>
        /// <returns>A JSML-encoded array of operation names.</returns>
        public string GetOperationNames()
        {
        	return _shim.GetOperationNames(_serviceContractName);
        }

        /// <summary>
        /// Invokes the specified operation.
        /// </summary>
        /// <param name="operationName">The name of the operation to invoke.</param>
        /// <param name="requestJsml">The request object, as JSML.</param>
        /// <returns>The response object, as JSML.</returns>
        public string InvokeOperation(string operationName, string requestJsml)
        {
        	return InvokeHelper(operationName, requestJsml);
        }

		/// <summary>
		/// Invokes the specified operation asynchronously.
		/// </summary>
		/// <remarks>
		/// The <cref="AsyncInvocationCompleted"/> event will be fired when the request completes, and the response
		/// will be available from the event args.
		/// </remarks>
		/// <param name="operationName">The name of the operation to invoke.</param>
		/// <param name="requestJsml">The request object, as JSML.</param>
		/// <returns>An invocation ID string.</returns>
		public string InvokeOperationAsync(string operationName, string requestJsml)
		{
			// generate an invocation ID
    		var id = Guid.NewGuid().ToString();

			// invoke operation asynchronously
    		var asyncTask = new AsyncTask();
    		string response = null;
			asyncTask.Run(
				delegate
				{
					response = InvokeHelper(operationName, requestJsml);
				},
				() => OnInvocationCompleted(id, response),
				error => OnInvocationError(id, error));

    		return id;
		}

    	private string InvokeHelper(string operationName, string requestJsml)
		{
			return _shim.InvokeOperation(_serviceContractName, operationName, requestJsml);
		}

		private void OnInvocationCompleted(string id, string responseJsml)
		{
			var args = new AsyncInvocationCompletedEventArgs(id, responseJsml);
			EventsHelper.Fire(_asyncInvocationCompleted, this, args);
		}

		private void OnInvocationError(string id, Exception error)
		{
			var args = new AsyncInvocationErrorEventArgs(id, error);
			EventsHelper.Fire(_asyncInvocationError, this, args);
		}

	}
}
