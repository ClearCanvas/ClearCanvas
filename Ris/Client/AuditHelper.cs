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

using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Helper class for RIS client-side auditing.
	/// </summary>
	/// <remarks>
	/// Static methods on this class are safe for concurrent access by multiple threads.
	/// </remarks>
	public static class AuditHelper
	{
		/// <summary>
		/// Defines the set of operations that are audited.
		/// </summary>
		public static class Operations
		{
			public const string FolderItemPreview = "FolderItem:Preview";
			public const string DocumentWorkspaceOpen = "Workspace:Open";
		}

		private static readonly AuditLog _auditLog = new AuditLog(ProductInformation.Component, "RIS");

		/// <summary>
		/// Log that the specified document workspace was opened.
		/// </summary>
		/// <param name="document"></param>
		public static void DocumentWorkspaceOpened(Document document)
		{
			var data = document.GetAuditData();
			if(data != null)
			{
				Log(data.Operation, data);
			}
		}

		/// <summary>
		/// Log that the preview page for the specified folder items was viewed.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="selectedItems"></param>
		public static void FolderItemPreviewed(IFolder folder, object[] selectedItems)
		{
			// the folder system can be null sometimes (e.g. a container folder),
			// in which case this can't be audited (and presumably doesn't need to be)
			if (folder.FolderSystem == null)
				return;

			var datas = folder.FolderSystem.GetPreviewAuditData(folder, selectedItems);
			foreach (var auditData in datas)
			{
				Log(auditData.Operation, auditData);
			}
		}

		/// <summary>
		/// Log the specified operation.
		/// </summary>
		/// <param name="operation">The name of the operation performed.</param>
		/// <param name="detailsDataContract">The audit message details.</param>
		private static void Log(string operation, object detailsDataContract)
		{
			if (LoginSession.Current == null)
				return;

			lock (_auditLog)
			{
				_auditLog.WriteEntry(operation, JsmlSerializer.Serialize(detailsDataContract, "Audit"));
			}
		}

	}
}
