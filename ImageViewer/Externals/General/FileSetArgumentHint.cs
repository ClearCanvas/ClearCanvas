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

namespace ClearCanvas.ImageViewer.Externals.General
{
	public class FileSetArgumentHint : IArgumentHint
	{
		private IList<FileInfo> _fileInfos;

		public FileSetArgumentHint(IEnumerable<string> filenames) : this(EnumerateFiles(filenames)) {}

		public FileSetArgumentHint(IEnumerable<FileInfo> fileInfos)
		{
			_fileInfos = new List<FileInfo>(fileInfos).AsReadOnly();
		}

		public ArgumentHintValue this[string key]
		{
			get
			{
				Converter<FileInfo, string> converter;
				switch (key)
				{
					case "FILENAME":
						converter = delegate(FileInfo fileinfo) { return fileinfo.FullName; };
						break;
					case "DIRECTORY":
						converter = delegate(FileInfo fileinfo) { return fileinfo.DirectoryName; };
						break;
					case "FILENAMEONLY":
						converter = delegate(FileInfo fileinfo) { return fileinfo.Name; };
						break;
					case "EXTENSIONONLY":
						converter = delegate(FileInfo fileinfo) { return fileinfo.Extension; };
						break;
					default:
						return ArgumentHintValue.Empty;
				}

				List<string> list = new List<string>();
				foreach (FileInfo fileInfo in _fileInfos)
				{
					string result = converter(fileInfo);
					if (!list.Contains(result))
						list.Add(result);
				}
				return new ArgumentHintValue(list.ToArray());
			}
		}

		public void Dispose() {}

		private static IEnumerable<FileInfo> EnumerateFiles(IEnumerable<string> filenames)
		{
			foreach (string filename in filenames)
				yield return new FileInfo(filename);
		}
	}
}