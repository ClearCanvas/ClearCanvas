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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Base class for Audit helpers.
	/// </summary>
	public abstract class DicomAuditHelper
	{
		#region Static Members
		private static string _processId;
		private static string _processIpAddress;
		private static readonly object SyncLock = new object();
		private static string _application;
		private static string _processName;
		private static string _operation;
		#endregion

		#region Members
		private readonly AuditMessage _message = new AuditMessage();
        protected readonly List<ActiveParticipantContents> _participantList = new List<ActiveParticipantContents>(3);
        protected readonly List<AuditSourceIdentificationContents> _auditSourceList = new List<AuditSourceIdentificationContents>(1);
		protected readonly Dictionary<string, AuditParticipantObject> _participantObjectList = new Dictionary<string, AuditParticipantObject>();
		#endregion

		#region Constructors
		public DicomAuditHelper(string operation)
		{
			_operation = operation;
		}
		#endregion

		#region Static Properties
		public static string ProcessIpAddress
		{
			get
			{
				lock (SyncLock)
				{
					if (_processIpAddress == null)
					{
						string hostName = Dns.GetHostName();
						IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
						foreach (IPAddress ip in ipAddresses)
						{
							if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
							{
                                // Force IPv4 address if its available, its more human readable
								_processIpAddress = ip.ToString();
							    break;
							}
							if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
							{
								_processIpAddress = ip.ToString();
							}
						}
					}
					return _processIpAddress;
				}
			}
		}
		public static string ClientIpAddress
		{
			get
			{
				if (HostingEnvironment.IsHosted)
				{
					string ipList = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

					if (!string.IsNullOrEmpty(ipList))
					{
						return ipList.Split(',')[0];
					}

					return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
				}

				return string.Empty;
			}
		}
		public static string ProcessName
		{
			get
			{
				lock (SyncLock)
				{
					if (_processName == null) _processName = Process.GetCurrentProcess().ProcessName;
					return _processName;
				}
			}
		
		}

		public static string ProcessId
		{
			get
			{
				lock (SyncLock)
				{
					return _processId ?? (_processId = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
				}
			}

		}
		public static string Application
		{
			get
			{
				lock (SyncLock)
					return _application;
			}
			set
			{
				lock (SyncLock)
				{
					_application = value;
				}
			}
		}
		#endregion

		#region Properties
		protected AuditMessage AuditMessage
		{
			get { return _message; }
		}

		public string Operation
		{
			get { return _operation; }
		}
		#endregion

		#region Public Methods

		public bool Verify(out string failureMessage)
		{
			Exception ex;
			if (!Verify(out ex))
			{
				failureMessage = ex.Message;
				return false;
			}
			failureMessage = string.Empty;
			return true;
		}

		public bool Verify(out Exception exception)
		{
			XmlSchema schema;

			using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "DicomAuditMessageSchema.xsd"))
			{
				if (stream == null)
					throw new DicomException("Unable to load XSD resource (is the XSD an embedded resource?): " + "DicomAuditMessageSchema.xsd");
				schema = XmlSchema.Read(stream, null);
			}

			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.Schemas = new XmlSchemaSet();
				xmlReaderSettings.Schemas.Add(schema);
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;

				XmlReader xmlReader = XmlTextReader.Create(new StringReader(Serialize()), xmlReaderSettings);
				while (xmlReader.Read()) ;
				xmlReader.Close();
			}
			catch (Exception e)
			{
				exception = e;
				return false;
			}

			exception = null;
			return true;
		}

		public string Serialize()
		{
			return Serialize(false);
		}

		public string Serialize(bool format)
		{
			AuditMessage.ActiveParticipant = _participantList.ToArray();
			AuditMessage.AuditSourceIdentification = CollectionUtils.FirstElement(_auditSourceList);

            List<ParticipantObjectIdentificationContents> list = new List<ParticipantObjectIdentificationContents>(_participantObjectList.Values.Count);
			foreach (AuditParticipantObject o in _participantObjectList.Values)
			{
                list.Add(new ParticipantObjectIdentificationContents(o));
			}
			AuditMessage.ParticipantObjectIdentification = list.ToArray();

			TextWriter tw = new StringWriter();
			
			XmlWriterSettings settings = new XmlWriterSettings();

			settings.Encoding = Encoding.UTF8;
			if (format)
			{
				settings.NewLineOnAttributes = false;
				settings.Indent = true;
				settings.IndentChars = "  ";
			}
			else
			{
				settings.NewLineOnAttributes = false;
				settings.Indent = false;
			}
			XmlWriter writer = XmlWriter.Create(tw,settings);

			XmlSerializer serializer = new XmlSerializer(typeof(AuditMessage));
			serializer.Serialize(writer, AuditMessage);
			return tw.ToString();
		}
		#endregion

		#region Protected Methods
	

		protected void InternalAddActiveDicomParticipant(AssociationParameters parms)
		{
			if (parms is ClientAssociationParameters)
			{
				_participantList.Add(
                    new ActiveParticipantContents(RoleIDCode.Source, "AETITLE=" + parms.CallingAE, null, null,
													  parms.LocalEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
				_participantList.Add(
                    new ActiveParticipantContents(RoleIDCode.Destination, "AETITLE=" + parms.CalledAE, null, null,
													  parms.RemoteEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
			}
			else
			{
				_participantList.Add(
                    new ActiveParticipantContents(RoleIDCode.Source, "AETITLE=" + parms.CallingAE, null, null,
													  parms.RemoteEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
				_participantList.Add(
                    new ActiveParticipantContents(RoleIDCode.Destination, "AETITLE=" + parms.CalledAE, null, null,
													  parms.LocalEndPoint.Address.ToString(), NetworkAccessPointTypeEnum.IpAddress, null));
			}
		}

		protected void InternalAddActiveDicomParticipant(string sourceAE, string sourceHost, string destinationAE, string destinationHost)
		{
			IPAddress x;
            _participantList.Add(new ActiveParticipantContents(RoleIDCode.Source, "AETITLE=" + sourceAE, null, null,
				sourceHost, IPAddress.TryParse(sourceHost, out x) ? NetworkAccessPointTypeEnum.IpAddress : NetworkAccessPointTypeEnum.MachineName, null));
            _participantList.Add(new ActiveParticipantContents(RoleIDCode.Destination, "AETITLE=" + destinationAE, null, null,
				destinationHost, IPAddress.TryParse(destinationHost, out x) ? NetworkAccessPointTypeEnum.IpAddress : NetworkAccessPointTypeEnum.MachineName, null));
		}

		protected void InternalAddAuditSource(DicomAuditSource auditSource)
		{
			_auditSourceList.Add(new AuditSourceIdentificationContents(auditSource));
		}
		
		protected void InternalAddParticipantObject(string key, AuditParticipantObject study)
		{
			_participantObjectList.Add(key, study);
		}

		protected bool HasParticipantObject(string key)
		{
			return _participantObjectList.ContainsKey(key);
		}

		protected void InternalAddActiveParticipant(AuditActiveParticipant participant)
		{
			_participantList.Add(new ActiveParticipantContents(participant));
		}
		#endregion

		protected void InternalAddStorageInstance(StorageInstance instance)
		{
			if (_participantObjectList.ContainsKey(instance.StudyInstanceUid))
			{
				AuditStudyParticipantObject study = _participantObjectList[instance.StudyInstanceUid] as AuditStudyParticipantObject;

				if (study!=null)
				{
					study.AddStorageInstance(instance);
				}
			}
			else
			{
				AuditStudyParticipantObject o = new AuditStudyParticipantObject(instance.StudyInstanceUid);
				o.AddStorageInstance(instance);
				_participantObjectList.Add(instance.StudyInstanceUid, o);
			}
		}
	}
}
