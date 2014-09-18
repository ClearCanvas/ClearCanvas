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

using System.Collections.Generic;
using System.IO;
using ClearCanvas.ImageServer.Common;
using ICSharpCode.SharpZipLib.Zip;

namespace ClearCanvas.ImageServer.Core.SharpZipLib
{
    internal class SharpZipReader : IZipServiceReader
    {
        #region Private Members

        private ZipFile _zipFile;

        #endregion Private Members

        #region Public Properties

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

        #endregion Public Properties

        #region Constructors

        internal SharpZipReader(string zipFile)
        {
            _zipFile = new ZipFile(zipFile)
            {
                UseZip64 = UseZip64.Dynamic
            };
            _zipFile.BeginUpdate();
        }

        #endregion Constructors

        #region Methods

        public void Extract(string sourceFile, string destinationFolder, bool overwrite)
        {
            var zipEntry = _zipFile.GetEntry(sourceFile);
			Directory.CreateDirectory(destinationFolder);
			var destinationFile = Path.Combine(destinationFolder, zipEntry.Name);
            using (var s = _zipFile.GetInputStream(zipEntry))
            using (var o = new FileStream(destinationFile, overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew, FileAccess.Write))
            {
	            s.CopyTo(o);
				o.Flush(true);
				o.Close();
            }
        }

        public void ExtractAll(string destinationFolder, bool overwrite)
        {
            foreach (var zip in _zipFile)
            {
                var zipEntry = zip as ZipEntry;
                if (zipEntry != null)
                {
                    var destinationFile = Path.Combine(destinationFolder, zipEntry.Name);
					Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                    using (var s = _zipFile.GetInputStream(zipEntry))
                    using (
                        var o = new FileStream(destinationFile, overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew, FileAccess.Write))
                    {
						s.CopyTo(o);
						o.Flush(true);
						o.Close();
					}
                }
            }
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
