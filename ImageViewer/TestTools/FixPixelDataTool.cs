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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("extractEmbeddedOverlays", "explorerlocal-contextmenu/Fix Pixel Data", "Go")]
	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class FixPixelDataTool : Tool<ILocalImageExplorerToolContext>
	{
		public void Go()
		{
			string[] files = BuildFileList();
			var args = new SelectFolderDialogCreationArgs
			{
				Path = GetDirectoryOfFirstPath(),
				AllowCreateNewFolder = true,
				Prompt = "Select output folder"
			};

			var result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action != DialogBoxAction.Ok)
				return;

			try
			{
				foreach (string file in files)
				{
					DicomFile dicomFile = new DicomFile(file);
					dicomFile.Load();

					if (dicomFile.TransferSyntax.Encapsulated)
						continue;

					DicomAttribute attribute;
					if (!dicomFile.DataSet.TryGetAttribute(DicomTags.PixelData, out attribute))
						continue;

					new OverlayPlaneModuleIod(dicomFile.DataSet).ExtractEmbeddedOverlays();
					var rawPixelData = (byte[])attribute.Values;

					DicomPixelData pd = new DicomUncompressedPixelData(dicomFile);
					if (DicomUncompressedPixelData.ZeroUnusedBits(rawPixelData, pd.BitsAllocated, pd.BitsStored, pd.HighBit))
					{
						Platform.Log(LogLevel.Info, "Zeroed some unused bits.");
					}
					if (DicomUncompressedPixelData.RightAlign(rawPixelData, pd.BitsAllocated, pd.BitsStored, pd.HighBit))
					{
						var newHighBit = (ushort) (pd.HighBit - pd.LowBit);
						Platform.Log(LogLevel.Info, "Right aligned pixel data (High Bit: {0}->{1}).", pd.HighBit, newHighBit);

						pd.HighBit = newHighBit; //correct high bit after right-aligning.
						dicomFile.DataSet[DicomTags.HighBit].SetUInt16(0, newHighBit);
					}

					string sourceFileName = System.IO.Path.GetFileNameWithoutExtension(file);
					string fileName = System.IO.Path.Combine(result.FileName, sourceFileName);
					fileName += ".fixed-pixeldata.dcm";
					dicomFile.Save(fileName);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, Context.DesktopWindow);
			}
		}

		private string GetDirectoryOfFirstPath()
		{
			foreach (string path in Context.SelectedPaths)
				return System.IO.Path.GetDirectoryName(path);

			return null;
		}

		private string[] BuildFileList()
		{
			List<string> fileList = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
			}

			return fileList.ToArray();
		}
	}
}
