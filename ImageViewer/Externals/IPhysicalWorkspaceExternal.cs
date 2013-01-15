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

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Externals
{
	public interface IPresentationImageExternal : IExternal
	{
		bool CanLaunch(IPresentationImage image);
		bool Launch(IPresentationImage image);
	}

	public interface IDisplaySetExternal : IExternal
	{
		bool CanLaunch(IEnumerable<IPresentationImage> images);
		bool Launch(IEnumerable<IPresentationImage> images);

		bool CanLaunch(IDisplaySet displaySet);
		bool Launch(IDisplaySet displaySet);
	}

	public interface IImageSetExternal : IExternal
	{
		bool CanLaunch(IEnumerable<IDisplaySet> displaySets);
		bool Launch(IEnumerable<IDisplaySet> displaySets);

		bool CanLaunch(IImageSet imageSet);
		bool Launch(IImageSet imageSet);
	}

	public interface IPhysicalWorkspaceExternal : IExternal
	{
		bool CanLaunch(IEnumerable<IImageSet> imageSets);
		bool Launch(IEnumerable<IImageSet> imageSets);

		bool CanLaunch(IPhysicalWorkspace physicalWorkspace);
		bool Launch(IPhysicalWorkspace physicalWorkspace);
	}
}