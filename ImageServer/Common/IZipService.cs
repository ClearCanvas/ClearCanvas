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

namespace ClearCanvas.ImageServer.Common
{
	public class ProgressUpdatedEventArgs:EventArgs
	{
		public string Status { get; set; }
		public float Percentage { get; set; }
	}

    /// <summary>
    /// Interface for reading from Zip file.
    /// </summary>
    public interface IZipServiceReader : IDisposable
    {
        string Comment { get; }
        string TempFileFolder { get; set; }
        ICollection<string> EntryFileNames { get; } 
        void Extract(string sourceFile, string destinationFile, bool overwrite);
        void ExtractAll(string destinationFolder, bool overwrite);
    }

    /// <summary>
    /// Interface for writing to a Zip file.
    /// </summary>
    public interface IZipServiceWriter : IDisposable
    {
        bool ForceCompress { get; set; }
        string Comment { get; set; }
        string TempFileFolder { get; set; }
        ICollection<string> EntryFileNames { get; } 
        void AddFile(string sourceFile, string directoryPathInArchive);
        void AddFileStream(string directoryPathInArchive, Stream sourceFile, string comment);
        void AddDirectory(string sourceDirectory);
        void Save();

		event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
    }

    /// <summary>
    /// Service for getting an implementation for reading and writing Zip files
    /// </summary>
    public interface IZipService
    {
        IZipServiceReader OpenRead(string zipFile);
        IZipServiceWriter OpenWrite(string zipFile);
    }
}
