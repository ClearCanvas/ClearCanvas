#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.IO
{
	/// <summary>
	/// Represents a mechanism for opening a <see cref="Stream"/> to a DICOM file or dataset.
	/// </summary>
	public abstract class DicomStreamOpener
	{
		/// <summary>
		/// Opens a new <see cref="Stream"/> instance to the DICOM file or dataset.
		/// </summary>
		public abstract Stream Open();

		/// <summary>
		/// Creates a <see cref="DicomStreamOpener"/> for opening a <see cref="FileStream"/> to a DICOM file or dataset on disk.
		/// </summary>
		/// <param name="filename">The filename of a DICOM file or dataset on disk.</param>
		/// <returns>A <see cref="DicomStreamOpener"/> for opening the specified <paramref name="filename"/>.</returns>
		/// <exception cref="NullReferenceException">Thrown if <paramref name="filename"/> is null or empty.</exception>
		public static DicomStreamOpener Create(string filename)
		{
			Platform.CheckForEmptyString(filename, "filename");
			return new FileStreamOpener(filename);
		}

		/// <summary>
		/// Creates a <see cref="DicomStreamOpener"/> encapsulating a delegate for opening a <see cref="Stream"/> to a DICOM file or dataset.
		/// </summary>
		/// <param name="delegate">The delegate for opening a <see cref="Stream"/> to a DICOM file or dataset.</param>
		/// <returns>A <see cref="DicomStreamOpener"/> encapsulating the specified <paramref name="delegate"/>.</returns>
		/// <exception cref="NullReferenceException">Thrown if <paramref name="delegate"/> is null.</exception>
		public static DicomStreamOpener Create(Func<Stream> @delegate)
		{
			Platform.CheckForNullReference(@delegate, "delegate");
			return new DelegateStreamOpener(@delegate);
		}

		/// <summary>
		/// Creates a <see cref="DicomStreamOpener"/> encapsulating a delegate for opening a <see cref="Stream"/> to a DICOM file or dataset.
		/// </summary>
		/// <param name="delegate">The delegate for opening a <see cref="Stream"/> to a DICOM file or dataset.</param>
		/// <returns>A <see cref="DicomStreamOpener"/> encapsulating the given delegate.</returns>
		public static explicit operator DicomStreamOpener(Func<Stream> @delegate)
		{
			return Create(@delegate);
		}

		private class FileStreamOpener : DicomStreamOpener
		{
			private readonly string _filename;

			public FileStreamOpener(string filename)
			{
				_filename = filename;
			}

			public override Stream Open()
			{
				return File.OpenRead(_filename);
			}
		}

		private class DelegateStreamOpener : DicomStreamOpener
		{
			private readonly Func<Stream> _delegate;

			public DelegateStreamOpener(Func<Stream> @delegate)
			{
				_delegate = @delegate;
			}

			public override Stream Open()
			{
				return _delegate.Invoke();
			}
		}
	}
}