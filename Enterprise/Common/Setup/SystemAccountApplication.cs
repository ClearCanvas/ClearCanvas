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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Setup
{
	/// <summary>
	/// Connects to the enterprise server to manage system accounts.
	/// </summary>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class SystemAccountApplication : IApplicationRoot
	{
		public enum Action
		{
			Create,
			Modify,
			Remove
		}


		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			var cmdLine = new SystemAccountCommandLine();
			try
			{
				cmdLine.Parse(args);

				using (new AuthenticationScope(cmdLine.UserName, "SystemAccount Utility", Dns.GetHostName(), cmdLine.Password))
				{
					switch (cmdLine.Action)
					{
						case Action.Create:
							CreateAccount(cmdLine);
							break;
						case Action.Modify:
							ModifyAccount(cmdLine);
							break;
						case Action.Remove:
							RemoveAccount(cmdLine);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private void CreateAccount(SystemAccountCommandLine cmdLine)
		{
			throw new NotImplementedException();
		}

		private void ModifyAccount(SystemAccountCommandLine cmdLine)
		{
			throw new NotImplementedException();
		}

		private void RemoveAccount(SystemAccountCommandLine cmdLine)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
