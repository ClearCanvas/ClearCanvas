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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("undo", "imageviewer-contextmenu/MenuUndo", "Undo", KeyStroke = XKeys.Control | XKeys.Z, InitiallyAvailable = false)]
	[MenuAction("undo", "global-menus/MenuEdit/MenuUndo", "Undo", KeyStroke = XKeys.Control | XKeys.Z)]
	[ButtonAction("undo", "global-toolbars/ToolbarStandard/ToolbarUndo", "Undo", KeyStroke = XKeys.Control | XKeys.Z)]
	[EnabledStateObserver("undo", "UndoEnabled", "UndoEnabledChanged")]
	[IconSet("undo", "Icons.UndoToolSmall.png", "Icons.UndoToolMedium.png", "Icons.UndoToolLarge.png")]
	[Tooltip("undo", "TooltipUndo")]
	[GroupHint("undo", "Application.Edit.Undo")]

	[MenuAction("redo", "imageviewer-contextmenu/MenuRedo", "Redo", KeyStroke = XKeys.Control | XKeys.Y, InitiallyAvailable = false)]
	[MenuAction("redo", "global-menus/MenuEdit/MenuRedo", "Redo", KeyStroke = XKeys.Control | XKeys.Y)]
	[ButtonAction("redo", "global-toolbars/ToolbarStandard/ToolbarRedo", "Redo", KeyStroke = XKeys.Control | XKeys.Y)]
	[EnabledStateObserver("redo", "RedoEnabled", "RedoEnabledChanged")]
	[IconSet("redo", "Icons.RedoToolSmall.png", "Icons.RedoToolMedium.png", "Icons.RedoToolLarge.png")]
	[Tooltip("redo", "TooltipRedo")]
	[GroupHint("redo", "Application.Edit.Redo")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class EditHistoryTool : ImageViewerTool
	{
		private bool _undoEnabled;
		private event EventHandler _undoEnabledChangedEvent;

		private bool _redoEnabled;
		private event EventHandler _redoEnabledChangedEvent;

		/// <summary>
		/// Constructor
		/// </summary>
		public EditHistoryTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.ImageViewer.CommandHistory.CurrentCommandChanged += new EventHandler(OnCurrentCommandChanged);
		}

		public bool UndoEnabled
		{
			get { return _undoEnabled; }
			private set
			{
				if (value != _undoEnabled)
				{
					_undoEnabled = value;
					EventsHelper.Fire(_undoEnabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler UndoEnabledChanged
		{
			add { _undoEnabledChangedEvent += value; }
			remove { _undoEnabledChangedEvent -= value; }
		}

		public bool RedoEnabled
		{
			get { return _redoEnabled; }
			private set
			{
				if (value != _redoEnabled)
				{
					_redoEnabled = value;
					EventsHelper.Fire(_redoEnabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler RedoEnabledChanged
		{
			add { _redoEnabledChangedEvent += value; }
			remove { _redoEnabledChangedEvent -= value; }
		}

		public void Undo()
		{
			this.ImageViewer.CommandHistory.Undo();
		}

		public void Redo()
		{
			this.ImageViewer.CommandHistory.Redo();
		}

		void OnCurrentCommandChanged(object sender, EventArgs e)
		{
			this.UndoEnabled = this.ImageViewer.CommandHistory.CurrentCommandIndex > -1;
			this.RedoEnabled = this.ImageViewer.CommandHistory.CurrentCommandIndex < this.ImageViewer.CommandHistory.LastCommandIndex;
		}
	}
}