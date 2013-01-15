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
using System.IO;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal static class FileUtilities
	{
		public static bool HasExtension(string fileName, string[] allowedExtensions)
		{
			if (String.IsNullOrEmpty(fileName))
				return false;

			string extension = Path.GetExtension(fileName);
			if (String.IsNullOrEmpty(extension))
				return (allowedExtensions.Length == 0);

			extension = extension.Replace(".", "").Trim();

			return CollectionUtils.Contains(
				allowedExtensions,
				delegate(string test)
					{
						string testExtension = test.Replace(".", "").Trim();
						return String.Compare(extension, testExtension, true) == 0;
					});
		}

		public static string CorrectFileNameExtension(string filePath, string[] allowedExtensions)
		{
			if (String.IsNullOrEmpty(Path.GetFileName(filePath) ?? ""))
				return "";

			if (HasExtension(filePath, allowedExtensions) || allowedExtensions.Length == 0)
				return filePath;

			return String.Format("{0}.{1}", filePath, allowedExtensions[0]);
		}
	}
}
