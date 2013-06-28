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
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
	/// <summary>
	/// Base class for <see cref="DicomEditorComponent"/> tools.
	/// </summary>
	public abstract class DicomEditorTool : Tool<IDicomEditorToolContext>
	{
		private readonly bool _isLocalOnly;
		private event EventHandler _enabledChanged;
		private bool _enabled;

		/// <summary>
		/// Constructs a new <see cref="DicomEditorTool"/> for all types of DICOM sources.
		/// </summary>
		protected DicomEditorTool() : this(false) {}

		/// <summary>
		/// Constructs a new <see cref="DicomEditorTool"/> for the specified types of DICOM sources.
		/// </summary>
		/// <param name="isLocalOnly">Specifies that the tool should only be enabled for locally-available DICOM files (i.e. not streamed).</param>
		protected DicomEditorTool(bool isLocalOnly)
		{
			_isLocalOnly = isLocalOnly;
		}

		/// <summary>
		/// Gets if the tool is only enabled for locally-available DICOM files (i.e. not streamed).
		/// </summary>
		public bool IsLocalOnly
		{
			get { return _isLocalOnly; }
		}

		/// <summary>
		/// Gets or sets if the tool is currently enabled.
		/// </summary>
		/// <remarks>
		/// Note that the tool cannot be enabled if it was constructed as a local-file only tool and the currently loaded file is not local.
		/// </remarks>
		public virtual bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_isLocalOnly)
					value = value & this.Context.IsLocalFile;

				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Notifies that the <see cref="Enabled"/> state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Called by the framework to allow the tool to initialize itself.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			this.Enabled = true;
			this.Context.TagEdited += OnTagEdited;
			this.Context.DisplayedDumpChanged += OnDisplayedDumpChanged;
			this.Context.SelectedTagChanged += OnSelectedTagChanged;
			this.Context.IsLocalFileChanged += OnIsLocalFileChanged;
		}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			this.Context.TagEdited -= OnTagEdited;
			this.Context.DisplayedDumpChanged -= OnDisplayedDumpChanged;
			this.Context.SelectedTagChanged -= OnSelectedTagChanged;
			this.Context.IsLocalFileChanged -= OnIsLocalFileChanged;

			base.Dispose(disposing);
		}

		public void Activate()
		{
			Context.EnsureChangesCommitted();

			// ensures any pending updates have a chance to resolve before the tool action is performed
			var synchronizationContext = SynchronizationContext.Current;
			if (synchronizationContext != null)
			{
				synchronizationContext.Post(s => DoActivate(), null);
			}
			else
			{
				DoActivate();
			}
		}

		private void DoActivate()
		{
			try
			{
				ActivateCore();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, Context.DesktopWindow);
			}
		}

		protected abstract void ActivateCore();

		protected virtual void OnDisplayedDumpChanged(object sender, DisplayedDumpChangedEventArgs e) {}

		protected virtual void OnTagEdited(object sender, EventArgs e) {}

		protected virtual void OnIsLocalFileChanged(object sender, EventArgs e) {}

		protected virtual void OnSelectedTagChanged(object sender, EventArgs e) {}
	}
}