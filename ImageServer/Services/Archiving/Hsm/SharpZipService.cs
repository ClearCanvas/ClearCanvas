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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ICSharpCode.SharpZipLib.Zip;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    class SharpZipServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IZipService))
                return null;

            return new SharpZipService();
        }
    }
    class SharpZipService : IZipService
    {
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

        private ZipFile _zipFile;
        
        public void Dispose()
        {
            if (_zipFile != null)
            {
                _zipFile.Close();
                _zipFile = null;
            }           
        }

        public bool ForceCompress { get;set; }
        public string Comment
        {
            get
            {
                return _zipFile.ZipFileComment;
            }
            set 
            { 
                _zipFile.SetComment(value);
            } 
        }
        public string TempFileFolder { get; set; }

        public ICollection<string> EntryFileNames {
            get
            {
                var list = new List<string>();
                foreach (var zip in _zipFile)
                {
                    var zipEntry = zip as ZipEntry;
                    list.Add(zipEntry.Name);
                }
                return list;
            }
        }

        public void OpenRead(string zipFile)
        {
            _zipFile = new ZipFile(zipFile)
                {
                    UseZip64 = UseZip64.Dynamic
                };
            _zipFile.BeginUpdate();
        }

        public void OpenWrite(string zipFile)
        {
            _zipFile = ZipFile.Create(zipFile);
            _zipFile.UseZip64 = UseZip64.Dynamic;        
            _zipFile.BeginUpdate();
        }

        public void Extract(string sourceFile, string destinationFile, bool overwrite)
        {
            var zipEntry = _zipFile.GetEntry(sourceFile);
            var byteArray = new byte[1024 * 16];
            using (var s = _zipFile.GetInputStream(zipEntry))
            using (var o = new FileStream(destinationFile,overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew))
            {
                var bytesRead = s.Read(byteArray, 0, byteArray.Length);

                o.Write(byteArray, 0, bytesRead);
            }
        }

        public void ExtractAll(string destinationFolder, bool overwrite)
        {
            foreach (var zip in _zipFile)
            {
                var zipEntry = zip as ZipEntry;

                var byteArray = new byte[1024*16];
                var destinationFile = Path.Combine(destinationFolder, zipEntry.Name);
                using (var s = _zipFile.GetInputStream(zipEntry))
                using (var o = new FileStream(destinationFile, overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew))
                {
                    var bytesRead = s.Read(byteArray, 0, byteArray.Length);
                    o.Write(byteArray, 0, bytesRead);
                }   
            }
            
        }

        public void AddFile(string sourceFile, string directoryPathInArchive)
        {
            _zipFile.Add(new StaticDiskDataSource(sourceFile),
                         Path.Combine(directoryPathInArchive, Path.GetFileName(sourceFile)),
                         ForceCompress ? CompressionMethod.Deflated : CompressionMethod.Stored);
        }

        public void AddFileStream(string directoryPathInArchive, Stream sourceFile, string comment)
        {
            _zipFile.Add(new FileStreamDataSource(sourceFile), directoryPathInArchive, ForceCompress ? CompressionMethod.Deflated : CompressionMethod.Stored);
            
            var entry = _zipFile.GetEntry(directoryPathInArchive);
            entry.Comment = comment;
        }

        public void AddDirectory(string sourceDirectory)
        {
            _zipFile.AddDirectory(sourceDirectory);
        }

        public void Save()
        {
            _zipFile.CommitUpdate();
        }
    }
}
