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
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Externals.General
{
	[ExtensionOf(typeof (ExternalFactoryExtensionPoint))]
	public sealed class CommandLineExternalDefinitionFactory : ExternalFactoryBase<CommandLineExternal>
	{
		public CommandLineExternalDefinitionFactory() : base(SR.DescriptionCommandLineExternal) { }

		public override IExternalPropertiesComponent CreatePropertiesComponent()
		{
			return new CommandLineExternalPropertiesComponent();
		}
	}

	public class CommandLineExternal : ExternalBase
	{
		private string _arguments = string.Empty;
		private string _command = string.Empty;
		private string _workingDirectory = string.Empty;

		private string _username = string.Empty;
		private string _domain = string.Empty;
		private SecureString _password = null;

		private bool _allowMultiValueFields = true;
		private string _multiValueFieldSeparator;

		private bool _waitForExit = false;

		public string Command
		{
			get { return this._command; }
			set
			{
				if (this._command != value)
				{
					this._command = value;
					base.NotifyPropertyChanged("Command");
				}
			}
		}

		public string WorkingDirectory
		{
			get { return this._workingDirectory; }
			set
			{
				if (this._workingDirectory != value)
				{
					this._workingDirectory = value;
					base.NotifyPropertyChanged("WorkingDirectory");
				}
			}
		}

		public string Arguments
		{
			get { return _arguments; }
			set
			{
				if (this._arguments != value)
				{
					this._arguments = value;
					this.NotifyPropertyChanged("Arguments");
				}
			}
		}

		public string Username
		{
			get { return this._username; }
			set
			{
				if (this._username != value)
				{
					this._username = value;
					base.NotifyPropertyChanged("Username");
				}
			}
		}

		public string Domain
		{
			get { return this._domain; }
			set
			{
				if (this._domain != value)
				{
					this._domain = value;
					base.NotifyPropertyChanged("Domain");
				}
			}
		}

		public SecureString SecurePassword
		{
			get { return this._password; }
			set
			{
				if (this._password != value)
				{
					if (this._password != null)
						this._password.Dispose();

					this._password = value;
					base.NotifyPropertyChanged("SecurePassword");
					base.NotifyPropertyChanged("Password");
				}
			}
		}

		public string Password
		{
			get
			{
				if (this._password == null)
					return null;
				StringBuilder sb = new StringBuilder(this._password.Length);
				for (int n = 0; n < this._password.Length; n++)
					sb.Append('*');
				return sb.ToString();
			}
			set
			{
				SecureString newPassword = null;
				if (value != null)
				{
					newPassword = new SecureString();
					foreach (char c in value)
						newPassword.AppendChar(c);
				}
				this.SecurePassword = newPassword;
			}
		}

		public bool AllowMultiValueFields
		{
			get { return this._allowMultiValueFields; }
			set
			{
				if (this._allowMultiValueFields != value)
				{
					this._allowMultiValueFields = value;
					base.NotifyPropertyChanged("AllowMultiValueFields");
				}
			}
		}

		public string MultiValueFieldSeparator
		{
			get { return this._multiValueFieldSeparator; }
			set
			{
				if (this._multiValueFieldSeparator != value)
				{
					this._multiValueFieldSeparator = value;
					base.NotifyPropertyChanged("MultiValueFieldSeparator");
				}
			}
		}

#if UNIT_TESTS

		internal bool WaitForExit
		{
			get { return _waitForExit; }
			set
			{
				if (_waitForExit != value)
				{
					_waitForExit = value;
					this.NotifyPropertyChanged("WaitForExit");
				}
			}
		}

#endif

		public override bool IsValid
		{
			get { return base.IsValid && !string.IsNullOrEmpty(_command); }
		}

		protected override bool CanLaunch(IArgumentHintResolver hintResolver)
		{
			string resolvedArgument = hintResolver.Resolve(this.Arguments, _allowMultiValueFields, " ");
			if (resolvedArgument == null)
				return false;
			return true;
		}

		protected override bool PerformLaunch(IArgumentHintResolver hintResolver)
		{
			string multiValueSeparator = _multiValueFieldSeparator;
			if (string.IsNullOrEmpty(multiValueSeparator))
				multiValueSeparator = " ";

			var command = ExpandEnvironmentVariables(_command);
			var workingDirectory = ExpandEnvironmentVariables(_workingDirectory);
			var arguments = ExpandEnvironmentVariables(_arguments);

			command = hintResolver.Resolve(command, _allowMultiValueFields, multiValueSeparator);
			workingDirectory = hintResolver.Resolve(workingDirectory, _allowMultiValueFields, multiValueSeparator);
			arguments = hintResolver.Resolve(arguments, _allowMultiValueFields, multiValueSeparator);

			ProcessStartInfo nfo;
			if (string.IsNullOrEmpty(arguments))
				nfo = new ProcessStartInfo(command);
			else
				nfo = new ProcessStartInfo(command, arguments);
			if (Directory.Exists(workingDirectory))
				nfo.WorkingDirectory = workingDirectory;

			switch (base.WindowStyle)
			{
				case WindowStyle.Minimized:
					nfo.WindowStyle = ProcessWindowStyle.Minimized;
					break;
				case WindowStyle.Maximized:
					nfo.WindowStyle = ProcessWindowStyle.Maximized;
					break;
				case WindowStyle.Hidden:
					nfo.WindowStyle = ProcessWindowStyle.Hidden;
					break;
				case WindowStyle.Normal:
				default:
					nfo.WindowStyle = ProcessWindowStyle.Normal;
					break;
			}

			if (!string.IsNullOrEmpty(this._username))
			{
				nfo.UserName = this._username;
				nfo.Domain = this._domain;
				nfo.Password = this._password;
			}
			nfo.UseShellExecute = false;

			Process process = new Process();
			process.StartInfo = nfo;

			Platform.Log(LogLevel.Debug, "Command Line Execute: {2}> {0} {1}", nfo.FileName, nfo.Arguments, nfo.WorkingDirectory);

			bool result = process.Start();

			if (_waitForExit)
			{
				process.WaitForExit();
				process.Dispose();
			}

			return result;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(this.Command);
			if (!string.IsNullOrEmpty(this.Arguments))
				sb.AppendFormat(" {0}", this.Arguments);
			return sb.ToString();
		}

		#region Environment Variable Handling

		private static readonly Regex _environmentVariableRegex = new Regex("%(.*?)%", RegexOptions.Compiled);

		/// <summary>
		/// Expands environment variables in the <paramref name="input"/> string.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Replacement only occurs for defined environment variables. For example, suppose <paramref name="input"/> is "MyENV = %MyENV%".
		/// If the environment variable MyENV is set to 42, this method returns "MyENV = 42". If MyENV is not set, no change occurs; "MyENV = %MyENV%"
		/// is returned.
		/// </para>
		/// <para>
		/// This method differs from <see cref="Environment.ExpandEnvironmentVariables"/> in that this method always reads the environment variables
		/// at the time the expansion is requested, rather than using the values of environment variables as they were when the process was started
		/// (which is what the .NET version does). Since the whole point of this <see cref="IExternal"/> is to spawn a new process, we should let it
		/// use the most up-to-date environment variables which is what would happen if the user were to execute the command manually from a command
		/// line.
		/// </para>
		/// </remarks>
		/// <param name="input">A string containing the names of zero or more environment variables. Each environment variable name is quoted with the percent sign character (%).</param>
		/// <returns>A string with each environment variable replaced by its value.</returns>
		private static string ExpandEnvironmentVariables(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;
			return _environmentVariableRegex.Replace(input, m =>
			                                                	{
			                                                		var name = m.Groups[1].Value;
			                                                		if (string.IsNullOrEmpty(name))
			                                                			return "%";
			                                                		var result = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
			                                                		if (result == null)
			                                                			result = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
			                                                		if (result == null)
			                                                			result = m.Groups[0].Value;
			                                                		return result;
			                                                	});
		}

		#endregion

		public string GetState()
		{
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("CommandLineProperties");
			root.SetAttribute("name", base.Name);
			root.SetAttribute("label", base.Label);
			root.SetAttribute("enabled", base.Enabled.ToString());
			root.AppendChild(WriteProperty(doc, "WindowStyle", base.WindowStyle.ToString()));
			root.AppendChild(WriteProperty(doc, "Command", this.Command));
			root.AppendChild(WriteProperty(doc, "WorkingDirectory", this.WorkingDirectory));
			root.AppendChild(WriteProperty(doc, "AllowMultiValueFields", this.AllowMultiValueFields.ToString()));
			root.AppendChild(WriteProperty(doc, "MultiValueFieldSeparator", this.MultiValueFieldSeparator));
			root.AppendChild(WriteProperty(doc, "Username", this.Username));
			root.AppendChild(WriteProperty(doc, "Domain", this.Domain));
			root.AppendChild(WriteProperty(doc, "Password", this.Password));
			root.AppendChild(WriteProperty(doc, "Arguments", this.Arguments));
			doc.AppendChild(root);
			return doc.InnerXml;
		}

		public void SetState(string stateData)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(stateData);

				XmlElement root = null;
				foreach (XmlNode node in doc.ChildNodes)
				{
					if (node is XmlElement && node.Name == "CommandLineProperties")
					{
						root = (XmlElement) node;
						break;
					}
				}
				if (root == null)
					throw new Exception("Root node not found.");

				base.Name = root.GetAttribute("name");
				base.Label = root.GetAttribute("label");
				base.Enabled = bool.Parse(root.GetAttribute("enabled"));
				foreach (XmlNode node in root.ChildNodes)
				{
					if (node is XmlElement)
					{
						string data = node.FirstChild.Value;
						switch (node.Name)
						{
							case "WindowStyle":
								base.WindowStyle = (WindowStyle) Enum.Parse(typeof (WindowStyle), data);
								break;
							case "Command":
								this.Command = data;
								break;
							case "WorkingDirectory":
								this.WorkingDirectory = data;
								break;
							case "Arguments":
								this.Arguments = data;
								break;
							case "AllowMultiValueFields":
								this.AllowMultiValueFields = bool.Parse(data);
								break;
							case "MultiValueFieldSeparator":
								this.MultiValueFieldSeparator = data;
								break;
							case "Username":
								this.Username = data;
								break;
							case "Domain":
								this.Domain = data;
								break;
							case "Password":
								this.Password = data;
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Invalid state data.", "stateData", ex);
			}
		}

		private static XmlNode WriteProperty(XmlDocument parentDocument, string name, string value)
		{
			XmlElement element = parentDocument.CreateElement(name);
			XmlCDataSection cdata = parentDocument.CreateCDataSection(value);
			element.AppendChild(cdata);
			return element;
		}
	}
}