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
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Defines the interface of a extension to <see cref="StudyEditor"/>
	/// </summary>
	public interface IWebEditStudyProcessorExtension : IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether the extension is enabled.
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// Initializes the extension.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Called when study is about to be updated.
		/// </summary>
		/// <param name="context"></param>
		void OnStudyEditing(WebEditStudyContext context);

		/// <summary>
		/// Called after the study has been updated.
		/// </summary>
		/// <param name="context"></param>
		void OnStudyEdited(WebEditStudyContext context);
	}

	public class WebEditStudyProcessorExtensionPoint:ExtensionPoint<IWebEditStudyProcessorExtension>
	{}
}