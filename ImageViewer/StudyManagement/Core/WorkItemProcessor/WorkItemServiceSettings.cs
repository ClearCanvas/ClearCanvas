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

using System.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
{
    /// TODO (CR Jun 2012): Move to Common and/or use the new settings provider?
    // (SW) didn't make it the new settings provider because there'd be no easy way for the user to edit.
    [SettingsGroupDescription("Settings for the WorkItemService.")]
    [SettingsProvider(typeof(LocalFileSettingsProvider))]
    public partial class WorkItemServiceSettings 
	{
	}
}
