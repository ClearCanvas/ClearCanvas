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

namespace ClearCanvas.Enterprise.Core.Printing
{
	/// <summary>
	/// Defines a model for a page to be printed.
	/// </summary>
	/// <remarks>
	/// A page model consists of an HTML template, and a dictionary of variables to be passed into the template
	/// to produce the final HTML stream to be printed.
	/// </remarks>
	public interface IPageModel
	{
		/// <summary>
		/// Gets the URL of the template.
		/// </summary>
		Uri TemplateUrl { get; }

		/// <summary>
		/// Gets the set of variables accessible to the template.
		/// </summary>
		Dictionary<string, object> Variables { get; } 
	}
}
