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

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
 	public class DicomDirectoryTests : AbstractTest
	{
		/// <summary>
		/// Routine for comparing two DICOMDIRs.  The Root Directory REcord for the two dicomdirs should be passed in.
		/// </summary>
		/// <remarks>
		/// The routine will traverse through the directory records and ensure that each record matches.
		/// </remarks>
		/// <param name="sq1"></param>
		/// <param name="sq2"></param>
		/// <param name="failureReason"></param>
		/// <returns></returns>
		private static bool Compare(DirectoryRecordSequenceItem sq1, DirectoryRecordSequenceItem sq2, out string failureReason)
		{
			if ((sq1.NextDirectoryRecord == null && sq2.NextDirectoryRecord != null)
			 || (sq2.NextDirectoryRecord == null && sq2.NextDirectoryRecord != null))
			{
				failureReason = "NextDirectoryRecord exists for one record, but not for the other";
				return false;
			}

			if ((sq1.LowerLevelDirectoryRecord == null && sq2.LowerLevelDirectoryRecord != null)
			 || (sq2.LowerLevelDirectoryRecord == null && sq2.LowerLevelDirectoryRecord != null))
			{
				failureReason = "LowerLevelDirectoryRecord exists for one record, but not for the other";
				return false;
			}
			List<DicomAttributeComparisonResult> failures = new List<DicomAttributeComparisonResult>();
			sq1.Equals(sq2, ref failures);

			if (failures.Count > 0)
			{
				bool foundTag = false;
				DicomTag lower = DicomTagDictionary.GetDicomTag(DicomTags.OffsetOfReferencedLowerLevelDirectoryEntity);
				DicomTag next = DicomTagDictionary.GetDicomTag(DicomTags.OffsetOfTheNextDirectoryRecord);
				failureReason = string.Empty;

				foreach (DicomAttributeComparisonResult result in failures)
				{
					if ((result.TagName == null)
						|| (!result.TagName.Equals(lower.Name) && !result.TagName.Equals(next.Name)))
					{
						foundTag = true;
						failureReason = result.Details;
					}
				}

				if (foundTag) return false;
			}

			if (sq1.LowerLevelDirectoryRecord != null)
			{
				if (!Compare(sq1.LowerLevelDirectoryRecord, sq2.LowerLevelDirectoryRecord, out failureReason))
					return false;
			}
			if (sq1.NextDirectoryRecord != null)
			{
				if (!Compare(sq1.NextDirectoryRecord, sq2.NextDirectoryRecord, out failureReason))
					return false;
			}

			failureReason = String.Empty;
			return true;
		}

		[Test]
		public void CreateDicomdirTest()
		{
			DicomFile file = new DicomFile("CreateFileTest.dcm");

			SetupMR(file.DataSet);
			SetupMetaInfo(file);

			DicomDirectory writer = new DicomDirectory("");
			int fileCount = 1;

			writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

			file = new DicomFile("CreateXaFileTest.dcm");
			SetupMultiframeXA(file.DataSet, 256, 256, 10);
			SetupMetaInfo(file);
			writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

			DicomFile newfile = new DicomFile("test2.dcm");
			SetupKoForImage(newfile.DataSet, file.DataSet);
			SetupMetaInfo(newfile);
			writer.AddFile(newfile, String.Format("DICOM\\FILE{0}", fileCount++));

			IList<DicomAttributeCollection> seriesList = SetupMRSeries(2, 2, DicomUid.GenerateUid().UID);
			foreach (DicomAttributeCollection collection in seriesList)
			{
				file = new DicomFile("test.dcm", new DicomAttributeCollection(), collection);
				fileCount++;
				SetupMetaInfo(file);

				writer.AddFile(file, String.Format("DIR001\\FILE{0}", fileCount));
			}

			writer.FileSetId = "TestDicomdir";
			writer.Save("DICOMDIR");


			DicomDirectory reader = new DicomDirectory("");

			reader.Load("DICOMDIR");


			string failureReason;
			Assert.IsTrue(Compare(writer.RootDirectoryRecord, reader.RootDirectoryRecord, out failureReason), failureReason);
		}

	

		[Test]
		public void ComparisonTest()
		{
			DicomDirectory writer = new DicomDirectory("DICOMDIR");

			int fileCount = 1;

			DicomFile file = new DicomFile("CreateFileTest.dcm");
			SetupMR(file.DataSet);
			SetupMetaInfo(file);
			writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

			file = new DicomFile("CreateFileTest.dcm");
			SetupMR(file.DataSet);
			SetupMetaInfo(file);
			writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

			DicomFile newfile = new DicomFile("test2.dcm");
			SetupKoForImage(newfile.DataSet, file.DataSet);
			SetupMetaInfo(newfile);
			writer.AddFile(newfile, String.Format("DICOM\\FILE{0}", fileCount++));

			file = new DicomFile("CreateXaFileTest.dcm");
			SetupMultiframeXA(file.DataSet, 128, 128, 4);
			SetupMetaInfo(file);
			writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

			newfile = new DicomFile("test2.dcm");
			SetupKoForImage(newfile.DataSet, file.DataSet);
			SetupMetaInfo(newfile);
			writer.AddFile(newfile, String.Format("DICOM\\FILE{0}", fileCount++));


			file = new DicomFile("CreateXaFileTest.dcm");
			SetupMultiframeXA(file.DataSet, 64, 64, 4);
			SetupMetaInfo(file);
			writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

			newfile = new DicomFile("test2.dcm");
			SetupKoForImage(newfile.DataSet, file.DataSet);
			SetupMetaInfo(newfile);
			writer.AddFile(newfile, String.Format("DICOM\\FILE{0}", fileCount++));

			IList<DicomAttributeCollection> seriesList = SetupMRSeries(3, 1, DicomUid.GenerateUid().UID);
			foreach (DicomAttributeCollection collection in seriesList)
			{
				file = new DicomFile("test.dcm", new DicomAttributeCollection(), collection);
				SetupMetaInfo(file);
				writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));
			}

			seriesList = SetupMRSeries(4, 4, DicomUid.GenerateUid().UID);
			foreach (DicomAttributeCollection collection in seriesList)
			{
				file = new DicomFile("test.dcm", new DicomAttributeCollection(), collection);
				SetupMetaInfo(file);
				writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

				file = new DicomFile("test2.dcm");
				SetupKoForImage(file.DataSet, collection);
				SetupMetaInfo(file);
				writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));
			}

			seriesList = SetupMRSeries(10, 1, DicomUid.GenerateUid().UID);
			foreach (DicomAttributeCollection collection in seriesList)
			{
				file = new DicomFile("test.dcm", new DicomAttributeCollection(), collection);
				SetupMetaInfo(file);
				writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

				file = new DicomFile("test2.dcm");
				SetupKoForImage(file.DataSet, collection);
				SetupMetaInfo(file);
				writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));
			}

			seriesList = SetupMRSeries(4, 3, DicomUid.GenerateUid().UID);
			foreach (DicomAttributeCollection collection in seriesList)
			{
				file = new DicomFile("test.dcm", new DicomAttributeCollection(), collection);
				SetupMetaInfo(file);
				writer.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));
			}

			writer.FileSetId = "TestDicomdir";
			writer.Save("DICOMDIR");


			DicomDirectory reader = new DicomDirectory("DICOMDIR");
			reader.Load("DICOMDIR");


			string failureReason;
			Assert.IsTrue(Compare(writer.RootDirectoryRecord, reader.RootDirectoryRecord, out failureReason), failureReason);

			// Test Append
			// Add a few more files to the file read in, then rewrite and read and compare
			seriesList = SetupMRSeries(4, 4, DicomUid.GenerateUid().UID);
			foreach (DicomAttributeCollection collection in seriesList)
			{
				file = new DicomFile("test.dcm", new DicomAttributeCollection(), collection);
				SetupMetaInfo(file);
				reader.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));

				file = new DicomFile("test2.dcm");
				SetupKoForImage(file.DataSet, collection);
				SetupMetaInfo(file);
				reader.AddFile(file, String.Format("DICOM\\FILE{0}", fileCount++));
			}

			reader.FileSetId = "TestDicomdir";
			reader.Save("DICOMDIR");


			DicomDirectory reader2 = new DicomDirectory("DICOMDIR");
			reader2.Load("DICOMDIR");


			Assert.IsTrue(Compare(reader.RootDirectoryRecord, reader2.RootDirectoryRecord, out failureReason), failureReason);
		}
	}
}

#endif