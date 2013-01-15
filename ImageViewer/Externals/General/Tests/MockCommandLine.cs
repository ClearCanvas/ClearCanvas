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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageViewer.Externals.General.Tests
{
	public class MockCommandLine : IDisposable
	{
		private string _scriptFile;
		private string _scriptContent;
		private string _outputFile;
		private string _outputContent;

		private ArgumentList _executedArguments;
		private string _executedCommand;
		private string _executedWorkingDirectory;

		public MockCommandLine()
			: this(Environment.CurrentDirectory) {}

		public MockCommandLine(string scriptDirectory)
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows)
			{
				_scriptFile = Path.Combine(scriptDirectory, "command_line_external_test_payload.cmd");
				_outputFile = Path.Combine(Environment.CurrentDirectory, string.Format("command_line_external_test_output.{0}.out", Guid.NewGuid()));
				_scriptContent = string.Concat(
					"@echo off\r\n",
					string.Format("echo // {1} Command Execution Report > {0}\r\n", _outputFile, this.GetType().FullName),
					string.Format("echo // Platform {1} >> {0}\r\n", _outputFile, Environment.OSVersion),
					string.Format("echo CMD %0 >> {0}\r\n", _outputFile),
					string.Format("echo PWD %CD% >> {0}\r\n", _outputFile),
					string.Format("echo ARG[0] %1 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[1] %2 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[2] %3 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[3] %4 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[4] %5 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[5] %6 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[6] %7 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[7] %8 >> {0}\r\n", _outputFile),
					string.Format("echo ARG[8] %9 >> {0}\r\n", _outputFile),
					"\r\n");
			}
			else
			{
				throw new NotSupportedException(string.Format("No mock command available for the current platform ({0}).", Environment.OSVersion));
			}

			Trace.WriteLine(string.Format("Creating a payload command at {0} for output to {1}", _scriptFile, _outputFile));
			File.WriteAllText(_scriptFile, _scriptContent);
		}

		public void Dispose()
		{
			_scriptContent = null;

			if (_scriptFile != null && File.Exists(_scriptFile))
			{
				File.Delete(_scriptFile);
				_scriptFile = null;
			}

			if (_outputFile != null && File.Exists(_outputFile))
			{
				File.Delete(_outputFile);
				_outputFile = null;
			}
		}

		public string ScriptFilename
		{
			get { return _scriptFile; }
		}

		public IArguments ExecutedArguments
		{
			get { return _executedArguments; }
		}

		public string ExecutedCommand
		{
			get { return _executedCommand; }
		}

		public string ExecutedWorkingDirectory
		{
			get { return _executedWorkingDirectory; }
		}

		public string ExecutionReport
		{
			get { return _outputContent; }
		}

		public void Refresh()
		{
			if (File.Exists(_outputFile))
			{
				_outputContent = File.ReadAllText(_outputFile);
				_executedArguments = new ArgumentList();

				Regex pattern = new Regex(@"^(\w+)(?:\[(\d+)\])?\s(.*)\s*", RegexOptions.Compiled);
				foreach (string line in _outputContent.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
				{
					Match match = pattern.Match(line);
					if (!match.Success)
						continue;

					if (match.Groups[1].Value == "CMD")
						_executedCommand = match.Groups[3].Value.Trim().Trim('"');
					else if (match.Groups[1].Value == "PWD")
						_executedWorkingDirectory = match.Groups[3].Value.Trim();
					else if (match.Groups[1].Value == "ARG")
					{
						int index = int.Parse(match.Groups[2].Value);
						string value = match.Groups[3].Value.Trim();
						if (string.IsNullOrEmpty(value))
							value = null;
						_executedArguments[index] = value;
					}
				}
			}
		}

		public interface IArguments
		{
			string this[int index] { get; }
		}

		private class ArgumentList : IArguments
		{
			private readonly List<string> _list = new List<string>();

			public string this[int index]
			{
				get
				{
					if (index < 0 || index >= _list.Count)
						return null;
					return _list[index];
				}
				set
				{
					if (index >= _list.Count)
						_list.AddRange(new string[index - _list.Count + 1]);
					_list[index] = value;
				}
			}
		}
	}
}

#endif