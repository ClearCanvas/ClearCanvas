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
using ClearCanvas.Dicom.Tests;
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
            RollbackEncountered = true;
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
                processor.AddCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                processor.AddCommand(new FileDeleteCommand(file, true));
                processor.AddCommand(new FileDeleteCommand(file, true)); // Should fail

                Assert.IsFalse(processor.Execute());
                Assert.IsTrue(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);
                Assert.IsTrue(File.Exists(file));
            }

            // Cleanup the file
		    File.Delete(file);
		}

        [Test]
        public void AggregateTest()
        {
            string file;

            using (var processor = new TestCommandProcessor())
            {
                file = Path.Combine(processor.ProcessorContext.TempDirectory, "AggregateTest.dcm");

                var aggregateCommand = new AggregateCommand();
                processor.AddCommand(aggregateCommand);

                aggregateCommand.AddSubCommand(new SaveDicomFileCommand(file, _dicomFile, false));
                aggregateCommand.AddSubCommand(new FileDeleteCommand(file, true));

                Assert.IsTrue(processor.Execute());
                Assert.IsFalse(processor.TestContext.RollbackEncountered);
                Assert.AreEqual(processor.TestContext.CommandsExecuted, 3);

                Assert.IsTrue(File.Exists(file));
            }
        }
	}
}

#endif