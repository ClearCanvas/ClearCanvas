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

#pragma warning disable 1591,0419,1574,1587, 649

using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.Dicom.Utilities.Xml;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Command.Tests
{
    internal class TestContext : ICommandProcessorContext
    {
        public TestContext()
        {
            BackupDirectory = Path.GetTempPath();
        }

        public void Dispose()
        {
            
        }

        public void PreExecute(ICommand command)
        {
            CommandsExecuted++;
        }

        public void Commit()
        {
            CommitEncountered = true;
        }

        public void Rollback()
        {
            RollbackEncountered = true;
        }

        public int CommandsExecuted { get; set; }
        public bool RollbackEncountered { get; set; }
        public bool CommitEncountered { get; set; }
        public string TempDirectory
        {
            get { return Path.GetTempPath(); }
        }

        public string BackupDirectory
        {
            get;
            set;
        }
    }

    internal class TestCommandProcessor : CommandProcessor
    {
        public TestCommandProcessor() : base("Test",new TestContext())
        {}

        public TestContext TestContext { get { return ProcessorContext as TestContext; } }
    }

	[TestFixture]
	public class CommandTests : AbstractTest
	{
	    private DicomFile _dicomFile;
		
        public CommandTests()
        {
            _dicomFile = new DicomFile();
            SetupMR(_dicomFile.DataSet);
        }
        
		[Test]
		public void TestSaveAndDeleteCommand()
		{
		    string file;

            using (var processor = new TestCommandProcessor())
            {
                file = Path.Combine(processor.ProcessorContext.TempDirectory, "Test.dcm");
                
                if (File.Exists(file)) File.Delete(file);

                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 2);

                Assert.IsTrue(File.Exists(file));
            }

            using (var processor = new TestCommandProcessor())
            {

                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsTrue(File.Exists(file));
            }

            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsTrue(File.Exists(file));
            }

            // Delete the file
            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new FileDeleteCommand(file, false));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsFalse(File.Exists(file));
            }

            // Resave and delete
            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                processor.AddCommand(new FileDeleteCommand(file, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 2);

                Assert.IsFalse(File.Exists(file));
            }

            // Resave and delete Rollback
            using (var processor = new TestCommandProcessor())
            {
                string newfile = Path.Combine(processor.ProcessorContext.TempDirectory, "Test2.dcm");
            
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                processor.AddCommand(new FileDeleteCommand(file, true));
                processor.AddCommand(new RenameFileCommand(file, newfile, true)); // Should fail

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);
                Assert.IsFalse(File.Exists(file));
            }

            // Cleanup the file
            if (File.Exists(file))
		        File.Delete(file);
		}

        [Test]
        public void AggregateTest()
        {
            string file;

            using (var processor = new TestCommandProcessor())
            {
                file = Path.Combine(processor.ProcessorContext.TempDirectory, "AggregateTest.dcm");
                if (File.Exists(file)) File.Delete(file);

                var aggregateCommand = new AggregateCommand();
                processor.AddCommand(aggregateCommand);

                aggregateCommand.AddSubCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                aggregateCommand.AddSubCommand(new FileDeleteCommand(file, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);

                Assert.IsFalse(File.Exists(file));
            }

            using (var processor = new TestCommandProcessor())
            {
                file = Path.Combine(processor.ProcessorContext.TempDirectory, "AggregateTest.dcm");

                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));

                var aggregateCommand = new AggregateCommand();
                processor.AddCommand(aggregateCommand);

                aggregateCommand.AddSubCommand(new SaveDicomFileCommand(file, _dicomFile, true));

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);

                Assert.IsFalse(File.Exists(file));
            }
        }

        [Test]
        public void DirectoryTests()
        {
            string directory;

            using (var processor = new TestCommandProcessor())
            {
                directory = Path.Combine(processor.ProcessorContext.TempDirectory, "DirectoryTest");
                string file = Path.Combine(directory, "Test.dcm");
                string file2 = Path.Combine(directory, "Test2.dcm");

                DirectoryUtility.DeleteIfExists(directory);

                processor.AddCommand(new CreateDirectoryCommand(directory));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));
                processor.AddCommand(new SaveDicomFileCommand(file2, _dicomFile, true));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 5);

                Assert.IsFalse(Directory.Exists(directory));
                DirectoryUtility.DeleteIfExists(directory);
            }

            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new CreateDirectoryCommand(directory));
                processor.AddCommand(new CreateDirectoryCommand(directory));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 2);

                Assert.IsTrue(Directory.Exists(directory));
            }

            using (var processor = new TestCommandProcessor())
            {
                string file = Path.Combine(directory, "Test.dcm");

                processor.AddCommand(new CreateDirectoryCommand(directory));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);

                // Directory shoudl still exist
                Assert.IsTrue(Directory.Exists(directory));
                Assert.IsFalse(File.Exists(file));
            }

            using (var processor = new TestCommandProcessor())
            {
                string file = Path.Combine(directory, "Test.dcm");
                string file2 = Path.Combine(directory, "Test2.dcm");

                DirectoryUtility.DeleteIfExists(directory);

                processor.AddCommand(new CreateDirectoryCommand(directory));
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, true));
                processor.AddCommand(new SaveDicomFileCommand(file2, _dicomFile, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);

                Assert.IsTrue(File.Exists(file));
                Assert.IsTrue(File.Exists(file2));
            }

            string directory2;
            using (var processor = new TestCommandProcessor())
            {
                directory2 = Path.Combine(processor.ProcessorContext.TempDirectory, "DirectoryTest2");
                string file = Path.Combine(directory2, "Test.dcm");
                string file2 = Path.Combine(directory2, "Test2.dcm");

                processor.AddCommand(new CopyDirectoryCommand(directory, directory2, null));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsTrue(File.Exists(file));
                Assert.IsTrue(File.Exists(file2));
            }

            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new DeleteDirectoryCommand(directory2, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsFalse(Directory.Exists(directory2));
                Assert.IsTrue(Directory.Exists(directory));
            }

            using (var processor = new TestCommandProcessor())
            {
                directory2 = Path.Combine(processor.ProcessorContext.TempDirectory, "DirectoryTest2");
                string file = Path.Combine(directory2, "Test.dcm");
                string file2 = Path.Combine(directory2, "Test2.dcm");

                processor.AddCommand(new MoveDirectoryCommand(directory, directory2, null));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsTrue(File.Exists(file));
                Assert.IsTrue(File.Exists(file2));
                Assert.IsTrue(Directory.Exists(directory));
            }
            Assert.IsFalse(Directory.Exists(directory));

            using (var processor = new TestCommandProcessor())
            {
                processor.AddCommand(new DeleteDirectoryCommand(directory2, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 1);

                Assert.IsFalse(Directory.Exists(directory2));
                Assert.IsFalse(Directory.Exists(directory));
            }
        }
        [Test]
        public void StudyXmlTest()
        {
            string directory;

            using (var processor = new TestCommandProcessor())
            {
                var studyXml = new StudyXml(DicomUid.GenerateUid().UID);
                var images = SetupMRSeries(2, 5, studyXml.StudyInstanceUid);
                directory = Path.Combine(processor.ProcessorContext.TempDirectory, "StudyXmlTest");
                
                DirectoryUtility.DeleteIfExists(directory);

                processor.AddCommand(new CreateDirectoryCommand(directory));

                foreach (var i in images)
                {
                    string file = Path.Combine(directory, i[DicomTags.SopInstanceUid] + ".dcm");
                    processor.AddCommand(new SaveDicomFileCommand(file, new DicomFile(file,new DicomAttributeCollection(), i), false));
                    processor.AddCommand(new InsertInstanceXmlCommand(studyXml, file));
                }

                Assert.IsTrue(processor.Execute(),processor.FailureReason);
                Assert.IsTrue((images.Count*2 + 1) == processor.TestContext.CommandsExecuted);

                foreach (var i in images)
                {
                    Assert.IsTrue(studyXml.Contains(i[DicomTags.SeriesInstanceUid],i[DicomTags.SopInstanceUid]));
                }
                DirectoryUtility.DeleteIfExists(directory);
            }
        }

        [Test]
        public void StudyXmlTestFailure()
        {
            string directory;

            using (var processor = new TestCommandProcessor())
            {
                var studyXml = new StudyXml(DicomUid.GenerateUid().UID);
                var images = SetupMRSeries(2, 5, studyXml.StudyInstanceUid);
                directory = Path.Combine(processor.ProcessorContext.TempDirectory, "StudyXmlTest2");

                DirectoryUtility.DeleteIfExists(directory);

                processor.AddCommand(new CreateDirectoryCommand(directory));

                foreach (var i in images)
                {
                    string file = Path.Combine(directory, i[DicomTags.SopInstanceUid] + ".dcm");
                    processor.AddCommand(new SaveDicomFileCommand(file, new DicomFile(file, new DicomAttributeCollection(), i), false));
                    processor.AddCommand(new InsertInstanceXmlCommand(studyXml, file));
                }

                string file2 = Path.Combine(directory, "Test.dcm");

                processor.AddCommand(new SaveDicomFileCommand(file2, _dicomFile, true));
                processor.AddCommand(new SaveDicomFileCommand(file2, _dicomFile, true));


                Assert.IsFalse(processor.Execute(), processor.FailureReason);
                Assert.IsTrue((images.Count * 2 + 3) == processor.TestContext.CommandsExecuted);

                foreach (var i in images)
                {
                    Assert.IsFalse(studyXml.Contains(i[DicomTags.SeriesInstanceUid], i[DicomTags.SopInstanceUid]));
                }
                
                // Directory should be deleted too
                Assert.IsFalse(Directory.Exists(directory));
            }
        }
	}
}

#endif