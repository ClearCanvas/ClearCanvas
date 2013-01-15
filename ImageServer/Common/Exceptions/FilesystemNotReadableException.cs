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

namespace ClearCanvas.ImageServer.Common.Exceptions
{
	/// <summary>
	/// Represents the exception thrown when the study is online but the filesystem is missing or not writable.
	/// </summary>
	public class FilesystemNotReadableException : SopInstanceProcessingException
	{

		public string Path { get; set; }
		public string Reason { get; set; }

		public FilesystemNotReadableException()
			: base("Study is online but the filesystem is no longer readable.")
		{
		}

		public FilesystemNotReadableException(string path)
			: base(String.Format("Filesystem is not readable: {0}", path))
		{
			Path = path;
		}

		public override string ToString()
		{
			return string.Format("{0} : {1}", Path, Reason);
		}
	}
}
