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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuStack", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarStack", "Select", "SortMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.S)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.StackToolSmall.png", "Icons.StackToolMedium.png", "Icons.StackToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Stacking.Standard")]

	[MouseWheelHandler(ModifierFlags.None)]
	[MouseToolButton(XMouseButtons.Left, true)]

	[KeyboardAction("stackup", "imageviewer-keyboard/ToolsStandardStack/StackUp", "StackUp", KeyStroke = XKeys.PageUp)]
	[KeyboardAction("stackdown", "imageviewer-keyboard/ToolsStandardStack/StackDown", "StackDown", KeyStroke = XKeys.PageDown)]
	[KeyboardAction("jumptobeginning", "imageviewer-keyboard/ToolsStandardStack/JumpToBeginning", "JumpToBeginning", KeyStroke = XKeys.Home)]
	[KeyboardAction("jumptoend", "imageviewer-keyboard/ToolsStandardStack/JumpToEnd", "JumpToEnd", KeyStroke = XKeys.End)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public partial class StackTool : MouseImageViewerTool
	{
		private MemorableUndoableCommand _memorableCommand;
		private int _initialPresentationImageIndex;
		private IImageBox _currentImageBox;
        private SimpleActionModel _sortMenuModel;

		public StackTool()
			: base(SR.TooltipStack)
		{
			CursorToken = new CursorToken("Icons.StackToolSmall.png", GetType().Assembly);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public ActionModelNode SortMenuModel
		{
			get
			{
                if (_sortMenuModel == null)
                {
                    _sortMenuModel = new SimpleActionModel(new ApplicationThemeResourceResolver(GetType().Assembly));
                    foreach (var item in ImageComparerList.Items)
                    {
                        var itemVar = item;
                        var action = _sortMenuModel.AddAction(itemVar.Name, itemVar.Description, null, itemVar.Description, () => Sort(itemVar));
                        action.Checked = GetSortMenuItemCheckState(itemVar);
                    }
                }

			    return _sortMenuModel;
			}
		}

        private bool GetSortMenuItemCheckState(ImageComparerList.Item item)
		{
			return SelectedPresentationImage != null && SelectedPresentationImage.ParentDisplaySet != null &&
			       item.Comparer.Equals(SelectedPresentationImage.ParentDisplaySet.PresentationImages.SortComparer);
		}

        private void Sort(ImageComparerList.Item item)
		{
			IImageBox imageBox = ImageViewer.SelectedImageBox;
			IDisplaySet displaySet;
			if (imageBox == null || (displaySet = ImageViewer.SelectedImageBox.DisplaySet) == null)
				return;

			if (displaySet.PresentationImages.Count == 0)
				return;

			//try to keep the top-left image the same.
			IPresentationImage topLeftImage = imageBox.TopLeftPresentationImage;

			var command = new MemorableUndoableCommand(imageBox) {BeginState = imageBox.CreateMemento()};

            displaySet.PresentationImages.Sort(item.Comparer);
			imageBox.TopLeftPresentationImage = topLeftImage;
			imageBox.Draw();

			command.EndState = imageBox.CreateMemento();
			if (!command.BeginState.Equals(command.EndState))
			{
				var historyCommand = new DrawableUndoableCommand(imageBox) {Name = SR.CommandSortImages};
			    historyCommand.Enqueue(command);
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		private void CaptureBeginState(IImageBox imageBox)
		{
			_memorableCommand = new MemorableUndoableCommand(imageBox) {BeginState = imageBox.CreateMemento()};
			// Capture state before stack
		    _currentImageBox = imageBox;

			_initialPresentationImageIndex = imageBox.SelectedTile.PresentationImageIndex;
		}

		private bool CaptureEndState()
		{
			if (_memorableCommand == null || _currentImageBox == null)
			{
				_currentImageBox = null;
                return false;
			}

            bool commandAdded = false;

			// If nothing's changed then just return
			if (_initialPresentationImageIndex != _currentImageBox.SelectedTile.PresentationImageIndex)
			{
				// Capture state after stack
				_memorableCommand.EndState = _currentImageBox.CreateMemento();
				if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
				{
					var historyCommand = new DrawableUndoableCommand(_currentImageBox) {Name = SR.CommandStack};
				    historyCommand.Enqueue(_memorableCommand);
                    Context.Viewer.CommandHistory.AddCommand(historyCommand);
				    commandAdded = true;
				}
			}

			_memorableCommand = null;
			_currentImageBox = null;

		    return commandAdded;
		}

		private void JumpToBeginning()
		{
            if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

            IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = 0;
			if (CaptureEndState())
                imageBox.Draw();
		}

		private void JumpToEnd()
		{
            if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

            IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;

			if (imageBox.DisplaySet == null)
				return;

			CaptureBeginState(imageBox);
			imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count - 1;
            if (CaptureEndState())
                imageBox.Draw();
        }

		private void StackUp()
		{
            if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

            IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(-imageBox.Tiles.Count, imageBox);
		    CaptureEndState();
            //No draw - AdvanceImage has already done it.
        }

		private void StackDown()
		{
            if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

            IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			CaptureBeginState(imageBox);
			AdvanceImage(+imageBox.Tiles.Count, imageBox);
		    CaptureEndState();
            //No draw - AdvanceImage has already done it.
		}

		private static void AdvanceImage(int increment, IImageBox selectedImageBox)
		{
		    int prevTopLeftPresentationImageIndex = selectedImageBox.TopLeftPresentationImageIndex;
			selectedImageBox.TopLeftPresentationImageIndex += increment;

            if (selectedImageBox.TopLeftPresentationImageIndex != prevTopLeftPresentationImageIndex)
                selectedImageBox.Draw(); 
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Start(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			CaptureBeginState(mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Track(mouseInformation);

			if (mouseInformation.Tile == null)
				return false;

			if (DeltaY == 0)
				return true;

			int increment;

			if (DeltaY > 0)
				increment = 1;
			else
				increment = -1;

			AdvanceImage(increment, mouseInformation.Tile.ParentImageBox);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (this.SelectedPresentationImage == null)
				return false;

			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			if (this.SelectedPresentationImage == null)
				return;

			CaptureEndState();
		}

		public override void StartWheel()
		{
            if (Context.Viewer.SelectedTile == null)
				return;

			if (this.SelectedPresentationImage == null)
				return;

            IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
			if (imageBox == null)
				return;

			CaptureBeginState(imageBox);
		}

		protected override void WheelBack()
		{
			if (this.SelectedPresentationImage == null)
				return;

            AdvanceImage(1, Context.Viewer.SelectedTile.ParentImageBox);
		}

		protected override void WheelForward()
		{
			if (this.SelectedPresentationImage == null)
				return;

            AdvanceImage(-1, Context.Viewer.SelectedTile.ParentImageBox);
		}

		public override void StopWheel()
		{
			if (this.SelectedPresentationImage == null)
				return;

			CaptureEndState();
		}
    }

    #region Oto
    partial class StackTool : IStack
    {
        #region IViewerStackService Members

        void IStack.StackBy(int delta)
        {
            if (Context.Viewer.SelectedTile == null)
                throw new InvalidOperationException("No tile selected.");

            if (this.SelectedPresentationImage == null)
                throw new InvalidOperationException("No image selected.");

            IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
            CaptureBeginState(imageBox);
            AdvanceImage(delta, imageBox);
            //No draw - AdvanceImage has already done it.
            CaptureEndState();
        }

        void IStack.StackTo(int instanceNumber, int? frameNumber)
        {
            if (Context.Viewer.SelectedTile == null)
                throw new InvalidOperationException("No tile selected.");

            if (this.SelectedPresentationImage == null)
                throw new InvalidOperationException("No image selected.");

            var displaySet = Context.Viewer.SelectedPresentationImage.ParentDisplaySet;
            
            //First will throw if no such image exists.
            var image = (IPresentationImage)displaySet.PresentationImages.OfType<IImageSopProvider>().First(
                i => i.ImageSop.InstanceNumber == instanceNumber 
                        && (!frameNumber.HasValue || (i.ImageSop.NumberOfFrames > 1 && frameNumber.Value == i.Frame.FrameNumber)));

            IImageBox imageBox = Context.Viewer.SelectedTile.ParentImageBox;
            CaptureBeginState(imageBox);
            imageBox.TopLeftPresentationImage = image;
            if (!CaptureEndState())
                return; /// TODO (CR Dec 2011): Should we still select the top-left??

            imageBox.Draw();
            imageBox.Tiles[0].Select();
        }

        #endregion
    }
    #endregion
}
