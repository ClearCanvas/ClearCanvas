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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Automation;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Layout;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MouseToolButton(XMouseButtons.Right, true)]
	[DefaultMouseToolButton(XMouseButtons.Middle)]
	//
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuWindowLevel", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuWindowLevel", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarWindowLevel", "Select", "PresetDropDownMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.W)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.WindowLevelToolSmall.png", "Icons.WindowLevelToolMedium.png", "Icons.WindowLevelToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Lut.WindowLevel")]
	//
	[KeyboardAction("incrementwindowwidth", "imageviewer-keyboard/ToolsStandardWindowLevel/IncrementWindowWidth", "IncrementWindowWidth", KeyStroke = XKeys.Right)]
	[KeyboardAction("decrementwindowwidth", "imageviewer-keyboard/ToolsStandardWindowLevel/DecrementWindowWidth", "DecrementWindowWidth", KeyStroke = XKeys.Left)]
	[KeyboardAction("incrementwindowcenter", "imageviewer-keyboard/ToolsStandardWindowLevel/IncrementWindowCenter", "IncrementWindowCenter", KeyStroke = XKeys.Up)]
	[KeyboardAction("decrementwindowcenter", "imageviewer-keyboard/ToolsStandardWindowLevel/DecrementWindowCenter", "DecrementWindowCenter", KeyStroke = XKeys.Down)]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class WindowLevelTool : MouseImageViewerTool, ILookupTable
	{
		private readonly VoiLutImageOperation _operation;
		private MemorableUndoableCommand _memorableCommand;
		private ImageOperationApplicator _applicator;
		private ToolModalityBehaviorHelper _toolBehavior;

		public WindowLevelTool()
			: base(SR.TooltipWindowLevel)
		{
			CursorToken = new CursorToken("Icons.WindowLevelToolSmall.png", GetType().Assembly);
			_operation = new VoiLutImageOperation(Apply);
		}

		public override void Initialize()
		{
			base.Initialize();

			_toolBehavior = new ToolModalityBehaviorHelper(ImageViewer);
		}

		protected override void Dispose(bool disposing)
		{
			_memorableCommand = null;
			_applicator = null;
			base.Dispose(disposing);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private IVoiLutManager GetSelectedImageVoiLutManager()
		{
			return _operation.GetOriginator(SelectedPresentationImage) as IVoiLutManager;
		}

		private bool CanWindowLevel()
		{
			IVoiLutManager manager = GetSelectedImageVoiLutManager();
			return manager != null && manager.Enabled && manager.VoiLut is IVoiLutLinear;
		}

		private void CaptureBeginState()
		{
			if (!CanWindowLevel())
				return;

			IVoiLutManager originator = GetSelectedImageVoiLutManager();
			_applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			_memorableCommand = new MemorableUndoableCommand(originator);
			_memorableCommand.BeginState = originator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!CanWindowLevel() || _memorableCommand == null)
				return;

			if (SelectedVoiLutProvider.VoiLutManager.VoiLut is IBasicVoiLutLinear)
			{
				_memorableCommand.EndState = GetSelectedImageVoiLutManager().CreateMemento();
				UndoableCommand applicatorCommand = _toolBehavior.Behavior.SelectedImageWindowLevelTool ? null : _applicator.ApplyToLinkedImages();
				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(SelectedPresentationImage);

				if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
					historyCommand.Enqueue(_memorableCommand);
				if (applicatorCommand != null)
					historyCommand.Enqueue(applicatorCommand);

				if (historyCommand.Count > 0)
				{
					historyCommand.Name = SR.CommandWindowLevel;
					Context.Viewer.CommandHistory.AddCommand(historyCommand);
				}
			}
		}

		private void IncrementWindowWidth()
		{
			IncrementWindowWithUndo(CurrentSensitivity, 0);
		}

		private void DecrementWindowWidth()
		{
			IncrementWindowWithUndo(-CurrentSensitivity, 0);
		}

		private void IncrementWindowCenter()
		{
			IncrementWindowWithUndo(0, CurrentSensitivity);
		}

		private void DecrementWindowCenter()
		{
			IncrementWindowWithUndo(0, -CurrentSensitivity);
		}

		private void IncrementWindow(double windowIncrement, double levelIncrement)
		{
			if (!CanWindowLevel())
				return;

			IVoiLutManager manager = SelectedVoiLutProvider.VoiLutManager;

			IVoiLutLinear linearLut = manager.VoiLut as IVoiLutLinear;
			IBasicVoiLutLinear standardLut = linearLut as IBasicVoiLutLinear;
			if (standardLut == null)
			{
				BasicVoiLutLinear installLut = new BasicVoiLutLinear(linearLut.WindowWidth, linearLut.WindowCenter);
				manager.InstallVoiLut(installLut);
			}

			standardLut = manager.VoiLut as IBasicVoiLutLinear;
			standardLut.WindowWidth += windowIncrement;
			standardLut.WindowCenter += levelIncrement;
			SelectedVoiLutProvider.Draw();
		}

		private void SetWindow(double windowWidth, double windowCenter, out IBasicVoiLutLinear voiLut)
		{
			voiLut = null;
			if (!CanWindowLevel())
				return;

			IVoiLutManager manager = SelectedVoiLutProvider.VoiLutManager;
			var linearLut = manager.VoiLut as IVoiLutLinear;
			var standardLut = linearLut as IBasicVoiLutLinear;
			if (standardLut == null)
			{
				standardLut = new BasicVoiLutLinear(windowWidth, windowCenter);
				manager.InstallVoiLut(standardLut);
			}
			else
			{
				standardLut.WindowWidth = windowWidth;
				standardLut.WindowCenter = windowCenter;
			}

			voiLut = standardLut;
			SelectedVoiLutProvider.Draw();
		}

		private void IncrementWindowWithUndo(double windowIncrement, double levelIncrement)
		{
			CaptureBeginState();
			IncrementWindow(windowIncrement, levelIncrement);
			CaptureEndState();
		}

		private void SetWindowWithUndo(double windowWidth, double windowCenter, out IBasicVoiLutLinear voiLut)
		{
			CaptureBeginState();
			SetWindow(windowWidth, windowCenter, out voiLut);
			CaptureEndState();
		}

		private void Apply(IPresentationImage image)
		{
			IVoiLutLinear selectedLut = (IVoiLutLinear) SelectedVoiLutProvider.VoiLutManager.VoiLut;

			IVoiLutProvider provider = ((IVoiLutProvider) image);
			if (!(provider.VoiLutManager.VoiLut is IBasicVoiLutLinear))
			{
				BasicVoiLutLinear installLut = new BasicVoiLutLinear(selectedLut.WindowWidth, selectedLut.WindowCenter);
				provider.VoiLutManager.InstallVoiLut(installLut);
			}

			IBasicVoiLutLinear lut = (IBasicVoiLutLinear) provider.VoiLutManager.VoiLut;
			lut.WindowWidth = selectedLut.WindowWidth;
			lut.WindowCenter = selectedLut.WindowCenter;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			CaptureBeginState();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			double sensitivity = CurrentSensitivity;
			IncrementWindow(DeltaX*sensitivity, DeltaY*sensitivity);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			CaptureEndState();
		}

		#region ILookupTable Members

		IVoiLut ILookupTable.SetWindowLevel(double windowWidth, double windowCentre)
		{
			IBasicVoiLutLinear voiLut;
			SetWindowWithUndo(windowWidth, windowCentre, out voiLut);
			return voiLut;
		}

		IVoiLut ILookupTable.ApplyPreset(string name)
		{
			//Use the exact same path the user would take to execute it.
			var preset = GetApplicablePresets().First(p => p.Operation.Name == name);
			new PresetVoiLutActionContainer(this, "imageviewer-contextmenu", preset, 0).Action.Click();
			return GetSelectedImageVoiLutManager().VoiLut;
		}

		IVoiLut ILookupTable.GetLookupTableAt(RectangularGrid.Location tileLocation)
		{
			var voiLutManager = GetSelectedImageVoiLutManager();
			if (voiLutManager == null)
				throw new InvalidOperationException("Selected image can't have a voi lut");

			return voiLutManager.VoiLut;
		}

		#endregion
	}
}