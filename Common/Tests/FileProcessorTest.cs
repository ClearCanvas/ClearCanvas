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

#if	UNIT_TESTS

#pragma warning disable 1591

using System;
using System.IO;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Common.Tests
{
	[TestFixture]
	public class FileProcessorTest
	{
		// The delegate
		private FileProcessor.ProcessFile _del;

		// Root test directory
		private string _testDir = Directory.GetCurrentDirectory() + @"..\..\..\..\..\UnitTestFiles\ClearCanvas.Common.Tests.FileProcessorTest";

		// The delgate function
		static void PrintPath(string path)
		{
			System.Console.WriteLine(path);
		}

		public FileProcessorTest()
		{
		}

		public string[] CreateFiles(string path, string extension, int numFiles)
		{
			string file;
			string[] fileList = new string[numFiles];
			FileStream stream;
			
			for (int i = 0; i < numFiles; i++)
			{
				file = String.Format("{0}\\File{1}{2}", path, i, extension);
				fileList[i] = file;
				stream = File.Create(file);
				// Close the file so we don't have a problem deleting the directory later
				stream.Close();
			}

			return fileList;
		}

		public string[] CreateDirectories(string path, int numDirs)
		{
			string dir;
			string[] dirList= new string[numDirs];
			
			for (int i = 0; i < numDirs; i++)
			{
				dir = String.Format("{0}\\Dir{1}", path, i);
				dirList[i] = dir;
				Directory.CreateDirectory(dir);
			}

			return dirList;
		}

		[TestFixtureSetUp]
		public void Init()
		{
			// Assign the delegate
			_del = new FileProcessor.ProcessFile(PrintPath);

			// Delete the old test directory, if it somehow didn't get deleted on teardown
			if (Directory.Exists(_testDir))
				Directory.Delete(_testDir, true);

			// Create the new test directory
			Directory.CreateDirectory(_testDir);
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			// Get rid of the test directory
			Directory.Delete(_testDir, true);
		}

		[Test]
		public void ProcessEmptyDirectory()
		{
			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void ProcessDirectoryWithFilesOnly()
		{
			CreateFiles(_testDir, "", 10);
			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void ProcessDirectoryWithSubdirectoriesOnly()
		{
			CreateDirectories(_testDir, 3);
			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void ProcessDirectoryWithFileAndSubdirectories()
		{
			string[] dirList = CreateDirectories(_testDir, 3);
			CreateFiles(_testDir, "", 5);
			CreateFiles(dirList[0], "",  6);

			FileProcessor.Process(_testDir, "", _del, true);
		}

		[Test]
		public void ProcessFileOnly()
		{
			string[] fileList = CreateFiles(_testDir, "", 1);

			FileProcessor.Process(fileList[0], "", _del, true);
		}

		[Test]
		public void ProcessWildcards()
		{
			CreateFiles(_testDir, ".txt", 5);
			CreateFiles(_testDir, ".abc", 5);
			
			FileProcessor.Process(_testDir, "*.abc", _del, true);
		}
		
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void ProcessPathDoesNotExist()
		{
			FileProcessor.Process("c:\\NoSuchPath", "", _del, true);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ProcessPathEmpty()
		{
			FileProcessor.Process("", "", _del, true);
		}
	}
}

#endif