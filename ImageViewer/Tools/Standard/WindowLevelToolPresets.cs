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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[ActionPlaceholder("auto", "windowlevel-dropdown/MenuWindowLevelPresetList", "Tools.Image.Manipulation.Lut.Presets")]
	[ActionPlaceholder("auto", "imageviewer-contextmenu/MenuWindowLevelPresets/MenuWindowLevelPresetList", "Tools.Image.Manipulation.Lut.Presets")]
	public partial class WindowLevelTool
	{
		private class PresetVoiLutActionContainer
		{
			private readonly WindowLevelTool _ownerTool;
			private readonly PresetVoiLut _preset;
			private readonly MenuAction _action;

			public PresetVoiLutActionContainer(WindowLevelTool ownerTool, string actionSite, PresetVoiLut preset, int index)
			{
				_ownerTool = ownerTool;
				_preset = preset;

				string actionId = String.Format("{1}:apply{0}", _preset.Operation.Name, typeof (WindowLevelTool).FullName);

				ActionPlaceholder actionPlaceholder = ownerTool.FindActionPlaceholder(actionSite);
				_action = actionPlaceholder.CreateMenuAction(actionId, string.Format("presetLut{0}", index), ClickActionFlags.None, ownerTool._resolver);
				_action.Label = _preset.Operation.Name;
				_action.KeyStroke = _preset.KeyStroke;
				_action.SetClickHandler(this.Apply);
			}

			public ClickAction Action
			{
				get { return _action; }
			}

			private void Apply()
			{
				ImageOperationApplicator applicator = new ImageOperationApplicator(_ownerTool.SelectedPresentationImage, _preset.Operation);
				UndoableCommand historyCommand = _ownerTool._toolBehavior.Behavior.SelectedImageWindowLevelPresetsTool ? applicator.ApplyToReferenceImage() : applicator.ApplyToAllImages();
				if (historyCommand != null)
				{
					historyCommand.Name = SR.CommandWindowLevelPreset;
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				}
			}
		}

		private readonly IResourceResolver _resolver = new ApplicationThemeResourceResolver(typeof (WindowLevelTool).Assembly);

		public ActionModelNode PresetDropDownMenuModel
		{
			get
			{
				ActionModelRoot root = new ActionModelRoot();
				root.InsertActions(CreateActions("windowlevel-dropdown").ToArray());
				if (root.ChildNodes.Count == 0)
				{
					ClickAction dummy = new ClickAction(typeof (WindowLevelTool).FullName + ":dummy",
					                                    new ActionPath("windowlevel-dropdown/dummy", _resolver),
					                                    ClickActionFlags.None, _resolver);
					dummy.Persistent = false;
					dummy.Label = SR.LabelNone;
					root.InsertAction(dummy);
				}
				return root;
			}
		}

		public override IActionSet Actions
		{
			get
			{
				IActionSet baseActions = base.Actions;
				return baseActions.Union(new ActionSet(CreateContextMenuActions()));
			}
		}

		private ActionPlaceholder FindActionPlaceholder(string actionSite)
		{
			// note that we use base.Actions here because this method is called by CreateActions and we don't want a stack overflow
			return ActionPlaceholder.GetPlaceholderAction(actionSite, base.Actions, "auto");
		}

		private IEnumerable<IAction> CreateContextMenuActions()
		{
			return CreateActions("imageviewer-contextmenu");
		}

		private List<PresetVoiLut> GetApplicablePresets()
		{
			List<PresetVoiLut> presets = new List<PresetVoiLut>();

			if (this.SelectedPresentationImage is IImageSopProvider)
			{
				//Only temporary until we enable the full functionality in the presets.
				PresetVoiLut autoPreset = new PresetVoiLut(new AutoPresetVoiLutOperationComponent());
				autoPreset.KeyStroke = XKeys.F2;
				presets.Add(autoPreset);

				ImageSop sop = ((IImageSopProvider) this.SelectedPresentationImage).ImageSop;

				PresetVoiLutGroupCollection groups = PresetVoiLutSettings.DefaultInstance.GetPresetGroups();
				PresetVoiLutGroup group = CollectionUtils.SelectFirst(groups,
				                                                      delegate(PresetVoiLutGroup testGroup) { return testGroup.AppliesTo(sop); });
				if (group != null)
				{
					foreach (PresetVoiLut preset in group.Clone().Presets)
					{
						if (preset.Operation.AppliesTo(this.SelectedPresentationImage))
							presets.Add(preset);
					}
				}

				presets.Sort(new PresetVoiLutComparer());
			}

			return presets;
		}

		private List<IAction> CreateActions(string actionSite)
		{
			List<IAction> actions = new List<IAction>();

			int i = 0;
			foreach (PresetVoiLut preset in GetApplicablePresets())
				actions.Add(new PresetVoiLutActionContainer(this, actionSite, preset, ++i).Action);

			return actions;
		}
	}
}