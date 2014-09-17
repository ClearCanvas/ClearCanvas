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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Core.Command.Archiving
{
	
	/// <summary>
	/// <see cref="CommandBase"/> to create Zip file containing all the dcm files in a study
	/// </summary>
	public sealed class CreateStudyZipCommand : CommandBase
	{
		private readonly string _zipFile;
		private readonly StudyXml _studyXml;
		private readonly string _studyFolder;
		private readonly string _tempFolder;

		public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="zipFile">The path of the zip file to create</param>
		/// <param name="studyXml">The <see cref="StudyXml"/> file describing the contents of the study.</param>
		/// <param name="studyFolder">The folder the study is stored in.</param>
		/// <param name="tempFolder">The folder for tempory files when creating the zip file.</param>
		public CreateStudyZipCommand(string zipFile, StudyXml studyXml, string studyFolder, string tempFolder) : base("Create study zip file",true)
		{
			_zipFile = zipFile;
			_studyXml = studyXml;
			_studyFolder = studyFolder;
			_tempFolder = tempFolder;
		}

		public bool ForceCompress { get; set; }

		/// <summary>
		/// Do the work.
		/// </summary>
		protected override void OnExecute(CommandProcessor theProcessor)
		{

		    var zipService = Platform.GetService<IZipService>();
			using (var zipWriter = zipService.OpenWrite(_zipFile))
			{
				zipWriter.ForceCompress = ForceCompress;
                zipWriter.TempFileFolder = _tempFolder;
                zipWriter.Comment = String.Format("Archive for study {0}", _studyXml.StudyInstanceUid);

				zipWriter.ProgressUpdated += (s, e) => EventsHelper.Fire(this.ProgressUpdated, this, e);


				// Add the studyXml file
                zipWriter.AddFile(Path.Combine(_studyFolder, String.Format("{0}.xml", _studyXml.StudyInstanceUid)), String.Empty);

				// Add the studyXml.gz file
                zipWriter.AddFile(Path.Combine(_studyFolder, String.Format("{0}.xml.gz", _studyXml.StudyInstanceUid)), String.Empty);

			    string uidMapXmlPath = Path.Combine(_studyFolder, "UidMap.xml");
                if (File.Exists(uidMapXmlPath))
                    zipWriter.AddFile(uidMapXmlPath, String.Empty);

				// Add each sop from the StudyXmlFile
				var i = 0;
				foreach (SeriesXml seriesXml in _studyXml)
				{
					i++;
					foreach (InstanceXml instanceXml in seriesXml)
					{
						string filename = Path.Combine(_studyFolder, seriesXml.SeriesInstanceUid);
						filename = Path.Combine(filename, String.Format("{0}.dcm", instanceXml.SopInstanceUid));

						zipWriter.AddFile(filename, seriesXml.SeriesInstanceUid);
					}

				}

				zipWriter.Save();
				
			}
		}

		/// <summary>
		/// Undo the work.
		/// </summary>
		protected override void OnUndo()
		{
			File.Delete(_zipFile);
		}
	}
}
