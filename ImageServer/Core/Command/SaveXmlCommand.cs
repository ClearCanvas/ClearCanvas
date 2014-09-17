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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Command
{
    /// <summary>
    /// Insert DICOM file into a <see cref="StudyXml"/> file and save to disk.
    /// </summary>
    public class SaveXmlCommand : CommandBase, IDisposable
    {
        #region Private Members

        private readonly StudyXml _stream;
        private readonly string _xmlPath;
        private readonly string _gzPath;
        private string _xmlBackupPath;
        private string _gzBackupPath;
        private bool _fileSaved;
        #endregion

        #region Private Static Members
        private static readonly StudyXmlOutputSettings _outputSettings = ImageServerCommonConfiguration.DefaultStudyXmlOutputSettings;
        #endregion

        #region Constructors

        public SaveXmlCommand(StudyXml stream, StudyStorageLocation storageLocation)
            : base("Insert into Study XML", true)
        {
            Platform.CheckForNullReference(stream, "StudyStream object");
            Platform.CheckForNullReference(storageLocation, "Study Storage Location");

            _stream = stream;
            _xmlPath = Path.Combine(storageLocation.GetStudyPath(), storageLocation.StudyInstanceUid + ".xml");
            _gzPath = _xmlPath + ".gz";
        }

		public SaveXmlCommand(StudyXml stream, string rootStudyPath, string studyInstanceUid)
			: base("Insert into Study XML", true)
		{
			Platform.CheckForNullReference(stream, "StudyStream object");
			Platform.CheckForNullReference(rootStudyPath, "Study folder path");
			Platform.CheckForNullReference(studyInstanceUid, "Study Instance Uid");

			_stream = stream;
			_xmlPath = Path.Combine(rootStudyPath, studyInstanceUid + ".xml");
			_gzPath = _xmlPath + ".gz";
		}

        #endregion

        #region Private Methods

        private void Backup()
        {
            if (File.Exists(_xmlPath))
            {
                try
                {
                    var random = new Random();
                    _xmlBackupPath = String.Format("{0}.bak.{1}", _xmlPath, random.Next());
                    File.Copy(_xmlPath, _xmlBackupPath);
                }
                catch (IOException)
                {
                    _xmlBackupPath = null;
                    throw;
                }
            }
            if (File.Exists(_gzPath))
            {
                try
                {
                    var random = new Random();
                    _gzBackupPath = String.Format("{0}.bak.{1}", _gzPath, random.Next());
                    File.Copy(_gzPath, _gzBackupPath);
                }
                catch (IOException)
                {
                    _gzBackupPath = null;
                    throw;
                }
            }
        }

        private void WriteStudyStream(string streamFile, string gzStreamFile, StudyXml theStream)
        {
            var theMemento = theStream.GetMemento(_outputSettings);

            // allocate the random number generator here, in case we need it below
            var rand = new Random();
            string tmpStreamFile = streamFile + "_tmp";
            string tmpGzStreamFile = gzStreamFile + "_tmp";
            for (int i = 0; ; i++)
                try
                {
					//FileUtils.Delete checks for existence
                    FileUtils.Delete(tmpStreamFile);
                    FileUtils.Delete(tmpGzStreamFile);

                    _fileSaved = true;

                    using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(tmpStreamFile, FileMode.CreateNew),
                                      gzipStream = FileStreamOpener.OpenForSoleUpdate(tmpGzStreamFile, FileMode.CreateNew))
                    {
                        StudyXmlIo.WriteXmlAndGzip(theMemento, xmlStream, gzipStream);
                        xmlStream.Close();
                        gzipStream.Close();
                    }

					//FileUtils.Delete checks for existence
                    FileUtils.Delete(streamFile);
                    File.Move(tmpStreamFile, streamFile);

					//FileUtils.Delete checks for existence
                    FileUtils.Delete(_gzPath);
                    File.Move(tmpGzStreamFile, _gzPath);
                    return;
                }
                catch (IOException)
                {
                    if (i < 5)
                    {
                        Thread.Sleep(rand.Next(5, 50)); // Sleep 5-50 milliseconds
                        continue;
                    }

                    throw;
                }
        }

        #endregion

        #region Overridden Protected Methods

		protected override void OnExecute(CommandProcessor theProcessor)
        {
            Backup();

            WriteStudyStream(_xmlPath, _gzBackupPath, _stream);
        }

        protected override void OnUndo()
        {
            if (_fileSaved)
                FileUtils.Delete(_xmlPath);

            if (false == String.IsNullOrEmpty(_xmlBackupPath) && File.Exists(_xmlBackupPath))
            {
                // restore original file
                File.Copy(_xmlBackupPath, _xmlPath, true);
            }

            if (_fileSaved)
                FileUtils.Delete(_gzPath);

            if (false == String.IsNullOrEmpty(_gzBackupPath) && File.Exists(_gzBackupPath))
            {
                // restore original file
                File.Copy(_gzBackupPath, _gzPath, true);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (false == String.IsNullOrEmpty(_xmlBackupPath))
            {
                FileUtils.Delete(_xmlBackupPath);
            }
            if (false == String.IsNullOrEmpty(_gzBackupPath))
            {
                FileUtils.Delete(_gzBackupPath);
            }
        }

        #endregion

        #endregion
    }
}