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
using System.Linq;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Common
{
    public interface IDicomServiceNode : IServiceNode, IApplicationEntity
    {
        bool IsLocal { get; }

        //object GetData(string key);
    }

    public interface IServiceNode
    {
        bool IsSupported<T>() where T : class;
        void GetService<T>(Action<T> withService) where T : class;
        T GetService<T>() where T : class;
    }

    public abstract class ServiceNode : IServiceNode
    {
        #region IServiceNode Members

        public virtual bool IsSupported<T>() where T : class
        {            
            try
            {
                var context = new ServiceNodeServiceProviderContext(this);
                foreach (IServiceNodeServiceProvider provider in new ServiceNodeServiceProviderExtensionPoint().CreateExtensions())
                {
                    provider.SetContext(context);
                    if (provider.IsSupported(typeof(T)))
                        return true;
                }
            }
            catch (NotSupportedException)
            {
            }

            return false;
        }

        public void GetService<T>(Action<T> withService) where T : class
        {
            WithService(GetService<T>(), withService);
        }

        #endregion

        public virtual T GetService<T>() where T : class
        {
            try
            {
                var context = new ServiceNodeServiceProviderContext(this);
                foreach (IServiceNodeServiceProvider provider in new ServiceNodeServiceProviderExtensionPoint().CreateExtensions())
                {
                    provider.SetContext(context);
                    var service = provider.GetService(typeof(T));
                    if (service != null)
                        return service as T;
                }
            }
            catch (NotSupportedException)
            {
            }

            throw new NotSupportedException(String.Format("Service node doesn't support service '{0}'.", typeof(T).FullName));
        }

        public static void WithService<T>(T service, Action<T> withService) where T : class
        {
            try
            {
                withService(service);
            }
            finally
            {
                var disposable = service as IDisposable;
                if (disposable != null)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Warn, ex, "Error disposing service object of type '{0}'", typeof(T).FullName);
                    }
                }
            }
        }
    }

    public static class ServiceNodeExtensions
    {
        public static IDicomServiceNode ToServiceNode(this DicomServerConfiguration serverConfiguration)
        {
            Platform.CheckForNullReference(serverConfiguration, "serverConfiguration");
            return new DicomServiceNode(serverConfiguration);
        }

        public static IDicomServiceNode ToServiceNode(this ServerDirectoryEntry directoryEntry)
        {
            Platform.CheckForNullReference(directoryEntry, "directoryEntry");
            return new DicomServiceNode(directoryEntry);
        }

        public static IDicomServiceNode ToServiceNode(this IApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            var dicomServiceNode = server as IDicomServiceNode;
            if (dicomServiceNode != null)
                return dicomServiceNode;

            IDicomServiceNode serviceNode = null;
            if (!String.IsNullOrEmpty(server.Name))
                serviceNode = ServerDirectory.ServerDirectory.GetRemoteServerByName(server.Name);

            if (serviceNode == null)
                serviceNode = ServerDirectory.ServerDirectory.GetRemoteServersByAETitle(server.AETitle).FirstOrDefault();

            if (serviceNode == null)
                return new DicomServiceNode(server);

            return serviceNode;
        }

        public static ServerDirectoryEntry ToDataContract(this IDicomServiceNode serviceNode)
        {
            Platform.CheckExpectedType(serviceNode, typeof(DicomServiceNode));
            var node = (DicomServiceNode) serviceNode;
            return new ServerDirectoryEntry(node.Server){IsPriorsServer = node.IsPriorsServer, Data = node.ExtensionData};
        }

        public static bool ResolveServer(this Identifier identifier, bool defaultToLocal)
        {
            var server = FindServer(identifier, defaultToLocal);
            if (server != null)
            {
                identifier.RetrieveAE = server;
                return true;
            }

            return false;
        }

        public static IDicomServiceNode FindServer(this IIdentifier identifier, bool defaultToLocal)
        {
            if (identifier.RetrieveAE != null)
                return identifier.RetrieveAE.ToServiceNode();

            Platform.Log(LogLevel.Debug, "Identifier.RetrieveAE is not set.");

            IDicomServiceNode server = null;
            if (!String.IsNullOrEmpty(identifier.RetrieveAeTitle))
                server = ServerDirectory.ServerDirectory.GetRemoteServersByAETitle(identifier.RetrieveAeTitle).FirstOrDefault();

            var local = ServerDirectory.ServerDirectory.GetLocalServer();
            if ((server == null && defaultToLocal) || identifier.RetrieveAeTitle == local.AETitle)
                server = local;

            return server;
        }
    }

    internal class ServiceNodeServiceProviderContext : IServiceNodeServiceProviderContext
    {
        public ServiceNodeServiceProviderContext(IServiceNode serviceNode)
        {
            ServiceNode = serviceNode;
        }

        public IServiceNode ServiceNode { get; set; }
    }

    public interface IServiceNodeServiceProviderContext
    {
        IServiceNode ServiceNode { get; }
    }

    public interface IServiceNodeServiceProvider
    {
        void SetContext(IServiceNodeServiceProviderContext context);

        bool IsSupported(Type type);
        object GetService(Type type);
    }

    public abstract class ServiceNodeServiceProvider : IServiceNodeServiceProvider
    {
        protected IServiceNodeServiceProviderContext Context { get; private set; }

        public abstract bool IsSupported(Type type);
        public abstract object GetService(Type type);

        #region IServiceNodeServiceProvider Members

        void IServiceNodeServiceProvider.SetContext(IServiceNodeServiceProviderContext context)
        {
            Context = context;
        }

        #endregion
    }

    [ExtensionPoint]
    public class ServiceNodeServiceProviderExtensionPoint : ExtensionPoint<IServiceNodeServiceProvider>
    {
    }
}
