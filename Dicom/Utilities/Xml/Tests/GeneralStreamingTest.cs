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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Xml.Tests
{
	[TestFixture]
	public class GeneralStreamingTest : AbstractTest
	{
		[Test]
		public void ConstructorTest()
		{
			StudyXml stream = new StudyXml();

			stream = new StudyXml("1.1.1");

		}

		private void WriteStudyStream(string streamFile, StudyXml theStream)
		{
			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			settings.IncludeSourceFileName = false;

			var doc = theStream.GetMemento(settings);

			if (File.Exists(streamFile))
				File.Delete(streamFile);

			using (Stream fileStream = new FileStream(streamFile, FileMode.CreateNew))
			{
				StudyXmlIo.Write(doc, fileStream);
				fileStream.Close();
			}

			return;
		}
		/// <summary>
		/// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
		/// </summary>
		/// <param name="location">The location a study is stored.</param>
		/// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
		private StudyXml LoadStudyStream(string location)
		{
			StudyXml theXml = new StudyXml();

			if (File.Exists(location))
			{
				using (Stream fileStream = new FileStream(location, FileMode.Open))
				{
					var theMemento = new StudyXmlMemento();

					StudyXmlIo.Read(theMemento, fileStream);

					theXml.SetMemento(theMemento);

					fileStream.Close();
				}
			}


			return theXml;
		}

		[Test]
		public void CreationTest()
		{
			IList<DicomAttributeCollection> instanceList;

			string studyInstanceUid = DicomUid.GenerateUid().UID;

			instanceList = SetupMRSeries(4, 10, studyInstanceUid);



			StudyXml studyXml = new StudyXml(studyInstanceUid);

			string studyXmlFilename = Path.GetTempFileName();

			foreach (DicomAttributeCollection instanceCollection in instanceList)
			{
				instanceCollection[DicomTags.PixelData] = null;


				DicomFile theFile = new DicomFile("test", new DicomAttributeCollection(), instanceCollection);
				SetupMetaInfo(theFile);

				studyXml.AddFile(theFile);

				WriteStudyStream(studyXmlFilename, studyXml);
			}

			StudyXml newXml = LoadStudyStream(studyXmlFilename);

			if (!Compare(newXml, instanceList))
				Assert.Fail("Comparison of StudyXML failed against base loaded from disk");

			if (!Compare(studyXml, instanceList))
				Assert.Fail("Comparison of StudyXML failed against base in memory");

		}

		private bool Compare(StudyXml studyXml, IList<DicomAttributeCollection> instanceList)
		{
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					foreach (DicomAttributeCollection baseCollection in instanceList)
					{
						string sopInstanceUid = baseCollection[DicomTags.SopInstanceUid].ToString();
						if (sopInstanceUid.Equals(instanceXml.SopInstanceUid))
						{
							if (!baseCollection.Equals(instanceXml.Collection))
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}
	}
}

#endif