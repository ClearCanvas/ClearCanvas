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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	public static class DocumentManager
	{
		private readonly static List<IFolderSystem> _folderSystems = new List<IFolderSystem>();
		private static readonly Dictionary<string, Document> _documentMap = new Dictionary<string, Document>();

		public static Document Get(string documentKey)
		{
			return !string.IsNullOrEmpty(documentKey) && _documentMap.ContainsKey(documentKey) ? _documentMap[documentKey] : null;
		}

		public static TDocument Get<TDocument>(EntityRef subject)
			where TDocument : Document
		{
			var documentKey = GenerateDocumentKey(typeof(TDocument), subject);
			return (TDocument) Get(documentKey);
		}

		public static List<TDocument> GetAll<TDocument>()
			where TDocument : Document
		{
			var documents = new List<TDocument>();
			var documentKeyBase = GenerateDocumentKey(typeof(TDocument), null);

			foreach (var key in _documentMap.Keys)
			{
				if (!string.IsNullOrEmpty(documentKeyBase) && key.Contains(documentKeyBase))
					documents.Add((TDocument) _documentMap[key]);
			}

			return documents;
		}

		public static string GenerateDocumentKey(Document doc, EntityRef subject)
		{
			return GenerateDocumentKey(doc.GetType(), subject);
		}

		public static void RegisterDocument(Document document)
		{
			if (!_documentMap.ContainsKey(document.Key))
				_documentMap[document.Key] = document;
		}

		public static void UnregisterDocument(Document document)
		{
			if (_documentMap.ContainsKey(document.Key))
				_documentMap.Remove(document.Key);
		}

		public static void RegisterFolderSystem(IFolderSystem folderSystem)
		{
			if (!_folderSystems.Contains(folderSystem))
				_folderSystems.Add(folderSystem);
		}

		public static void UnregisterFolderSystem(IFolderSystem folderSystem)
		{
			if (_folderSystems.Contains(folderSystem))
				_folderSystems.Remove(folderSystem);
		}

		public static void InvalidateFolder(Type folderType)
		{
			CollectionUtils.ForEach(_folderSystems,
				folderSystem => folderSystem.InvalidateFolders(folderType));
		}

		#region Private helper

		private static string GenerateDocumentKey(Type documentType, EntityRef subject)
		{
			return subject == null
				? string.Format("{0}", documentType)
				: string.Format("{0}+{1}", documentType, subject.ToString(false));
		}

		#endregion
	}
}
