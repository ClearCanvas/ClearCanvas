﻿#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

#if UNIT_TESTS

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Volumes.Tests
{
	[MenuAction("asyncLoadException", "global-menus/&Debug/Volume Cache - Throw Async Exception", "ThrowAsyncVolumeLoadException")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class VolumeCacheTestTool : ImageViewerTool
	{
		public void ThrowAsyncVolumeLoadException()
		{
			VolumeCache.ThrowAsyncVolumeLoadException = true;
		}
	}
}

#endif