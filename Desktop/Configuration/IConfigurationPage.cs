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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// An interface defining a configuration page, that is hosted
	/// in a container component along with other such pages
	/// (an example would be a typical Tools/Options dialog).
	/// </summary>
	public interface IConfigurationPage
	{
		/// <summary>
		/// Gets the path to this page.
		/// </summary>
		string GetPath();

		/// <summary>
		/// Gets the <see cref="IApplicationComponent"/> that is hosted in this page.
		/// </summary>
		IApplicationComponent GetComponent();

		/// <summary>
		/// Saves any configuration changes that were made.
		/// </summary>
		void SaveConfiguration();
	}
}
