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
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ClearCanvas.Utilities.BuildTasks
{
	/// <summary>
	/// Base class for MSBuild <see cref="Task"/>s that operate on XML documents.
	/// </summary>
	public abstract class XmlTaskBase : Task
	{
		/// <summary>
		/// Initializes an <see cref="XmlTaskBase"/>.
		/// </summary>
		protected XmlTaskBase()
		{
			ErrorIfNotExists = true;
		}

		/// <summary>
		/// Gets or sets the paths to the XML document file(s).
		/// </summary>
		[Required]
		public ITaskItem[] File { get; set; }

		/// <summary>
		/// Gets or sets the XPath that refers to the XML nodes on which the operation will be performed.
		/// </summary>
		[Required]
		public string XPath { get; set; }

		/// <summary>
		/// Gets or sets any XML namespace declarations used in <see cref="XPath"/> as semicolon-separated list of PREFIX=URI declaration pairs.
		/// </summary>
		public string XPathNamespaces { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not it is an error if the nodes selected by <see cref="XPath"/> do not exist. The default value is True.
		/// </summary>
		public bool ErrorIfNotExists { get; set; }

		/// <summary>
		/// Gets the current state of the loaded <see cref="XmlDocument"/>.
		/// </summary>
		/// <remarks>
		/// This property is only available while an <see cref="XmlTaskBase"/> is being executed.
		/// </remarks>
		protected XmlDocument XmlDocument { get; private set; }

		/// <summary>
		/// Gets the declared XML namespaces used in <see cref="XPath"/>.
		/// </summary>
		/// <remarks>
		/// This property is only available while an <see cref="XmlTaskBase"/> is being executed.
		/// </remarks>
		private XmlNamespaceManager XPathNamespaceManager { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not any changes were made to <see cref="XmlDocument"/>.
		/// </summary>
		protected bool Modified { get; set; }

		/// <summary>
		/// Gets a value indicating whether or not it is an error if the XML document is empty or the file does not exist.
		/// </summary>
		protected virtual bool AllowEmptyDocument
		{
			get { return false; }
		}

		/// <summary>
		/// Flags the <see cref="Modified"/> flag if it is not already set.
		/// </summary>
		/// <param name="modified">A value indicating whether or not the flag should be set if it is not already set.</param>
		protected void FlagModified(bool modified)
		{
			Modified = Modified || modified;
		}

		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <returns>True if the task succeeded; False otherwise.</returns>
		public override sealed bool Execute()
		{
			// validate parameters
			string validationMessage;
			if (!ValidateParameters(out validationMessage))
			{
				Log.LogError(validationMessage);
				return false;
			}

			var result = false;
			if (File != null)
			{
				foreach (var file in File)
				{
					if (file != null && ExecuteCore(file.GetMetadata("FullPath")))
						result = true;
				}
			}
			return result;
		}

		private bool ExecuteCore(string filename)
		{
			XmlDocument = new XmlDocument();
			Modified = false;
			try
			{
				if (!string.IsNullOrEmpty(filename) && System.IO.File.Exists(filename))
				{
					// load the file
					try
					{
						XmlDocument.Load(filename);
					}
					catch (XmlException ex)
					{
						Log.LogError("Invalid XML near line {0}, position {1}", ex.LineNumber, ex.LinePosition);
						return false;
					}
				}
				else if (!AllowEmptyDocument)
				{
					// if empty documents are not allowed, error out now
					Log.LogError("File not found: {0}", File);
					return false;
				}

				XPathNamespaceManager = new XmlNamespaceManager(XmlDocument.NameTable);
				if (!string.IsNullOrEmpty(XPathNamespaces))
				{
					foreach (var declaration in XPathNamespaces.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries))
					{
						var index = declaration.IndexOf('=');
						if (index < 0)
						{
							Log.LogError("XML namespace declarations should be formatted as a semicolon-delimited list of PREFIX=URI entries");
							return false;
						}

						var prefix = declaration.Substring(0, index);
						var uri = declaration.Substring(index + 1);
						if (XPathNamespaceManager.HasNamespace(prefix))
						{
							Log.LogError("Duplicate declaration for XML namespace prefix {0}", prefix);
							return false;
						}
						XPathNamespaceManager.AddNamespace(prefix, uri);
					}
				}

				// perform the task
				var result = PerformTask();
				if (result && Modified)
				{
					// save output if task was successful and changes were made
					if (!string.IsNullOrEmpty(filename))
						XmlDocument.Save(filename);
				}
				return result;
			}
#if !DEBUG
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex);
				return false;
			}		
#endif
			finally
			{
				XPathNamespaceManager = null;
				XmlDocument = null;
				Modified = false;
			}
		}

		/// <summary>
		/// Called to validate task parameters before the task is actually executed.
		/// </summary>
		/// <param name="message">A message to be logged if validation did not succeed.</param>
		/// <returns>True if validation succeeded; False otherwise.</returns>
		protected virtual bool ValidateParameters(out string message)
		{
			if (File == null || File.Length == 0)
			{
				message = "File is a required parameter";
				return false;
			}

			if (string.IsNullOrEmpty(XPath))
			{
				message = "XPath is a required parameter";
				return false;
			}

			message = null;
			return true;
		}

		/// <summary>
		/// Called to execute the task.
		/// </summary>
		/// <param name="xmlNodes">A list of <see cref="XmlNode"/>s on which the operation is to be performed.</param>
		/// <returns>True if the task succeeded; False otherwise.</returns>
		protected abstract bool PerformTask(XmlNodeList xmlNodes);

		private bool PerformTask()
		{
			try
			{
				var xmlNodes = XmlDocument.SelectNodes(XPath, XPathNamespaceManager) ?? new EmptyXmlNodeList();
				if (ErrorIfNotExists && xmlNodes.Count == 0)
				{
					Log.LogError("No results for XPath expression {0}", XPath);
					return false;
				}
				return PerformTask(xmlNodes);
			}
			catch (XPathException)
			{
				Log.LogError("Invalid XPath expression {0}", XPath);
				return false;
			}
		}

		/// <summary>
		/// Implementation of an empty <see cref="XmlNodeList"/>.
		/// </summary>
		private class EmptyXmlNodeList : XmlNodeList
		{
			private static readonly object[] _empty = new object[0];

			public override int Count
			{
				get { return 0; }
			}

			public override IEnumerator GetEnumerator()
			{
				return _empty.GetEnumerator();
			}

			public override XmlNode Item(int index)
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
}