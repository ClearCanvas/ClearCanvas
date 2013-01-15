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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public abstract class Document
	{
		private readonly string _key;
		private readonly IDesktopWindow _desktopWindow;
		private event EventHandler<ClosedEventArgs> _closed;

		protected Document(EntityRef subject, IDesktopWindow desktopWindow)
		{
			_key = DocumentManager.GenerateDocumentKey(this, subject);
			_desktopWindow = desktopWindow;
		}

		public string Key
		{
			get { return _key; }
		}

		public void Open()
		{
			var workspace = GetWorkspace(_key);
			if (workspace != null)
			{
				workspace.Activate();
			}
			else
			{
				workspace = LaunchWorkspace();

				AuditHelper.DocumentWorkspaceOpened(this);

				if (workspace != null)
				{
					workspace.Closed += DocumentClosedEventHandler;
					DocumentManager.RegisterDocument(this);
				}
			}
		}

		public bool Close()
		{
			var workspace = GetWorkspace(_key);
			return workspace != null && workspace.Close();
		}

		public abstract bool SaveAndClose();

		public abstract string GetTitle();

		public abstract IApplicationComponent GetComponent();

		/// <summary>
		/// Gets the audit data for opening this document, or null if auditing is not required.
		/// </summary>
		/// <returns></returns>
		public abstract OpenWorkspaceOperationAuditData GetAuditData();

		public event EventHandler<ClosedEventArgs> Closed
		{
			add { _closed += value; }
			remove { _closed -= value; }
		}

		#region Private Helpers

		private void DocumentClosedEventHandler(object sender, ClosedEventArgs e)
		{
			DocumentManager.UnregisterDocument(this);
			EventsHelper.Fire(_closed, this, e);
		}

		private Workspace LaunchWorkspace()
		{
			Workspace workspace = null;

			try
			{
				workspace = ApplicationComponent.LaunchAsWorkspace(
					_desktopWindow,
					GetComponent(),
					GetTitle(),
					_key);
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, _desktopWindow);
			}

			return workspace;
		}

		private static Workspace GetWorkspace(string documentKey)
		{
			foreach (var window in Desktop.Application.DesktopWindows)
			{
				if (!string.IsNullOrEmpty(documentKey) && window.Workspaces.Contains(documentKey))
					return window.Workspaces[documentKey];
			}

			return null;
		}

		#endregion
	}
}
