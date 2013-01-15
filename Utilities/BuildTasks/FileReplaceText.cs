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
using System.Text;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	public class FileReplaceText : Task
	{
		private string _filePath;
		private string _textToReplace;
		private string _replacementText;

		public FileReplaceText()
		{
		}

		public string FilePath
		{
			get { return _filePath; }
			set { _filePath = value; }
		}

		public string TextToReplace
		{
			get { return _textToReplace; }
			set { _textToReplace = value; }
		}

		public string ReplacementText
		{
			get { return _replacementText; }
			set { _replacementText = value; }
		}

		public override bool Execute()
		{
			try
			{
				if (!File.Exists(_filePath))
				{
					base.Log.LogMessage(String.Format("File does not exist: {0}", _filePath));
					return false;
				}

				string oldText = "";
				Encoding encoding;
				using (StreamReader reader = new StreamReader(_filePath, true))
				{
					encoding = reader.CurrentEncoding;
					base.Log.LogMessage(String.Format("Detected encoding: {0}", encoding));
					oldText = reader.ReadToEnd();
				}

				string newText = oldText.Replace(_textToReplace, _replacementText);
				if (oldText != newText)
				{
					base.Log.LogMessage(String.Format("Replacing '{0}' with '{1}' in file '{2}'", _textToReplace, _replacementText, _filePath));

					using (StreamWriter writer = new StreamWriter(_filePath, false, encoding))
					{
						writer.Write(newText);
					}
				}
				else
				{
					base.Log.LogMessage(String.Format("The text to replace ('{0}') could not be found in file '{1}'", _textToReplace, _filePath));
				}
			}
			catch(Exception e)
			{
				base.Log.LogErrorFromException(e, true);
				return false;
			}

			return true;
		}
	}
}