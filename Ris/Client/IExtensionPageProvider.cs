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

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines a base interface for providing extension pages to the application.
	/// </summary>
	/// <typeparam name="TPage">The interface to the extension page.</typeparam>
	/// <typeparam name="TContext">The interface to the context which is passed to the extension page.</typeparam>
	public interface IExtensionPageProvider<TPage, TContext>
		where TPage : IExtensionPage
	{
		TPage[] GetPages(TContext context);
	}

	/// <summary>
	/// Defines a base interface to an extension page.
	/// </summary>
	public interface IExtensionPage
	{
		/// <summary>
		/// Gets the path to the extension page.  The meaning of this path depends upon the type of container
		/// in which the page is displayed.
		/// </summary>
		Path Path { get; }

		/// <summary>
		/// Gets the application component that implements the extension page functionality.  This method
		/// will be called exactly once during the lifetime of the page.
		/// </summary>
		/// <returns></returns>
		IApplicationComponent GetComponent();
	}

}
