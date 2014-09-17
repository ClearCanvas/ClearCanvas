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
using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Holds parameters that initialize the display of a folder browser dialog.
	/// </summary>
	public class SelectFolderDialogCreationArgs
	{
		private string _path;
		private string _prompt;
		private bool _allowCreateNewFolder;

		/// <summary>
		/// Constructs an object holding parameters for the display of a folder browser dialog.
		/// </summary>
		public SelectFolderDialogCreationArgs()
		{
			_path = Environment.CurrentDirectory;
			_allowCreateNewFolder = true;
		}

		/// <summary>
		/// Constructs an object holding parameters for the display of a folder browser dialog.
		/// </summary>
		/// <param name="path"></param>
		public SelectFolderDialogCreationArgs(string path)
		{
			_path = path;
			_allowCreateNewFolder = true;
		}

		/// <summary>
		/// Gets or sets the path that the folder browser dialog will show initially.
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		/// <summary>
		/// Gets or sets the prompt to the user shown on the dialog.
		/// </summary>
		[Localizable(true)]
		public string Prompt
		{
			get { return _prompt; }
			set { _prompt = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if the creation of a new folder should be allowed by the dialog.
		/// </summary>
		public bool AllowCreateNewFolder
		{
			get { return _allowCreateNewFolder; }
			set { _allowCreateNewFolder = value; }
		}
	}
}