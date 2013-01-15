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

using System.IO;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public class FileArgumentHint : IArgumentHint
	{
		private FileInfo _fileInfo;

		public FileArgumentHint(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public FileArgumentHint(string filename) : this(new FileInfo(filename)) {}

		public ArgumentHintValue this[string key]
		{
			get
			{
				switch (key)
				{
					case "FILENAME":
						return new ArgumentHintValue(_fileInfo.FullName);
					case "DIRECTORY":
						return new ArgumentHintValue(_fileInfo.DirectoryName);
					case "FILENAMEONLY":
						return new ArgumentHintValue(_fileInfo.Name);
					case "EXTENSIONONLY":
						return new ArgumentHintValue(_fileInfo.Extension);
				}
				return ArgumentHintValue.Empty;
			}
		}

		public void Dispose() {}
	}
}