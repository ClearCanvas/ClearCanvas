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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Server.ShredHost
{
	internal sealed class WcfHelper
    {
		private enum HostBindingType
		{
            BasicHttp,
			WSHttp,
			WSDualHttp,
			NetTcp,
			NamedPipes
		}

		static public ServiceEndpointDescription StartBasicHttpHost<TServiceType, TServiceInterfaceType>(string name, string description, int port, string serviceAddressBase)
        {
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.BasicHttp, port, 0, serviceAddressBase);
        }

		static public ServiceEndpointDescription StartHttpHost<TServiceType, TServiceInterfaceType>(string name, string description, int port, string serviceAddressBase)
		{
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.WSHttp, port, 0, serviceAddressBase);
		}

		static public ServiceEndpointDescription StartHttpDualHost<TServiceType, TServiceInterfaceType>(string name, string description, int port, string serviceAddressBase)
		{
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.WSDualHttp, port, 0, serviceAddressBase);
		}

		static public ServiceEndpointDescription StartNetTcpHost<TServiceType, TServiceInterfaceType>(string name, string description, int port, int metaDataHttpPort, string serviceAddressBase)
		{
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.NetTcp, metaDataHttpPort, port, serviceAddressBase);
		}

		static public ServiceEndpointDescription StartNetPipeHost<TServiceType, TServiceInterfaceType>(string name, string description, int metaDataHttpPort, string serviceAddressBase)
		{
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.NamedPipes, metaDataHttpPort, 0, serviceAddressBase);
		}

		static private ServiceEndpointDescription StartHost<TServiceType, TServiceInterfaceType>
			(
				string name, 
				string description, 
				HostBindingType bindingType,
				int httpPort,
				int tcpPort,
				string serviceAddressBase)
        {

			ServiceEndpointDescription sed = new ServiceEndpointDescription(name, description);

			sed.Binding = GetBinding<TServiceInterfaceType>(bindingType);
			sed.ServiceHost = new ServiceHost(typeof(TServiceType));
			var endpointAddress = GetEndpointAddress(name, bindingType, tcpPort, httpPort, serviceAddressBase);

			ServiceMetadataBehavior metadataBehavior = sed.ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (null == metadataBehavior)
			{
				if (bindingType == HostBindingType.BasicHttp ||
					bindingType == HostBindingType.WSHttp ||
					bindingType == HostBindingType.WSDualHttp)
				{
					metadataBehavior = new ServiceMetadataBehavior();
					metadataBehavior.HttpGetEnabled = true;
					metadataBehavior.HttpGetUrl = endpointAddress;
					sed.ServiceHost.Description.Behaviors.Add(metadataBehavior);
				}
			}

			ServiceDebugBehavior debugBehavior = sed.ServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
			if (null == debugBehavior)
			{
				debugBehavior = new ServiceDebugBehavior();
				debugBehavior.IncludeExceptionDetailInFaults = true;
				sed.ServiceHost.Description.Behaviors.Add(debugBehavior);
			}

			sed.ServiceHost.AddServiceEndpoint(typeof(TServiceInterfaceType), sed.Binding, endpointAddress);
			sed.ServiceHost.Open();

			return sed;
        }

		static private Binding GetBinding<TServiceInterfaceType>(HostBindingType bindingType)
		{
			object[] contractObjects = typeof(TServiceInterfaceType).GetCustomAttributes(typeof(ServiceContractAttribute), false);
			string serviceConfigurationName = null;
			if (contractObjects.Length > 0)
				serviceConfigurationName = ((ServiceContractAttribute)contractObjects[0]).ConfigurationName;

			if (String.IsNullOrEmpty(serviceConfigurationName))
				serviceConfigurationName = typeof(TServiceInterfaceType).Name;
							
			Binding binding;

			if (bindingType == HostBindingType.NetTcp)
			{
				string configurationName = String.Format("{0}_{1}", typeof(NetTcpBinding).Name, serviceConfigurationName);
				try
				{
					binding = new NetTcpBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, String.Format("unable to load binding configuration {0}; using default binding configuration", configurationName));
					binding = new NetTcpBinding();
				}

				((NetTcpBinding)binding).PortSharingEnabled = true;
			}
			else if (bindingType == HostBindingType.NamedPipes)
			{
				string configurationName = String.Format("{0}_{1}", typeof(NetNamedPipeBinding).Name, serviceConfigurationName);
				try
				{
					binding = new NetNamedPipeBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new NetNamedPipeBinding();
				}
			}
			else if (bindingType == HostBindingType.WSDualHttp)
			{
				string configurationName = String.Format("{0}_{1}", typeof(WSDualHttpBinding).Name, serviceConfigurationName);
				try
				{
					binding = new WSDualHttpBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new WSDualHttpBinding();
				}
			}
            else if (bindingType == HostBindingType.WSHttp)
			{
				string configurationName = String.Format("{0}_{1}", typeof(WSHttpBinding).Name, serviceConfigurationName);
				try
				{
					binding = new WSHttpBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new WSHttpBinding();
				}
			}
            else
            {	
                string configurationName = String.Format("{0}_{1}", typeof(BasicHttpBinding).Name, serviceConfigurationName);
                try
                {
                    binding = new BasicHttpBinding(configurationName);
                }
                catch
                {
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new BasicHttpBinding();
                }
            }

			return binding;
		}

        static public void StopHost(ServiceEndpointDescription sed)
        {
            sed.ServiceHost.Close();
        }

		private static Uri GetEndpointAddress(string endpointName, HostBindingType bindingType, int tcpPort, int httpPort, string serviceAddressBase)
		{
			var serviceBase = string.IsNullOrEmpty(serviceAddressBase) ? endpointName : serviceAddressBase;
			var serviceAdress = string.Format("{0}/{1}", serviceBase, endpointName);

			if (bindingType == HostBindingType.NetTcp)
				return new UriBuilder(String.Format("net.tcp://localhost:{0}/{1}", tcpPort, serviceAdress)).Uri;
			if (bindingType == HostBindingType.NamedPipes)
				return new UriBuilder(String.Format("net.pipe://localhost/{0}", serviceAdress)).Uri;

			return new UriBuilder(String.Format("http://localhost:{0}/{1}", httpPort, serviceAdress)).Uri;
		}
	}
}