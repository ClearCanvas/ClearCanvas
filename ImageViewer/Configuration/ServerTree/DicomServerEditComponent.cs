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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomServerEditComponent"/>
	/// </summary>
	[ExtensionPoint]
	public sealed class DicomServerEditComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(DicomServerEditComponentViewExtensionPoint))]
	public class DicomServerEditComponent : ApplicationComponent
	{
		private class ConflictingServerValidationRule : IValidationRule
		{
			private readonly string _propertyName;

			public ConflictingServerValidationRule(string propertyName)
			{
				_propertyName = propertyName;
			}

			#region IValidationRule Members

			public string PropertyName
			{
				get { return _propertyName; }
			}

			public ValidationResult GetResult(IApplicationComponent component)
			{
				DicomServerEditComponent serverComponent = (DicomServerEditComponent)component;

				ServerTree serverTree = serverComponent._serverTree;

				bool isConflicted;
				string conflictingServerPath;

				if (serverTree.CurrentNode.IsServer)
				{
					isConflicted = !serverTree.CanEditCurrentServer(serverComponent.ServerName, 
					                                               serverComponent.ServerAE, 
					                                               serverComponent.ServerHost, 
					                                               serverComponent.ServerPort, out conflictingServerPath);
				}
				else
				{
					isConflicted = !serverTree.CanAddServerToCurrentGroup(serverComponent.ServerName, 
					                                                     serverComponent.ServerAE, 
					                                                     serverComponent.ServerHost, 
					                                                     serverComponent.ServerPort, out conflictingServerPath);
				}

				if (isConflicted)
				{
					return new ValidationResult(false, String.Format(SR.FormatServerConflict, conflictingServerPath));
				}

				return new ValidationResult(true, "");

			}

			#endregion
		}

		public static readonly int MinimumPort = 1;
		public static readonly int MaximumPort = 65535;

		#region Private Fields

		private readonly ServerTree _serverTree;
		private string _serverName;
        private bool _serverNameReadOnly;
		private string _serverLocation;
		private string _serverAE;
		private string _serverHost;
		private int _serverPort;
		private bool _isStreaming;
		private int _headerServicePort;
		private int _wadoServicePort;
		private bool _isPriorsServer;

		#endregion

		public DicomServerEditComponent(ServerTree dicomServerTree)
		{
			_serverTree = dicomServerTree;
            if (_serverTree.CurrentNode.IsServer)
            {
                _serverNameReadOnly = true;
                var server = (IServerTreeDicomServer) _serverTree.CurrentNode;
                _serverName = server.Name;
                _serverLocation = server.Location;
                _serverAE = server.AETitle;
                _serverHost = server.HostName;
                _serverPort = server.Port;
                _isStreaming = server.IsStreaming;
                _headerServicePort = server.HeaderServicePort;
                _wadoServicePort = server.WadoServicePort;

				GetServerNodeMetaProperties(_serverName, out _isPriorsServer);
            }
            else
            {
                _serverNameReadOnly = false;
                _serverName = String.Empty;
                _serverLocation = String.Empty;
                _serverAE = String.Empty;
                _serverHost = String.Empty;
                _serverPort = ServerTreeDicomServer.DefaultPort;
                _isStreaming = false;
                _headerServicePort = ServerTreeDicomServer.DefaultHeaderServicePort;
                _wadoServicePort = ServerTreeDicomServer.DefaultWadoServicePort;
            }
		}

		public override void Start()
		{
			base.Start();

			// All of the properties contribute to conflicts in the server tree, 
			// so we'll just show the validation errors for each one.
			base.Validation.Add(new ConflictingServerValidationRule("ServerName"));
			base.Validation.Add(new ConflictingServerValidationRule("ServerAE"));
			base.Validation.Add(new ConflictingServerValidationRule("ServerHost"));
			base.Validation.Add(new ConflictingServerValidationRule("ServerPort"));
		}

		#region Presentation Model

		[ValidateNotNull(Message = "MessageServerNameCannotBeEmpty")]
		public string ServerName
		{
			get { return _serverName; }
			set
			{
                if (_serverName == value || _serverNameReadOnly)
					return;

				_serverName = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerName");
			}
		}

        public bool ServerNameReadOnly
	    {
            get { return _serverNameReadOnly; }
	    }

		public string ServerLocation
		{
			get { return _serverLocation; }
			set
			{
				if (_serverLocation == value)
					return;
				
				_serverLocation = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerLocation");
			}
		}

		[ValidateLength(1, 16, Message = "MessageServerAEInvalidLength")]
		[ValidateRegex(@"[\r\n\e\f\\]+", SuccessOnMatch = false, Message = "MessageServerAEInvalidCharacters")]
		public string ServerAE
		{
			get { return _serverAE; }
			set
			{
				if (_serverAE == value)
					return;

				_serverAE = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerAE");
			}
		}

		[ValidateNotNull(Message = "MessageHostNameCannotBeEmpty")]
		public string ServerHost
		{
			get { return _serverHost; }
			set
			{
				if (_serverHost == value)
					return;
				
				_serverHost = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerHost");
			}
		}

		[ValidateGreaterThanAttribute(0, Inclusive = false, Message = "MessagePortInvalid")]
		[ValidateLessThanAttribute(65536, Inclusive = false, Message = "MessagePortInvalid")]
		public int ServerPort
		{
			get { return _serverPort; }
			set
			{
				if (_serverPort == value)
					return;
				
				_serverPort = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("ServerPort");
			}
		}

		public bool IsStreaming
		{
			get { return _isStreaming;  }
			set
			{
				if (_isStreaming == value)
					return;

				_isStreaming = value;
				//StreamingPortsEnabled = !IsStreamin
				AcceptEnabled = true;
				NotifyPropertyChanged("IsStreaming");
			}
		}

		[ValidateGreaterThanAttribute(0, Inclusive = false, Message = "MessagePortInvalid")]
		[ValidateLessThanAttribute(65536, Inclusive = false, Message = "MessagePortInvalid")]
		public int HeaderServicePort
		{
			get { return _headerServicePort; }
			set
			{
				if (_headerServicePort == value)
					return;

				_headerServicePort = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("HeaderServicePort");
			}
		}

		[ValidateGreaterThanAttribute(0, Inclusive = false, Message = "MessagePortInvalid")]
		[ValidateLessThanAttribute(65536, Inclusive = false, Message = "MessagePortInvalid")]
		public int WadoServicePort
		{
			get { return _wadoServicePort; }
			set
			{
				if (_wadoServicePort == value)
					return;

				_wadoServicePort = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("WadoServicePort");
			}
		}

		public bool IsPriorsServer
		{
			get { return _isPriorsServer; }
			set
			{
				if (_isPriorsServer == value)
					return;

				_isPriorsServer = value;
				AcceptEnabled = true;
				NotifyPropertyChanged("IsPriorsServer");
			}
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
			set
			{
				if (value == this.Modified)
					return;

				this.Modified = value;
				NotifyPropertyChanged("AcceptEnabled");
			}
		}

		public void Accept()
		{
			ServerAE = ServerAE.Trim();
			ServerName = ServerName.Trim();
			ServerLocation = ServerLocation.Trim();
			ServerHost = ServerHost.Trim();

			if (base.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
			    var current = _serverTree.CurrentNode;
			    var allButCurrent = _serverTree.RootServerGroup.GetAllServers().Where(s => s != current).Cast<IServerTreeDicomServer>();
                var sameAETitleCount = allButCurrent.Count(s => s.AETitle == _serverAE);
                if (sameAETitleCount > 0)
                {
                    var message = sameAETitleCount == 1
                                      ? SR.ConfirmAETitleConflict_OneServer
                                      : String.Format(SR.ConfirmAETitleConflict_MultipleServers, sameAETitleCount);

                    if (DialogBoxAction.No == Host.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
                        return;
                }

			    var newServer = new ServerTreeDicomServer(
					_serverName, 
					_serverLocation, 
					_serverHost, 
					_serverAE, 
					_serverPort, 
					_isStreaming,
					_headerServicePort,
					_wadoServicePort);

				// edit current server
				if (_serverTree.CurrentNode.IsServer)
				{
					_serverTree.ReplaceDicomServerInCurrentGroup(newServer);
				}
					// add new server
				else if (_serverTree.CurrentNode.IsServerGroup)
				{
					((IServerTreeGroup) _serverTree.CurrentNode).AddChild(newServer);
					_serverTree.CurrentNode = newServer;
				}

                _serverTree.Save();

				SetServerNodeMetaProperties(_serverName, _isPriorsServer);

                _serverTree.FireServerTreeUpdatedEvent();

				this.ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
			}
		}

		public void Cancel()
		{
			Host.Exit();
		}

		#endregion

		private void GetServerNodeMetaProperties(string serverName, out bool isPriorsServer)
		{
			var directoryEntry = (ServerDirectoryEntry) null;
			Platform.GetService<IServerDirectory>(service => directoryEntry = (service.GetServers(new GetServersRequest {Name = serverName}).ServerEntries ?? new List<ServerDirectoryEntry>()).FirstOrDefault());

			isPriorsServer = false;
			if (directoryEntry != null)
			{
				isPriorsServer = directoryEntry.IsPriorsServer;
			}
		}

		private void SetServerNodeMetaProperties(string serverName, bool isPriorsServer)
		{
			var directoryEntry = (ServerDirectoryEntry) null;
			Platform.GetService<IServerDirectory>(service => directoryEntry = (service.GetServers(new GetServersRequest {Name = serverName}).ServerEntries ?? new List<ServerDirectoryEntry>()).FirstOrDefault());

			if (directoryEntry != null && (directoryEntry.IsPriorsServer != isPriorsServer))
			{
				directoryEntry.IsPriorsServer = isPriorsServer;

				Platform.GetService<IServerDirectory>(service => service.UpdateServer(new UpdateServerRequest {ServerEntry = directoryEntry}));
			}
		}
	}
}