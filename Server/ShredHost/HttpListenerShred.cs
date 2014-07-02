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
using System.Net;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Server.ShredHost
{
    //TODO (CR Orion): this should not need to be abstract - it's usable as-is without the need to inherit.

    /// <summary>
    /// Represents a shred that listens to and handles http requests.
    /// </summary>
    public abstract class HttpListenerShred : Shred
    {
		/// <summary>
		/// Lists of Windows error codes
		/// (See http://msdn.microsoft.com/en-us/library/ms681382(v=vs.85).aspx)
		/// </summary>
		class WindowsErrorCodes
		{
			public static int ERROR_SHARING_VIOLATION = 32;
		}

        #region Private Members
        private HttpListener _listener;
    	private Thread _backgroundThread;

        private readonly object _completionPortLock = new object();
        private int _completionPortCount;

    	#endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="HttpListenerShred"/> to listern at the specified address.
        /// </summary>
        protected HttpListenerShred(int port, string uriSubPath)
        {
            Port = port;
            UriSubPath = uriSubPath;
            UseCompletionPorts = false;
            MaxCompletionPortCount = 10;
        }
        
        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the URI where the shred is listening at for incoming http requests.
        /// </summary>
        protected String BaseUri
        {
            get { return String.Format("{0}://+:{1}{2}", 
                    Uri.UriSchemeHttp, Port, UriSubPath); }
        }

		/// <summary>
		/// Gets the server Uri
		/// </summary>
		protected String ServerEndPointUri
		{
			get
			{
				
				var host = Dns.GetHostName(); 
				return String.Format("{0}://{1}:{2}{3}", Uri.UriSchemeHttp, host, Port, UriSubPath);
			}
		}

        /// <summary>
        /// Gets or sets the name of the shred.
        /// </summary>
        protected string Name { get; set; }

        protected int Port { get; set; }

        protected string UriSubPath { get; set; }

        protected bool UseCompletionPorts { get; set; }

        protected int MaxCompletionPortCount { get; set; }

        #endregion

        #region Protected Methods

		protected void StartListeningAsync(Action<HttpListenerContext> callback)
		{
			Platform.Log(LogLevel.Info, "Started listening at {0}", BaseUri);
			
			// start the listener on a separate thread
			_backgroundThread = new Thread(StartListening) {Name = GetDisplayName()};
		    _backgroundThread.Start(callback);
		}

        protected virtual void OnStartError(string errorMessage) { }

    	#endregion

        #region Public Methods

		protected virtual void OnStarted()
		{
		}

    	public override void Stop()
    	{
    		StopListener();

			// wait for the background thread to complete. Ideally it should have been terminated by now
			if (_backgroundThread != null && _backgroundThread.IsAlive)
			{
				if (_backgroundThread.Join(3000))
				{
					Platform.Log(LogLevel.Info, "{0} has stopped gracefully", GetDisplayName());
				}
				else
				{
					Platform.Log(LogLevel.Warn, "{0} failed to stop gracefully", GetDisplayName());
				}
			}
			else
			{
				Platform.Log(LogLevel.Info, "{0} has stopped gracefully", GetDisplayName());
			}
		}

    	
    	#endregion

		#region Private Methods

        private void StartListening(object callback)
        {
            try
            {
                StartListener((Action<HttpListenerContext>)callback);
            }
            catch (HttpListenerException e)
            {
                // When the port is tied up by another process, the system throws HttpListenerException with error code = 32 
                // and the message "The process cannot access the file because it is being used by another process". 
                // For clarity, we make the error message more informative in this case
                if (e.ErrorCode == WindowsErrorCodes.ERROR_SHARING_VIOLATION)
                {
                    string errorMessage = string.Format("Unable to start {0} on port {1}. The port is being used by another process", GetDisplayName(), Port);
                    Platform.Log(LogLevel.Fatal, errorMessage);
                    OnStartError(errorMessage);
                }
                else
                {
                    string errorMessage = string.Format("Unable to start {0}. System Error Code={1}", GetDisplayName(), e.ErrorCode);
                    Platform.Log(LogLevel.Fatal, e, errorMessage);
                    OnStartError(errorMessage);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Fatal, e, "Unable to start {0}", GetDisplayName());
                OnStartError(e.Message);
            }
        }

        private void StartListener(Action<HttpListenerContext> callback)
        {
			_listener = new HttpListener();
			_listener.Prefixes.Add(BaseUri);
			_listener.Start();

			OnStarted();

            if (!UseCompletionPorts)
                ListenSync(callback);
            else
                ListenAsync(callback);
        }

        private void ListenSync(Action<HttpListenerContext> callback)
        {
            Platform.Log(LogLevel.Info, "Starting HttpListener in Synchronous mode.");

            while (_listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(o => ProcessRequest(context, callback));
                }
                catch (Exception e)
                {
                    if (_listener.IsListening)
                        Platform.Log(LogLevel.Warn, e, "Unexpected error in HttpListenerShred.");
                }
            }
        }

        private void ListenAsync(Action<HttpListenerContext> callback)
        {
            Platform.Log(LogLevel.Info, "Starting HttpListener in asynchronous mode.");

            while (_listener.IsListening)
            {
                lock (_completionPortLock)
                {
                    if (_completionPortCount >= MaxCompletionPortCount)
                        Monitor.Wait(_completionPortLock);

                    //Signals that we should quit.
                    if (_completionPortCount < 0) break;

                    ++_completionPortCount;
                }

                try
                {
                    _listener.BeginGetContext(asyncResult =>
                    {
                        try
                        {
                            var context = _listener.EndGetContext(asyncResult);
                            ProcessRequest(context, callback);
                        }
                        catch (Exception e)
                        {
                            if (_listener.IsListening)
                                Platform.Log(LogLevel.Warn, e, "Unexpected error in HttpListenerShred.");
                        }

                        lock (_completionPortLock)
                        {
                            --_completionPortCount;
                            Monitor.Pulse(_completionPortLock);
                        }

                    }, null);
                }
                catch (Exception e)
                {
                    if (_listener.IsListening)
                        Platform.Log(LogLevel.Warn, e, "Unexpected error in HttpListenerShred.");
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context, Action<HttpListenerContext> callback)
        {
            try
            {
                callback(context);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }
        }

        private void StopListener()
    	{
            lock (_completionPortLock)
            {
                //Signals we should quit.
                _completionPortCount = -1;
                Monitor.Pulse(_completionPortLock);    
            }

            if (_listener != null)
                _listener.Stop();
        }

		#endregion
	}
}
