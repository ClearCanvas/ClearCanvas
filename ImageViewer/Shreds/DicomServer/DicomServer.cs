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

using System.IO;
using System.Net;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class DicomServer
	{
		#region Context

		public class DicomServerContext : IDicomServerContext
		{
			private readonly DicomServer _server;
		
			internal DicomServerContext(DicomServer server)
			{
				_server = server;
     		}

			#region IDicomServerContext Members

			public string AETitle
			{
				get { return _server.AETitle; }
			}

			public string Host
			{
				get { return _server.Host; }	
			}

			public int Port
			{
				get { return _server.Port; }	
			}

		    public StorageConfiguration StorageConfiguration
		    {
                get { return StudyStore.GetConfiguration(); }
		    }

			#endregion
		}

		#endregion

		private readonly IDicomServerContext _context;
		private readonly DicomScp<IDicomServerContext> _scp;

		private readonly string _aeTitle;
		private readonly string _host;
		private readonly int _port;

		public DicomServer(DicomServerConfiguration serverConfiguration)
		{
			_aeTitle = serverConfiguration.AETitle;
            _host = serverConfiguration.HostName;
			_port = serverConfiguration.Port;

			_context = new DicomServerContext(this);
			_scp = new DicomScp<IDicomServerContext>(_context, AssociationVerifier.VerifyAssociation);
		}

		#region Public Properties

		public string AETitle
		{
			get { return _aeTitle; }
		}

		public string Host
		{
			get { return _host; }	
		}

		public int Port
		{
			get { return _port; }
		}

		#endregion

		#region Server Startup/Shutdown

		public void Start()
		{
			IPHostEntry entry = Dns.GetHostEntry(_host);
			IPAddress address = entry.AddressList[0];
			IPAddress localhost = Dns.GetHostEntry("localhost").AddressList[0];
			if (localhost.Equals(address))
				address = IPAddress.Any;

			_scp.AeTitle = _aeTitle;
			_scp.ListenPort = _port;
			_scp.Start(address);
		}

		public void Stop()
		{
			_scp.Stop();
		}

		#endregion
	}
}
