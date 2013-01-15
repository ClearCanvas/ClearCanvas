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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A host for components that are children of other components.
	/// </summary>
    public class ChildComponentHost : ApplicationComponentHost
    {
        private readonly IApplicationComponentHost _parentHost;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parentHost">The object that hosts the <paramref name="childComponent"/>'s parent component.</param>
		/// <param name="childComponent">The child application component being hosted.</param>
        public ChildComponentHost(IApplicationComponentHost parentHost, IApplicationComponent childComponent)
            : base(childComponent)
        {
            Platform.CheckForNullReference(parentHost, "parentHost");

            _parentHost = parentHost;
        }

		/// <summary>
		/// Gets the <see cref="DesktopWindow"/> that owns the parent component.
		/// </summary>
        public override DesktopWindow DesktopWindow
        {
            get { return _parentHost.DesktopWindow; }
        }

		/// <summary>
		/// Gets the title of the parent host.
		/// </summary>
        public override string Title
        {
            get { return _parentHost.Title; }
        }

    }
}
