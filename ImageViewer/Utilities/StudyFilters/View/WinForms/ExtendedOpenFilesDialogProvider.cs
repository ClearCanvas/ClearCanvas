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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	[ExtensionOf(typeof (ExtendedOpenFilesDialog))]
	public class ExtendedOpenFilesDialogProvider : IExtendedOpenFilesDialogProvider
	{
		public IEnumerable<string> GetFiles(FileDialogCreationArgs args)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			PrepareFileDialog(dialog, args);
			dialog.CheckFileExists = true;
			dialog.ShowReadOnly = false;
			dialog.Multiselect = true;

			DialogResult dr = dialog.ShowDialog();
			if (dr == DialogResult.OK)
				return dialog.FileNames;

			return null;
		}

		private static void PrepareFileDialog(FileDialog dialog, FileDialogCreationArgs args)
		{
			dialog.AddExtension = !string.IsNullOrEmpty(args.FileExtension);
			dialog.DefaultExt = args.FileExtension;
			dialog.FileName = args.FileName;
			dialog.InitialDirectory = args.Directory;
			dialog.RestoreDirectory = true;
			dialog.Title = args.Title;

			dialog.Filter = StringUtilities.Combine(args.Filters, "|",
			                                        delegate(FileExtensionFilter f) { return f.Description + "|" + f.Filter; });
		}
	}
}