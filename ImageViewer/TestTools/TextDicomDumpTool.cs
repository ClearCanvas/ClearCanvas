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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.Desktop.Actions;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("go", "explorerlocal-contextmenu/Dump Files (Text)", "Go")]
	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class TextDicomDumpTool : Tool<ILocalImageExplorerToolContext>
	{
		public void Go()
		{
			var result = base.Context.DesktopWindow.ShowSaveFileDialogBox(new FileDialogCreationArgs("dump.txt"));
			if (result.Action != DialogBoxAction.Ok)
				return;

			FileInfo info = new FileInfo(result.FileName);

			using (var writeStream = info.OpenWrite())
			{
				using (var writer = new StreamWriter(writeStream))
				{
					foreach(var path in base.Context.SelectedPaths)
					{
						FileProcessor.Process(path, "*.*",
							delegate(string file)
								{
									try
									{
										if (Directory.Exists(file))
											return;

										var dicomFile = new DicomFile(file);
										dicomFile.Load(DicomReadOptions.Default |
										               DicomReadOptions.DoNotStorePixelDataInDataSet);
										writer.WriteLine(dicomFile.Dump(String.Empty, DicomDumpOptions.None));
										writer.WriteLine();
									}
									catch (Exception e)
									{
										writer.WriteLine("Failed: {0}\n{1}", file, e);
									}
								}, true);
					}
				}
			}
		}
	}
}
