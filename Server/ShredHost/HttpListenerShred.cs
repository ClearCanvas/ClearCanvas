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
    /// <summary>
    /// Represents a shred that listens to and handles http requests.
    /// </summary>
    public abstract class HttpListenerShred : Shred
    {
        #region Private Members
        private HttpListener _listener;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="HttpListenerShred"/> to listern at the specified address.
        /// </summary>
        protected HttpListenerShred(int port, string uriSubPath)
        {
            Port = port;
            UriSubPath = uriSubPath;
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


        #endregion

        #region Protected Methods

        protected void StartListening(Action<HttpListenerContext> callback)
        {
            Platform.Log(LogLevel.Info, "Started listening at {0}", BaseUri);

            _listener = new HttpListener();
            _listener.Prefixes.Add(BaseUri);
            _listener.Start();

			OnStarted();

			while(_listener.IsListening)
			{
				try
				{
					var context = _listener.GetContext();
					ThreadPool.QueueUserWorkItem(n => callback(context));
				}
				catch (Exception e)
				{
					if (_listener.IsListening)
						Platform.Log(LogLevel.Warn, e, "Unexpected error in HttpListenerShred.");
				}
			}
        }

    	#endregion

        #region Overridden Public Methods

		protected virtual void OnStarted()
		{
		}

    	public override void Stop()
        {
            _listener.Stop();
        }

        #endregion

    }
}
