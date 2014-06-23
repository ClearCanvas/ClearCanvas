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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions
{
    internal class ZipStudyFolderCommand : CommandBase, IDisposable
    {
        private readonly string _source;
        private readonly string _dest;
        private string _destBackup;

        public ZipStudyFolderCommand(string source, string dest)
            : base("Zip study folder", true)
        {
            Platform.CheckForNullReference(source, "source");
            Platform.CheckForNullReference(dest, "dest");
            _source = source;
            _dest = dest;
        }

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			if (RequiresRollback)
			{
				Backup();
			}
		    var zipService = Platform.GetService<IZipService>();
		    using (var zipWriter = zipService.OpenWrite(_dest))
		    {
                zipWriter.Comment = String.Format("Archive for deleted study from path {0}", _source);
                zipWriter.AddDirectory(_source);
                zipWriter.Save();
			}
		}

    	private void Backup()
        {
            if (File.Exists(_dest))
            {
                _destBackup = _dest + ".bak";
                FileUtils.Delete(_destBackup);

                FileUtils.Copy(_dest, _destBackup, true);
            }
        }

        protected override void OnUndo()
        {
			FileUtils.Delete(_dest);

            // restore backup
            if (File.Exists(_destBackup))
            {
                File.Move(_destBackup, _dest);
            }
            
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(_destBackup))
            {
                FileUtils.Delete(_destBackup);
            }
            
        }

        #endregion
    }
}