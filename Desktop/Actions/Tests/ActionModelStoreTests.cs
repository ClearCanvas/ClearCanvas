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

#if UNIT_TESTS
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions.Tests
{
	[TestFixture]
	public class ActionModelStoreTests
	{
		private const string MENUNAME = "imageviewer-contextmenu";

		private string[,] _actionSetup;
		private List<IAction> _allActions;

		public ActionModelStoreTests()
		{
		}

		private void CreateActionSet()
		{
			_allActions = new List<IAction>();

			for (int i = 0; i < _actionSetup.Length / 3; ++i)
			{ 
				string actionId = _actionSetup[i, 0];
				string actionPath = _actionSetup[i, 1];
				string groupHint = _actionSetup[i, 2];

				ClickAction action = new ClickAction(actionId, new ActionPath(actionPath, null), ClickActionFlags.None, null);
				action.GroupHint = new GroupHint(groupHint);
				action.Persistent = true;

				_allActions.Add(action);
			}
		}

		private void CreateDefaultXmlDocument()
		{
			XmlDocument document = new XmlDocument();

			XmlElement actionModels = document.CreateElement("action-models");
			document.AppendChild(actionModels);

			XmlElement actionModel = document.CreateElement("action-model");
			actionModels.AppendChild(actionModel);

			actionModel.SetAttribute("id", String.Format("{0}:{1}", this.GetType().FullName, MENUNAME));
			
			List<IAction> defaultActions = _allActions.FindAll(delegate(IAction action) { return action.ActionID.StartsWith("test") == false; });

			foreach (IAction action in defaultActions)
			{
				XmlElement actionElement = document.CreateElement("action");

				actionElement.SetAttribute("id", action.ActionID);
				actionElement.SetAttribute("path", action.Path.ToString());
				actionElement.SetAttribute("group-hint", action.GroupHint.Hint);

				actionModel.AppendChild(actionElement);
			}

			ActionModelSettings.Default.ActionModelsXml = document;
		}

		private void VerifyXmlDocument()
		{
			XmlDocument document = ActionModelSettings.Default.ActionModelsXml;
			XmlElement actionModel = (XmlElement)document.SelectSingleNode("//action-model");

			int i = 0;
			foreach (XmlElement xmlAction in actionModel.ChildNodes)
			{
				string id = xmlAction.GetAttribute("id");
				Assert.AreEqual(id, _allActions[i++].ActionID);
			}
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[TestFixtureTearDown]
		public void CleanUp()
		{ 
		}

		[Test]
		public void TestGroupHintAlgorithm()
		{
			//null
			GroupHint left = new GroupHint(null);
			Assert.AreEqual(String.Empty, left.Hint);

			// left = "", right=""
			GroupHint right = new GroupHint("");
			Assert.AreEqual(1, left.MatchScore(right));

			// left = "NonEmpty.Test", right=""
			left = new GroupHint("NonEmpty.Test");
			Assert.AreEqual(1, left.MatchScore(right));

			// left = "", right="NonEmpty.Test"
			left = right;
			right = new GroupHint("NonEmpty.Test");
			Assert.AreEqual(0, left.MatchScore(right));

			// left = "Tools.Standard.ImageManipulation.Lut.LutPresets", right="Tools.Standard.ImageManipulation.Lut"
			left = new GroupHint("Tools.Standard.ImageManipulation.Lut.LutPresets");
			right = new GroupHint("Tools.Standard.ImageManipulation.Lut");
			Assert.AreEqual(5, left.MatchScore(right));

			// left = "Tools.Standard.ImageManipulation.Lut", right="Tools.Standard.ImageManipulation.Lut.LutPresets"
			GroupHint temp = left;
			left = right;
			right = temp;
			Assert.AreEqual(-5, left.MatchScore(right));

			// left = "Tools.Standard.ImageManipulation.Lut", right="Tools.Standard.ImageManipulation.Lut"
			right = left;
			Assert.AreEqual(5, left.MatchScore(right));

			// left = "Tools.Standard.ImageManipulation.Lut.LutPresets", right="Tools.Standard.ImageManipulation.Lut.Auto"
			left = new GroupHint("Tools.Standard.ImageManipulation.Lut.LutPresets");
			right = new GroupHint("Tools.Standard.ImageManipulation.Lut.Auto");
			Assert.AreEqual(5, left.MatchScore(right));

			// left = "Tools.Standard.ImageManipulation.Lut.Auto", right="Tools.Standard.ImageManipulation.Lut.LutPresets"
			left = right;
			right = temp;
			Assert.AreEqual(-5, left.MatchScore(right));
			
			// left = "Tools.Standard.ImageManipulation.Lut", right="DisplaySets"
			right = new GroupHint("DisplaySets");
			Assert.AreEqual(0, left.MatchScore(right));

			// left = "DisplaySets", right="Tools.Standard.ImageManipulation.Lut"
			temp = left;
			left = right;
			right = temp;
			Assert.AreEqual(0, left.MatchScore(right));
		}

		[Test]
		public void TestActionModelStore()
		{
			_actionSetup = new string[,]
			{
				{ "ClearCanvas.ImageViewer.Tools.Standard.StackTool:activate", "imageviewer-contextmenu/MenuToolsStandardStack", "Tools.Image.Manipulation.Stacking.Standard" }, 
				{ "ClearCanvas.ImageViewer.Tools.Standard.WindowLevelTool:activate", "imageviewer-contextmenu/MenuToolsStandardWindowLevel", "Tools.Image.Manipulation.Lut.WindowLevel"}, 
				{ "ClearCanvas.ImageViewer.Tools.Standard.LutPresetTool:auto", "imageviewer-contextmenu/MenuToolsStandardLutPresets/auto", "Tools.Image.Manipulation.Lut.Presets" }, 
				{ "BogusDefault", "imageviewer-contextmenu/BogusDefault", "" }, 
				{ "ClearCanvas.ImageViewer.Tools.Standard.PanTool:activate", "imageviewer-contextmenu/MenuToolsStandardPan", "Tools.Image.Manipulation.Pan" },
				{ "test1", "imageviewer-contextmenu/Tools.Image.Manipulation.SomethingNew", "Tools.Image.Manipulation.SomethingNew" },
                { "ClearCanvas.ImageViewer.Tools.Standard.ZoomTool:activate", "imageviewer-contextmenu/MenuToolsStandardZoom", "Tools.Image.Manipulation.Zoom" } ,
				{ "ClearCanvas.ImageViewer.Tools.Standard.ProbeTool:activate", "imageviewer-contextmenu/MenuToolsStandardProbe", "Tools.Image.Interrogation.Probe" },
				{ "test2", "imageviewer-contextmenu/Tools.Image.SomethingNew", "Tools.Image.SomethingNew" },
				{ "ClearCanvas.ImageViewer.Tools.Volume.ZoomVolumeTool:activate", "imageviewer-contextmenu/Zoom Volume", "Tools.VolumeImage.Manipulation.Zoom" },
				{ "ClearCanvas.ImageViewer.Tools.Volume.RotateVolumeTool:activate", "imageviewer-contextmenu/Rotate Volume", "Tools.VolumeImage.Manipulation.Rotate"},
				{ "ClearCanvas.ImageViewer.Tools.Measurement.RulerTool:activate", "imageviewer-contextmenu/ToolsMeasurementRuler", "Tools.Measurement.Basic.ROI.Linear" },
				{ "ClearCanvas.ImageViewer.DefaultInsertionPoint", "imageviewer-contextmenu/DefaultInsertionPoint", "" },
				{ "test3", "imageviewer-contextmenu/DefaultInsertionPointTest", "" },
				{ "test4", "imageviewer-contextmenu/DefaultInsertionPointTest2", "Something.Completely.New" },
				{ "ClearCanvas.ImageViewer.Layout.Basic.ContextMenuLayoutTool:display0", "imageviewer-contextmenu/DisplaySets", "DisplaySets"},
				{ "test5", "imageviewer-contextmenu/DisplaySets.DisplaySet1", "DisplaySets" },
				{ "test6", "imageviewer-contextmenu/DisplaySets.DisplaySet2", "DisplaySets" },
				{ "test7", "imageviewer-contextmenu/DisplaySets.DisplaySet3", "DisplaySets.DisplaySet3" }
			};

			CreateActionSet();

			CreateDefaultXmlDocument();

			ActionSet allActions = new ActionSet(_allActions);

			ActionModelSettings.Default.BuildAndSynchronize(this.GetType().FullName, MENUNAME, allActions);

			VerifyXmlDocument();
		}
	}
}

#endif