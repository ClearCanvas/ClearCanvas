#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Common;

// ReSharper disable LocalizableElement

namespace ClearCanvas.Utilities.Manifest
{
	[ExtensionOf(typeof (ApplicationRootExtensionPoint))]
	internal sealed class ManifestTool : IApplicationRoot
	{
		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Console.WriteLine(@"Manifest Verification Tool");
			Console.WriteLine(@"  Version {0}", ProductInformation.GetVersion(true, true, true));
			Console.WriteLine(@"  {0}", ProductInformation.Copyright);
			Console.WriteLine();
			Console.Write(@"Verifying... ");

			if (!ManifestVerification.Valid)
			{
				Console.WriteLine(@"Failed!");
				Console.WriteLine();
				Console.WriteLine(ManifestVerification.ErrorMessage);
			}
			else
			{
				Console.WriteLine(@"Success!");
			}
		}

		#endregion
	}
}

// ReSharper restore LocalizableElement