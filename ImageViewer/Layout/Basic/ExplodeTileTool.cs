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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using System;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[DefaultMouseToolButton(XMouseButtons.Left)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ExplodeTileTool : MouseImageViewerTool
	{
		private delegate void NotifyRemoveUnexplodedTileCommand(IImageBox imageBox);

		private readonly Dictionary<IImageBox, UnexplodeTileCommand> _unexplodeCommands;
		private ListObserver<IImageBox> _imageBoxesObserver;

		#region UnexplodeTileCommand

		private class UnexplodeTileCommand : Command, IDisposable
		{
			private readonly IImageBox _imageBox;
			private readonly ListObserver<ITile> _listObserver;
			private readonly NotifyRemoveUnexplodedTileCommand _remove;
			private object _unexplodeMemento;

			public UnexplodeTileCommand(IImageBox imageBox, object unexplodeMemento, NotifyRemoveUnexplodedTileCommand remove)
			{
				_imageBox = imageBox;
				_unexplodeMemento = unexplodeMemento;
				_remove = remove;
				_listObserver = new ListObserver<ITile>(imageBox.Tiles, OnTilesChanged);
			}

			private void OnTilesChanged()
			{
				Dispose();
			}

			public override void Execute()
			{
 				if (_unexplodeMemento == null)
					return;

				IImageBox imageBox = _imageBox;
				object unexplodeMemento = _unexplodeMemento;
				Dispose();

				Execute(imageBox, unexplodeMemento);
			}

			private static void Execute(IImageBox imageBox, object unexplodeMemento)
			{
				MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
				memorableCommand.BeginState = imageBox.CreateMemento();

				IDisplaySet displaySet = imageBox.DisplaySet;
				IPresentationImage selectedImage;
				if (imageBox.SelectedTile != null)
					selectedImage = imageBox.SelectedTile.PresentationImage;
				else
					selectedImage = imageBox.TopLeftPresentationImage;

				imageBox.SetMemento(unexplodeMemento);

				//TODO (architecture): this wouldn't be necessary if we had a SetImageBoxGrid(imageBox[,]).
				//This stuff with mementos is actually a hacky workaround.

				bool locked = imageBox.DisplaySetLocked;
				imageBox.DisplaySetLocked = false;
				imageBox.DisplaySet = displaySet;
				imageBox.DisplaySetLocked = locked;

				if (selectedImage != null)
					imageBox.TopLeftPresentationImage = selectedImage;

				imageBox.Draw();
                imageBox.SelectDefaultTile();

				memorableCommand.EndState = imageBox.CreateMemento();

				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
				historyCommand.Name = SR.CommandExplodeTile;
				historyCommand.Enqueue(memorableCommand);
				imageBox.ParentPhysicalWorkspace.ImageViewer.CommandHistory.AddCommand(historyCommand);
			}
		
			#region IDisposable Members

			public void  Dispose()
			{
				if (_unexplodeMemento != null)
				{
					_unexplodeMemento = null;
					_listObserver.Dispose();
					_remove(_imageBox);
				}
			}

			#endregion
		}

		#endregion
		public ExplodeTileTool()
		{
			//this tool is activated on a double-click
			base.Behaviour &= ~MouseButtonHandlerBehaviour.CancelStartOnDoubleClick;
			_unexplodeCommands = new Dictionary<IImageBox, UnexplodeTileCommand>();
		}

		public override void Initialize()
		{
			base.Initialize();
			_imageBoxesObserver = new ListObserver<IImageBox>(ImageViewer.PhysicalWorkspace.ImageBoxes, OnImageBoxesChanged);
			_imageBoxesObserver.SuppressChangedEvent = true;
		}

		protected override void Dispose(bool disposing)
		{
			_imageBoxesObserver.Dispose();
			DisposeUnexplodeCommands();

			base.Dispose(disposing);
		}

		private static bool CanExplodeTiles(IImageBox imageBox)
		{
			return imageBox.Tiles.Count > 1 && !imageBox.ParentPhysicalWorkspace.Locked;
		}

		private void DisposeUnexplodeCommands()
		{
			foreach (UnexplodeTileCommand command in _unexplodeCommands.Values)
				command.Dispose();

			_unexplodeCommands.Clear();
		}

		private void RemoveUnexplodeCommand(IImageBox imageBox)
		{
			if (_unexplodeCommands.ContainsKey(imageBox))
			{
				_unexplodeCommands[imageBox].Dispose();
				_unexplodeCommands.Remove(imageBox);
			}
		}

		private void OnImageBoxesChanged()
		{
			DisposeUnexplodeCommands();
		}

		private void ExplodeSelectedTile(IImageBox imageBox)
		{
			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(imageBox);
			memorableCommand.BeginState = imageBox.CreateMemento();

			//set this here so checked will be correct.
			object unexplodeMemento = memorableCommand.BeginState;
			_unexplodeCommands[imageBox] = new UnexplodeTileCommand(imageBox, unexplodeMemento, RemoveUnexplodeCommand);
			
			IDisplaySet displaySet = imageBox.DisplaySet;
			IPresentationImage selectedImage = imageBox.SelectedTile.PresentationImage;
			imageBox.SetTileGrid(1, 1);

			//TODO (architecture): this wouldn't be necessary if we had a SetImageBoxGrid(imageBox[,]).
			//This stuff with mementos is actually a hacky workaround.

			bool locked = imageBox.DisplaySetLocked;
			imageBox.DisplaySetLocked = false;
			imageBox.DisplaySet = displaySet;
			imageBox.DisplaySetLocked = locked;

			imageBox.TopLeftPresentationImage = selectedImage;

			memorableCommand.EndState = imageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(imageBox);
			historyCommand.Name = SR.CommandExplodeTile;
			historyCommand.Enqueue(memorableCommand);
			imageBox.ParentPhysicalWorkspace.ImageViewer.CommandHistory.AddCommand(historyCommand);
		
			imageBox.Draw();
            imageBox.SelectDefaultTile();
        }

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ClickCount < 2)
				return false;

			IPhysicalWorkspace workspace = base.ImageViewer.PhysicalWorkspace;
			if (workspace == null)
				return false;

			IImageBox imageBox = workspace.SelectedImageBox;
			if (imageBox == null)
				return false;

			if (imageBox.SelectedTile == null || imageBox.SelectedTile.PresentationImage == null)
				return false;

			// This tool could just be lumped into the explode image box tool, but I like the separation.
			if (ExplodeImageBoxTool.IsExploded(base.ImageViewer))
				return false;

			if (_unexplodeCommands.ContainsKey(imageBox))
			{
				_unexplodeCommands[imageBox].Execute();
				return true;
			}
			else if (CanExplodeTiles(imageBox))
			{
				ExplodeSelectedTile(imageBox);
				return true;
			}

			return false;
		}
	}
}
