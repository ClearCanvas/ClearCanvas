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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Core.Command;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
	/// <summary>
	/// Utility for processing files within a study.
	/// </summary>
	public class ProcessStudyUtility
	{
		#region Subclass

		/// <summary>
		/// Represents a file to be processed by <see cref="ProcessStudyUtility"/>
		/// </summary>
		public class ProcessorFile : IDisposable
		{
			public ProcessorFile(DicomFile file, WorkItemUid uid)
			{
				File = file;
				ItemUid = uid;
			}

			public ProcessorFile(string path, WorkItemUid uid)
			{
				FilePath = path;
				ItemUid = uid;
			}

			/// <summary>
			/// Path to the <see cref="DicomFile"/> to process.  Can be used instead of <see cref="File"/>.
			/// </summary>
			public string FilePath { get; set; }

			/// <summary>
			/// The DICOM File to process.  Can be used instead of <see cref="FilePath"/>.
			/// </summary>
			public DicomFile File { get; set; }

			/// <summary>
			/// An optional <see cref="WorkItemUid"/> associated with the file to be processed.  Will be updated appropriately.
			/// </summary>
			public WorkItemUid ItemUid { get; set; }

			public void Dispose()
			{
				File = null;
				ItemUid = null;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The <see cref="StudyLocation"/> for the study being processed.
		/// </summary>
		public StudyLocation StudyLocation { get; private set; }

		public bool IsReprocess { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Note that all SOP Instances processed must be from the same study.
		/// </para>
		/// </remarks>
		/// <param name="location">The StudyLocation for the study being processed</param>
		public ProcessStudyUtility(StudyLocation location)
		{
			Platform.CheckForNullReference(location, "location");
			StudyLocation = location;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Process a specific DICOM file which may be related to a <see cref="WorkItem"/> request.
		/// </summary>
		/// <remarks>
		/// <para>
		/// On success and if <see cref="uid"/> is set, the <see cref="WorkItemUid"/> field is marked as complete.  If processing fails, 
		/// the FailureCount field is incremented for the <see cref="WorkItemUid"/>.
		/// </para>
		/// </remarks>
		/// <param name="studyXml">The <see cref="StudyXml"/> file to update with information from the file.</param>
		/// <param name="file">The file to process.</param>
		/// <param name="uid">An optional WorkQueueUid associated with the entry, that will be deleted upon success or failed on failure.</param>
		/// <exception cref="ApplicationException"/>
		/// <exception cref="DicomDataException"/>
		public void ProcessFile(DicomFile file, StudyXml studyXml, WorkItemUid uid)
		{
			Platform.CheckForNullReference(file, "file");
			Platform.CheckForNullReference(studyXml, "studyXml");
			var processFile = new ProcessorFile(file, uid);
			InsertBatch(new List<ProcessorFile> {processFile}, studyXml);
		}

		/// <summary>
		/// Process a batch of DICOM Files related to a specific Study.  Updates for all the files will be processed together.
		/// </summary>
		/// <param name="list">The list of files to batch together.</param>
		/// <param name="studyXml">The <see cref="StudyXml"/> file to update with information from the file.</param>
		public void ProcessBatch(IList<ProcessorFile> list, StudyXml studyXml)
		{
			Platform.CheckTrue(list.Count > 0, "list");
			Platform.CheckForNullReference(studyXml, "studyXml");
			InsertBatch(list, studyXml);
		}

		#endregion

		#region Private Methods

		private void InsertBatch(IList<ProcessorFile> list, StudyXml studyXml)
		{
			using (var processor = new ViewerCommandProcessor("Processing WorkItem DICOM file(s)"))
			{
				try
				{
					// Create an AggregrateCommand where we batch together all the database updates
					// and execute them together as the last command.
					var batchDatabaseCommand = new AggregateCommand();

					foreach (var file in list)
					{
						if (!string.IsNullOrEmpty(file.FilePath) && file.File == null)
						{
							try
							{
								file.File = new DicomFile(file.FilePath);

								// WARNING:  If we ever do anything where we update files and save them,
								// we may have to change this.
								file.File.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);
							}
							catch (FileNotFoundException)
							{
								Platform.Log(LogLevel.Warn, "File to be processed is not found, ignoring: {0}",
								             file.FilePath);

								if (file.ItemUid != null)
									batchDatabaseCommand.AddSubCommand(new CompleteWorkItemUidCommand(file.ItemUid));

								continue;
							}
						}
						else
						{
							file.FilePath = file.File.Filename;
						}

						String seriesUid = file.File.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
						String sopUid = file.File.DataSet[DicomTags.SopInstanceUid].GetString(0, String.Empty);

						String finalDest = StudyLocation.GetSopInstancePath(seriesUid, sopUid);

						if (file.FilePath != finalDest)
						{
							processor.AddCommand(CommandFactory.CreateRenameFileCommand(file.FilePath, finalDest, false));
						}

						// Update the StudyStream object
						var insertStudyXmlCommand = new InsertStudyXmlCommand(file.File, studyXml, StudyLocation, false);
						processor.AddCommand(insertStudyXmlCommand);

						if (file.ItemUid != null)
							batchDatabaseCommand.AddSubCommand(new CompleteWorkItemUidCommand(file.ItemUid));
					}

					// Now save the batched updates to the StudyXml file.
					processor.AddCommand(new SaveStudyXmlCommand(studyXml, StudyLocation));

					// Update the Study table, based on the studyXml
					var updateReason = IsReprocess ? InsertOrUpdateStudyCommand.UpdateReason.Reprocessing
						: InsertOrUpdateStudyCommand.UpdateReason.LiveImport;

					batchDatabaseCommand.AddSubCommand(new InsertOrUpdateStudyCommand(StudyLocation, studyXml, updateReason));

					// Now, add all the batched database updates
					processor.AddCommand(batchDatabaseCommand);

					// Do the actual processing
					if (!processor.Execute())
					{
						Platform.Log(LogLevel.Error, "Failure processing {0} for Study: {1}",
						             processor.Description, StudyLocation.Study.StudyInstanceUid);
						throw new ApplicationException(
							"Unexpected failure (" + processor.FailureReason + ") executing command for Study: " +
							StudyLocation.Study.StudyInstanceUid, processor.FailureException);
					}

					StudyLocation.Study = processor.ViewerContext.ContextStudy;

					Platform.Log(LogLevel.Info, "Processed {0} SOPs for Study {1}", list.Count, StudyLocation.Study.StudyInstanceUid);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
					             processor.Description);
					processor.Rollback();
					throw new ApplicationException("Unexpected exception when processing file.", e);
				}
			}
		}

		#endregion
	}
}