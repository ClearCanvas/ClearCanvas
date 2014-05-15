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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="TabComponentContainer"/>.
    /// </summary>
    public class TabPage : ContainerPage
    {
		private readonly string _name;
		
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the page.</param>
        /// <param name="component">The <see cref="IApplicationComponent"/> to be hosted in this page.</param>
        public TabPage([Localizable(true)]string name, IApplicationComponent component)
            :base(component)
        {
			_name = name;
        }

		/// <summary>
		/// Creates a tab page for the specified component, using the last segment of the supplied path as the name.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="component"></param>
		public TabPage(Path path, IApplicationComponent component)
			:this(path.LastSegment.LocalizedText, component)
    	{
    	}

		/// <summary>
		/// Gets the name of the page.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>
		/// Gets the <see cref="Name"/> property.
		/// </summary>
		public override string ToString()
		{
			return this.Name;
		}
    }
}
