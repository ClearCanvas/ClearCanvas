#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS
//
// The ClearCanvas RIS/PACS is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of 
// the License, or (at your option) any later version.
//
// ClearCanvas RIS/PACS is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with ClearCanvas RIS/PACS.  If not, 
// see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Controls.WinForms
{
	[Serializable]
	public class PathAccessException : UnauthorizedAccessException
	{
		private static readonly string _defaultMessage = new UnauthorizedAccessException().Message;
		private readonly string _path = null;

		public PathAccessException() {}

		public PathAccessException(string path)
		{
			_path = path;
		}

		public PathAccessException(string message, string path)
			: base(message)
		{
			_path = path;
		}

		public PathAccessException(string path, Exception inner)
			: base(_defaultMessage, inner)
		{
			_path = path;
		}

		public PathAccessException(string message, string path, Exception inner)
			: base(message, inner)
		{
			_path = path;
		}

		protected PathAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}

		public string Path
		{
			get { return _path ?? string.Empty; }
		}
	}
}