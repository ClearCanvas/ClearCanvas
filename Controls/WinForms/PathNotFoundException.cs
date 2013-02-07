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
using System.Runtime.Serialization;

namespace ClearCanvas.Controls.WinForms
{
	[Serializable]
	public class PathNotFoundException : DirectoryNotFoundException
	{
		private static readonly string _defaultMessage = new DirectoryNotFoundException().Message;
		private readonly string _path = null;

		public PathNotFoundException() : base() {}

		public PathNotFoundException(string path)
		{
			_path = path;
		}

		public PathNotFoundException(string message, string path) : base(message)
		{
			_path = path;
		}

		public PathNotFoundException(string path, Exception inner) : base(_defaultMessage, inner)
		{
			_path = path;
		}

		public PathNotFoundException(string message, string path, Exception inner) : base(message, inner)
		{
			_path = path;
		}

		protected PathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public string Path
		{
			get { return _path ?? string.Empty; }
		}
	}
}