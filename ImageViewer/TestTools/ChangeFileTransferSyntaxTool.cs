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

#if DEBUG

using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.Common;
using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class ChangeFileTransferSyntaxTool : Tool<ILocalImageExplorerToolContext>
	{
		public ChangeFileTransferSyntaxTool()
		{
		}

		public override IActionSet Actions
		{
			get
			{
				List<IAction> actions = new List<IAction>();
				IResourceResolver resolver = new ResourceResolver(typeof(ChangeFileTransferSyntaxTool).GetType().Assembly);

				actions.Add(CreateAction(TransferSyntax.ExplicitVrLittleEndian, resolver));
				actions.Add(CreateAction(TransferSyntax.ImplicitVrLittleEndian, resolver));

				foreach (IDicomCodecFactory factory in ClearCanvas.Dicom.Codec.DicomCodecRegistry.GetCodecFactories())
				{
					actions.Add(CreateAction(factory.CodecTransferSyntax, resolver));
				}

				return new ActionSet(actions);
			}
		}

		private IAction CreateAction(TransferSyntax syntax, IResourceResolver resolver)
		{
			ClickAction action = new ClickAction(syntax.UidString,
					new ActionPath("explorerlocal-contextmenu/Change Transfer Syntax/" + syntax.ToString(), resolver),
					ClickActionFlags.None, resolver);
			action.SetClickHandler(delegate { ChangeToSyntax(syntax); });
			action.Label = syntax.ToString();
			return action;
		}

		private void ChangeToSyntax(TransferSyntax syntax)
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

			foreach (string file in files)
			{
				try
				{
					DicomFile dicomFile = new DicomFile(file);
					dicomFile.Load();
					dicomFile.ChangeTransferSyntax(syntax);
					string sourceFileName = System.IO.Path.GetFileNameWithoutExtension(file);
					string fileName = System.IO.Path.Combine(result.FileName, sourceFileName);
					fileName += ".compressed.dcm";
					dicomFile.Save(fileName);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, Context.DesktopWindow);
				}
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
#endif