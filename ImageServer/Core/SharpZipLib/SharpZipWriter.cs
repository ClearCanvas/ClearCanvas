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
using ClearCanvas.ImageServer.Common;
using ICSharpCode.SharpZipLib.Zip;

namespace ClearCanvas.ImageServer.Core.SharpZipLib
{
	internal class SharpZipWriter : IZipServiceWriter
	{
		#region Internal Classes

		private class FileStreamDataSource : IStaticDataSource
		{
			private readonly Stream _inputStream;

			public FileStreamDataSource(Stream inputStream)
			{
				_inputStream = inputStream;
			}

			public Stream GetSource()
			{
				return _inputStream;
			}
		}

		#endregion Internal Classes

		#region Private Members

		private ZipFile _zipFile;

		#endregion Private Members

		#region Public Properties

		public bool ForceCompress { get; set; }

		public string Comment
		{
			get { return _zipFile.ZipFileComment; }
			set { _zipFile.SetComment(value); }
		}

		public string TempFileFolder { get; set; }

		public ICollection<string> EntryFileNames
		{
			get
			{
				var list = new List<string>();
				foreach (var zip in _zipFile)
				{
					var zipEntry = zip as ZipEntry;
					if (zipEntry != null)
						list.Add(zipEntry.Name);
				}
				return list;
			}
		}

		public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

		#endregion Public Properties

		#region Constructors

		internal SharpZipWriter(string zipFile)
		{
			if (File.Exists(zipFile))
			{
				_zipFile = new ZipFile(zipFile)
				           {
					           UseZip64 = UseZip64.Dynamic
				           };
				_zipFile.BeginUpdate();
			}
			else
			{
				_zipFile = ZipFile.Create(zipFile);
				_zipFile.UseZip64 = UseZip64.Dynamic;
				_zipFile.BeginUpdate();
			}
		}

		#endregion Constructors

		#region Methods

		public void AddFile(string sourceFile, string directoryPathInArchive)
		{
			_zipFile.Add(new StaticDiskDataSource(sourceFile),
			             Path.Combine(directoryPathInArchive, Path.GetFileName(sourceFile)),
			             ForceCompress ? CompressionMethod.Deflated : CompressionMethod.Stored);
		}

		public void AddFileStream(string directoryPathInArchive, Stream sourceFile, string comment)
		{
			sourceFile.Position = 0;

			_zipFile.Add(new FileStreamDataSource(sourceFile), directoryPathInArchive, ForceCompress ? CompressionMethod.Deflated : CompressionMethod.Stored);
		}

		public void AddDirectory(string sourceDirectory)
		{
			_zipFile.AddDirectory(sourceDirectory);
		}

		public void Save()
		{
			_zipFile.CommitUpdate();
		}

		public void Dispose()
		{
			if (_zipFile != null)
			{
				_zipFile.Close();
				_zipFile = null;
			}
		}

		#endregion Methods
	}
}