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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tests;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[MenuAction("setBase", "global-menus/MenuDebug/MenuFusion/Base Display Set", "SetBaseDisplaySet")]
	[MenuAction("setOverlay", "global-menus/MenuDebug/MenuFusion/Overlay Display Set", "SetOverlayDisplaySet")]
	[MenuAction("setFusion", "global-menus/MenuDebug/MenuFusion/Fusion Display Set", "SetFusionDisplaySet")]
	[MenuAction("assertFusion", "global-menus/MenuDebug/MenuFusion/Assert Fusion Results", "AssertFusionResults")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class TestDisplaySetsTool : ImageViewerTool
	{
		private TestDisplaySetGenerator _testDisplaySetGenerator;
		private IList<ISopDataSource> _baseSopDataSources;
		private IList<ISopDataSource> _overlaySopDataSources;

		private TestDisplaySetGenerator TestDisplaySetGenerator
		{
			get
			{
				if (_testDisplaySetGenerator == null)
				{
					_baseSopDataSources = TestDataFunction.Threed.CreateSops(true, Modality.CT, new Vector3D(0.8f, 0.8f, 0.8f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit);
					_overlaySopDataSources = TestDataFunction.Threed.CreateSops(true, Modality.PT, new Vector3D(1.0f, 1.0f, 1.0f), Vector3D.zUnit, Vector3D.xUnit, Vector3D.yUnit);
					_testDisplaySetGenerator = new TestDisplaySetGenerator(_baseSopDataSources, _overlaySopDataSources);
				}
				return _testDisplaySetGenerator;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_testDisplaySetGenerator != null)
				{
					_testDisplaySetGenerator.Dispose();
					_testDisplaySetGenerator = null;
				}

				if (_baseSopDataSources != null)
				{
					DisposeAll(_baseSopDataSources);
					_baseSopDataSources = null;
				}

				if (_overlaySopDataSources != null)
				{
					DisposeAll(_overlaySopDataSources);
					_overlaySopDataSources = null;
				}
			}
			base.Dispose(disposing);
		}

		public void SetBaseDisplaySet()
		{
			SetDisplaySet(TestDisplaySetGenerator.CreateBaseDisplaySet());
		}

		public void SetOverlayDisplaySet()
		{
			SetDisplaySet(TestDisplaySetGenerator.CreateOverlayDisplaySet());
		}

		public void SetFusionDisplaySet()
		{
			SetDisplaySet(TestDisplaySetGenerator.CreateFusionDisplaySet());
		}

		public void AssertFusionResults()
		{
			try
			{
				using (var outputStream = File.CreateText(string.Format("{0}.AssertFusionResults.csv", this.GetType().FullName)))
				{
					using (var referenceDisplaySet = TestDisplaySetGenerator.CreateBaseDisplaySet())
					{
						using (var testDisplaySet = TestDisplaySetGenerator.CreateFusionDisplaySet())
						{
							int index = 0;
							foreach (var testImage in testDisplaySet.PresentationImages)
							{
								var colorMapProvider = (IColorMapProvider) testImage;
								colorMapProvider.ColorMapManager.InstallColorMap("Grayscale");

								var layerOpacityProvider = (ILayerOpacityProvider) testImage;
								layerOpacityProvider.LayerOpacityManager.Thresholding = false;
								layerOpacityProvider.LayerOpacityManager.Opacity = 0.5f;

								Bitmap diff;
								var referenceImage = referenceDisplaySet.PresentationImages[index];
								var result = ImageDiff.Compare(ImageDiffAlgorithm.Euclidian, referenceImage, testImage, out diff);
								outputStream.WriteLine("{0}, {1:f4}", index, result);
								diff.Save(index + ".png");
								diff.Dispose();

								++index;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, "Fusion test assertion fail.", this.Context.DesktopWindow);
			}
		}

		private void SetDisplaySet(IDisplaySet displaySet)
		{
			if (ImageViewer == null || ImageViewer.PhysicalWorkspace.SelectedImageBox == null)
				return;
			if (ImageViewer.PhysicalWorkspace.SelectedImageBox.DisplaySetLocked)
				return;
			var oldDisplaySet = ImageViewer.PhysicalWorkspace.SelectedImageBox.DisplaySet;
			ImageViewer.PhysicalWorkspace.SelectedImageBox.DisplaySet = displaySet;
			if (oldDisplaySet != null)
				oldDisplaySet.Dispose();
			ImageViewer.PhysicalWorkspace.SelectedImageBox.Draw();
		}

		private static void DisposeAll<T>(IEnumerable<T> list) where T : IDisposable
		{
			foreach (var item in list)
				item.Dispose();
		}
	}
}

#endif