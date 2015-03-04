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
using System.Configuration;
using System.Diagnostics;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.ImageServer.Common.ServiceModel;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// A collection of useful ImageServer utility functions.
	/// </summary>
    static public class ServerPlatform
    {
        #region Private Fields
        private static string _version;
        private static readonly object _syncLock = new object();
        private static readonly DicomAuditSource  _auditSource = new DicomAuditSource(ProductInformation.Component);
        private static string _hostId;
    	private static string _serverInstanceId;
    	private static string _processorId;
	    #endregion

        /// <summary>
        /// Generates an alert message with an expiration time.
        /// </summary>
        /// <param name="category">An alert category</param>
        /// <param name="level">Alert level</param>
        /// <param name="source">Name of the source where the alert is raised</param>
        /// <param name="alertCode">Alert type code</param>
        /// <param name="contextData">The user-defined application context data</param>
        /// <param name="expirationTime">Expiration time for the alert</param>
        /// <param name="message">The alert message or formatted message.</param>
        /// <param name="args">Paramaters used in the alert message, when specified.</param>
        public static void Alert(AlertCategory category, AlertLevel level, String source, 
                                    int alertCode, object contextData, TimeSpan expirationTime, 
                                    String message, params object[] args)
        {
            Platform.CheckForNullReference(source, "source");
            Platform.CheckForNullReference(message, "message");
            IAlertService service = Platform.GetService<IAlertService>();
            if (service != null)
            {
                AlertSource src = new AlertSource(source, ServerInstanceId) { Host = ServerInstanceId };
            	Alert alert = new Alert
                              	{
                              		Category = category,
                              		Level = level,
                              		Code = alertCode,
                              		ExpirationTime = Platform.Time.Add(expirationTime),
                              		Source = src,
                              		Message = String.Format(message, args),
                              		ContextData = contextData
                              	};

            	service.GenerateAlert(alert);
            }
        }

        /// <summary>
        /// A well known AuditSource for ImageServer audit logging that is based on the component name.
        /// </summary>
        public static DicomAuditSource AuditSource
        {
            get
            {
				return _auditSource;
            }
        }

		/// <summary>
		/// Returns the duration of the user session based on the application settings
		/// </summary>	
	    public static TimeSpan WebSessionTimeout
	    {
            get
            {
                int timeout;
                if (Int32.TryParse(ConfigurationManager.AppSettings["SessionTimeout"], out timeout))
                {
                    return TimeSpan.FromMinutes(timeout);
                }
                else
                {
                    return TimeSpan.FromMinutes(60);
                }
            }
	    }

		/// <summary>
		/// The Streaming folder.    DO NOT CHANGE!
		/// </summary>
		public const string StreamingStorageFolder = "Streaming";

		/// <summary>
		/// The Reconcile folder.    DO NOT CHANGE!
		/// </summary>
		public const string ReconcileStorageFolder = "Reconcile";

		/// <summary>
		/// The default DICOM file extension.  DO NOT CHANGE!
		/// </summary>
		public const string DicomFileExtension = ".dcm";

		/// <summary>
		/// The default Duplicate DICOM file extension.  
		/// </summary>
		/// <remarks>
		/// Note, due to historical reasons, this value does not have a "." in the
		/// duplicate value.  The extensions is input in the WorkQueueUid table without the period.
		/// </remarks>
		public const string DuplicateFileExtension = "dup";


        /// <summary>
        /// Returns the version number (including the suffix and the release type)
        /// </summary>
        public static String VersionString
        {
            get
            {
                lock (_syncLock)
                {
                    if (String.IsNullOrEmpty(_version))
                    {
                        try
                        {
                            _version = StringUtilities.Combine<string>(new string[]
                                                        {
                                                            ProductInformation.Version.ToString(),
                                                            ProductInformation.VersionSuffix,
                                                            ProductInformation.Release
                                                        }, " ", true);
                        }
                        catch (Exception ex)
                        {
                            Platform.Log(LogLevel.Error, ex);
                        }
                    }
                }

                return _version;
            }
        }

		/// <summary>
		/// Flag telling if instance level logging is enabled.
		/// </summary>
    	public static LogLevel InstanceLogLevel
    	{
			get { return Settings.Default.InstanceLogging ? LogLevel.Info : LogLevel.Debug; }
    	}

    	/// <summary>
    	/// Returns a string that can be used to identify the host machine where the server is running
    	/// </summary>
    	public static string HostId
    	{
    		get
    		{
    			if (String.IsNullOrEmpty(_hostId))
    			{
    				String strHostName = Dns.GetHostName();
    				if (String.IsNullOrEmpty(strHostName) == false)
    					_hostId = strHostName;
    				else
    				{
    					// Find host by name
    					IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

    					// Enumerate IP addresses, pick an IPv4 address first
    					foreach (IPAddress ipaddress in iphostentry.AddressList)
    					{
    						if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    						{
    							_hostId = ipaddress.ToString();
    							break;
    						}
    					}
    				}
    			}

    			return _hostId;
    		}
    	}

		/// <summary>
		/// Server Instance Id
		/// </summary>
    	public static string ServerInstanceId
    	{
    		get
    		{
    			if (String.IsNullOrEmpty(_serverInstanceId))
    			{
    				_serverInstanceId = String.Format("Host={0}/Pid={1}", HostId, Process.GetCurrentProcess().Id);
    			}

    			return _serverInstanceId;
    		}
    	}

    	/// <summary>
    	/// A string representing the ID of the work queue processor.
    	/// </summary>
    	/// <remarks>
    	/// <para>
    	/// This ID is used to reset the work queue items.
    	/// </para>
    	/// <para>
    	/// For the time being, the machine ID is tied to the IP address. Assumimg the server
    	/// will be installed on a machine with DHCP disabled or if the DNS server always assign
    	/// the same IP for the machine, this will work fine.
    	/// </para>
    	/// <para>
    	/// Because of this implemenation, all instances of WorkQueueProcessor will have the same ID.
    	/// </para>
    	/// </remarks>
    	public static string ProcessorId
    	{
    		get
    		{
    			if (_processorId == null)
    			{
    				try
    				{
    					String strHostName = Dns.GetHostName();

    					// Find host by name
    					IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

    					// Enumerate IP addresses, pick an IPv4 address first
    					foreach (IPAddress ipaddress in iphostentry.AddressList)
    					{
    						if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    						{
    							_processorId = ipaddress.ToString();
    							break;
    						}
    					}
    					if (_processorId == null)
    					{
    						foreach (IPAddress ipaddress in iphostentry.AddressList)
    						{
    							_processorId = ipaddress.ToString();
    							break;
    						}
    					}
    				}
    				catch (Exception e)
    				{
    					Platform.Log(LogLevel.Error, e, "Cannot resolve hostname into IP address");
    				}
    			}

    			if (_processorId == null)
    			{
    				Platform.Log(LogLevel.Warn, "Could not determine hostname or IP address of the local machine. Work Queue Processor ID is set to Unknown");
    				_processorId = "Unknown";

    			}

    			return _processorId;
    		}
    	}

	    public static bool IsManifestVerified
	    {
	        get { return new ProductManifestChecker().VerifyManifest(); }
	    }   
    }
}
