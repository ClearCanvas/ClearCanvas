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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Contains the results of a common file dialog operation.
	/// </summary>
	public class FileDialogResult
	{
		private readonly string[] _filenames;
		private readonly DialogBoxAction _action;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="filename"></param>
		public FileDialogResult(DialogBoxAction action, string filename)
		{
			_action = action;
			_filenames = new[]{filename};
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="action"></param>
		/// <param name="filenames"></param>
		public FileDialogResult(DialogBoxAction action, string[] filenames)
		{
			_action = action;
			_filenames = filenames;
		}


		/// <summary>
		/// Gets the filename.
		/// </summary>
		public string FileName
		{
			get { return CollectionUtils.FirstElement(_filenames); }
		}

		/// <summary>
		/// Gets the filenames (if multi-select was enabled).
		/// </summary>
		public string[] FileNames
		{
			get { return _filenames; }
		}

		/// <summary>
		/// Gets the result of the file dialog.
		/// </summary>
		public DialogBoxAction Action
		{
			get { return _action; }
		}
	
	}
}
