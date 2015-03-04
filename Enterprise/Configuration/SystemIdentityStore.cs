#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Serialization;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{
	[XmlType("SystemIdentityStore")]
	public class SystemIdentityStore
	{
		[XmlElement("SecretKey", DataType = "base64Binary")]
		public byte[] SecretKey { get; set; }

		private static readonly object _syncroot = new object();
		private static SystemIdentityStore _store;

		internal static SystemIdentityStore Load()
		{
			if (_store != null) return _store;

			lock (_syncroot)
			{
				if (_store == null)
				{
					const string userName = "{9067D0B7-41F0-4D1B-90CC-C384E688FA9F}";
					var serializer = new XmlSerializer(typeof (SystemIdentityStore));
					var documentName = typeof (SystemIdentityStore).FullName;
					var versionString = VersionUtils.ToPaddedVersionString(new Version(0, 0), false, false);

					var criteria = new ConfigurationDocumentSearchCriteria();
					criteria.User.EqualTo(userName);
					criteria.DocumentName.EqualTo(documentName);
					criteria.DocumentVersionString.EqualTo(versionString);

					SystemIdentityStore store = null;
					using (var scope = new PersistenceScope(PersistenceContextType.Read))
					{
						var broker = scope.Context.GetBroker<IConfigurationDocumentBroker>();
						var document = broker.Find(criteria).FirstOrDefault();
						if (document != null)
						{
							try
							{
								using (var reader = new StringReader(document.Body.DocumentText))
									store = (SystemIdentityStore) serializer.Deserialize(reader);
							}
							catch (Exception)
							{
								store = null;
							}
						}
						scope.Complete();
					}

					if (store == null || store.SecretKey == null || store.SecretKey.Length == 0)
					{
						if (store == null) store = new SystemIdentityStore();
						store.SecretKey = new byte[128];
						using (var crng = new RNGCryptoServiceProvider())
							crng.GetBytes(store.SecretKey);

						using (var scope = new PersistenceScope(PersistenceContextType.Update))
						using (var writer = new StringWriter())
						{
							serializer.Serialize(writer, store);

							var broker = scope.Context.GetBroker<IConfigurationDocumentBroker>();
							var document = broker.Find(criteria).FirstOrDefault();
							if (document != null)
							{
								document.Body.DocumentText = writer.ToString();
							}
							else
							{
								document = new ConfigurationDocument(documentName, versionString, userName, null);
								document.Body.DocumentText = writer.ToString();
								scope.Context.Lock(document, DirtyState.New);
							}
							scope.Complete();
						}
					}

					Interlocked.Exchange(ref _store, store);
				}
				return _store;
			}
		}
	}
}