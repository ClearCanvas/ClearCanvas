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

using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="NavigatorComponentContainer"/>.
    /// </summary>
    public class NavigatorPage : ContainerPage
    {

        private readonly Path _path;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to this page in the navigation tree.</param>
        /// <param name="component">The application component to be displayed by this page</param>
        public NavigatorPage(string path, IApplicationComponent component)
            :this(new Path(path, new ResourceResolver(new [] { component.GetType().Assembly })), component)
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path">The path to this page in the navigation tree.</param>
		/// <param name="component">The application component to be displayed by this page</param>
		public NavigatorPage(Path path, IApplicationComponent component)
			:base(component)
    	{
			Platform.CheckForNullReference(path, "path");

			_path = path;
    	}

        /// <summary>
        /// Gets the path to this page.
        /// </summary>
        public Path Path
        {
            get { return _path; }
        }

		/// <summary>
		/// Returns <see cref="ClearCanvas.Desktop.Path.ToString"/>.
		/// </summary>
		public override string ToString()
		{
			return this.Path.ToString();
		}
    }
}
