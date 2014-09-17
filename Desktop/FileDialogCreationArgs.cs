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
using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Describes a file dialog extension filter.
	/// </summary>
	public class FileExtensionFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="description"></param>
		public FileExtensionFilter(string filter, [param : Localizable(true)] string description)
		{
			Filter = filter;
			Description = description;
		}

		/// <summary>
		/// Gets or sets the value of the filter, which must be a wildcard expression (e.g. *.txt).
		/// </summary>
		public string Filter { get; set; }

		/// <summary>
		/// Gets or sets the value displayed for the filter, e.g. Text files (*.txt).
		/// </summary>
		[Localizable(true)]
		public string Description { get; set; }
	}

	/// <summary>
	/// Holds parameters that initialize the display of a common file dialog.
	/// </summary>
	public class FileDialogCreationArgs
	{
		private readonly List<FileExtensionFilter> _filters;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="directory"></param>
		/// <param name="filename"></param>
		/// <param name="fileExtension"></param>
		/// <param name="filters"></param>
		public FileDialogCreationArgs(string filename, string directory, string fileExtension, IEnumerable<FileExtensionFilter> filters)
		{
			Directory = directory;
			FileName = filename;
			FileExtension = fileExtension;
			PreventSaveToInstallPath = true; // default to true, since this is good policy in the majority of use cases
			_filters = new List<FileExtensionFilter>(filters);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filename"></param>
		public FileDialogCreationArgs(string filename)
			: this(filename, null, null, new FileExtensionFilter[] {}) {}

		/// <summary>
		/// Constructor
		/// </summary>
		public FileDialogCreationArgs()
			: this(null, null, null, new FileExtensionFilter[] {}) {}

		/// <summary>
		/// Gets or sets the default extension to append to the filename, if not specified by user.
		/// </summary>
		public string FileExtension { get; set; }

		/// <summary>
		/// Gets or sets the initial value of the file name.
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the initial directory.
		/// </summary>
		public string Directory { get; set; }

		/// <summary>
		/// Gets or sets the title of the file dialog.
		/// </summary>
		[Localizable(true)]
		public string Title { get; set; }

		/// <summary>
		/// For an Open File dialog, gets or sets a value indicating whether the dialog allows multiple files to be selected.
		/// </summary>
		/// <remarks>
		/// This property is ignored for a Save File dialog.
		/// </remarks>
		public bool MultiSelect { get; set; }

		/// <summary>
		/// Gets the list of file extension filters.
		/// </summary>
		public List<FileExtensionFilter> Filters
		{
			get { return _filters; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to prevent saving files to the install directory or any of its child directories.
		/// </summary>
		public bool PreventSaveToInstallPath { get; set; }
	}
}