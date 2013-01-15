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

#if UNIT_TESTS

using System.IO;

namespace ClearCanvas.Common.Utilities.Tests
{
	/// <summary>
	/// A read-writeable <see cref="FileStream"/> using a temporary file that will be deleted upon disposal.
	/// </summary>
	public sealed class TempFileStream : FileStream
	{
		public TempFileStream()
			: base(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite) {}

		protected override void Dispose(bool disposing)
		{
			var path = disposing ? Name : null;

			base.Dispose(disposing);

			if (!string.IsNullOrEmpty(path) && File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}
}

#endif