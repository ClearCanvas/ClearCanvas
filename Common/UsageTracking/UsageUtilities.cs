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

#define DEBUG_SERVER

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.UsageTracking
{
	/// <summary>
	/// Enum for specifying if a Usage Tracking message will be sent on the current thread or a background thread.
	/// </summary>
	public enum UsageTrackingThread
	{
		/// <summary>
		/// The Usage Tracking message will be sent on a background thread
		/// </summary>
		Background,

		/// <summary>
		/// The UsageTracking message will be sent on the current thread.
		/// </summary>
		Current,
	}

	/// <summary>
	/// Static helper class for implementing usage tracking of ClearCanvas applications.
	/// </summary>
	public static class UsageUtilities
	{
		#region Private Members

		private static event EventHandler<ItemEventArgs<DisplayMessage>> Message;
		private static readonly object _syncLock = new object();
		private static bool _first = true;

		#endregion

		#region Public Static Properties

	    public static bool IsEnabled
	    {
	        get { return UsageTrackingSettings.Default.Enabled; }
	    }

		/// <summary>
		/// Event which can receive display messages from the UsageTracking server
		/// </summary>
		/// <remarks>
		/// Note that the configuration option in <see cref="UsageTrackingSettings"/> must be enabled to receive these
		/// messages.
		/// </remarks>
		public static event EventHandler<ItemEventArgs<DisplayMessage>> MessageEvent
		{
			add
			{
				lock (_syncLock)
					Message += value;
			}
			remove
			{
				lock (_syncLock)
					Message -= value;
			}
		}

		#endregion

		#region Private Methods

		private static bool TrySend(RegisterRequest message, Binding binding, EndpointAddress endpointAddress)
		{
			try
			{
                RegisterResponse response;
				using (UsageTrackingServiceClient client = new UsageTrackingServiceClient(binding, endpointAddress))
				{
					response = client.Register(message);
				}
				if (response != null
				    && response.Message != null
				    && UsageTrackingSettings.Default.DisplayMessages)
				{
					EventsHelper.Fire(Message, null, new ItemEventArgs<DisplayMessage>(response.Message));
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private const string _trackingServerHost = "cc-blackmesa";
		private const string _trackingServerIp = "10.19.20.245";
		private const string _trackingServiceEndpoint = "https://{0}/Tracking/Service.svc";

		private static string TrackingServerHost
		{
			get { return _trackingServerHost; }
		}

		private static string TrackingServerIp
		{
			get { return _trackingServerIp; }
		}

		private static string TrackingServiceEndpoint
		{
			get { return _trackingServiceEndpoint; }
		}

		/// <summary>
		/// Send the UsageTracking message.
		/// </summary>
		/// <param name="theMessage"></param>
		private static void Send(object theMessage)
		{
			try
			{
				lock (_syncLock)
				{
					if (_first)
					{
#if DEBUG_SERVER
						// Note, this is required when in debug mode and communicating with the test server,
						// which doesn't have an official cert, it isn't required for communicating with
						// the production server.
						ServicePointManager.ServerCertificateValidationCallback +=
							((sender, certificate, chain, sslPolicyErrors) =>
							 true);
#endif

						_first = false;
					}
				}

				UsageMessage message = theMessage as UsageMessage;
				if (message != null)
				{
					RegisterRequest req = new RegisterRequest
					                      	{
					                      		Message = message
					                      	};

#if UNIT_TESTS_USAGE_TRACKING
                    WSHttpBinding binding = new WSHttpBinding();
                    EndpointAddress endpointAddress = new EndpointAddress("http://localhost:8080/UsageTracking");
                    TrySend(req, binding, endpointAddress);
#else
					WSHttpBinding binding = new WSHttpBinding(SecurityMode.Transport);
					binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
					binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
					EndpointAddress endpointAddress = new EndpointAddress(string.Format(TrackingServiceEndpoint, TrackingServerHost));
					if (!TrySend(req, binding, endpointAddress))
					{
						endpointAddress = new EndpointAddress(string.Format(TrackingServiceEndpoint, TrackingServerIp));
						TrySend(req, binding, endpointAddress);
					}
#endif
				}
			}
			catch (Exception e)
			{
				// Fail silently
#if	DEBUG
				Platform.Log(LogLevel.Debug, e);
#endif
			}
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Register the usage of the application with a ClearCanvas server on a background thread.
		/// </summary>
		/// <remarks>
		/// A check is done of the <see cref="UsageTrackingSettings"/>, and if usage tracking is enabled, the 
		/// <paramref name="message"/> is sent to the ClearCanvas server.
		/// </remarks>
		/// <param name="message">The usage message to send.</param>
		/// <param name="thread">Flag telling if the usage will be sent on the current thread or a background thread.</param>
		public static void Register(UsageMessage message, UsageTrackingThread thread)
		{
			if (UsageTrackingSettings.Default.Enabled)
				try
				{
					UsageMessage theMessage = message;
					if (thread == UsageTrackingThread.Current)
						Send(theMessage);
					else if (thread == UsageTrackingThread.Background)
						ThreadPool.QueueUserWorkItem(Send, theMessage);
				}
				catch (Exception e)
				{
					// Fail silently
					Platform.Log(LogLevel.Debug, e);
				}
		}

		/// <summary>
		/// Get a <see cref="UsageMessage"/> for the application.
		/// </summary>
		/// <returns>
		/// <para>
		/// A new <see cref="UsageMessage"/> object with product, region, timestamp, license, and OS information filled in.
		/// </para>
		/// <para>
		/// The <see cref="UsageMessage"/> instance is used in conjunction with <see cref="Register"/> to send a usage message
		/// to ClearCanvas servers.
		/// </para>
		/// </returns>
		public static UsageMessage GetUsageMessage()
		{
			UsageMessage msg;

			// if license key cannot be retrieved, send an empty string to maintain the existing data on the server
			string licenseString = String.Empty;
			string licenseType = String.Empty;
			DateTime? licenseExpiryTime = null;
			try
			{
				licenseString = LicenseInformation.LicenseKey;
				licenseExpiryTime = LicenseInformation.ExpiryTime;
				licenseType = LicenseInformation.LicenseType;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "An error has occurred when trying to get the license string");
			}
			finally
			{
				msg = new UsageMessage
				      	{
				      		Version = ProductInformation.GetVersion(true, true),
				      		Product = ProductInformation.Product,
				      		Component = GetComponentName(),
				      		Edition = ProductInformation.Edition,
				      		Release = ProductInformation.Release,
				      		AllowDiagnosticUse = LicenseInformation.DiagnosticUse != LicenseDiagnosticUse.None,
				      		Region = CultureInfo.CurrentCulture.Name,
				      		Timestamp = Platform.Time,
				      		OS = Environment.OSVersion.ToString(),
				      		MachineIdentifier = EnvironmentUtilities.MachineIdentifier,
				      		MessageType = UsageType.Other,
				      		LicenseString = licenseString,
				      		LicenseType = licenseType
				      	};

				if (licenseExpiryTime.HasValue)
					msg.LicenseExpiryTimeUTC = licenseExpiryTime.Value.ToUniversalTime();
			}

			return msg;
		}

        private static string GetComponentName()
        {
            if (string.IsNullOrEmpty(ProductInformation.SubComponent))
                return ProductInformation.Component;
            else
            {
                //TODO Phoenix5 - separate these fields
                return string.Format("{0} ({1})", ProductInformation.Component, ProductInformation.SubComponent);
            }
        }

		public static IEnumerable<UsageApplicationData> GetApplicationData(UsageType type)
		{
			return new UsageApplicationDataProviderExtensionPoint().CreateExtensions().Select(p => GetApplicationData(p, type)).Where(d => d != null);
		}

		private static UsageApplicationData GetApplicationData(IUsageApplicationDataProvider provider, UsageType type)
		{
			try
			{
				return provider.GetData(type);
			}
			catch (Exception)
			{
				return null;
			}
		}

		#endregion
	}
}