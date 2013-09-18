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
#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Externals.General.Tests
{
	[TestFixture]
	public class CommandLineExternalTest
	{
		private const int _processEndWaitDelay = 1000;

		[Test]
		public void TestBasic()
		{
			string workingDirectory = Environment.CurrentDirectory;
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;

				Assert.IsTrue(external.IsValid, "Minimum parameters for Command Line External should be just the command itself");

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					Assert.IsTrue(external.CanLaunch(image), "Check that external can launch the dummy image");

					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					AssertAreEqualIgnoreCase(external.Command, command.ExecutedCommand, "Wrong command was executed");
					AssertAreEqualIgnoreCase(workingDirectory, command.ExecutedWorkingDirectory, "Command executed in wrong working directory.");
				}
			}
		}

		[Test]
		public void TestWorkingDirectory()
		{
			string workingDirectory = Environment.CurrentDirectory;
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.WorkingDirectory = Path.GetPathRoot(workingDirectory);
				external.Command = command.ScriptFilename;

				Assert.IsTrue(external.IsValid, "Minimum parameters for Command Line External should be just the command itself");

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					AssertAreEqualIgnoreCase(external.Command, command.ExecutedCommand, "Wrong command was executed");
					AssertAreEqualIgnoreCase(external.WorkingDirectory, command.ExecutedWorkingDirectory, "Command executed in wrong working directory.");
				}
			}
		}

		[Test]
		public void TestEnvironmentVariableExpansion()
		{
			try
			{
				TestEnvironmentVariableExpansionCore();
			}
			catch (SecurityException)
			{
				const string msg = "Test failing with security exception - you may need to start unit tests with UAC elevation";
				Console.WriteLine(msg);
				throw;
			}
		}

		private void TestEnvironmentVariableExpansionCore()
		{
			string baseDirectory = Path.Combine(Path.Combine(Path.GetTempPath(), "ClearCanvas"), this.GetType().Name);
			string commandDirectory = Path.Combine(baseDirectory, "TestEnvironmentVariableExpansion");
			string invalidDirectory = Path.Combine(baseDirectory, "TestEnvironmentVariableExpansion.April");
			string workingDirectory = Path.Combine(baseDirectory, "TestEnvironmentVariableExpansion.Archer");

			Directory.CreateDirectory(commandDirectory);
			Directory.CreateDirectory(invalidDirectory);
			Directory.CreateDirectory(workingDirectory);

			using (MockCommandLine command = new MockCommandLine(commandDirectory))
			{
				using (var processVars = new EnvironmentVariablesTestConstruct(EnvironmentVariableTarget.Process))
				{
					processVars["CMDDIR"] = "TestEnvironmentVariableExpansion";
					processVars["WRKDIR"] = "TestEnvironmentVariableExpansion.April";

					using (var userVars = new EnvironmentVariablesTestConstruct(EnvironmentVariableTarget.User))
					{
						userVars["CMDDIR"] = "TestEnvironmentVariableExpansion";
						userVars["WRKDIR"] = "TestEnvironmentVariableExpansion.Archer";
						userVars["ARGA"] = "Kirk";
						userVars["ARGB"] = "Picard";

						using (var machineVars = new EnvironmentVariablesTestConstruct(EnvironmentVariableTarget.Machine))
						{
							machineVars["CMDDIR"] = "TestEnvironmentVariableExpansion";
							machineVars["ARGA"] = "Decker";
							machineVars["ARGB"] = "Locutus";
							machineVars["ARGC"] = "Sisko";
							machineVars["ARGD"] = "Janeway";
							machineVars["ARGE"] = "$00100020$";

							CommandLineExternal external = new CommandLineExternal();
							external.WorkingDirectory = baseDirectory + Path.DirectorySeparatorChar + userVars.Format("WRKDIR");
							external.Command = baseDirectory + Path.DirectorySeparatorChar + userVars.Format("CMDDIR") + Path.DirectorySeparatorChar + Path.GetFileName(command.ScriptFilename);
							external.Arguments = string.Format("\"{0}\" \"Chateau {1} 2347\" \"{2} vs. {3}\" \"Over 9000%%\" \"%Kim%: Ensign for Life\" \"{4}\" \"$00100021$\"", userVars.Format("ARGA"), userVars.Format("ARGB"), userVars.Format("ARGC"), userVars.Format("ARGD"), userVars.Format("ARGE"));
							external.WaitForExit = true;

							using (MockDicomPresentationImage image = new MockDicomPresentationImage())
							{
								image[0x00100020].SetStringValue("Archer");
								image[0x00100021].SetStringValue(userVars.Format("ARGB"));

								external.Launch(image);

								Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
								command.Refresh();

								Trace.WriteLine(string.Format("Command Execution Report"));
								Trace.WriteLine(command.ExecutionReport);

								// verify that command path is processed for environment variables
								AssertAreEqualIgnoreCase(commandDirectory + Path.DirectorySeparatorChar + Path.GetFileName(command.ScriptFilename), command.ExecutedCommand, "Wrong command was executed: ENVVARS aren't being processed correctly");

								// verify that working directory is processed for environment variables and that process-scope variables are **not** being used
								AssertAreEqualIgnoreCase(workingDirectory, command.ExecutedWorkingDirectory, "Command executed in wrong working directory: ENVVARS aren't being processed correctly (should not be using process-scope)");

								// verify that user-scope variables override machine-scope variables
								Assert.AreEqual("\"Kirk\"", command.ExecutedArguments[0], "Wrong argument passed at index {0}: ENVVARS of different scopes aren't being processed with the correct priority", 0);

								// verify that variable expansion takes place without modifying literals
								Assert.AreEqual("\"Chateau Picard 2347\"", command.ExecutedArguments[1], "Wrong argument passed at index {0}: ENVVARS aren't processing literals", 1);

								// verify that variable expansion handles more than one variable in a single argument
								Assert.AreEqual("\"Sisko vs. Janeway\"", command.ExecutedArguments[2], "Wrong argument passed at index {0}: ENVVARS aren't processing more than one variable", 2);

								// verify that variable expansion handles literal percent sign character escape sequence
								Assert.AreEqual("\"Over 9000%\"", command.ExecutedArguments[3], "Wrong argument passed at index {0}: ENVVARS aren't processing literal percent sign characters", 3);

								// verify that variable expansion treats undefined variables as a literal sequence
								Assert.AreEqual("\"%Kim%: Ensign for Life\"", command.ExecutedArguments[4], "Wrong argument passed at index {0}: ENVVARS aren't processing undefined variables", 4);

								// verify that variable expansion results are processed for special fields
								Assert.AreEqual("\"Archer\"", command.ExecutedArguments[5], "Wrong argument passed at index {0}: ENVVARS should be processed for built-in/DICOM special fields", 5);

								// verify that special field expansion results are **not** processed for environment variables
								Assert.AreEqual('"' + userVars.Format("ARGB") + '"', command.ExecutedArguments[6], "Wrong argument passed at index {0}: Built-in/DICOM special fields should not be processed for ENVVARS", 6);
							}
						}
					}
				}
			}

			Directory.Delete(commandDirectory);
			Directory.Delete(invalidDirectory);
			Directory.Delete(workingDirectory);
		}

		[Test]
		public void TestArguments()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.Arguments = "\"USS Enterprise\" \"USS Defiant\" \"USS Voyager\" \"USS Excelsior\" \"USS Reliant\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					Assert.AreEqual("\"USS Enterprise\"", command.ExecutedArguments[0], "Wrong argument passed at index {0}", 0);
					Assert.AreEqual("\"USS Defiant\"", command.ExecutedArguments[1], "Wrong argument passed at index {0}", 1);
					Assert.AreEqual("\"USS Voyager\"", command.ExecutedArguments[2], "Wrong argument passed at index {0}", 2);
					Assert.AreEqual("\"USS Excelsior\"", command.ExecutedArguments[3], "Wrong argument passed at index {0}", 3);
					Assert.AreEqual("\"USS Reliant\"", command.ExecutedArguments[4], "Wrong argument passed at index {0}", 4);
				}
			}
		}

		[Test]
		public void TestArgumentFields()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.Arguments = "$$ \"$filename$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					Assert.IsFalse(external.CanLaunch(image), "Fields are case sensitive - unresolved field should fail the launch");

					external.Arguments = "$$ \"$FILENAME$\"";

					Assert.IsTrue(external.CanLaunch(image), "Fields are case sensitive - unresolved field should fail the launch");

					external.Launch(image);

					Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
					command.Refresh();

					Trace.WriteLine(string.Format("Command Execution Report"));
					Trace.WriteLine(command.ExecutionReport);

					AssertAreEqualIgnoreCase("$", command.ExecutedArguments[0], "Wrong argument passed at index {0}", 0);

					// these file paths may or may not have spaces in them, but we don't care either way for this test
					AssertAreEqualIgnoreCase(image.Filename, command.ExecutedArguments[1].Trim('"'), "Wrong argument passed at index {0}", 1);
				}
			}
		}

		[Test]
		public void TestSingleImage()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.AllowMultiValueFields = false;
				external.Arguments = "\"$FILENAME$\" \"$DIRECTORY$\" \"$EXTENSIONONLY$\" \"$FILENAMEONLY$\" \"$00100020$\" \"$00100021$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					using (MockDicomPresentationImage otherImage = new MockDicomPresentationImage())
					{
						image[0x00100020].SetStringValue("I've got a lovely bunch of coconuts");
						image[0x00100021].SetStringValue("Here they are all standing in a row");
						otherImage[0x00100020].SetStringValue("Big ones, small ones, some as big as your head");

						Assert.IsFalse(external.CanLaunch(new IPresentationImage[] {image, otherImage}));
						Assert.IsTrue(external.CanLaunch(image));

						external.Launch(image);

						Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
						command.Refresh();

						Trace.WriteLine(string.Format("Command Execution Report"));
						Trace.WriteLine(command.ExecutionReport);

						// these file paths may or may not have spaces in them, but we don't care either way for this test
						AssertAreEqualIgnoreCase(image.Filename, command.ExecutedArguments[0].Trim('"'), "Wrong argument for filename field");
						AssertAreEqualIgnoreCase(Path.GetDirectoryName(image.Filename), command.ExecutedArguments[1].Trim('"'), "Wrong argument for directory field");
						AssertAreEqualIgnoreCase(Path.GetExtension(image.Filename), command.ExecutedArguments[2].Trim('"'), "Wrong argument for extension field");
						AssertAreEqualIgnoreCase(Path.GetFileName(image.Filename), command.ExecutedArguments[3].Trim('"'), "Wrong argument for filename only field");
						Assert.AreEqual("I've got a lovely bunch of coconuts", command.ExecutedArguments[4].Trim('"'), "Wrong argument for 00100020 field");
						Assert.AreEqual("Here they are all standing in a row", command.ExecutedArguments[5].Trim('"'), "Wrong argument for 00100021 field");
					}
				}
			}
		}

		[Test]
		public void TestMultipleImages()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.AllowMultiValueFields = true;
				external.MultiValueFieldSeparator = "\" \"";
				external.Arguments = "\"$FILENAME$\" \"$DIRECTORY$\" \"$EXTENSIONONLY$\" \"$FILENAMEONLY$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					using (MockDicomPresentationImage otherImage = new MockDicomPresentationImage())
					{
						using (MockDicomPresentationImage thirdImage = new MockDicomPresentationImage())
						{
							IPresentationImage[] images = new IPresentationImage[] {image, otherImage, thirdImage};
							Assert.IsTrue(external.CanLaunch(images));
							external.Launch(images);

							Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
							command.Refresh();

							Trace.WriteLine(string.Format("Command Execution Report"));
							Trace.WriteLine(command.ExecutionReport);

							// these file paths may or may not have spaces in them, but we don't care either way for this test
							AssertAreEqualIgnoreCase(image.Filename, command.ExecutedArguments[0].Replace("\"", ""), "Wrong argument for 1st filename field");
							AssertAreEqualIgnoreCase(otherImage.Filename, command.ExecutedArguments[1].Replace("\"", ""), "Wrong argument for 2nd filename field");
							AssertAreEqualIgnoreCase(thirdImage.Filename, command.ExecutedArguments[2].Replace("\"", ""), "Wrong argument for 3rd filename field");
							AssertAreEqualIgnoreCase(Path.GetDirectoryName(image.Filename), command.ExecutedArguments[3].Replace("\"", ""), "Wrong argument for directory field");
							AssertAreEqualIgnoreCase(Path.GetExtension(image.Filename), command.ExecutedArguments[4].Replace("\"", ""), "Wrong argument for extension field");
							AssertAreEqualIgnoreCase(Path.GetFileName(image.Filename), command.ExecutedArguments[5].Replace("\"", ""), "Wrong argument for 1st filename only field");
							AssertAreEqualIgnoreCase(Path.GetFileName(otherImage.Filename), command.ExecutedArguments[6].Replace("\"", ""), "Wrong argument for 2nd filename only field");
							AssertAreEqualIgnoreCase(Path.GetFileName(thirdImage.Filename), command.ExecutedArguments[7].Replace("\"", ""), "Wrong argument for 3rd filename only field");
						}
					}
				}
			}
		}

		[Test]
		public void TestDicomFieldsWithMultipleImages()
		{
			using (MockCommandLine command = new MockCommandLine())
			{
				CommandLineExternal external = new CommandLineExternal();
				external.Command = command.ScriptFilename;
				external.AllowMultiValueFields = true;
				external.MultiValueFieldSeparator = " ";
				external.Arguments = "\"$00100020$\" \"$00100021$\"";

				using (MockDicomPresentationImage image = new MockDicomPresentationImage())
				{
					using (MockDicomPresentationImage otherImage = new MockDicomPresentationImage())
					{
						using (MockDicomPresentationImage thirdImage = new MockDicomPresentationImage())
						{
							using (MockDicomPresentationImage fourthImage = new MockDicomPresentationImage())
							{
								using (MockDicomPresentationImage anotherImage = new MockDicomPresentationImage())
								{
									image[0x00100020].SetStringValue("The cake is a lie");
									otherImage[0x00100020].SetStringValue("The cake is a lie");
									thirdImage[0x00100020].SetStringValue("The cake is a lie");
									fourthImage[0x00100020].SetStringValue("The cake is a lie");
									anotherImage[0x00100020].SetStringValue("The cake is a lie");

									image[0x00100021].SetStringValue("Look, my liege!");
									otherImage[0x00100021].SetStringValue("Camelot!");
									thirdImage[0x00100021].SetStringValue("Camelot!");
									fourthImage[0x00100021].SetStringValue("Camelot!");
									anotherImage[0x00100021].SetStringValue("It's only a model");

									IPresentationImage[] images = new IPresentationImage[] {image, otherImage, thirdImage, fourthImage, anotherImage};
									Assert.IsTrue(external.CanLaunch(images));
									external.Launch(images);

									Thread.Sleep(_processEndWaitDelay); // wait for the external to finish
									command.Refresh();

									Trace.WriteLine(string.Format("Command Execution Report"));
									Trace.WriteLine(command.ExecutionReport);

									// these file paths may or may not have spaces in them, but we don't care either way for this test
									Assert.AreEqual("\"The cake is a lie\"", command.ExecutedArguments[0], "Wrong argument for 00100020 field of first image");
									Assert.AreEqual("\"Look, my liege!\"", command.ExecutedArguments[1], "Wrong argument for 00100021 field of first image");
								}
							}
						}
					}
				}
			}
		}

		private static void AssertAreEqualIgnoreCase(string a, string b, string message, params object[] args)
		{
			if (!string.IsNullOrEmpty(a))
				a = a.ToLowerInvariant();
			if (!string.IsNullOrEmpty(b))
				b = b.ToLowerInvariant();
			Assert.AreEqual(a, b, message, args);
		}
	}
}

#endif